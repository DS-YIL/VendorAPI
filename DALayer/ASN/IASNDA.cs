using SCMModels.ASNModels;
using SCMModels.RFQModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DALayer.ASN
{
   public interface IASNDA
    {
        Task<statuscheckmodel> CreateAsn(ASNShipmentHeaderModel model);
    }
}
