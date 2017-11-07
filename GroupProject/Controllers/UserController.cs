using GroupProject.Constants;
using GroupProject.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;

namespace GroupProject.Controllers
{
    [Authorize(AuthenticationSchemes = "Fitness247UserAuthentication")]
    public class UserController:Controller
    {
        private DB_Context db_context;

        public UserController(DB_Context db_context)
        {
            this.db_context = db_context;
        }

        [AllowAnonymous]
        public IActionResult Index()
        {
            User loggedInuser = db_context.Users.Where(x => x.Email == HttpContext.User.Identity.Name || x.UserName == HttpContext.User.Identity.Name).ToList().First();
            ViewBag.User = loggedInuser;
            return View();
        }

        [AllowAnonymous]
        public IActionResult Login(string requestPath)
        {
            ViewBag.RequestPath = requestPath ?? "/user";
            return Index();
        }
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (!IsAuthentic(model.Username, model.Password))
                return View();

            // create claims
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, model.Username)
            };

            // create identity
            ClaimsIdentity identity = new ClaimsIdentity(claims, "cookie");

            // create principal
            ClaimsPrincipal principal = new ClaimsPrincipal(identity);

            // sign-in
            await HttpContext.SignInAsync(
                    scheme: "Fitness247UserAuthentication",
                    principal: principal,
                    properties: new AuthenticationProperties
                    {
                        IsPersistent = model.RememberMe, // for 'remember me' feature
                        ExpiresUtc = DateTime.UtcNow.AddMinutes(30)
                    });

            return Redirect(model.RequestPath ?? "/user");
        }
        public async Task<IActionResult> Logout(string requestPath)
        {
            await HttpContext.SignOutAsync(
                    scheme: "Fitness247UserAuthentication");

            return Redirect("/");
        }

        private bool IsAuthentic(string username, string password)
        {
            int count = db_context.Users.Count(x => x._UserRole < UserRoles.Instructor && (x.Email == username || x.UserName == username) && x.Password == password);
            if (count == 1)
                return true;
            return false;
        }

        public PartialViewResult BookingsCalendar()
        {
            List<Class> classes = db_context.Classes.AsNoTracking().ToList<Class>();
            ViewBag.Classes = classes;
            return PartialView();
        }

        public PartialViewResult MyReservations()
        {
            return PartialView();
        }

        public PartialViewResult MyPurchases()
        {
            return PartialView();
        }

        [HttpPost]
        public JsonResult GetClasses()
        {
            var searchParam = HttpContext.Request.Form["search[value]"];
            var start = HttpContext.Request.Form["start"][0];
            var length = HttpContext.Request.Form["length"][0];
            var totalCount = db_context.Classes.Count();
            var filterCount = db_context.Classes.Count();
            List<dynamic> filteredClasses = db_context.Classes.Include(c => c.ClassType).ThenInclude(c => c.Location).Include(r => r.ClassSchedule).AsNoTracking().Skip(int.Parse(start)).Take(int.Parse(length)).ToList<dynamic>();

            if (searchParam[0].Trim() != "")
            {
                filteredClasses = db_context.Classes.Include(c => c.ClassType).ThenInclude(c => c.Location).Include(r => r.ClassSchedule).AsNoTracking().Where(x => x.ClassType.ClassName.Contains(searchParam[0]) || x.ClassSchedule.ScheduleName.Contains(searchParam[0])).Skip(int.Parse(start)).Take(int.Parse(length)).ToList<dynamic>();
                filterCount = filteredClasses.Count();
            }

            dynamic data = new
            {
                draw = HttpContext.Request.Form["draw"],
                recordsTotal = totalCount,
                recordsFiltered = filterCount,
                data = filteredClasses
            };
            return Json(data);
        }
    }
}
