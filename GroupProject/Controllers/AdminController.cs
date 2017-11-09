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
using Microsoft.EntityFrameworkCore;

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
            User loggedInuser = db_context.Users.Where(x => x.Email == HttpContext.User.Identity.Name|| x.UserName==HttpContext.User.Identity.Name).ToList().First();
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
            Instructor instructor = new Instructor() { FirstName = "Rushiraaj", LastName = "Jadeja", Email = "rushi@hotmail.com", Password = "qwerty", _UserRole = 49, UserName = "rushi", _Gender = 1, AddressLine1 = "Apt 528, 1907 Deerpark Drive", City = "Fullerton", State = "CA", Country = "US", Pincode = 92831, DateOfBirth = DateTime.Parse("1991-12-27"), Designation = "Instructor", Bio = "abc", Description = "", HourlyRate = 10.50 };
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
            int count = db_context.Users.Count(x => x._UserRole >= UserRoles.Instructor && (x.Email == username||x.UserName==username) && x.Password == password);
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
        [HttpPost]
        public JsonResult GetInstructors()
        {
            User loggedInuser = db_context.Users.Where(x => x.Email == HttpContext.User.Identity.Name || x.UserName == HttpContext.User.Identity.Name).ToList().First();
            var searchParam = HttpContext.Request.Form["search[value]"];
            var start = HttpContext.Request.Form["start"][0];
            var length = HttpContext.Request.Form["length"][0];
            var totalCount = db_context.Instructors.Count();
            var filterCount = db_context.Instructors.Count();
            List<dynamic> filteredInstructor = db_context.Instructors.AsNoTracking().Where(x=>x._UserRole<=loggedInuser._UserRole).Skip(int.Parse(start)).Take(int.Parse(length)).Select(x => new { x.UserID, x.FirstName, x.LastName, x.Email, x.Bio }).ToList<dynamic>();

            string firstName, lastName;
            if (searchParam[0].Trim() != "")
            {
                string[] keywords = searchParam[0].Split(" ");
                if (keywords.Length > 1)
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
                    filteredInstructor = db_context.Instructors.AsNoTracking().Where(x => ((x.FirstName.Contains(firstName) && x.LastName.Contains(lastName)) || x.Email.Contains(searchParam)) && x._UserRole<=loggedInuser._UserRole).Skip(int.Parse(start)).Take(int.Parse(length)).Select(x => new { x.UserID, x.FirstName, x.LastName, x.Email, x.Bio }).ToList<dynamic>();
                else
                    filteredInstructor = db_context.Instructors.AsNoTracking().Where(x => (x.FirstName.Contains(firstName) || x.LastName.Contains(lastName) || x.Email.Contains(searchParam)) && x._UserRole<=loggedInuser._UserRole).Skip(int.Parse(start)).Take(int.Parse(length)).Select(x => new { x.UserID, x.FirstName, x.LastName, x.Email, x.Bio }).ToList<dynamic>();

                filterCount = filteredInstructor.Count();
            }
            //zfilteredInstructor.ForEach(x => x.Password = "");

            dynamic data = new
            {
                draw = HttpContext.Request.Form["draw"],
                recordsTotal = totalCount,
                recordsFiltered = filterCount,
                data = filteredInstructor
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

        public async Task<PartialViewResult> UpdateInstructor(int instructorID)
        {
            Instructor model = await db_context.Instructors.AsNoTracking().SingleAsync(x => x.UserID == instructorID);
            return PartialView(model);
        }

        [HttpPost]
        public JsonResult UpdateInstructor(Instructor Instructor)
        {
            Instructor model = db_context.Instructors.Single(x => x.UserID == Instructor.UserID);
            Instructor.Password = model.Password;
            Instructor._UserRole = model._UserRole;
            Instructor.ActivatedFlag = model.ActivatedFlag;
            db_context.Entry(model).CurrentValues.SetValues(Instructor);
            int result = db_context.SaveChanges();
            return Json(true);
        }

        #endregion

        #region Locations
        public PartialViewResult Locations()
        {
            return PartialView();
        }
        [HttpPost]
        public JsonResult GetLocations()
        {
            
            var searchParam = HttpContext.Request.Form["search[value]"];
            var start = HttpContext.Request.Form["start"][0];
            var length = HttpContext.Request.Form["length"][0];
            var totalCount = db_context.Locations.Count();
            var filterCount = db_context.Locations.Count();
            List<dynamic> filteredLocations = db_context.Locations.AsNoTracking().Skip(int.Parse(start)).Take(int.Parse(length)).ToList<dynamic>();

            if (searchParam[0].Trim() != "")
            {
                filteredLocations = db_context.Locations.AsNoTracking().Where(x=>x.LocationName== searchParam[0] || x.City== searchParam[0] || x.AddressLine1== searchParam[0] || x.AddressLine2== searchParam[0]).Skip(int.Parse(start)).Take(int.Parse(length)).ToList<dynamic>();
                filterCount = filteredLocations.Count();
            }

            dynamic data = new
            {
                draw = HttpContext.Request.Form["draw"],
                recordsTotal = totalCount,
                recordsFiltered = filterCount,
                data = filteredLocations
            };
            return Json(data);
        }

        public PartialViewResult AddLocation()
        {
            return PartialView();
        }
        [HttpPost]
        public JsonResult AddLocation(Location Location)
        {
            db_context.Locations.Add(Location);
            db_context.SaveChanges();
            return Json(true);
        }
        public async Task<PartialViewResult> UpdateLocation(int locationID)
        {
            Location model = await db_context.Locations.AsNoTracking().SingleAsync(x => x.LocationID == locationID);
            return PartialView(model);
        }

        [HttpPost]
        public JsonResult UpdateLocation(Location Location)
        {
            Location model = db_context.Locations.Single(x => x.LocationID == Location.LocationID);
            db_context.Entry(model).CurrentValues.SetValues(Location);
            int result = db_context.SaveChanges();
            return Json(true);
        }
        #endregion

        #region Class Packages
        public PartialViewResult ClassPackages()
        {
            return PartialView();
        }
        [HttpPost]
        public JsonResult GetClassPackages()
        {
            var searchParam = HttpContext.Request.Form["search[value]"];
            var start = HttpContext.Request.Form["start"][0];
            var length = HttpContext.Request.Form["length"][0];
            var totalCount = db_context.ClassPackages.Count();
            var filterCount = db_context.ClassPackages.Count();
            List<dynamic> filteredClassPackages = db_context.ClassPackages.AsNoTracking().Skip(int.Parse(start)).Take(int.Parse(length)).ToList<dynamic>();

            if (searchParam[0].Trim() != "")
            {
                filteredClassPackages = db_context.ClassPackages.AsNoTracking().Where(x => x.ClassPackageName.Contains(searchParam[0])||x.ClassPackageDescription.Contains(searchParam[0])||x.Badge.Contains(searchParam[0])).Skip(int.Parse(start)).Take(int.Parse(length)).ToList<dynamic>();
                filterCount = filteredClassPackages.Count();
            }

            dynamic data = new
            {
                draw = HttpContext.Request.Form["draw"],
                recordsTotal = totalCount,
                recordsFiltered = filterCount,
                data = filteredClassPackages
            };
            return Json(data);
        }
        public PartialViewResult AddClassPackage()
        {
            return PartialView();
        }
        [HttpPost]
        public JsonResult AddClassPackage(ClassPackage ClassPackage)
        {
            db_context.ClassPackages.Add(ClassPackage);
            db_context.SaveChanges();
            return Json(true);
        }
        public async Task<PartialViewResult> UpdateClassPackage(int classPackageID)
        {
            ClassPackage model = await db_context.ClassPackages.AsNoTracking().SingleAsync(x => x.ClassPackageID == classPackageID);
            return PartialView(model);
        }

        [HttpPost]
        public JsonResult UpdateClassPackage(ClassPackage ClassPackage)
        {
            ClassPackage model = db_context.ClassPackages.Single(x => x.ClassPackageID == ClassPackage.ClassPackageID);
            db_context.Entry(model).CurrentValues.SetValues(ClassPackage);
            int result = db_context.SaveChanges();
            return Json(true);
        }
        #endregion

        #region Class Types
        public PartialViewResult ClassTypes()
        {
            return PartialView();
        }

        [HttpPost]
        public JsonResult GetClassTypes()
        {
            var searchParam = HttpContext.Request.Form["search[value]"];
            var start = HttpContext.Request.Form["start"][0];
            var length = HttpContext.Request.Form["length"][0];
            var totalCount = db_context.ClassTypes.Count();
            var filterCount = db_context.ClassTypes.Count();
            List<dynamic> filteredClassTypes = db_context.ClassTypes.Include(c=>c.Location).Include(c=>c.Instructor).AsNoTracking().Skip(int.Parse(start)).Take(int.Parse(length)).ToList<dynamic>();

            if (searchParam[0].Trim() != "")
            {
                filteredClassTypes = db_context.ClassTypes.Include(c => c.Location).Include(c => c.Instructor).AsNoTracking().Where(x => x.ClassName.Contains(searchParam[0]) || x.ClassDescription.Contains(searchParam[0]) || x.Location.LocationName.Contains(searchParam[0]) || x.Instructor.FirstName.Contains(searchParam[0]) || x.Instructor.LastName.Contains(searchParam[0])).Skip(int.Parse(start)).Take(int.Parse(length)).ToList<dynamic>();
                filterCount = filteredClassTypes.Count();
            }

            dynamic data = new
            {
                draw = HttpContext.Request.Form["draw"],
                recordsTotal = totalCount,
                recordsFiltered = filterCount,
                data = filteredClassTypes
            };
            return Json(data);
        }

        public PartialViewResult AddClassType()
        {
            ViewBag.Locations= db_context.Locations.AsNoTracking().ToList<Location>();
            ViewBag.Instructors= db_context.Instructors.AsNoTracking().ToList<Instructor>();
            return PartialView();
        }

        [HttpPost]
        public JsonResult AddClassType(ClassType ClassType)
        {
            db_context.ClassTypes.Add(ClassType);
            db_context.SaveChanges();
            return Json(true);
        }

        public async Task<PartialViewResult> UpdateClassType(int ClassTypeID)
        {
            ClassType model = await db_context.ClassTypes.AsNoTracking().SingleAsync(x => x.ClassTypeID == ClassTypeID);
            ViewBag.Locations = db_context.Locations.AsNoTracking().ToList<Location>();
            ViewBag.Instructors = db_context.Instructors.AsNoTracking().ToList<Instructor>();
            return PartialView(model);
        }

        [HttpPost]
        public JsonResult UpdateClassType(ClassType ClassType)
        {
            ClassType model = db_context.ClassTypes.Single(x => x.ClassTypeID == ClassType.ClassTypeID);
            db_context.Entry(model).CurrentValues.SetValues(ClassType);
            int result = db_context.SaveChanges();
            return Json(true);
        }
        #endregion

        #region Room Layout
        public PartialViewResult RoomLayouts()
        {
            return PartialView();
        }

        [HttpPost]
        public JsonResult GetRoomLayouts()
        {
            var searchParam = HttpContext.Request.Form["search[value]"];
            var start = HttpContext.Request.Form["start"][0];
            var length = HttpContext.Request.Form["length"][0];
            var totalCount = db_context.RoomLayouts.Count();
            var filterCount = db_context.RoomLayouts.Count();
            List<dynamic> filteredRoomLayouts = db_context.RoomLayouts.Include(c => c.Location).AsNoTracking().Skip(int.Parse(start)).Take(int.Parse(length)).ToList<dynamic>();

            if (searchParam[0].Trim() != "")
            {
                filteredRoomLayouts = db_context.RoomLayouts.Include(c => c.Location).AsNoTracking().Where(x => x.RoomName.Contains(searchParam[0]) || x.Location.LocationName.Contains(searchParam[0])).Skip(int.Parse(start)).Take(int.Parse(length)).ToList<dynamic>();
                filterCount = filteredRoomLayouts.Count();
            }

            dynamic data = new
            {
                draw = HttpContext.Request.Form["draw"],
                recordsTotal = totalCount,
                recordsFiltered = filterCount,
                data = filteredRoomLayouts
            };
            return Json(data);
        }

        public PartialViewResult AddRoomLayout()
        {
            ViewBag.Locations = db_context.Locations.AsNoTracking().ToList<Location>();
            return PartialView();
        }

        [HttpPost]
        public JsonResult AddRoomLayout(RoomLayout RoomLayout)
        {
            db_context.RoomLayouts.Add(RoomLayout);
            db_context.SaveChanges();
            return Json(true);
        }

        public async Task<PartialViewResult> UpdateRoomLayout(int RoomLayoutID)
        {
            RoomLayout model = await db_context.RoomLayouts.AsNoTracking().SingleAsync(x => x.RoomLayoutID == RoomLayoutID);
            ViewBag.Locations = db_context.Locations.AsNoTracking().ToList<Location>();
            return PartialView(model);
        }

        [HttpPost]
        public JsonResult UpdateRoomLayout(RoomLayout RoomLayout)
        {
            RoomLayout model = db_context.RoomLayouts.Single(x => x.RoomLayoutID == RoomLayout.RoomLayoutID);
            db_context.Entry(model).CurrentValues.SetValues(RoomLayout);
            int result = db_context.SaveChanges();
            return Json(true);
        }
        #endregion

        #region Class Schedules
        public PartialViewResult ClassSchedules()
        {
            return PartialView();
        }
        [HttpPost]
        public JsonResult GetClassSchedules()
        {
            var searchParam = HttpContext.Request.Form["search[value]"];
            var start = HttpContext.Request.Form["start"][0];
            var length = HttpContext.Request.Form["length"][0];
            var totalCount = db_context.ClassSchedules.Count();
            var filterCount = db_context.ClassSchedules.Count();
            List<dynamic> filteredClassSchedules = db_context.ClassSchedules.Include(c => c.ClassType).ThenInclude(c=>c.Location).Include(c=>c.ClassType).ThenInclude(c=>c.Instructor).Include(r=>r.RoomLayout).AsNoTracking().Skip(int.Parse(start)).Take(int.Parse(length)).ToList<dynamic>();
            
            if (searchParam[0].Trim() != "")
            {
                filteredClassSchedules = db_context.ClassSchedules.Include(c => c.ClassType).ThenInclude(c=>c.Location).Include(c => c.ClassType).ThenInclude(c => c.Instructor).Include(r => r.RoomLayout).AsNoTracking().Where(x => x.ClassType.ClassName.Contains(searchParam[0]) || x.RoomLayout.RoomName.Contains(searchParam[0])).Skip(int.Parse(start)).Take(int.Parse(length)).ToList<dynamic>();
                filterCount = filteredClassSchedules.Count();
            }

            dynamic data = new
            {
                draw = HttpContext.Request.Form["draw"],
                recordsTotal = totalCount,
                recordsFiltered = filterCount,
                data = filteredClassSchedules
            };
            return Json(data);
        }

        public PartialViewResult AddClassSchedule()
        {
            ViewBag.RoomLayouts = db_context.RoomLayouts.AsNoTracking().ToList<RoomLayout>();
            ViewBag.ClassTypes = db_context.ClassTypes.AsNoTracking().ToList<ClassType>();
            return PartialView();
        }

        [HttpPost]
        public JsonResult AddClassSchedule(ClassSchedule ClassSchedule)
        {
            db_context.ClassSchedules.Add(ClassSchedule);

            for (DateTime date = ClassSchedule.StartDate; date.Date <= ClassSchedule.EndDate.Date; date = date.AddDays(1))
            {
                if (ClassSchedule.Days.Contains(date.ToString("ddd")))
                {
                    Class c = new Class()
                    {
                        ClassScheduleID = ClassSchedule.ClassScheduleID,
                        ClassTypeID = ClassSchedule.ClassTypeID,
                        StartDate = ClassSchedule.StartDate,
                        EndDate = ClassSchedule.EndDate,
                        StartTime = ClassSchedule.StartTime,
                        EndTime = ClassSchedule.EndTime,
                        AllowWaitlist = true,
                        IsCancelled = false
                    };
                    db_context.Classes.Add(c);
                }
            }

            db_context.SaveChanges();
            return Json(true);
        }

        public async Task<PartialViewResult> UpdateClassSchedule(int ClassScheduleID)
        {
            ClassSchedule model = await db_context.ClassSchedules.AsNoTracking().SingleAsync(x => x.ClassScheduleID == ClassScheduleID);
            ViewBag.RoomLayouts = db_context.RoomLayouts.AsNoTracking().ToList<RoomLayout>();
            ViewBag.ClassTypes = db_context.ClassTypes.AsNoTracking().ToList<ClassType>();
            return PartialView(model);
        }

        [HttpPost]
        public JsonResult UpdateClassSchedule(ClassSchedule ClassSchedule)
        {
            ClassSchedule model = db_context.ClassSchedules.Single(x => x.ClassScheduleID == ClassSchedule.ClassScheduleID);
            db_context.Entry(model).CurrentValues.SetValues(ClassSchedule);
            int result = db_context.SaveChanges();
            return Json(true);
        }
        #endregion

        #endregion
    }
}