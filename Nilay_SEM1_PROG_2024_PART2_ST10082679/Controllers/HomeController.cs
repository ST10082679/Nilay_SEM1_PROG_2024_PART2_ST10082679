using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Nilay_SEM1_PROG_2024_PART2_ST10082679.Context;
using Nilay_SEM1_PROG_2024_PART2_ST10082679.Models;
using Nilay_SEM1_PROG_2024_PART2_ST10082679.Helpers;
using System.Diagnostics;

namespace Nilay_SEM1_PROG_2024_PART2_ST10082679.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public async Task<IActionResult> EmployeeDashboard()
        {
            //gets userId from the session 
            var userId = HttpContext.Session.GetString("UserId");

            //checks if user is logged in
            if (userId != null)
            {
             
                using (AgriDbContext dbContext = new AgriDbContext())
                {
                var farmers = await dbContext.Users.Where(s => s.Role.Equals("farmer")).ToListAsync();

                return View(farmers);
                }            
            }
            else
            {
                return RedirectToAction("LoginEmployee");
            }
        }

        public async Task<IActionResult> FarmerDashboard()
        {
            //gets userId from the session 
            var userId = HttpContext.Session.GetString("UserId");

            //checks if user is logged in
            if (userId != null)
            {
                //gets a list of the semesters associated with the userId 
                //using (DBContext dbContext = new DBContext())
                //{
                //var semesters = await dbContext.Semesters.Where(s => s.UserId == Int32.Parse(userId)).ToListAsync();

                //return View(semesters);
                //}
                return View();
            }
            else
            {
                return RedirectToAction("LoginEmployee");
            }
        }

        public async Task<IActionResult> AddFarmer()
        {
            //gets userId from the session 
            var userId = HttpContext.Session.GetString("UserId");

            //checks if user is logged in
            if (userId != null)
            {
                //gets a list of the semesters associated with the userId 
                //using (DBContext dbContext = new DBContext())
                //{
                //var semesters = await dbContext.Semesters.Where(s => s.UserId == Int32.Parse(userId)).ToListAsync();

                //return View(semesters);
                //}
                return View();
            }
            else
            {
                return RedirectToAction("LoginEmployee");
            }
        }
        public IActionResult Register()
        {
            //if user is not logged in they can go to the register view 
            if (HttpContext.Session.GetString("UserId") == null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("EmployeeDashboard");
            }
        }

        public IActionResult LoginEmployee()
        {
            if (HttpContext.Session.GetString("UserId") == null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("EmployeeDashboard");
            }
        }

        public IActionResult LoginFarmer()
        {
            if (HttpContext.Session.GetString("UserId") == null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("FarmerDashboard");
            }
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("LoginEmployee");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(User userObj)
        {
            // Error handling to make sure the fields are filled out 
            if (string.IsNullOrEmpty(userObj.Email) || string.IsNullOrEmpty(userObj.Password) || string.IsNullOrEmpty(userObj.Name))
            {
                ViewBag.Message = "Please make sure both fields are filled out";
                return View(userObj);
            }
            //intercats with database using dbcontext
            using (AgriDbContext dbContext = new AgriDbContext())
            {
                PasswordHasher hasher = new PasswordHasher();
                // gets user from the databse established on student number provided
                var user = await dbContext.Users.Where(u => u.Email.Equals(userObj.Email) && u.Role.Equals("employee")).FirstOrDefaultAsync();

                if (user == null)
                {
                    //generates the salt
                    userObj.Salt = hasher.GenerateSalt();
                    //hashes password
                    userObj.Password = hasher.HashPassword(userObj.Password, userObj.Salt);
                    userObj.Role = "employee";
                    //adds to database
                    await dbContext.Users.AddAsync(userObj);
                    await dbContext.SaveChangesAsync();

                    return RedirectToAction("LoginEmployee");
                }
                else
                {
                    ViewBag.Message = "User already registered. Login instead";
                }
            }

            return View(userObj);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddFarmer(User userObj)
        {
            // Error handling to make sure the fields are filled out 
            if (string.IsNullOrEmpty(userObj.Email) || string.IsNullOrEmpty(userObj.Password) || string.IsNullOrEmpty(userObj.Name))
            {
                ViewBag.Message = "Please make sure both fields are filled out";
                return View(userObj);
            }
            //intercats with database using dbcontext
            using (AgriDbContext dbContext = new AgriDbContext())
            {
                PasswordHasher hasher = new PasswordHasher();
                // gets user from the databse established on student number provided
                var user = await dbContext.Users.Where(u => u.Email.Equals(userObj.Email) && u.Role.Equals("farmer")).FirstOrDefaultAsync();

                if (user == null)
                {
                    //generates the salt
                    userObj.Salt = hasher.GenerateSalt();
                    //hashes password
                    userObj.Password = hasher.HashPassword(userObj.Password, userObj.Salt);
                    userObj.Role = "farmer";
                    //adds to database
                    await dbContext.Users.AddAsync(userObj);
                    await dbContext.SaveChangesAsync();

                    return RedirectToAction("EmployeeDashboard");
                }
                else
                {
                    ViewBag.Message = "Farmer already registered. Login instead";
                }
            }

            return View(userObj);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> LoginEmployee(User userObj)
        {
            //This makes sure that the student number and password fields are filled out 
            if (string.IsNullOrEmpty(userObj.Email) || string.IsNullOrEmpty(userObj.Password))
            {
                ViewBag.Message = "Please make sure both fields are filled out!";
                return View(userObj);
            }
            //interacts with database using dbcontect
            using (AgriDbContext dbContext = new AgriDbContext())
            {
                PasswordHasher hasher = new PasswordHasher();
                //gets user from the database that correspnds with student number
                var user = await dbContext.Users.Where(u => u.Email.Equals(userObj.Email) && u.Role.Equals("employee")).FirstOrDefaultAsync();

                if (user != null)
                {
                    //verifies password with hashed password
                    if (hasher.Verifypassword(userObj.Password.ToString(), user.Password, user.Salt))
                    {
                        //sets userId in the session
                        HttpContext.Session.SetString("UserId", user.UserId.ToString());
                        //once user is logged in this will go to the index page 
                        return RedirectToAction("EmployeeDashboard");
                    }
                    else
                    {
                        //error shown if student number and password does not match 
                        ViewBag.Message = "Student number and password does not match";
                    }
                }
                else
                {
                    //error shown if user is not registered 
                    ViewBag.Message = "User not registered";
                }
            }

            return View(userObj);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> LoginFarmer(User userObj)
        {
            //This makes sure that the student number and password fields are filled out 
            if (string.IsNullOrEmpty(userObj.Email) || string.IsNullOrEmpty(userObj.Password))
            {
                ViewBag.Message = "Please make sure both fields are filled out!";
                return View(userObj);
            }
            //interacts with database using dbcontect
            using (AgriDbContext dbContext = new AgriDbContext())
            {
                PasswordHasher hasher = new PasswordHasher();
                //gets user from the database that correspnds with student number
                var user = await dbContext.Users.Where(u => u.Email.Equals(userObj.Email) && u.Role.Equals("farmer")).FirstOrDefaultAsync();

                if (user != null)
                {
                    //verifies password with hashed password
                    if (hasher.Verifypassword(userObj.Password.ToString(), user.Password, user.Salt))
                    {
                        //sets userId in the session
                        HttpContext.Session.SetString("UserId", user.UserId.ToString());
                        //once user is logged in this will go to the index page 
                        return RedirectToAction("FarmerDashboard");
                    }
                    else
                    {
                        //error shown if student number and password does not match 
                        ViewBag.Message = "Student number and password does not match";
                    }
                }
                else
                {
                    //error shown if user is not registered 
                    ViewBag.Message = "User not registered";
                }
            }

            return View(userObj);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
