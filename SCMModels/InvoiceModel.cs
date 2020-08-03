using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCMModels
{
   public class InvoiceModel
    { 
        public string InvoiceNo { get; set; }
        public int VendorId { get; set; }
        public string VuniqueId { get; set; }
        public long? PONumber { get; set; }
        public DateTime CreatedDate { get; set; }
        public int CreatedBy { get; set; }
        public DateTime ModifiedDate { get; set; }
        public int ModifiedBy { get; set; }
        public string Remarks { get; set; }
        public string ModifiedBy_VuniqueId { get; set; }
        public int DocumentId { get; set; }
        public int InvoiceId { get; set; }
        public string DocumentName { get; set; }
        public int DocumentTypeId { get; set; }
        public string Path { get; set; }
        public int UploadedBy { get; set; }
        public DateTime UploadedDate { get; set; }
     

      
    }

    public class DocumentDetailsInvoice
    {
        public int DocumentId { get; set; }
        public int InvoiceId { get; set; }
        public string DocumentName { get; set; }
        public int DocumentTypeId { get; set; }
        public string Path { get; set; }
        public int UploadedBy { get; set; }
        public DateTime UploadedDate { get; set; }
        public bool IsDeleted { get; set; }
        public int vendorId { get; set; }
    }
}
