using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCMModels.ASNModels
{
   public class AsnStatusModel
    {
       public int ASNStatus { get; set; }
       public string ValidationStatus { get; set; }
       public string AcceptanceStatus { get; set; }
       public string TPOPIndicator { get; set; }
       public string InboundDeliveryStatus { get; set; }
       public string OriginalSystem { get; set; }
       public string Split { get; set; }
    }
}
