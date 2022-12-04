using System.ComponentModel.DataAnnotations;

namespace DemoSoftserve
{
    public class Customer
    {
        public Guid CustomerID { get; set; }
        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public string UserName { get; set; } = string.Empty;

        [EmailAddress(ErrorMessage = "Not a valid email")]
        public string EmailAddress { get; set; } = string.Empty;

        public DateTime DateOfBirth { get; set; }

        public int Age { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime DateEdited { get; set; }

        public bool IsDeleted { get; set; }
    }
}
