namespace EAD_Web_Service_API.Models
{
    public class CustomerLoginResponse
    {
        public string Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string Profile_Picture { get; set; }
        public bool Activation_Status { get; set; }
    }
}
