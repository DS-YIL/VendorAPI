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
        public string purchdoc { get; set; }
        public string vendor { get; set; }

        public string invoiceNo { get; set; }
        public long POMasterTableId { get; set; }

        public ParticularInvoice particularInvoice { get; set; }
    }

    public class ParticularInvoice
    {
        public long POMasterTableId { get; set; }
        public string InvoiceNo { get; set; }
    }
}
