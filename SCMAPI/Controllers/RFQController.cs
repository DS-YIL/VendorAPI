/*
    Name of File : <<RFQController>>  Author :<<Prasanna>>  
    Date of Creation <<03-08-2020>>
    Purpose : <<This is the rfq controller to write rfq api's such as quote add,edit, terms add edit >>
    Review Date :<<>>   Reviewed By :<<>>
    Version : 0.1 <change version only if there is major change - new release etc>
    Sourcecode Copyright : Yokogawa India Limited
*/
using BALayer.RFQ;
using DALayer.Common;
using DALayer.Emails;
using SCMModels;
using SCMModels.RemoteModel;
using SCMModels.RFQModels;
using SCMModels.SCMModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;

namespace SCMAPI.Controllers
{
	/*Name of Class : <<RFQController>>  Author :<<Prasanna>>  
	Date of Creation <<03-08-2020>>
	Purpose : <<This is the rfq controller to write rfq api's such as quote add,edit, terms add edit >>>
	Review Date :<<>>   Reviewed By :<<>>*/
	[Authorize]
	[RoutePrefix("Api/RFQ")]
	public class RFQController : ApiController
	{
		private readonly IRFQBA _rfqBusenessAcess;
		private readonly IEmailTemplateDA _EmailTemplateDA;
		private ErrorLog log = new ErrorLog();
		public RFQController(IRFQBA rfqBA, IEmailTemplateDA IEmailTemplateDA)
		{
			this._rfqBusenessAcess = rfqBA;
			this._EmailTemplateDA = IEmailTemplateDA;
		}

		/*Name of Function : <<GetItemsByRevisionId>>  Author :<<Prasanna>>  
	    Date of Creation <<03-08-2020>>
	    Purpose : <<GetItemsByRevisionId >>
	    Review Date :<<>>   Reviewed By :<<>>*/
		[Route("GetItemsByRevisionId/{id}")]
		[ResponseType(typeof(List<RfqItemModel>))]
		public async Task<IHttpActionResult> GetItemsByRevisionId(int id)
		{
			List<RfqItemModel> status = new List<RfqItemModel>();
			status = await _rfqBusenessAcess.GetItemsByRevisionId(id);
			return Ok(status);
		}

		/*Name of Function : <<InsertDocument>>  Author :<<Prasanna>>  
	    Date of Creation <<03-08-2020>>
	    Purpose : <<Insert RFQ Document >>
	    Review Date :<<>>   Reviewed By :<<>>*/
		[Route("InsertDocument")]
		[HttpPost]
		public IHttpActionResult InsertDocument()
		{

			var filePath = "";
			var fileserverpath = "";
			List<RfqDocumentsModel> listobj = new List<RfqDocumentsModel>();
			var httpRequest = HttpContext.Current.Request;
			if (httpRequest.Files.Count > 0)
			{
				string filename = string.Empty;
				string[] listdata;
				string RFQRevId, RFQItemId, updatedBy, updatedRevisionId, pageType;
				foreach (string file in httpRequest.Files)
				{
					filename = file;
					break;
				}

				listdata = filename.Split('_');
				RFQRevId = listdata[0];
				RFQItemId = listdata[1];
				updatedBy = listdata[2];
				pageType = listdata[3];
				for (int i = 0; i <= httpRequest.Files.Count - 1; i++)
				{
					//string url = "10.29.15.183:90/Api/mpr/uploadfile";
					var postedFile = httpRequest.Files[i];
					//var serverPath = HttpContext.Current.Server.MapPath("~/VSCMDocs");
					var serverPath = ConfigurationManager.AppSettings["AttachedDocPath"];
					filePath = serverPath + string.Format("\\" + DateTime.Now.Year.ToString() + "\\" + DateTime.Now.ToString("MMM")) + "\\" + RFQRevId + "_" + RFQItemId + "_" + updatedBy + "_" + pageType;
					//filePath = HttpContext.Current.Server.MapPath("~/VSCMDocs") + string.Format("\\" + DateTime.Now.Year.ToString() + "\\" + DateTime.Now.ToString("MMM")) + "\\" + RFQRevId + "_" + RFQItemId + "_" + updatedBy + "_" + pageType;
					//  fileserverpath= ConfigurationManager.AppSettings["AttachedDocPathForServer"] + "\\" + filename;
					string dbfilePath = filename + "\\" + ToValidFileName(postedFile.FileName);
					if (!Directory.Exists(filePath))
						Directory.CreateDirectory(filePath);
					filePath = Path.Combine(filePath, ToValidFileName(postedFile.FileName));
					postedFile.SaveAs(filePath);

					//if (!Directory.Exists(fileserverpath))
					//    Directory.CreateDirectory(fileserverpath);
					//fileserverpath = Path.Combine(fileserverpath, postedFile.FileName);
					//postedFile.SaveAs(fileserverpath);

					List<RfqDocumentsModel> obj = new List<RfqDocumentsModel>();
					RfqDocumentsModel eachobj = new RfqDocumentsModel();


					eachobj.UploadedBy = updatedBy;
					eachobj.DocumentName = postedFile.FileName;
					eachobj.UploadedDate = System.DateTime.Now;
					if (RFQRevId.Contains("Technical"))
					{
						eachobj.DocumentType = 6;
						eachobj.RfqItemsId = Convert.ToInt32(RFQItemId);
						updatedRevisionId = RFQRevId.Replace("Technical", "");
						eachobj.RfqRevisionId = RFQRevId;
					}
					else
					{
						eachobj.DocumentType = Convert.ToInt32(RFQItemId);
						eachobj.RfqRevisionId = RFQRevId;

					}
					eachobj.Path = string.Format(DateTime.Now.Year.ToString() + "\\" + DateTime.Now.ToString("MMM") + "\\" + dbfilePath);
					eachobj.IsDeleted = false;
					obj.Add(eachobj);


					listobj = _rfqBusenessAcess.InsertDocument(eachobj);
					//_rfqBusenessAcess.InsertDocumentTOYSCM(eachobj);

				}

			}
			uploadfileToScm(httpRequest, "InsertVscmVendorQuoteDocument");
			return Json(listobj);
		}

		/*Name of Function : <<GetRfqDetailsById>>  Author :<<Prasanna>>  
	    Date of Creation <<03-08-2020>>
	    Purpose : <<GetRfqDetailsBy Rfq revisionid >>
	    Review Date :<<>>   Reviewed By :<<>>*/
		[Route("GetRfqDetailsById/{RevisionId}")]
		public async Task<IHttpActionResult> GetRfqDetailsById(int RevisionId)
		{
			//RfqRevisionModel revision = new RfqRevisionModel();
			//revision = await _rfqBusenessAcess.GetRfqDetailsById(RevisionId);
			return Ok(await _rfqBusenessAcess.GetRfqDetailsById(RevisionId));
		}

		/*Name of Function : <<InsertOrEditRfqItemInfo>>  Author :<<Prasanna>>  
	    Date of Creation <<21-10-2020>>
	    Purpose : <<Insert Quotation foritem >>
	    Review Date :<<>>   Reviewed By :<<>>*/
		[Route("InsertOrEditRfqItemInfo")]
		public IHttpActionResult InsertOrEditRfqItemInfo(RfqItemModel model)
		{
			return Json(this._rfqBusenessAcess.InsertOrEditRfqItemInfo(model));
		}
		/*Name of Function : <<editRfqItemInfo>>  Author :<<Prasanna>>  
	    Date of Creation <<21-10-2020>>
	    Purpose : <<Insert Quotation for item for RemoteRfqVendorBOMs  >>
	    Review Date :<<>>   Reviewed By :<<>>*/

