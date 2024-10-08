// ---------------------------------------------------------------------------
// File: UserLoginRequest.cs
// Author: IT21189630
// Date Created: 2024-09-30
// Description: This class contains all the properties related to a login request.
// Version: 1.0.0
// ---------------------------------------------------------------------------
namespace EAD_Web_Service_API.Models
{
    public class UserLoginRequest
    {
        public string email { get; set; }
        public string password { get; set; }
    }
}
