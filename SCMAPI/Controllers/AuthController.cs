using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SCMAPI.Controllers
{
    public class AuthController : ApiController
    {
        [HttpPost]
        [ActionName("requesttoken")]
        public TokenRequest RequestToken([FromBody]TokenRequest request)
        {
            TokenRequest tokenobj = new TokenRequest();

            if (!ModelState.IsValid)
            {
                tokenobj.error = "Invalid Request";
                return tokenobj;
            }
            string token;
            if (_authservice.IsAuthenticated(request, out token))
            {
                tokenobj.access_token = token;
                return tokenobj;

            }
            tokenobj.error = "Invalid Request";
            return tokenobj;
        }
    }
}
