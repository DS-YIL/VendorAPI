using DALayer.Common;
using Npgsql;
using SCMModels;
using SCMModels.ASNModels;
using SCMModels.RemoteModel;
using SCMModels.RFQModels;
using SCMModels.SCMModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DALayer.ASN
{
	public class ASNDA : IASNDA
	{
		VSCMEntities vscm = new VSCMEntities();
		private ErrorLog log = new ErrorLog();
		public bool CreateAsn(ASNShipmentHeaderModel model)
		{
			try
			{
				var asnShipmentHeaderModels = new ASNShipmentHeader();
				var asnShipmentLines = new ASNShipmentLine();
				var asnItemDetails = new ASNItemDetail();
				var asnStatus = new ASNStatu();
				if (model != null)
				{
					vscm.Database.Connection.Open();
					if (model.ASNId == 0)
					{
						Int64 sequenceNo = Convert.ToInt64(vscm.ASNShipmentHeaders.Max(li => li.SequenceNo));
						if (sequenceNo == null || sequenceNo == 0)
							sequenceNo = 1;
						else
						{
							sequenceNo = sequenceNo + 1;
						}
						var value = vscm.RemoteSP_sequenceNumber(sequenceNo).FirstOrDefault();
						asnShipmentHeaderModels.PO_ReferenceNo = model.PO_ReferenceNo;
						asnShipmentHeaderModels.InvoiceNo = model.InvoiceNo;
						asnShipmentHeaderModels.ASNNo = "ASN/" + DateTime.Now.ToString("MMyy") + "/" + value;

						asnShipmentHeaderModels.SequenceNo = sequenceNo;
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
					}
					//update
					else
					{
						asnShipmentHeaderModels = vscm.ASNShipmentHeaders.Where(x => x.ASNId == model.ASNId).FirstOrDefault();
						if (asnShipmentHeaderModels != null)
						{
							asnShipmentHeaderModels.PO_ReferenceNo = model.PO_ReferenceNo;
							asnShipmentHeaderModels.InvoiceNo = model.InvoiceNo;
							asnShipmentHeaderModels.ASNNo = model.ASNNo;
							asnShipmentHeaderModels.HSNCode = model.HSNCode;
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

						if (asnShipmentLines != null)
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
						if (asnItemDetails != null)
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

						if (asnStatus != null)
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

					}
				}
			}
			catch (Exception e)
			{
				log.ErrorMessage("ASNDA", "CreateAsn", e.Message.ToString() + e.InnerException.ToString() + e.ToString());

			}
			return true;
		}

		public List<ASNShipmentHeader> getAsnList()
		{
			List<ASNShipmentHeader> asnList = new List<ASNShipmentHeader>();
			try
			{
				asnList = vscm.ASNShipmentHeaders.OrderByDescending(x => x.ASNId).ToList();
				return asnList;
			}
			catch (Exception ex)
			{
				throw;
			}
		}
		public ASNShipmentHeaderModel getAsnDetailsByAsnNo(int ASNId)
		{
			try
			{
				ASNShipmentHeaderModel asnShipmentHeaderModel = new ASNShipmentHeaderModel();

				var asnShipmentHeader = vscm.ASNShipmentHeaders.Where(x => x.ASNId == ASNId).FirstOrDefault();

				asnShipmentHeaderModel.PO_ReferenceNo = asnShipmentHeader.PO_ReferenceNo;
				asnShipmentHeaderModel.ASNId = asnShipmentHeader.ASNId;

				var asnStatus = vscm.ASNStatus.Where(x => x.ASNId == asnShipmentHeaderModel.ASNId).FirstOrDefault();
				var asnShipmentLines = vscm.ASNShipmentLines.Where(x => x.ASNId == asnShipmentHeaderModel.ASNId).FirstOrDefault();
				var asnItemDetails = vscm.ASNItemDetails.Where(x => x.ASNId == asnShipmentHeaderModel.ASNId).FirstOrDefault();

				asnShipmentHeaderModel.InvoiceNo = asnShipmentHeader.InvoiceNo;
				asnShipmentHeaderModel.ASNNo = asnShipmentHeader.ASNNo;
				asnShipmentHeaderModel.HSNCode = asnShipmentHeader.HSNCode;
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
				asnShipmentHeaderModel.TotalGrossWeight_Kgs = Convert.ToSingle(asnItemDetails.TotalGrossWt_Kgs);
				asnShipmentHeaderModel.TotalNetWeight_Kgs = Convert.ToSingle(asnItemDetails.TotalNetWt_Kgs);
				asnShipmentHeaderModel.CustomerBatchNo = asnItemDetails.CustomerBatchNo;
				asnShipmentHeaderModel.SupplierBatchNo = asnItemDetails.SupplierBatchNo;

				return asnShipmentHeaderModel;

			}
			catch (Exception ex)
			{
				throw;
			}
		}

		public InvoiceModel GetInvoiceDetails(string InvoiceNo)
		{
			try
			{
				InvoiceDetail invoiceDetails = new InvoiceDetail();
				POMasterTable pOMasterTable = new POMasterTable();

				InvoiceModel invoiceModel = new InvoiceModel();

				invoiceDetails = vscm.InvoiceDetails.Where(x => x.InvoiceNo == InvoiceNo).FirstOrDefault();
				invoiceModel.InvoiceNo = invoiceDetails.InvoiceNo;
				invoiceModel.Remarks = invoiceDetails.Remarks;

				return invoiceModel;
				// invoiceDetails.

			}
			catch (Exception ex)
			{
				return null;
			}
		}

		public async Task<InvoiceDetail> UpdateInvoice(InvoiceDetail invoiceModel)
		{
			InvoiceDetail InvoiceDetails = new InvoiceDetail();
			try
			{
				int invoiceId = 0;
				var invoiceData = vscm.InvoiceDetails.Where(li => li.InvoiceId == invoiceModel.InvoiceId).FirstOrDefault();
				if (invoiceData == null)
				{
					var invoiceDetails = new InvoiceDetail();
					invoiceDetails.InvoiceNo = invoiceModel.InvoiceNo;
					invoiceDetails.PONO = invoiceModel.PONO;
					invoiceDetails.VendorId = invoiceModel.VendorId;
					invoiceDetails.CreatedDate = DateTime.Now;
					invoiceDetails.CreatedBy = invoiceModel.CreatedBy;
					invoiceDetails.Remarks = invoiceModel.Remarks;
					vscm.InvoiceDetails.Add(invoiceDetails);
					vscm.SaveChanges();
					invoiceId = invoiceDetails.InvoiceId;
				}
				else
				{
					invoiceData.InvoiceNo = invoiceModel.InvoiceNo;
					invoiceData.PONO = invoiceModel.PONO;
					invoiceData.VendorId = invoiceModel.VendorId;
					invoiceData.ModifiedBy = invoiceModel.CreatedBy;
					invoiceData.ModifiedDate = DateTime.Now;
					invoiceData.Remarks = invoiceModel.Remarks;
					vscm.SaveChanges();
					invoiceId = invoiceData.InvoiceId;
				}
				foreach (var item in invoiceModel.InvoiceDocuments)
				{
					var invoiceDoc = vscm.InvoiceDocuments.Where(li => li.DocumentId == item.DocumentId).FirstOrDefault();
					if (invoiceDoc == null)
					{
						var invoicedocuments = new InvoiceDocument();
						invoicedocuments.InvoiceId = invoiceId;
						invoicedocuments.DocumentTypeId = item.DocumentTypeId;
						invoicedocuments.DocumentName = item.DocumentName;
						invoicedocuments.Path = item.Path;
						invoicedocuments.UploadedDate = DateTime.Now;
						invoicedocuments.IsDeleted = false;
						invoicedocuments.UploadedBy = item.UploadedBy;
						vscm.InvoiceDocuments.Add(invoicedocuments);
						vscm.SaveChanges();
					}
					else
					{
						invoiceDoc.InvoiceId = invoiceId;
						invoiceDoc.DocumentTypeId = item.DocumentTypeId;
						invoiceDoc.DocumentName = item.DocumentName;
						invoiceDoc.Path = item.Path;
						invoiceDoc.UploadedDate = DateTime.Now;
						invoiceDoc.IsDeleted = false;
						invoiceDoc.UploadedBy = item.UploadedBy;
						vscm.SaveChanges();
					}
				}
				InvoiceDetails = vscm.InvoiceDetails.Where(li => li.InvoiceId == invoiceId).FirstOrDefault();
				InvoiceDetails.InvoiceDocuments = InvoiceDetails.InvoiceDocuments.Where(li => li.IsDeleted != true).ToList();
			}
			catch (Exception e)
			{
				log.ErrorMessage("ASNDA", "UpdateInvoice", e.Message.ToString() + e.InnerException.ToString() + e.ToString());

			}
			return InvoiceDetails;

		}

		public DataTable getPONumbersbyVendor(int vendorId)
		{
			DataTable POTable = new DataTable();
			POTable.TableName = "POTable";
			try
			{
				using (YSCMEntities Context = new YSCMEntities())
				{
					string query = "select distinct pt.PONO,mprpa.VendorId,null as HSNCode from MPRPADetails mprpa inner join PAItem pt on pt.PAID=mprpa.PAId where  PAStatus='Approved' and POStatus='PO Released' and pt.PONO is not null and mprpa.VendorId=" + vendorId + "";
					var cmd = Context.Database.Connection.CreateCommand();
					cmd.CommandText = query;
					cmd.Connection.Open();
					POTable.Load(cmd.ExecuteReader());
					cmd.Connection.Close();

				}
			}
			catch (Exception e)
			{
				log.ErrorMessage("ASNDA", "getPONumbersbyVendor", e.Message.ToString() + e.InnerException.ToString() + e.ToString());

			}
			return POTable;
		}

		public List<StagingPoSapModels> getPOInvoiceDetailsbyVendor(int vendorId)
		{
			DataTable POTable = new DataTable();
			var lstStagingPO_Invoices = new List<StagingPoSapModels>();
			try
			{
				using (YSCMEntities Context = new YSCMEntities())
				{
					string query = "select distinct pt.PONO,mprpa.VendorId,null as HSNCode from MPRPADetails mprpa inner join PAItem pt on pt.PAID=mprpa.PAId where  PAStatus='Approved' and POStatus='PO Released' and pt.PONO is not null and mprpa.VendorId=" + vendorId + "";
					var cmd = Context.Database.Connection.CreateCommand();
					cmd.CommandText = query;
					cmd.Connection.Open();
					POTable.Load(cmd.ExecuteReader());

					// Read all rows and output the first column in each row

					foreach (DataRow row in POTable.Rows)
					{

						StagingPoSapModels stagingPoSapModels = new StagingPoSapModels();

						stagingPoSapModels.PONO = row["PONO"].ToString();
						stagingPoSapModels.InvoiceDetails = vscm.InvoiceDetails.Where(li => li.PONO == stagingPoSapModels.PONO).ToList();
						foreach (InvoiceDetail item in stagingPoSapModels.InvoiceDetails)
						{
							item.InvoiceDocuments = item.InvoiceDocuments.Where(li => li.IsDeleted != true).ToList();
						}
						lstStagingPO_Invoices.Add(stagingPoSapModels);
					}


					// Close connection
					cmd.Connection.Close();
				}
			}
			catch (Exception e)
			{
				log.ErrorMessage("ASNDA", "getPOInvoiceDetailsbyVendor", e.Message.ToString() + e.InnerException.ToString() + e.ToString());

			}
			return lstStagingPO_Invoices;
		}

		public bool DeleteInvoiceFile(int DocumentId)
		{
			InvoiceDocument invDoc = vscm.InvoiceDocuments.Where(li => li.DocumentId == DocumentId).FirstOrDefault();
			if (invDoc != null)
			{
				invDoc.IsDeleted = true;
				vscm.SaveChanges();
			}
			return true;
		}
	}
}
