using SCMModels.ASNModels;
using SCMModels.RFQModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BALayer.ASN
{
    public interface IASNBA
    {
        Task<statuscheckmodel> CreateAsn(ASNShipmentHeaderModel model);
    }
}
