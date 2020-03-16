using SCMModels.MPRMasterModels;
using SCMModels.RFQModels;
using SCMModels.SCMModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using SCMModels;
using SCMModels.RemoteModel;

namespace DALayer.RFQ
{
    public interface IRFQDA
    {
        List<RFQQuoteView> getRFQItems(int RevisionId);
        bool updateVendorQuotes(List<RFQQuoteView> RFQQuoteViewList, List<YILTermsandCondition> termsList);
        DataTable getRFQCompareItems(int RevisionId);
        bool rfqStatusUpdate(List<RFQQuoteView> RFQQuoteViewList);

        //rfq module

        Task<RFQMasterModel> GetRFQById(int masterID);
        Task<statuscheckmodel> CreateRfQ(RfqRevisionModel model);
        //Task<List<RfqRevisionModel>> GetAllRFQs();
        Task<List<RFQMasterModel>> getallrfqlist();
        Task<RemoteRFQRevisions_N> GetRfqDetailsById(int revisionId);
        
        Task<statuscheckmodel> UpdateRfqRevision(RfqRevisionModel model);
        Task<statuscheckmodel> UpdateRfqItemByBulk(RfqItemModel model);
        Task<statuscheckmodel> UpdateSingleRfqItem(RfqItemModel model);
        Task<statuscheckmodel> UpdateBulkRfqRevision(RfqRevisionModel model);
        statuscheckmodel DeleteRfqById(int rfqmasterid);
        statuscheckmodel DeleteRfqRevisionbyId(int id);
        statuscheckmodel DeleteRfqItemById(int id);
        statuscheckmodel DeleteBulkItemsByItemId(List<int> id);
        List<RfqDocumentsModel> InsertDocument(RfqDocumentsModel model);
        List<RfqDocumentsModel> InsertDocumentTOYSCM(RfqDocumentsModel model);
        statuscheckmodel CommunicationAdd(RfqCommunicationModel model);
        Task<List<RfqItemModel>> GetItemsByRevisionId(int revisionid);
        Task<List<RfqRevisionModel>> GetAllrevisionRFQs();
        Task<RfqItemModel> GetItemsByItemId(int id);
        List<VendormasterModel> GetAllvendorList();
        Task<statuscheckmodel> CreateNewRfq(RFQMasterModel model);
        Task<VendormasterModel> GetvendorById(int id);
        Task<statuscheckmodel> InsertVendorterms(VendorRfqtermModel vendor);
        List<RfqItemModel> InsertRfqItemInfo(RfqItemModel model);
        List<RfqItemModel> editRfqItemInfo(RfqItemModel model);
        Task<statuscheckmodel> DeleteRfqIteminfoByid(int id,int rfqitemid);
        Task<statuscheckmodel> DeleteRfqIteminfoByidformultiple(int id,int BOMid);
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
        Task<List<MPRApproversViewModel>> GetAllMPRApprovers();
        Task<statuscheckmodel> InsertMprBuyerGroups(MPRBuyerGroupModel model);
        Task<statuscheckmodel> UpdateMprBuyerGroups(MPRBuyerGroupModel model);
        Task<statuscheckmodel> InsertMPRApprover(MPRApproverModel model);
        Task<MPRApproverModel> GetMPRApprovalsById(int id);
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
        List<RFQListView> getRFQList(rfqFilterParams Rfqfilterparams);
        List<VendorRegistrationModel> InsertVendordata(VendorRegistrationModel obj);
        List<StateList> StateNameList();
        List<NatureOfBusiness> natureOfBusinessesList();
        List<VendorRegistrationModel> SaveVendorDetails(VendorRegistrationModel model);
        List<DocumentType> DocumentMasterList();
        List<documentDetails> InsertDocuments(List<documentDetails> obj);
        bool DeletefileAttached(documentDetails model);
        List<RFQTerms> GetTermMaster(int rfqrevisionId);
        List<RFQTerms> UpdateVendorTerms(List<RFQTerms> obj);
       List<DocumentTypeMaster> GetMasterDocumentTypeList();
        string UpdateVendorCommunication(VendorCommunicaton model);
        bool DeletefileAttachedforDocuments(RFQDocument obj);
        List<VendorCommunicaton> GetVendorCommunicationForRFQRevId(int RFQRevisionId);
        List<RFQTerms> GetTermsByRfqRevisionId(int RfqRevisionId);
        //List<RfqDocumentsModel>GetAttachedDocuments
        List<RemoteRFQDocument> GetRfqdocumentdetailsById(int revisionId);
        List<RemoteRFQDocument> GetRfqdocumentdetailsById(int revisionId, int rfqitemId);
        VendorRegistrationModel GetVendorDetails(int vendorId);
        string changepassword(Changepassword obj);
        bool sendVendormail(int RFQRevisionId);
        bool checkemail(Changepassword emailId);
        bool sendLinkForForgetPassword(forgetpassword email);
        string Resetpassword(forgetpassword model);
        bool CheckLinkExpiryOrNot(forgetpassword model);
    }
}