		[Route("editRfqItemInfo")]
		public IHttpActionResult editRfqItemInfo(RfqItemModel model)
		{
			return Json(this._rfqBusenessAcess.editRfqItemInfo(model));
		}

		/*Name of Function : <<RfqIteminfoDeleteByid>>  Author :<<Prasanna>>  
	    Date of Creation <<21-10-2020>>
	    Purpose : <<RfqIteminfoDeleteByid  >>
	    Review Date :<<>>   Reviewed By :<<>>*/

		[HttpGet]
		[Route("RfqIteminfoDeleteByid/{id}/{rfqitemid}")]
		public async Task<IHttpActionResult> DeleteRfqIteminfoByid(int id, int rfqitemid)
		{
			statuscheckmodel status = new statuscheckmodel();
			status = await _rfqBusenessAcess.DeleteRfqIteminfoByid(id, rfqitemid);
			return Ok(status);
		}
		/*Name of Function : <<RfqIteminfoDeleteByidformultiple>>  Author :<<Prasanna>>  
	    Date of Creation <<21-10-2020>>
	    Purpose : <<RfqIteminfoDeleteByidformultiple  >>
	    Review Date :<<>>   Reviewed By :<<>>*/
		[HttpGet]
		[Route("RfqIteminfoDeleteByidformultiple/{id}/{BOMId}")]
		public async Task<IHttpActionResult> DeleteRfqIteminfoByidformultiple(int id, int BOMId)
		{
			statuscheckmodel status = new statuscheckmodel();
			status = await _rfqBusenessAcess.DeleteRfqIteminfoByidformultiple(id, BOMId);
			return Ok(status);
		}

		/*Name of Function : <<GetUnitMasterList>>  Author :<<Prasanna>>  
	    Date of Creation <<04-09-2020>>
	    Purpose : <<Get Unit MasterList  >>
	    Review Date :<<>>   Reviewed By :<<>>*/

		[HttpGet]
		[Route("GetUnitMasterList")]
		public async Task<IHttpActionResult> GetUnitMasterList()
		{
			List<UnitMasterModel> model = new List<UnitMasterModel>();
			model = await _rfqBusenessAcess.GetUnitMasterList();
			return Ok(model);
		}

		/*Name of Function : <<VendorRegister>>  Author :<<Prasanna>>  
	    Date of Creation <<04-09-2020>>
	    Purpose : <<VendorRegister  >>
	    Review Date :<<>>   Reviewed By :<<>>*/
		[HttpPost]
		[Route("VendorRegister")]
		public IHttpActionResult VendorRegistration(VendorRegistrationModel obj)
		{
			return Json(this._rfqBusenessAcess.InsertVendordata(obj));
		}
		/*Name of Function : <<GetStateList>>  Author :<<Prasanna>>  
	    Date of Creation <<04-09-2020>>
	    Purpose : <<GetStateList  >>
	    Review Date :<<>>   Reviewed By :<<>>*/
		[HttpGet]
		[Route("GetStateList")]
		public IHttpActionResult GetStateNameList()
		{
			return Ok(this._rfqBusenessAcess.StateNameList());
		}
		/*Name of Function : <<GetNaturOfBusiness>>  Author :<<Prasanna>>  
	    Date of Creation <<04-09-2020>>
	    Purpose : <<GetNaturOfBusiness  >>
	    Review Date :<<>>   Reviewed By :<<>>*/
		[HttpGet]
		[Route("GetNaturOfBusiness")]
		public IHttpActionResult GetNatureOfBusiness()
		{
			return Ok(this._rfqBusenessAcess.natureOfBusinessesList());
		}

		/*Name of Function : <<UploadFile>>  Author :<<Prasanna>>  
	    Date of Creation <<04-09-2020>>
	    Purpose : <<UploadFile rfq docs  >>
	    Review Date :<<>>   Reviewed By :<<>>*/
		[Route("UploadFile")]
		[HttpPost]
		public IHttpActionResult UploadFile()
		{

			var filePath = "";
			string dbfilePath = "";
			try
			{
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

				uploadfileToScm(httpRequest, "UploadVscmFile");
			}
			catch (Exception e)
			{
				log.ErrorMessage("RFQController", "UploadFile", e.Message.ToString());
			}
			return Ok(dbfilePath);
		}

		/*Name of Function : <<ToValidFileName>>  Author :<<Prasanna>>  
	    Date of Creation <<04-09-2020>>
	    Purpose : <<ToValidFileName  >>
	    Review Date :<<>>   Reviewed By :<<>>*/

		private static string ToValidFileName(string fileName)
		{
			fileName = fileName.ToLower().Replace(" ", "_").Replace("(", "_").Replace(")", "_").Replace("&", "_").Replace("*", "_").Replace("-", "_").Replace("+", "_").Replace("?", "_");
			return string.Join("_", fileName.Split(Path.GetInvalidFileNameChars()));
		}

		/*Name of Function : <<ToValidFileName>>  Author :<<Prasanna>>  
		Date of Creation <<04-09-2020>>
		Purpose : <<ToValidFileName  >>
		Review Date :<<>>   Reviewed By :<<>>*/
		[HttpGet]
		[Route("GetDocumentTypeList")]
		public IHttpActionResult GetDocumentsList()
		{
			return Ok(this._rfqBusenessAcess.DocumentMasterList());
		}
		/*Name of Function : <<ToValidFileName>>  Author :<<Prasanna>>  
		Date of Creation <<04-09-2020>>
		Purpose : <<ToValidFileName  >>
		Review Date :<<>>   Reviewed By :<<>>*/
		[HttpPost]
		[Route("SaveVendorDetails")]
		public IHttpActionResult SaveVendorData(VendorRegistrationModel model)
		{
			return Ok(this._rfqBusenessAcess.SaveVendorDetails(model));
		}
		/*Name of Function : <<updateRegTerms>>  Author :<<Prasanna>>  
		Date of Creation <<01-06-2021>>
		Purpose : <<updateRegTerms  >>
		Review Date :<<>>   Reviewed By :<<>>*/
		[HttpPost]
		[Route("updateRegTerms")]
		public IHttpActionResult updateRegTerms(VendorRegistrationModel model)
		{
			return Ok(this._rfqBusenessAcess.updateRegTerms(model));
		}
		/*Name of Function : <<deleteAttachedfile>>  Author :<<Prasanna>>  
		Date of Creation <<04-09-2020>>
		Purpose : <<deleteAttachedfile  >>
		Review Date :<<>>   Reviewed By :<<>>*/
		[HttpPost]
		[Route("deleteAttachedfile")]
		public IHttpActionResult deletefile(documentDetails model)
		{
			var path1 = model.PhysicalPath.Replace("\",\"", "\\");
			var eachobj = new documentDetails();
			string[] listofdata = { };
			string filename = model.PhysicalPath;
			string[] parts = model.PhysicalPath.Split('_');
			string lastWord = parts[0];
			eachobj.PhysicalPath = ConfigurationManager.AppSettings["AttachedDocPath"] + "\\" + parts[0] + "_" + parts[1] + "\\" + model.PhysicalPath;
			//if(parts[0].Contains("Technical"))
			//       {
			//       parts[0] = parts[0].Replace("Technical", "");
			//       }

			eachobj.DocumentationTypeId = Convert.ToInt32(parts[0]);
			eachobj.VendorId = Convert.ToInt32(parts[1]);


			return Ok(this._rfqBusenessAcess.DeletefileAttached(eachobj));
		}
		/*Name of Function : <<deleteRegAttachedfile>>  Author :<<Prasanna>>  
		Date of Creation <<04-09-2020>>
		Purpose : <<deleteRegAttachedfile  >>
		Review Date :<<>>   Reviewed By :<<>>*/
		[HttpPost]
		[Route("deleteRegAttachedfile")]
		public IHttpActionResult deleteRegAttachedfile(documentDetails model)
		{
			return Ok(this._rfqBusenessAcess.DeletefileAttached(model));
		}

