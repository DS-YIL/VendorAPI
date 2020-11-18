using DALayer.ASN;
using SCMModels;
using SCMModels.ASNModels;
using SCMModels.RemoteModel;
using SCMModels.RFQModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace BALayer.ASN
{
	public class ASNBA : IASNBA
	{
		// public readonly IASNDA _asnDataAcess;
		//public ASNBA(IASNDA ASNDA)
		//{
		//    this._asnDataAcess = ASNDA;
		//}
		ASNDA _asnDataAcess = new ASNDA();

		public bool CreateAsn(ASNShipmentHeaderModel model)
		{
			return _asnDataAcess.CreateAsn(model);
		}

		public List<ASNShipmentHeader> getAsnList()
		{
			return _asnDataAcess.getAsnList();
		}

		public ASNShipmentHeaderModel getAsnDetailsByAsnNo(int ASNId)
		{
			return _asnDataAcess.getAsnDetailsByAsnNo(ASNId);
		}

		public DataTable getPONumbersbyVendor(int vendorId)
		{
			return _asnDataAcess.getPONumbersbyVendor(vendorId);
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

