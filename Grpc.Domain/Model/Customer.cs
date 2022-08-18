using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Grpc.Domain.Model
{
    public class Customer : EntityBase
    {
        [NotMapped]
        public override string Namespace => "SalesLT";

        [NotMapped]
        public override object Id
        {
            get => CustomerId;
            set => CustomerId = Convert.ToInt32(value);
        }

        [Key]
        public int CustomerId { get; set; }
        public bool NameStyle { get; set; }
        public string Title { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Suffix { get; set; }
        public string CompanyName { get; set; }
        public string SalesPerson { get; set; }
        public string EmailAddress { get; set; }
        public string Phone { get; set; }
        public string PasswordHash { get; set; }
        public string PasswordSalt { get; set; }
        public Guid RowGuid { get; set; }
        public DateTime ModifiedDate { get; set; }
    }
}