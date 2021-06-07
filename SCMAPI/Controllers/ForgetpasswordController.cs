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
        [Route("UploadFile")]
        [HttpPost]
        public IHttpActionResult UploadFile()
        {
            var httpRequest = HttpContext.Current.Request;
			//var serverPath = HttpContext.Current.Server.MapPath("~/VSCMDocs");
			var serverPath = ConfigurationManager.AppSettings["AttachedDocPath"];
			string parsedFileName = "";
            var revisionId = httpRequest.Files.AllKeys[0];
            if (httpRequest.Files.Count > 0)
            {
                foreach (string file in httpRequest.Files)
                {
                    var postedFile = httpRequest.Files[file];
                    byte[] fileData = null;
                    using (var binaryReader = new BinaryReader(postedFile.InputStream))
                    {
                        fileData = binaryReader.ReadBytes(postedFile.ContentLength);
                    }



                    GC.Collect();
                    parsedFileName = string.Format(DateTime.Now.Year.ToString() + "\\" + DateTime.Now.ToString("MMM") + "\\" + revisionId + "\\" + ToValidFileName(postedFile.FileName));
                    serverPath = serverPath + string.Format("\\" + DateTime.Now.Year.ToString() + "\\" + DateTime.Now.ToString("MMM")) + "\\" + revisionId;
                    var path = Path.Combine(serverPath, ToValidFileName(postedFile.FileName));
                    if (!Directory.Exists(serverPath))
                        Directory.CreateDirectory(serverPath);
                    var memory = new MemoryStream();
                    FileStream fs = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write);
                    var updatedStream = new MemoryStream(fileData);
                    updatedStream.Seek(0, SeekOrigin.Begin);
                    updatedStream.CopyToAsync(fs).Wait();
                    fs.Flush();
                    GC.Collect();
                }
            }
            return Ok(parsedFileName);



        }

		[Route("UploadVendorRegFile")]
		[HttpPost]
		public IHttpActionResult UploadVendorRegFile()
		{
			var filePath = "";
			string dbfilePath = "";
			List<RemoteVendorRegisterDocumentDetail> listobj = new List<RemoteVendorRegisterDocumentDetail>();
			var httpRequest = HttpContext.Current.Request;
			//var serverPath = HttpContext.Current.Server.MapPath("~/VSCMDocs");
			var serverPath = ConfigurationManager.AppSettings["AttachedDocPath"];
			if (httpRequest.Files.Count > 0)
			{
				string filename = string.Empty;
				string[] listdata;
				string VUniqueId, docid, vendorid, docType;
				foreach (string file in httpRequest.Files)
				{
					filename = file;
					break;
				}
				listdata = filename.Split('_');
				VUniqueId = listdata[0];
				vendorid = listdata[1];
				docid = listdata[2];
				docType = listdata[3];
				for (int i = 0; i <= httpRequest.Files.Count - 1; i++)
				{
					var postedFile = httpRequest.Files[i];
					filePath = serverPath + string.Format("\\" + DateTime.Now.Year.ToString() + "\\" + DateTime.Now.ToString("MMM") + "\\" + VUniqueId + "_" + vendorid + "_" + docid + "_" + docType);
					dbfilePath = string.Format(DateTime.Now.Year.ToString() + "\\" + DateTime.Now.ToString("MMM") + "\\" + VUniqueId + "_" + vendorid + "_" + docid + "_" + docType + "\\" + ToValidFileName(postedFile.FileName));
					if (!Directory.Exists(filePath))
						Directory.CreateDirectory(filePath);
					filePath = Path.Combine(filePath, ToValidFileName(postedFile.FileName));
					postedFile.SaveAs(filePath);
					//List<documentDetails> obj = new List<documentDetails>();
					//documentDetails eachobj = new documentDetails();
					//eachobj.DocumentationTypeId = Convert.ToInt32(docid);
					//eachobj.DocumentName = postedFile.FileName;
					//eachobj.PhysicalPath = dbfilePath;
					//eachobj.VendorId = Convert.ToInt32(vendorid);
					//obj.Add(eachobj);
					//listobj = _rfqBusenessAcess.InsertDocuments(obj);

				}
			}
			return Ok(dbfilePath);
		}
		private static string ToValidFileName(string fileName)
        {
            fileName = fileName.ToLower().Replace(" ", "_").Replace("(", "_").Replace(")", "_").Replace("&", "_").Replace("*", "_").Replace("-", "_").Replace("+", "_").Replace("?", "_");
			return string.Join("_", fileName.Split(Path.GetInvalidFileNameChars()));
        }

    }
}
