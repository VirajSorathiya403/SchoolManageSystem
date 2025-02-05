using System;

namespace SchoolManageSys.Models
{
    public class UserModel
    {
        public int? UserId { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string MobileNo { get; set; }
        public string Email { get; set; }
        public int UserRoleId { get; set; }
        public string? UserRoleName { get; set; }
        public string ContactPersonName { get; set; }
    }
    
    public class UserRoleDropDownModel
    {
        public int UserRoleId { get; set; }
        public string UserRoleName { get; set; }
    }
    
    public class UserAuthModel
    {
        public string UserEmail { get; set; }
        public string Password { get; set; }
    }

}