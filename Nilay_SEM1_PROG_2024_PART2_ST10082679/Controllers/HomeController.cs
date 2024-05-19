using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Nilay_SEM1_PROG_2024_PART2_ST10082679.Context;
using Nilay_SEM1_PROG_2024_PART2_ST10082679.Models;
using Nilay_SEM1_PROG_2024_PART2_ST10082679.Helpers;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Diagnostics.Eventing.Reader;
using Microsoft.AspNetCore.Http;

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
                    // check role access
                    var userIsEmployee = await dbContext.Users.Where(s => s.UserId == Int32.Parse(userId) && s.Role.Equals("employee")).FirstOrDefaultAsync();
                    if (userIsEmployee == null)
                    {
                        return RedirectToAction("FarmerDashboard");
                    }

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
               
                using (AgriDbContext dbContext = new AgriDbContext())
                {
                    // check role access
                    var userIsFarmer = await dbContext.Users.Where(s => s.UserId == Int32.Parse(userId) && s.Role.Equals("farmer")).FirstOrDefaultAsync();
                    if (userIsFarmer == null)
                    {
                        return RedirectToAction("EmployeeDashboard");
                    }

                    var products = await dbContext.Products.Where(s => s.UserId == Int32.Parse(userId)).ToListAsync();

                    return View(products);
                }          
                
            }
            else
            {
                return RedirectToAction("LoginEmployee");
            }
        }
        //--------------------------------------------------------------------------------------//
        public async Task<IActionResult> EmployeeFarmerView(int id)
        {
            //gets userId from the session 
            var userId = HttpContext.Session.GetString("UserId");
            HttpContext.Session.SetString("FarmerUserId", id.ToString());
            ViewBag.Categories = ProductCategories.items;

            var category = HttpContext.Session.GetString("Category");
            var startDate = HttpContext.Session.GetString("StartDate");
            var endDate = HttpContext.Session.GetString("EndDate");

            //checks if user is logged in
            if (userId != null)
            {

                using (AgriDbContext dbContext = new AgriDbContext())
                {

                    // check role access
                    var userIsEmployee = await dbContext.Users.Where(s => s.UserId == Int32.Parse(userId) && s.Role.Equals("employee")).FirstOrDefaultAsync();
                    if (userIsEmployee == null)
                    {
                        return RedirectToAction("FarmerDashboard");
                    }

                    var query = dbContext.Products.Where(s => s.UserId == id);

                    if (category != null)
                    {
                        query = query.Where(s => s.Category.Equals(category));
                    }

                    if (startDate != null && endDate != null)
                    {
                        var parsedStartDate = DateTime.Parse(startDate);
                        var parsedEndDate = DateTime.Parse(endDate);

                        if (parsedStartDate < parsedEndDate)
                        {
                            query = query.Where(s => s.ProductDate > parsedStartDate && s.ProductDate < parsedEndDate);
                        }
                    }

                    var products = await query.ToListAsync();
                    return View(products);

                }

            }
            else
            {
                return RedirectToAction("LoginEmployee");
            }
        }

        public IActionResult FilterTable(string Category, DateTime StartDate, DateTime EndDate)
        {
            //gets semesterId from session
            var farmerUserId = HttpContext.Session.GetString("FarmerUserId");
            ViewBag.Categories = ProductCategories.items;
            //sets the week numer in the session

            if (Category != null)
            {
                HttpContext.Session.SetString("Category", Category);
            } else
            {
                HttpContext.Session.Remove("Category");
            }

            if (StartDate.Year >= 1731)
            {
                HttpContext.Session.SetString("StartDate", StartDate.ToString());
            }
            else
            {
                HttpContext.Session.Remove("StartDate");
            }

            if (EndDate.Year >= 1731)
            {
                HttpContext.Session.SetString("EndDate", EndDate.ToString());
            }
            else
            {
                HttpContext.Session.Remove("EndDate");
            }

            //goes to the semester dashboard with updated semesterId
            return RedirectToAction("EmployeeFarmerView", new { Id = farmerUserId });
        }

        //--------------------------------------------------------------------------------------//
        public async Task<IActionResult> AddFarmer()
        {
            //gets userId from the session 
            var userId = HttpContext.Session.GetString("UserId");

            using (AgriDbContext dbContext = new AgriDbContext())
            {
                // check role access
                var userIsEmployee = await dbContext.Users.Where(s => s.UserId == Int32.Parse(userId) && s.Role.Equals("employee")).FirstOrDefaultAsync();
                if (userIsEmployee == null)
                {
                    return RedirectToAction("FarmerDashboard");
                }
                //checks if user is logged in
                if (userId != null)
                {
                    return View();
                }
                else
                {
                    return RedirectToAction("LoginEmployee");
                }
            }
        }

        public async Task<IActionResult> AddProduct()
        {
            //gets userId from the session 
            var userId = HttpContext.Session.GetString("UserId");

            ViewBag.Categories = ProductCategories.items;
            using (AgriDbContext dbContext = new AgriDbContext())
            {
                // check role access
                var userIsFarmer = await dbContext.Users.Where(s => s.UserId == Int32.Parse(userId) && s.Role.Equals("farmer")).FirstOrDefaultAsync();
                if (userIsFarmer == null)
                {
                    return RedirectToAction("EmployeeDashboard");
                }

                //checks if user is logged in
                if (userId != null)
                {
                    return View();
                }
                else
                {
                    return RedirectToAction("LoginEmployee");
                }
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

        //--------------------------------------------------------------------------------------//
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddProduct(Product productObj)
        {
            // Error handling to make sure the fields are filled out    
            var userId = HttpContext.Session.GetString("UserId");

            if (string.IsNullOrEmpty(productObj.Name) || string.IsNullOrEmpty(productObj.Category) || userId == null || productObj.ProductDate.Year <= 1731)
            {
                ViewBag.Message = "Please make sure both fields are filled out";
                ViewBag.Categories = ProductCategories.items;
                return View(productObj);
            }


            //intercats with database using dbcontext
            using (AgriDbContext dbContext = new AgriDbContext())
            {

                productObj.UserId = Int32.Parse(userId);
                //adds to database
                await dbContext.Products.AddAsync(productObj);
                await dbContext.SaveChangesAsync();

                return RedirectToAction("FarmerDashboard");
                
            }

        }
        //--------------------------------------------------------------------------------------//
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
