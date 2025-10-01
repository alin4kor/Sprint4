﻿
namespace Grocery.Core.Models
{
    public partial class Client : Model
    {
        public string EmailAddress { get; set; }
        public string Password { get; set; }
        public enum Role { None, Admin }
        public Role UserRole { get; set; } = Role.None;

        public Client(int id, string name, string emailAddress, string password, Role userRole) : base(id, name)
        {
            EmailAddress=emailAddress;
            Password=password;
            UserRole=userRole;
        }
    }
}
