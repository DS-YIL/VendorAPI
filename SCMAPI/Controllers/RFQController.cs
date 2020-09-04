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
	[Authorize]
	[RoutePrefix("Api/RFQ")]
	public class RFQController : ApiController
	{
		private readonly IRFQBA _rfqBusenessAcess;
		public RFQController(IRFQBA rfqBA)
		{
			this._rfqBusenessAcess = rfqBA;
		}

		[Route("GetItemsByRevisionId/{id}")]
		[ResponseType(typeof(List<RfqItemModel>))]
		public async Task<IHttpActionResult> GetItemsByRevisionId(int id)
		{
			List<RfqItemModel> status = new List<RfqItemModel>();
			status = await _rfqBusenessAcess.GetItemsByRevisionId(id);
			return Ok(status);
		}

		[Route("UpdateRfqRevision")]
		[ResponseType(typeof(statuscheckmodel))]
		public async Task<IHttpActionResult> UpdateRfqRevision(RfqRevisionModel model)
		{
			statuscheckmodel status = new statuscheckmodel();
			status = await _rfqBusenessAcess.UpdateRfqRevision(model);
			return Ok(status);
		}
		[Route("UpdateRfqItemByBulk")]
		[ResponseType(typeof(statuscheckmodel))]
		public async Task<IHttpActionResult> UpdateRfqItemByBulk(RfqItemModel model)
		{
			statuscheckmodel status = new statuscheckmodel();
			status = await _rfqBusenessAcess.UpdateRfqItemByBulk(model);
			return Ok(status);
		}

		[Route("UpdateSingleRfqItem")]
		[ResponseType(typeof(statuscheckmodel))]
		public async Task<IHttpActionResult> UpdateSingleRfqItem(RfqItemModel model)
		{
			statuscheckmodel status = new statuscheckmodel();
			status = await _rfqBusenessAcess.UpdateSingleRfqItem(model);
			return Ok(status);
		}

		[Route("UpdateBulkRfqRevision")]
		[ResponseType(typeof(statuscheckmodel))]
		public async Task<IHttpActionResult> UpdateBulkRfqRevision(RfqRevisionModel model)
		{
			statuscheckmodel status = new statuscheckmodel();
			status = await _rfqBusenessAcess.UpdateBulkRfqRevision(model);
			return Ok(status);
		}

		[Route("DeleteRfqItemById")]
		[ResponseType(typeof(statuscheckmodel))]
		public IHttpActionResult DeleteRfqItemById(int id)
		{
			statuscheckmodel status = new statuscheckmodel();
			status = _rfqBusenessAcess.DeleteRfqItemById(id);
			return Ok();
		}

		[Route("DeleteBulkItemsByItemId")]
		[ResponseType(typeof(statuscheckmodel))]
		public IHttpActionResult DeleteBulkItemsByItemId(List<int> id)
		{
			statuscheckmodel status = new statuscheckmodel();
			status = _rfqBusenessAcess.DeleteBulkItemsByItemId(id);
			return Ok();
		}

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
				string RFQRevId, RFQItemId, updatedBy, updatedRevisionId;
				foreach (string file in httpRequest.Files)
				{
					filename = file;
					break;
				}

				listdata = filename.Split('_');
				RFQRevId = listdata[0];
				RFQItemId = listdata[1];
				updatedBy = listdata[2];

				for (int i = 0; i <= httpRequest.Files.Count - 1; i++)
				{
					//string url = "10.29.15.183:90/Api/mpr/uploadfile";
					var postedFile = httpRequest.Files[i];
					filePath = HttpContext.Current.Server.MapPath("~/VSCMDocs") + string.Format("\\" + DateTime.Now.Year.ToString() + "\\" + DateTime.Now.ToString("MMM")) + "\\" + "\\" + ToValidFileName(filename);
					//  fileserverpath= ConfigurationManager.AppSettings["AttachedDocPathForServer"] + "\\" + filename;
					string dbfilePath = filename + "\\" + postedFile.FileName;
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
						eachobj.DocumentType = 1;
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
			return Json(listobj);
		}


		[Route("GetItemsByItemId")]
		[ResponseType(typeof(RfqItemModel))]
		public async Task<IHttpActionResult> GetItemsByItemId(int id)
		{
			RfqItemModel status = new RfqItemModel();
			status = await _rfqBusenessAcess.GetItemsByItemId(id);
			return Ok(status);
		}
		[Route("GetAllvendorList")]
		[ResponseType(typeof(List<VendormasterModel>))]
		public IHttpActionResult GetAllvendorList()
		{
			List<VendormasterModel> status = new List<VendormasterModel>();
			status = _rfqBusenessAcess.GetAllvendorList();
			return Ok(status);
		}

		[Route("GetRfqDetailsById/{RevisionId}")]
		//[ResponseType(typeof(RfqRevisionModel))]
		public async Task<IHttpActionResult> GetRfqDetailsById(int RevisionId)
		{
			//RfqRevisionModel revision = new RfqRevisionModel();
			//revision = await _rfqBusenessAcess.GetRfqDetailsById(RevisionId);
			return Ok(await _rfqBusenessAcess.GetRfqDetailsById(RevisionId));
		}

		[Route("InsertRfqItemInfo")]
		[ResponseType(typeof(statuscheckmodel))]
		public IHttpActionResult InsertRfqItemInfo(RfqItemModel model)
		{
			statuscheckmodel status = new statuscheckmodel();

			return Json(this._rfqBusenessAcess.InsertRfqItemInfo(model));
		}
		[Route("editRfqItemInfo")]
		[ResponseType(typeof(statuscheckmodel))]
		public IHttpActionResult editRfqItemInfo(RfqItemModel model)
		{
			statuscheckmodel status = new statuscheckmodel();

			return Json(this._rfqBusenessAcess.editRfqItemInfo(model));
		}

		[HttpGet]
		[Route("RfqIteminfoDeleteByid/{id}/{rfqitemid}")]
		//  [ResponseType(typeof(statuscheckmodel))]
		public async Task<IHttpActionResult> DeleteRfqIteminfoByid(int id, int rfqitemid)
		{
			statuscheckmodel status = new statuscheckmodel();
			status = await _rfqBusenessAcess.DeleteRfqIteminfoByid(id, rfqitemid);
			return Ok(status);
		}
		[HttpGet]
		[Route("RfqIteminfoDeleteByidformultiple/{id}/{BOMId}")]
		//  [ResponseType(typeof(statuscheckmodel))]
		public async Task<IHttpActionResult> DeleteRfqIteminfoByidformultiple(int id, int BOMId)
		{
			statuscheckmodel status = new statuscheckmodel();
			status = await _rfqBusenessAcess.DeleteRfqIteminfoByidformultiple(id, BOMId);
			return Ok(status);
		}
		[Route("DeleteRfqitemandinfosById/{id}")]
		// [ResponseType(typeof(statuscheckmodel))]
		public async Task<IHttpActionResult> DeleteRfqitemandinfosById(int id)
		{
			statuscheckmodel status = new statuscheckmodel();
			status = await _rfqBusenessAcess.DeleteRfqitemandinfosById(id);
			return Ok(status);
		}
		[Route("UpdateRfqItemInfoById")]
		[ResponseType(typeof(statuscheckmodel))]
		public async Task<IHttpActionResult> UpdateRfqItemInfoById(RfqItemInfoModel model)
		{
			statuscheckmodel status = new statuscheckmodel();
			status = await _rfqBusenessAcess.UpdateRfqItemInfoById(model);
			return Ok(status);
		}

		[Route("GetRfqItemByMPrId")]
		[ResponseType(typeof(RfqItemModel))]
		public async Task<IHttpActionResult> GetRfqItemByMPrId(int id)
		{
			RfqItemModel status = new RfqItemModel();
			status = await _rfqBusenessAcess.GetRfqItemByMPrId(id);
			return Ok(status);
		}

		[Route("InsertSingleIteminfos")]
		[ResponseType(typeof(statuscheckmodel))]
		public async Task<IHttpActionResult> InsertSingleIteminfos(RfqItemInfoModel model)
		{
			statuscheckmodel status = new statuscheckmodel();
			status = await _rfqBusenessAcess.InsertSingleIteminfos(model);
			return Ok(status);
		}

		[Route("InsertBulkItemInfos")]
		[ResponseType(typeof(statuscheckmodel))]
		public async Task<IHttpActionResult> InsertBulkItemInfos(List<RfqItemInfoModel> model)
		{
			statuscheckmodel status = new statuscheckmodel();
			status = await _rfqBusenessAcess.InsertBulkItemInfos(model);
			return Ok(status);
		}
		[HttpGet]
		[Route("GetUnitMasterList")]
		// [ResponseType(typeof(List<UnitMasterModel>))]
		public async Task<IHttpActionResult> GetUnitMasterList()
		{
			List<UnitMasterModel> model = new List<UnitMasterModel>();
			model = await _rfqBusenessAcess.GetUnitMasterList();
			return Ok(model);
		}
		[Route("GetAllMPRBuyerGroups")]
		[ResponseType(typeof(List<MPRBuyerGroupModel>))]
		public async Task<IHttpActionResult> GetAllMPRBuyerGroups()
		{
			List<MPRBuyerGroupModel> model = new List<MPRBuyerGroupModel>();
			model = await _rfqBusenessAcess.GetAllMPRBuyerGroups();
			return Ok(model);
		}
		[Route("InsertBuyerGroup")]
		[ResponseType(typeof(statuscheckmodel))]
		public async Task<IHttpActionResult> InsertBuyerGroup(MPRBuyerGroupModel model)
		{
			statuscheckmodel status = new statuscheckmodel();
			status = await _rfqBusenessAcess.InsertMprBuyerGroups(model);
			return Ok(status);
		}
		[Route("UpdateMprBuyerGroups")]
		[ResponseType(typeof(statuscheckmodel))]
		public async Task<IHttpActionResult> UpdateMprBuyerGroups(MPRBuyerGroupModel model)
		{
			statuscheckmodel status = new statuscheckmodel();
			status = await _rfqBusenessAcess.UpdateMprBuyerGroups(model);
			return Ok(status);
		}
		[Route("GetMPRApprovalsById/{id}")]
		[ResponseType(typeof(MPRApproverModel))]
		public async Task<IHttpActionResult> GetMPRApprovalsById(int id)
		{
			MPRApproverModel model = new MPRApproverModel();
			model = await _rfqBusenessAcess.GetMPRApprovalsById(id);
			return Ok(model);
		}

		[Route("InsertMPRApprover")]
		[ResponseType(typeof(statuscheckmodel))]
		public async Task<IHttpActionResult> InsertMPRApprover(MPRApproverModel model)
		{
			statuscheckmodel status = new statuscheckmodel();
			status = await _rfqBusenessAcess.InsertMPRApprover(model);
			return Ok(status);
		}

		[Route("GetMPRBuyerGroupsById")]
		[ResponseType(typeof(MPRBuyerGroupModel))]
		public async Task<IHttpActionResult> GetMPRBuyerGroupsById(int id)
		{
			MPRBuyerGroupModel status = new MPRBuyerGroupModel();
			status = await _rfqBusenessAcess.GetMPRBuyerGroupsById(id);
			return Ok(status);
		}

		[Route("GetAllMPRApprovals")]
		[ResponseType(typeof(List<MPRApproverModel>))]
		public async Task<IHttpActionResult> GetAllMPRApprovals()
		{
			List<MPRApproverModel> model = new List<MPRApproverModel>();
			model = await _rfqBusenessAcess.GetAllMPRApprovals();
			return Ok(model);
		}
		[Route("GetAllMPRDepartments")]
		[ResponseType(typeof(List<MPRDepartmentModel>))]
		public async Task<IHttpActionResult> GetAllMPRDepartments()
		{
			List<MPRDepartmentModel> model = new List<MPRDepartmentModel>();
			model = await _rfqBusenessAcess.GetAllMPRDepartments();
			return Ok(model);
		}
		[Route("GetMPRDepartmentById/{id}")]
		[ResponseType(typeof(MPRDepartmentModel))]
		public async Task<IHttpActionResult> GetMPRDepartmentById(int id)
		{
			MPRDepartmentModel model = new MPRDepartmentModel();
			model = await _rfqBusenessAcess.GetMPRDepartmentById(id);
			return Ok(model);
		}
		[Route("GetAllMPRDepartments")]
		[ResponseType(typeof(List<MPRDispatchLocationModel>))]
		public async Task<IHttpActionResult> GetAllMPRDispatchLocations()
		{
			List<MPRDispatchLocationModel> model = new List<MPRDispatchLocationModel>();
			model = await _rfqBusenessAcess.GetAllMPRDispatchLocations();
			return Ok(model);
		}
		[Route("GetMPRDispatchLocationById/{id}")]
		[ResponseType(typeof(MPRDispatchLocationModel))]
		public async Task<IHttpActionResult> GetMPRDispatchLocationById(int id)
		{
			MPRDispatchLocationModel model = new MPRDispatchLocationModel();
			model = await _rfqBusenessAcess.GetMPRDispatchLocationById(id);
			return Ok(model);
		}
		[Route("GetAllCustomDuty")]
		[ResponseType(typeof(List<MPRCustomsDutyModel>))]
		public async Task<IHttpActionResult> GetAllCustomDuty()
		{
			List<MPRCustomsDutyModel> model = new List<MPRCustomsDutyModel>();
			model = await _rfqBusenessAcess.GetAllCustomDuty();
			return Ok(model);
		}
		[Route("InsertYILTerms")]
		[ResponseType(typeof(statuscheckmodel))]
		public async Task<IHttpActionResult> InsertYILTerms(YILTermsandConditionModel model)
		{
			statuscheckmodel status = new statuscheckmodel();
			status = await _rfqBusenessAcess.InsertYILTerms(model);
			return Ok(model);
		}
		[Route("InsertYILTermsGroup")]
		[ResponseType(typeof(statuscheckmodel))]
		public async Task<IHttpActionResult> InsertYILTermsGroup(YILTermsGroupModel model)
		{
			statuscheckmodel status = new statuscheckmodel();
			status = await _rfqBusenessAcess.InsertYILTermsGroup(model);
			return Ok(model);
		}
		[Route("InsertRFQTerms")]
		[ResponseType(typeof(statuscheckmodel))]
		public async Task<IHttpActionResult> InsertRFQTerms(RFQTermsModel model)
		{
			statuscheckmodel status = new statuscheckmodel();
			status = await _rfqBusenessAcess.InsertRFQTerms(model);
			return Ok(model);
		}
		[Route("UpdateRFQTerms")]
		[ResponseType(typeof(statuscheckmodel))]
		public async Task<IHttpActionResult> UpdateRFQTerms(RFQTermsModel model)
		{
			statuscheckmodel status = new statuscheckmodel();
			status = await _rfqBusenessAcess.UpdateRFQTerms(model);
			return Ok(status);
		}
		[Route("GetRfqTermsById")]
		[ResponseType(typeof(RFQTermsModel))]
		public async Task<IHttpActionResult> GetRfqTermsById(int id)
		{
			RFQTermsModel status = new RFQTermsModel();
			status = await _rfqBusenessAcess.GetRfqTermsById(id);
			return Ok(status);
		}
		[Route("GetYILTermsByBuyerGroupID")]
		[ResponseType(typeof(YILTermsandConditionModel))]
		public async Task<IHttpActionResult> GetYILTermsByBuyerGroupID(int id)
		{
			YILTermsandConditionModel status = new YILTermsandConditionModel();
			status = await _rfqBusenessAcess.GetYILTermsByBuyerGroupID(id);
			return Ok(status);
		}
		[Route("GetYILTermsGroupById")]
		[ResponseType(typeof(YILTermsGroupModel))]
		public async Task<IHttpActionResult> GetYILTermsGroupById(int id)
		{
			YILTermsGroupModel status = new YILTermsGroupModel();
			status = await _rfqBusenessAcess.GetYILTermsGroupById(id);
			return Ok(status);
		}

		[HttpPost]
		[Route("VendorRegister")]
		public IHttpActionResult VendorRegistration(VendorRegistrationModel obj)
		{
			return Json(this._rfqBusenessAcess.InsertVendordata(obj));
		}
		[HttpGet]
		[Route("GetStateList")]
		public IHttpActionResult GetStateNameList()
		{
			return Ok(this._rfqBusenessAcess.StateNameList());
		}
		[HttpGet]
		[Route("GetNaturOfBusiness")]
		public IHttpActionResult GetNatureOfBusiness()
		{
			return Ok(this._rfqBusenessAcess.natureOfBusinessesList());
		}
		[Route("UploadFile")]
		[HttpPost]
		public IHttpActionResult UploadFile()
		{
			var filePath = "";
			string dbfilePath = "";
			List<RemoteVendorRegisterDocumentDetail> listobj = new List<RemoteVendorRegisterDocumentDetail>();
			var httpRequest = HttpContext.Current.Request;
			var serverPath = HttpContext.Current.Server.MapPath("~/VSCMDocs");
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
			fileName = fileName.ToLower().Replace(" ", "_").Replace("(", "_").Replace(")", "_").Replace("&", "_").Replace("*", "_").Replace("-", "_");
			return string.Join("_", fileName.Split(Path.GetInvalidFileNameChars()));
		}

		[HttpGet]
		[Route("GetDocumentTypeList")]
		public IHttpActionResult GetDocumentsList()
		{
			return Ok(this._rfqBusenessAcess.DocumentMasterList());
		}
		[HttpPost]
		[Route("SaveVendorDetails")]
		public IHttpActionResult SaveVendorData(VendorRegistrationModel model)
		{
			return Ok(this._rfqBusenessAcess.SaveVendorDetails(model));
		}
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
		[HttpPost]
		[Route("deleteRegAttachedfile")]
		public IHttpActionResult deleteRegAttachedfile(documentDetails model)
		{
			return Ok(this._rfqBusenessAcess.DeletefileAttached(model));
		}
		[HttpPost]
		[Route("deleteAttachedDocuments")]
		public IHttpActionResult deleteattacheddocument(documentDetails model)
		{
			var path1 = model.PhysicalPath.Replace("\",\"", "\\");
			var eachobj = new RFQDocument();
			string[] listofdata = { };
			string filename = model.PhysicalPath;

			//string lastWord = parts[0];
			string[] path = model.PhysicalPath.Split('\\');
			string[] parts = path[2].Split('_');
			//listofdata = lastWord.Split(',');
			//eachobj.Path = ConfigurationManager.AppSettings["AttachedDocPath"] + "\\" + parts[0] + "_" + parts[1] + "\\" + model.PhysicalPath;
			eachobj.Path = model.PhysicalPath;
			if (parts[0].Contains("Technical"))
			{
				parts[0] = parts[0].Replace("Technical", "");
				eachobj.DocumentType = 1;
				eachobj.rfqItemsid = Convert.ToInt32(parts[0]);
			}
			else
			{
				eachobj.DocumentType = Convert.ToInt32(parts[1]);
			}

			eachobj.rfqRevisionId = Convert.ToInt32(parts[0]);
			//string uploadedby = parts[2];
			eachobj.UploadedBy = parts[2];
			eachobj.DocumentName = path[3];//model.PhysicalPath;

			return Ok(this._rfqBusenessAcess.DeletefileAttachedforDocuments(eachobj));
		}
		[HttpPost]
		[Route("GetRfqByVendorId")]
		public IHttpActionResult GetRfqByVendorId(rfqFilterParams obj)
		{
			return Ok(this._rfqBusenessAcess.GetRfqByVendorId(obj));
		}
		[HttpGet]
		[Route("GetAllMasterCurrency")]
		[ResponseType(typeof(List<CurrencyMasterModel>))]
		public async Task<IHttpActionResult> GetAllMaster()
		{
			List<CurrencyMasterModel> model = new List<CurrencyMasterModel>();
			model = await this._rfqBusenessAcess.GetAllMasterCurrency();
			return Ok(model);
		}
		[HttpGet]
		[Route("GetAllRFQTerms/{RFQRevisionId}")]
		public IHttpActionResult GetTermsMasterData(int RFQRevisionId)
		{
			return Ok(this._rfqBusenessAcess.GetTermMaster(RFQRevisionId));
		}
		[HttpPost]
		[Route("RFQTermUpdate")]
		public IHttpActionResult UpdateVendorTerms(List<RFQTerms> obj)
		{

			return Json(this._rfqBusenessAcess.UpdateVendorTerms(obj));
		}
		[HttpGet]
		[Route("GetDocumentMasterList")]
		public IHttpActionResult GetMasterListData()
		{
			return Ok(this._rfqBusenessAcess.GetMasterDocumentTypeList());

		}
		[HttpPost]
		[Route("InsertVendorCommunication")]
		public IHttpActionResult InsertRFQVendorCommunication(VendorCommunicaton modelobj)
		{
			return Ok(this._rfqBusenessAcess.UpdateVendorCommunication(modelobj));
		}
		[HttpGet]
		[Route("GetCommunicationListForRFQRevisionId/{RFQRevisionId}")]
		public IHttpActionResult GetRfqCommunicationForRFQRevId(int RFQRevisionId)
		{
			return Ok(this._rfqBusenessAcess.GetVendorCommunicationForRFQRevId(RFQRevisionId));
		}
		[HttpGet]
		[Route("GetTermsByRFQrevisionId/{RFQRevisionId}")]
		public IHttpActionResult GetTermsRFQRevId(int RFQRevisionId)
		{
			return Ok(this._rfqBusenessAcess.GetTermsByRfqRevisionId(RFQRevisionId));
		}
		[HttpPost]
		[Route("savedatafromexcelToDB")]
		public IHttpActionResult exceldata()
		{
			try
			{
				var httpRequest = HttpContext.Current.Request;
				if (httpRequest.Files.Count > 0)
				{
					var filePath = "";
					//  filePath = "C://Users//464_0095//Desktop//New folder//Testing//testingfordoc.xlsx";
					var postedFile = httpRequest.Files[0];
					filePath = ConfigurationManager.AppSettings["AttachedDocPath"] + "\\" + postedFile.FileName;

					if (!Directory.Exists(filePath))
						Directory.CreateDirectory(filePath);
					filePath = Path.Combine(filePath, postedFile.FileName);
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
					YSCMEntities entities = new YSCMEntities();
					foreach (DataRow row in dtexcel.Rows)
					{
						string unitname = row["UnitId"].ToString();
						var data = entities.UnitMasters.Where(x => x.UnitName == unitname).FirstOrDefault();
						entities.MPRItemInfoes.Add(new MPRItemInfo
						{
							//CompanyCode = row["Company Code"].ToString(),
							ItemDescription = row["ItemDescription"].ToString(),
							RevisionId = 3330,
							Quantity = Convert.ToInt32(row["Quantity"]),
							SOLineItemNo = row["SOLineItemNo"].ToString(),
							TargetSpend = Convert.ToInt32(row["TargetSpend"]),
							MfgPartNo = row["MfgPartNo"].ToString(),
							MfgModelNo = row["MfgModelNo"].ToString(),
							ReferenceDocNo = row["ReferenceDocNo"].ToString(),
							UnitId = data.UnitId,
							Itemid = row["Itemid"].ToString(),


						});
						iSucceRows++;


					}

					entities.SaveChanges();
					int succRecs = iSucceRows;
				}
				return Ok();

			}
			catch (Exception e)
			{
				throw;
			}
		}
		//This resource is For all types of role

		[HttpGet]
		[Route("Vendordetails")]
		public IHttpActionResult GetResource1()
		{
			var identity = (ClaimsIdentity)User.Identity;
			var VendorId = identity.Claims
					  .FirstOrDefault(c => c.Type == "VendorId").Value;
			var Vuniqueid = identity.Claims
					  .FirstOrDefault(c => c.Type == "VuniqueId").Value;
			var VendorCode = identity.Claims
					  .FirstOrDefault(c => c.Type == "VendorCode").Value;

			List<VendorModel> listobj = new List<VendorModel>();
			VendorModel eachobj = new VendorModel();
			eachobj.UserName = identity.Name;
			eachobj.vendorId = Convert.ToInt32(VendorId);
			eachobj.VUniqueId = Vuniqueid;
			eachobj.VendorCode = VendorCode;
			listobj.Add(eachobj);
			return Ok(eachobj);
		}

		//This resource is only For Admin and SuperAdmin role

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

		//This resource is only For SuperAdmin role

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
		[HttpGet]
		[Route("getdocumentbyrevid/{revisionid}")]
		public IHttpActionResult getdocumentsbyRevid(int revisionid)

		{
			return Ok(this._rfqBusenessAcess.GetRfqdocumentdetailsById(revisionid));
		}
		[HttpGet]
		[Route("getdocumentbyrevid/{revisionid}/{rfqitemsid}")]
		public IHttpActionResult getdocumentsbyRevid(int revisionid, int rfqitemsid)
		{
			return Ok(this._rfqBusenessAcess.GetRfqdocumentdetailsById(revisionid, rfqitemsid));
		}
		[HttpGet]
		[Route("Getvendordetails/{vendorid}")]
		public IHttpActionResult getvendordetails(int vendorid)
		{
			return Ok(this._rfqBusenessAcess.GetVendorDetails(vendorid));
		}
		[HttpPost]
		[Route("changepassword")]
		public IHttpActionResult changepassword(Changepassword model)
		{
			return Ok(this._rfqBusenessAcess.changepassword(model));
		}
		[HttpGet]
		[Route("finalsubmitfromVendor/{RFQRevisionId}")]
		public IHttpActionResult finalsubmitfromVendor(int RFQRevisionId)
		{
			VSCMEntities vscm = new VSCMEntities();
			YSCMEntities obj = new YSCMEntities();
			Boolean check = false;

			//if (check == true)
			//{

			RemoteRFQStatu statusobj = new RemoteRFQStatu();
			statusobj.RfqRevisionId = RFQRevisionId;
			statusobj.StatusId = 8;
			statusobj.DeleteFlag = false;
			statusobj.updatedDate = System.DateTime.Now;
			vscm.RemoteRFQStatus.Add(statusobj);
			vscm.SaveChanges();
			int rfqstatusid = statusobj.RfqStatusId;
			RFQStatu statusobjs = new RFQStatu();
			statusobjs.RfqStatusId = rfqstatusid;
			statusobjs.RfqRevisionId = RFQRevisionId;
			statusobjs.StatusId = 8;
			statusobjs.DeleteFlag = false;
			statusobjs.updatedDate = System.DateTime.Now;
			obj.RFQStatus.Add(statusobjs);

			obj.SaveChanges();
			check = this._rfqBusenessAcess.sendemailfromvendor(RFQRevisionId);
			//  }
			return Json(check);
		}
		[HttpGet]
		[Route("checkrfqitemsid")]
		public IHttpActionResult checkrfqitemsid(int rfqitemsid)
		{
			return Ok(this._rfqBusenessAcess.checkrfqitemexists(rfqitemsid));
		}


	}

}
