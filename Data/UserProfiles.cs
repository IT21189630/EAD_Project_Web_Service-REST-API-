// ---------------------------------------------------------------------------
// File: UserProfiles.cs
// Author: IT21189630
// Date Created: 2024-09-29
// Description: This file contains the deafault profile avatars for the user roles.
// Version: 1.0.0
// ---------------------------------------------------------------------------
namespace EAD_Web_Service_API.Data
{
    public static class UserProfiles
    {
        public static readonly Dictionary<string, string> Profiles = new Dictionary<string, string>
        {
            { "Admin", "https://png.pngtree.com/element_our/20240816/093f247ea6c48a5591748e4868b6ef93.png" },
            { "Vendor", "https://png.pngtree.com/png-vector/20240914/ourmid/pngtree-cartoon-user-avatar-vector-png-image_13572228.png" },
            { "CSR", "https://png.pngtree.com/png-vector/20240913/ourmid/pngtree-staff-avatar-png-image_13306718.png" }
        };
    }
}
