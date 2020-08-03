using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCMModels.ASNModels
{
   public class ASNShipmentHeaderModel
    {
        public int  ASNId { get; set; }
        public string PO_ReferenceNo { get; set; }
        public string InvoiceNo { get; set; }
        public string ASNNo { get; set; }
        public string InboundDeliveryNo { get; set; }
        public DateTime? ShippingDate { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public DateTime? DocumentDate { get; set; }
        public int? Qty { get; set; }
        public int? FreightInvNo { get; set; }
        public int? GroupingId { get; set; }
        public string CarrierParty { get; set; }
        public int? Carrier_SCAC_Id { get; set; }
        public string IncotermLoc { get; set; }
        public int? Incoterm { get; set; }
        public string MeansOfTransport { get; set; }
        public float TotalGrossWeight_Kgs { get; set; }
        public float TotalNetWeight_Kgs { get; set; }
        public float TotalVolume { get; set; }
        public string ShipmentAssignment { get; set; }
        public DateTime CreatedDate { get; set; }

       // Item Details
        public double? SupplierCumulativeQuantity { get; set; }
        public double? CustomerCumulativeQuantity { get; set; }
        public string PurchasingDocumentNo { get; set; }
        public string OriginalPONumber { get; set; }
        public string OriginalPOItem { get; set; }
        public float TotalGrossWt { get; set; }
        public float TotalNetWt { get; set; }
        public string CustomerBatchNo { get; set; }
        public string SupplierBatchNo { get; set; }

        //Status Models
        public int ASNStatus { get; set; }
        public string ValidationStatus { get; set; }
        public string AcceptanceStatus { get; set; }
        public string TPOPIndicator { get; set; }
        public string InboundDeliveryStatus { get; set; }
        public string OriginalSystem { get; set; }
        public string Split { get; set; }

        //Shipment Lines
        public int ASNShipmentLine { get; set; }
        public string ShipFrom { get; set; }
        public string ShipTo { get; set; }
        public string CustomerLocation { get; set; }
        public int? ContainerNumber { get; set; }
        public string CountryOfOrigin { get; set; }
        public string TruckNo { get; set; }
        public string Comment { get; set; }
        public string PackingShip { get; set; }
        public ASNShipmentLinesModel asnShipmentLinesModel { get; set; }
        public AsnItemDetailsModel asnItemDetailsModel { get; set; }
        public AsnStatusModel asnStatusModel { get; set; }

    }
}
