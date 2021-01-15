using SCMModels.RemoteModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCMModels
{
	public class StagingPoSapModels
	{
		public int stageid { get; set; }
		public string PONo { get; set; }
		public string vendor { get; set; }
		public string VendorAdress { get; set; }
		public string invoiceNo { get; set; }
		public string ShipTo { get; set; }
		public string sloc { get; set; }
		public string Vendor { get; set; }
		public string Customer { get; set; }

		public long POMasterTableId { get; set; }


		public List<RemoteInvoiceDetail> InvoiceDetails { get; set; }
	}
	public class PoItemDetails
	{
		public string PONo { get; set; }
		public DateTime PODate { get; set; }
		public string Approver1 { get; set; }
		public string Approver2 { get; set; }
		public string VendorCode { get; set; }
		public string POItemNo { get; set; }
		public string Material { get; set; }
		public string MaterialDescription { get; set; }
		public string poitemdescription { get; set; }
		public Nullable<decimal> deliveredqty { get; set; }
		public Nullable<decimal> unitprice { get; set; }

		public Nullable<decimal> itemamount { get; set; }
		public string HSNCode { get; set; }
		public Nullable<decimal> POQty { get; set; }
		public Nullable<decimal> SupplierCumulativeQty { get; set; }
		public Nullable<decimal> RemainingQty { get; set; }
	}


}
