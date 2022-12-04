using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mail;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace DemoSoftserve.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        
        private static List<Customer> customers = new List<Customer>()
            {
                new Customer()
                {
                    CustomerID = Guid.NewGuid(),
                    FirstName = "Test",
                    LastName = "One",
                    UserName = "TestOne",
                    EmailAddress = "testone@gmail.com",
                    DateOfBirth = DateTime.Now,
                    Age = 20,
                    DateCreated = DateTime.Now,
                    DateEdited = DateTime.Now,
                    IsDeleted = false
                }
            };

        private static int CalculateAge(DateTime dateOfBirth)
        {
            int age = 0;
            age = DateTime.Now.Year - dateOfBirth.Year;
            if (DateTime.Now.DayOfYear < dateOfBirth.DayOfYear)
                age = age - 1;

            return age;
        }
        


        [HttpGet]
        [Route("getCustomers")]
        public async Task<ActionResult<Customer>> GetCustomers()
        {
            
            return Ok(customers);
        }

        [HttpGet]
        [Route("getCustomer")]
        public async Task<ActionResult<Customer>> GetCustomer(Guid id)
        {
            var customer = customers.Find(x => x.CustomerID == id);
            if (customer == null)
            {
                return BadRequest("Customer Not Found");
            }

            return Ok(customer);
        }

        [HttpPost]
        [Route("addCustomers")]
        public async Task<ActionResult<Customer>> AddCustomers(Customer request)
        {
            request.CustomerID = Guid.NewGuid();
            request.UserName = request.FirstName + request.LastName;

            //email validation
            bool isEmail = Regex.IsMatch(request.EmailAddress, @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z", RegexOptions.IgnoreCase);
            if(!isEmail)
            {
                return BadRequest("Invalid Email Address");
            }

            //DOB validation
            if (request.DateOfBirth == DateTime.Now)
            {
                return BadRequest("Invalid Date of Birth");
            }

            request.Age = CalculateAge(request.DateOfBirth);

            request.DateCreated = DateTime.Now;
            request.DateEdited = DateTime.Now;
            request.IsDeleted = false;

            customers.Add(request);

            var fileName = "Customers.json";
            JsonFileUtils.Write(customers, fileName);

            return Ok(customers);
        }

        [HttpPut]
        [Route("updateCustomers")]
        public async Task<ActionResult<Customer>> UpdateCustomers(Customer request)
        {
            var customer = customers.Find(x => x.CustomerID == request.CustomerID);
            if (customer == null)
            {
                return BadRequest("Customer Not Found");
            }

            customer.FirstName = request.FirstName;
            customer.LastName = request.LastName;
            customer.UserName = request.UserName;
            customer.EmailAddress = request.EmailAddress;
            customer.DateOfBirth = request.DateOfBirth;
            customer.Age = request.Age;
            customer.DateEdited = DateTime.Now;
            customer.IsDeleted = request.IsDeleted;

            var fileName = "Customers.json";
            JsonFileUtils.Write(customers, fileName);

            return Ok(customers);
        }

        [HttpDelete]
        [Route("deleteCustomer")]
        public async Task<ActionResult<Customer>> DeleteCustomer(Guid id)
        {
            var customer = customers.Find(x => x.CustomerID == id);
            if (customer == null)
            {
                return BadRequest("Customer Not Found");
            }

            customers.Remove(customer);

            var fileName = "Customers.json";
            JsonFileUtils.Write(customers, fileName);

            return Ok(customers);
        }
    }
    public static class JsonFileUtils
    {
        private static readonly JsonSerializerOptions _options =
            new() { DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull };

        public static void Write(object obj, string fileName)
        {
            var options = new JsonSerializerOptions(_options)
            {
                WriteIndented = true
            };
            var jsonString = JsonSerializer.Serialize(obj, options);
            File.WriteAllText(fileName, jsonString);
        }
    }
}
