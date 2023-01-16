using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace SuperHeroAPI.Models
{
    public class Employee
    {

        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Place { get; set; } = string.Empty;
        public string Adress { get; set; } = string.Empty;
        public double NetoSalary { get; set; }
        public double GrossSalary { get; set; }
        public string Position { get; set; } = string.Empty;
    }
}
