using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SCMAPI.Models
{
    public class UserProfile
    {
        public List<string> RoleIds { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Company { get; set; }
    }
}