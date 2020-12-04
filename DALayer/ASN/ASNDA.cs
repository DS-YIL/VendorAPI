using DALayer.Common;
using DALayer.Emails;
using Npgsql;
using SCMModels;
using SCMModels.RemoteModel;
using SCMModels.RFQModels;
using SCMModels.SCMModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Validation;
using System.Linq;
using System.Threading.Tasks;

namespace DALayer.ASN
{
	public class ASNDA : IASNDA
	{
		private IEmailTemplateDA emailTemplateDA = default(IEmailTemplateDA);
		private ErrorLog log = new ErrorLog();
		public ASNDA(IEmailTemplateDA EmailTemplateDA)
		{
			this.emailTemplateDA = EmailTemplateDA;
		}

		VSCMEntities vscm = new VSCMEntities();
		YSCMEntities obj = new YSCMEntities();
		/*Name of Function : <<getPONumbersbyVendor>>  Author :<<Prasanna>>  
		Date of Creation <<24-11-2020>>
		Purpose : <<function is used to  get open po numbers based on vendor id>>
		Review Date :<<>>   Reviewed By :<<>>*/
		public List<StagingPoSapModels> getPONumbersbyVendor(int vendorId)
		{
			var vendorDetails = vscm.RemoteVendorMasters.Where(x => x.Vendorid == vendorId).FirstOrDefault();
			var lstStagingPO = new List<StagingPoSapModels>();
			try
			{
				NpgsqlConnection conn = new NpgsqlConnection("Server=10.29.15.212;Port=5432;User Id=postgres;Password=Yil@pgdb@212*_;Database=WMS");
				conn.Open();
				// Define a query
				NpgsqlCommand cmd = new NpgsqlCommand("select distinct pono from wms.wms_polist where vendorcode='" + vendorDetails.VendorCode + "'", conn);
				// Execute a query
				NpgsqlDataReader dr = cmd.ExecuteReader();
				// Read all rows and output the first column in each row
				while (dr.Read())
				{
					StagingPoSapModels stagingPoSapModels = new StagingPoSapModels();
					stagingPoSapModels.PONO = dr["pono"].ToString();
					var pores = vscm.RemoteASNShipmentHeaders.Where(li => li.PONo == stagingPoSapModels.PONO).FirstOrDefault();
					//stagingPoSapModels.VendorAdress = vendorDetails.Street;
					if (pores == null)
						lstStagingPO.Add(stagingPoSapModels);
				}
				// Close connection
				conn.Close();

			}
			catch (Exception e)
			{
				log.ErrorMessage("ASNDA", "getPONumbersbyVendor", e.Message.ToString() + e.InnerException.ToString() + e.ToString());

			}
			return lstStagingPO;
		}

