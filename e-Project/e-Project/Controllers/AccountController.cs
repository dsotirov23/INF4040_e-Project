using Microsoft.AspNetCore.Mvc;


public class AccountController : Controller
{
    private readonly AppDbContext _context;

    public AccountController(AppDbContext context)
    {
        _context = context;
    }

    public IActionResult Register() => View();

    [HttpPost]
    public IActionResult Register(e_Project.Models.User user)
    {

        if (_context.Users.Any(u => u.Email == user.Email))
        {
            ModelState.AddModelError("Email", "This email already exists in the system.");
        }

        bool hasLetter = false, hasNum = false, hasDot = false;
        if (!string.IsNullOrEmpty(user.Password) && user.Password.Length >= 8)
        {
            foreach (char c in user.Password)
            {
                if (char.IsLetter(c)) hasLetter = true;
                else if (char.IsDigit(c)) hasNum = true;
                else if (c == '.') hasDot = true;
            }
        }

        if (!hasLetter || !hasNum || !hasDot)
        {
            ModelState.AddModelError("Password", "Password must be 8+ characters with letters, numbers, and dots.");
        }

        if (!string.IsNullOrEmpty(user.Phone))
        {
            foreach (char c in user.Phone)
            {
                if (!char.IsDigit(c))
                {
                    ModelState.AddModelError("Phone", "Phone number must contain only numbers.");
                    break;
                }
            }
        }
        else
        {
            ModelState.AddModelError("Phone", "Phone number is required.");
        }

        if (ModelState.IsValid)
        {
            _context.Users.Add(user);
            _context.SaveChanges();
            return RedirectToAction("Login");
        }

        return View(user);
    }

    public IActionResult Login() => View();

    [HttpPost]
    public IActionResult Login(string email, string password)
    {
        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            ViewBag.Error = "Some of the two fields is missing";
            return View();
        }

        var user = _context.Users.FirstOrDefault(u => u.Email == email);

        if (user == null)
        {
            ViewBag.Error = "Such an email does not exist in the system";
            return View();
        }

        if (user.Password != password)
        {
            ViewBag.Error = "Password is wrong";
            return View();
        }

        HttpContext.Session.SetString("UserEmail", user.Email);
        return RedirectToAction("Index", "Home");
    }

    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Index", "Home");
    }
}