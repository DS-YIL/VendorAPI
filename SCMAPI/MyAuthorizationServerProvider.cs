using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;

namespace SCMAPI
{
    public class MyAuthorizationServerProvider : OAuthAuthorizationServerProvider
    {
        public override async Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            context.Validated();
        }
        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            using (UserMasterRepository _repo = new UserMasterRepository())
            {
                var user = _repo.ValidateUser(context.UserName, context.Password);
                if (user != null)
                {
                    var identity = new ClaimsIdentity(context.Options.AuthenticationType);
					identity.AddClaim(new Claim("Vuserid", Convert.ToString(user.Vuserid)));
					identity.AddClaim(new Claim(ClaimTypes.Role, user.UserName));
                    identity.AddClaim(new Claim(ClaimTypes.Name, user.UserName));
                    identity.AddClaim(new Claim("VuniqueId", user.VUniqueId));
                    identity.AddClaim(new Claim("VendorId", Convert.ToString(user.vendorId)));
                    identity.AddClaim(new Claim("VendorCode", Convert.ToString(user.VendorCode)));
					identity.AddClaim(new Claim("Street", Convert.ToString(user.Street)));

					context.Validated(identity);
                    
                }

                else
                {
                    context.SetError("invalid_grant", "Provided username and password is incorrect");
                    return;

                }

            }
        }


    }
}
