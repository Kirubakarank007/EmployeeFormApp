using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;

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
    public IActionResult Submit(Employee emp, IFormFile img)
    {
        if (!ModelState.IsValid || img == null || img.Length > 2 * 1024 * 1024 ||(img.ContentType != "image/jpeg" && img.ContentType != "image/png"))
        {
            ModelState.AddModelError("","Invalid Images");
            return View(emp);
        }

        //save image to "Upload" folder
        var uploadFolder=Path.Combine(_env.WebRootPath,"Uploads");
        if(!Directory.Exists(uploadFolder))
        {
            Directory.CreateDirectory(uploadFolder);
        }


        var fileName=Path.GetDirectoryName(img.FileName);
        if(fileName is null){
            fileName = emp.EmpId+emp.Name;
        }
        var filePath = Path.Combine(uploadFolder,fileName);
        using (var stream=new FileStream(filePath,FileMode.Create)){
            img.CopyTo(stream);
        }
        emp.ImagePath="Uploads"+fileName;
        _empservice.Add(emp);

        ViewBag.Message="File submitted successfully";
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
        if(res is null)
        {
            ViewBag.Message = "Employee not found";
            return NotFound();
        }
        return View(res);
    }
    
}