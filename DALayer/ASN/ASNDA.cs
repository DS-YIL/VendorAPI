using DALayer.Common;
using DALayer.Emails;
using Dapper;
using Npgsql;
using SCMModels;
using SCMModels.RemoteModel;
using SCMModels.RFQModels;
using SCMModels.SCMModels;
using System;
using System.Collections.Generic;
using System.Configuration;
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
				NpgsqlConnection conn = new NpgsqlConnection(ConfigurationManager.AppSettings["WMSServerPath"]);
				conn.Open();
				// Define a query
				NpgsqlCommand cmd = new NpgsqlCommand("select distinct pono,sloc from wms.wms_polist where isclosed is false and vendorcode='" + vendorDetails.VendorCode + "' order by pono", conn);
				// Execute a query
				NpgsqlDataReader dr = cmd.ExecuteReader();
				// Read all rows and output the first column in each row
				while (dr.Read())
				{
					StagingPoSapModels stagingPoSapModels = new StagingPoSapModels();
					stagingPoSapModels.PONo = dr["pono"].ToString();
					var sloc = dr["sloc"].ToString();
					if (!string.IsNullOrEmpty(sloc))
					{
						var deliveryAddress = obj.StorageLocAddresses.Where(li => li.StorageLocation == sloc).FirstOrDefault();
						if (deliveryAddress != null)
						{
							stagingPoSapModels.ShipTo = deliveryAddress.Name + "," + deliveryAddress.Street + deliveryAddress.Street4 + deliveryAddress.City + "," + deliveryAddress.PostalCode;
							stagingPoSapModels.sloc = sloc;
							stagingPoSapModels.vendor = deliveryAddress.Vendor;
							stagingPoSapModels.Customer = deliveryAddress.Customer;
						}
					}
					var pores = vscm.RemoteASNItemDetails.Where(li => li.PONo == stagingPoSapModels.PONo).FirstOrDefault();
					//stagingPoSapModels.VendorAdress = vendorDetails.Street;
					//if (pores == null)
					lstStagingPO.Add(stagingPoSapModels);
				}
				// Close connection
				conn.Close();

			}
			catch (Exception ex)
			{
				log.ErrorMessage("ASNDA", "getPONumbersbyVendor", ex.Message + "; " + ex.StackTrace.ToString());

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
				NpgsqlConnection conn = new NpgsqlConnection(ConfigurationManager.AppSettings["WMSServerPath"]);
				conn.Open();
				// Define a query
				PONo = PONo.Replace(",", "','");
				string query = "select  wp.pono ,wp.vendorcode,pomat.materialid ,pomat.materialdescription ,pomat.poitemdescription,pomat.itemno,pomat.itemdeliverydate, pomat.materialqty, pomat.deliveredqty,mat.hsncode from wms.wms_polist wp  inner join wms.wms_pomaterials pomat on pomat.pono = wp.pono inner join wms.\"MaterialMasterYGS\" mat on mat.material = pomat.materialid where wp.pono in ('" + PONo + "') order by wp.pono";
				NpgsqlCommand cmd = new NpgsqlCommand(query, conn);
				// Execute a query
				NpgsqlDataReader dr = cmd.ExecuteReader();
				// Read all rows and output the first column in each row
				while (dr.Read())
				{
					PoItemDetails PoItemDetails = new PoItemDetails();
					PoItemDetails.PONo = dr["pono"].ToString();
					PoItemDetails.PODate = Convert.ToDateTime(dr["itemdeliverydate"]);
					//PoItemDetails.Approver1 = Convert.ToString(dr["approver1"]);
					//PoItemDetails.Approver2 = Convert.ToString(dr["approver2"]);
					PoItemDetails.VendorCode = Convert.ToString(dr["vendorcode"]);
					PoItemDetails.Material = Convert.ToString(dr["materialid"]);
					PoItemDetails.MaterialDescription = Convert.ToString(dr["materialdescription"]);
					PoItemDetails.poitemdescription = Convert.ToString(dr["poitemdescription"]);
					PoItemDetails.deliveredqty = Convert.ToDecimal(dr["deliveredqty"]);
					PoItemDetails.POItemNo = Convert.ToString(dr["itemno"]);
					PoItemDetails.HSNCode = Convert.ToString(dr["hsncode"]);
					PoItemDetails.POQty = Convert.ToDecimal(dr["materialqty"]);//quoted qty
					var res = vscm.RemoteASNItemDetails.ToList();
					RemoteASNItemDetail ASNitem = new RemoteASNItemDetail();
					if (res.Count > 0)
						ASNitem = vscm.RemoteASNItemDetails.Where(li => li.PONo == PoItemDetails.PONo && li.Material == PoItemDetails.Material && li.POItemNo == PoItemDetails.POItemNo).FirstOrDefault();
					if (ASNitem != null)
						PoItemDetails.SupplierCumulativeQty = ASNitem.SupplierCumulativeQty;//already delivered qty
					else
						PoItemDetails.SupplierCumulativeQty = PoItemDetails.deliveredqty;//already delivered qty
					PoItemDetails.RemainingQty = PoItemDetails.POQty - PoItemDetails.SupplierCumulativeQty;//remaining qty
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
		public int InsertandEditAsn(RemoteASNShipmentHeader model)
		{
			int ASNId = 0;
			try
			{
				var PONos = string.Join(",", (model.RemoteASNItemDetails.Select(c => c.PONo).ToList()).Distinct().ToArray());
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
						//asnShipmentHeaderModel.PODate = model.PODate;
						//asnShipmentHeaderModel.Approver1 = model.Approver1;
						//asnShipmentHeaderModel.Approver2 = model.Approver2;
						asnShipmentHeaderModel.InvoiceNo = model.InvoiceNo;
						asnShipmentHeaderModel.InvoiceDate = model.InvoiceDate;
						asnShipmentHeaderModel.ASNNo = "ASN/" + DateTime.Now.ToString("MMyy") + "/" + value;
						asnShipmentHeaderModel.SequenceNo = sequenceNo;
						asnShipmentHeaderModel.PONos = PONos;
						asnShipmentHeaderModel.VendorId = model.VendorId;
						asnShipmentHeaderModel.VendorName = model.VendorName;
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
							ASNItemDetails.PONo = item.PONo;
							ASNItemDetails.PODate = item.PODate;
							ASNItemDetails.POItemNo = item.POItemNo;
							ASNItemDetails.Material = item.Material;
							ASNItemDetails.MaterialDescription = item.MaterialDescription;
							ASNItemDetails.HSNCode = item.HSNCode;
							ASNItemDetails.POQty = item.POQty;//Quoted Qty						
							ASNItemDetails.SupplierCumulativeQty = item.ASNQty; //delivered Qty
							ASNItemDetails.RemainingQty = item.POQty - ASNItemDetails.SupplierCumulativeQty;//Remaining qty		
							ASNItemDetails.Remarks = item.Remarks;
							vscm.RemoteASNItemDetails.Add(ASNItemDetails);
							vscm.SaveChanges();
						}

						//update in YSCM
						ASNShipmentHeader asnShipmentHeaderModelLocal = new ASNShipmentHeader();
						asnShipmentHeaderModelLocal.ASNId = ASNId;
						//asnShipmentHeaderModelLocal.PONo = model.PONo;
						//asnShipmentHeaderModelLocal.PODate = model.PODate;
						//asnShipmentHeaderModelLocal.Approver1 = model.Approver1;
						//asnShipmentHeaderModelLocal.Approver2 = model.Approver2;
						asnShipmentHeaderModelLocal.InvoiceNo = model.InvoiceNo;
						asnShipmentHeaderModelLocal.InvoiceDate = model.InvoiceDate;
						asnShipmentHeaderModelLocal.ASNNo = asnShipmentHeaderModel.ASNNo;
						asnShipmentHeaderModelLocal.SequenceNo = asnShipmentHeaderModel.SequenceNo;
						asnShipmentHeaderModelLocal.PONos = PONos;
						asnShipmentHeaderModelLocal.VendorId = model.VendorId;
						asnShipmentHeaderModelLocal.VendorName = model.VendorName;
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
						//List<string> ponos = new List<string>();
						List<RemoteASNItemDetail> ASNItemDetailsList = vscm.RemoteASNItemDetails.Where(li => li.ASNId == ASNId).ToList();
						foreach (RemoteASNItemDetail itemlocal in ASNItemDetailsList)
						{
							ASNItemDetail ASNItemDetails = new ASNItemDetail();
							ASNItemDetails.ASNItemId = itemlocal.ASNItemId;
							ASNItemDetails.ASNId = itemlocal.ASNId;
							ASNItemDetails.PONo = itemlocal.PONo;
							//ponos.Add(ASNItemDetails.PONo);
							ASNItemDetails.PODate = itemlocal.PODate;
							ASNItemDetails.POItemNo = itemlocal.POItemNo;
							ASNItemDetails.Material = itemlocal.Material;
							ASNItemDetails.MaterialDescription = itemlocal.MaterialDescription;
							ASNItemDetails.HSNCode = itemlocal.HSNCode;
							ASNItemDetails.POQty = itemlocal.POQty;//Quoted Qty
							ASNItemDetails.SupplierCumulativeQty = itemlocal.SupplierCumulativeQty; //delivered Qty
							ASNItemDetails.RemainingQty = itemlocal.RemainingQty;//Remaining qty	
							ASNItemDetails.Remarks = itemlocal.Remarks;
							obj.ASNItemDetails.Add(ASNItemDetails);
							obj.SaveChanges();
						}

						//update ASN details in wms
						var pono = string.Join(",", PONos);

						updateASNDetailsinWMS(model, pono, asnShipmentHeaderModel.ASNNo);

					}
					else
					{
						RemoteASNShipmentHeader asnShipmentHeaderModel = vscm.RemoteASNShipmentHeaders.Where(li => li.ASNId == model.ASNId).FirstOrDefault();
						asnShipmentHeaderModel.InvoiceNo = model.InvoiceNo;
						asnShipmentHeaderModel.InvoiceDate = model.InvoiceDate;
						asnShipmentHeaderModel.PONos = PONos;
						asnShipmentHeaderModel.VendorId = model.VendorId;
						asnShipmentHeaderModel.VendorName = model.VendorName;
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
							Nullable<decimal> SupplierCumulativeQty = 0;
							if (ASNItemDetails.SupplierCumulativeQty != null)
								ASNItemDetails.SupplierCumulativeQty = ASNItemDetails.SupplierCumulativeQty + item.ASNQty; //delivered Qty
							ASNItemDetails.RemainingQty = ASNItemDetails.POQty - ASNItemDetails.SupplierCumulativeQty;
							ASNItemDetails.Remarks = item.Remarks;
							vscm.SaveChanges();
						}

						//Update in YSCM

						ASNShipmentHeader asnShipmentHeaderModelLocal = obj.ASNShipmentHeaders.Where(li => li.ASNId == model.ASNId).FirstOrDefault();
						asnShipmentHeaderModelLocal.InvoiceNo = model.InvoiceNo;
						asnShipmentHeaderModelLocal.InvoiceDate = model.InvoiceDate;
						asnShipmentHeaderModelLocal.PONos = PONos;
						asnShipmentHeaderModelLocal.VendorId = model.VendorId;
						asnShipmentHeaderModelLocal.VendorName = model.VendorName;
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
						//List<string> ponos = new List<string>();
						foreach (RemoteASNItemDetail item in ASNItemDetailsList)
						{
							//ponos.Add(item.PONo);
							ASNItemDetail ASNItemDetails = obj.ASNItemDetails.Where(li => li.ASNItemId == item.ASNItemId).FirstOrDefault();
							ASNItemDetails.HSNCode = item.HSNCode;
							ASNItemDetails.POQty = item.POQty;//Quoted Qty
							ASNItemDetails.SupplierCumulativeQty = item.SupplierCumulativeQty; //delivered Qty
							ASNItemDetails.RemainingQty = item.RemainingQty;
							ASNItemDetails.Remarks = item.Remarks;
							obj.SaveChanges();
						}
						//update ASN details in wms
						var pono = string.Join(",", PONos);

						updateASNDetailsinWMS(model, PONos, asnShipmentHeaderModel.ASNNo);
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
			return ASNId;
		}

		/*Name of Function : <<updateASNDetailsinWMS>>  Author :<<Prasanna>>  
		Date of Creation <<08-12-2020>>
		Purpose : <<function is used to  update ASNDetails from vendor portal  to WMS>>
		Review Date :<<>>   Reviewed By :<<>>*/
		public void updateASNDetailsinWMS(RemoteASNShipmentHeader asnData, string pono, string ASNNo)
		{
			try
			{
				using (NpgsqlConnection pgsql = new NpgsqlConnection(ConfigurationManager.AppSettings["WMSServerPath"]))
				{
					DateTime Date = Convert.ToDateTime(asnData.DeliveryDate);
					var DeliveryDate = Date.ToString("yyyy-MM-dd");
					Nullable<System.DateTime> itemdeliverydate = asnData.DeliveryDate;
					pgsql.Open();
					//string query1 = "Select Count(*) as count from wms.wms_asn where asn = '" + ASNNo + "'";
					//int pocount = int.Parse(pgsql.ExecuteScalar(query1, null).ToString());
					//if (pocount == 0)
					//{
					//	DateTime updatedon = DateTime.Now;
					//	string asn = ASNNo;
					//	//pono = "4507962964";
					//	string vendoruserid = asnData.CreatedBy;
					//	var insertquery = "INSERT INTO wms.wms_asn(pono, asn, deliverydate, updatedon, invoiceno, invoicedate, vendorid, vendoruserid)VALUES(@pono, @asn, @deliverydate,@updatedon , @invoiceno, @invoicedate,@vendorid, @vendoruserid)";
					//	var results = pgsql.Execute(insertquery, new
					//	{
					//		pono,
					//		asn,
					//		asnData.DeliveryDate,
					//		updatedon,
					//		asnData.InvoiceNo,
					//		asnData.InvoiceDate,
					//		asnData.VendorId,
					//		vendoruserid,
					//	});
					//}
					//else
					//{
					//	string qry = "Update wms.wms_asn set pono = " + pono + ",deliverydate = '" + DeliveryDate + "',invoiceno='" + asnData.InvoiceNo + "',invoicedate = '" + asnData.InvoiceDate + "' where asnno = '" + asnData.ASNNo + "'";
					//	var results11 = pgsql.ExecuteScalar(qry);
					//}


					foreach (RemoteASNItemDetail itemlocal in asnData.RemoteASNItemDetails)
					{

						//check asn no  already exist or not, if asno no not exist update asn no based on po and material 
						string query2 = "Select Count(*) as count from wms.wms_pomaterials where  asnno is  null and pono='" + itemlocal.PONo + "' and materialid='" + itemlocal.Material + "' and itemno='" + itemlocal.POItemNo + "'";
						int pocount2 = int.Parse(pgsql.ExecuteScalar(query2, null).ToString());
						if (pocount2 > 0)
						{
							string qry = "Update wms.wms_pomaterials set asnno= '" + ASNNo + "', itemdeliverydate = '" + DeliveryDate + "',asnqty = '" + itemlocal.ASNQty + "',invoiceno='" + asnData.InvoiceNo + "',billoflodingnumber='" + asnData.BillofLodingNumber + "' where  pono='" + itemlocal.PONo + "' and materialid='" + itemlocal.Material + "' and itemno='" + itemlocal.POItemNo + "'";
							var results11 = pgsql.ExecuteScalar(qry);
						}
						else
						{
							//for second time update if quantity is partially delivered first time new row will add in materials table
							string query3 = "Select Count(*) as count from wms.wms_pomaterials where  asnno ='" + ASNNo + "' and pono='" + itemlocal.PONo + "' and materialid='" + itemlocal.Material + "' and itemno='" + itemlocal.POItemNo + "'";
							int pocount3 = int.Parse(pgsql.ExecuteScalar(query3, null).ToString());
							if (pocount3 == 0)
							{
								string materialid = itemlocal.Material;
								decimal materialqty = Convert.ToDecimal(itemlocal.POQty);
								int itemno = Convert.ToInt32(itemlocal.POItemNo);
								itemlocal.MaterialDescription = itemlocal.MaterialDescription;
								string query = "select * from wms.wms_pomaterials pomat where pomat.pono ='" + itemlocal.PONo + "' and pomat.materialid='" + itemlocal.Material + "' and itemno ='" + itemlocal.POItemNo + "'";
								var result = pgsql.QueryFirstOrDefault<PoItemDetails>(
												query, null, commandType: CommandType.Text);
								var poitemdescription = result.poitemdescription;
								var unitprice = result.unitprice;
								var itemamount = result.itemamount;
								var insertquery = "INSERT INTO wms.wms_pomaterials(pono, materialid, materialdescription,poitemdescription,materialqty,itemno,itemdeliverydate,unitprice,itemamount,asnno,invoiceno,asnqty,billoflodingnumber)VALUES(@pono, @materialid, @materialdescription,@poitemdescription,@materialqty,@itemno,@itemdeliverydate,@unitprice,@itemamount,@asnno,@invoiceno,@asnqty,@billoflodingnumber)";
								var results = pgsql.Execute(insertquery, new
								{
									itemlocal.PONo,
									materialid,
									itemlocal.MaterialDescription,
									poitemdescription,
									materialqty,
									itemno,
									itemdeliverydate,
									unitprice,
									itemamount,
									ASNNo,
									asnData.InvoiceNo,
									itemlocal.ASNQty,
									asnData.BillofLodingNumber
								});
							}

							else
							{
								string qry = "Update wms.wms_pomaterials set  itemdeliverydate = " + DeliveryDate + ",asnqty = '" + itemlocal.ASNQty + "',invoiceno='" + asnData.InvoiceNo + "',billoflodingnumber='" + asnData.BillofLodingNumber + "' where asnno='" + ASNNo + "' and  pono ='" + itemlocal.PONo + "' and materialid='" + itemlocal.Material + "' and itemno='" + itemlocal.POItemNo + "'";
								var results11 = pgsql.ExecuteScalar(qry);
							}

						}
					}
				}
			}
			catch (DbEntityValidationException e)
			{
				foreach (var eve in e.EntityValidationErrors)
				{
					Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
						eve.Entry.Entity.GetType().Name, eve.Entry.State);
					foreach (var ve in eve.ValidationErrors)
					{
						log.ErrorMessage("ASNDA", "updateASNDetailsinWMS", ve.ErrorMessage);
						Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
							ve.PropertyName, ve.ErrorMessage);
					}
				}

			}

		}

		/*Name of Function : <<getAsnList>>  Author :<<Prasanna>>  
		Date of Creation <<07-12-2020>>
		Purpose : <<function is used to  getAsnList>>
		Review Date :<<>>   Reviewed By :<<>>*/
		public List<RemoteASNShipmentHeader> getAsnList(ASNfilters ASNfilters)
		{
			List<RemoteASNShipmentHeader> asnList = new List<RemoteASNShipmentHeader>();
			try
			{
				using (VSCMEntities Context = new VSCMEntities())
				{
					Context.Configuration.ProxyCreationEnabled = false;
					var query = default(string);
					query = "select * from RemoteASNShipmentHeader where ( Deleteflag=1 or Deleteflag is null)";
					if (!string.IsNullOrEmpty(ASNfilters.ToDate))
						query += " and CreatedDate <= '" + ASNfilters.ToDate + "'";
					if (!string.IsNullOrEmpty(ASNfilters.FromDate))
						query += "  and CreatedDate >= '" + ASNfilters.FromDate + "'";
					if (!string.IsNullOrEmpty(ASNfilters.Vendorid))
						query += "  and VendorId = '" + ASNfilters.Vendorid + "'";
					if (!string.IsNullOrEmpty(ASNfilters.VendorName))
						query += "  and VendorName = '" + ASNfilters.VendorName + "'";
					if (!string.IsNullOrEmpty(ASNfilters.ASNNo))
						query += "  and ASNNo = '" + ASNfilters.ASNNo + "'";
					query += " order by ASNId desc ";
					asnList = Context.RemoteASNShipmentHeaders.SqlQuery(query).ToList<RemoteASNShipmentHeader>();
				}
				return asnList;
			}
			catch (Exception ex)
			{
				throw;
			}
		}

		/*Name of Function : <<getAsnDetailsByAsnNo>>  Author :<<Prasanna>>  
		Date of Creation <<04-12-2020>>
		Purpose : <<function is used to  getAsnDetailsByAsnNo>>
		Review Date :<<>>   Reviewed By :<<>>*/
		public RemoteASNShipmentHeader getAsnDetailsByAsnNo(int ASNId)
		{
			RemoteASNShipmentHeader ASNDetails = new RemoteASNShipmentHeader();
			try
			{
				ASNDetails = vscm.RemoteASNShipmentHeaders.Where(li => li.ASNId == ASNId).FirstOrDefault();
				foreach (RemoteASNCommunication item in ASNDetails.RemoteASNCommunications)
				{
					item.Employee = obj.VendorEmployeeViews.Where(li => li.EmployeeNo == item.RemarksFrom).FirstOrDefault();
				}

			}
			catch (Exception ex)
			{
				throw;
			}
			return ASNDetails;
		}

		/*Name of Function : <<updateASNComminications>>  Author :<<Prasanna>>  
		Date of Creation <<07-12-2020>>
		Purpose : <<function is used updateASNComminications>>
		Review Date :<<>>   Reviewed By :<<>>*/
		public bool updateASNComminications(RemoteASNCommunication asncom)
		{
			try
			{
				int ASNCCId = 0;
				//insert in vscm
				RemoteASNCommunication com = vscm.RemoteASNCommunications.Where(li => li.ASNCCId == asncom.ASNCCId).FirstOrDefault();
				if (com == null)
				{
					RemoteASNCommunication asnComRemote = new RemoteASNCommunication();
					asnComRemote.ASNId = asncom.ASNId;
					asnComRemote.Remarks = asncom.Remarks;
					asnComRemote.RemarksFrom = asncom.RemarksFrom;
					asnComRemote.RemarksDate = DateTime.Now;
					vscm.RemoteASNCommunications.Add(asnComRemote);
					vscm.SaveChanges();
					ASNCCId = asnComRemote.ASNId;
				}

				//inert in yscm
				ASNCommunication comLocal = obj.ASNCommunications.Where(li => li.ASNCCId == asncom.ASNCCId).FirstOrDefault();
				if (comLocal == null)
				{
					ASNCommunication asnComLocal = new ASNCommunication();
					asnComLocal.ASNCCId = ASNCCId;
					asnComLocal.ASNId = asncom.ASNId;
					asnComLocal.Remarks = asncom.Remarks;
					asnComLocal.RemarksFrom = asncom.RemarksFrom;
					asnComLocal.RemarksDate = DateTime.Now;
					obj.ASNCommunications.Add(asnComLocal);
					obj.SaveChanges();
				}
			}
			catch (Exception ex)
			{
				log.ErrorMessage("ASNDA", "updateASNComminications", ex.Message + "; " + ex.StackTrace.ToString());
			}
			return true;
		}

		public RemoteInvoiceDetail GetInvoiceDetails(RemoteInvoiceDetail invoiceDetails)
		{
			RemoteInvoiceDetail invDetails = new RemoteInvoiceDetail();
			try
			{
				invDetails = vscm.RemoteInvoiceDetails.Where(li => li.InvoiceNo == invoiceDetails.InvoiceNo && li.ASNId == invoiceDetails.ASNId).FirstOrDefault();
				// invoiceDetails.

			}
			catch (Exception ex)
			{
				log.ErrorMessage("ASNDA", "GetInvoiceDetails", ex.Message + "; " + ex.StackTrace.ToString());
			}
			return invDetails;
		}

		/*Name of Function : <<UpdateInvoice>>  Author :<<Prasanna>>  
		Date of Creation <<18-11-2020>>
		Purpose : <<function is used to UpdateInvoice>>
		Review Date :<<>>   Reviewed By :<<>>*/
		public async Task<RemoteInvoiceDetail> UpdateInvoice(RemoteInvoiceDetail invoiceModel)
		{
			RemoteInvoiceDetail InvoiceDetails = new RemoteInvoiceDetail();
			try
			{
				int invoiceId = 0;
				var invoiceData = vscm.RemoteInvoiceDetails.Where(li => li.InvoiceId == invoiceModel.InvoiceId).FirstOrDefault();
				if (invoiceData == null)
				{
					var invoiceDetails = new RemoteInvoiceDetail();
					invoiceDetails.ASNId = invoiceModel.ASNId;
					invoiceDetails.InvoiceNo = invoiceModel.InvoiceNo;
					invoiceDetails.VendorId = invoiceModel.VendorId;
					invoiceDetails.CreatedDate = DateTime.Now;
					invoiceDetails.CreatedBy = invoiceModel.CreatedBy;
					invoiceDetails.Remarks = invoiceModel.Remarks;
					vscm.RemoteInvoiceDetails.Add(invoiceDetails);
					vscm.SaveChanges();
					invoiceId = invoiceDetails.InvoiceId;
				}
				else
				{
					invoiceData.InvoiceNo = invoiceModel.InvoiceNo;
					invoiceData.VendorId = invoiceModel.VendorId;
					invoiceData.ModifiedBy = invoiceModel.CreatedBy;
					invoiceData.ModifiedDate = DateTime.Now;
					invoiceData.Remarks = invoiceModel.Remarks;
					vscm.SaveChanges();
					invoiceId = invoiceData.InvoiceId;
				}
				foreach (var item in invoiceModel.RemoteInvoiceDocuments)
				{
					var invoiceDoc = vscm.RemoteInvoiceDocuments.Where(li => li.DocumentId == item.DocumentId).FirstOrDefault();
					if (invoiceDoc == null)
					{
						var invoicedocuments = new RemoteInvoiceDocument();
						invoicedocuments.InvoiceId = invoiceId;
						invoicedocuments.DocumentTypeId = item.DocumentTypeId;
						invoicedocuments.DocumentName = item.DocumentName;
						invoicedocuments.Path = item.Path;
						invoicedocuments.UploadedDate = DateTime.Now;
						invoicedocuments.IsDeleted = false;
						invoicedocuments.UploadedBy = item.UploadedBy;
						vscm.RemoteInvoiceDocuments.Add(invoicedocuments);
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
				var LocalinvoiceData = obj.InvoiceDetails.Where(li => li.InvoiceId == invoiceId).FirstOrDefault();
				if (invoiceData == null)
				{
					var invoiceDetails = new InvoiceDetail();
					invoiceDetails.InvoiceId = invoiceId;
					invoiceDetails.ASNId = invoiceModel.ASNId;
					invoiceDetails.InvoiceNo = invoiceModel.InvoiceNo;
					invoiceDetails.VendorId = invoiceModel.VendorId;
					invoiceDetails.CreatedDate = DateTime.Now;
					invoiceDetails.CreatedBy = invoiceModel.CreatedBy;
					invoiceDetails.Remarks = invoiceModel.Remarks;
					obj.InvoiceDetails.Add(invoiceDetails);
					obj.SaveChanges();
				}
				else
				{
					invoiceData.InvoiceNo = invoiceModel.InvoiceNo;
					invoiceData.VendorId = invoiceModel.VendorId;
					invoiceData.ModifiedBy = invoiceModel.CreatedBy;
					invoiceData.ModifiedDate = DateTime.Now;
					invoiceData.Remarks = invoiceModel.Remarks;
					obj.SaveChanges();
				}
				var RemoteInvoiceDocuments = vscm.RemoteInvoiceDocuments.Where(li => li.InvoiceId == invoiceId).ToList();
				foreach (var item in RemoteInvoiceDocuments)
				{
					var invoiceDoc = obj.InvoiceDocuments.Where(li => li.DocumentId == item.DocumentId).FirstOrDefault();
					if (invoiceDoc == null)
					{
						var invoicedocuments = new InvoiceDocument();
						invoicedocuments.DocumentId = item.DocumentId;
						invoicedocuments.InvoiceId = invoiceId;
						invoicedocuments.DocumentTypeId = item.DocumentTypeId;
						invoicedocuments.DocumentName = item.DocumentName;
						invoicedocuments.Path = item.Path;
						invoicedocuments.UploadedDate = DateTime.Now;
						invoicedocuments.IsDeleted = false;
						invoicedocuments.UploadedBy = item.UploadedBy;
						obj.InvoiceDocuments.Add(invoicedocuments);
						obj.SaveChanges();
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
						obj.SaveChanges();
					}
				}

				InvoiceDetails = vscm.RemoteInvoiceDetails.Where(li => li.InvoiceId == invoiceId).FirstOrDefault();
				InvoiceDetails.RemoteInvoiceDocuments = InvoiceDetails.RemoteInvoiceDocuments.Where(li => li.IsDeleted != true).ToList();
				emailTemplateDA.sendInvoiceMailtoBuyer(invoiceModel.InvoiceNo);
			}
			catch (Exception ex)
			{
				log.ErrorMessage("ASNDA", "UpdateInvoice", ex.Message + "; " + ex.StackTrace.ToString());

			}
			return InvoiceDetails;

		}


		/*Name of Function : <<getPOInvoiceDetailsbyVendor>>  Author :<<Prasanna>>  
		Date of Creation <<24-11-2020>>
		Purpose : <<function is used to  get open po numbers based on vendor id>>
		Review Date :<<>>   Reviewed By :<<>>*/

		//public List<StagingPoSapModels> getPOInvoiceDetailsbyVendor(int vendorId)
		//{
		//	DataTable POTable = new DataTable();
		//	var lstStagingPO_Invoices = new List<StagingPoSapModels>();
		//	try
		//	{
		//		var vendorDetails = vscm.RemoteVendorMasters.Where(x => x.Vendorid == vendorId).FirstOrDefault();
		//		var lstStagingPO = new List<StagingPoSapModels>();
		//		NpgsqlConnection conn = new NpgsqlConnection(ConfigurationManager.AppSettings["WMSServerPath"]);
		//		conn.Open();
		//		// Define a query
		//		NpgsqlCommand cmd = new NpgsqlCommand("select distinct pono from wms.wms_polist where vendorcode='" + vendorDetails.VendorCode + "'", conn);
		//		// Execute a query
		//		NpgsqlDataReader dr = cmd.ExecuteReader();
		//		// Read all rows and output the first column in each row
		//		while (dr.Read())
		//		{
		//			StagingPoSapModels stagingPoSapModels = new StagingPoSapModels();
		//			stagingPoSapModels.PONo = dr["pono"].ToString();
		//			lstStagingPO.Add(stagingPoSapModels);
		//		}
		//		// Close connection
		//		conn.Close();

		//		for (int i = 0; i < lstStagingPO.Count; i++)
		//		{
		//			StagingPoSapModels stagingPoSapModels = new StagingPoSapModels();
		//			stagingPoSapModels.PONo = lstStagingPO[i].PONo;
		//			stagingPoSapModels.InvoiceDetails = vscm.RemoteInvoiceDetails.Where(li => li.PONO == stagingPoSapModels.PONo).ToList();
		//			foreach (RemoteInvoiceDetails item in stagingPoSapModels.InvoiceDetails)
		//			{
		//				item.InvoiceDocuments = item.InvoiceDocuments.Where(li => li.IsDeleted != true).ToList();
		//			}
		//			lstStagingPO_Invoices.Add(stagingPoSapModels);
		//		}
		//	}
		//	catch (Exception ex)
		//	{
		//		log.ErrorMessage("ASNDA", "getPOInvoiceDetailsbyVendor", ex.Message + "; " + ex.StackTrace.ToString());

		//	}
		//	return lstStagingPO_Invoices;
		//}


		/*Name of Function : <<DeleteInvoiceFile>>  Author :<<Prasanna>>  
		Date of Creation <<18-11-2020>>
		Purpose : <<function is used to DeleteInvoiceFile>>
		Review Date :<<>>   Reviewed By :<<>>*/
		public bool DeleteInvoiceFile(int DocumentId)
		{
			RemoteInvoiceDocument invDoc = vscm.RemoteInvoiceDocuments.Where(li => li.DocumentId == DocumentId).FirstOrDefault();
			if (invDoc != null)
			{
				invDoc.IsDeleted = true;
				vscm.SaveChanges();
			}
			InvoiceDocument invDocLoc = obj.InvoiceDocuments.Where(li => li.DocumentId == DocumentId).FirstOrDefault();
			if (invDoc != null)
			{
				invDoc.IsDeleted = true;
			}
			return true;
		}
	}
}
