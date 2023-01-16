namespace SuperHeroAPI.Response
{
    public class EmployeeResponse
    {


        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Place { get; set; } = string.Empty;
        public string Adress { get; set; } = string.Empty;
        public string Position { get; set; } = string.Empty;
        public double NetoSalary { get; set; }
        public double NetoSalaryEUR { get; set; }
        public double NetoSalaryUSD { get; set; }

        public double Tax { get; set; }
        public double Pio { get; set; }
        public double Health { get; set; }
        public double Unemployment { get; set; }
        public double GrossSalary { get; set; }
        public double GrossSalaryEUR { get; set; }
        public double GrossSalaryUSD { get; set; }




    }
}