		/*Name of Function : <<deleteAttachedDocuments>>  Author :<<Prasanna>>  
		Date of Creation <<15-10-2020>>
		Purpose : <<deleteAttachedDocuments  >>
		Review Date :<<>>   Reviewed By :<<>>*/
		[HttpPost]
		[Route("deleteAttachedDocuments")]
		public IHttpActionResult deleteattacheddocument(documentDetails model)
		{
			var eachobj = new RFQDocument();

			eachobj.RfqDocId = model.Id;
			return Ok(this._rfqBusenessAcess.DeletefileAttachedforDocuments(eachobj));
		}
		/*Name of Function : <<GetRfqByVendorId>>  Author :<<Prasanna>>  
		Date of Creation <<15-10-2020>>
		Purpose : <<GetRfqByVendorId  >>
		Review Date :<<>>   Reviewed By :<<>>*/
		[HttpPost]
		[Route("GetRfqByVendorId")]
		public IHttpActionResult GetRfqByVendorId(rfqFilterParams obj)
		{
			return Ok(this._rfqBusenessAcess.GetRfqByVendorId(obj));
		}

		/*Name of Function : <<GetAllMasterCurrency>>  Author :<<Prasanna>>  
		Date of Creation <<15-10-2020>>
		Purpose : <<GetAllMasterCurrency  >>
		Review Date :<<>>   Reviewed By :<<>>*/
		[HttpGet]
		[Route("GetAllMasterCurrency")]
		[ResponseType(typeof(List<CurrencyMasterModel>))]
		public async Task<IHttpActionResult> GetAllMaster()
		{
			List<CurrencyMasterModel> model = new List<CurrencyMasterModel>();
			model = await this._rfqBusenessAcess.GetAllMasterCurrency();
			return Ok(model);
		}

		/*Name of Function : <<GetAllRFQTerms>>  Author :<<Prasanna>>  
		Date of Creation <<15-10-2020>>
		Purpose : <<GetAllRFQTerms  >>
		Review Date :<<>>   Reviewed By :<<>>*/
		[HttpGet]
		[Route("GetAllRFQTerms/{RFQRevisionId}")]
		public IHttpActionResult GetTermsMasterData(int RFQRevisionId)
		{
			return Ok(this._rfqBusenessAcess.GetTermMaster(RFQRevisionId));
		}
		/*Name of Function : <<RFQTermUpdate>>  Author :<<Prasanna>>  
		Date of Creation <<23-10-2020>>
		Purpose : <<RFQTermUpdate  >>
		Review Date :<<>>   Reviewed By :<<>>*/
		[HttpPost]
		[Route("RFQTermUpdate")]
		public IHttpActionResult UpdateVendorTerms(List<RemoteRfqTerm> obj)
		{

			return Json(this._rfqBusenessAcess.UpdateVendorTerms(obj));
		}
		/*Name of Function : <<RFQTermUpdate>>  Author :<<Prasanna>>  
		Date of Creation <<04-09-2020>>
		Purpose : <<RFQTermUpdate  >>
		Review Date :<<>>   Reviewed By :<<>>*/
		[HttpGet]
		[Route("GetDocumentMasterList")]
		public IHttpActionResult GetMasterListData()
		{
			return Ok(this._rfqBusenessAcess.GetMasterDocumentTypeList());

		}
		/*Name of Function : <<InsertVendorCommunication>>  Author :<<Prasanna>>  
		Date of Creation <<04-09-2020>>
		Purpose : <<InsertVendorCommunication  >>
		Review Date :<<>>   Reviewed By :<<>>*/
		[HttpPost]
		[Route("InsertVendorCommunication")]
		public IHttpActionResult InsertRFQVendorCommunication(VendorCommunicaton modelobj)
		{
			return Ok(this._rfqBusenessAcess.UpdateVendorCommunication(modelobj));
		}
		/*Name of Function : <<GetCommunicationListForRFQRevisionId>>  Author :<<Prasanna>>  
		Date of Creation <<04-09-2020>>
		Purpose : <<GetCommunicationListForRFQRevisionId  >>
		Review Date :<<>>   Reviewed By :<<>>*/
		[HttpGet]
		[Route("GetCommunicationListForRFQRevisionId/{RFQRevisionId}")]
		public IHttpActionResult GetRfqCommunicationForRFQRevId(int RFQRevisionId)
		{
			return Ok(this._rfqBusenessAcess.GetVendorCommunicationForRFQRevId(RFQRevisionId));
		}
		/*Name of Function : <<GetTermsByRFQrevisionId>>  Author :<<Prasanna>>  
		Date of Creation <<04-09-2020>>
		Purpose : <<GetTermsByRFQrevisionId  >>
		Review Date :<<>>   Reviewed By :<<>>*/
		[HttpGet]
		[Route("GetTermsByRFQrevisionId/{RFQRevisionId}")]
		public IHttpActionResult GetTermsRFQRevId(int RFQRevisionId)
		{
			return Ok(this._rfqBusenessAcess.GetTermsByRfqRevisionId(RFQRevisionId));
		}


		/*Name of Function : <<Vendordetails>>  Author :<<Prasanna>>  
		Date of Creation <<06-11-2020>>
		Purpose : <<Vendordetails  >>
		Review Date :<<>>   Reviewed By :<<>>*/
		[HttpGet]
		[Route("Vendordetails")]
		public IHttpActionResult GetResource1()
		{
			VSCMEntities vscm = new VSCMEntities();
			var identity = (ClaimsIdentity)User.Identity;
			var VendorId = identity.Claims
					  .FirstOrDefault(c => c.Type == "VendorId").Value;
			var Vuniqueid = identity.Claims
					  .FirstOrDefault(c => c.Type == "VuniqueId").Value;
			var VendorCode = identity.Claims
					  .FirstOrDefault(c => c.Type == "VendorCode").Value;
			var Vuserid = identity.Claims
					  .FirstOrDefault(c => c.Type == "Vuserid").Value;
			var address = identity.Claims
					  .FirstOrDefault(c => c.Type == "Street").Value;

			List<VendorModel> listobj = new List<VendorModel>();
			VendorModel eachobj = new VendorModel();
			eachobj.UserName = identity.Name;
			eachobj.Vuserid = Vuserid;
			eachobj.vendorId = Convert.ToInt32(VendorId);
			eachobj.VUniqueId = Vuniqueid;
			eachobj.VendorCode = VendorCode;
			eachobj.Street = address;
			if (vscm.RemoteVendorRegisterMasters.Where(li => li.Vendorid == eachobj.vendorId).FirstOrDefault() != null)
				eachobj.isRegister = true;
			else
				eachobj.isRegister = false;
			listobj.Add(eachobj);
			return Ok(eachobj);
		}