		/*Name of Function : <<getItemDetailsByPoNo>>  Author :<<Prasanna>>  
		Date of Creation <<24-11-2020>>
		Purpose : <<function is used to  get itemdetails by PoNo>>
		Review Date :<<>>   Reviewed By :<<>>*/
		public List<PoItemDetails> getItemDetailsByPoNo(string PONo)
		{
			var PoItemDetailsList = new List<PoItemDetails>();
			try
			{
				NpgsqlConnection conn = new NpgsqlConnection("Server=10.29.15.212;Port=5432;User Id=postgres;Password=Yil@pgdb@212*_;Database=WMS");
				conn.Open();
				// Define a query
				NpgsqlCommand cmd = new NpgsqlCommand("select  wp.pono ,wp.vendorcode,pomat.materialid ,pomat.materialdescription ,pomat.itemno,pomat.itemdeliverydate, pomat.materialqty, mat.hsncode from wms.wms_polist wp  inner join wms.wms_pomaterials pomat on pomat.pono = wp.pono inner join wms.\"MaterialMasterYGS\" mat on mat.material = pomat.materialid where wp.pono='" + PONo + "'", conn);
				// Execute a query
				NpgsqlDataReader dr = cmd.ExecuteReader();
				// Read all rows and output the first column in each row
				while (dr.Read())
				{
					PoItemDetails PoItemDetails = new PoItemDetails();
					PoItemDetails.PONO = dr["pono"].ToString();
					PoItemDetails.PODate = Convert.ToDateTime(dr["itemdeliverydate"]);
					PoItemDetails.VendorCode = Convert.ToString(dr["vendorcode"]);
					PoItemDetails.Material = Convert.ToString(dr["materialid"]);
					PoItemDetails.MaterialDescription = Convert.ToString(dr["materialdescription"]);
					PoItemDetails.POItemNo = Convert.ToString(dr["itemno"]);
					PoItemDetails.HSNCode = Convert.ToString(dr["hsncode"]);
					PoItemDetails.POQty = Convert.ToDecimal(dr["materialqty"]);//quoted qty
					var res = vscm.RemoteASNItemDetails.ToList();
					RemoteASNItemDetail ASNitem = new RemoteASNItemDetail();
					if (res.Count > 0)
						ASNitem = vscm.RemoteASNItemDetails.Where(li => li.PONo == PoItemDetails.PONO && li.Material == PoItemDetails.Material).FirstOrDefault();
					if (ASNitem != null)
						PoItemDetails.SupplierCumulativeQty = ASNitem.SupplierCumulativeQty;//already delivered qty
					PoItemDetails.PoCumulativeQty = PoItemDetails.POQty - PoItemDetails.SupplierCumulativeQty;//remaining qty
					PoItemDetailsList.Add(PoItemDetails);
				}
				// Close connection
				conn.Close();

				//using (YSCMEntities Context = new YSCMEntities())
				//{
				//	string query = "select * from ItemdetailsByPoNoView where PONO ='4508494794'";
				//	var cmd = Context.Database.Connection.CreateCommand();
				//	cmd.CommandText = query;
				//	cmd.Connection.Open();
				//	ItemTable.Load(cmd.ExecuteReader());
				//	cmd.Connection.Close();

				//}
			}
			catch (DbEntityValidationException e)
			{
				foreach (var eve in e.EntityValidationErrors)
				{
					Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
						eve.Entry.Entity.GetType().Name, eve.Entry.State);
					foreach (var ve in eve.ValidationErrors)
					{
						log.ErrorMessage("ASNDA", "getItemDetailsByPoNo", ve.ErrorMessage);
						Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
							ve.PropertyName, ve.ErrorMessage);
					}
				}

			}
			return PoItemDetailsList;

		}

		/*Name of Function : <<CreateAsn>>  Author :<<Prasanna>>  
		Date of Creation <<26-11-2020>>
		Purpose : <<function is used to  Create ASN >>
		Review Date :<<>>   Reviewed By :<<>>*/
		public bool InsertandEditAsn(RemoteASNShipmentHeader model)
		{
			int ASNId = 0;
			try
			{
				if (model != null)
				{
					if (model.ASNId == 0)
					{
						RemoteASNShipmentHeader asnShipmentHeaderModel = new RemoteASNShipmentHeader();
						Int32 sequenceNo = Convert.ToInt32(vscm.RemoteASNShipmentHeaders.Max(li => li.SequenceNo));
						if (sequenceNo == null || sequenceNo == 0)
							sequenceNo = 1;
						else
						{
							sequenceNo = sequenceNo + 1;
						}
						var value = vscm.RemoteSP_sequenceNumber(sequenceNo).FirstOrDefault();
						asnShipmentHeaderModel.PONo = model.PONo;
						asnShipmentHeaderModel.InvoiceNo = model.InvoiceNo;
						asnShipmentHeaderModel.InvoiceDate = model.InvoiceDate;
						asnShipmentHeaderModel.ASNNo = "ASN/" + DateTime.Now.ToString("MMyy") + "/" + value;
						asnShipmentHeaderModel.SequenceNo = sequenceNo;
						asnShipmentHeaderModel.VendorId = model.VendorId;
						asnShipmentHeaderModel.ShipFrom = model.ShipFrom;
						asnShipmentHeaderModel.ShipTo = model.ShipTo;
						asnShipmentHeaderModel.ShippingDate = model.ShippingDate;
						asnShipmentHeaderModel.DeliveryDate = model.DeliveryDate;
						asnShipmentHeaderModel.FreightInvNo = model.FreightInvNo;
						asnShipmentHeaderModel.TransporterName = model.TransporterName;
						asnShipmentHeaderModel.BillofLodingNumber = model.BillofLodingNumber;
						asnShipmentHeaderModel.IncoTerm = model.IncoTerm;
						asnShipmentHeaderModel.IncotermLoc = model.IncotermLoc;
						asnShipmentHeaderModel.IncotermDescription = model.IncotermDescription;

						asnShipmentHeaderModel.ModeOfTransport = model.ModeOfTransport;
						asnShipmentHeaderModel.DeliveryNote = model.DeliveryNote;
						asnShipmentHeaderModel.TotalGrossWeight_Kgs = model.TotalGrossWeight_Kgs;
						asnShipmentHeaderModel.TotalNetWeight_Kgs = model.TotalNetWeight_Kgs;
						asnShipmentHeaderModel.TotalVolume = model.TotalVolume;
						asnShipmentHeaderModel.Insurance = model.Insurance;
						asnShipmentHeaderModel.CreatedBy = model.CreatedBy;
						asnShipmentHeaderModel.CreatedDate = DateTime.Now;
						vscm.RemoteASNShipmentHeaders.Add(asnShipmentHeaderModel);
						vscm.SaveChanges();
						ASNId = asnShipmentHeaderModel.ASNId;
						foreach (RemoteASNItemDetail item in model.RemoteASNItemDetails)
						{
							RemoteASNItemDetail ASNItemDetails = new RemoteASNItemDetail();
							ASNItemDetails.ASNId = ASNId;
							ASNItemDetails.PONo = asnShipmentHeaderModel.PONo;
							ASNItemDetails.POItemNo = item.POItemNo;
							ASNItemDetails.Material = item.Material;
							ASNItemDetails.MaterialDescription = item.MaterialDescription;
							ASNItemDetails.HSNCode = item.HSNCode;
							ASNItemDetails.POQty = item.POQty;//Quoted Qty						
							ASNItemDetails.SupplierCumulativeQty = item.ASNQty; //delivered Qty
							ASNItemDetails.PoCumulativeQty = item.POQty - ASNItemDetails.SupplierCumulativeQty;//Remaining qty		
							ASNItemDetails.Remarks = item.Remarks;
							vscm.RemoteASNItemDetails.Add(ASNItemDetails);
							vscm.SaveChanges();
						}

						//update in YSCM
						ASNShipmentHeader asnShipmentHeaderModelLocal = new ASNShipmentHeader();
						asnShipmentHeaderModelLocal.ASNId = ASNId;
						asnShipmentHeaderModelLocal.PONo = model.PONo;
						asnShipmentHeaderModelLocal.InvoiceNo = model.InvoiceNo;
						asnShipmentHeaderModelLocal.InvoiceDate = model.InvoiceDate;
						asnShipmentHeaderModelLocal.ASNNo = asnShipmentHeaderModel.ASNNo;
						asnShipmentHeaderModelLocal.SequenceNo = asnShipmentHeaderModel.SequenceNo;
						asnShipmentHeaderModelLocal.VendorId = model.VendorId;
						asnShipmentHeaderModelLocal.ShipFrom = model.ShipFrom;
						asnShipmentHeaderModelLocal.ShipTo = model.ShipTo;
						asnShipmentHeaderModelLocal.ShippingDate = model.ShippingDate;
						asnShipmentHeaderModelLocal.DeliveryDate = model.DeliveryDate;
						asnShipmentHeaderModelLocal.FreightInvNo = model.FreightInvNo;
						asnShipmentHeaderModelLocal.TransporterName = model.TransporterName;
						asnShipmentHeaderModelLocal.BillofLodingNumber = model.BillofLodingNumber;
						asnShipmentHeaderModelLocal.IncoTerm = model.IncoTerm;
						asnShipmentHeaderModelLocal.IncotermLoc = model.IncotermLoc;
						asnShipmentHeaderModelLocal.IncotermDescription = model.IncotermDescription;
						asnShipmentHeaderModelLocal.ModeOfTransport = model.ModeOfTransport;
						asnShipmentHeaderModelLocal.DeliveryNote = model.DeliveryNote;
						asnShipmentHeaderModelLocal.TotalGrossWeight_Kgs = model.TotalGrossWeight_Kgs;
						asnShipmentHeaderModelLocal.TotalNetWeight_Kgs = model.TotalNetWeight_Kgs;
						asnShipmentHeaderModelLocal.TotalVolume = model.TotalVolume;
						asnShipmentHeaderModelLocal.Insurance = model.Insurance;
						asnShipmentHeaderModelLocal.CreatedBy = model.CreatedBy;
						asnShipmentHeaderModelLocal.CreatedDate = DateTime.Now;
						obj.ASNShipmentHeaders.Add(asnShipmentHeaderModelLocal);
						obj.SaveChanges();

						List<RemoteASNItemDetail> ASNItemDetailsList = vscm.RemoteASNItemDetails.Where(li => li.ASNId == ASNId).ToList();
						foreach (RemoteASNItemDetail itemlocal in ASNItemDetailsList)
						{
							ASNItemDetail ASNItemDetails = new ASNItemDetail();
							ASNItemDetails.ASNItemId = itemlocal.ASNItemId;
							ASNItemDetails.ASNId = itemlocal.ASNId;
							ASNItemDetails.PONo = itemlocal.PONo;
							ASNItemDetails.POItemNo = itemlocal.POItemNo;
							ASNItemDetails.Material = itemlocal.Material;
							ASNItemDetails.MaterialDescription = itemlocal.MaterialDescription;
							ASNItemDetails.HSNCode = itemlocal.HSNCode;
							ASNItemDetails.POQty = itemlocal.POQty;//Quoted Qty
																   //ASNItemDetails.ASNQty = itemlocal.ASNQty;//entered by vendor		
							ASNItemDetails.SupplierCumulativeQty = itemlocal.SupplierCumulativeQty; //delivered Qty
							ASNItemDetails.PoCumulativeQty = itemlocal.PoCumulativeQty;//Remaining qty	
							ASNItemDetails.Remarks = itemlocal.Remarks;
							obj.ASNItemDetails.Add(ASNItemDetails);
							obj.SaveChanges();
						}
					}
					else
					{
						RemoteASNShipmentHeader asnShipmentHeaderModel = vscm.RemoteASNShipmentHeaders.Where(li => li.ASNId == model.ASNId).FirstOrDefault();
						asnShipmentHeaderModel.InvoiceNo = model.InvoiceNo;
						asnShipmentHeaderModel.InvoiceDate = model.InvoiceDate;
						asnShipmentHeaderModel.VendorId = model.VendorId;
						asnShipmentHeaderModel.ShipFrom = model.ShipFrom;
						asnShipmentHeaderModel.ShipTo = model.ShipTo;
						asnShipmentHeaderModel.ShippingDate = model.ShippingDate;
						asnShipmentHeaderModel.DeliveryDate = model.DeliveryDate;
						asnShipmentHeaderModel.FreightInvNo = model.FreightInvNo;
						asnShipmentHeaderModel.TransporterName = model.TransporterName;
						asnShipmentHeaderModel.BillofLodingNumber = model.BillofLodingNumber;
						asnShipmentHeaderModel.IncoTerm = model.IncoTerm;
						asnShipmentHeaderModel.IncotermLoc = model.IncotermLoc;
						asnShipmentHeaderModel.IncotermDescription = model.IncotermDescription;
						asnShipmentHeaderModel.ModeOfTransport = model.ModeOfTransport;
						asnShipmentHeaderModel.DeliveryNote = model.DeliveryNote;
						asnShipmentHeaderModel.TotalGrossWeight_Kgs = model.TotalGrossWeight_Kgs;
						asnShipmentHeaderModel.TotalNetWeight_Kgs = model.TotalNetWeight_Kgs;
						asnShipmentHeaderModel.TotalVolume = model.TotalVolume;
						asnShipmentHeaderModel.Insurance = model.Insurance;
						asnShipmentHeaderModel.CreatedBy = model.CreatedBy;
						asnShipmentHeaderModel.CreatedDate = DateTime.Now;
						vscm.SaveChanges();
						ASNId = asnShipmentHeaderModel.ASNId;
						foreach (RemoteASNItemDetail item in model.RemoteASNItemDetails)
						{
							RemoteASNItemDetail ASNItemDetails = vscm.RemoteASNItemDetails.Where(li => li.ASNItemId == item.ASNItemId).FirstOrDefault();
							ASNItemDetails.HSNCode = item.HSNCode;
							ASNItemDetails.POQty = item.POQty;//Quoted Qty
															  //ASNItemDetails.ASNQty = item.ASNQty;//entered by vendor		
							Nullable<decimal> SupplierCumulativeQty = 0;
							if (ASNItemDetails.SupplierCumulativeQty != null)
								ASNItemDetails.SupplierCumulativeQty = ASNItemDetails.SupplierCumulativeQty + item.ASNQty; //delivered Qty
							ASNItemDetails.PoCumulativeQty = ASNItemDetails.POQty - ASNItemDetails.SupplierCumulativeQty;
							ASNItemDetails.Remarks = item.Remarks;
							vscm.SaveChanges();
						}

						//Update in YSCM

						ASNShipmentHeader asnShipmentHeaderModelLocal = obj.ASNShipmentHeaders.Where(li => li.ASNId == model.ASNId).FirstOrDefault();
						asnShipmentHeaderModelLocal.InvoiceNo = model.InvoiceNo;
						asnShipmentHeaderModelLocal.InvoiceDate = model.InvoiceDate;
						asnShipmentHeaderModelLocal.VendorId = model.VendorId;
						asnShipmentHeaderModelLocal.ShipFrom = model.ShipFrom;
						asnShipmentHeaderModelLocal.ShipTo = model.ShipTo;
						asnShipmentHeaderModelLocal.ShippingDate = model.ShippingDate;
						asnShipmentHeaderModelLocal.DeliveryDate = model.DeliveryDate;
						asnShipmentHeaderModelLocal.FreightInvNo = model.FreightInvNo;
						asnShipmentHeaderModelLocal.TransporterName = model.TransporterName;
						asnShipmentHeaderModelLocal.BillofLodingNumber = model.BillofLodingNumber;
						asnShipmentHeaderModelLocal.IncoTerm = model.IncoTerm;
						asnShipmentHeaderModelLocal.IncotermLoc = model.IncotermLoc;
						asnShipmentHeaderModelLocal.IncotermDescription = model.IncotermDescription;
						asnShipmentHeaderModelLocal.ModeOfTransport = model.ModeOfTransport;
						asnShipmentHeaderModelLocal.DeliveryNote = model.DeliveryNote;
						asnShipmentHeaderModelLocal.TotalGrossWeight_Kgs = model.TotalGrossWeight_Kgs;
						asnShipmentHeaderModelLocal.TotalNetWeight_Kgs = model.TotalNetWeight_Kgs;
						asnShipmentHeaderModelLocal.TotalVolume = model.TotalVolume;
						asnShipmentHeaderModelLocal.Insurance = model.Insurance;
						obj.SaveChanges();
						ASNId = asnShipmentHeaderModel.ASNId;

						List<RemoteASNItemDetail> ASNItemDetailsList = vscm.RemoteASNItemDetails.Where(li => li.ASNId == ASNId).ToList();

						foreach (RemoteASNItemDetail item in ASNItemDetailsList)
						{
							ASNItemDetail ASNItemDetails = obj.ASNItemDetails.Where(li => li.ASNItemId == item.ASNItemId).FirstOrDefault();
							ASNItemDetails.HSNCode = item.HSNCode;
							ASNItemDetails.POQty = item.POQty;//Quoted Qty
															  //ASNItemDetails.ASNQty = item.ASNQty;//entered by vendor		
							ASNItemDetails.SupplierCumulativeQty = item.SupplierCumulativeQty; //delivered Qty
							ASNItemDetails.PoCumulativeQty = item.PoCumulativeQty;
							ASNItemDetails.Remarks = item.Remarks;
							obj.SaveChanges();
						}
					}
				}
				emailTemplateDA.sendASNMailtoBuyer(ASNId);
			}
			catch (DbEntityValidationException e)
			{
				foreach (var eve in e.EntityValidationErrors)
				{
					Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
						eve.Entry.Entity.GetType().Name, eve.Entry.State);
					foreach (var ve in eve.ValidationErrors)
					{
						log.ErrorMessage("ASNDA", "InsertandEditAsn", ve.ErrorMessage);
						Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
							ve.PropertyName, ve.ErrorMessage);
					}
				}

			}
			return true;
		}

		public List<RemoteASNShipmentHeader> getAsnList()
		{
			List<RemoteASNShipmentHeader> asnList = new List<RemoteASNShipmentHeader>();
			try
			{
				asnList = vscm.RemoteASNShipmentHeaders.OrderByDescending(x => x.ASNId).ToList();
				return asnList;
			}
			catch (Exception ex)
			{
				throw;
			}
		}
		public RemoteASNShipmentHeader getAsnDetailsByAsnNo(int ASNId)
		{
			RemoteASNShipmentHeader ASNDetails = new RemoteASNShipmentHeader();
			try
			{
				return vscm.RemoteASNShipmentHeaders.Where(li => li.ASNId == ASNId).FirstOrDefault();

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


		/*Name of Function : <<getPONumbersbyVendor>>  Author :<<Prasanna>>  
		Date of Creation <<24-11-2020>>
		Purpose : <<function is used to  get open po numbers based on vendor id>>
		Review Date :<<>>   Reviewed By :<<>>*/

		public List<StagingPoSapModels> getPOInvoiceDetailsbyVendor(int vendorId)
		{
			DataTable POTable = new DataTable();
			var lstStagingPO_Invoices = new List<StagingPoSapModels>();
			try
			{
				var vendorDetails = vscm.RemoteVendorMasters.Where(x => x.Vendorid == vendorId).FirstOrDefault();
				var lstStagingPO = new List<StagingPoSapModels>();
				NpgsqlConnection conn = new NpgsqlConnection("Server=10.29.15.212;Port=5432;User Id=postgres;Password=Wms@1234*;Database=WMS");
				conn.Open();
				// Define a query
				NpgsqlCommand cmd = new NpgsqlCommand("select distinct pono from wms.wms_polist where vendorid='" + vendorDetails.VendorCode + "'", conn);
				// Execute a query
				NpgsqlDataReader dr = cmd.ExecuteReader();
				// Read all rows and output the first column in each row
				while (dr.Read())
				{
					StagingPoSapModels stagingPoSapModels = new StagingPoSapModels();
					stagingPoSapModels.PONO = dr["pono"].ToString();
					lstStagingPO.Add(stagingPoSapModels);
				}
				// Close connection
				conn.Close();

				for (int i = 0; i < lstStagingPO.Count; i++)
				{
					StagingPoSapModels stagingPoSapModels = new StagingPoSapModels();
					stagingPoSapModels.PONO = lstStagingPO[i].PONO;
					stagingPoSapModels.InvoiceDetails = vscm.InvoiceDetails.Where(li => li.PONO == stagingPoSapModels.PONO).ToList();
					foreach (InvoiceDetail item in stagingPoSapModels.InvoiceDetails)
					{
						item.InvoiceDocuments = item.InvoiceDocuments.Where(li => li.IsDeleted != true).ToList();
					}
					lstStagingPO_Invoices.Add(stagingPoSapModels);
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
