using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCMModels.ASNModels
{
  public  class ASNShipmentLinesModel
    {
        public int ASNShipmentLine { get; set; }
        public string ShipFrom { get; set; }
        public string ShipTo { get; set; }
        public string CustomerLocation { get; set; }
        public int ContainerNumber { get; set; }
        public string CountryOfOrigin { get; set; }
        public string TruckNo { get; set; }
        public string Comment { get; set; }
        public string PackingShip { get; set; }
    }
}