		/*Name of Function : <<resource2>>  Author :<<Prasanna>>  
		Date of Creation <<04-09-2020>>
		Purpose : <<This resource is only For Admin and SuperAdmin role  >>
		Review Date :<<>>   Reviewed By :<<>>*/
		[HttpGet]
		[Route("api/test/resource2")]
		public IHttpActionResult GetResource2()
		{
			var identity = (ClaimsIdentity)User.Identity;
			var Email = identity.Claims
					  .FirstOrDefault(c => c.Type == "Email").Value;

			var UserName = identity.Name;

			return Ok("Hello " + UserName + ", Your Email ID is :" + Email);
		}

		/*Name of Function : <<resource3>>  Author :<<Prasanna>>  
		Date of Creation <<04-09-2020>>
		Purpose : <<This resource is only For SuperAdmin role >>
		Review Date :<<>>   Reviewed By :<<>>*/
		[HttpGet]
		[Route("api/test/resource3")]
		public IHttpActionResult GetResource3()
		{
			var identity = (ClaimsIdentity)User.Identity;
			var roles = identity.Claims
						.Where(c => c.Type == ClaimTypes.Role)
						.Select(c => c.Value);
			var identity1 = (ClaimsIdentity)User.Identity;
			IEnumerable<Claim> claims = identity.Claims;
			return Ok("Hello " + identity.Name + "Your Role(s) are: " + string.Join(",", roles.ToList()));
		}

		/*Name of Function : <<getdocumentbyrevid>>  Author :<<Prasanna>>  
		Date of Creation <<04-09-2020>>
		Purpose : <<getdocumentbyrevid >>
		Review Date :<<>>   Reviewed By :<<>>*/
		[HttpGet]
		[Route("getdocumentbyrevid/{revisionid}")]
		public IHttpActionResult getdocumentsbyRevid(int revisionid)

		{
			return Ok(this._rfqBusenessAcess.GetRfqdocumentdetailsById(revisionid));
		}
		/*Name of Function : <<getdocumentbyrevid>>  Author :<<Prasanna>>  
		Date of Creation <<04-09-2020>>
		Purpose : <<getdocumentbyrevid and item id>>
		Review Date :<<>>   Reviewed By :<<>>*/

		[HttpGet]
		[Route("getdocumentbyrevid/{revisionid}/{rfqitemsid}")]
		public IHttpActionResult getdocumentsbyRevid(int revisionid, int rfqitemsid)
		{
			return Ok(this._rfqBusenessAcess.GetRfqdocumentdetailsById(revisionid, rfqitemsid));
		}

		/*Name of Function : <<Getvendordetails>>  Author :<<Prasanna>>  
		Date of Creation <<04-09-2020>>
		Purpose : <<Getvendordetails by vendor id>>
		Review Date :<<>>   Reviewed By :<<>>*/
		[HttpGet]
		[Route("Getvendordetails/{vendorid}")]
		public IHttpActionResult getvendordetails(int vendorid)
		{
			return Ok(this._rfqBusenessAcess.GetVendorDetails(vendorid));
		}
		/*Name of Function : <<change password>>  Author :<<Prasanna>>  
		Date of Creation <<04-09-2020>>
		Purpose : <<change password>>
		Review Date :<<>>   Reviewed By :<<>>*/
		[HttpPost]
		[Route("changepassword")]
		public IHttpActionResult changepassword(Changepassword model)
		{
			return Ok(this._rfqBusenessAcess.changepassword(model));
		}

		/*Name of Function : <<finalsubmitfromVendor>>  Author :<<Prasanna>>  
		Date of Creation <<04-09-2020>>
		Purpose : <<final submit fromVendor>>
		Review Date :<<>>   Reviewed By :<<>>*/
		[HttpGet]
		[Route("finalsubmitfromVendor/{RFQRevisionId}/{updatedby}")]
		public IHttpActionResult finalsubmitfromVendor(int RFQRevisionId, string updatedby)
		{
			VSCMEntities vscm = new VSCMEntities();
			YSCMEntities obj = new YSCMEntities();
			Boolean check = false;
			int RfqMasterId = vscm.RemoteRFQRevisions_N.Where(li => li.rfqRevisionId == RFQRevisionId).FirstOrDefault().rfqMasterId;

			//if (check == true)
			//{
			try
			{
				RemoteRFQStatu statusobj = new RemoteRFQStatu();
				statusobj.RfqRevisionId = RFQRevisionId;
				statusobj.RfqMasterId = RfqMasterId;
				statusobj.StatusId = 8;
				statusobj.DeleteFlag = false;
				statusobj.updatedby = updatedby;
				statusobj.updatedDate = System.DateTime.Now;
				vscm.RemoteRFQStatus.Add(statusobj);
				RemoteRFQRevisions_N remoteRfqRevision = vscm.RemoteRFQRevisions_N.Where(li => li.rfqRevisionId == RFQRevisionId).FirstOrDefault();
				if (remoteRfqRevision != null)
				{
					remoteRfqRevision.StatusId = 8;
				}

				vscm.SaveChanges();
				int rfqstatusid = statusobj.RfqStatusId;
				RFQStatu statusobjs = new RFQStatu();
				statusobjs.RfqStatusId = rfqstatusid;
				statusobjs.RfqRevisionId = RFQRevisionId;
				statusobjs.RfqMasterId = RfqMasterId;
				statusobjs.StatusId = 8;
				statusobjs.DeleteFlag = false;
				statusobjs.updatedby = updatedby;
				statusobjs.updatedDate = System.DateTime.Now;
				obj.RFQStatus.Add(statusobjs);
				RFQRevisions_N localRfqRevision = obj.RFQRevisions_N.Where(li => li.rfqRevisionId == RFQRevisionId).FirstOrDefault();
				if (localRfqRevision != null)
				{
					localRfqRevision.StatusId = 8;
				}
				obj.SaveChanges();
				check = this._EmailTemplateDA.sendQuotemailtoRequestor(RFQRevisionId);
				//this._rfqBusenessAcess.sendemailfromvendor(RFQRevisionId);
				if (vscm.RemoteRFQDocuments.Where(li => li.rfqRevisionId == RFQRevisionId && li.DocumentType == 6).ToList().Count > 0)
				{

					//send notification to mpr Checker,MPR Approver,RFQRequester, Buyer Manager, mpr incharges
					this._EmailTemplateDA.sendTechNotificationMail(RFQRevisionId);
				}
			}
			catch (Exception e)
			{
				log.ErrorMessage("RFQController", "finalsubmitfromVendor", e.Message.ToString() + e.InnerException.ToString() + e.ToString());
				//Log the exception
			}
			//  }
			return Json(check);
		}

		/*Name of Function : <<rfqStatusUpdate>>  Author :<<Prasanna>>  
		Date of Creation <<04-09-2020>>
		Purpose : <<rfq Status Update>>
		Review Date :<<>>   Reviewed By :<<>>*/
		[HttpPost]
		[Route("rfqStatusUpdate")]
		public IHttpActionResult rfqStatusUpdate([FromBody] RFQStatu Result)
		{
			return Ok(this._rfqBusenessAcess.rfqStatusUpdate(Result));
		}

		/*Name of Function : <<checkrfqitemsid>>  Author :<<Prasanna>>  
		Date of Creation <<04-09-2020>>
		Purpose : <<checkrfqitemsid>>
		Review Date :<<>>   Reviewed By :<<>>*/
		[HttpGet]
		[Route("checkrfqitemsid")]
		public IHttpActionResult checkrfqitemsid(int rfqitemsid)
		{
			return Ok(this._rfqBusenessAcess.checkrfqitemexists(rfqitemsid));
		}

