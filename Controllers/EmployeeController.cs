using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using System.IO;

public class EmployeeController : Controller
{
    private readonly EmployeeService _empservice;
    private readonly IWebHostEnvironment _env;

    public EmployeeController(EmployeeService emp, IWebHostEnvironment env)
    {
        _empservice = emp;
        _env = env;
    }

    [HttpGet]
    public IActionResult Submit()
    {
        return View();
    }

    [HttpPost]
    [HttpPost]
public IActionResult Submit(Employee emp, IFormFile img)
{
    // Basic validation
    if (!ModelState.IsValid || img == null)
    {
        ModelState.AddModelError("", "Please upload a valid image.");
        return View(emp);
    }

    // Check file size (limit to 2MB)
    if (img.Length > 2 * 1024 * 1024)
    {
        ModelState.AddModelError("", "Image must be under 2MB.");
        return View(emp);
    }

    // Check file extension (allow only jpeg and png)
    var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
    var fileExtension = Path.GetExtension(img.FileName).ToLower();
    if (!allowedExtensions.Contains(fileExtension))
    {
        ModelState.AddModelError("", "Invalid Image: File must be JPEG or PNG.");
        return View(emp);
    }

    // Save image to the "Uploads" folder
    var uploadFolder = Path.Combine(_env.WebRootPath, "Uploads");
    if (!Directory.Exists(uploadFolder))
    {
        Directory.CreateDirectory(uploadFolder);
    }

    // Generate a unique file name
    var fileName = emp.EmpId + "_" + emp.Name + fileExtension;
    var filePath = Path.Combine(uploadFolder, fileName);

    // Save the file to the server
    using (var stream = new FileStream(filePath, FileMode.Create))
    {
        img.CopyTo(stream);
    }

    // Save the image path in the Employee model (relative to wwwroot)
    emp.ImagePath = "/Uploads/" + fileName;

    // Add the employee to the database
    _empservice.Add(emp);

    ViewBag.Message = "File submitted successfully!";
    return View();
}

    [HttpGet]
    public IActionResult Retrieve()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Retrieve(int empId)
    {
        var res = _empservice.GetById(empId);
        if (res == null)
        {
            ViewBag.Message = "Employee not found";
            return View("NotFound");
        }

        return View(res);
    }
}
