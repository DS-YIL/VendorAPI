using BALayer.RFQ;
using SCMModels;
using SCMModels.MPRMasterModels;
using SCMModels.RemoteModel;
using SCMModels.RFQModels;
using SCMModels.SCMModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;

namespace SCMAPI.Controllers
{
    [RoutePrefix("Api/Forgetpassword")]
    public class ForgetpasswordController : ApiController
    {
        private readonly IRFQBA _rfqBusenessAcess;
        public ForgetpasswordController(IRFQBA rfqBA)
        {
            this._rfqBusenessAcess = rfqBA;
        }
        [HttpPost]
        [Route("checkemail")]
        public IHttpActionResult checkemailidexists(Changepassword email)
        {
         
            bool exists = this._rfqBusenessAcess.checkemail(email);
            return Json(exists);
        }
        [HttpPost]
        [Route("sendLinkforForgetpassword")]
        public IHttpActionResult Forgetpasswordmail(forgetpassword email)
        {
            email.baseURL = ConfigurationManager.AppSettings["baseURL"];
            email.tokenduration = ConfigurationManager.AppSettings["tokenExpiryduration"];
            email.fromemail= ConfigurationManager.AppSettings["fromemail"];
            bool exists = this._rfqBusenessAcess.sendLinkForForgetPassword(email);
            return Json(exists);
            //tets2
        }
        [HttpPost]
        [Route("Resetpassword")]
        public IHttpActionResult Resetpassword(forgetpassword email)

        {
            email.baseURL = ConfigurationManager.AppSettings["baseURL"];
            email.tokenduration = ConfigurationManager.AppSettings["tokenExpiryduration"];
            email.fromemail = ConfigurationManager.AppSettings["fromemail"];
            string exists = this._rfqBusenessAcess.Resetpassword(email);
            return Json(exists);
        }
        [HttpPost]
        [Route("checkforgetpasswordlink")]
        public IHttpActionResult checkforgetpasswordlink(forgetpassword model)
        {
            //test
            return Ok(this._rfqBusenessAcess.CheckLinkExpiryOrNot(model));
        }
    
}
}
