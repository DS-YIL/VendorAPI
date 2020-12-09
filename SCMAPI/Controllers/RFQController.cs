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
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
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
			fileName = fileName.ToLower().Replace(" ", "_").Replace("(", "_").Replace(")", "_").Replace("&", "_").Replace("*", "_").Replace("-", "_").Replace("+", "_");
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

	}

}
