// ---------------------------------------------------------------------------
// File: IUser.cs
// Author: IT21189630
// Date Created: 2024-09-29
// Description: This interface contains all the properties related to a normal user.
// Version: 1.0.0
// ---------------------------------------------------------------------------
using EAD_Web_Service_API.Data;

namespace EAD_Web_Service_API.Models
{
    public interface IUser
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Username { get; set; }
        public string Profile_Picture { get; set; }
        public UserRoles Role { get; set; }
        public bool? Status { get; set; }
    }
}