		/*Name of Function : <<getDBMastersList>>  Author :<<Prasanna>>  
		Date of Creation <<23-10-2020>>
		Purpose : <<get table details based on dynamic query>>
		Review Date :<<>>   Reviewed By :<<>>*/
		[HttpPost]
		[Route("getDBMastersList")]
		public IHttpActionResult getDBMastersList([FromBody] DynamicSearchResult Result)
		{
			return Ok(this._rfqBusenessAcess.getDBMastersList(Result));
		}

		/*Name of Function : <<uploadfileToScm>>  Author :<<Prasanna>>  
		Date of Creation <<15-10-2020>>
		Purpose : <<upload files To Scm>>
		Review Date :<<>>   Reviewed By :<<>>*/
		public bool uploadfileToScm(HttpRequest request, string method)
		{
			//http://10.29.15.183:90/Api/mpr/UploadVscmFile/
			string serviceUrl = ConfigurationManager.AppSettings["SCMDocUploadPath"];
			//log.ErrorMessage("VendorRFQController", "serviceurl called", serviceUrl);
			serviceUrl = serviceUrl + method;
			bool status = false;
			byte[] fileArray = null;
			//var re = request.Files.AllKeys[0];
			string fileName = request.Files[0].FileName;
			using (var binaryReader = new BinaryReader(request.Files[0].InputStream))
			{
				fileArray = binaryReader.ReadBytes(request.Files[0].ContentLength);
			}
			try
			{
				using (var client = new HttpClient())
				{
					//client.DefaultRequestHeaders.Add("Connection", "Keep-Alive");
					//client.DefaultRequestHeaders.Add("Keep-Alive", "3600");
					client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
					using (var content = new MultipartFormDataContent())
					{
						var fileContent = new ByteArrayContent(fileArray);//(System.IO.File.ReadAllBytes(fileName));
						fileContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
						{
							FileName = fileName
						};
						content.Add(fileContent);
						//log.ErrorMessage("VendorRFQController", "postmethod before called", serviceUrl);
						//ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
						var result = client.PostAsync(serviceUrl, content).Result;
						//log.ErrorMessage("VendorRFQController", "postmethod After  called", serviceUrl);
					}
				}
			}
			catch (Exception e)
			{
				log.ErrorMessage("RFQController", "uploadfileToScm", e.Message.ToString() + e.InnerException.ToString() + e.ToString());
				//Log the exception
			}
			return status;
		}

		//bank gurantee
		[HttpPost]
		[Route("updateBG")]
		public IHttpActionResult updateBG(RemoteBankGuarantee bg)
		{
			return Ok(this._rfqBusenessAcess.updateBG(bg));
		}

		[HttpPost]
		[Route("getBGList")]
		public IHttpActionResult getBGList(BGfilters BGfilters)
		{
			return Ok(_rfqBusenessAcess.getBGList(BGfilters));
		}

		[HttpGet]
		[Route("getBGDetails/{BGId}")]
		public IHttpActionResult getBGDetails(int BGId)
		{
			return Ok(this._rfqBusenessAcess.getBGDetails(BGId));
		}
		[HttpGet]
		[Route("DeleteBGFile/{DocId}")]
		public IHttpActionResult DeleteBGFile(int DocId)
		{
			return Ok(_rfqBusenessAcess.DeleteBGFile(DocId));
		}

