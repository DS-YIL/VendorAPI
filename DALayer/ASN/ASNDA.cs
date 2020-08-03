using Npgsql;
using SCMModels;
using SCMModels.ASNModels;
using SCMModels.RemoteModel;
using SCMModels.RFQModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DALayer.ASN
{
   public class ASNDA:IASNDA
    {
        VSCMEntities vscm = new VSCMEntities();
       public async Task<statuscheckmodel> CreateAsn(ASNShipmentHeaderModel model)
        {
            statuscheckmodel status = new statuscheckmodel();
            try
            {
                var asnShipmentHeaderModels = new ASNShipmentHeader();
                var asnShipmentLines = new ASNShipmentLine();
                var asnItemDetails = new ASNItemDetail();
                var asnStatus = new ASNStatu();
                if(model != null)
                {
                    vscm.Database.Connection.Open();
                    if(model.ASNId == 0)
                    {
                        asnShipmentHeaderModels.PO_ReferenceNo = model.PO_ReferenceNo;
                        asnShipmentHeaderModels.InvoiceNo = model.InvoiceNo;
                        asnShipmentHeaderModels.ASNNo = model.ASNNo;
                        asnShipmentHeaderModels.InboundDeliveryNo = model.InboundDeliveryNo;
                        asnShipmentHeaderModels.ShippingDate = model.ShippingDate;
                        asnShipmentHeaderModels.DeliveryDate = model.DeliveryDate;
                        asnShipmentHeaderModels.DocumentDate = model.DocumentDate;
                        asnShipmentHeaderModels.Qty = model.Qty;
                        asnShipmentHeaderModels.FreightInvNo = model.FreightInvNo;
                        asnShipmentHeaderModels.GroupingId = model.GroupingId;
                        asnShipmentHeaderModels.CarrierParty = model.CarrierParty;
                        asnShipmentHeaderModels.Carrier_SCAC_Id = model.Carrier_SCAC_Id;
                        asnShipmentHeaderModels.IncotermLoc = model.IncotermLoc;
                        asnShipmentHeaderModels.Incoterm = model.Incoterm;
                        asnShipmentHeaderModels.MeansOfTransport = model.MeansOfTransport;
                        asnShipmentHeaderModels.TotalGrossWeight_Kgs = model.TotalGrossWeight_Kgs;
                        asnShipmentHeaderModels.TotalNetWeight_Kgs = model.TotalNetWeight_Kgs;
                        asnShipmentHeaderModels.TotalVolume = model.TotalVolume;
                        asnShipmentHeaderModels.ShipmentAssignment = model.ShipmentAssignment;
                        asnShipmentHeaderModels.CreatedDate = DateTime.Now;
                        vscm.ASNShipmentHeaders.Add(asnShipmentHeaderModels);
                        vscm.SaveChanges();

                        int id = asnShipmentHeaderModels.ASNId;

                        asnShipmentLines.ASNId = id;
                        asnShipmentLines.ShipFrom = model.ShipFrom;
                        asnShipmentLines.ShipTo = model.ShipTo;
                        asnShipmentLines.CustomerLocation = model.CustomerLocation;
                        asnShipmentLines.ContainerNumber = model.ContainerNumber;
                        asnShipmentLines.CountryOfOrigin = model.CountryOfOrigin;
                        asnShipmentLines.TruckNo = model.TruckNo;
                        asnShipmentLines.Comment = model.Comment;
                        asnShipmentLines.PackingShip = model.PackingShip;
                        vscm.ASNShipmentLines.Add(asnShipmentLines);
                        vscm.SaveChanges();

                        asnItemDetails.ASNId = id;
                        asnItemDetails.SupplierCumulativeQuantity = model.SupplierCumulativeQuantity;
                        asnItemDetails.CustomerCumulativeQuantity = model.CustomerCumulativeQuantity;
                        asnItemDetails.PurchasingDocumentNo = model.PurchasingDocumentNo;
                        asnItemDetails.OriginalPONumber = model.OriginalPONumber;
                        asnItemDetails.OriginalPOItem = model.OriginalPOItem;
                        asnItemDetails.TotalGrossWt_Kgs = model.TotalGrossWt;
                        asnItemDetails.TotalNetWt_Kgs = model.TotalNetWt;
                        asnItemDetails.CustomerBatchNo = model.CustomerBatchNo;
                        asnItemDetails.SupplierBatchNo = model.SupplierBatchNo;
                        vscm.ASNItemDetails.Add(asnItemDetails);
                        vscm.SaveChanges();

                        asnStatus.ASNId = id;
                        asnStatus.ASNStatus = Convert.ToString(model.ASNStatus);
                        asnStatus.ValidationStatus = model.ValidationStatus;
                        asnStatus.AcceptanceStatus = model.AcceptanceStatus;
                        asnStatus.TPOPIndicator = model.TPOPIndicator;
                        asnStatus.InboundDeliveryStatus = model.InboundDeliveryStatus;
                        asnStatus.OriginalSystem = model.OriginalSystem;
                        asnStatus.Split = model.Split;
                        vscm.ASNStatus.Add(asnStatus);
                        vscm.SaveChanges();

                        status.Sid = id;
                    }
                }

                return status;
            }
            catch(Exception ex)
            {
                throw;
            }
        }

        public async Task<statuscheckmodel> EditAsn(ASNShipmentHeaderModel model)
        {
            statuscheckmodel status = new statuscheckmodel();
            try
            {
                var asnShipmentHeaderModels = new ASNShipmentHeader();
                var asnShipmentLines = new ASNShipmentLine();
                var asnItemDetails = new ASNItemDetail();
                var asnStatus = new ASNStatu();
                if (model != null)
                {
                    vscm.Database.Connection.Open();

                    asnShipmentHeaderModels = vscm.ASNShipmentHeaders.Where(x => x.ASNNo == model.ASNNo).FirstOrDefault();
                    if(asnShipmentHeaderModels != null)
                    {
                        asnShipmentHeaderModels.PO_ReferenceNo = model.PO_ReferenceNo;
                        asnShipmentHeaderModels.InvoiceNo = model.InvoiceNo;
                        asnShipmentHeaderModels.ASNNo = model.ASNNo;
                        asnShipmentHeaderModels.InboundDeliveryNo = model.InboundDeliveryNo;
                        asnShipmentHeaderModels.ShippingDate = model.ShippingDate;
                        asnShipmentHeaderModels.DeliveryDate = model.DeliveryDate;
                        asnShipmentHeaderModels.DocumentDate = model.DocumentDate;
                        asnShipmentHeaderModels.Qty = model.Qty;
                        asnShipmentHeaderModels.FreightInvNo = model.FreightInvNo;
                        asnShipmentHeaderModels.GroupingId = model.GroupingId;
                        asnShipmentHeaderModels.CarrierParty = model.CarrierParty;
                        asnShipmentHeaderModels.Carrier_SCAC_Id = model.Carrier_SCAC_Id;
                        asnShipmentHeaderModels.IncotermLoc = model.IncotermLoc;
                        asnShipmentHeaderModels.Incoterm = model.Incoterm;
                        asnShipmentHeaderModels.MeansOfTransport = model.MeansOfTransport;
                        asnShipmentHeaderModels.TotalGrossWeight_Kgs = model.TotalGrossWeight_Kgs;
                        asnShipmentHeaderModels.TotalNetWeight_Kgs = model.TotalNetWeight_Kgs;
                        asnShipmentHeaderModels.TotalVolume = model.TotalVolume;
                        asnShipmentHeaderModels.ShipmentAssignment = model.ShipmentAssignment;
                        vscm.SaveChanges();

                    }

                    asnShipmentLines = vscm.ASNShipmentLines.Where(x => x.ASNId == model.ASNId).FirstOrDefault();

                    if(asnShipmentLines != null)
                    { 
                        asnShipmentLines.ShipFrom = model.ShipFrom;
                        asnShipmentLines.ShipTo = model.ShipTo;
                        asnShipmentLines.CustomerLocation = model.CustomerLocation;
                        asnShipmentLines.ContainerNumber = model.ContainerNumber;
                        asnShipmentLines.CountryOfOrigin = model.CountryOfOrigin;
                        asnShipmentLines.TruckNo = model.TruckNo;
                        asnShipmentLines.Comment = model.Comment;
                        asnShipmentLines.PackingShip = model.PackingShip;
                        vscm.SaveChanges();
                    }

                    asnItemDetails = vscm.ASNItemDetails.Where(x => x.ASNId == model.ASNId).FirstOrDefault();
                    if(asnItemDetails != null)
                    {
                        asnItemDetails.SupplierCumulativeQuantity = model.SupplierCumulativeQuantity;
                        asnItemDetails.CustomerCumulativeQuantity = model.CustomerCumulativeQuantity;
                        asnItemDetails.PurchasingDocumentNo = model.PurchasingDocumentNo;
                        asnItemDetails.OriginalPONumber = model.OriginalPONumber;
                        asnItemDetails.OriginalPOItem = model.OriginalPOItem;
                        asnItemDetails.TotalGrossWt_Kgs = model.TotalGrossWt;
                        asnItemDetails.TotalNetWt_Kgs = model.TotalNetWt;
                        asnItemDetails.CustomerBatchNo = model.CustomerBatchNo;
                        asnItemDetails.SupplierBatchNo = model.SupplierBatchNo;
                        vscm.SaveChanges();
                    }

                    asnStatus = vscm.ASNStatus.Where(x => x.ASNId == model.ASNId).FirstOrDefault();

                    if(asnStatus != null)
                    {
                        asnStatus.ASNStatus = Convert.ToString(model.ASNStatus);
                        asnStatus.ValidationStatus = model.ValidationStatus;
                        asnStatus.AcceptanceStatus = model.AcceptanceStatus;
                        asnStatus.TPOPIndicator = model.TPOPIndicator;
                        asnStatus.InboundDeliveryStatus = model.InboundDeliveryStatus;
                        asnStatus.OriginalSystem = model.OriginalSystem;
                        asnStatus.Split = model.Split;
                        vscm.SaveChanges();
                    }

                    status.Sid = model.ASNId;
                    
                }

                return status;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public List<ASNShipmentHeader> getAsnList()
        {
            List<ASNShipmentHeader> asnList = new List<ASNShipmentHeader>();
            try
            {
                asnList = vscm.ASNShipmentHeaders.OrderByDescending( x => x.ASNId).ToList();
                return asnList;
            }
            catch(Exception ex)
            {
                throw;
            }
        }
         public ASNShipmentHeaderModel getAsnDetailsByAsnNo(int asnNo)
        {
            try
            {
                ASNShipmentHeaderModel asnShipmentHeaderModel = new ASNShipmentHeaderModel();

                var asnShipmentHeader = vscm.ASNShipmentHeaders.Where(x => x.ASNNo == asnNo.ToString()).FirstOrDefault();

                asnShipmentHeaderModel.PO_ReferenceNo = asnShipmentHeader.PO_ReferenceNo;
                asnShipmentHeaderModel.ASNId = asnShipmentHeader.ASNId;

                var asnStatus = vscm.ASNStatus.Where(x => x.ASNId == asnShipmentHeaderModel.ASNId).FirstOrDefault();
                var asnShipmentLines = vscm.ASNShipmentLines.Where(x => x.ASNId == asnShipmentHeaderModel.ASNId).FirstOrDefault();
                var asnItemDetails = vscm.ASNItemDetails.Where(x => x.ASNId == asnShipmentHeaderModel.ASNId).FirstOrDefault();

                asnShipmentHeaderModel.InvoiceNo = asnShipmentHeader.InvoiceNo;
                asnShipmentHeaderModel.ASNNo = asnShipmentHeader.ASNNo;
                asnShipmentHeaderModel.InboundDeliveryNo = asnShipmentHeader.InboundDeliveryNo;
                asnShipmentHeaderModel.ShippingDate = asnShipmentHeader.ShippingDate;
                asnShipmentHeaderModel.DeliveryDate = asnShipmentHeader.DeliveryDate;
                asnShipmentHeaderModel.DocumentDate = asnShipmentHeader.DocumentDate;
                asnShipmentHeaderModel.Qty = asnShipmentHeader.Qty;
                asnShipmentHeaderModel.FreightInvNo = asnShipmentHeader.FreightInvNo;
                asnShipmentHeaderModel.GroupingId = asnShipmentHeader.GroupingId;
                asnShipmentHeaderModel.CarrierParty = asnShipmentHeader.CarrierParty;
                asnShipmentHeaderModel.Carrier_SCAC_Id = asnShipmentHeader.Carrier_SCAC_Id;
                asnShipmentHeaderModel.IncotermLoc = asnShipmentHeader.IncotermLoc;
                asnShipmentHeaderModel.Incoterm = asnShipmentHeader.Incoterm;
                asnShipmentHeaderModel.MeansOfTransport = asnShipmentHeader.MeansOfTransport;
                asnShipmentHeaderModel.TotalGrossWt = Convert.ToSingle(asnShipmentHeader.TotalGrossWeight_Kgs);
                asnShipmentHeaderModel.TotalNetWt = Convert.ToSingle(asnShipmentHeader.TotalNetWeight_Kgs);
                asnShipmentHeaderModel.TotalVolume = Convert.ToSingle(asnShipmentHeader.TotalVolume);
                asnShipmentHeaderModel.ShipmentAssignment = asnShipmentHeader.ShipmentAssignment;

                //Shipment Lines
                asnShipmentHeaderModel.ShipFrom = asnShipmentLines.ShipFrom;
                asnShipmentHeaderModel.ShipTo = asnShipmentLines.ShipTo;
                asnShipmentHeaderModel.CustomerLocation = asnShipmentLines.CustomerLocation;
                asnShipmentHeaderModel.ContainerNumber = asnShipmentLines.ContainerNumber;
                asnShipmentHeaderModel.CountryOfOrigin = asnShipmentLines.CountryOfOrigin;
                asnShipmentHeaderModel.TruckNo = asnShipmentLines.TruckNo;
                asnShipmentHeaderModel.Comment = asnShipmentLines.Comment;
                asnShipmentHeaderModel.PackingShip = asnShipmentLines.PackingShip;

                //Status
                asnShipmentHeaderModel.ASNStatus = Convert.ToInt32(asnStatus.ASNStatus);
                asnShipmentHeaderModel.ValidationStatus = asnStatus.ValidationStatus;
                asnShipmentHeaderModel.AcceptanceStatus = asnStatus.AcceptanceStatus;
                asnShipmentHeaderModel.TPOPIndicator = asnStatus.TPOPIndicator;
                asnShipmentHeaderModel.InboundDeliveryStatus = asnStatus.InboundDeliveryStatus;
                asnShipmentHeaderModel.OriginalSystem = asnStatus.OriginalSystem;
                asnShipmentHeaderModel.Split = asnStatus.Split;

                //Item Details
                asnShipmentHeaderModel.SupplierCumulativeQuantity = asnItemDetails.SupplierCumulativeQuantity;
                asnShipmentHeaderModel.CustomerCumulativeQuantity = asnItemDetails.CustomerCumulativeQuantity;
                asnShipmentHeaderModel.PurchasingDocumentNo = asnItemDetails.PurchasingDocumentNo;
                asnShipmentHeaderModel.OriginalPONumber = asnItemDetails.OriginalPONumber;
                asnShipmentHeaderModel.OriginalPOItem = asnItemDetails.OriginalPOItem;
                asnShipmentHeaderModel.TotalGrossWeight_Kgs = Convert.ToSingle( asnItemDetails.TotalGrossWt_Kgs);
                asnShipmentHeaderModel.TotalNetWeight_Kgs = Convert.ToSingle(asnItemDetails.TotalNetWt_Kgs);
                asnShipmentHeaderModel.CustomerBatchNo = asnItemDetails.CustomerBatchNo;
                asnShipmentHeaderModel.SupplierBatchNo = asnItemDetails.SupplierBatchNo;

                return asnShipmentHeaderModel;

            }
            catch(Exception ex)
            {
                throw;
            }
        }

        public List<DocumentDetailsInvoice> InsertDocuments_Invoice(List<DocumentDetailsInvoice> model)
        {
            List<DocumentDetailsInvoice> Listobj = new List<DocumentDetailsInvoice>();
            var eachobj = new DocumentDetailsInvoice();
            try
            {

                if (model != null)
                {
                    foreach (var item in model)
                    {
                        //InvoiceDocument invoiceDocument = new InvoiceDocument();
                        var invoicedocuments = new InvoiceDocument();


                        if (item.DocumentName == "Invoice")
                        {
                            invoicedocuments.DocumentTypeId = 1;
                        }
                        else if (item.DocumentName == "Tax")
                        {
                            invoicedocuments.DocumentTypeId = 2;
                        }
                        else
                        {
                            invoicedocuments.DocumentTypeId = 3;
                        }


                        if (item.DocumentId != 0)
                        {
                           if(item.DocumentTypeId != 3)
                            {
                                invoicedocuments = vscm.InvoiceDocuments.Where(x => x.DocumentId == item.DocumentId).FirstOrDefault();
                                invoicedocuments.IsDeleted = true;
                                vscm.SaveChanges();
                            }
                            
                        }

                        
                        //invoicedocuments.DocumentTypeId = item.DocumentTypeId;
                        invoicedocuments.DocumentName = item.DocumentName;
                        invoicedocuments.Path = item.Path;
                        invoicedocuments.UploadedDate = DateTime.Now;
                        invoicedocuments.IsDeleted = item.IsDeleted;
                        invoicedocuments.UploadedBy = item.vendorId;
                        vscm.InvoiceDocuments.Add(invoicedocuments);
                        vscm.SaveChanges();
                        eachobj.DocumentName = invoicedocuments.Path.Substring(invoicedocuments.Path.IndexOf('_') + 1);
                        eachobj.DocumentId = invoicedocuments.DocumentId;
                        Listobj.Add(eachobj);
                    }

                }
            }
            catch(Exception ex)
            {
              string msg =  ex.Message;
            }
            return Listobj;
        }

        public InvoiceModel GetInvoiceDetails(string InvoiceNo)
        {
            try
            {
                InvoiceDetail invoiceDetails = new InvoiceDetail();
                POMasterTable pOMasterTable = new POMasterTable();

                InvoiceModel invoiceModel = new InvoiceModel();

                invoiceDetails =  vscm.InvoiceDetails.Where(x => x.InvoiceNo == InvoiceNo).FirstOrDefault();
                pOMasterTable = vscm.POMasterTables.Where(x => x.POMasterId == invoiceDetails.POMasterId).FirstOrDefault();

                invoiceModel.PONumber = pOMasterTable.PONumber;
                invoiceModel.InvoiceNo = invoiceDetails.InvoiceNo;
                invoiceModel.Remarks = invoiceDetails.Remarks;

                return invoiceModel;
               // invoiceDetails.

            }
            catch(Exception ex)
            {
                return null;
            }
        }

        public async Task<statuscheckmodel> EditInvoice(InvoiceModel invoiceModel)
        {
            try
            {
                statuscheckmodel status = new statuscheckmodel();
                var POMasterTable = new POMasterTable();
                int id;

                InvoiceDetail invoiceDetail = vscm.InvoiceDetails.Where(x => x.InvoiceNo == invoiceModel.InvoiceNo).FirstOrDefault();
               // id = POMasterId.POMasterId;
                
                var invoiceDetails = new InvoiceDetail();

                //invoiceDetails.POMasterId = id;
                //invoiceDetails.InvoiceNo = invoiceModel.InvoiceNo;
                //invoiceDetails.CreatedDate = DateTime.Now;
                //invoiceDetails.CreatedBy = invoiceModel.VendorId;
                invoiceDetails.ModifiedBy = invoiceModel.VendorId;
                invoiceDetails.Remarks = invoiceModel.Remarks;
                invoiceDetails.ModifiedDate = DateTime.Now;

                //vscm.InvoiceDetails.Add(invoiceDetails);

                vscm.SaveChanges();

                status.Sid = invoiceDetail.InvoiceId;
                return status;
            }
            catch(Exception ex)
            {
                return null;
            }
        }

            public async Task<statuscheckmodel> CreateInvoice(InvoiceModel invoiceModel)
        {
            try
            {
                statuscheckmodel status = new statuscheckmodel();
                var POMasterTable = new POMasterTable();
                int id;

                bool IsPOExists = vscm.POMasterTables.Any( x => x.PONumber == invoiceModel.PONumber);

                if (!IsPOExists)
                {
                    POMasterTable.VendorId = invoiceModel.VendorId;
                    POMasterTable.VuniqueId = invoiceModel.VuniqueId;
                    POMasterTable.PONumber = invoiceModel.PONumber;
                    POMasterTable.CreatedDate = DateTime.Now;
                    POMasterTable.CreatedBy = invoiceModel.VendorId;
                    POMasterTable.ModifiedDate = DateTime.Now;
                    POMasterTable.ModifiedBy_VuniqueId = invoiceModel.VuniqueId;
                    vscm.POMasterTables.Add(POMasterTable);
                    vscm.SaveChanges();

                    id = POMasterTable.POMasterId;
                    // 

                    //POMasterTable.POMasterId = POMasterId.POMasterId;
                    //POMasterTable.VendorId = invoiceModel.VendorId;
                    //POMasterTable.VuniqueId = invoiceModel.VuniqueId;
                    //POMasterTable.PONumber = invoiceModel.PONumber;
                    //POMasterTable.CreatedDate = DateTime.Now;
                    //POMasterTable.CreatedBy = invoiceModel.VendorId;
                    //POMasterTable.ModifiedDate = DateTime.Now;
                    //POMasterTable.ModifiedBy_VuniqueId = invoiceModel.VuniqueId;
                    // vscm.SaveChanges();
                }
                else
                {
                    POMasterTable POMasterId = vscm.POMasterTables.Where(x => x.PONumber == invoiceModel.PONumber).FirstOrDefault();
                    id = POMasterId.POMasterId;
                }



                var invoiceDetails = new InvoiceDetail();

                invoiceDetails.POMasterId = id;
                invoiceDetails.InvoiceNo = invoiceModel.InvoiceNo;
                invoiceDetails.CreatedDate = DateTime.Now;
                invoiceDetails.CreatedBy = invoiceModel.VendorId;
                invoiceDetails.ModifiedBy = invoiceModel.VendorId;
                invoiceDetails.Remarks = invoiceModel.Remarks;
                invoiceDetails.ModifiedDate = DateTime.Now;

                vscm.InvoiceDetails.Add(invoiceDetails);

                vscm.SaveChanges();

                status.Sid = id;
                return status;
            }
            catch(Exception ex)
            {
                return null;
            }

        }

        public List<StagingPoSapModels> getPONumbersbyVendor(int vendorId)
        {
            var vendorDetails = vscm.RemoteVendorMasters.Where(x => x.Vendorid == vendorId).FirstOrDefault();
            var lstStagingPO = new List<StagingPoSapModels>();
            var lstStagingPO_Invoices = new List<StagingPoSapModels>();
           

            try
            {
                NpgsqlConnection conn = new NpgsqlConnection("Server=10.29.15.68;Port=5432;User Id=postgres;Password=1234;Database=WMS");
                conn.Open();

                // Define a query
                NpgsqlCommand cmd = new NpgsqlCommand("select distinct purchdoc from wms.stag_po_sap where vendor='" + vendorDetails.VendorCode + "'" , conn);

                // Execute a query
                NpgsqlDataReader dr = cmd.ExecuteReader();

                // Read all rows and output the first column in each row
                while (dr.Read())
                {
                    StagingPoSapModels stagingPoSapModels = new StagingPoSapModels();

                    stagingPoSapModels.purchdoc = dr["purchdoc"].ToString();
                    lstStagingPO.Add(stagingPoSapModels);
                }


                // Close connection
                conn.Close();
                var poMasterTable = new POMasterTable();
                StagingPoSapModels stag = new StagingPoSapModels();
                List<int> lstPOId = new List<int>();
                for(int i=0; i < lstStagingPO.Count; i++)
                {
                   long PONum = long.Parse(lstStagingPO[i].purchdoc);
                    poMasterTable = vscm.POMasterTables.Where(p => p.PONumber == PONum).FirstOrDefault();
                    if(poMasterTable != null)
                    {
                        if (poMasterTable.POMasterId != 0)
                        {
                            //lstPOId.Add(poMasterTable.POMasterId);
                            var lstInvoiveDetails = vscm.InvoiceDetails.Where(p => p.POMasterId == poMasterTable.POMasterId).ToList();
                            // lstStagingPO[i].POMasterTableId = poMasterTable.POMasterId;
                            for(int k =0; k < lstInvoiveDetails.Count; k++)
                            {
                                StagingPoSapModels stagingPoSap = new StagingPoSapModels();
                                stagingPoSap.invoiceNo = lstInvoiveDetails[k].InvoiceNo;
                                stagingPoSap.purchdoc = poMasterTable.PONumber.ToString();
                                lstStagingPO_Invoices.Add(stagingPoSap);
                            }
                        }
                    }
                   
                   
                   // var InvoiceNumber = vscm.InvoiceDetails.Where(x => x.POMasterId == Convert.ToInt32(poMasterTable.POMasterId));
                }


                //List<ParticularInvoice> lstParticularInvoice = new List<ParticularInvoice>();
                //ParticularInvoice particularInvoice = new ParticularInvoice();
                //StagingPoSapModels stag = new StagingPoSapModels();

                //for (int j = 0; j < lstPOId.Count; j++)
                //{
                //    int poNumber = lstPOId[j];
                //    var lstInvoice = vscm.InvoiceDetails.Where(p => p.POMasterId == poNumber).ToList();
                //   // var lstInvoice = vscm.InvoiceDetails.Where(p => p.POMasterId == poMasterTable.POMasterId).ToList();
                //    for (int k = 0; k < lstInvoice.Count; k++)
                //    {
                //        //particularInvoice.InvoiceNo = lstInvoice[k].InvoiceNo;
                //        //particularInvoice.POMasterTableId = lstInvoice[k].POMasterId;
                //        //lstParticularInvoice.Add(particularInvoice);
                //        lstStagingPO[k].particularInvoice = new ParticularInvoice
                //        {
                //            POMasterTableId = lstInvoice[k].POMasterId,
                //            InvoiceNo = lstInvoice[k].InvoiceNo
                //        };

                //    }

                //}

                lstStagingPO.AddRange(lstStagingPO_Invoices);
                return lstStagingPO;
                
            }
            catch(Exception ex)
            {
                throw;
            }
            //using (var db = new WMSModels("public"))
            //{
            //    var admin = db.stag_po_sap.ToList();
            //    return lstStagingPO;
            //}

           // var listPO = wmsEntities.stag_po_sap.ToList();
          
        }

    }
}
