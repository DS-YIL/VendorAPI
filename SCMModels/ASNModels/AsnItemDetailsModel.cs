using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCMModels.ASNModels
{
   public class AsnItemDetailsModel
    {
       public float SupplierCumulativeQuantity { get; set; }
       public float CustomerCumulativeQuantity { get; set; }
       public string PurchasingDocumentNo { get; set; }
       public string OriginalPONumber { get; set; }
       public string OriginalPOItem { get; set; }
       public float TotalGrossWt { get; set; }
       public float TotalNetWt { get; set; }
       public string CustomerBatchNo { get; set; }
       public string SupplierBatchNo { get; set; }
    }
}
