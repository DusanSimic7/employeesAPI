using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc;
using SuperHeroAPI.Services.EmployeeService;
using System.Text;
using PdfSharpCore;
using PdfSharpCore.Pdf;
using TheArtOfDev.HtmlRenderer.PdfSharp;
using SuperHeroAPI.Enums;
using DocumentFormat.OpenXml.Drawing.Charts;
using System.Runtime.Serialization.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;

namespace SuperHeroAPI.Controllers
{


    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {

        // properties
        private readonly IEmployeeService _employeeService;
        public FeesEnum _feesEnum;



        // constructor
        public EmployeeController(IEmployeeService employeeService, FeesEnum feesEnum)
        {
            _employeeService = employeeService;
            _feesEnum = feesEnum;

        }


        [HttpPost]
        public async Task<ActionResult<List<Employee>>> AddEmployee(Employee employee)
        {
            return Ok(await _employeeService.AddEmployee(employee));    
        }



        [HttpGet]
        public async Task<ActionResult<List<Employee>>> GetAllEmployees()
        {
           

                return Ok(await _employeeService.GetAllEmployees());
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<Employee>> GetSingleEmployee(int id)
        {
   
           return Ok(await _employeeService.GetSingleEmployee(id));
        }




        [HttpGet]
        [Route("export-csv")]
        public async Task<ActionResult> ExportToCsv()
        {
           
            var builder = new StringBuilder();
            builder.AppendLine("Id, FirstName, LastName, Place, Adress, NetoSalary, GrossSalary, Position");

            foreach (var employee in await _employeeService.GetAllEmployees())
            {
                builder.AppendLine($"{employee.Id}, {employee.FirstName}, {employee.LastName}, {employee.Place}, {employee.Adress}, {employee.NetoSalary}, {employee.GrossSalary}, {employee.Position}");
            }
            return File(Encoding.UTF8.GetBytes(builder.ToString()), "text/csv", "employees.csv");
        }


        [HttpGet]
        [Route("export-xlsx")]
        public async Task<ActionResult> ExportToXlsx()
        {

            var employees = await _employeeService.GetAllEmployees();
            string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            string fileName = "employees.xlsx";
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Employees");
                worksheet.Cell(1, 1).Value = "Id";
                worksheet.Cell(1, 2).Value = "FirstName";
                worksheet.Cell(1, 3).Value = "LastName";
                worksheet.Cell(1, 4).Value = "Place";
                worksheet.Cell(1, 5).Value = "Adress";
                worksheet.Cell(1, 6).Value = "NetoSalary";
                worksheet.Cell(1, 7).Value = "GrossSalary";
                worksheet.Cell(1, 8).Value = "Position";

                for (int index = 1; index <= employees.Count; index++)
                {
                    worksheet.Cell(index + 1, 1).Value = employees[index - 1].Id;
                    worksheet.Cell(index + 1, 2).Value = employees[index - 1].FirstName;
                    worksheet.Cell(index + 1, 3).Value = employees[index - 1].LastName;
                    worksheet.Cell(index + 1, 4).Value = employees[index - 1].Place;
                    worksheet.Cell(index + 1, 5).Value = employees[index - 1].Adress;
                    worksheet.Cell(index + 1, 6).Value = employees[index - 1].NetoSalary;
                    worksheet.Cell(index + 1, 7).Value = employees[index - 1].GrossSalary;
                    worksheet.Cell(index + 1, 8).Value = employees[index - 1].Position;
                }

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    return File(content, contentType, fileName);
                }

            }

        }