		/*
            Name of Function : <<Downloadexcel>>  Author :<<Prasanna>>  
            Date of Creation <<19-03-2021>>
            Purpose : <<Down load RFQ data in excel>>
            Review Date :<<>>   Reviewed By :<<>>
            Version : 0.1 <change version only if there is major change - new release etc>
            Sourcecode Copyright : Yokogawa India Limited
       */
		[HttpGet]
		[Route("Downloadexcel/{revisionid}")]
		public HttpResponseMessage Downloadexcel(int revisionid)
		{
			try
			{
				string sourcePath = "D:\\YILProjects\\SCM\\SCMFiles\\";
				//string sourcePath = "D:\\Excelpath\\";
				string targetpath = "D:\\YILProjects\\SCM\\SCMFiles\\";
				string srcfilename = "RFQDownloadTemplate.xlsx";
				string targetfilename = "Akil" + DateTime.Now.ToString("ddMMyyyyhhmmss") + ".xlsx";
				string sourceFile = System.IO.Path.Combine(sourcePath, srcfilename);
				string destFile = System.IO.Path.Combine(targetpath, targetfilename);
				if (!System.IO.Directory.Exists(targetpath))
				{
					System.IO.Directory.CreateDirectory(targetpath);
				}
				System.IO.File.Copy(sourceFile, destFile, false);
				Microsoft.Office.Interop.Excel._Application docExcel = new Microsoft.Office.Interop.Excel.Application();
				docExcel.Visible = false;
				docExcel.DisplayAlerts = false;
				Microsoft.Office.Interop.Excel._Workbook workbooksExcel = docExcel.Workbooks.Open(destFile, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
				Microsoft.Office.Interop.Excel._Worksheet worksheetExcel = (Microsoft.Office.Interop.Excel._Worksheet)workbooksExcel.ActiveSheet;

				YSCMEntities obj = new YSCMEntities();
				var data = obj.RfqForVendorDownloads.Where(x => x.rfqRevisionId == revisionid).ToList();
				Microsoft.Office.Interop.Excel.Range range = worksheetExcel.UsedRange;
				foreach (var item in data)
				{
					if (!string.IsNullOrEmpty(item.RFQNo))
						(range.Worksheet.Cells["2", "G"]).Value2 = item.RFQNo;
					if (!string.IsNullOrEmpty(item.rfqRevisionId.ToString()))
						(range.Worksheet.Cells["2", "O"]).Value2 = item.rfqRevisionId;
					if (!string.IsNullOrEmpty(item.RFQValidDate.ToString()))
						(range.Worksheet.Cells["3", "G"]).Value2 = item.RFQValidDate;
					if (!string.IsNullOrEmpty(item.ReqRemarks))
						(range.Worksheet.Cells["3", "L"]).Value2 = item.ReqRemarks;
					if (!string.IsNullOrEmpty(item.RFQValidDate.ToString()))
						(range.Worksheet.Cells["2", "L"]).Value2 = item.RFQValidDate;
				}

				int i = 6;
				foreach (var item1 in data)
				{
					if (!string.IsNullOrEmpty(item1.RFQItemsId.ToString()))
						(range.Worksheet.Cells[i, "A"]).Value2 = item1.RFQItemsId;
					else
						(range.Worksheet.Cells[i, "A"]).Value2 = 0;

					(range.Worksheet.Cells[i, "B"]).Value2 = i - 5;

					if (!string.IsNullOrEmpty(item1.ItemId.ToString()))
						(range.Worksheet.Cells[i, "C"]).Value2 = item1.ItemId;
					else
						(range.Worksheet.Cells[i, "C"]).Value2 = "";

					if (!string.IsNullOrEmpty(item1.ItemDescription))
						(range.Worksheet.Cells[i, "D"]).Value2 = item1.ItemDescription;
					else
						(range.Worksheet.Cells[i, "D"]).Value2 = "";

					if (!string.IsNullOrEmpty(item1.QuotationQty.ToString()))
						(range.Worksheet.Cells[i, "E"]).Value2 = item1.QuotationQty;
					else
						(range.Worksheet.Cells[i, "E"]).Value2 = 0;

					if (item1.UOM != null)
						(range.Worksheet.Cells[i, "F"]).Value2 = item1.UOM;
					else
						(range.Worksheet.Cells[i, "F"]).Value2 = 0;

					if (!string.IsNullOrEmpty(item1.CurrencyValue.ToString()))
						(range.Worksheet.Cells[i, "G"]).Value2 = item1.CurrencyValue;
					else
						(range.Worksheet.Cells[i, "G"]).Value2 = 0;

					if (!string.IsNullOrEmpty(item1.UnitPrice.ToString()))
						(range.Worksheet.Cells[i, "H"]).Value2 = item1.UnitPrice;
					else
						(range.Worksheet.Cells[i, "H"]).Value2 = 0;

					if (item1.HSNCode != null)
						(range.Worksheet.Cells[i, "I"]).Value2 = item1.HSNCode;
					else
						(range.Worksheet.Cells[i, "I"]).Value2 = 0;

					if (!string.IsNullOrEmpty(item1.DiscountPercentage.ToString()))
						(range.Worksheet.Cells[i, "J"]).Value2 = item1.DiscountPercentage;
					else
						(range.Worksheet.Cells[i, "J"]).Value2 = 0;

					if (!string.IsNullOrEmpty(item1.Discount.ToString()))
						(range.Worksheet.Cells[i, "K"]).Value2 = item1.Discount;
					else
						(range.Worksheet.Cells[i, "K"]).Value2 = 0;

					if (!string.IsNullOrEmpty(item1.VendorModelNo))
						(range.Worksheet.Cells[i, "L"]).Value2 = item1.VendorModelNo;
					else
						(range.Worksheet.Cells[i, "L"]).Value2 = "";

					if (!string.IsNullOrEmpty(item1.MfgPartNo))
						(range.Worksheet.Cells[i, "M"]).Value2 = item1.MfgPartNo;
					else
						(range.Worksheet.Cells[i, "M"]).Value2 = "";

					if (!string.IsNullOrEmpty(item1.MfgModelNo))
						(range.Worksheet.Cells[i, "N"]).Value2 = item1.MfgModelNo;
					else
						(range.Worksheet.Cells[i, "N"]).Value2 = "";

					if (!string.IsNullOrEmpty(item1.ManufacturerName))
						(range.Worksheet.Cells[i, "O"]).Value2 = item1.ManufacturerName;
					else
						(range.Worksheet.Cells[i, "O"]).Value2 = "";

					if (!string.IsNullOrEmpty(item1.CGSTPercentage.ToString()))
						(range.Worksheet.Cells[i, "P"]).Value2 = item1.CGSTPercentage;
					else
						(range.Worksheet.Cells[i, "P"]).Value2 = 0;

					if (!string.IsNullOrEmpty(item1.IGSTPercentage.ToString()))
						(range.Worksheet.Cells[i, "Q"]).Value2 = item1.IGSTPercentage;
					else
						(range.Worksheet.Cells[i, "Q"]).Value2 = 0;

					if (!string.IsNullOrEmpty(item1.SGSTPercentage.ToString()))
						(range.Worksheet.Cells[i, "R"]).Value2 = item1.SGSTPercentage;
					else
						(range.Worksheet.Cells[i, "R"]).Value2 = 0;

					if (!string.IsNullOrEmpty(item1.PFAmount.ToString()))
						(range.Worksheet.Cells[i, "S"]).Value2 = item1.PFAmount;
					else
						(range.Worksheet.Cells[i, "S"]).Value2 = 0;

					if (!string.IsNullOrEmpty(item1.PFPercentage.ToString()))
						(range.Worksheet.Cells[i, "T"]).Value2 = item1.PFPercentage;
					else
						(range.Worksheet.Cells[i, "T"]).Value2 = 0;

					if (!string.IsNullOrEmpty(item1.FreightAmount.ToString()))
						(range.Worksheet.Cells[i, "U"]).Value2 = item1.FreightAmount;
					else
						(range.Worksheet.Cells[i, "U"]).Value2 = 0;

					if (!string.IsNullOrEmpty(item1.FreightPercentage.ToString()))
						(range.Worksheet.Cells[i, "V"]).Value2 = item1.FreightPercentage;
					else
						(range.Worksheet.Cells[i, "V"]).Value2 = 0;

					if (!string.IsNullOrEmpty(item1.DeliveryDate.ToString()))
					{
						DateTime dt = DateTime.ParseExact(item1.DeliveryDate.ToString(), "dd-MM-yyyy HH:mm:ss", null);
						string date = dt.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
						(range.Worksheet.Cells[i, "W"]).Value2 = date;
					}
					else
						(range.Worksheet.Cells[i, "W"]).Value2 = "";

					if (item1.Remarks != null && !string.IsNullOrEmpty(item1.Remarks))
						(range.Worksheet.Cells[i, "X"]).Value2 = item1.Remarks;
					else
						(range.Worksheet.Cells[i, "X"]).Value2 = "";
					if (!string.IsNullOrEmpty(item1.RFQSplitItemId.ToString()))
						(range.Worksheet.Cells[i, "Y"]).Value2 = item1.RFQSplitItemId;
					else
						(range.Worksheet.Cells[i, "Y"]).Value2 = 0;
					i++;
				}
				range.Worksheet.Protect("password", Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value);

				workbooksExcel.Save();
				workbooksExcel.Close(false, Type.Missing, Type.Missing);
				docExcel.Application.DisplayAlerts = true;
				docExcel.Application.Quit();
				var result = getGenetatedExcel(destFile);
				return result;

			}
			catch (Exception ex)
			{
				throw;
			}
		}

		public HttpResponseMessage getGenetatedExcel(string filepath)
		{
			var path = filepath;
			HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
			var stream = new FileStream(path, FileMode.Open, FileAccess.Read);
			result.Content = new StreamContent(stream);
			//result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
			result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
			//stream.Dispose();
			//stream.Close();
			return (result);
		}


		/*
            Name of Function : <<UploadRfqData>>  Author :<<Prasanna>>  
            Date of Creation <<19-03-2021>>
            Purpose : <<Loading and Updating RfqData through excel>>
            Review Date :<<>>   Reviewed By :<<>>
            Version : 0.1 <change version only if there is major change - new release etc>
            Sourcecode Copyright : Yokogawa India Limited
       */
		[HttpPost]
		[Route("UploadRfqData")]
		public IHttpActionResult UploadRfqData()
		{
			try
			{
				var httpRequest = HttpContext.Current.Request;
				var serverPath = HttpContext.Current.Server.MapPath("~/SCMDocs");
				string parsedFileName = "";
				if (httpRequest.Files.Count > 0)
				{
					var Id = httpRequest.Files.AllKeys[0];
					var postedFile = httpRequest.Files[0];
					parsedFileName = string.Format(DateTime.Now.Year.ToString() + "\\" + DateTime.Now.ToString("MMM") + "\\" + Id + "\\" + ToValidFileName(postedFile.FileName));
					serverPath = serverPath + string.Format("\\" + DateTime.Now.Year.ToString() + "\\" + DateTime.Now.ToString("MMM")) + "\\" + Id;
					var filePath = Path.Combine(serverPath, ToValidFileName(postedFile.FileName));
					if (!Directory.Exists(serverPath))
						Directory.CreateDirectory(serverPath);
					postedFile.SaveAs(filePath);


					DataTable dtexcel = new DataTable();
					bool hasHeaders = false;
					string HDR = hasHeaders ? "Yes" : "No";
					string strConn;
					if (filePath.Substring(filePath.LastIndexOf('.')).ToLower() == ".xlsx")
						strConn = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + filePath + ";Extended Properties=\"Excel 12.0;HDR=" + HDR + ";IMEX=0\"";
					else
						strConn = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + filePath + ";Extended Properties=\"Excel 8.0;HDR=" + HDR + ";IMEX=0\"";

					OleDbConnection conn = new OleDbConnection(strConn);
					conn.Open();
					DataTable schemaTable = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, new object[] { null, null, null, "TABLE" });

					DataRow schemaRow = schemaTable.Rows[0];
					string sheet = schemaRow["TABLE_NAME"].ToString();
					if (!sheet.EndsWith("_"))
					{
						string query = "SELECT  * FROM [Sheet1$]";
						OleDbDataAdapter daexcel = new OleDbDataAdapter(query, conn);
						dtexcel.Locale = CultureInfo.CurrentCulture;
						daexcel.Fill(dtexcel);
					}

					conn.Close();
					int iSucceRows = 0;
					YSCMEntities obj = new YSCMEntities();
					VSCMEntities remoteObj = new VSCMEntities();

					int i = 0;
					string errorMessages = "";
					foreach (DataRow row in dtexcel.Rows)
					{

						if (i > 3 && row[0] != null && !string.IsNullOrEmpty((row[0]).ToString()))
						{
							if (string.IsNullOrEmpty(row[7].ToString()))
							{
								return Ok(new SCMModels.RFQModels.ServerSideValidation { ErrorMessage = "Unit Price Cannot be Empty At Line -" + i + " Cell No 7", ValidData = false });
							}

							if (string.IsNullOrEmpty(row[8].ToString()))
							{
								return Ok(new SCMModels.RFQModels.ServerSideValidation { ErrorMessage = "HSN Code Cannot be Empty At Line - " + i + "Cell No 8", ValidData = false });
							}
							if (string.IsNullOrEmpty(row[16].ToString()))
							{
								if (string.IsNullOrEmpty(row[15].ToString()) && string.IsNullOrEmpty(row[17].ToString()))
									return Ok(new SCMModels.RFQModels.ServerSideValidation { ErrorMessage = "CGST or SGST Percentage Cannot be Empty At Line - " + i + "Cell No 15 or 17", ValidData = false });

							}


							int itemsid = Convert.ToInt32(row[0]);
							RemoteRFQItems_N remoteRFQItems_N = remoteObj.RemoteRFQItems_N.Where(x => x.RFQItemsId == itemsid).FirstOrDefault();
							remoteRFQItems_N.ItemId = row[2].ToString();
							if (!string.IsNullOrEmpty(row[4].ToString()))
								remoteRFQItems_N.QuotationQty = Convert.ToDouble(row[4]);
							if (!string.IsNullOrEmpty(row[11].ToString()))
								remoteRFQItems_N.VendorModelNo = (row[11].ToString());
							if (!string.IsNullOrEmpty(row[12].ToString()))
								remoteRFQItems_N.MfgPartNo = row[12].ToString();
							if (!string.IsNullOrEmpty(row[13].ToString()))
								remoteRFQItems_N.MfgModelNo = row[13].ToString();

							if (!string.IsNullOrEmpty(row[14].ToString()))
								remoteRFQItems_N.ManufacturerName = row[14].ToString();

							if (!string.IsNullOrEmpty(row[15].ToString()))
								remoteRFQItems_N.CGSTPercentage = Convert.ToDecimal(row[15]);

							if (!string.IsNullOrEmpty(row[16].ToString()))
								remoteRFQItems_N.IGSTPercentage = Convert.ToDecimal(row[16]);

							if (!string.IsNullOrEmpty(row[17].ToString()))
								remoteRFQItems_N.SGSTPercentage = Convert.ToDecimal(row[17]);

							if (!string.IsNullOrEmpty(row[18].ToString()))
								remoteRFQItems_N.PFAmount = Convert.ToDecimal(row[18]);

							if (!string.IsNullOrEmpty(row[19].ToString()))
								remoteRFQItems_N.PFPercentage = Convert.ToDecimal(row[19]);

							if (!string.IsNullOrEmpty(row[20].ToString()))
								remoteRFQItems_N.FreightAmount = Convert.ToDecimal(row[20]);

							if (!string.IsNullOrEmpty(row[21].ToString()))
								remoteRFQItems_N.FreightPercentage = Convert.ToDecimal(row[21]);

							if (!string.IsNullOrEmpty(row[8].ToString()))
								remoteRFQItems_N.HSNCode = row[8].ToString();
							remoteObj.SaveChanges();

							RemoteRFQItemsInfo_N remoteitemsInfo_N = remoteObj.RemoteRFQItemsInfo_N.Where(x => x.RFQItemsId == itemsid && x.DeleteFlag == false).FirstOrDefault();
							if (remoteitemsInfo_N != null)
							{

								if (!string.IsNullOrEmpty(row[5].ToString()))
									remoteitemsInfo_N.UOM = Convert.ToInt32(row[5]);
								if (!string.IsNullOrEmpty(row[6].ToString()))
									remoteitemsInfo_N.CurrencyValue = Convert.ToDecimal(row[6]);
								if (!string.IsNullOrEmpty(row[7].ToString()))
									remoteitemsInfo_N.UnitPrice = Convert.ToDecimal(row[7]);
								if (!string.IsNullOrEmpty(row[9].ToString()))
									remoteitemsInfo_N.DiscountPercentage = Convert.ToDecimal(row[9]);
								if (!string.IsNullOrEmpty(row[10].ToString()))
									remoteitemsInfo_N.Discount = Convert.ToDecimal(row[10]);
								if (!string.IsNullOrEmpty(row[22].ToString()))
								{
									var Text = row[22].ToString();

									DateTime dt = DateTime.ParseExact(Text, "dd/MM/yyyy", null);
									remoteitemsInfo_N.DeliveryDate = dt;
								}
								if (!string.IsNullOrEmpty(row[23].ToString()))
									remoteitemsInfo_N.Remarks = row[23].ToString();
								remoteObj.SaveChanges();
							}
							else
							{
								RemoteRFQItemsInfo_N newremoteRfqItemsInfon = new RemoteRFQItemsInfo_N();
								int rfqsplititemid = obj.RFQItemsInfo_N.Max(li => li.RFQSplitItemId);
								newremoteRfqItemsInfon.RFQSplitItemId = rfqsplititemid + 1;
								if (!string.IsNullOrEmpty(row[5].ToString()))
									newremoteRfqItemsInfon.UOM = Convert.ToInt32(row[5]);
								newremoteRfqItemsInfon.RFQItemsId = itemsid;
								if (!string.IsNullOrEmpty(row[6].ToString()))
									newremoteRfqItemsInfon.CurrencyValue = Convert.ToDecimal(row[6]);

								if (!string.IsNullOrEmpty(row[7].ToString()))
									newremoteRfqItemsInfon.UnitPrice = Convert.ToDecimal(row[7]);
								if (!string.IsNullOrEmpty(row[9].ToString()))
									newremoteRfqItemsInfon.DiscountPercentage = Convert.ToDecimal(row[9]);

								if (!string.IsNullOrEmpty(row[10].ToString()))
									newremoteRfqItemsInfon.Discount = Convert.ToDecimal(row[10]);

								if (!string.IsNullOrEmpty(row[22].ToString()))
									newremoteRfqItemsInfon.DeliveryDate = Convert.ToDateTime(row[22]);
								if (!string.IsNullOrEmpty(row[23].ToString()))
									newremoteRfqItemsInfon.Remarks = row[23].ToString();
								remoteObj.RemoteRFQItemsInfo_N.Add(newremoteRfqItemsInfon);
								remoteObj.SaveChanges();

							}

							RFQItems_N rFQItems_N = obj.RFQItems_N.Where(x => x.RFQItemsId == itemsid).FirstOrDefault();

							rFQItems_N.ItemId = row[2].ToString();
							if (!string.IsNullOrEmpty(row[4].ToString()))
								rFQItems_N.QuotationQty = Convert.ToDouble(row[4]);
							if (!string.IsNullOrEmpty(row[11].ToString()))
								rFQItems_N.VendorModelNo = (row[11].ToString());
							if (!string.IsNullOrEmpty(row[12].ToString()))
								rFQItems_N.MfgPartNo = row[12].ToString();
							if (!string.IsNullOrEmpty(row[13].ToString()))
								rFQItems_N.MfgModelNo = row[13].ToString();
							if (!string.IsNullOrEmpty(row[14].ToString()))
								rFQItems_N.ManufacturerName = row[14].ToString();
							if (!string.IsNullOrEmpty(row[15].ToString()))
								rFQItems_N.CGSTPercentage = Convert.ToDecimal(row[15]);
							if (!string.IsNullOrEmpty(row[16].ToString()))
								rFQItems_N.IGSTPercentage = Convert.ToDecimal(row[16]);
							if (!string.IsNullOrEmpty(row[17].ToString()))
								rFQItems_N.SGSTPercentage = Convert.ToDecimal(row[17]);
							if (!string.IsNullOrEmpty(row[18].ToString()))
								rFQItems_N.PFAmount = Convert.ToDecimal(row[18]);
							if (!string.IsNullOrEmpty(row[19].ToString()))
								rFQItems_N.PFPercentage = Convert.ToDecimal(row[19]);
							if (!string.IsNullOrEmpty(row[20].ToString()))
								rFQItems_N.FreightAmount = Convert.ToDecimal(row[20]);
							if (!string.IsNullOrEmpty(row[21].ToString()))
								rFQItems_N.FreightPercentage = Convert.ToDecimal(row[21]);
							if (!string.IsNullOrEmpty(row[8].ToString()))
								rFQItems_N.HSNCode = row[8].ToString();
							obj.SaveChanges();

							RFQItemsInfo_N itemsInfo_N = obj.RFQItemsInfo_N.Where(x => x.RFQItemsId == itemsid && x.DeleteFlag == false).FirstOrDefault();
							if (itemsInfo_N != null)
							{
								if (!string.IsNullOrEmpty(row[5].ToString()))
									itemsInfo_N.UOM = Convert.ToInt32(row[5]);
								if (!string.IsNullOrEmpty(row[6].ToString()))
									itemsInfo_N.CurrencyValue = Convert.ToDecimal(row[6]);
								if (!string.IsNullOrEmpty(row[7].ToString()))
									itemsInfo_N.UnitPrice = Convert.ToDecimal(row[7]);
								if (!string.IsNullOrEmpty(row[9].ToString()))
									itemsInfo_N.DiscountPercentage = Convert.ToDecimal(row[9]);
								if (!string.IsNullOrEmpty(row[10].ToString()))
									itemsInfo_N.Discount = Convert.ToDecimal(row[10]);
								if (!string.IsNullOrEmpty(row[22].ToString()))
								{
									DateTime dt = DateTime.ParseExact(row[22].ToString(), "dd/MM/yyyy", CultureInfo.InvariantCulture);
									itemsInfo_N.DeliveryDate = dt;
								}
								if (!string.IsNullOrEmpty(row[23].ToString()))
									itemsInfo_N.Remarks = row[23].ToString();
								obj.SaveChanges();
							}
							else
							{
								RFQItemsInfo_N newRfqItemsInfon = new RFQItemsInfo_N();
								int rfqsplititemid = obj.RFQItemsInfo_N.Max(li => li.RFQSplitItemId);
								newRfqItemsInfon.RFQSplitItemId = rfqsplititemid + 1;
								if (!string.IsNullOrEmpty(row[5].ToString()))
									newRfqItemsInfon.UOM = Convert.ToInt32(row[5]);
								newRfqItemsInfon.RFQItemsId = itemsid;
								if (!string.IsNullOrEmpty(row[6].ToString()))
									newRfqItemsInfon.CurrencyValue = Convert.ToDecimal(row[6]);
								if (!string.IsNullOrEmpty(row[7].ToString()))
									newRfqItemsInfon.UnitPrice = Convert.ToDecimal(row[7]);
								if (!string.IsNullOrEmpty(row[9].ToString()))
									newRfqItemsInfon.DiscountPercentage = Convert.ToDecimal(row[9]);
								if (!string.IsNullOrEmpty(row[10].ToString()))
									newRfqItemsInfon.Discount = Convert.ToDecimal(row[10]);
								if (!string.IsNullOrEmpty(row[22].ToString()))
								{
									DateTime dt = DateTime.ParseExact(row[22].ToString(), "dd/MM/yyyy", CultureInfo.InvariantCulture);
									itemsInfo_N.DeliveryDate = dt;
								}
								if (!string.IsNullOrEmpty(row[23].ToString()))
									newRfqItemsInfon.Remarks = row[23].ToString();
								obj.RFQItemsInfo_N.Add(newRfqItemsInfon);
								obj.SaveChanges();

							}
							var mPRItemInfo = obj.MPRItemInfoes.Join(obj.RFQItems_N, mpr => mpr.Itemdetailsid, rfq => rfq.MPRItemDetailsid, (mpr, rfq) => new { mpr, rfq }).Where(mpritem => mpritem.rfq.RFQItemsId == itemsid).FirstOrDefault();
							MPRItemInfo mpritemsinfor = obj.MPRItemInfoes.Where(x => x.Itemdetailsid == mPRItemInfo.mpr.Itemdetailsid).FirstOrDefault();
							if (!string.IsNullOrEmpty(row[3].ToString()))
								mpritemsinfor.ItemDescription = row[3].ToString();
							obj.SaveChanges();
						}
						i++;
					}


					int succRecs = iSucceRows;
				}
				return Ok(parsedFileName);

			}
			catch (Exception e)
			{
				return Ok(e);
			}


		}

		/*Name of Function : <<RfqIteminfoDeleteByidformultiple>>  Author :<<Prasanna>>  
	    Date of Creation <<21-10-2020>>
	    Purpose : <<RfqIteminfoDeleteByidformultiple  >>
	    Review Date :<<>>   Reviewed By :<<>>*/
		[HttpGet]
		[Route("deleteRFQFormatFile")]
		public IHttpActionResult deleteRFQFormatFile()
		{

			string partialName = "Akil";
			DirectoryInfo hdDirectoryInWhichToSearch = new DirectoryInfo(@"D:\\YILProjects\\SCM\\SCMFiles\\");
			FileInfo[] filesInDir = hdDirectoryInWhichToSearch.GetFiles("*" + partialName + "*.*");
			foreach (FileInfo file in filesInDir)
			{
				try
				{
					var stream = new FileStream(file.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
					stream.Dispose();
					stream.Close();
					File.Delete(file.FullName); 
				}
				catch (Exception e)
				{
					continue;
				}
			}

			return Ok(true);
		}
	}

}
