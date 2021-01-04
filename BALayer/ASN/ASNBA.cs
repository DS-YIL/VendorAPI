using DALayer.ASN;
using SCMModels;
using SCMModels.RemoteModel;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
namespace BALayer.ASN
{
	public class ASNBA : IASNBA
	{
		public readonly ASNDA _asnDataAcess;
		public ASNBA(ASNDA ASNDA)
		{
			this._asnDataAcess = ASNDA;
		}
		// public readonly IASNDA _asnDataAcess;
		//public ASNBA(IASNDA ASNDA)
		//{
		//    this._asnDataAcess = ASNDA;
		//}
		//ASNDA _asnDataAcess = new ASNDA();

		public int InsertandEditAsn(RemoteASNShipmentHeader model)
		{
			return _asnDataAcess.InsertandEditAsn(model);
		}

		public List<RemoteASNShipmentHeader> getAsnList(ASNfilters ASNfilters)
		{
			return _asnDataAcess.getAsnList(ASNfilters);
		}

		public RemoteASNShipmentHeader getAsnDetailsByAsnNo(int ASNId)
		{
			return _asnDataAcess.getAsnDetailsByAsnNo(ASNId);
		}

		public List<StagingPoSapModels> getPONumbersbyVendor(int vendorId)
		{
			return _asnDataAcess.getPONumbersbyVendor(vendorId);
		}
		public List<PoItemDetails> getItemDetailsByPoNo(string PONo)
		{
			return _asnDataAcess.getItemDetailsByPoNo(PONo);
		}
		public bool updateASNComminications(RemoteASNCommunication asncom)
		{
			return _asnDataAcess.updateASNComminications(asncom);
		}

		//public List<StagingPoSapModels> getPOInvoiceDetailsbyVendor(int vendorId)
		//{
		//	return _asnDataAcess.getPOInvoiceDetailsbyVendor(vendorId);
		//}
		
		public Task<RemoteInvoiceDetail> UpdateInvoice(RemoteInvoiceDetail invoiceModel)
		{
			return _asnDataAcess.UpdateInvoice(invoiceModel);
		}

		public RemoteInvoiceDetail GetInvoiceDetails(RemoteInvoiceDetail invoiceDetails)
		{
			return _asnDataAcess.GetInvoiceDetails(invoiceDetails);
		}

		public bool DeleteInvoiceFile(int DocumentId)
		{
			return _asnDataAcess.DeleteInvoiceFile(DocumentId);
		}

	}
}

