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

		Task<RemoteRFQRevisions_N> GetRfqDetailsById(int revisionId);
		List<RfqDocumentsModel> InsertDocument(RfqDocumentsModel model);
		Task<List<RfqItemModel>> GetItemsByRevisionId(int revisionid);
		List<RfqItemModel> InsertOrEditRfqItemInfo(RfqItemModel model);
		List<RfqItemModel> editRfqItemInfo(RfqItemModel model);
		Task<statuscheckmodel> DeleteRfqIteminfoByid(int id, int rfqitemid);
		Task<statuscheckmodel> DeleteRfqIteminfoByidformultiple(int id, int BOMid);
		Task<List<UnitMasterModel>> GetUnitMasterList();
		Task<List<CurrencyMasterModel>> GetAllMasterCurrency();
		Task<List<V_RFQList>> GetRfqByVendorId(rfqFilterParams obj);
		List<VendorRegistrationModel> InsertVendordata(VendorRegistrationModel obj);
		List<StateList> StateNameList();
		List<NatureOfBusiness> natureOfBusinessesList();
		RemoteVendorRegisterMaster SaveVendorDetails(VendorRegistrationModel model);
		RemoteVendorRegisterMaster updateRegTerms(VendorRegistrationModel model);
		List<DocumentType> DocumentMasterList();
		bool DeletefileAttached(documentDetails model);
		List<RemoteRfqTerm> GetTermMaster(int rfqrevisionId);
		List<RemoteRfqTerm> UpdateVendorTerms(List<RemoteRfqTerm> obj);
		List<RemoteDocumentTypeMaster> GetMasterDocumentTypeList();
		List<RemoteRFQCommunication> UpdateVendorCommunication(VendorCommunicaton model);
		bool DeletefileAttachedforDocuments(RFQDocument obj);
		List<VendorCommunicaton> GetVendorCommunicationForRFQRevId(int RFQRevisionId);
		List<RFQTerms> GetTermsByRfqRevisionId(int RfqRevisionId);
		List<RemoteRFQDocument> GetRfqdocumentdetailsById(int revisionId);
		List<RemoteRFQDocument> GetRfqdocumentdetailsById(int revisionId, int rfqitemId);
		VendorRegistrationModel GetVendorDetails(int vendorId);
		string changepassword(Changepassword obj);
		bool checkemail(Changepassword emailId);
		bool sendLinkForForgetPassword(forgetpassword email);
		string Resetpassword(forgetpassword model);
		bool CheckLinkExpiryOrNot(forgetpassword model);
		bool checkrfqitemexists(int rfqitemsid);
		DataTable getDBMastersList(DynamicSearchResult Result);
		bool rfqStatusUpdate(RFQStatu rfqstatus);
		RemoteBankGuarantee updateBG(RemoteBankGuarantee bg);
		List<RemoteBankGuarantee> getBGList(BGfilters BGfilters);

		RemoteBankGuarantee getBGDetails(int bgid);
		bool DeleteBGFile(int DocumentId);
	}
}
