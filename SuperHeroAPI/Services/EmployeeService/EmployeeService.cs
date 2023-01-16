using Microsoft.AspNetCore.Mvc;
using SuperHeroAPI.Enums;
using SuperHeroAPI.Response;

namespace SuperHeroAPI.Services.EmployeeService
{
    public class EmployeeService : IEmployeeService
    {
        // properties
        private readonly DataContext _context;
        public FeesEnum _feesEnum;


        //constructor
        public EmployeeService(DataContext context, FeesEnum feesEnum)
        {
            _context = context;
            _feesEnum = feesEnum;
        }



        public async Task<List<Employee>> AddEmployee(Employee employee)
        {
            var fees = SumFees(employee);

            var gross = employee.NetoSalary + fees;

            employee.GrossSalary = (int)gross;

            _context.Employees.Add(employee);

            await _context.SaveChangesAsync();

            return await _context.Employees.ToListAsync();
        }


        public async Task<List<Employee>> GetAllEmployees()
        {
            return await _context.Employees.ToListAsync();
        }

       
        public async Task<EmployeeResponse?> GetSingleEmployee(int id)
        {
            var employee = await _context.Employees.FindAsync(id);

            if (employee == null)
            {
                return null;
            }

            var data = new EmployeeResponse
            {
                Id = employee.Id,
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                Place = employee.Place,
                Adress = employee.Adress,
                NetoSalary = employee.NetoSalary,
                NetoSalaryEUR = Math.Round(employee.NetoSalary * Exchange("RSD", "EUR"), 1),
                NetoSalaryUSD = Math.Round(employee.NetoSalary * Exchange("RSD", "USD"), 1),

                GrossSalary = employee.GrossSalary,
                GrossSalaryEUR = Math.Round(employee.GrossSalary * Exchange("RSD", "EUR"), 1),
                GrossSalaryUSD = Math.Round(employee.GrossSalary * Exchange("RSD", "USD"), 1),
                Position = employee.Position,
                Pio = Math.Round(Pio(employee), 1),
                Health = Health(employee),
                Unemployment = Unemployment(employee),
                Tax = Tax(employee)
            };

            return data;
        }

        public double Exchange(string value1, string value2)
        {
            API_Obj data = Rates.Import(value1);

            if (value2 == "EUR")
            {
                return data.conversion_rates.EUR;

            }

            return data.conversion_rates.USD;

        }





        public double SumFees(Employee employee)
        {
            var pio = employee.NetoSalary * _feesEnum.Pio;
            var health = employee.NetoSalary * _feesEnum.Health;
            var unemployment = employee.NetoSalary * _feesEnum.Unemployment;
            var tax = employee.NetoSalary * _feesEnum.Tax;

            var fees = pio + health + unemployment + tax;

            return fees;
        }

        public double Pio(Employee employee)
        {
            return employee.NetoSalary * _feesEnum.Pio;
        }

        public double Health(Employee employee)
        {
            return employee.NetoSalary * _feesEnum.Health;
        }

        public double Unemployment(Employee employee)
        {
            return employee.NetoSalary * _feesEnum.Unemployment;
        }

        public double Tax(Employee employee)
        {
            return employee.NetoSalary * _feesEnum.Tax;
        }



    }
}
