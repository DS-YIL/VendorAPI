using DALayer.RFQ;
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
/*
    Name of File : <<RFQBA>>  Author :<<Prasanna>>  
    Date of Creation <<03-08-2020>>
    Purpose : <<This is the business layer, medium  between IRFQBA and IFRFQDA  layer >>
    Review Date :<<>>   Reviewed By :<<>>
    Version : 0.1 <change version only if there is major change - new release etc>
    Sourcecode Copyright : Yokogawa India Limited
*/

namespace BALayer.RFQ
{
	public class RFQBA : IRFQBA
	{
		public readonly IRFQDA _rfqDataAcess;
		public RFQBA(IRFQDA RFQDA)
		{
			this._rfqDataAcess = RFQDA;
		}

		///rfqmodule
		public async Task<List<RfqItemModel>> GetItemsByRevisionId(int revisionid)
		{
			return await _rfqDataAcess.GetItemsByRevisionId(revisionid);
		}
		public Task<RfqRevisionModel> getrfqrevisionbyid(int id)
		{
			throw new NotImplementedException();
		}
		public List<RfqDocumentsModel> InsertDocument(RfqDocumentsModel model)
		{
			return _rfqDataAcess.InsertDocument(model);
		}
		public async Task<RemoteRFQRevisions_N> GetRfqDetailsById(int revisionId)
		{
			return await _rfqDataAcess.GetRfqDetailsById(revisionId);
		}
		public List<RfqItemModel> InsertOrEditRfqItemInfo(RfqItemModel model)
		{
			return _rfqDataAcess.InsertOrEditRfqItemInfo(model);
		}
		public async Task<statuscheckmodel> DeleteRfqIteminfoByid(int id, int rfqitemid)
		{
			return await _rfqDataAcess.DeleteRfqIteminfoByid(id, rfqitemid);
		}
		public async Task<List<UnitMasterModel>> GetUnitMasterList()
		{
			return await _rfqDataAcess.GetUnitMasterList();
		}
		public async Task<List<CurrencyMasterModel>> GetAllMasterCurrency()
		{
			return await _rfqDataAcess.GetAllMasterCurrency();
		}
		public async Task<List<V_RFQList>> GetRfqByVendorId(rfqFilterParams obj)
		{
			return await _rfqDataAcess.GetRfqByVendorId(obj);
		}

		public List<VendorRegistrationModel> InsertVendordata(VendorRegistrationModel obj)
		{
			return _rfqDataAcess.InsertVendordata(obj);
		}

		public List<StateList> StateNameList()
		{
			return _rfqDataAcess.StateNameList();
			//throw new NotImplementedException();
		}

		public List<NatureOfBusiness> natureOfBusinessesList()
		{
			return _rfqDataAcess.natureOfBusinessesList();
			//throw new NotImplementedException();
		}

		public List<DocumentType> DocumentMasterList()
		{
			return _rfqDataAcess.DocumentMasterList();
			//throw new NotImplementedException();
		}

		public RemoteVendorRegisterMaster SaveVendorDetails(VendorRegistrationModel model)
		{
			return _rfqDataAcess.SaveVendorDetails(model);
		}
		public RemoteVendorRegisterMaster updateRegTerms(VendorRegistrationModel model)
		{
			return _rfqDataAcess.updateRegTerms(model);
		}

		public bool DeletefileAttached(documentDetails model)
		{
			return _rfqDataAcess.DeletefileAttached(model);
			// throw new NotImplementedException();
		}

		public List<RemoteRfqTerm> GetTermMaster(int rfqrevisionId)
		{
			return _rfqDataAcess.GetTermMaster(rfqrevisionId);
			//throw new NotImplementedException();
		}

		public List<RemoteRfqTerm> UpdateVendorTerms(List<RemoteRfqTerm> obj)
		{
			return _rfqDataAcess.UpdateVendorTerms(obj);
			// throw new NotImplementedException();
		}

