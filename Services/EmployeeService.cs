using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

public class EmployeeService{

    public readonly List<Employee> _employees=new();

   public void Add(Employee emp){
    
    _employees.Add(emp);
    Console.WriteLine(emp);
   }

    public Employee GetById(int id){
        
        var res= _employees.FirstOrDefault(e => e.EmpId == id);
    
        return res!;
    }
}