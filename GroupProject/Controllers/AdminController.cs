using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using GroupProject.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using GroupProject.Constants;

namespace GroupProject.Controllers
{
    [Authorize(AuthenticationSchemes = "Fitness247AdminAuthentication")]
    public class AdminController : Controller
    {
        private DB_Context db_context;
        
        public AdminController(DB_Context db_context)
        {
            this.db_context = db_context;
        }

        public IActionResult Index()
        {
            User loggedInuser = db_context.Users.Single(x => x.Email == HttpContext.User.Identity.Name);
            ViewBag.User = loggedInuser;
            return View();
        }
        [AllowAnonymous]
        public IActionResult Login(string requestPath)
        {
            ViewBag.RequestPath = requestPath ?? "/admin";
            return View();
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
                    scheme: "Fitness247AdminAuthentication",
                    principal: principal,
                    properties: new AuthenticationProperties
                    {
                        IsPersistent = model.RememberMe, // for 'remember me' feature
                        ExpiresUtc = DateTime.UtcNow.AddMinutes(30)
                    });

            return Redirect(model.RequestPath ?? "/admin");
        }
        public async Task<IActionResult> Logout(string requestPath)
        {
            await HttpContext.SignOutAsync(
                    scheme: "Fitness247AdminAuthentication");

            return RedirectToAction("Login");
        }

        public IActionResult Update(DB_Context db_context)
        {
            User person = db_context.Users.Single(x => x.FirstName == "Pranav") as User;
            person.Password = "123456";
            db_context.Update<User>(person);
            db_context.SaveChanges();
            return RedirectToAction("Index");
        }
        public IActionResult Add(DB_Context db_Context)
        {
            Instructor instructor = new Instructor() { FirstName = "Rushiraaj", LastName = "Jadeja", Email = "rushi@hotmail.com", Password = "qwerty", _UserRole = 49,UserName="rushi",_Gender=1,AddressLine1="Apt 528, 1907 Deerpark Drive",City="Fullerton",State="CA",Country="US",Pincode=92831,DateOfBirth=DateTime.Parse("1991-12-27"),Designation="Instructor",Bio="abc",Description="",HourlyRate=10.50 };
            db_Context.Add<Instructor>(instructor);
            db_Context.SaveChanges();
            return RedirectToAction("Index");
        }

        public PartialViewResult Dashboard()
        {
            return PartialView();
        }
        public PartialViewResult Reservations()
        {
            return PartialView();
        }

        public PartialViewResult Schedules()
        {
            return PartialView();
        }

        public PartialViewResult Members()
        {
            return PartialView();
        }
        public PartialViewResult Configure()
        {
            return PartialView();
        }
        private bool IsAuthentic(string username, string password)
        {
            int count = db_context.Users.Count(x => x._UserRole >= UserRoles.Instructor && x.Email == username && x.Password == password);
            if (count == 1)
                return true;
            return false;
        }

        #region Configure
        #region Instructors
        public PartialViewResult Instructors()
        {
            return PartialView();
        }

        public JsonResult GetInstructors()
        {
            
            var searchParam = HttpContext.Request.Query["search[value]"];
            List<Instructor> filteredInstructor = db_context.Instructors.ToList();
            
            string firstName, lastName;
            if (searchParam.Count>=1)
            {
                string[] keywords = searchParam[0].Split(" ");
                if(keywords.Length>1)
                {
                    firstName = keywords[0];
                    lastName = keywords[1];
                }
                else
                {
                    firstName = searchParam[0];
                    lastName = searchParam[0];
                }

                if (firstName != lastName)
                    filteredInstructor = db_context.Instructors.Where(x => (x.FirstName.Contains(firstName) && x.LastName.Contains(lastName)) || x.Email.Contains(searchParam)).ToList();
                else
                    filteredInstructor = db_context.Instructors.Where(x => x.FirstName.Contains(firstName) || x.LastName.Contains(lastName) || x.Email.Contains(searchParam)).ToList();
            }
            filteredInstructor.ForEach(x => x.Password = "");
            dynamic data = new
            {
                draw = HttpContext.Request.Query["draw"],
                recordsTotal = db_context.Instructors.Count(),
                recordsFiltered = filteredInstructor.Count,
                data=filteredInstructor
            };
            return Json(data);


        }

        public PartialViewResult AddInstructor()
        {
            return PartialView();
        }
        [HttpPost]
        public JsonResult AddInstructor(Instructor Instructor)
        {
            Instructor._UserRole = 49;
            Instructor.Password = "qwerty123";
            db_context.Instructors.Add(Instructor);
            db_context.SaveChanges();
            return Json(true);
        }

        #endregion
        #endregion
    }
}