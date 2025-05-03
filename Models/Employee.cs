using System.ComponentModel.DataAnnotations;

public class Employeee{
    [Required]
    public int id {get;set;}

    [Required]
    public string? Name {get;set;}

    [Required]
    public int EmpId {get;set;}

    [Required]
    [EmailAddress]
    public string? EmailAddress{get;set;}

    public string? ImagePath {get;set;}
}