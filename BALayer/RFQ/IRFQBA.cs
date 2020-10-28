using SCMModels.MPRMasterModels;
using SCMModels.RFQModels;
using SCMModels.SCMModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Data;
using SCMModels;
using SCMModels.RemoteModel;

namespace BALayer.RFQ
{
	public interface IRFQBA
	{
		
		//Task<List<RfqRevisionModel>> GetAllRFQs();
		Task<RemoteRFQRevisions_N> GetRfqDetailsById(int revisionId);
		Task<statuscheckmodel> UpdateRfqRevision(RfqRevisionModel model);
		Task<statuscheckmodel> UpdateRfqItemByBulk(RfqItemModel model);
		Task<statuscheckmodel> UpdateSingleRfqItem(RfqItemModel model);
		Task<statuscheckmodel> UpdateBulkRfqRevision(RfqRevisionModel model);
		
		statuscheckmodel DeleteRfqRevisionbyId(int id);
		statuscheckmodel DeleteRfqItemById(int id);
		statuscheckmodel DeleteBulkItemsByItemId(List<int> id);
		List<RfqDocumentsModel> InsertDocument(RfqDocumentsModel model);
		List<RfqDocumentsModel> InsertDocumentTOYSCM(RfqDocumentsModel model);
		Task<List<RfqItemModel>> GetItemsByRevisionId(int revisionid);
		
		Task<RfqItemModel> GetItemsByItemId(int id);
		List<VendormasterModel> GetAllvendorList();	
		
		List<RfqItemModel> InsertOrEditRfqItemInfo(RfqItemModel model);
		Task<statuscheckmodel> DeleteRfqIteminfoByid(int id, int DeleteRfqIteminfoByid);
		Task<statuscheckmodel> DeleteRfqitemandinfosById(int id);
		Task<statuscheckmodel> UpdateRfqItemInfoById(RfqItemInfoModel model);
		Task<RfqItemModel> GetRfqItemByMPrId(int id);
		Task<statuscheckmodel> InsertSingleIteminfos(RfqItemInfoModel model);
		Task<statuscheckmodel> InsertBulkItemInfos(List<RfqItemInfoModel> model);
		Task<List<UnitMasterModel>> GetUnitMasterList();
		Task<statuscheckmodel> InsertRfqRemainder(RfqRemainderTrackingModel model);
		Task<RfqRemainderTrackingModel> getrfqremaindersById(int id);
		Task<statuscheckmodel> Insertrfqvendorterms(RfqVendorTermModel model);
		Task<RfqVendorTermModel> getRfqVendorById(int id);
		Task<statuscheckmodel> RemoveRfqVendorTermsById(int id);
		Task<statuscheckmodel> RemoveVendorRfqByid(int id);
		Task<statuscheckmodel> InsertNewCurrencyMaster(CurrencyMasterModel model);
		Task<statuscheckmodel> UpdateNewCurrencyMaster(CurrencyMasterModel model);
		Task<statuscheckmodel> InsertCurrentCurrencyHistory(CurrencyHistoryModel model);
		Task<statuscheckmodel> UpdateCurrentCurrencyHistory(CurrencyHistoryModel model);
		Task<List<CurrencyMasterModel>> GetAllMasterCurrency();
		Task<CurrencyMasterModel> GetMasterCurrencyById(int currencyId);
		Task<statuscheckmodel> RemoveMasterCurrencyById(int currencyId);
		Task<CurrencyHistoryModel> GetcurrencyHistoryById(int currencyId);
		Task<MPRBuyerGroupModel> GetMPRBuyerGroupsById(int id);
		Task<List<MPRBuyerGroupModel>> GetAllMPRBuyerGroups();
		Task<MPRApproverModel> GetMPRApprovalsById(int id);
		
		Task<statuscheckmodel> InsertMprBuyerGroups(MPRBuyerGroupModel model);
		Task<statuscheckmodel> UpdateMprBuyerGroups(MPRBuyerGroupModel model);
		Task<statuscheckmodel> InsertMPRApprover(MPRApproverModel model);
		Task<List<MPRApproverModel>> GetAllMPRApprovals();
		Task<List<MPRDepartmentModel>> GetAllMPRDepartments();
		Task<MPRDepartmentModel> GetMPRDepartmentById(int id);
		Task<List<MPRDispatchLocationModel>> GetAllMPRDispatchLocations();
		Task<MPRDispatchLocationModel> GetMPRDispatchLocationById(int id);
		Task<List<MPRCustomsDutyModel>> GetAllCustomDuty();
		Task<statuscheckmodel> InsertYILTerms(YILTermsandConditionModel model);
		Task<statuscheckmodel> InsertYILTermsGroup(YILTermsGroupModel model);
		Task<statuscheckmodel> InsertRFQTerms(RFQTermsModel model);
		Task<statuscheckmodel> UpdateRFQTerms(RFQTermsModel model);
		Task<YILTermsandConditionModel> GetYILTermsByBuyerGroupID(int id);
		Task<YILTermsGroupModel> GetYILTermsGroupById(int id);
		Task<RFQTermsModel> GetRfqTermsById(int termsid);
		Task<RfqItemModel> GetItemByItemId(int id);
		Task<List<V_RFQList>> GetRfqByVendorId(rfqFilterParams obj);
		List<VendorRegistrationModel> InsertVendordata(VendorRegistrationModel obj);
		List<StateList> StateNameList();
		List<NatureOfBusiness> natureOfBusinessesList();
		List<DocumentType> DocumentMasterList();
		List<RemoteVendorRegisterDocumentDetail> InsertDocuments(List<documentDetails> obj);
		RemoteVendorRegisterMaster SaveVendorDetails(VendorRegistrationModel model);
		bool DeletefileAttached(documentDetails model);
		List<RemoteRfqTerm> GetTermMaster(int rfqrevisionId);
		List<RemoteRfqTerm> UpdateVendorTerms(List<RemoteRfqTerm> obj);
		List<RemoteDocumentTypeMaster> GetMasterDocumentTypeList();
		List<RemoteRFQCommunication> UpdateVendorCommunication(VendorCommunicaton model);
		bool DeletefileAttachedforDocuments(RFQDocument obj);
		List<VendorCommunicaton> GetVendorCommunicationForRFQRevId(int RFQRevevisionId);
		List<RfqItemModel> editRfqItemInfo(RfqItemModel model);
		List<RFQTerms> GetTermsByRfqRevisionId(int RfqRevisionId);
		List<RemoteRFQDocument> GetRfqdocumentdetailsById(int revisionId);
		List<RemoteRFQDocument> GetRfqdocumentdetailsById(int revisionId, int rfqitemsid);
		VendorRegistrationModel GetVendorDetails(int vendorId);
		string changepassword(Changepassword obj);
		bool sendemailfromvendor(int RFQRevisionId);
		Task<statuscheckmodel> DeleteRfqIteminfoByidformultiple(int id, int BOMid);
		bool checkemail(Changepassword emailId);
		bool sendLinkForForgetPassword(forgetpassword email);
		string Resetpassword(forgetpassword model);
		bool CheckLinkExpiryOrNot(forgetpassword model);
		bool checkrfqitemexists(int rfqitemsid);
		DataTable getDBMastersList(DynamicSearchResult Result);
		bool rfqStatusUpdate(RFQStatu rfqstatus);
	}

}