		public List<RemoteDocumentTypeMaster> GetMasterDocumentTypeList()
		{
			return _rfqDataAcess.GetMasterDocumentTypeList();
			//throw new NotImplementedException();
		}

		public List<RemoteRFQCommunication> UpdateVendorCommunication(VendorCommunicaton model)
		{
			return _rfqDataAcess.UpdateVendorCommunication(model);
			// throw new NotImplementedException();
		}

		public bool DeletefileAttachedforDocuments(RFQDocument obj)
		{
			return _rfqDataAcess.DeletefileAttachedforDocuments(obj);
		}

		public List<VendorCommunicaton> GetVendorCommunicationForRFQRevId(int RFQRevevisionId)
		{
			return _rfqDataAcess.GetVendorCommunicationForRFQRevId(RFQRevevisionId);
		}

		public List<RfqItemModel> editRfqItemInfo(RfqItemModel model)
		{
			return _rfqDataAcess.editRfqItemInfo(model);
		}

		public List<RFQTerms> GetTermsByRfqRevisionId(int RfqRevisionId)
		{
			return _rfqDataAcess.GetTermsByRfqRevisionId(RfqRevisionId);
		}

		public List<RemoteRFQDocument> GetRfqdocumentdetailsById(int revisionId)
		{
			return _rfqDataAcess.GetRfqdocumentdetailsById(revisionId);
		}

		public List<RemoteRFQDocument> GetRfqdocumentdetailsById(int revisionId, int rfqitemsid)
		{
			return _rfqDataAcess.GetRfqdocumentdetailsById(revisionId, rfqitemsid);
		}

		public VendorRegistrationModel GetVendorDetails(int vendorId)
		{
			return _rfqDataAcess.GetVendorDetails(vendorId);
			// throw new NotImplementedException();
		}

		public string changepassword(Changepassword obj)
		{
			return _rfqDataAcess.changepassword(obj);
		}


		public Task<statuscheckmodel> DeleteRfqIteminfoByidformultiple(int id, int BOMid)
		{
			return this._rfqDataAcess.DeleteRfqIteminfoByidformultiple(id, BOMid);
		}

		public bool checkemail(Changepassword emailId)
		{
			return this._rfqDataAcess.checkemail(emailId);
		}

		public bool sendLinkForForgetPassword(forgetpassword email)
		{
			return this._rfqDataAcess.sendLinkForForgetPassword(email);
		}

		public string Resetpassword(forgetpassword model)
		{
			return this._rfqDataAcess.Resetpassword(model);
		}

		public bool CheckLinkExpiryOrNot(forgetpassword model)
		{
			return this._rfqDataAcess.CheckLinkExpiryOrNot(model);
		}

		public bool checkrfqitemexists(int rfqitemsid)
		{
			return this._rfqDataAcess.checkrfqitemexists(rfqitemsid);
		}

		public bool rfqStatusUpdate(RFQStatu rfqStatus)
		{
			return this._rfqDataAcess.rfqStatusUpdate(rfqStatus);
		}

		public DataTable getDBMastersList(DynamicSearchResult Result)
		{
			return this._rfqDataAcess.getDBMastersList(Result);

		}

		//public List<RfqDocumentsModel> InsertDocumentTOYSCM(RfqDocumentsModel model)
		//{
		//    throw new NotImplementedException();
		//}

		//bank guarantee
		public RemoteBankGuarantee updateBG(RemoteBankGuarantee bg)
		{
			return this._rfqDataAcess.updateBG(bg);
		}
		public List<RemoteBankGuarantee> getBGList(BGfilters BGfilters)
		{
			return this._rfqDataAcess.getBGList(BGfilters);

		}
		public RemoteBankGuarantee getBGDetails(int bgid)
		{
			return this._rfqDataAcess.getBGDetails(bgid);

		}
		public bool DeleteBGFile(int DocumentId)
		{
			return this._rfqDataAcess.DeleteBGFile(DocumentId);

		}
	}
}
