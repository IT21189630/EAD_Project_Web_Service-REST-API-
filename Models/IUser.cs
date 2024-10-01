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
    }
}
