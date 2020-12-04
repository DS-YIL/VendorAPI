/*
    Name of File : <<IFRFQBA>>  Author :<<Prasanna>>  
    Date of Creation <<03-08-2020>>
    Purpose : <<This is the interface between rfq controller and RFQ business layer >>
    Review Date :<<>>   Reviewed By :<<>>
    Version : 0.1 <change version only if there is major change - new release etc>
    Sourcecode Copyright : Yokogawa India Limited
*/
using SCMModels.RFQModels;
using SCMModels.SCMModels;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Data;
using SCMModels;
using SCMModels.RemoteModel;

namespace BALayer.RFQ
{
	/*Name of Class : <<IRFQBA>>  Author :<<Prasanna>>  
	Date of Creation <<03-08-2020>>
	Purpose : <<This is the interface between rfq controller and RFQ business laye >>>
	Review Date :<<>>   Reviewed By :<<>>*/
	public interface IRFQBA
	{

		Task<RemoteRFQRevisions_N> GetRfqDetailsById(int revisionId);
		List<RfqDocumentsModel> InsertDocument(RfqDocumentsModel model);
		Task<List<RfqItemModel>> GetItemsByRevisionId(int revisionid);
		List<RfqItemModel> InsertOrEditRfqItemInfo(RfqItemModel model);
		Task<statuscheckmodel> DeleteRfqIteminfoByid(int id, int DeleteRfqIteminfoByid);
		Task<List<UnitMasterModel>> GetUnitMasterList();
		Task<List<CurrencyMasterModel>> GetAllMasterCurrency();		
		Task<List<V_RFQList>> GetRfqByVendorId(rfqFilterParams obj);
		List<VendorRegistrationModel> InsertVendordata(VendorRegistrationModel obj);
		List<StateList> StateNameList();
		List<NatureOfBusiness> natureOfBusinessesList();
		List<DocumentType> DocumentMasterList();
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
