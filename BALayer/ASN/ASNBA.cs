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

		public bool InsertandEditAsn(RemoteASNShipmentHeader model)
		{
			return _asnDataAcess.InsertandEditAsn(model);
		}

		public List<RemoteASNShipmentHeader> getAsnList(int vendorId)
		{
			return _asnDataAcess.getAsnList(vendorId);
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

		public List<StagingPoSapModels> getPOInvoiceDetailsbyVendor(int vendorId)
		{
			return _asnDataAcess.getPOInvoiceDetailsbyVendor(vendorId);
		}
		public Task<InvoiceDetail> UpdateInvoice(InvoiceDetail invoiceModel)
		{
			return _asnDataAcess.UpdateInvoice(invoiceModel);
		}

		public InvoiceModel GetInvoiceDetails(string invoiceNo)
		{
			return _asnDataAcess.GetInvoiceDetails(invoiceNo);
		}

		public bool DeleteInvoiceFile(int DocumentId)
		{
			return _asnDataAcess.DeleteInvoiceFile(DocumentId);
		}

	}
}

