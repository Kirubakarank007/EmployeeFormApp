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
    public IActionResult AddEmployee()
    {
        return View();
    }

    [HttpPost]
    [HttpPost]
public IActionResult AddEmployee(Employee emp, IFormFile img)
{
        ModelState.Clear();
        var existing = _empservice.GetById(emp.EmpId);
        Console.WriteLine(existing);
        if (existing != null)
        {
            if (existing.EmpId == emp.EmpId)
            {
                ViewBag.Message = $"Employee with ID {emp.EmpId} already exists.";
                return View(emp);
            }
        }
        var existingByEmail = _empservice.GetByEmail(emp.EmailAddress);
        if (existingByEmail != null)
        {
            ViewBag.Message = $"Employee with Email Address {emp.EmailAddress} already exists.";
            return View(emp);
        }
    // if (existing.EmailAddress == emp.EmailAddress)
        // {
        //     ViewBag.Message = $"Employee with Email Address {emp.EmailAddress} already exists.";
        //     return View(emp);
        // }
        // Basic validation
        if (!ModelState.IsValid || img == null)
        {
            // ModelState.AddModelError("", "");
            ViewBag.Message = "Please upload a valid image.";

            return View(emp);
        }

    // Check file size (limit to 2MB)
    if (img.Length > 2 * 1024 * 1024)
    {
        // ModelState.AddModelError("", "");
        ViewBag.Message = "Image must be under 2MB.";

        return View(emp);
    }

    // Check file extension (allow only jpeg and png)
    var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
    var fileExtension = Path.GetExtension(img.FileName).ToLower();
    if (!allowedExtensions.Contains(fileExtension))
    {
        ViewBag.Message = "Invalid Image: File must be JPEG or PNG.";

        return View();
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
    public IActionResult SearchEmployee()
    {
        return View();
    }

    [HttpPost]
    public IActionResult SearchEmployee(int empId)
    {
        var res = _empservice.GetById(empId);
        if (res == null)
        {
            ViewBag.Message = "Employee not found";
            return View("NotFound");
        }

        return View(res);
    }

    [HttpGet]
    public IActionResult AllEmployees()
    {
        var employees = _empservice.GetAll(); 
        return View(employees);
    }

    [HttpPost]
    public IActionResult Delete(int empId)
    {
        var employee = _empservice.GetById(empId);
        if (employee == null)
        {
            return NotFound();
        }
        if (!string.IsNullOrEmpty(employee.ImagePath))
        {
            var imagePath = Path.Combine(_env.WebRootPath, employee.ImagePath.TrimStart('/'));
            if (System.IO.File.Exists(imagePath))
            {
                System.IO.File.Delete(imagePath);
            }
        }
        _empservice.Delete(empId);
        return RedirectToAction("AllEmployees");
    }

}
