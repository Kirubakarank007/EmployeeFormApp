using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

public class EmployeeService{

    private readonly AppDbContext _context;

    public EmployeeService(AppDbContext context)
    {
        _context=context;
    }

   public void Add(Employee emp)
   {
        _context.Employees.Add(emp);
        _context.SaveChanges();
   }

    public Employee GetById(int id){
        var res= _context.Employees.FirstOrDefault(e => e.EmpId == id);
        return res!;
    }

    public List<Employee> GetAll()
    {
        return _context.Employees.ToList();
    }

}