        [HttpGet]
        [Route("export-PDF/{id}")]
        public async Task<ActionResult> ExportToPdf(int id)
        {
            var employee = await _employeeService.GetSingleEmployee(id);

            var sumFees = employee.Tax + employee.Pio + employee.Health + employee.Unemployment;

            var document = new PdfDocument();

            
           string HtmlContent = "<div style='width:100%; text-align:center;'>"; // opened div

             
                HtmlContent += "<h2> "+employee.FirstName+ " " +employee.LastName+" payroll</h2>";
            
                HtmlContent += "<table style='width:100%; border: 1px solid #000'>"; // opened table

                HtmlContent += "<thead style='front-weight:bold'>";

                    HtmlContent += "<tr><th>First name</th><th>Last name</th><th>Place</th><th>Address</th><th>Position</th></tr>";

                    HtmlContent += "<tr>";


                      HtmlContent += "<td style='border:1px solid #000'> "+employee.FirstName+" </td>";
                      HtmlContent += "<td style='border:1px solid #000'> "+employee.LastName+" </td>";
                      HtmlContent += "<td style='border:1px solid #000'> "+employee.Place+" </td>";
                      HtmlContent += "<td style='border:1px solid #000'> "+employee.Adress+" </td>";
                      HtmlContent += "<td style='border:1px solid #000'> "+employee.Position+" </td>";

                   HtmlContent += "</tr>";


               HtmlContent += "</thead>";



            HtmlContent += "</table>"; // closed table



            HtmlContent += "<h2 style='text-align:left;margin-top:25px'>Salary calculation</h2>";

            HtmlContent += "<h4 style='text-align:left;margin-top:20px'>Neto salary in RSD: "+employee.NetoSalary+"</h4>";
            HtmlContent += "<h4 style='text-align:left;'>Neto salary in EUR: "+ Math.Round(employee.NetoSalary* _employeeService.Exchange("RSD", "EUR"), 1) + "</h4>";
            HtmlContent += "<h4 style='text-align:left;'>Neto salary in RSD: "+ Math.Round(employee.NetoSalary * _employeeService.Exchange("RSD", "USD"), 1) + "</h4>";

            HtmlContent += "<table style='width:30%;'>"; // opened table

                HtmlContent += "<tr><th style='text-align:left;border:1px solid #000'>TAX</th><th>"+ Math.Round(employee.NetoSalary * _feesEnum.Tax, 1)+ "rsd</th> <th style='border:1px solid #000'>Rate</th><th>10.00</th></tr>";
                HtmlContent += "<tr><th style='text-align:left;border:1px solid #000'>PIO</th><th>"+ Math.Round(employee.NetoSalary * _feesEnum.Pio, 1) + "rsd</th> <th style='border:1px solid #000'>Rate</th><th>14.00</th></tr>";
                HtmlContent += "<tr><th style='text-align:left;border:1px solid #000'>Health</th><th>"+ Math.Round(employee.NetoSalary * _feesEnum.Health, 1)+ "rsd</th> <th style='border:1px solid #000'>Rate</th><th>5.15</th></tr>";
                HtmlContent += "<tr><th style='text-align:left;border:1px solid #000'>Unemployment</th><th>"+ Math.Round(employee.NetoSalary * _feesEnum.Unemployment, 1)+ "rsd</th> <th style='border:1px solid #000'>Rate</th><th>0.75</th></tr>";
                HtmlContent += "<tr><th style='text-align:left;border:1px solid #000'>Sum</th><th>" + Math.Round(sumFees, 1)+ "rsd</th> <th style='border:1px solid #000'>Sum</th><th>29.9</th></tr>";

            HtmlContent += "</table>"; // closed table

            HtmlContent += "<h4 style='text-align:left;margin-top:10px'>Gross salary: " + employee.GrossSalary + "rsd</h4>";



            HtmlContent += "<table style='width:30%;margin-top:20px'>"; // opened table

                HtmlContent += "<tr style='margin-top:20px'><th style='text-align:left;border:1px solid red'>TAX</th><th>" + Math.Round(employee.NetoSalary * _feesEnum.Tax * _employeeService.Exchange("RSD", "EUR"), 1) + "eur</th> <th style='border:1px solid #000'>Rate</th><th>10.00</th></tr>";
                HtmlContent += "<tr><th style='text-align:left;border:1px solid red'>PIO</th><th>" + Math.Round(employee.NetoSalary * _feesEnum.Pio * _employeeService.Exchange("RSD", "EUR"), 1) + "eur</th> <th style='border:1px solid #000'>Rate</th><th>14.00</th></tr>";
                HtmlContent += "<tr><th style='text-align:left;border:1px solid red'>Health</th><th>" + Math.Round(employee.NetoSalary * _feesEnum.Health * _employeeService.Exchange("RSD", "EUR"), 1) + "eur</th> <th style='border:1px solid #000'>Rate</th><th>5.15</th></tr>";
                HtmlContent += "<tr><th style='text-align:left;border:1px solid red'>Unemployment</th><th>" + Math.Round(employee.NetoSalary * _feesEnum.Unemployment * _employeeService.Exchange("RSD", "EUR"), 1) + "eur</th> <th style='border:1px solid #000'>Rate</th><th>0.75</th></tr>";
                HtmlContent += "<tr><th style='text-align:left;border:1px solid red'>Sum</th><th>" + Math.Round(sumFees * _employeeService.Exchange("RSD", "EUR"), 1) + "eur</th> <th style='border:1px solid #000'>Sum</th><th>29.9</th></tr>";

            HtmlContent += "</table>"; // closed table

            HtmlContent += "<h4 style='text-align:left;margin-top:10px'>Gross salary: "+ Math.Round(employee.GrossSalary * _employeeService.Exchange("RSD", "EUR"), 1)+"eur</h4>";




            HtmlContent += "<table style='width:30%;margin-top:20px'>"; // opened table

            HtmlContent += "<tr style='margin-top:20px'><th style='text-align:left;border:1px solid blue'>TAX</th><th>" + Math.Round(employee.NetoSalary * _feesEnum.Tax * _employeeService.Exchange("RSD", "USD"), 1) + "usd</th> <th style='border:1px solid #000'>Rate</th><th>10.00</th></tr>";
            HtmlContent += "<tr><th style='text-align:left;border:1px solid blue'>PIO</th><th>" + Math.Round(employee.NetoSalary * _feesEnum.Pio * _employeeService.Exchange("RSD", "USD"), 1) + "usd</th> <th style='border:1px solid #000'>Rate</th><th>14.00</th></tr>";
            HtmlContent += "<tr><th style='text-align:left;border:1px solid blue'>Health</th><th>" + Math.Round(employee.NetoSalary * _feesEnum.Health * _employeeService.Exchange("RSD", "USD"), 1) + "usd</th> <th style='border:1px solid #000'>Rate</th><th>5.15</th></tr>";
            HtmlContent += "<tr><th style='text-align:left;border:1px solid blue'>Unemployment</th><th>" + Math.Round(employee.NetoSalary * _feesEnum.Unemployment * _employeeService.Exchange("RSD", "USD"), 1) + "usd</th> <th style='border:1px solid #000'>Rate</th><th>0.75</th></tr>";
            HtmlContent += "<tr><th style='text-align:left;border:1px solid blue'>Sum</th><th>" + Math.Round(sumFees * _employeeService.Exchange("RSD", "USD"), 1) + "usd</th> <th style='border:1px solid #000'>Sum</th><th>29.9</th></tr>";

            HtmlContent += "</table>"; // closed table

            HtmlContent += "<h4 style='text-align:left;margin-top:10px'>Gross salary: " + Math.Round(employee.GrossSalary * _employeeService.Exchange("RSD", "USD"), 1) + "usd</h4>";




            HtmlContent += "</div>"; //closed div



            PdfGenerator.AddPdfPages(document, HtmlContent, PageSize.A4);
            byte[]? response = null;
            using (MemoryStream ms = new MemoryStream())
            {
                document.Save(ms);
                response = ms.ToArray();
            }

            string Filename = "employee.pdf";

            return File(response, "appliation/pdf", Filename);
        }



        [HttpGet]
        [Route("Exchange/{value1}/{value2}")]
        public async Task<ActionResult> Exchange(string value1, string value2)
        {
            if (value2 == "USD" || value2 == "EUR")
            {
                return Ok(_employeeService.Exchange(value1, value2));
            }

            return BadRequest("Currency is not correct!");



            /*
              API_Obj data = Rates.Import(value1);

              if (value2 == "EUR")
              {
                  return Ok(data.conversion_rates.EUR);

              }

              return Ok(data.conversion_rates.USD);
            */
        }




    }
}
