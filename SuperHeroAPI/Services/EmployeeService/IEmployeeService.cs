using Microsoft.AspNetCore.Mvc;
using SuperHeroAPI.Models;
using SuperHeroAPI.Response;

namespace SuperHeroAPI.Services.EmployeeService
{
    public interface IEmployeeService
    {

        double Exchange(string value1, string value2);

        double SumFees(Employee employee);
        double Pio(Employee employee);
        double Health(Employee employee);
        double Unemployment(Employee employee);
        double Tax(Employee employee);
        


        Task<List<Employee>> AddEmployee(Employee employee);

        Task<List<Employee>> GetAllEmployees();

        Task<EmployeeResponse?> GetSingleEmployee(int id);

    }
}
