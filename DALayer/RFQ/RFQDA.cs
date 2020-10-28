using DALayer.Common;
using DALayer.Emails;
using SCMModels;
using SCMModels.MPRMasterModels;
using SCMModels.RemoteModel;
using SCMModels.RFQModels;
using SCMModels.SCMModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace DALayer.RFQ
{
	public class RFQDA : IRFQDA
	{
		private IEmailTemplateDA emailTemplateDA = default(IEmailTemplateDA);
		private ErrorLog log = new ErrorLog();
		public RFQDA(IEmailTemplateDA EmailTemplateDA)
		{
			this.emailTemplateDA = EmailTemplateDA;
		}

		VSCMEntities vscm = new VSCMEntities();
		YSCMEntities obj = new YSCMEntities();

		public async Task<statuscheckmodel> UpdateRfqRevision(RfqRevisionModel model)
		{
			statuscheckmodel status = new statuscheckmodel();

			try
			{
				vscm.Database.Connection.Open();
				var remotedata = vscm.RemoteRFQRevisions.Where(x => x.rfqRevisionId == model.RfqRevisionId).FirstOrDefault<RemoteRFQRevision>();
				if (remotedata != null)
				{
					RemoteRFQRevision revision = new RemoteRFQRevision();
					remotedata.SalesTax = model.salesTax;
					remotedata.RFQValidDate = model.RfqValidDate;
					remotedata.CreatedBy = model.CreatedBy;
					remotedata.PaymentTermDays = model.PaymentTermDays;
					remotedata.DeliveryMaxWeeks = model.DeliveryMaxWeeks;
					remotedata.DeliveryMinWeeks = model.DeliveryMinWeeks;
					remotedata.BankGuarantee = model.BankGuarantee;
					remotedata.ExciseDuty = model.ExciseDuty;
					remotedata.CustomsDuty = model.CustomsDuty;
					remotedata.Insurance = model.Insurance;
					remotedata.Freight = model.freight;
					remotedata.PackingForwarding = model.PackingForwading;
					remotedata.CreatedDate = model.CreatedDate;
					vscm.SaveChanges();
				}
				vscm.Database.Connection.Close();
				obj.Database.Connection.Open();
				var result = obj.RFQRevisions_N.Where(x => x.rfqRevisionId == model.RfqRevisionId).Include(x => x.RFQMaster).FirstOrDefault<RFQRevisions_N>();
				if (result != null)
				{
					RFQRevision revision = new RFQRevision();
					result.SalesTax = model.salesTax;
					result.RFQValidDate = model.RfqValidDate;
					result.CreatedBy = model.CreatedBy;
					result.PaymentTermDays = model.PaymentTermDays;
					result.DeliveryMaxWeeks = model.DeliveryMaxWeeks;
					result.DeliveryMinWeeks = model.DeliveryMinWeeks;
					result.BankGuarantee = model.BankGuarantee;
					result.ExciseDuty = model.ExciseDuty;
					result.CustomsDuty = model.CustomsDuty;
					result.Insurance = model.Insurance;
					result.Freight = model.freight;
					result.PackingForwarding = model.PackingForwading;
					result.CreatedDate = model.CreatedDate;

					obj.SaveChanges();

				}

				status.Sid = model.RfqMasterId;

			}
			catch (Exception e)
			{

				log.ErrorMessage("RFQDA", "UpdateRfqRevision", e.Message.ToString() + e.InnerException.ToString() + e.ToString());
			}
			return status;
		}
		public async Task<statuscheckmodel> UpdateBulkRfqRevision(RfqRevisionModel model)
		{
			statuscheckmodel status = new statuscheckmodel();
			var result = obj.RFQRevisions_N.Where(x => x.rfqMasterId == model.RfqMasterId).Include(x => x.RFQMaster).ToList<RFQRevisions_N>();
			try
			{

				if (result != null)
				{
					foreach (var item in result)
					{
						item.SalesTax = model.salesTax;
						item.RFQValidDate = model.RfqValidDate;
						item.CreatedBy = model.CreatedBy;
						item.PaymentTermDays = model.PaymentTermDays;
						item.DeliveryMaxWeeks = model.DeliveryMaxWeeks;
						item.DeliveryMinWeeks = model.DeliveryMinWeeks;
						item.BankGuarantee = model.BankGuarantee;
						item.ExciseDuty = model.ExciseDuty;
						item.CustomsDuty = model.CustomsDuty;
						item.Insurance = model.Insurance;
						item.Freight = model.freight;
						item.PackingForwarding = model.PackingForwading;
						item.CreatedDate = model.CreatedDate;

						obj.SaveChanges();
					}

				}
				status.Sid = model.RfqMasterId;

			}
			catch (Exception ex)
			{

				log.ErrorMessage("RFQDA", "UpdateBulkRfqRevision", ex.Message + "; " + ex.StackTrace.ToString());
			}
			return status;
		}
		public async Task<statuscheckmodel> UpdateRfqItemByBulk(RfqItemModel model)
		{
			statuscheckmodel status = new statuscheckmodel();
			try
			{
				vscm.Database.Connection.Open();
				var Remoteresult = vscm.RemoteRFQItems_N.Where(x => x.RFQRevisionId == model.RFQRevisionId).Include(x => x.RemoteRFQRevisions_N).ToList<RemoteRFQItems_N>();

				if (Remoteresult != null)
				{
					foreach (var item in Remoteresult)
					{
						item.HSNCode = model.HSNCode;
						item.QuotationQty = model.QuotationQty;
						item.VendorModelNo = model.VendorModelNo;
						item.RequestRemarks = model.RequsetRemarks;
						item.CGSTPercentage = model.CGSTPercentage;
						item.SGSTPercentage = model.SGSTPercentage;
						item.IGSTPercentage = model.IGSTPercentage;
						item.FreightAmount = model.FreightAmount;
						item.FreightPercentage = model.FreightPercentage;
						item.PFAmount = model.PFAmount;
						item.PFPercentage = model.PFPercentage;
						item.RequestRemarks = model.RequsetRemarks;
						obj.SaveChanges();
					}
				}
				vscm.Database.Connection.Close();

				obj.Database.Connection.Open();

				var result = obj.RFQItems.Where(x => x.RFQRevisionId == model.RFQRevisionId).Include(x => x.RFQRevision).ToList<RFQItem>();

				if (result != null)
				{
					foreach (var item in result)
					{
						item.HSNCode = model.HSNCode;
						item.QuotationQty = model.QuotationQty;
						item.VendorModelNo = model.VendorModelNo;
						item.RequestRemarks = model.RequsetRemarks;
						item.CGSTPercentage = model.CGSTPercentage;
						item.SGSTPercentage = model.SGSTPercentage;
						item.IGSTPercentage = model.IGSTPercentage;
						item.FreightAmount = model.FreightAmount;
						item.FreightPercentage = model.FreightPercentage;
						item.PFAmount = model.PFAmount;
						item.PFPercentage = model.PFPercentage;
						item.RequestRemarks = model.RequsetRemarks;
						obj.SaveChanges();
					}
				}

			}
			catch (Exception ex)
			{
				log.ErrorMessage("RFQDA", "UpdateRfqItemByBulk", ex.Message + "; " + ex.StackTrace.ToString());
			}
			return status;
		}
		public async Task<List<RfqItemModel>> GetItemsByRevisionId(int revisionid)
		{
			List<RfqItemModel> rfq = new List<RfqItemModel>();
			try
			{
				var data = vscm.RemoteRFQItems_N.Where(x => x.RFQRevisionId == revisionid).ToList();
				var itemid = data.Select(x => new RfqItemModel()
				{
					RFQItemID = x.RFQItemsId,

				}).FirstOrDefault();
				var iteminfodata = vscm.RemoteRFQItemsInfo_N.ToList();

				foreach (var item in data)
				{
					rfq.Add(new RfqItemModel()
					{
						RFQItemID = item.RFQItemsId,
						HSNCode = item.HSNCode,
						ItemDescription = item.ItemDescription,
						ItemName = item.ItemName,
						QuotationQty = item.QuotationQty,
						VendorModelNo = item.VendorModelNo,
						CustomDuty = Convert.ToDecimal(item.CustomDuty),
						RequsetRemarks = item.RequestRemarks,
						iteminfo = iteminfodata.Where(x => x.RFQItemsId == item.RFQItemsId && x.DeleteFlag == false).Select(x => new RfqItemInfoModel()
						{
							RFQSplitItemId = x.RFQSplitItemId,
							Quantity = x.Qty,
							UOM = x.UOM,
							UnitPrice = x.UnitPrice,
							DiscountPercentage = x.DiscountPercentage,
							Discount = x.Discount,
							CurrencyID = x.CurrencyId,
							CurrencyValue = x.CurrencyValue,
							Remarks = x.Remarks
						}).ToList()
					});

				}



			}

			catch (Exception ex)
			{
				log.ErrorMessage("RFQDA", "GetItemsByRevisionId", ex.Message + "; " + ex.StackTrace.ToString());
			}
			return rfq;
		}

		public async Task<statuscheckmodel> UpdateSingleRfqItem(RfqItemModel model)
		{
			statuscheckmodel status = new statuscheckmodel();
			try
			{
				vscm.Database.Connection.Open();
				var remotedata = vscm.RemoteRFQItems_N.Where(x => x.RFQItemsId == model.RFQItemID).Include(x => x.RemoteRFQRevisions_N).FirstOrDefault<RemoteRFQItems_N>();
				if (remotedata != null)
				{
					remotedata.HSNCode = model.HSNCode;
					remotedata.QuotationQty = model.QuotationQty;
					remotedata.VendorModelNo = model.VendorModelNo;
					remotedata.RequestRemarks = model.RequsetRemarks;
					remotedata.CGSTPercentage = model.CGSTPercentage;
					remotedata.SGSTPercentage = model.SGSTPercentage;
					remotedata.IGSTPercentage = model.IGSTPercentage;
					remotedata.FreightAmount = model.FreightAmount;
					remotedata.FreightPercentage = model.FreightPercentage;
					remotedata.PFAmount = model.PFAmount;
					remotedata.PFPercentage = model.PFPercentage;
					remotedata.RequestRemarks = model.RequsetRemarks;
					vscm.SaveChanges();
				}
				vscm.Database.Connection.Close();

				obj.Database.Connection.Open();
				var data = obj.RFQItems.Where(x => x.RFQItemsId == model.RFQItemID).Include(x => x.RFQRevision).FirstOrDefault<RFQItem>();
				if (data != null)
				{
					data.HSNCode = model.HSNCode;
					data.QuotationQty = model.QuotationQty;
					data.VendorModelNo = model.VendorModelNo;
					data.RequestRemarks = model.RequsetRemarks;
					data.CGSTPercentage = model.CGSTPercentage;
					data.SGSTPercentage = model.SGSTPercentage;
					data.IGSTPercentage = model.IGSTPercentage;
					data.FreightAmount = model.FreightAmount;
					data.FreightPercentage = model.FreightPercentage;
					data.PFAmount = model.PFAmount;
					data.PFPercentage = model.PFPercentage;
					data.RequestRemarks = model.RequsetRemarks;
					obj.SaveChanges();
				}
				int id = remotedata.RFQItemsId;
				status.Sid = id;

			}
			catch (Exception ex)
			{
				log.ErrorMessage("RFQDA", "UpdateSingleRfqItem", ex.Message + "; " + ex.StackTrace.ToString());
			}
			return status;
		}
		public statuscheckmodel DeleteRfqRevisionbyId(int id)
		{
			statuscheckmodel staus = new statuscheckmodel();
			try
			{
				var data = obj.RFQRevisions.Where(x => x.rfqRevisionId == id && x.DeleteFlag == false).Select(x => x).SingleOrDefault();
				if (data != null)
				{
					data.DeleteFlag = true;
					obj.SaveChanges();
				}

			}
			catch (Exception ex)
			{

				log.ErrorMessage("RFQDA", "DeleteRfqRevisionbyId", ex.Message + "; " + ex.StackTrace.ToString());

			}
			return staus;
		}
		public statuscheckmodel DeleteRfqItemById(int id)
		{
			statuscheckmodel status = new statuscheckmodel();
			try
			{
				var data = obj.RFQItems.Where(x => x.RFQRevisionId == id && x.DeleteFlag == false).Select(x => x).SingleOrDefault();
				if (data != null)
				{
					data.DeleteFlag = true;
					obj.SaveChanges();
				}

			}
			catch (Exception ex)
			{
				log.ErrorMessage("RFQDA", "DeleteRfqItemById", ex.Message + "; " + ex.StackTrace.ToString());
			}
			return status;
		}
		public statuscheckmodel DeleteBulkItemsByItemId(List<int> id)
		{
			statuscheckmodel status = new statuscheckmodel();
			try
			{
				var data = obj.RFQItems.Where(x => id.Contains(x.RFQItemsId)).ToList();
				if (data != null)
				{
					data.Select(x => x.DeleteFlag == true);
					obj.SaveChanges();
				}
				return status;
			}
			catch (Exception ex)
			{
				throw;
			}
		}
		public List<RfqDocumentsModel> InsertDocument(RfqDocumentsModel model)
		{
			List<RfqDocumentsModel> listobj = new List<RfqDocumentsModel>();
			RfqDocumentsModel _eachobj = new RfqDocumentsModel();
			try
			{
				int rfqdocid = 0;
				if (model != null)
				{
					var remotedataforDocumentdetails = new RemoteRFQDocument();
					remotedataforDocumentdetails.DocumentName = model.DocumentName;

					remotedataforDocumentdetails.UploadedBy = model.UploadedBy;
					remotedataforDocumentdetails.uploadedDate = model.UploadedDate;

					string updatedRevisionId = model.RfqRevisionId;
					if (updatedRevisionId.Contains("Technical"))
					{
						remotedataforDocumentdetails.rfqRevisionId = Convert.ToInt32(updatedRevisionId.Replace("Technical", ""));
						remotedataforDocumentdetails.rfqItemsid = model.RfqItemsId;
					}
					else
					{
						remotedataforDocumentdetails.rfqRevisionId = Convert.ToInt32(model.RfqRevisionId);
					}

					remotedataforDocumentdetails.DocumentType = model.DocumentType;
					remotedataforDocumentdetails.Path = model.Path;
					remotedataforDocumentdetails.DeleteFlag = model.IsDeleted;
					vscm.RemoteRFQDocuments.Add(remotedataforDocumentdetails);

					vscm.SaveChanges();
					rfqdocid = remotedataforDocumentdetails.RfqDocId;
					var remotedataforDocumentdetail = new RFQDocument();
					remotedataforDocumentdetail.DocumentName = model.DocumentName;

					remotedataforDocumentdetail.UploadedBy = model.UploadedBy;
					remotedataforDocumentdetail.UploadedDate = model.UploadedDate;
					if (updatedRevisionId.Contains("Technical"))
					{
						remotedataforDocumentdetail.rfqRevisionId = Convert.ToInt32(updatedRevisionId.Replace("Technical", ""));
						remotedataforDocumentdetail.rfqItemsid = model.RfqItemsId;

					}
					else
					{
						remotedataforDocumentdetail.rfqRevisionId = Convert.ToInt32(model.RfqRevisionId);
					}

					remotedataforDocumentdetail.DocumentType = model.DocumentType;
					remotedataforDocumentdetail.Path = model.Path;
					remotedataforDocumentdetail.DeleteFlag = model.IsDeleted;
					remotedataforDocumentdetail.RfqDocId = rfqdocid;
					obj.RFQDocuments.Add(remotedataforDocumentdetail);
					obj.SaveChanges();
					_eachobj.errmsg = "OK";
					listobj.Add(_eachobj);

				}
			}
			catch (Exception ex)
			{
				log.ErrorMessage("RFQDA", "InsertDocument", ex.Message + "; " + ex.StackTrace.ToString());
			}
			return listobj;
		}
		public List<RfqDocumentsModel> InsertDocumentTOYSCM(RfqDocumentsModel model)
		{
			List<RfqDocumentsModel> listobj = new List<RfqDocumentsModel>();
			try
			{

				if (model != null)
				{
					var remotedataforDocumentdetail = new RFQDocument();
					remotedataforDocumentdetail.DocumentName = model.DocumentName;

					remotedataforDocumentdetail.UploadedBy = model.UploadedBy;
					remotedataforDocumentdetail.UploadedDate = model.UploadedDate;

					string updatedRevisionId = model.RfqRevisionId;
					if (updatedRevisionId.Contains("Technical"))
					{
						remotedataforDocumentdetail.rfqRevisionId = Convert.ToInt32(updatedRevisionId.Replace("Technical", ""));
						remotedataforDocumentdetail.rfqItemsid = model.RfqItemsId;
					}
					else
					{
						remotedataforDocumentdetail.rfqRevisionId = Convert.ToInt32(model.RfqRevisionId);
					}

					remotedataforDocumentdetail.DocumentType = model.DocumentType;
					remotedataforDocumentdetail.Path = model.Path;
					remotedataforDocumentdetail.DeleteFlag = model.IsDeleted;
					obj.RFQDocuments.Add(remotedataforDocumentdetail);
					obj.SaveChanges();

				}
			}
			catch (Exception ex)
			{
				throw;
			}
			return listobj;
		}
		public async Task<RfqItemModel> GetItemsByItemId(int id)
		{
			RfqItemModel model = new RfqItemModel();
			try
			{
				var items = from x in obj.RFQItems where x.RFQItemsId == id && x.DeleteFlag == false select x;
				foreach (var item in items)
				{
					model.HSNCode = item.HSNCode;
					model.QuotationQty = item.QuotationQty;
					model.VendorModelNo = item.VendorModelNo;
					model.RequsetRemarks = item.RequestRemarks;
					model.CGSTPercentage = item.CGSTPercentage;
					model.IGSTPercentage = item.IGSTPercentage;
					model.SGSTPercentage = item.SGSTPercentage;
					model.CustomDuty = item.CustomDuty;
					model.FreightAmount = item.FreightAmount;
					model.FreightPercentage = item.FreightPercentage;
				}
				return model;
			}
			catch (Exception ex)
			{
				throw;
			}
		}
		public async Task<RfqItemModel> GetItemByItemId(int id)
		{
			RfqItemModel model = new RfqItemModel();
			try
			{
				//var items = from x in obj.RFQItems where x.RFQItemsId == id && x.DeleteFlag == false select x;
				var items = obj.RFQItems.Where(x => x.RFQItemsId == id && x.DeleteFlag == false).FirstOrDefault();//Include(x => x.RFQItemsInfoe)
				model.HSNCode = items.HSNCode;
				model.QuotationQty = items.QuotationQty;
				model.VendorModelNo = items.VendorModelNo;
				model.RequsetRemarks = items.RequestRemarks;
				model.CGSTPercentage = items.CGSTPercentage;
				model.IGSTPercentage = items.IGSTPercentage;
				model.SGSTPercentage = items.SGSTPercentage;
				model.CustomDuty = items.CustomDuty;
				model.FreightAmount = items.FreightAmount;
				model.FreightPercentage = items.FreightPercentage;
				//var data= obj.RFQItemsInfoes.Where(x => x.RFQItemsId == items.RFQItemsId).Select(x => new RfqItemInfoModel()
				// {
				//     UnitPrice = x.UnitPrice
				// });
				var price = obj.RFQItemsInfoes.Where(x => x.RFQItemsId == items.RFQItemsId).FirstOrDefault();
				model.ItemUnitPrice = price.UnitPrice;
				Nullable<decimal> Discountpercentage = price.DiscountPercentage;
				Nullable<decimal> Discount = price.Discount;
				if (items.CustomDuty != 0 && items.CustomDuty != null)
				{
					model.CustomDutyAmount = (model.ItemUnitPrice * items.CustomDuty) / 100;
					if (items.taxInclusiveOfDiscount == false)
					{
						model.NetAmount = model.ItemUnitPrice + model.CustomDutyAmount;
						if (Discountpercentage != 0 || Discount != 0)
						{
							if (Discount != 0 && Discount != null)
							{
								model.DiscountAmount = Discount;
								model.NetAmount = model.NetAmount - model.DiscountAmount;
								model.FinalNetAmount = model.NetAmount;
							}
							else
							{
								model.Discountpercentage = Discountpercentage;
								model.DiscountAmount = (model.NetAmount) * (Discountpercentage / 100);
								model.FinalNetAmount = (model.NetAmount) - (model.DiscountAmount);
							}
						}
					}
					else
					{
						if (Discountpercentage != 0 || Discount != 0 || Discountpercentage != null)
						{
							if (Discount != 0 && Discount != null)
							{
								model.DiscountAmount = Discount;
								model.NetAmount = (model.ItemUnitPrice) - (model.DiscountAmount);
								model.NetAmount = model.ItemUnitPrice + model.CustomDutyAmount;
							}
							else
							{
								model.DiscountAmount = (model.ItemUnitPrice) * (Discountpercentage / 100);
								model.NetAmount = (model.ItemUnitPrice) - (model.DiscountAmount);
								model.NetAmount = model.ItemUnitPrice + model.CustomDutyAmount;
							}
						}
					}
				}
				else
				{
					if (items.taxInclusiveOfDiscount == false || items.taxInclusiveOfDiscount == null)
					{
						if (Discountpercentage != 0 || Discount != 0)
						{
							if (Discountpercentage != 0 || Discount != 0 || Discountpercentage != null)
							{
								if (Discount != 0 && Discount != null)
								{
									model.DiscountAmount = Discount;
									model.NetAmount = (model.ItemUnitPrice) - (model.DiscountAmount);
									model.SGSTAmount = (model.NetAmount * items.SGSTPercentage) / 100;
									model.CGSTAmount = (model.NetAmount * items.CGSTPercentage) / 100;
									model.IGSTAmount = (model.NetAmount * items.IGSTPercentage) / 100;
									model.TotalTaxAmount = model.SGSTAmount + model.CGSTAmount + model.IGSTAmount;
									model.FinalNetAmount = model.NetAmount + model.TotalTaxAmount;
								}
								else
								{
									model.DiscountAmount = (model.NetAmount * Discountpercentage) / 100;
									model.NetAmount = (model.ItemUnitPrice) - (model.DiscountAmount);
									model.SGSTAmount = (model.NetAmount * items.SGSTPercentage) / 100;
									model.CGSTAmount = (model.NetAmount * items.CGSTPercentage) / 100;
									model.IGSTAmount = (model.NetAmount * items.IGSTPercentage) / 100;
									model.TotalTaxAmount = model.SGSTAmount + model.CGSTAmount + model.IGSTAmount;
									model.FinalNetAmount = model.NetAmount + model.TotalTaxAmount;
								}
							}
						}


					}
					else
					{
						if (Discountpercentage != 0 || Discount != 0 || Discountpercentage != null)
						{
							if (Discount != 0 && Discount != null)
							{
								model.DiscountAmount = Discount;
								model.NetAmount = model.ItemUnitPrice - model.DiscountAmount;
								model.SGSTAmount = (model.NetAmount * items.SGSTPercentage) / 100;
								model.CGSTAmount = (model.NetAmount * items.CGSTPercentage) / 100;
								model.IGSTAmount = (model.NetAmount * items.IGSTPercentage) / 100;
								model.TotalTaxAmount = model.SGSTAmount + model.CGSTAmount + model.IGSTAmount;
								model.FinalNetAmount = model.NetAmount + model.TotalTaxAmount;
							}
							else
							{
								model.DiscountAmount = (model.ItemUnitPrice * Discountpercentage) / 100;
								model.NetAmount = model.ItemUnitPrice - model.DiscountAmount;
								model.SGSTAmount = (model.ItemUnitPrice * items.SGSTPercentage) / 100;
								model.CGSTAmount = (model.ItemUnitPrice * items.CGSTPercentage) / 100;
								model.IGSTAmount = (model.ItemUnitPrice * items.IGSTPercentage) / 100;
								model.TotalTaxAmount = model.SGSTAmount + model.CGSTAmount + model.IGSTAmount;
								model.FinalNetAmount = model.NetAmount + model.TotalTaxAmount;
							}
						}
					}
				}
				return model;
			}
			catch (Exception ex)
			{
				throw;
			}
		}
		public List<VendormasterModel> GetAllvendorList()
		{
			List<VendormasterModel> vendor = new List<VendormasterModel>();
			try
			{
				var data = obj.VendorMasters.Where(x => x.Deleteflag == false).ToList();
				vendor = data.Select(x => new VendormasterModel()
				{
					ContactNo = x.ContactNo,
					Vendorid = x.Vendorid,
					VendorCode = x.VendorCode,
					VendorName = x.VendorName,

				}).ToList();
				return vendor;
			}
			catch (Exception ex)
			{

				throw;
			}
		}

		/// <summary>
		/// insert data from vendor 
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		public List<RfqItemModel> InsertOrEditRfqItemInfo(RfqItemModel model)
		{
			statuscheckmodel status = new statuscheckmodel();
			RfqItemModel eachobj = new RfqItemModel();
			List<RfqItemModel> _listobj = new List<RfqItemModel>();
			int spiltitemid = 0;
			double? totalprice = 0;
			try
			{
				//  int spiltitemid = 0;
				// double? totalprice = 0;
				int RFQItemsId = 0;
				var rfqremoteitem = vscm.RemoteRFQItems_N.Where(x => x.RFQItemsId == model.RFQItemID).FirstOrDefault();
				rfqremoteitem.HSNCode = model.HSNCode;
				rfqremoteitem.QuotationQty = model.QuotationQty;
				rfqremoteitem.VendorModelNo = model.VendorModelNo;
				rfqremoteitem.IGSTPercentage = model.IGSTPercentage;
				rfqremoteitem.SGSTPercentage = model.SGSTPercentage;
				rfqremoteitem.CGSTPercentage = model.CGSTPercentage;
				rfqremoteitem.MfgPartNo = model.MfgPartNo;
				rfqremoteitem.MfgModelNo = model.MfgModelNo;
				rfqremoteitem.ManufacturerName = model.ManufacturerName;
				rfqremoteitem.PFAmount = model.PFAmount;
				rfqremoteitem.PFPercentage = model.PFPercentage;
				rfqremoteitem.FreightAmount = model.FreightAmount;
				rfqremoteitem.FreightPercentage = model.FreightPercentage;
				rfqremoteitem.CustomDuty = model.CustomDuty;
				rfqremoteitem.taxInclusiveOfDiscount = model.taxInclusiveOfDiscount;
				rfqremoteitem.UpdatedBy = model.UpdatedBy;
				rfqremoteitem.UpdatedOn = DateTime.Now;
				//int rfqitemsid = rfqremoteitem.RFQItemsId;
				vscm.SaveChanges();

				//yscm update
				var rfqitem = obj.RFQItems_N.Where(x => x.RFQItemsId == model.RFQItemID).FirstOrDefault();
				rfqitem.HSNCode = model.HSNCode;
				rfqitem.QuotationQty = model.QuotationQty;
				rfqitem.VendorModelNo = model.VendorModelNo;
				rfqitem.IGSTPercentage = model.IGSTPercentage;
				rfqitem.SGSTPercentage = model.SGSTPercentage;
				rfqitem.CGSTPercentage = model.CGSTPercentage;
				rfqitem.MfgPartNo = model.MfgPartNo;
				rfqitem.MfgModelNo = model.MfgModelNo;
				rfqitem.ManufacturerName = model.ManufacturerName;
				rfqitem.PFAmount = model.PFAmount;
				rfqitem.PFPercentage = model.PFPercentage;
				rfqitem.FreightAmount = model.FreightAmount;
				rfqitem.FreightPercentage = model.FreightPercentage;
				rfqitem.CustomDuty = model.CustomDuty;
				rfqitem.taxInclusiveOfDiscount = model.taxInclusiveOfDiscount;
				rfqitem.UpdatedBy = model.UpdatedBy;
				rfqitem.UpdatedOn = DateTime.Now;
				//int rfqitemsid = rfqremoteitem.RFQItemsId;
				obj.SaveChanges();
				if (model.multipleitem == "yes")
				{
					foreach (var item in model.iteminfo)
					{
						int bomid = Convert.ToInt32(model.RFQVendorbomItemId);
						RemoteRfqVendorBOM databom = vscm.RemoteRfqVendorBOMs.Where(x => x.RFQVendorbomItemId == bomid).FirstOrDefault();
						if (databom != null)
						{
							databom.RfqItemsId = item.RFQItemsId;
							databom.HSNCode = model.HSNCode;
							databom.QuotationQty = model.QuotationQty;
							databom.VendorModelNo = model.VendorModelNo;
							databom.IGSTPercentage = model.IGSTPercentage;
							databom.SGSTPercentage = model.SGSTPercentage;
							databom.CGSTPercentage = model.CGSTPercentage;
							databom.MfgPartNo = model.MfgPartNo;
							databom.MfgModelNo = model.MfgModelNo;
							databom.ManufacturerName = model.ManufacturerName;
							databom.PFPercentage = model.PFPercentage;
							databom.PFAmount = model.PFAmount;
							databom.PFPercentage = model.PFPercentage;
							databom.FreightAmount = model.FreightAmount;
							databom.CustomDuty = model.CustomDuty;
							databom.Qty = item.Quantity;
							databom.UOM = item.UOM;
							databom.UnitPrice = item.UnitPrice;
							databom.DiscountPercentage = item.DiscountPercentage;
							databom.Discount = item.Discount;
							databom.CurrencyId = item.CurrencyID;
							databom.CurrencyValue = item.CurrencyValue;
							databom.Remarks = item.Remarks;
							databom.DeliveryDate = item.DeliveryDate;
							databom.ItemDescription = model.ItemDescription;
							databom.DeleteFlag = false;
							//obj.RfqVendorBOMs.Add(databom);
							vscm.SaveChanges();
							bomid = databom.RFQVendorbomItemId;
						}
						else
						{
							RFQItemsId = item.RFQItemsId;
							var info = new RemoteRfqVendorBOM();
							info.ItemDescription = item.ItemDescriptionForMultiple;
							info.ItemName = item.ItemNameForMultiple;
							info.RfqItemsId = item.RFQItemsId;
							info.HSNCode = model.HSNCode;
							info.QuotationQty = model.QuotationQty;
							info.VendorModelNo = model.VendorModelNo;
							info.IGSTPercentage = model.IGSTPercentage;
							info.SGSTPercentage = model.SGSTPercentage;
							info.CGSTPercentage = model.CGSTPercentage;
							info.MfgPartNo = model.MfgPartNo;
							info.MfgModelNo = model.MfgModelNo;
							info.ManufacturerName = model.ManufacturerName;
							info.PFPercentage = model.PFPercentage;
							info.PFAmount = model.PFAmount;
							info.PFPercentage = model.PFPercentage;
							info.FreightAmount = model.FreightAmount;
							info.CustomDuty = model.CustomDuty;
							//rfqremoteitem.taxInclusiveOfDiscount = model.taxInclusiveOfDiscount;
							info.Qty = item.Quantity;
							info.UOM = item.UOM;
							info.UnitPrice = item.UnitPrice;
							info.DiscountPercentage = item.DiscountPercentage;
							info.Discount = item.Discount;
							info.CurrencyId = item.CurrencyID;
							info.CurrencyValue = item.CurrencyValue;
							info.Remarks = item.Remarks;
							info.DeliveryDate = item.DeliveryDate;
							info.ItemDescription = model.ItemDescription;
							info.DeleteFlag = false;
							rfqitem.UpdatedBy = model.UpdatedBy;
							rfqitem.UpdatedOn = DateTime.Now;
							vscm.RemoteRfqVendorBOMs.Add(info);
							vscm.SaveChanges();
							bomid = info.RFQVendorbomItemId;
						}
						//in yscm

						RfqVendorBOM Localdatabom = obj.RfqVendorBOMs.Where(x => x.RFQVendorbomItemId == bomid).FirstOrDefault();
						if (Localdatabom != null)
						{
							Localdatabom.RfqItemsId = item.RFQItemsId;
							Localdatabom.HSNCode = model.HSNCode;
							Localdatabom.QuotationQty = model.QuotationQty;
							Localdatabom.VendorModelNo = model.VendorModelNo;
							Localdatabom.IGSTPercentage = model.IGSTPercentage;
							Localdatabom.SGSTPercentage = model.SGSTPercentage;
							Localdatabom.CGSTPercentage = model.CGSTPercentage;
							Localdatabom.MfgPartNo = model.MfgPartNo;
							Localdatabom.MfgModelNo = model.MfgModelNo;
							Localdatabom.ManufacturerName = model.ManufacturerName;
							Localdatabom.PFPercentage = model.PFPercentage;
							Localdatabom.PFAmount = model.PFAmount;
							Localdatabom.PFPercentage = model.PFPercentage;
							Localdatabom.FreightAmount = model.FreightAmount;
							Localdatabom.CustomDuty = model.CustomDuty;
							Localdatabom.Qty = item.Quantity;
							Localdatabom.UOM = item.UOM;
							Localdatabom.UnitPrice = item.UnitPrice;
							Localdatabom.DiscountPercentage = item.DiscountPercentage;
							Localdatabom.Discount = item.Discount;
							Localdatabom.CurrencyId = item.CurrencyID;
							Localdatabom.CurrencyValue = item.CurrencyValue;
							Localdatabom.Remarks = item.Remarks;
							Localdatabom.DeliveryDate = item.DeliveryDate;
							Localdatabom.ItemDescription = model.ItemDescription;
							Localdatabom.DeleteFlag = false;
							obj.SaveChanges();
						}
						else
						{
							var infos = new RfqVendorBOM();
							infos.RFQVendorbomItemId = bomid;
							infos.ItemDescription = item.ItemDescriptionForMultiple;
							infos.ItemName = item.ItemNameForMultiple;
							infos.RfqItemsId = item.RFQItemsId;
							infos.HSNCode = model.HSNCode;
							infos.QuotationQty = model.QuotationQty;
							infos.VendorModelNo = model.VendorModelNo;
							infos.IGSTPercentage = model.IGSTPercentage;
							infos.SGSTPercentage = model.SGSTPercentage;
							infos.CGSTPercentage = model.CGSTPercentage;
							infos.MfgPartNo = model.MfgPartNo;
							infos.MfgModelNo = model.MfgModelNo;
							infos.ManufacturerName = model.ManufacturerName;
							infos.PFPercentage = model.PFPercentage;
							infos.PFAmount = model.PFAmount;
							infos.PFPercentage = model.PFPercentage;
							infos.FreightAmount = model.FreightAmount;
							infos.CustomDuty = model.CustomDuty;
							//rfqremoteitem.taxInclusiveOfDiscount = model.taxInclusiveOfDiscount;
							infos.Qty = item.Quantity;
							infos.UOM = item.UOM;
							infos.UnitPrice = item.UnitPrice;
							infos.DiscountPercentage = item.DiscountPercentage;
							infos.Discount = item.Discount;
							infos.CurrencyId = item.CurrencyID;
							infos.CurrencyValue = item.CurrencyValue;
							infos.Remarks = item.Remarks;
							infos.DeliveryDate = item.DeliveryDate;
							infos.ItemDescription = model.ItemDescription;
							infos.DeleteFlag = false;
							obj.RfqVendorBOMs.Add(infos);
							obj.SaveChanges();

						}
						List<RemoteRfqVendorBOM> itemsforupdate = vscm.RemoteRfqVendorBOMs.Where(li => li.RfqItemsId == RFQItemsId && li.DeleteFlag == false).ToList<RemoteRfqVendorBOM>();

						foreach (var data1 in itemsforupdate)
						{
							double? subtotalprice = 0;
							subtotalprice = Convert.ToDouble(data1.Qty) * Convert.ToDouble(data1.UnitPrice);
							if (Convert.ToInt32(data1.Discount) != 0)
							{
								double? Discount = subtotalprice - Convert.ToDouble(data1.Discount);
								subtotalprice = Discount;
							}
							if (Convert.ToInt32(data1.DiscountPercentage) != 0)
							{
								double? DiscountPercentage = (subtotalprice - subtotalprice * Convert.ToDouble(data1.DiscountPercentage) / 100);
								subtotalprice = DiscountPercentage;
							}
							if (Convert.ToInt32(data1.FreightAmount) != 0)
							{
								double? FreightAmount = subtotalprice - Convert.ToDouble(data1.FreightAmount);
								subtotalprice = FreightAmount;
							}
							if (Convert.ToInt32(data1.FreightPercentage) != 0)
							{
								double? FreightPercentage = (subtotalprice - subtotalprice * Convert.ToDouble(data1.FreightPercentage) / 100);
								subtotalprice = FreightPercentage;
							}
							if (Convert.ToInt32(data1.PFAmount) != 0)
							{
								double? PFAmount = subtotalprice - Convert.ToDouble(data1.PFAmount);
								subtotalprice = PFAmount;
							}
							if (Convert.ToInt32(data1.PFPercentage) != 0)
							{

								double? PFPercentage = (subtotalprice * Convert.ToDouble(data1.PFPercentage) / 100);
								subtotalprice += PFPercentage;
							}
							totalprice += subtotalprice;

						}
						RfqVendorBOM bompriceupdate = obj.RfqVendorBOMs.Where(li => li.RFQVendorbomItemId == bomid).FirstOrDefault();
						if (bompriceupdate != null)
						{
							bompriceupdate.UnitPrice = Convert.ToInt32(totalprice);
							obj.SaveChanges();
						}

						RemoteRFQItemsInfo_N itemsinfos = vscm.RemoteRFQItemsInfo_N.Where(li => li.RFQItemsId == RFQItemsId && li.RFQSplitItemId == model.RFQSplitItemId).FirstOrDefault();
						if (itemsinfos != null)
						{
							itemsinfos.UnitPrice = Convert.ToDecimal(totalprice);
							vscm.SaveChanges();
						}
						else
						{
							var remoteinfo = new RemoteRFQItemsInfo_N();
							remoteinfo.RFQItemsId = item.RFQItemsId;
							remoteinfo.Qty = item.Quantity;
							remoteinfo.UOM = item.UOM;
							remoteinfo.UnitPrice = item.UnitPrice;
							remoteinfo.DiscountPercentage = item.DiscountPercentage;
							remoteinfo.Discount = item.Discount;
							remoteinfo.CurrencyId = item.CurrencyID;
							remoteinfo.CurrencyValue = item.CurrencyValue;
							remoteinfo.Remarks = item.Remarks;
							remoteinfo.DeliveryDate = item.DeliveryDate;
							remoteinfo.SyncDate = System.DateTime.Now;
							remoteinfo.UpdatedBy = model.UpdatedBy;
							remoteinfo.UpdatedOn = DateTime.Now;
							vscm.RemoteRFQItemsInfo_N.Add(remoteinfo);
							vscm.SaveChanges();
							spiltitemid = remoteinfo.RFQSplitItemId;
						}

						RFQItemsInfo_N rfqinsertdata = obj.RFQItemsInfo_N.Where(li => li.RFQSplitItemId == spiltitemid).FirstOrDefault();
						if (itemsinfos != null)
						{
							itemsinfos.UnitPrice = Convert.ToDecimal(totalprice);
							vscm.SaveChanges();
						}
						else
						{
							var remoteinfo = new RFQItemsInfo_N();
							remoteinfo.RFQItemsId = item.RFQItemsId;
							remoteinfo.Qty = item.Quantity;
							remoteinfo.UOM = item.UOM;
							remoteinfo.UnitPrice = item.UnitPrice;
							remoteinfo.DiscountPercentage = item.DiscountPercentage;
							remoteinfo.Discount = item.Discount;
							remoteinfo.CurrencyId = item.CurrencyID;
							remoteinfo.CurrencyValue = item.CurrencyValue;
							remoteinfo.Remarks = item.Remarks;
							remoteinfo.DeliveryDate = item.DeliveryDate;
							remoteinfo.RFQSplitItemId = spiltitemid;
							//remoteinfo.SyncDate = System.DateTime.Now;
							remoteinfo.UpdatedBy = model.UpdatedBy;
							remoteinfo.UpdatedOn = DateTime.Now;
							obj.RFQItemsInfo_N.Add(remoteinfo);
							obj.SaveChanges();
							//spiltitemid = remoteinfo.RFQSplitItemId;
						}
					}
				}
				else if (model.multipleitem == "no")
				{
					foreach (var item in model.iteminfo)
					{
						RemoteRFQItemsInfo_N itemsinfos = vscm.RemoteRFQItemsInfo_N.Where(li => li.RFQItemsId == item.RFQItemsId && li.RFQSplitItemId == item.RFQSplitItemId).FirstOrDefault();
						if (itemsinfos != null)
						{
							itemsinfos.RFQItemsId = item.RFQItemsId;
							itemsinfos.Qty = item.Quantity;
							itemsinfos.UOM = item.UOM;
							itemsinfos.UnitPrice = item.UnitPrice;
							itemsinfos.DiscountPercentage = item.DiscountPercentage;
							itemsinfos.Discount = item.Discount;
							itemsinfos.CurrencyId = item.CurrencyID;
							itemsinfos.CurrencyValue = item.CurrencyValue;
							itemsinfos.Remarks = item.Remarks;
							itemsinfos.DeliveryDate = item.DeliveryDate;
							itemsinfos.SyncDate = System.DateTime.Now;
							itemsinfos.UpdatedBy = model.UpdatedBy;
							itemsinfos.UpdatedOn = DateTime.Now;
							itemsinfos.DeleteFlag = false;
							vscm.SaveChanges();
							spiltitemid = itemsinfos.RFQSplitItemId;
						}
						else
						{
							var remoteinfo = new RemoteRFQItemsInfo_N();
							remoteinfo.RFQItemsId = item.RFQItemsId;
							remoteinfo.Qty = item.Quantity;
							remoteinfo.UOM = item.UOM;
							remoteinfo.UnitPrice = item.UnitPrice;
							remoteinfo.DiscountPercentage = item.DiscountPercentage;
							remoteinfo.Discount = item.Discount;
							remoteinfo.CurrencyId = item.CurrencyID;
							remoteinfo.CurrencyValue = item.CurrencyValue;
							remoteinfo.Remarks = item.Remarks;
							remoteinfo.DeliveryDate = item.DeliveryDate;
							remoteinfo.SyncDate = System.DateTime.Now;
							remoteinfo.UpdatedBy = model.UpdatedBy;
							remoteinfo.UpdatedOn = DateTime.Now;
							vscm.RemoteRFQItemsInfo_N.Add(remoteinfo);
							vscm.SaveChanges();
							spiltitemid = remoteinfo.RFQSplitItemId;
						}

						RFQItemsInfo_N rfqinsertdata = obj.RFQItemsInfo_N.Where(li => li.RFQSplitItemId == spiltitemid).FirstOrDefault();
						if (rfqinsertdata != null)
						{
							rfqinsertdata.RFQItemsId = item.RFQItemsId;
							rfqinsertdata.Qty = item.Quantity;//item.Quantity;
							rfqinsertdata.UOM = item.UOM;
							rfqinsertdata.UnitPrice = item.UnitPrice;
							rfqinsertdata.DiscountPercentage = item.DiscountPercentage;
							rfqinsertdata.Discount = item.Discount;
							rfqinsertdata.CurrencyId = item.CurrencyID;
							rfqinsertdata.CurrencyValue = item.CurrencyValue;
							rfqinsertdata.Remarks = item.Remarks;
							rfqinsertdata.DeliveryDate = item.DeliveryDate;
							vscm.SaveChanges();
						}
						else
						{
							var info = new RFQItemsInfo_N();
							info.RFQItemsId = item.RFQItemsId;
							info.Qty = item.Quantity;
							info.UOM = item.UOM;
							info.UnitPrice = item.UnitPrice;
							info.DiscountPercentage = item.DiscountPercentage;
							info.Discount = item.Discount;
							info.CurrencyId = item.CurrencyID;
							info.CurrencyValue = item.CurrencyValue;
							info.Remarks = item.Remarks;
							info.RFQSplitItemId = spiltitemid;
							info.DeliveryDate = item.DeliveryDate;
							//remoteinfo.SyncDate = System.DateTime.Now;
							info.UpdatedBy = model.UpdatedBy;
							info.UpdatedOn = DateTime.Now;
							obj.RFQItemsInfo_N.Add(info);
							obj.SaveChanges();
							//spiltitemid = remoteinfo.RFQSplitItemId;
						}

					}
				}


				var rfqitems = new RFQItems_N();
				var mpritemsdetailsid = obj.RFQItems_N.Where(x => x.RFQItemsId == model.RFQItemID).FirstOrDefault();
				var rfqitemsss = obj.MPRRfqItems.Where(x => x.RfqItemsid == model.RFQItemID && x.MPRItemDetailsid == mpritemsdetailsid.MPRItemDetailsid).FirstOrDefault();
				var infosdata = new MPRRfqItemInfo();
				//if (rfqitems != null)
				//    infosdata = obj.MPRRfqItemInfos.Where(li => li.MPRRFQitemId == rfqitemsss.MPRRFQitemId).FirstOrDefault();
				////if (infosdata.Mprrfqsplititemid != 0)
				//{
				infosdata.rfqsplititemid = spiltitemid;
				infosdata.MPRRFQitemId = rfqitemsss.MPRRFQitemId;
				//infosdata.
				obj.MPRRfqItemInfos.Add(infosdata);
				obj.SaveChanges();
				// }
				eachobj.errormsg = "Success";
				_listobj.Add(eachobj);

			}
			catch (Exception ex)
			{
				log.ErrorMessage("RFQDA", "InsertOrEditRfqItemInfo", ex.Message + "; " + ex.StackTrace.ToString());
			}
			return _listobj;
		}

		public List<RfqItemModel> editRfqItemInfo(RfqItemModel model)
		{
			statuscheckmodel status = new statuscheckmodel();
			RfqItemModel eachobj = new RfqItemModel();
			List<RfqItemModel> _listobj = new List<RfqItemModel>();
			int spiltitemid = 0;
			double? totalprice = 0;
			try
			{
				//for remoteserver
				if (model != null)
				{
					// vscm.Database.Connection.Open();
					var Remotedata = new RemoteRFQItem();
					//var rfqRemoteitem = from x in vscm.RemoteRFQItems_N where x.RFQItemsId == model.RFQItemID select x;
					var items = new RemoteRFQItemsInfo();

					var rfqremoteitem = vscm.RemoteRFQItems_N.Where(x => x.RFQItemsId == model.RFQItemID).FirstOrDefault();
					// var rfqremoteitemyscm=obj.RFQItemsInfoes
					if (rfqremoteitem != null)
					{
						rfqremoteitem.HSNCode = model.HSNCode;
						rfqremoteitem.QuotationQty = model.QuotationQty;
						rfqremoteitem.VendorModelNo = model.VendorModelNo;
						rfqremoteitem.IGSTPercentage = model.IGSTPercentage;
						rfqremoteitem.SGSTPercentage = model.SGSTPercentage;
						rfqremoteitem.CGSTPercentage = model.CGSTPercentage;
						rfqremoteitem.MfgPartNo = model.MfgPartNo;
						rfqremoteitem.MfgModelNo = model.MfgModelNo;
						rfqremoteitem.ManufacturerName = model.ManufacturerName;
						rfqremoteitem.PFAmount = model.PFAmount;
						rfqremoteitem.PFPercentage = model.PFPercentage;
						rfqremoteitem.FreightAmount = model.FreightAmount;
						rfqremoteitem.FreightPercentage = model.FreightPercentage;
						rfqremoteitem.CustomDuty = model.CustomDuty;

						rfqremoteitem.taxInclusiveOfDiscount = model.taxInclusiveOfDiscount;
						rfqremoteitem.UpdatedBy = model.UpdatedBy;
						rfqremoteitem.UpdatedOn = DateTime.Now;
						// vscm.RemoteRFQItems_N.Add(rfqremoteitem);
						vscm.SaveChanges();
						eachobj.QuotationQty = model.QuotationQty;
						eachobj.ItemUnitPrice = model.ItemUnitPrice;
					}

					foreach (var item in model.iteminfo)
					{

						var remoteiteminfo = vscm.RemoteRFQItemsInfo_N.Where(x => x.RFQItemsId == model.RFQItemID && x.RFQSplitItemId == model.RFQSplitItemId).FirstOrDefault();
						if (remoteiteminfo != null)
						{
							remoteiteminfo.RFQItemsId = item.RFQItemsId;
							remoteiteminfo.Qty = item.Quantity;
							remoteiteminfo.UOM = item.UOM;
							remoteiteminfo.UnitPrice = item.UnitPrice;
							remoteiteminfo.DiscountPercentage = item.DiscountPercentage;
							remoteiteminfo.Discount = item.Discount;
							remoteiteminfo.CurrencyId = item.CurrencyID;
							remoteiteminfo.CurrencyValue = item.CurrencyValue;
							remoteiteminfo.Remarks = item.Remarks;
							remoteiteminfo.DeliveryDate = item.DeliveryDate;
							remoteiteminfo.SyncDate = System.DateTime.Now;
							remoteiteminfo.UpdatedBy = model.UpdatedBy;
							remoteiteminfo.UpdatedOn = DateTime.Now;
							remoteiteminfo.DeleteFlag = false;
							// vscm.RemoteRFQItemsInfoes.Add(remoteinfo);

							vscm.SaveChanges();


						}
						int idBOM = Convert.ToInt32(model.RFQVendorbomItemId);
						RemoteRfqVendorBOM databom = vscm.RemoteRfqVendorBOMs.Where(x => x.RFQVendorbomItemId == idBOM).FirstOrDefault();
						if (databom != null)
						{
							databom.RfqItemsId = item.RFQItemsId;
							databom.HSNCode = model.HSNCode;
							databom.QuotationQty = model.QuotationQty;
							databom.VendorModelNo = model.VendorModelNo;
							databom.IGSTPercentage = model.IGSTPercentage;
							databom.SGSTPercentage = model.SGSTPercentage;
							databom.CGSTPercentage = model.CGSTPercentage;
							databom.MfgPartNo = model.MfgPartNo;
							databom.MfgModelNo = model.MfgModelNo;
							databom.ManufacturerName = model.ManufacturerName;
							databom.PFPercentage = model.PFPercentage;
							databom.PFAmount = model.PFAmount;
							databom.PFPercentage = model.PFPercentage;
							databom.FreightAmount = model.FreightAmount;
							databom.CustomDuty = model.CustomDuty;
							databom.Qty = item.Quantity;
							databom.UOM = item.UOM;
							databom.UnitPrice = item.UnitPrice;
							databom.DiscountPercentage = item.DiscountPercentage;
							databom.Discount = item.Discount;
							databom.CurrencyId = item.CurrencyID;
							databom.CurrencyValue = item.CurrencyValue;
							databom.Remarks = item.Remarks;
							databom.DeliveryDate = item.DeliveryDate;
							databom.ItemDescription = model.ItemDescription;
							databom.DeleteFlag = false;
							//obj.RfqVendorBOMs.Add(databom);
							vscm.SaveChanges();
						}
						List<RemoteRfqVendorBOM> itemsforupdate = vscm.RemoteRfqVendorBOMs.Where(li => li.RfqItemsId == item.RFQItemsId && li.DeleteFlag == false).ToList<RemoteRfqVendorBOM>();

						foreach (var data1 in itemsforupdate)
						{
							double? subtotalprice = 0;
							subtotalprice = Convert.ToDouble(data1.Qty) * Convert.ToDouble(data1.UnitPrice);
							if (Convert.ToInt32(data1.Discount) != 0)
							{
								double? Discount = subtotalprice - Convert.ToDouble(data1.Discount);
								subtotalprice = Discount;
							}
							if (Convert.ToInt32(data1.DiscountPercentage) != 0)
							{
								double? DiscountPercentage = (subtotalprice - subtotalprice * Convert.ToDouble(data1.DiscountPercentage) / 100);
								subtotalprice = DiscountPercentage;
							}
							if (Convert.ToInt32(data1.FreightAmount) != 0)
							{
								double? FreightAmount = subtotalprice - Convert.ToDouble(data1.FreightAmount);
								subtotalprice = FreightAmount;
							}
							if (Convert.ToInt32(data1.FreightPercentage) != 0)
							{
								double? FreightPercentage = (subtotalprice - subtotalprice * Convert.ToDouble(data1.FreightPercentage) / 100);
								subtotalprice = FreightPercentage;
							}
							if (Convert.ToInt32(data1.PFAmount) != 0)
							{
								double? PFAmount = subtotalprice - Convert.ToDouble(data1.PFAmount);
								subtotalprice = PFAmount;
							}
							if (Convert.ToInt32(data1.PFPercentage) != 0)
							{

								double? PFPercentage = (subtotalprice * Convert.ToDouble(data1.PFPercentage) / 100);
								subtotalprice += PFPercentage;
							}
							totalprice += subtotalprice;

						}

						RemoteRFQItemsInfo_N itemsinfo = vscm.RemoteRFQItemsInfo_N.Where(li => li.RFQItemsId == item.RFQItemsId).FirstOrDefault();
						if (itemsinfo != null)
						{
							itemsinfo.UnitPrice = Convert.ToDecimal(totalprice);
							vscm.SaveChanges();

						}

					}

					//var data = new RFQItems_N();
					//var rfqitem = obj.RFQItems_N.Where(x => x.RFQItemsId == model.RFQItemID).FirstOrDefault();
					//rfqitem.HSNCode = model.HSNCode;
					//rfqitem.QuotationQty = model.QuotationQty;
					//rfqitem.VendorModelNo = model.VendorModelNo;
					//obj.SaveChanges();
					var rfqitem = obj.RFQItems_N.Where(x => x.RFQItemsId == model.RFQItemID).FirstOrDefault();
					if (rfqitem != null)
					{
						rfqitem.HSNCode = model.HSNCode;
						rfqitem.QuotationQty = model.QuotationQty;
						rfqitem.VendorModelNo = model.VendorModelNo;
						rfqitem.IGSTPercentage = model.IGSTPercentage;
						rfqitem.SGSTPercentage = model.SGSTPercentage;
						rfqitem.CGSTPercentage = model.CGSTPercentage;
						rfqitem.MfgPartNo = model.MfgPartNo;
						rfqitem.MfgModelNo = model.MfgModelNo;
						rfqitem.ManufacturerName = model.ManufacturerName;
						rfqitem.PFAmount = model.PFAmount;
						rfqitem.PFPercentage = model.PFPercentage;
						rfqitem.FreightAmount = model.FreightAmount;
						rfqitem.FreightPercentage = model.FreightPercentage;
						rfqitem.CustomDuty = model.CustomDuty;

						rfqitem.taxInclusiveOfDiscount = model.taxInclusiveOfDiscount;
						rfqitem.UpdatedBy = model.UpdatedBy;
						rfqitem.UpdatedOn = DateTime.Now;
						// vscm.RemoteRFQItems_N.Add(rfqremoteitem);
						vscm.SaveChanges();
						eachobj.QuotationQty = model.QuotationQty;
						eachobj.ItemUnitPrice = model.ItemUnitPrice;
					}

					foreach (var item in model.iteminfo)
					{

						var inforrfq = obj.RFQItemsInfo_N.Where(x => x.RFQSplitItemId == model.RFQSplitItemId).FirstOrDefault();
						/// var ids = vscm.RemoteRFQItemsInfoes.Where(x => x.RFQItemsId == model.RFQItemID).OrderByDescending().ToList();

						if (inforrfq != null)
						{

							inforrfq.RFQItemsId = item.RFQItemsId;
							//inforrfq.RFQSplitItemId = spiltitemid;
							inforrfq.Qty = item.Quantity;
							inforrfq.UOM = item.UOM;
							inforrfq.UnitPrice = item.UnitPrice;
							inforrfq.DiscountPercentage = item.DiscountPercentage;
							inforrfq.Discount = item.Discount;
							inforrfq.CurrencyId = item.CurrencyID;
							inforrfq.CurrencyValue = item.CurrencyValue;
							inforrfq.Remarks = item.Remarks;
							inforrfq.DeliveryDate = item.DeliveryDate;
							inforrfq.DeleteFlag = false;
							inforrfq.UpdatedBy = model.UpdatedBy;
							inforrfq.UpdatedOn = DateTime.Now;
							//obj.RFQItemsInfo_N.Add(inforrfq);
							obj.SaveChanges();
							eachobj.errormsg = "Success";
						}
						int idBOM = Convert.ToInt32(model.RFQVendorbomItemId);
						RfqVendorBOM databom = obj.RfqVendorBOMs.Where(x => x.RFQVendorbomItemId == idBOM).FirstOrDefault();
						if (databom != null)
						{
							databom.RfqItemsId = item.RFQItemsId;
							databom.HSNCode = model.HSNCode;
							databom.QuotationQty = model.QuotationQty;
							databom.VendorModelNo = model.VendorModelNo;
							databom.IGSTPercentage = model.IGSTPercentage;
							databom.SGSTPercentage = model.SGSTPercentage;
							databom.CGSTPercentage = model.CGSTPercentage;
							databom.MfgPartNo = model.MfgPartNo;
							databom.MfgModelNo = model.MfgModelNo;
							databom.ManufacturerName = model.ManufacturerName;
							databom.PFPercentage = model.PFPercentage;
							databom.PFAmount = model.PFAmount;
							databom.PFPercentage = model.PFPercentage;
							databom.FreightAmount = model.FreightAmount;
							databom.CustomDuty = model.CustomDuty;
							databom.Qty = item.Quantity;
							databom.UOM = item.UOM;
							databom.UnitPrice = item.UnitPrice;
							databom.DiscountPercentage = item.DiscountPercentage;
							databom.Discount = item.Discount;
							databom.CurrencyId = item.CurrencyID;
							databom.CurrencyValue = item.CurrencyValue;
							databom.Remarks = item.Remarks;
							databom.DeliveryDate = item.DeliveryDate;
							databom.ItemDescription = model.ItemDescription;
							databom.DeleteFlag = false;
							//obj.RfqVendorBOMs.Add(databom);
							obj.SaveChanges();
						}
						RFQItemsInfo_N itemsinfo = obj.RFQItemsInfo_N.Where(li => li.RFQItemsId == item.RFQItemsId).FirstOrDefault();
						if (itemsinfo != null)
						{
							itemsinfo.UnitPrice = Convert.ToDecimal(totalprice);
							obj.SaveChanges();

						}

					}
					_listobj.Add(eachobj);

				}


			}
			catch (Exception ex)
			{
				log.ErrorMessage("RFQDA", "editRfqItemInfo", ex.Message + "; " + ex.StackTrace.ToString());
			}
			return _listobj;
		}



		//delete
		public async Task<statuscheckmodel> DeleteRfqIteminfoByid(int id, int rfqitemid)
		{
			statuscheckmodel status = new statuscheckmodel();
			try
			{
				double? totalprice = 0;
				//vscm.Database.Connection.Open();
				var Remotedata = vscm.RemoteRFQItemsInfo_N.Where(x => x.RFQSplitItemId == id && x.DeleteFlag == false).FirstOrDefault();
				var remotebomdata = vscm.RemoteRfqVendorBOMs.Where(x => x.RfqItemsId == rfqitemid && x.DeleteFlag == false).ToList();
				var bomdata = obj.RfqVendorBOMs.Where(x => x.RfqItemsId == rfqitemid && x.DeleteFlag == false).ToList();
				if (Remotedata != null)
				{

					Remotedata.DeleteFlag = true;
					vscm.SaveChanges();
				}
				if (remotebomdata != null)
				{
					foreach (var items in remotebomdata)
					{
						items.DeleteFlag = true;
						vscm.SaveChanges();
					}


				}
				if (bomdata != null)
				{
					foreach (var items in bomdata)
					{
						items.DeleteFlag = true;
						obj.SaveChanges();
					}

				}
				List<RemoteRfqVendorBOM> itemsforupdate = vscm.RemoteRfqVendorBOMs.Where(li => li.RfqItemsId == id && li.DeleteFlag == false).ToList<RemoteRfqVendorBOM>();

				foreach (var data1 in itemsforupdate)
				{
					double? subtotalprice = 0;
					subtotalprice = Convert.ToDouble(data1.Qty) * Convert.ToDouble(data1.UnitPrice);
					if (Convert.ToInt32(data1.Discount) != 0)
					{
						double? Discount = subtotalprice - Convert.ToDouble(data1.Discount);
						subtotalprice = Discount;
					}
					if (Convert.ToInt32(data1.DiscountPercentage) != 0)
					{
						double? DiscountPercentage = (subtotalprice - subtotalprice * Convert.ToDouble(data1.DiscountPercentage) / 100);
						subtotalprice = DiscountPercentage;
					}
					if (Convert.ToInt32(data1.FreightAmount) != 0)
					{
						double? FreightAmount = subtotalprice - Convert.ToDouble(data1.FreightAmount);
						subtotalprice = FreightAmount;
					}
					if (Convert.ToInt32(data1.FreightPercentage) != 0)
					{
						double? FreightPercentage = (subtotalprice - subtotalprice * Convert.ToDouble(data1.FreightPercentage) / 100);
						subtotalprice = FreightPercentage;
					}
					if (Convert.ToInt32(data1.PFAmount) != 0)
					{
						double? PFAmount = subtotalprice - Convert.ToDouble(data1.PFAmount);
						subtotalprice = PFAmount;
					}
					if (Convert.ToInt32(data1.PFPercentage) != 0)
					{

						double? PFPercentage = (subtotalprice * Convert.ToDouble(data1.PFPercentage) / 100);
						subtotalprice += PFPercentage;
					}
					totalprice += subtotalprice;
				}
				RemoteRFQItemsInfo_N itemsinfo = vscm.RemoteRFQItemsInfo_N.Where(li => li.RFQItemsId == id).FirstOrDefault();
				if (itemsinfo != null)
				{
					itemsinfo.UnitPrice = Convert.ToDecimal(totalprice);
					vscm.SaveChanges();
				}

				// vscm.Database.Connection.Close();

				//obj.Database.Connection.Open();
				var Localdata = obj.RFQItemsInfo_N.Where(x => x.RFQSplitItemId == id && x.DeleteFlag == false).FirstOrDefault();
				if (Localdata != null)
				{
					Localdata.DeleteFlag = true;
					Localdata.UnitPrice = Convert.ToDecimal(totalprice);
					obj.SaveChanges();
				}
				else
				{

				}
				//if (Remotedata == null || Localdata==null)
				//{
				//    status.Sid = 0;
				//}
				//status.Sid = Localdata.RFQItemsId;
				return status;
			}
			catch (Exception ex)
			{
				throw;
			}
		}
		public async Task<statuscheckmodel> DeleteRfqitemandinfosById(int id)
		{
			statuscheckmodel status = new statuscheckmodel();
			try
			{
				//remotedata
				vscm.Database.Connection.Open();
				var Remotedata = vscm.RemoteRFQItems_N.Where(x => x.RFQItemsId == id && x.DeleteFlag == false).FirstOrDefault();
				if (Remotedata != null)
				{
					Remotedata.DeleteFlag = true;
					vscm.SaveChanges();

					var remoteitemsdata = vscm.RemoteRFQItemsInfoes.Where(x => x.RFQItemsId == Remotedata.RFQItemsId && x.DeleteFlag == false).ToList();
					if (remoteitemsdata != null)
					{
						foreach (var item in remoteitemsdata)
						{
							item.DeleteFlag = true;
							vscm.SaveChanges();
						}
					}
					//int Itemid = Remotedata.RFQItemsId;
				}
				vscm.Database.Connection.Close();

				//localdata
				obj.Database.Connection.Open();
				var Localdata = obj.RFQItems.Where(x => x.RFQItemsId == id && x.DeleteFlag == false).FirstOrDefault();
				if (Localdata != null)
				{
					Localdata.DeleteFlag = true;
					obj.SaveChanges();

					var localitemsdata = obj.RFQItemsInfoes.Where(x => x.RFQItemsId == Remotedata.RFQItemsId && x.DeleteFlag == false).ToList();
					if (localitemsdata != null)
					{
						foreach (var item in localitemsdata)
						{
							item.DeleteFlag = true;
							obj.SaveChanges();
						}
					}
				}
				int Itemid = Localdata.RFQItemsId;
				obj.Database.Connection.Close();
				status.Sid = Itemid;
				return status;
			}
			catch (Exception ex)
			{

				throw;
			}
		}
		public async Task<statuscheckmodel> UpdateRfqItemInfoById(RfqItemInfoModel model)
		{
			statuscheckmodel status = new statuscheckmodel();
			try
			{
				//remote data
				var remoteitem = vscm.RemoteRFQItemsInfoes.Where(x => x.RFQItemsId == model.RFQItemsId).FirstOrDefault();
				remoteitem.Discount = model.Discount;
				remoteitem.DiscountPercentage = model.DiscountPercentage;
				remoteitem.Qty = model.Quantity;
				remoteitem.UnitPrice = model.UnitPrice;
				remoteitem.UOM = model.UOM;
				remoteitem.CurrencyValue = model.CurrencyValue;
				remoteitem.CurrencyId = model.CurrencyID;
				remoteitem.Remarks = model.Remarks;
				remoteitem.DeliveryDate = model.DeliveryDate;

				vscm.RemoteRFQItemsInfoes.Add(remoteitem);
				vscm.SaveChanges();
				int remoteitemid = remoteitem.RFQItemsId;
				vscm.Database.Connection.Close();

				obj.Database.Connection.Open();
				var localitem = obj.RFQItemsInfoes.Where(x => x.RFQItemsId == model.RFQItemsId).FirstOrDefault();
				localitem.Discount = model.Discount;
				localitem.DiscountPercentage = model.DiscountPercentage;
				localitem.Qty = model.Quantity;
				localitem.UnitPrice = model.UnitPrice;
				localitem.UOM = model.UOM;
				localitem.CurrencyValue = model.CurrencyValue;
				localitem.CurrencyId = model.CurrencyID;
				localitem.Remarks = model.Remarks;
				localitem.DeliveryDate = model.DeliveryDate;
				obj.RFQItemsInfoes.Add(localitem);
				obj.SaveChanges();
				return status;
			}
			catch (Exception ex)
			{
				throw;
			}
		}
		public async Task<RfqItemModel> GetRfqItemByMPrId(int id)
		{
			RfqItemModel itemmodel = new RfqItemModel();
			try
			{
				vscm.Database.Connection.Open();
				RfqItemInfoModel iteminfo = new RfqItemInfoModel();
				var itemdetails = vscm.RemoteRFQItems_N.Where(x => x.MPRItemDetailsid == id).Include(x => x.RemoteRFQItemsInfo_N).FirstOrDefault();
				itemmodel.HSNCode = itemdetails.HSNCode;
				itemmodel.RFQItemID = itemdetails.RFQItemsId;
				itemmodel.QuotationQty = itemdetails.QuotationQty;
				itemmodel.VendorModelNo = itemdetails.VendorModelNo;
				itemmodel.RequsetRemarks = itemdetails.RequestRemarks;
				foreach (var item in itemdetails.RemoteRFQItemsInfo_N)
				{
					iteminfo.RFQItemsId = item.RFQItemsId;
					iteminfo.RFQSplitItemId = item.RFQSplitItemId;
					iteminfo.UnitPrice = item.UnitPrice;
					iteminfo.UOM = item.UOM;
					iteminfo.Remarks = item.Remarks;
					iteminfo.DiscountPercentage = item.DiscountPercentage;
					iteminfo.Discount = item.Discount;
					iteminfo.DeliveryDate = Convert.ToDateTime(item.DeliveryDate);
					iteminfo.CurrencyID = item.CurrencyId;
					iteminfo.CurrencyValue = item.CurrencyValue;
					itemmodel.iteminfo.Add(iteminfo);
				}
				return itemmodel;
			}
			catch (Exception ex)
			{

				throw;
			}
		}

		//edit
		public async Task<RemoteRFQRevisions_N> GetRfqDetailsById(int revisionId)
		{
			//obj.Configuration.ProxyCreationEnabled = false;
			RemoteRFQRevisions_N rev = new RemoteRFQRevisions_N();
			try
			{
				rev = vscm.RemoteRFQRevisions_N.Include(x => x.RemoteRFQItems_N).Include(x => x.RemoteRFQDocuments).Where(li => li.rfqRevisionId == revisionId && li.DeleteFlag == false).FirstOrDefault();
				rev.RemoteRFQItems_N = rev.RemoteRFQItems_N.Where(li => li.DeleteFlag != true).ToList();
				rev.RemoteRFQDocuments = rev.RemoteRFQDocuments.Where(li => li.DocumentType != 1 && li.DeleteFlag == false && li.rfqItemsid == null).ToList();
				rev.RemoteRFQMaster.RemoteRFQCommunications = rev.RemoteRFQMaster.RemoteRFQCommunications.OrderByDescending(li => li.RemarksDate).ToList();
				rev.RFQStatusTrackDetails = obj.RFQStatusTrackDetails.Where(li => li.RfqMasterId == rev.rfqMasterId).ToList();
				foreach (RemoteRFQCommunication item in rev.RemoteRFQMaster.RemoteRFQCommunications)
				{
					item.Employee = obj.VendorEmployeeViews.Where(li => li.EmployeeNo == item.RemarksFrom).FirstOrDefault();
				}

				foreach (RemoteRFQItems_N item in rev.RemoteRFQItems_N)
				{
					item.RemoteRFQItemsInfo_N = item.RemoteRFQItemsInfo_N.Where(li => li.DeleteFlag != true).ToList();
					item.RemoteRFQDocuments = item.RemoteRFQDocuments.Where(li => li.DeleteFlag == false).ToList();
					item.RemoteRfqVendorBOMs = vscm.RemoteRfqVendorBOMs.Where(li => li.RfqItemsId == item.RFQItemsId).ToList();
					item.RemoteRfqVendorBOMs = item.RemoteRfqVendorBOMs.Where(li => li.DeleteFlag != true).ToList();
				}

			}
			catch (Exception ex)
			{
				log.ErrorMessage("RFQDA", "GetRfqDetailsById", ex.Message + "; " + ex.StackTrace.ToString());


			}

			//foreach (RemoteRFQItems_N item in rev.RemoteRFQItems_N)
			//{
			//    item.RemoteRFQDocuments = vscm.RemoteRFQDocuments.Where(li => li.rfqItemsid == item.RFQItemsId && li.rfqRevisionId == rev.RevisionNo && li.DeleteFlag.Equals(false) && li.rfqItemsid != null).ToList();

			//}


			//foreach(RemoteRFQDocument item in rev.RemoteRFQItems_N)
			return rev;
			//RfqRevisionModel revision = new RfqRevisionModel();
			//try
			//{
			//    var localrevision = vscm.RemoteRFQRevisions_N.Where(x => x.rfqRevisionId == revisionId).Include(x => x.RemoteRFQItems_N).FirstOrDefault();
			//    if (localrevision != null)
			//    {
			//        revision.RfqMasterId = localrevision.rfqMasterId;
			//        revision.RfqRevisionNo = localrevision.RevisionNo;
			//        revision.CreatedBy = localrevision.CreatedBy;
			//        revision.CreatedDate = localrevision.CreatedDate;
			//        revision.PackingForwading = localrevision.PackingForwarding;
			//        revision.salesTax = localrevision.SalesTax;
			//        revision.Insurance = localrevision.Insurance;
			//        revision.CustomsDuty = localrevision.CustomsDuty;
			//        revision.PaymentTermDays = localrevision.PaymentTermDays;
			//        revision.PaymentTermRemarks = localrevision.PaymentTermRemarks;
			//        revision.BankGuarantee = localrevision.BankGuarantee;
			//        revision.DeliveryMaxWeeks = localrevision.DeliveryMaxWeeks;
			//        revision.DeliveryMinWeeks = localrevision.DeliveryMinWeeks;

			//        var rfqmasters = from x in vscm.RemoteRFQMasters where x.RfqMasterId == localrevision.rfqMasterId select x;
			//        var masters = new RFQMasterModel();
			//        foreach (var item in rfqmasters)
			//        {
			//            masters.RfqMasterId = item.RfqMasterId;
			//            masters.RfqNo = item.RFQNo;
			//            masters.RfqUniqueNo = item.RFQUniqueNo;
			//            masters.VendorId = item.VendorId;
			//            var vendorMaster = obj.VendorMasters.FirstOrDefault(li => li.Vendorid == item.VendorId);
			//            masters.Vendor = new VendormasterModel();
			//            masters.Vendor.VendorName = vendorMaster.VendorName;
			//            masters.Vendor.Emailid = vendorMaster.Emailid;
			//            masters.MPRRevisionId = (int)item.MPRRevisionId;
			//            masters.CreatedBy = item.CreatedBy;

			//        }
			//        //revision.mprIncharges = obj.MPRIncharges.Where(li => li.RevisionId == masters.MPRRevisionId).ToList();
			//        //revision.rfqmaster = masters;
			//        var rfqitemss = vscm.RemoteRFQItems_N.Where(x => x.RFQRevisionId == localrevision.rfqRevisionId);
			//        //var rfqiteminfo=vscm.RemoteRFQItemsInfoes.Where(x=>x.r)
			//        foreach (var item in rfqitemss)
			//        {
			//            RfqItemModel rfqitems = new RfqItemModel();
			//            rfqitems.HSNCode = item.HSNCode;
			//            rfqitems.MRPItemsDetailsID = item.MPRItemDetailsid;
			//            rfqitems.QuotationQty = item.QuotationQty;
			//            rfqitems.RFQRevisionId = item.RFQRevisionId;
			//            rfqitems.RFQItemID = item.RFQItemsId;
			//            rfqitems.ItemName = item.ItemName;
			//            rfqitems.ItemDescription=item.ItemDescription;
			//            var rfqiteminfo = vscm.RemoteRFQItemsInfo_N.Where(x => x.RFQItemsId == item.RFQItemsId).ToList();
			//            //RfqItemInfoModel iteminfos = new RfqItemInfoModel();
			//            foreach (var iteminfo in rfqiteminfo)
			//            {
			//                rfqitems.RFQSplitItemId = iteminfo.RFQSplitItemId;
			//                rfqitems.QuotationQty = iteminfo.Qty;
			//                rfqitems.ItemUnitPrice = iteminfo.UnitPrice;

			//            }
			//            //revision.rfqitem.Select(x=>x)
			//            var scmRfqdocs = vscm.RemoteRFQDocuments.Where(li => li.rfqItemsid == item.RFQItemsId).ToList();
			//            foreach (var items in scmRfqdocs)
			//            {
			//                RfqDocumentsModel rfqDocs = new RfqDocumentsModel();
			//                rfqDocs.RfqDocumentId = items.RfqDocId;
			//                //rfqDocs.RfqRevisionId = items.rfqRevisionId;
			//                //rfqDocs.RfqItemsId = items.rfqItemsid;
			//                rfqDocs.DocumentName = items.DocumentName;
			//                rfqDocs.Path = items.Path;
			//                rfqDocs.UploadedBy = items.UploadedBy;
			//                rfqDocs.UploadedDate = items.uploadedDate;
			//               // rfqDocs.StatusRemarks = items.StatusRemarks;
			//                rfqDocs.Status = items.Status;
			//                rfqDocs.StatusBy = items.StatusBy;
			//                //rfqDocs.Statusdate = items.StatusDate;
			//                //vscm.RemoteRFQDocuments.Add(rfqDocs);
			//                revision.documents.Add(rfqDocs);
			//            }
			//           // revision.documents.Add(rfqitems);
			//            revision.rfqitem.Add(rfqitems);

			//        }
			//        var rfqterms = vscm.RemoteRfqTerms.Where(x => x.RfqRevisionId == revisionId).ToList();
			//        foreach (var item in rfqterms)
			//        {
			//            RFQTermsModel terms = new RFQTermsModel();
			//            terms.RfqTermsid = item.VRfqTermsid;
			//            terms.termsid = item.termsid;
			//            var termdata = vscm.RemoteRfqTerms.Where(li => li.VRfqTermsid == item.termsid).FirstOrDefault();
			//            terms.Terms =termdata==null ?"": termdata.Terms;

			//            terms.VendorResponse = item.VendorResponse;
			//            terms.Remarks = item.Remarks;
			//            revision.RFQTerms.Add(terms);
			//        }
			//    }
			//}
			//catch (Exception ex)
			//{

			//    throw;
			//}
			//return revision;
		}

		//public async Task<RfqRevisionModel> GetRfqDetailsById(int revisionId)
		//{
		//    RfqRevisionModel revision = new RfqRevisionModel();
		//    try
		//    {
		//        var localrevision = vscm.RemoteRFQRevisions.Where(x => x.rfqRevisionId == revisionId && x.DeleteFlag == false).Include(x => x.RemoteRFQMaster).Include(x => x.RemoteRFQItems_N).FirstOrDefault();
		//        if (localrevision != null)
		//        {
		//            revision.RfqMasterId = localrevision.rfqMasterId;
		//            revision.RfqRevisionNo = localrevision.RevisionNo;
		//            revision.CreatedBy = localrevision.CreatedBy;
		//            revision.CreatedDate = localrevision.CreatedDate;
		//            revision.PackingForwading = localrevision.PackingForwarding;
		//            revision.salesTax = localrevision.SalesTax;
		//            revision.Insurance = localrevision.Insurance;
		//            revision.CustomsDuty = localrevision.CustomsDuty;
		//            revision.PaymentTermDays = localrevision.PaymentTermDays;
		//            revision.PaymentTermRemarks = localrevision.PaymentTermRemarks;
		//            revision.BankGuarantee = localrevision.BankGuarantee;
		//            revision.DeliveryMaxWeeks = localrevision.DeliveryMaxWeeks;
		//            revision.DeliveryMinWeeks = localrevision.DeliveryMinWeeks;


		//            var rfqmasters = from x in obj.RFQMasters where x.RfqMasterId == localrevision.rfqMasterId select x;
		//            var masters = new RFQMasterModel();
		//            foreach (var item in rfqmasters)
		//            {
		//                masters.RfqMasterId = item.RfqMasterId;
		//                masters.RfqNo = item.RFQNo;
		//                masters.RfqUniqueNo = item.RFQUniqueNo;
		//                masters.VendorId = item.VendorId;
		//                var vendorMaster =obj.VendorMasters.FirstOrDefault(li => li.Vendorid == item.VendorId);
		//                masters.Vendor = new VendormasterModel();
		//                masters.Vendor.VendorName = vendorMaster.VendorName;
		//                masters.Vendor.Emailid = vendorMaster.Emailid;
		//                masters.MPRRevisionId = (int)item.MPRRevisionId;
		//                masters.CreatedBy = item.CreatedBy;
		//            }
		//            revision.rfqmaster = masters;
		//            var rfqitemss = obj.RFQItems.Where(x => x.RFQRevisionId == localrevision.rfqRevisionId).ToList();
		//            foreach (var item in rfqitemss)
		//            {
		//                //revision.rfqitem= localrevision.RFQItems.Select(x => new RfqItemModel()
		//                //  {
		//                //      HSNCode = item.HSNCode,
		//                //      RFQItemID = item.RFQItemID,
		//                //      RFQRevisionId = item.RFQRevisionId,
		//                //      QuotationQty = item.QuotationQty,
		//                //      VendorModelNo = item.VendorModelNo,
		//                //      RequsetRemarks = item.RequsetRemarks
		//                //  }).ToList();
		//                MPRItemInfo mprItem = obj.MPRItemInfoes.FirstOrDefault(li => li.Itemdetailsid == item.MPRItemDetailsid);
		//                RfqItemModel rfqitems = new RfqItemModel();
		//                rfqitems.HSNCode = item.HSNCode;
		//                rfqitems.MRPItemsDetailsID = item.MPRItemDetailsid;
		//                rfqitems.QuotationQty = item.QuotationQty;
		//                rfqitems.RFQRevisionId = item.RFQRevisionId;
		//                rfqitems.RFQItemID = item.RFQItemsId;
		//                rfqitems.ItemName = obj.MaterialMasterYGS.FirstOrDefault(li => li.Material == mprItem.Itemid).Materialdescription;
		//                rfqitems.ItemDescription = mprItem.ItemDescription;
		//                revision.rfqitem.Add(rfqitems);
		//            }
		//            var rfqterms = obj.RFQTerms.Where(x => x.RFQrevisionId == revisionId).ToList();
		//            foreach (var item in rfqterms)
		//            {
		//                RFQTermsModel terms = new RFQTermsModel();
		//                terms.RfqTermsid = item.RfqTermsid;
		//                terms.termsid = item.termsid;
		//                terms.Terms = obj.YILTermsandConditions.FirstOrDefault(li=>li.TermId== item.termsid).Terms;
		//                terms.VendorResponse = item.VendorResponse;
		//                terms.Remarks = item.Remarks;

		//                revision.RFQTerms.Add(terms);
		//            }
		//        }
		//    }
		//    catch (Exception ex)
		//    {

		//        throw;
		//    }
		//    return revision;
		//}



		public async Task<statuscheckmodel> InsertSingleIteminfos(RfqItemInfoModel model)
		{
			statuscheckmodel status = new statuscheckmodel();
			try
			{
				vscm.Database.Connection.Open();
				var remoteiteminfo = new RemoteRFQItemsInfo();
				remoteiteminfo.UOM = model.UOM;
				remoteiteminfo.UnitPrice = model.UnitPrice;
				remoteiteminfo.RFQItemsId = model.RFQItemsId;
				remoteiteminfo.DiscountPercentage = model.DiscountPercentage;
				remoteiteminfo.Qty = model.Quantity;
				remoteiteminfo.DeliveryDate = model.DeliveryDate;
				remoteiteminfo.CurrencyValue = model.CurrencyValue;
				remoteiteminfo.CurrencyId = model.CurrencyID;
				remoteiteminfo.Remarks = model.Remarks;

				vscm.RemoteRFQItemsInfoes.Add(remoteiteminfo);
				vscm.SaveChanges();
				int Remotesplitid = remoteiteminfo.RFQSplitItemId;
				vscm.Database.Connection.Close();

				obj.Database.Connection.Open();
				var localiteminfo = new RFQItemsInfo();
				localiteminfo.RFQSplitItemId = Remotesplitid;
				localiteminfo.UOM = model.UOM;
				localiteminfo.UnitPrice = model.UnitPrice;
				localiteminfo.RFQItemsId = model.RFQItemsId;
				localiteminfo.Remarks = model.Remarks;
				localiteminfo.DiscountPercentage = model.DiscountPercentage;
				localiteminfo.Qty = model.Quantity;
				localiteminfo.DeliveryDate = model.DeliveryDate;
				localiteminfo.CurrencyValue = model.CurrencyValue;
				localiteminfo.CurrencyId = model.CurrencyID;
				obj.RFQItemsInfoes.Add(localiteminfo);
				obj.SaveChanges();
				int localsplitid = localiteminfo.RFQSplitItemId;
				status.Sid = localsplitid;
				return status;
			}
			catch (Exception)
			{

				throw;
			}
		}
		public async Task<statuscheckmodel> InsertBulkItemInfos(List<RfqItemInfoModel> model)
		{
			statuscheckmodel status = new statuscheckmodel();
			try
			{
				if (model != null)
				{
					vscm.Database.Connection.Open();
					var remoteiteminfo = new RemoteRFQItemsInfo();
					foreach (var item in model)
					{
						remoteiteminfo.UOM = item.UOM;
						remoteiteminfo.UnitPrice = item.UnitPrice;
						remoteiteminfo.RFQItemsId = item.RFQItemsId;
						//remoteiteminfo.GSTPercentage = item.GSTPercentage;
						remoteiteminfo.DiscountPercentage = item.DiscountPercentage;
						remoteiteminfo.Qty = item.Quantity;
						remoteiteminfo.DeliveryDate = item.DeliveryDate;
						remoteiteminfo.CurrencyValue = item.CurrencyValue;
						remoteiteminfo.CurrencyId = item.CurrencyID;
						vscm.RemoteRFQItemsInfoes.Add(remoteiteminfo);
						vscm.SaveChanges();
					}
					//List<int> id=vscm.RemoteRFQItemsInfoes.Where
					// vscm.SaveChanges();
				}
				vscm.Database.Connection.Close();
				if (model != null)
				{
					obj.Database.Connection.Open();
					var localiteminfo = new RFQItemsInfo();
					foreach (var item in model)
					{
						localiteminfo.UOM = item.UOM;
						localiteminfo.UnitPrice = item.UnitPrice;
						localiteminfo.RFQItemsId = item.RFQItemsId;
						//localiteminfo.GSTPercentage = item.GSTPercentage;
						localiteminfo.DiscountPercentage = item.DiscountPercentage;
						localiteminfo.Qty = item.Quantity;
						localiteminfo.DeliveryDate = item.DeliveryDate;
						localiteminfo.CurrencyValue = item.CurrencyValue;
						localiteminfo.CurrencyId = item.CurrencyID;
						obj.RFQItemsInfoes.Add(localiteminfo);
						obj.SaveChanges();
					}
				}
				return status;
			}
			catch (Exception)
			{

				throw;
			}
		}
		public async Task<statuscheckmodel> InsertRfqRemainder(RfqRemainderTrackingModel model)
		{
			statuscheckmodel status = new statuscheckmodel();
			try
			{
				vscm.Database.Connection.Open();
				var remotecommunications = vscm.RemoteRFQCommunications.Where(x => x.RfqCCid == model.rfqccid && x.DeleteFlag == false).FirstOrDefault();
				var remotetracking = new RemoteRFQReminderTracking();
				if (remotecommunications != null)
				{
					remotetracking.rfqccid = model.rfqccid;
					remotetracking.ReminderTo = model.ReminderTo;
					remotetracking.MailsSentOn = model.MailsSentOn;
					remotetracking.Acknowledgementon = model.Acknowledgementon;
					remotetracking.AcknowledgementRemarks = model.AcknowledgementRemarks;
					vscm.RemoteRFQReminderTrackings.Add(remotetracking);
					vscm.SaveChanges();
				}
				int rid = remotetracking.Reminderid;
				vscm.Database.Connection.Close();

				var localtracking = new RFQReminderTracking();
				obj.Database.Connection.Open();
				var localcommunication = obj.RFQCommunications.Where(x => x.RfqCCid == model.rfqccid && x.DeleteFlag == false).FirstOrDefault();
				if (localcommunication != null)
				{
					localtracking.Reminderid = rid;
					localtracking.rfqccid = model.rfqccid;
					localtracking.ReminderTo = model.ReminderTo;
					localtracking.MailsSentOn = model.MailsSentOn;
					localtracking.Acknowledgementon = model.Acknowledgementon;
					localtracking.AcknowledgementRemarks = model.AcknowledgementRemarks;
					obj.RFQReminderTrackings.Add(localtracking);
					obj.SaveChanges();
				}
				status.Sid = rid;
				return status;
			}
			catch (Exception ex)
			{

				throw;
			}
		}
		public async Task<List<UnitMasterModel>> GetUnitMasterList()
		{
			List<UnitMasterModel> model = new List<UnitMasterModel>();
			try
			{
				var unitmaster = vscm.RemoteUnitMasters.Where(x => x.DeleteFlag == false).ToList();
				model = unitmaster.Select(x => new UnitMasterModel()
				{
					UnitID = x.UnitID,
					UnitName = x.UnitName,
					//Isdeleted=x.DeleteFlag
				}).ToList();
				return model;
			}
			catch (Exception ex)
			{
				throw;
			}
		}
		public async Task<RfqRemainderTrackingModel> getrfqremaindersById(int id)
		{
			RfqRemainderTrackingModel model = new RfqRemainderTrackingModel();
			try
			{
				var Tracking = obj.RFQReminderTrackings.Where(x => x.rfqccid == id && x.DeleteFlag == false).FirstOrDefault();
				if (Tracking != null)
				{
					model.Reminderid = Tracking.Reminderid;
					model.ReminderTo = Tracking.ReminderTo;
					model.MailsSentOn = Tracking.MailsSentOn;
					model.Acknowledgementon = Tracking.Acknowledgementon;
					model.AcknowledgementRemarks = Tracking.AcknowledgementRemarks;
				}
				return model;
			}
			catch (Exception ex)
			{

				throw;
			}
		}
		//public statuscheckmodel InsertDocument(RfqDocumentsModel model)
		//{
		//    throw new NotImplementedException();
		//}
		public async Task<statuscheckmodel> Insertrfqvendorterms(RfqVendorTermModel model)
		{
			statuscheckmodel status = new statuscheckmodel();
			var rfqterms = new RemoteRFQVendorTerm();
			try
			{
				vscm.Database.Connection.Open();
				if (model != null)
				{
					rfqterms.RFQversionId = model.RFQversionId;
					rfqterms.VendorTermsid = model.VendorTermsid;
					rfqterms.updatedBY = model.updatedBY;
					rfqterms.UpdateDate = model.UpdateDate;
				}
				vscm.RemoteRFQVendorTerms.Add(rfqterms);
				vscm.SaveChanges();
				int remoterfqid = rfqterms.RFQTermsid;
				vscm.Database.Connection.Close();
				var rfqlocalterms = new RFQVendorTerm();
				obj.Database.Connection.Open();
				if (model != null)
				{
					rfqlocalterms.RFQTermsid = remoterfqid;
					rfqlocalterms.RFQversionId = model.RFQversionId;
					rfqlocalterms.VendorTermsid = model.VendorTermsid;
					rfqlocalterms.updatedBY = model.updatedBY;
					rfqlocalterms.UpdateDate = model.UpdateDate;
				}
				obj.RFQVendorTerms.Add(rfqlocalterms);
				obj.SaveChanges();
				status.Sid = remoterfqid;
				return status;
			}
			catch (Exception ex)
			{

				throw;
			}
		}
		public async Task<RfqVendorTermModel> getRfqVendorById(int id)
		{
			RfqVendorTermModel model = new RfqVendorTermModel();
			try
			{
				var remotedata = vscm.RemoteRFQVendorTerms.Where(x => x.RFQversionId == id).SingleOrDefault();
				model.RFQTermsid = remotedata.RFQTermsid;
				model.RFQversionId = remotedata.RFQversionId;
				var terms = from x in vscm.RemoteVendorRFQTerms where x.VendorTermsid == remotedata.VendorTermsid select x;
				var vendorterms = new VendorRfqtermModel();
				foreach (var item in terms)
				{
					vendorterms.Terms = item.Terms;
					vendorterms.Indexno = item.Indexno;
					vendorterms.TermsCategoryId = item.TermsCategoryId;
					vendorterms.VendorID = item.VendorID;
				}
				model.VendorRFQTerm = vendorterms;
				return model;
			}
			catch (Exception ex)
			{
				throw;
			}
		}
		public async Task<statuscheckmodel> RemoveRfqVendorTermsById(int id)
		{
			statuscheckmodel status = new statuscheckmodel();
			try
			{
				vscm.Database.Connection.Open();
				var rfqterms = vscm.RemoteRFQVendorTerms.Where(x => x.RFQversionId == id && x.DeleteFlag == false).SingleOrDefault();
				if (rfqterms != null)
				{
					rfqterms.DeleteFlag = true;
					vscm.SaveChanges();
				}
				vscm.Database.Connection.Close();

				var rfqlocalterms = obj.RFQVendorTerms.Where(x => x.RFQversionId == id && x.DeleteFlag == false).SingleOrDefault();
				if (rfqlocalterms != null)
				{
					rfqlocalterms.DeleteFlag = true;
					obj.SaveChanges();
				}
				return status;
			}
			catch (Exception ex)
			{
				throw;
			}
		}
		public async Task<VendorRfqtermModel> getVendorRfqTermsByid(int id)
		{
			VendorRfqtermModel rfqterm = new VendorRfqtermModel();
			try
			{
				var vendorrfq = vscm.RemoteVendorRFQTerms.Where(x => x.VendorTermsid == id && x.deleteFlag == false).SingleOrDefault();
				if (vendorrfq != null)
				{
					rfqterm.VendorTermsid = vendorrfq.VendorTermsid;
					rfqterm.TermsCategoryId = vendorrfq.TermsCategoryId;
					rfqterm.VendorID = vendorrfq.VendorID;
					rfqterm.Terms = vendorrfq.Terms;
					rfqterm.Indexno = vendorrfq.Indexno;
				}
				return rfqterm;
			}
			catch (Exception ex)
			{
				throw;
			}
		}
		public async Task<statuscheckmodel> RemoveVendorRfqByid(int id)
		{
			statuscheckmodel status = new statuscheckmodel();
			vscm.Database.Connection.Open();
			try
			{
				var remotevendor = vscm.RemoteVendorRFQTerms.Where(x => x.VendorTermsid == id && x.deleteFlag == false).SingleOrDefault();
				if (remotevendor != null)
				{
					remotevendor.deleteFlag = true;
					vscm.SaveChanges();
				}
				var data = remotevendor.VendorTermsid;
				vscm.Database.Connection.Close();

				obj.Database.Connection.Open();
				var localvendor = obj.VendorRFQTerms.Where(x => x.VendorTermsid == data && x.deleteFlag == false).SingleOrDefault();
				if (localvendor != null)
				{
					localvendor.deleteFlag = true;
					obj.SaveChanges();
				}
				status.Sid = data;
				return status;
			}
			catch (Exception ex)
			{
				throw;
			}
		}
		public async Task<statuscheckmodel> InsertNewCurrencyMaster(CurrencyMasterModel model)
		{
			statuscheckmodel status = new statuscheckmodel();
			try
			{
				obj.Database.Connection.Open();
				var data = new CurrencyMaster();
				if (model != null)
				{
					data.CurrencyName = model.CurrencyName;
					data.CurrencyCode = model.CurrencyCode;
					data.UpdatedBy = model.UpdatedBy;
					data.updateddate = model.updateddate;
					data.DeletedBy = model.DeletedBy;
					data.DeletedDate = model.DeletedDate;
					data.DeleteFlag = false;
				}
				obj.CurrencyMasters.Add(data);
				obj.SaveChanges();
				Byte currenyid = data.CurrencyId;
				obj.Database.Connection.Close();
				var remotedata = new RemoteCurrencyMaster();
				vscm.Database.Connection.Open();
				if (model != null)
				{
					remotedata.CurrencyId = currenyid;
					remotedata.CurrencyCode = model.CurrencyCode;
					remotedata.CurrencyName = model.CurrencyName;
					remotedata.UpdatedBy = model.UpdatedBy;
					remotedata.updateddate = model.updateddate;
					remotedata.DeletedBy = model.DeletedBy;
					remotedata.DeletedDate = model.DeletedDate;
					remotedata.DeleteFlag = false;
				}
				vscm.RemoteCurrencyMasters.Add(remotedata);
				vscm.SaveChanges();
				status.Sid = currenyid;
				return status;
			}
			catch (Exception ex)
			{
				throw;
			}
		}
		public async Task<statuscheckmodel> UpdateNewCurrencyMaster(CurrencyMasterModel model)
		{
			statuscheckmodel status = new statuscheckmodel();
			try
			{
				obj.Database.Connection.Open();
				var data = obj.CurrencyMasters.Where(x => x.CurrencyId == model.CurrencyId && x.DeleteFlag == false).FirstOrDefault();
				if (model != null)
				{
					data.CurrencyName = model.CurrencyName;
					data.CurrencyCode = model.CurrencyCode;
					data.UpdatedBy = model.UpdatedBy;
					data.updateddate = model.updateddate;
					data.DeletedBy = model.DeletedBy;
					data.DeletedDate = model.DeletedDate;
					data.DeleteFlag = false;
				}
				obj.CurrencyMasters.Add(data);
				obj.SaveChanges();
				Byte currenyid = data.CurrencyId;
				obj.Database.Connection.Close();
				var remotedata = new RemoteCurrencyMaster();
				vscm.Database.Connection.Open();
				if (model != null)
				{
					remotedata.CurrencyId = currenyid;
					remotedata.CurrencyCode = model.CurrencyCode;
					remotedata.CurrencyName = model.CurrencyName;
					remotedata.UpdatedBy = model.UpdatedBy;
					remotedata.updateddate = model.updateddate;
					remotedata.DeletedBy = model.DeletedBy;
					remotedata.DeletedDate = model.DeletedDate;
					remotedata.DeleteFlag = false;
				}
				vscm.RemoteCurrencyMasters.Add(remotedata);
				vscm.SaveChanges();
				status.Sid = currenyid;
				return status;
			}
			catch (Exception ex)
			{
				throw;
			}
		}
		public async Task<statuscheckmodel> InsertCurrentCurrencyHistory(CurrencyHistoryModel model)
		{
			statuscheckmodel status = new statuscheckmodel();
			try
			{
				var data = new CurrencyHistory();
				obj.Database.Connection.Open();
				if (model != null)
				{
					data.CurrencyId = model.CurrencyId;
					data.CurrencyValue = model.CurrencyValue;
					data.EffectiveFrom = model.EffectiveFrom;
					data.EffectiveTo = model.EffectiveTo;
					data.UpdatedBy = model.UpdatedBy;
					data.UpdatedDate = model.UpdatedDate;
					data.IsActive = true;
				}
				obj.CurrencyHistories.Add(data);
				obj.SaveChanges();
				int historyid = data.CurrencyHistoryId;
				obj.Database.Connection.Close();

				var Remotedata = new RemoteCurrencyHistory();
				vscm.Database.Connection.Open();
				if (model != null)
				{
					Remotedata.CurrencyId = model.CurrencyId;
					Remotedata.CurrencyValue = model.CurrencyValue;
					Remotedata.EffectiveFrom = model.EffectiveFrom;
					Remotedata.EffectiveTo = model.EffectiveTo;
					Remotedata.UpdatedBy = model.UpdatedBy;
					Remotedata.UpdatedDate = model.UpdatedDate;
					Remotedata.IsActive = true;
				}
				vscm.RemoteCurrencyHistories.Add(Remotedata);
				vscm.SaveChanges();
				status.Sid = historyid;
				return status;
			}
			catch (Exception ex)
			{
				throw;
			}
		}
		public async Task<statuscheckmodel> UpdateCurrentCurrencyHistory(CurrencyHistoryModel model)
		{
			statuscheckmodel status = new statuscheckmodel();
			try
			{

				obj.Database.Connection.Open();
				var data = obj.CurrencyHistories.Where(x => x.CurrencyId == model.CurrencyId).FirstOrDefault();
				if (model != null)
				{
					data.CurrencyId = model.CurrencyId;
					data.CurrencyValue = model.CurrencyValue;
					data.EffectiveFrom = model.EffectiveFrom;
					data.EffectiveTo = model.EffectiveTo;
					data.UpdatedBy = model.UpdatedBy;
					data.UpdatedDate = model.UpdatedDate;
					data.IsActive = true;
				}
				obj.CurrencyHistories.Add(data);
				obj.SaveChanges();
				int historyid = data.CurrencyHistoryId;
				obj.Database.Connection.Close();


				vscm.Database.Connection.Open();
				var Remotedata = vscm.RemoteCurrencyHistories.Where(x => x.CurrencyId == model.CurrencyId).FirstOrDefault();
				if (model != null)
				{
					Remotedata.CurrencyId = model.CurrencyId;
					Remotedata.CurrencyValue = model.CurrencyValue;
					Remotedata.EffectiveFrom = model.EffectiveFrom;
					Remotedata.EffectiveTo = model.EffectiveTo;
					Remotedata.UpdatedBy = model.UpdatedBy;
					Remotedata.UpdatedDate = model.UpdatedDate;
					Remotedata.IsActive = true;
				}
				vscm.RemoteCurrencyHistories.Add(Remotedata);
				vscm.SaveChanges();
				status.Sid = historyid;
				return status;
			}
			catch (Exception ex)
			{
				throw;
			}
		}
		public async Task<List<CurrencyMasterModel>> GetAllMasterCurrency()
		{
			List<CurrencyMasterModel> model = new List<CurrencyMasterModel>();
			try
			{
				var currencydata = vscm.RemoteCurrencyMasters.Where(x => x.DeleteFlag == false).ToList();
				model = currencydata.Select(x => new CurrencyMasterModel()
				{
					CurrencyCode = x.CurrencyCode,
					CurrencyName = x.CurrencyName,
					CurrencyId = x.CurrencyId,
					//UpdatedBy = x.UpdatedBy,
					//updateddate = x.updateddate,
					// DeletedBy = x.DeletedBy,
					// DeletedDate = x.DeletedDate
				}).ToList();
				return model;
			}
			catch (Exception ex)
			{
				throw;
			}
		}
		public async Task<CurrencyMasterModel> GetMasterCurrencyById(int currencyId)
		{
			CurrencyMasterModel model = new CurrencyMasterModel();
			try
			{
				var currencydata = obj.CurrencyMasters.Where(x => x.CurrencyId == currencyId && x.DeleteFlag == false).FirstOrDefault<CurrencyMaster>();
				model.CurrencyName = currencydata.CurrencyName;
				model.CurrencyCode = currencydata.CurrencyCode;
				model.UpdatedBy = currencydata.UpdatedBy;
				model.updateddate = currencydata.updateddate;
				model.DeletedBy = currencydata.DeletedBy;
				model.DeletedDate = currencydata.DeletedDate;
				return model;
			}
			catch (Exception ex)
			{
				throw;
			}
		}
		public async Task<statuscheckmodel> RemoveMasterCurrencyById(int currencyId)
		{
			statuscheckmodel status = new statuscheckmodel();
			try
			{
				var currencydata = obj.CurrencyMasters.Where(x => x.CurrencyId == currencyId && x.DeleteFlag == false).FirstOrDefault<CurrencyMaster>();
				if (currencydata != null)
				{
					currencydata.DeleteFlag = true;
					obj.SaveChanges();

					var currencyhistorydata = obj.CurrencyHistories.Where(x => x.CurrencyId == currencydata.CurrencyId).FirstOrDefault<CurrencyHistory>();
					if (currencyhistorydata != null)
					{
						currencyhistorydata.IsActive = false;
						obj.SaveChanges();
					}
				}
				int id = currencydata.CurrencyId;
				status.Sid = id;
				return status;
			}
			catch (Exception ex)
			{
				throw;
			}
		}
		public async Task<CurrencyHistoryModel> GetcurrencyHistoryById(int currencyId)
		{
			CurrencyHistoryModel model = new CurrencyHistoryModel();
			try
			{
				var currenyhistorydata = obj.CurrencyHistories.Where(x => x.CurrencyId == currencyId && x.IsActive == true).ToList();
				foreach (var item in currenyhistorydata)
				{
					model.CurrencyHistoryId = item.CurrencyHistoryId;
					model.CurrencyValue = item.CurrencyValue;
					model.editedBy = item.editedBy;
					model.EditedDate = item.EditedDate;
					model.EffectiveFrom = item.EffectiveFrom;
					model.EffectiveTo = item.EffectiveTo;
					model.UpdatedBy = item.UpdatedBy;
					model.UpdatedDate = item.UpdatedDate;
				}
				return model;
			}
			catch (Exception ex)
			{
				throw;
			}
		}

		public async Task<statuscheckmodel> InsertMprBuyerGroups(MPRBuyerGroupModel model)
		{
			statuscheckmodel status = new statuscheckmodel();
			try
			{
				var data = new MPRBuyerGroup();
				if (model != null && model.BuyerGroupId == 0)
				{
					// data.BuyerGroupId = model.BuyerGroupId;
					data.BuyerGroup = model.BuyerGroup;
					data.BoolInUse = true;
				}
				obj.MPRBuyerGroups.Add(data);
				obj.SaveChanges();
				byte memberid = data.BuyerGroupId;
				foreach (var item in model.MPRBuyerGroup)
				{
					var groupmembers = new MPRBuyerGroupMember()
					{
						BuyerGroupId = memberid,
						GroupMember = item.GroupMember
					};
					data.MPRBuyerGroupMembers.Add(groupmembers);
					obj.SaveChanges();
				}
				int id = data.BuyerGroupId;
				status.Sid = id;
				return status;
			}
			catch (Exception ex)
			{
				throw;
			}
		}
		public async Task<statuscheckmodel> UpdateMprBuyerGroups(MPRBuyerGroupModel model)
		{
			statuscheckmodel status = new statuscheckmodel();
			try
			{
				var data = obj.MPRBuyerGroups.Where(x => x.BuyerGroupId == model.BuyerGroupId).FirstOrDefault();
				if (model != null)
				{
					data.BuyerGroup = model.BuyerGroup;
					data.BoolInUse = model.BoolInUse;
					obj.SaveChanges();
				}
				int id = data.BuyerGroupId;
				status.Sid = id;
				return status;
			}
			catch (Exception)
			{

				throw;
			}
		}
		public async Task<statuscheckmodel> InsertMprBuyerGroupMembers(MPRBuyerGroupMemberModel model)
		{
			statuscheckmodel status = new statuscheckmodel();
			try
			{
				if (true)
				{

				}
				var groupmember = new MPRBuyerGroupMember();
				groupmember.BuyerGroupMemberId = model.BuyerGroupMemberId;
				groupmember.BuyerGroupId = model.BuyerGroupId;
				groupmember.GroupMember = model.GroupMember;
				obj.MPRBuyerGroupMembers.Add(groupmember);
				obj.SaveChanges();
				int memberid = groupmember.BuyerGroupMemberId;
				status.Sid = memberid;
				return status;
			}
			catch (Exception ex)
			{

				throw;
			}
		}
		public async Task<statuscheckmodel> InsertMPRApprover(MPRApproverModel model)
		{
			statuscheckmodel status = new statuscheckmodel();
			try
			{
				var data = new MPRApprover();
				data.EmployeeNo = model.EmployeeNo;
				data.BoolActive = true;
				obj.MPRApprovers.Add(data);
				obj.SaveChanges();
				return status;
			}
			catch (Exception ex)
			{
				throw;
			}
		}
		public async Task<statuscheckmodel> InsertMprApprovers(MPRApproverModel model)
		{
			statuscheckmodel status = new statuscheckmodel();
			try
			{
				var data = new MPRApprover();
				data.EmployeeNo = model.EmployeeNo;
				data.DeactivatedBy = model.DeactivatedBy;
				data.DeactivatedOn = model.DeactivatedOn;
				obj.MPRApprovers.Add(data);
				obj.SaveChanges();
				return status;
			}
			catch (Exception ex)
			{
				throw;
			}
		}
		public async Task<statuscheckmodel> UpdateMprApprovers(MPRApproverModel model)
		{
			statuscheckmodel status = new statuscheckmodel();
			try
			{
				var data = obj.MPRApprovers.Where(x => x.EmployeeNo == model.EmployeeNo).FirstOrDefault();
				data.BoolActive = model.BoolActive;
				data.DeactivatedBy = model.DeactivatedBy;
				data.DeactivatedOn = model.DeactivatedOn;
				obj.MPRApprovers.Add(data);
				obj.SaveChanges();
				return status;
			}
			catch (Exception ex)
			{
				throw;
			}
		}
		public async Task<statuscheckmodel> InsertMprDepartMents(MPRDepartmentModel model)
		{
			statuscheckmodel status = new statuscheckmodel();
			try
			{
				var data = new MPRDepartment();
				data.Department = model.Department;
				data.SecondApprover = model.SecondApprover;
				data.ThirdApprover = model.ThirdApprover;
				obj.MPRDepartments.Add(data);
				obj.SaveChanges();
				int deptid = data.DepartmentId;
				status.Sid = deptid;
				return status;
			}
			catch (Exception ex)
			{
				throw;
			}
		}
		public async Task<statuscheckmodel> UpdateMprDepartMents(MPRDepartmentModel model)
		{
			statuscheckmodel status = new statuscheckmodel();
			try
			{
				var data = obj.MPRDepartments.Where(x => x.DepartmentId == model.DepartmentId).FirstOrDefault();
				data.Department = model.Department;
				data.SecondApprover = model.SecondApprover;
				data.ThirdApprover = model.ThirdApprover;
				data.BoolInUse = model.BoolInUse;
				obj.SaveChanges();
				int deptid = data.DepartmentId;
				status.Sid = deptid;
				return status;
			}
			catch (Exception ex)
			{
				throw;
			}
		}
		public async Task<statuscheckmodel> InsertMprProcurement(MPRProcurementSourceModel model)
		{
			statuscheckmodel status = new statuscheckmodel();
			var data = new MPRProcurementSource();
			try
			{
				if (model.ProcurementSourceId == 0)
				{
					data.ProcurementSource = model.ProcurementSource;
					obj.MPRProcurementSources.Add(data);
					obj.SaveChanges();
				}
				else
				{
					var mprdata = obj.MPRProcurementSources.Where(x => x.ProcurementSourceId == model.ProcurementSourceId).FirstOrDefault();
					mprdata.ProcurementSource = model.ProcurementSource;
					mprdata.BoolInUse = model.BoolInUse;
					obj.SaveChanges();
				}

				int pid = data.ProcurementSourceId;
				status.Sid = pid;
				return status;
			}
			catch (Exception ex)
			{
				throw;
			}
		}
		public async Task<statuscheckmodel> InsertGlobalgroup(GlobalGroupModel model)
		{
			statuscheckmodel status = new statuscheckmodel();
			try
			{
				var data = new GlobalGroup();
				data.GlobalGroupDescription = model.GlobalGroupDescription;
				obj.GlobalGroups.Add(data);
				obj.SaveChanges();
				int pid = data.GlobalGroupId;
				status.Sid = pid;
				return status;
			}
			catch (Exception ex)
			{
				throw;
			}
		}
		public async Task<statuscheckmodel> InsertGlobalgroupEmployee(GlobalGroupEmployeeModel model)
		{
			statuscheckmodel status = new statuscheckmodel();
			try
			{
				var data = new GlobalGroupEmployee();
				data.GlobalGroupId = model.GlobalGroupId;
				data.EmployeeNo = model.EmployeeNo;
				// data.UpdatedOn = model.UpdatedOn;
				obj.GlobalGroupEmployees.Add(data);
				obj.SaveChanges();

				return status;
			}
			catch (Exception ex)
			{
				throw;
			}
		}
		public async Task<statuscheckmodel> InsertMPRDispatchLocations(MPRDispatchLocationModel model)
		{
			statuscheckmodel status = new statuscheckmodel();

			try
			{
				var data = new MPRDispatchLocation();
				if (model.DispatchLocationId == 0)
				{
					data.DispatchLocation = model.DispatchLocation;
					data.XOrder = model.XOrder;
					obj.MPRDispatchLocations.Add(data);
					obj.SaveChanges();
				}
				else
				{
					var mprlocation = obj.MPRDispatchLocations.Where(x => x.DispatchLocationId == model.DispatchLocationId).FirstOrDefault();
					mprlocation.DispatchLocation = model.DispatchLocation;
					mprlocation.XOrder = model.XOrder;
					mprlocation.BoolInUse = model.BoolInUse;
					obj.SaveChanges();
				}
				return status;
			}
			catch (Exception ex)
			{
				throw;
			}
		}
		public async Task<statuscheckmodel> InsertMPRDocumentationDescription(MPRDocumentationDescriptionModel model)
		{
			statuscheckmodel status = new statuscheckmodel();
			try
			{
				var data = new MPRDocumentationDescription();
				if (model.DocumentationDescriptionId == 0)
				{
					data.DocumentationDescription = model.DocumentationDescription;
					obj.MPRDocumentationDescriptions.Add(data);
					obj.SaveChanges();
				}
				else
				{

				}
				return status;
			}
			catch (Exception ex)
			{
				throw;
			}
		}
		public async Task<statuscheckmodel> InsertMPRCustomDuty(MPRCustomsDutyModel model)
		{
			statuscheckmodel status = new statuscheckmodel();
			try
			{
				var data = new MPRCustomsDuty();
				data.CustomsDutyId = model.CustomsDutyId;
				data.CustomsDuty = model.CustomsDuty;
				data.BoolInUse = true;
				obj.SaveChanges();
				return status;
			}
			catch (Exception ex)
			{
				throw;
			}
		}
		public async Task<MPRBuyerGroupModel> GetMPRBuyerGroupsById(int id)
		{
			MPRBuyerGroupModel model = new MPRBuyerGroupModel();
			try
			{
				var data = obj.MPRBuyerGroups.SqlQuery("select * from  MPRBuyerGroups where BuyerGroupId=@id and BoolInUse=1", new SqlParameter("@id", id)).FirstOrDefault();
				model.BuyerGroup = data.BuyerGroup;
				model.BoolInUse = data.BoolInUse;
				return model;
			}
			catch (Exception ex)
			{
				throw;
			}
		}
		public async Task<List<MPRBuyerGroupModel>> GetAllMPRBuyerGroups()
		{
			List<MPRBuyerGroupModel> model = new List<MPRBuyerGroupModel>();
			try
			{
				var data = obj.MPRBuyerGroups.SqlQuery("select * from  MPRBuyerGroups where  BoolInUse=1");
				model = data.Select(x => new MPRBuyerGroupModel()
				{
					BuyerGroup = x.BuyerGroup,
					BuyerGroupId = x.BuyerGroupId,
					BoolInUse = x.BoolInUse
				}).ToList();
				return model;
			}
			catch (Exception ex)
			{
				throw;
			}
		}
		public async Task<MPRApproverModel> GetMPRApprovalsById(int id)
		{
			MPRApproverModel model = new MPRApproverModel();
			try
			{
				var data = obj.MPRApprovers.SqlQuery("select * from  MPRApprovers where EmployeeNo=@id and BoolActive=1", new SqlParameter("@id", id)).ToList();
				model = data.Select(x => new MPRApproverModel()
				{
					EmployeeNo = x.EmployeeNo,
					DeactivatedBy = x.DeactivatedBy,
					DeactivatedOn = x.DeactivatedOn
				}).FirstOrDefault();
				return model;
			}
			catch (Exception ex)
			{
				throw;
			}
		}
		public async Task<List<MPRApproverModel>> GetAllMPRApprovals()
		{
			List<MPRApproverModel> model = new List<MPRApproverModel>();
			try
			{
				var data = obj.MPRApprovers.SqlQuery("select * from  MPRApprovers where  BoolInUse=1");
				model = data.Select(x => new MPRApproverModel()
				{
					EmployeeNo = x.EmployeeNo,
					DeactivatedBy = x.DeactivatedBy,
					DeactivatedOn = x.DeactivatedOn
				}).ToList();
				return model;
			}
			catch (Exception ex)
			{
				throw;
			}
		}
		public async Task<List<MPRDepartmentModel>> GetAllMPRDepartments()
		{
			List<MPRDepartmentModel> model = new List<MPRDepartmentModel>();
			try
			{
				var data = obj.MPRDepartments.SqlQuery("select * from  MPRDepartments where  BoolInUse=1");
				model = data.Select(x => new MPRDepartmentModel()
				{
					DepartmentId = x.DepartmentId,
					Department = x.Department,
					SecondApprover = x.SecondApprover,
					ThirdApprover = x.ThirdApprover
				}).ToList();
				return model;
			}
			catch (Exception ex)
			{
				throw;
			}
		}
		public async Task<MPRDepartmentModel> GetMPRDepartmentById(int id)
		{
			MPRDepartmentModel model = new MPRDepartmentModel();
			try
			{
				var data = obj.MPRDepartments.SqlQuery("select * from  MPRDepartments where DepartmentId=@id and BoolActive=1", new SqlParameter("@id", id)).ToList();
				model = data.Select(x => new MPRDepartmentModel()
				{
					DepartmentId = x.DepartmentId,
					Department = x.Department,
					SecondApprover = x.SecondApprover,
					ThirdApprover = x.ThirdApprover
				}).FirstOrDefault();
				return model;
			}
			catch (Exception ex)
			{
				throw;
			}
		}
		public async Task<List<MPRDispatchLocationModel>> GetAllMPRDispatchLocations()
		{
			List<MPRDispatchLocationModel> model = new List<MPRDispatchLocationModel>();
			try
			{
				var data = obj.MPRDispatchLocations.SqlQuery("select * from  MPRDepartments where  BoolInUse=1");
				model = data.Select(x => new MPRDispatchLocationModel()
				{
					DispatchLocationId = x.DispatchLocationId,
					DispatchLocation = x.DispatchLocation,
					XOrder = x.XOrder,
					BoolInUse = x.BoolInUse
				}).ToList();
				return model;
			}
			catch (Exception ex)
			{
				throw;
			}
		}
		public async Task<MPRDispatchLocationModel> GetMPRDispatchLocationById(int id)
		{
			MPRDispatchLocationModel model = new MPRDispatchLocationModel();
			try
			{
				var data = obj.MPRDispatchLocations.SqlQuery("select * from  MPRDispatchLocations where DispatchLocationId=@id and BoolActive=1", new SqlParameter("@id", id)).ToList();
				model = data.Select(x => new MPRDispatchLocationModel()
				{
					DispatchLocationId = x.DispatchLocationId,
					DispatchLocation = x.DispatchLocation,
					XOrder = x.XOrder,
					BoolInUse = x.BoolInUse
				}).FirstOrDefault();
				return model;
			}
			catch (Exception ex)
			{
				throw;
			}
		}
		public async Task<List<MPRCustomsDutyModel>> GetAllCustomDuty()
		{
			List<MPRCustomsDutyModel> model = new List<MPRCustomsDutyModel>();
			try
			{
				var data = obj.MPRCustomsDuties.SqlQuery("select * from  MPRCustomsDuty where  BoolActive=1");
				model = data.Select(x => new MPRCustomsDutyModel()
				{
					CustomsDutyId = x.CustomsDutyId,
					CustomsDuty = x.CustomsDuty,
					BoolInUse = x.BoolInUse
				}).ToList();
				return model;
			}
			catch (Exception ex)
			{
				throw;
			}
		}
		public async Task<statuscheckmodel> InsertYILTerms(YILTermsandConditionModel model)
		{
			statuscheckmodel status = new statuscheckmodel();
			try
			{
				var data = new YILTermsandCondition();
				if (model != null)
				{
					data.TermGroupId = model.TermGroupId;
					data.BuyerGroupId = model.BuyerGroupId;
					data.Terms = model.Terms;
					data.DisplayOrder = model.DisplayOrder;
					data.DefaultSelect = true;
					data.CreatedBy = model.CreatedBy;
					data.CreatedDate = model.CreatedDate;
					data.DeletedBy = model.DeletedBy;
					data.DeletedDate = model.DeletedDate;
				}
				obj.YILTermsandConditions.Add(data);
				obj.SaveChanges();
				int termsid = data.TermId;
				status.Sid = termsid;
				return status;
			}
			catch (Exception ex)
			{
				throw;
			}
		}
		public async Task<statuscheckmodel> InsertYILTermsGroup(YILTermsGroupModel model)
		{
			statuscheckmodel status = new statuscheckmodel();
			try
			{
				var data = new YILTermsGroup();
				if (model != null)
				{
					data.TermGroup = model.TermGroup;
					data.CreatedBy = model.CreatedBy;
					data.CreatedDate = model.CreatedDate;
					data.DeletedBy = model.DeletedBy;
					data.DeletedDate = model.DeletedDate;
				}
				obj.YILTermsGroups.Add(data);
				obj.SaveChanges();
				int termgroupid = data.TermGroupId;
				status.Sid = termgroupid;
				return status;
			}
			catch (Exception)
			{

				throw;
			}
		}
		public async Task<statuscheckmodel> InsertRFQTerms(RFQTermsModel model)
		{
			statuscheckmodel status = new statuscheckmodel();
			try
			{
				var remoteterm = new RemoteRfqTerm();
				vscm.Database.Connection.Open();
				if (model != null)
				{
					remoteterm.RfqRevisionId = model.RFQrevisionId;
					remoteterm.termsid = model.termsid;
					remoteterm.VendorResponse = model.VendorResponse;
					remoteterm.TermGroup = model.TermGroup;
					remoteterm.Remarks = model.Remarks;
					remoteterm.Terms = model.Terms;
					remoteterm.CreatedBy = model.CreatedBy;
					remoteterm.CreatedDate = model.CreatedDate;
					remoteterm.UpdatedBy = model.UpdatedBy;
					remoteterm.UpdatedDate = model.UpdatedDate;
					remoteterm.DeletedBy = model.DeletedBy;
					remoteterm.DeletedDate = model.DeletedDate;
					//remoteterm.SyncStatus = true;
				}
				vscm.RemoteRfqTerms.Add(remoteterm);
				vscm.SaveChanges();
				vscm.Database.Connection.Close();
				int termsid = remoteterm.VRfqTermsid;

				var rfqterm = new RFQTerm();
				//obj.Database.Connection.Open();
				if (model != null)
				{
					rfqterm.RfqTermsid = termsid;
					rfqterm.RFQrevisionId = model.RFQrevisionId;
					rfqterm.termsid = model.termsid;
					rfqterm.VendorResponse = model.VendorResponse;
					//rfqterm.TermGroup = model.TermGroup;
					rfqterm.Remarks = model.Remarks;
					rfqterm.CreatedBy = model.CreatedBy;
					rfqterm.CreatedDate = model.CreatedDate;
					rfqterm.UpdatedBy = model.UpdatedBy;
					rfqterm.UpdatedDate = model.UpdatedDate;
					rfqterm.DeletedBy = model.DeletedBy;
					rfqterm.DeletedDate = model.DeletedDate;

				}
				obj.RFQTerms.Add(rfqterm);
				obj.SaveChanges();
				status.Sid = termsid;
				return status;
			}
			catch (Exception ex)
			{
				throw;
			}
		}
		public async Task<statuscheckmodel> UpdateRFQTerms(RFQTermsModel model)
		{
			statuscheckmodel status = new statuscheckmodel();
			try
			{
				var remoteterm = new RemoteRfqTerm();
				vscm.Database.Connection.Open();
				var remotedata = vscm.RemoteRfqTerms.Where(x => x.VRfqTermsid == model.RfqTermsid).FirstOrDefault();
				if (model != null)
				{
					remotedata.RfqRevisionId = model.RFQrevisionId;
					remotedata.termsid = (int)model.termsid;
					remotedata.VendorResponse = model.VendorResponse;
					remotedata.TermGroup = model.TermGroup;
					remotedata.Remarks = model.Remarks;
					remotedata.CreatedBy = model.CreatedBy;
					remotedata.CreatedDate = (DateTime)model.CreatedDate;
					remotedata.UpdatedBy = model.UpdatedBy;
					remotedata.UpdatedDate = model.UpdatedDate;
					remotedata.DeletedBy = model.DeletedBy;
					remotedata.DeletedDate = model.DeletedDate;
					vscm.SaveChanges();
				}
				vscm.Database.Connection.Close();

				obj.Database.Connection.Open();
				var localdata = obj.RFQTerms.Where(x => x.RfqTermsid == remotedata.VRfqTermsid).FirstOrDefault();
				if (model != null)
				{
					localdata.RFQrevisionId = model.RFQrevisionId;
					localdata.termsid = model.termsid;
					localdata.VendorResponse = model.VendorResponse;
					localdata.Remarks = model.Remarks;
					localdata.CreatedBy = model.CreatedBy;
					localdata.CreatedDate = model.CreatedDate;
					localdata.UpdatedBy = model.UpdatedBy;
					localdata.UpdatedDate = model.UpdatedDate;
					localdata.DeletedBy = model.DeletedBy;
					localdata.DeletedDate = model.DeletedDate;
					obj.SaveChanges();
				}
				return status;
			}
			catch (Exception ex)
			{
				throw;
			}
		}
		public async Task<YILTermsandConditionModel> GetYILTermsByBuyerGroupID(int id)
		{
			YILTermsandConditionModel model = new YILTermsandConditionModel();
			try
			{
				var data = from x in obj.YILTermsandConditions where x.BuyerGroupId == id select x;
				//var data = obj.YILTermsandConditions.SqlQuery("select * from YILTermsandConditions where BuyerGroupId={0} and DeleteFlag=0", id);
				model = data.Select(x => new YILTermsandConditionModel()
				{
					TermGroupId = x.TermGroupId,
					Terms = x.Terms,
					DisplayOrder = x.DisplayOrder,
					CreatedBy = x.CreatedBy,
					CreatedDate = x.CreatedDate,
					DeletedBy = x.DeletedBy,
					DeletedDate = x.DeletedDate
				}).FirstOrDefault();
				return model;
			}
			catch (Exception ex)
			{
				throw;
			}
		}
		public async Task<YILTermsGroupModel> GetYILTermsGroupById(int id)
		{
			YILTermsGroupModel model = new YILTermsGroupModel();
			try
			{
				var data = from x in obj.YILTermsGroups where x.TermGroupId == id && x.DeleteFlag == false select x;
				//var data = obj.YILTermsGroups.SqlQuery("select * from YILTermsGroup where TermGroupId={0} and DeleteFlag=0", id);
				model = data.Select(x => new YILTermsGroupModel()
				{
					TermGroupId = x.TermGroupId,
					TermGroup = x.TermGroup,
					CreatedBy = x.CreatedBy,
					CreatedDate = x.CreatedDate,
					DeletedBy = x.DeletedBy,
					DeletedDate = x.DeletedDate
				}).FirstOrDefault();
				return model;
			}
			catch (Exception ex)
			{
				throw;
			}
		}
		public async Task<RFQTermsModel> GetRfqTermsById(int termsid)
		{
			RFQTermsModel model = new RFQTermsModel();
			try
			{
				var data = obj.RFQTerms.Where(x => x.RfqTermsid == termsid && x.DeleteFlag == false).FirstOrDefault();
				model.termsid = data.termsid;
				model.UpdatedBy = data.UpdatedBy;
				model.UpdatedDate = data.UpdatedDate;
				model.VendorResponse = data.VendorResponse;
				model.CreatedBy = data.CreatedBy;
				model.CreatedDate = data.CreatedDate;
				model.DeletedBy = data.DeletedBy;
				model.DeletedDate = data.DeletedDate;
				model.Remarks = data.Remarks;
				return model;
			}
			catch (Exception ex)
			{
				throw;
			}
		}
		public async Task<List<V_RFQList>> GetRfqByVendorId(rfqFilterParams rfqfilterparams)
		{
			List<RFQMasterModel> model = new List<RFQMasterModel>();
			List<V_RFQList> data = null;
			try
			{

				int vendorid = Convert.ToInt32(rfqfilterparams.VendorId);

				var query = default(string);
				query = "select * from V_RFQList Where VendorId = " + rfqfilterparams.VendorId + " and (DeleteFlag = 0 or DeleteFlag is null) and VendorVisiblity = 1 and ActiveRevision=1";
				if (rfqfilterparams.typeOfFilter == "true")
				{
					if (!string.IsNullOrEmpty(rfqfilterparams.FromDate))
						query += " and RFQValidDate>='" + rfqfilterparams.FromDate + "'";
					if (!string.IsNullOrEmpty(rfqfilterparams.ToDate))
						query += " and RFQValidDate<='" + rfqfilterparams.ToDate + "'";
					//query += "RFQValidDate <= '" + rfqfilterparams.ToDate + "' and RFQValidDate >= '" + rfqfilterparams.FromDate + "'";
				}
				if (rfqfilterparams.typeOfFilter == "false")
				{
					if (!string.IsNullOrEmpty(rfqfilterparams.FromDate))
						query += "and CreatedDate>='" + rfqfilterparams.FromDate + "'";
					if (!string.IsNullOrEmpty(rfqfilterparams.ToDate))
						query += " and CreatedDate<='" + rfqfilterparams.ToDate + "'";
					//query += "CreatedDate <= '" + rfqfilterparams.ToDate + "' and CreatedDate >= '" + rfqfilterparams.FromDate + "'";
				}
				if (!string.IsNullOrEmpty(rfqfilterparams.RFQNo))
					query += " and RFQNo='" + rfqfilterparams.RFQNo + "'";
				if (rfqfilterparams.StatusId != 0)
					query += " and StatusId='" + rfqfilterparams.StatusId + "'";
				query += " order by MPRRevisionId desc";
				data = vscm.V_RFQList.SqlQuery(query).ToList<V_RFQList>();

				//if ((obj.typeOfFilter == "other"))
				//{
				//	data = new List<V_RFQList>();
				//	data = vscm.V_RFQList.Where(x => x.VendorId == vendorid && x.DeleteFlag == false && x.VendorVisiblity == true).ToList();

				//}
				//else if (obj.typeOfFilter == "true")
				//{
				//	data = vscm.V_RFQList.Where(x => x.VendorId == vendorid && x.DeleteFlag == false && x.VendorVisiblity == false && (x.RFQValidDate >= obj.FromDate && x.RFQValidDate <= obj.ToDate) || x.RFQNo == obj.RFQNo).OrderByDescending(x => x.MPRRevisionId).ToList();
				//}
				//else if (obj.typeOfFilter == "false")
				//{
				//	data = vscm.V_RFQList.Where(x => x.VendorId == vendorid && x.DeleteFlag == false && x.VendorVisiblity == false && (x.CreatedDate >= obj.FromDate && x.CreatedDate <= obj.ToDate) || x.RFQNo == obj.RFQNo).OrderByDescending(x => x.MPRRevisionId).ToList();
				//	//data = vscm.RemoteRFQMasters.Where(x => x.VendorId == vendorid && x.RFQNo == obj.RFQNo && x.DeleteFlag == false && x.VendorVisiblity == true).Include(x => x.RemoteRFQRevisions_N.Select(li => li.CreatedDate >= obj.FromDate && li.CreatedDate <= obj.ToDate)).OrderByDescending(x => x.MPRRevisionId).ToList();
				//}
				//if (data != null)
				//{
				//    model = data.Select(x => new RFQMasterModel()
				//    {
				//        RfqNo = x.RFQNo,
				//        RfqMasterId = x.RfqMasterId,
				//        MPRRevisionId = (int)x.MPRRevisionId,
				//        CreatedBy = x.CreatedBy,
				//        Revision = x.RemoteRFQRevisions_N.Select(y => new RfqRevisionModel()
				//        {
				//            RfqRevisionNo = y.RevisionNo,
				//            RfqRevisionId = y.rfqRevisionId,
				//            RfqValidDate = y.RFQValidDate,
				//            CreatedDate = y.CreatedDate,
				//            CreatedBy = y.CreatedBy,
				//            PackingForwading = y.PackingForwarding,
				//            ExciseDuty = y.ExciseDuty,
				//            salesTax = y.SalesTax,
				//            freight = y.Freight
				//        }).ToList()
				//    }).ToList();



				//    //var revisions = vscm.RemoteRFQRevisions.Where(x => x.rfqMasterId == data.RfqMasterId).ToList();
				//    //RfqRevisionModel revisionmodel = new RfqRevisionModel();
				//    //foreach (var item in revisions)
				//    //{
				//    //    revisionmodel.RfqRevisionId = item.rfqRevisionId;
				//    //    revisionmodel.RfqValidDate = item.RFQValidDate;
				//    //    revisionmodel.RfqRevisionNo = item.RevisionNo;
				//    //    model.Revision.Add(revisionmodel);
				//    //}
				//}

			}
			catch (Exception ex)
			{
				log.ErrorMessage("RFQDA", "GetRfqByVendorId", ex.Message + "; " + ex.StackTrace.ToString());
			}
			return data;
		}



		private static string ToValidFileName(string fileName)
		{
			fileName = fileName.ToLower().Replace(" ", "_").Replace("(", "_").Replace(")", "_").Replace("&", "_").Replace("*", "_").Replace("-", "_");
			return string.Join("_", fileName.Split(Path.GetInvalidFileNameChars()));
		}
		public List<StateList> StateNameList()
		{
			StateList _stateEachItem = null;
			List<StateList> _listItem = new List<StateList>();
			var data = vscm.RemoteStateMasters.Where(o => o.StateId != 0).ToList();
			foreach (var info in data)
			{
				_stateEachItem = new StateList();
				_stateEachItem.StateId = info.StateId;
				_stateEachItem.StateName = info.StateName.Trim();
				_listItem.Add(_stateEachItem);

			}
			return _listItem;

			//throw new NotImplementedException();
		}

		public List<NatureOfBusiness> natureOfBusinessesList()
		{
			NatureOfBusiness _natueofbusinesseachitem = null;
			List<NatureOfBusiness> _listItem = new List<NatureOfBusiness>();
			var data = vscm.RemoteNatureOfBusinessMasters.Where(o => o.Id != 0).ToList();
			foreach (var info in data)
			{
				_natueofbusinesseachitem = new NatureOfBusiness();
				_natueofbusinesseachitem.NatureofbusinessName = info.NatureOfBusinessType;
				_natueofbusinesseachitem.NaturofBusinessId = info.Id;
				_listItem.Add(_natueofbusinesseachitem);

			}
			return _listItem;
			// throw new NotImplementedException();
		}

		public RemoteVendorRegisterMaster SaveVendorDetails(VendorRegistrationModel model)
		{

			int vendorid = 0;
			int bankdetailsid = 0;
			int regId = 0;
			try
			{
				//List<VendorRegistrationModel> RegistrationModelobj = new List<VendorRegistrationModel>();
				//VendorRegistrationModel objid = new VendorRegistrationModel();
				if (model != null)
				{
					//vscm.Database.Connection.Open();
					// var Remotedata = new RemoteVendorRegisterMaster();
					if (model.VendorId == 0)
					{
						var Remotedata = new RemoteVendorRegisterMaster();
						// Remotedata.Id = model.UniqueId;
						//Remotedata.Onetimevendor = model.Onetimevendor;
						//Remotedata.EvaluationRequired = model.EvaluationRequired;
						//Remotedata.PerformanceVerificationRequired = model.PerformanceVerificationRequired;
						Remotedata.MSMERequired = model.MSMERequired;
						//Remotedata.VendorNoInSAP = model.VendorNoInSAP;
						Remotedata.VendorName = model.VendorName;
						Remotedata.Street = model.Street;
						Remotedata.PostalCode = model.PostalCode;
						Remotedata.City = model.City;
						Remotedata.State = model.State;
						Remotedata.StateId = model.StateId;
						Remotedata.LocalBranchOffice = model.LocalBranchOffice;
						Remotedata.BusinessArea = model.BusinessArea;
						Remotedata.PhoneAndExtn = model.PhoneAndExtn;
						Remotedata.Mobile = model.Mobile;
						Remotedata.Email = model.Email;
						Remotedata.AltEmail = model.AltEmail;
						Remotedata.Fax = model.Fax;

						Remotedata.ContactPersonForSales = model.ContactPersonForSales;
						Remotedata.PhoneNumberForSales = model.PhoneNumberForSales;
						Remotedata.EmailIdForSales = model.EmailIdForSales;
						Remotedata.AltEmailidForSales = model.AltEmailidForSales;

						Remotedata.ContactPersonForOperations = model.ContactPersonForOperations;
						Remotedata.PhoneNumberForOperations = model.PhoneNumberForOperations;
						Remotedata.EmailIdForOperations = model.EmailIdForOperations;
						Remotedata.AltEmailidForOperations = model.AltEmailidForOperations;

						Remotedata.ContactPersonForLogistics = model.ContactPersonForLogistics;
						Remotedata.PhoneNumberForLogistics = model.PhoneNumberForLogistics;
						Remotedata.EmailIdForLogistics = model.EmailIdForLogistics;
						Remotedata.AltEmailidForLogistics = model.AltEmailidForLogistics;

						Remotedata.ContactPersonForAccounts = model.ContactPersonForLogistics;
						Remotedata.PhoneNumberForAccounts = model.PhoneNumberForAccounts;
						Remotedata.EmailIdForAccounts = model.EmailIdForAccounts;
						Remotedata.AltEmailidForAccounts = model.AltEmailidForAccounts;

						Remotedata.SwiftCode = model.SwiftCode;
						Remotedata.CurrencyId = model.CurrencyId;
						Remotedata.CurrencyName = model.CurrencyName;
						Remotedata.VendorType = model.VendorType;
						Remotedata.Country = model.Country;

						Remotedata.Phone = model.Phone;
						Remotedata.GSTNo = model.GSTNo;
						Remotedata.PANNo = model.PANNo;
						Remotedata.CINNo = model.CINNo;
						Remotedata.TanNo = model.TanNo;
						Remotedata.NatureofBusiness = model.NatureofBusiness;
						Remotedata.SpecifyNatureOfBusiness = model.SpecifyNatureOfBusiness;
						Remotedata.RequestedOn = DateTime.Now;
						//Remotedata.PaymentTerms = model.PaymentTerms;
						vscm.RemoteVendorRegisterMasters.Add(Remotedata);
						vscm.SaveChanges();
						vendorid = Remotedata.Vendorid;
						regId = Remotedata.Id;
						//objid.UniqueId = Remotedata.Id;
						//RegistrationModelobj.Add(objid);
					}
					else
					{
						RemoteVendorRegisterMaster Remotedata = vscm.RemoteVendorRegisterMasters.Where(li => li.Vendorid == model.VendorId).FirstOrDefault<RemoteVendorRegisterMaster>();
						if (Remotedata != null)
						{
							// Remotedata.Id = model.UniqueId;
							//Remotedata.Onetimevendor = model.Onetimevendor;
							//Remotedata.EvaluationRequired = model.EvaluationRequired;
							//Remotedata.PerformanceVerificationRequired = model.PerformanceVerificationRequired;
							Remotedata.MSMERequired = model.MSMERequired;
							Remotedata.BusinessArea = model.BusinessArea;
							//Remotedata.VendorNoInSAP = model.VendorNoInSAP;
							Remotedata.VendorName = model.VendorName;
							Remotedata.Street = model.Street;
							Remotedata.PostalCode = model.PostalCode;
							Remotedata.City = model.City;
							Remotedata.State = model.State;
							Remotedata.StateId = model.StateId;
							Remotedata.LocalBranchOffice = model.LocalBranchOffice;
							Remotedata.PhoneAndExtn = model.PhoneAndExtn;
							Remotedata.Mobile = model.Mobile;
							Remotedata.Email = model.Email;
							Remotedata.AltEmail = model.AltEmail;
							Remotedata.Fax = model.Fax;

							Remotedata.ContactPersonForSales = model.ContactPersonForSales;
							Remotedata.PhoneNumberForSales = model.PhoneNumberForSales;
							Remotedata.EmailIdForSales = model.EmailIdForSales;
							Remotedata.AltEmailidForSales = model.AltEmailidForSales;

							Remotedata.ContactPersonForOperations = model.ContactPersonForOperations;
							Remotedata.PhoneNumberForOperations = model.PhoneNumberForOperations;
							Remotedata.EmailIdForOperations = model.EmailIdForOperations;
							Remotedata.AltEmailidForOperations = model.AltEmailidForOperations;

							Remotedata.ContactPersonForLogistics = model.ContactPersonForLogistics;
							Remotedata.PhoneNumberForLogistics = model.PhoneNumberForLogistics;
							Remotedata.EmailIdForLogistics = model.EmailIdForLogistics;
							Remotedata.AltEmailidForLogistics = model.AltEmailidForLogistics;

							Remotedata.ContactPersonForAccounts = model.ContactPersonForLogistics;
							Remotedata.PhoneNumberForAccounts = model.PhoneNumberForAccounts;
							Remotedata.EmailIdForAccounts = model.EmailIdForAccounts;
							Remotedata.AltEmailidForAccounts = model.AltEmailidForAccounts;

							Remotedata.SwiftCode = model.SwiftCode;
							Remotedata.CurrencyId = model.CurrencyId;
							Remotedata.CurrencyName = model.CurrencyName;
							Remotedata.VendorType = model.VendorType;
							Remotedata.Country = model.Country;

							Remotedata.Phone = model.Phone;
							Remotedata.GSTNo = model.GSTNo;
							Remotedata.PANNo = model.PANNo;
							Remotedata.CINNo = model.CINNo;
							Remotedata.TanNo = model.TanNo;
							Remotedata.NatureofBusiness = model.NatureofBusiness;
							Remotedata.SpecifyNatureOfBusiness = model.SpecifyNatureOfBusiness;
							Remotedata.RequestedOn = DateTime.Now;
							//Remotedata.PaymentTerms = model.PaymentTerms;
							vscm.SaveChanges();
							vendorid = Remotedata.Vendorid;
							regId = Remotedata.Id;
							//objid.UniqueId = Remotedata.Id;
							//RegistrationModelobj.Add(objid);
						}
					}


					if (vendorid != 0)
					{

						//var remotedataforbankdetails = new RemoteBankDetailsForVendor();
						RemoteBankDetailsForVendor remotedataforbankdetail = vscm.RemoteBankDetailsForVendors.Where(li => li.VendorId == vendorid).FirstOrDefault<RemoteBankDetailsForVendor>();
						if (remotedataforbankdetail == null)
						{
							var remotedataforbankdetails = new RemoteBankDetailsForVendor();
							remotedataforbankdetails.IFSCCode = model.IFSCCode;
							remotedataforbankdetails.IncoTerms = model.IncoTerms;
							remotedataforbankdetails.BankDetails = model.BankDetails;
							remotedataforbankdetails.BankerName = model.BankerName;
							remotedataforbankdetails.AccNo = model.AccNo;
							remotedataforbankdetails.AccountHolderName = model.AccountHolderName;
							remotedataforbankdetails.VendorId = vendorid;
							remotedataforbankdetails.LocationOrBranch = model.LocationOrBranch;
							vscm.RemoteBankDetailsForVendors.Add(remotedataforbankdetails);
							vscm.SaveChanges();
							bankdetailsid = remotedataforbankdetails.Id;
						}
						else
						{
							//var remotedataforbankdetail = new RemoteBankDetailsForVendor();
							remotedataforbankdetail.IFSCCode = model.IFSCCode;
							remotedataforbankdetail.IncoTerms = model.IncoTerms;
							remotedataforbankdetail.BankDetails = model.BankDetails;
							remotedataforbankdetail.BankerName = model.BankerName;
							remotedataforbankdetail.AccNo = model.AccNo;
							remotedataforbankdetail.AccountHolderName = model.AccountHolderName;
							remotedataforbankdetail.VendorId = vendorid;
							remotedataforbankdetail.LocationOrBranch = model.LocationOrBranch;
							// vscm.RemoteBankDetailsForVendors.Add(remotedataforbankdetails);
							vscm.SaveChanges();
							bankdetailsid = remotedataforbankdetail.Id;
						}

					}

					//yscm

					if (model.VendorId == 0)
					{
						var yscmdata = new VendorRegisterMaster();
						yscmdata.Id = regId;
						yscmdata.Onetimevendor = model.Onetimevendor;
						yscmdata.EvaluationRequired = model.EvaluationRequired;
						yscmdata.PerformanceVerificationRequired = model.PerformanceVerificationRequired;
						yscmdata.MSMERequired = model.MSMERequired;
						yscmdata.BusinessArea = model.BusinessArea;
						yscmdata.VendorNoInSAP = model.VendorNoInSAP;
						yscmdata.VendorName = model.VendorName;
						yscmdata.Street = model.Street;
						yscmdata.PostalCode = model.PostalCode;
						yscmdata.City = model.City;
						yscmdata.State = model.State;
						yscmdata.StateId = model.StateId;
						yscmdata.LocalBranchOffice = model.LocalBranchOffice;
						yscmdata.PhoneAndExtn = model.PhoneAndExtn;
						yscmdata.Mobile = model.Mobile;
						yscmdata.Email = model.Email;
						yscmdata.AltEmail = model.AltEmail;
						yscmdata.Fax = model.Fax;
						//yscmdata.ContactPerson = model.ContactPerson;
						//yscmdata.Phone = model.Phone;
						yscmdata.ContactPersonForSales = model.ContactPersonForSales;
						yscmdata.PhoneNumberForSales = model.PhoneNumberForSales;
						yscmdata.EmailIdForSales = model.EmailIdForSales;
						yscmdata.AltEmailidForSales = model.AltEmailidForSales;

						yscmdata.ContactPersonForOperations = model.ContactPersonForOperations;
						yscmdata.PhoneNumberForOperations = model.PhoneNumberForOperations;
						yscmdata.EmailIdForOperations = model.EmailIdForOperations;
						yscmdata.AltEmailidForOperations = model.AltEmailidForOperations;

						yscmdata.ContactPersonForLogistics = model.ContactPersonForLogistics;
						yscmdata.PhoneNumberForLogistics = model.PhoneNumberForLogistics;
						yscmdata.EmailIdForLogistics = model.EmailIdForLogistics;
						yscmdata.AltEmailidForLogistics = model.AltEmailidForLogistics;

						yscmdata.ContactPersonForAccounts = model.ContactPersonForLogistics;
						yscmdata.PhoneNumberForAccounts = model.PhoneNumberForAccounts;
						yscmdata.EmailIdForAccounts = model.EmailIdForAccounts;
						yscmdata.AltEmailidForAccounts = model.AltEmailidForAccounts;

						yscmdata.SwiftCode = model.SwiftCode;
						yscmdata.CurrencyId = model.CurrencyId;
						yscmdata.CurrencyName = model.CurrencyName;
						yscmdata.VendorType = model.VendorType;
						yscmdata.Country = model.Country;

						yscmdata.GSTNo = model.GSTNo;
						yscmdata.PANNo = model.PANNo;
						yscmdata.CINNo = model.CINNo;
						yscmdata.TanNo = model.TanNo;
						yscmdata.NatureofBusiness = model.NatureofBusiness;
						yscmdata.SpecifyNatureOfBusiness = model.SpecifyNatureOfBusiness;
						yscmdata.RequestedOn = DateTime.Now;
						//yscmdata.PaymentTerms = model.PaymentTerms;

						obj.VendorRegisterMasters.Add(yscmdata);
						obj.SaveChanges();
						//yscmid = Remotedata.Id;
						//objid.UniqueId = yscmdata.Id;
						//RegistrationModelobj.Add(objid);
					}
					else
					{
						VendorRegisterMaster yscmdata = obj.VendorRegisterMasters.Where(li => li.Vendorid == model.VendorId).FirstOrDefault<VendorRegisterMaster>();
						yscmdata.Id = regId;
						//yscmdata.Onetimevendor = model.Onetimevendor;
						//yscmdata.EvaluationRequired = model.EvaluationRequired;
						//yscmdata.PerformanceVerificationRequired = model.PerformanceVerificationRequired;
						yscmdata.MSMERequired = model.MSMERequired;
						yscmdata.BusinessArea = model.BusinessArea;
						//yscmdata.VendorNoInSAP = model.VendorNoInSAP;
						yscmdata.VendorName = model.VendorName;
						yscmdata.Street = model.Street;
						yscmdata.PostalCode = model.PostalCode;
						yscmdata.City = model.City;
						yscmdata.State = model.State;
						yscmdata.StateId = model.StateId;
						yscmdata.LocalBranchOffice = model.LocalBranchOffice;
						yscmdata.PhoneAndExtn = model.PhoneAndExtn;
						yscmdata.Mobile = model.Mobile;
						yscmdata.Email = model.Email;
						yscmdata.AltEmail = model.AltEmail;
						yscmdata.Fax = model.Fax;

						yscmdata.ContactPersonForSales = model.ContactPersonForSales;
						yscmdata.PhoneNumberForSales = model.PhoneNumberForSales;
						yscmdata.EmailIdForSales = model.EmailIdForSales;
						yscmdata.AltEmailidForSales = model.AltEmailidForSales;

						yscmdata.ContactPersonForOperations = model.ContactPersonForOperations;
						yscmdata.PhoneNumberForOperations = model.PhoneNumberForOperations;
						yscmdata.EmailIdForOperations = model.EmailIdForOperations;
						yscmdata.AltEmailidForOperations = model.AltEmailidForOperations;

						yscmdata.ContactPersonForLogistics = model.ContactPersonForLogistics;
						yscmdata.PhoneNumberForLogistics = model.PhoneNumberForLogistics;
						yscmdata.EmailIdForLogistics = model.EmailIdForLogistics;
						yscmdata.AltEmailidForLogistics = model.AltEmailidForLogistics;

						yscmdata.ContactPersonForAccounts = model.ContactPersonForLogistics;
						yscmdata.PhoneNumberForAccounts = model.PhoneNumberForAccounts;
						yscmdata.EmailIdForAccounts = model.EmailIdForAccounts;
						yscmdata.AltEmailidForAccounts = model.AltEmailidForAccounts;

						yscmdata.GSTNo = model.GSTNo;
						yscmdata.NatureofBusiness = model.NatureofBusiness;
						yscmdata.SpecifyNatureOfBusiness = model.SpecifyNatureOfBusiness;
						yscmdata.SwiftCode = model.SwiftCode;
						yscmdata.CurrencyId = model.CurrencyId;
						yscmdata.CurrencyName = model.CurrencyName;
						yscmdata.VendorType = model.VendorType;
						yscmdata.Country = model.Country;

						yscmdata.PANNo = model.PANNo;
						yscmdata.CINNo = model.CINNo;
						yscmdata.TanNo = model.TanNo;
						yscmdata.RequestedOn = DateTime.Now;
						//yscmdata.PaymentTerms = model.PaymentTerms;

						obj.SaveChanges();
						//id = yscmdata.Id;
						//objid.UniqueId = yscmdata.Id;
						//RegistrationModelobj.Add(objid);
					}


					if (vendorid != 0)
					{

						//var remotedataforbankdetails = new RemoteBankDetailsForVendor();
						BankDetailsForVendor yscmdataforbankdetail = obj.BankDetailsForVendors.Where(li => li.VendorId == model.VendorId).FirstOrDefault<BankDetailsForVendor>();
						if (yscmdataforbankdetail == null)
						{
							var dataforbankdetails = new BankDetailsForVendor();
							dataforbankdetails.Id = bankdetailsid;
							dataforbankdetails.IFSCCode = model.IFSCCode;
							dataforbankdetails.IncoTerms = model.IncoTerms;
							dataforbankdetails.BankDetails = model.BankDetails;
							dataforbankdetails.BankerName = model.BankerName;
							dataforbankdetails.AccNo = model.AccNo;
							dataforbankdetails.AccountHolderName = model.AccountHolderName;
							dataforbankdetails.VendorId = model.VendorId;
							dataforbankdetails.LocationOrBranch = model.LocationOrBranch;
							obj.BankDetailsForVendors.Add(dataforbankdetails);
							obj.SaveChanges();
						}
						else
						{
							//var remotedataforbankdetail = new RemoteBankDetailsForVendor();
							yscmdataforbankdetail.IFSCCode = model.IFSCCode;
							yscmdataforbankdetail.IncoTerms = model.IncoTerms;
							yscmdataforbankdetail.BankDetails = model.BankDetails;
							yscmdataforbankdetail.BankerName = model.BankerName;
							yscmdataforbankdetail.AccNo = model.AccNo;
							yscmdataforbankdetail.AccountHolderName = model.AccountHolderName;
							yscmdataforbankdetail.VendorId = vendorid;
							yscmdataforbankdetail.LocationOrBranch = model.LocationOrBranch;
							// vscm.RemoteBankDetailsForVendors.Add(remotedataforbankdetails);
							obj.SaveChanges();
						}

						this.InsertVendorDocuments(model.DocDetailsLists);
						//foreach (var item in model.DocDetailsLists)
						//{
						//	RemoteVendorRegisterDocumentDetail remotedataforDocumentDetails = vscm.RemoteVendorRegisterDocumentDetails.Where(li => li.VendorId == vendorid && li.Id == item.Id).FirstOrDefault<RemoteVendorRegisterDocumentDetail>();
						//	if (remotedataforDocumentDetails != null)
						//	{
						//		remotedataforDocumentDetails.UploadedBy = item.UploadedBy;
						//		remotedataforDocumentDetails.Status = true;
						//		vscm.SaveChanges();
						//	}

						//}
						//var DataforDocumentDetailsRemote = vscm.RemoteVendorRegisterDocumentDetails.Where(li => li.VendorId == vendorid && li.Deleteflag == false).ToList();
						//foreach (var data in DataforDocumentDetailsRemote)
						//{
						//	var rfqredDocLocal = obj.VendorRegisterDocumentDetails.Where(li => li.Id == data.Id).FirstOrDefault();
						//	if (rfqredDocLocal != null)
						//	{
						//		rfqredDocLocal.UploadedBy = data.UploadedBy;
						//		rfqredDocLocal.Status = true;
						//		obj.SaveChanges();

						//	}
						//}
					}
					//need to send mail to buyer manager(requestor)
					this.emailTemplateDA.sendMailtoBuyer(model.VendorId);

				}

			}
			catch (Exception ex)
			{
				log.ErrorMessage("RFQDA", "SaveVendorDetails", ex.Message + "; " + ex.StackTrace.ToString());
			}
			return vscm.RemoteVendorRegisterMasters.Where(li => li.Vendorid == vendorid).FirstOrDefault();
		}

		public void InsertVendorDocuments(List<RemoteVendorRegisterDocumentDetail> model)
		{
			int vendorid = Convert.ToInt32(model[0].VendorId);
			List<documentDetails> Listobj = new List<documentDetails>();
			var eachobj = new documentDetails();
			try
			{

				if (model != null)
				{

					foreach (var item in model)
					{

						RemoteVendorRegisterDocumentDetail remotedataforDocumentDetails = vscm.RemoteVendorRegisterDocumentDetails.Where(li => li.VendorId == vendorid && li.Id == item.Id).FirstOrDefault<RemoteVendorRegisterDocumentDetail>();
						if (remotedataforDocumentDetails == null)
						{
							var remotedataforDocumentDetail = new RemoteVendorRegisterDocumentDetail();
							remotedataforDocumentDetail.VendorId = item.VendorId;
							remotedataforDocumentDetail.DocumentName = item.DocumentName;
							remotedataforDocumentDetail.PhysicalPath = item.PhysicalPath;
							remotedataforDocumentDetail.UploadedBy = item.UploadedBy;
							remotedataforDocumentDetail.UploadedOn = DateTime.Now;
							//remotedataforDocumentDetail.Status = false;
							remotedataforDocumentDetail.Deleteflag = false;
							remotedataforDocumentDetail.DocumentationTypeId = item.DocumentationTypeId;
							vscm.RemoteVendorRegisterDocumentDetails.Add(remotedataforDocumentDetail);
							vscm.SaveChanges();
						}
						else
						{
							remotedataforDocumentDetails.VendorId = item.VendorId;
							remotedataforDocumentDetails.DocumentName = item.DocumentName;
							remotedataforDocumentDetails.PhysicalPath = item.PhysicalPath;
							remotedataforDocumentDetails.UploadedBy = item.UploadedBy;
							remotedataforDocumentDetails.UploadedOn = DateTime.Now;
							//remotedataforDocumentDetails.Status = false;
							remotedataforDocumentDetails.Deleteflag = false;
							remotedataforDocumentDetails.DocumentationTypeId = item.DocumentationTypeId;
							vscm.SaveChanges();
						}

					}
					var DataforDocumentDetailsRemote = vscm.RemoteVendorRegisterDocumentDetails.Where(li => li.VendorId == vendorid).ToList();
					foreach (var data in DataforDocumentDetailsRemote)
					{
						var rfqredDocLocal = obj.VendorRegisterDocumentDetails.Where(li => li.Id == data.Id).FirstOrDefault();
						if (rfqredDocLocal == null)
						{
							var localdataforDocumentDetail = new VendorRegisterDocumentDetail();
							localdataforDocumentDetail.Id = data.Id;
							localdataforDocumentDetail.VendorId = vendorid;
							localdataforDocumentDetail.DocumentName = data.DocumentName;
							localdataforDocumentDetail.PhysicalPath = data.PhysicalPath;
							localdataforDocumentDetail.UploadedBy = data.UploadedBy;
							localdataforDocumentDetail.UploadedOn = data.UploadedOn;
							//localdataforDocumentDetail.Status = data.Status;
							localdataforDocumentDetail.Deleteflag = data.Deleteflag;
							localdataforDocumentDetail.DocumentationTypeId = data.DocumentationTypeId;
							obj.VendorRegisterDocumentDetails.Add(localdataforDocumentDetail);
							obj.SaveChanges();
						}
						else
						{
							rfqredDocLocal.VendorId = vendorid;
							rfqredDocLocal.DocumentName = data.DocumentName;
							rfqredDocLocal.PhysicalPath = data.PhysicalPath;
							rfqredDocLocal.UploadedBy = data.UploadedBy;
							rfqredDocLocal.UploadedOn = data.UploadedOn;
							//rfqredDocLocal.Status = data.Status;
							rfqredDocLocal.Deleteflag = data.Deleteflag;
							rfqredDocLocal.DocumentationTypeId = data.DocumentationTypeId;
							obj.SaveChanges();

						}

					}
				}
			}
			catch (Exception e)
			{

			}
			//return vscm.RemoteVendorRegisterDocumentDetails.Where(li => li.VendorId == vendorid && li.Deleteflag == false).ToList();
		}

		public List<DocumentType> DocumentMasterList()
		{
			DocumentType _Documenteachitem = null;
			List<DocumentType> _listItem = new List<DocumentType>();
			var data = vscm.RemoteVendorRegisterDocumenetMasters.Where(o => o.DocumentationTypeId != 0).ToList();
			foreach (var info in data)
			{
				_Documenteachitem = new DocumentType();
				_Documenteachitem.DocumentName = info.Documentype;
				_Documenteachitem.DocId = info.DocumentationTypeId;
				_listItem.Add(_Documenteachitem);

			}
			return _listItem;

		}

		public List<RemoteVendorRegisterDocumentDetail> InsertDocuments(List<documentDetails> model)
		{
			int vendorid = Convert.ToInt32(model[0].VendorId);
			List<documentDetails> Listobj = new List<documentDetails>();
			var eachobj = new documentDetails();
			try
			{

				if (model != null)
				{

					foreach (var item in model)
					{

						RemoteVendorRegisterDocumentDetail remotedataforDocumentDetails = vscm.RemoteVendorRegisterDocumentDetails.Where(li => li.VendorId == vendorid && li.Id == item.Id).FirstOrDefault<RemoteVendorRegisterDocumentDetail>();
						if (remotedataforDocumentDetails == null)
						{
							var remotedataforDocumentDetail = new RemoteVendorRegisterDocumentDetail();
							remotedataforDocumentDetail.VendorId = item.VendorId;
							remotedataforDocumentDetail.DocumentName = item.DocumentName;
							remotedataforDocumentDetail.PhysicalPath = item.PhysicalPath;
							remotedataforDocumentDetail.UploadedBy = item.UploadedBy;
							remotedataforDocumentDetail.UploadedOn = DateTime.Now;
							//remotedataforDocumentDetail.Status = false;
							remotedataforDocumentDetail.Deleteflag = false;
							remotedataforDocumentDetail.DocumentationTypeId = item.DocumentationTypeId;
							vscm.RemoteVendorRegisterDocumentDetails.Add(remotedataforDocumentDetail);
							vscm.SaveChanges();
						}
						else
						{
							remotedataforDocumentDetails.VendorId = item.VendorId;
							remotedataforDocumentDetails.DocumentName = item.DocumentName;
							remotedataforDocumentDetails.PhysicalPath = item.PhysicalPath;
							remotedataforDocumentDetails.UploadedBy = item.UploadedBy;
							remotedataforDocumentDetails.UploadedOn = DateTime.Now;
							//remotedataforDocumentDetails.Status = false;
							remotedataforDocumentDetails.Deleteflag = false;
							remotedataforDocumentDetails.DocumentationTypeId = item.DocumentationTypeId;
							vscm.SaveChanges();
						}

					}
					var DataforDocumentDetailsRemote = vscm.RemoteVendorRegisterDocumentDetails.Where(li => li.VendorId == vendorid).ToList();
					foreach (var data in DataforDocumentDetailsRemote)
					{
						var rfqredDocLocal = obj.VendorRegisterDocumentDetails.Where(li => li.Id == data.Id).FirstOrDefault();
						if (rfqredDocLocal == null)
						{
							var localdataforDocumentDetail = new VendorRegisterDocumentDetail();
							localdataforDocumentDetail.Id = data.Id;
							localdataforDocumentDetail.VendorId = vendorid;
							localdataforDocumentDetail.DocumentName = data.DocumentName;
							localdataforDocumentDetail.PhysicalPath = data.PhysicalPath;
							localdataforDocumentDetail.UploadedBy = data.UploadedBy;
							localdataforDocumentDetail.UploadedOn = data.UploadedOn;
							//localdataforDocumentDetail.Status = data.Status;
							localdataforDocumentDetail.Deleteflag = data.Deleteflag;
							localdataforDocumentDetail.DocumentationTypeId = data.DocumentationTypeId;
							obj.VendorRegisterDocumentDetails.Add(localdataforDocumentDetail);
							obj.SaveChanges();
						}
						else
						{
							rfqredDocLocal.VendorId = vendorid;
							rfqredDocLocal.DocumentName = data.DocumentName;
							rfqredDocLocal.PhysicalPath = data.PhysicalPath;
							rfqredDocLocal.UploadedBy = data.UploadedBy;
							rfqredDocLocal.UploadedOn = data.UploadedOn;
							//rfqredDocLocal.Status = data.Status;
							rfqredDocLocal.Deleteflag = data.Deleteflag;
							rfqredDocLocal.DocumentationTypeId = data.DocumentationTypeId;
							obj.SaveChanges();

						}

					}
				}
			}
			catch (Exception ex)
			{
				log.ErrorMessage("RFQDA", "InsertDocuments", ex.Message + "; " + ex.StackTrace.ToString());
			}
			return vscm.RemoteVendorRegisterDocumentDetails.Where(li => li.VendorId == vendorid && li.Deleteflag == false).ToList();
		}

		public bool DeletefileAttached(documentDetails model)
		{
			Boolean deletestatus = false;
			try
			{
				RemoteVendorRegisterDocumentDetail remotedatafordelete = vscm.RemoteVendorRegisterDocumentDetails.Where(li => li.VendorId == model.VendorId && li.Id == model.Id).FirstOrDefault<RemoteVendorRegisterDocumentDetail>();
				if (remotedatafordelete != null)
				{
					remotedatafordelete.Deleteflag = true;
					vscm.SaveChanges();
					deletestatus = true;
				}
				else
				{
					deletestatus = false;
				}
				using (YSCMEntities Context = new YSCMEntities())
				{
					VendorRegisterDocumentDetail deptDelete = Context.VendorRegisterDocumentDetails.Where(li => li.VendorId == model.VendorId && li.Id == model.Id).FirstOrDefault();
					if (deptDelete != null)
					{
						deptDelete.Deleteflag = true;
						//Context.MPRDocuments.Remove(deptDelete);
						Context.SaveChanges();
					}
				}
			}
			catch (Exception ex)
			{
				log.ErrorMessage("RFQDA", "DeletefileAttached", ex.Message + "; " + ex.StackTrace.ToString());
			}
			return true;
		}

		public bool DeletefileAttachedforDocuments(RFQDocument model)
		{
			Boolean deletestatus = false;
			try
			{
				RemoteRFQDocument remotedatafordelete = vscm.RemoteRFQDocuments.Where(li => li.RfqDocId == model.RfqDocId).FirstOrDefault();
				if (remotedatafordelete != null)
				{
					remotedatafordelete.DeleteFlag = true;
					vscm.SaveChanges();
					deletestatus = true;
				}
				else
				{
					deletestatus = false;
				}

				RFQDocument remotedatafordeleteYSCM = obj.RFQDocuments.Where(li => li.RfqDocId == model.RfqDocId).FirstOrDefault();
				if (remotedatafordeleteYSCM != null)
				{
					remotedatafordeleteYSCM.DeleteFlag = true;
					obj.SaveChanges();
					deletestatus = true;
				}
				else
				{
					deletestatus = false;
				}
			}
			catch (Exception ex)
			{
				log.ErrorMessage("RFQDA", "DeletefileAttachedforDocuments", ex.Message + "; " + ex.StackTrace.ToString());
			}
			return deletestatus;
		}

		public List<RemoteRfqTerm> GetTermMaster(int rfqrevisionId)
		{
			return vscm.RemoteRfqTerms.Where(o => o.DeleteFlag != true && o.RfqRevisionId == rfqrevisionId).ToList();

		}

		public List<RemoteRfqTerm> UpdateVendorTerms(List<RemoteRfqTerm> model)
		{
			int rfqrevisionId = model[0].RfqRevisionId;
			foreach (var item in model)
			{
				RemoteRfqTerm remotedataforupdateterm = vscm.RemoteRfqTerms.Where(li => li.VRfqTermsid == item.VRfqTermsid).FirstOrDefault();
				if (remotedataforupdateterm != null)
				{
					remotedataforupdateterm.VendorResponse = item.VendorResponse;
					remotedataforupdateterm.Remarks = item.Remarks;
					vscm.SaveChanges();
				}
				RFQTerm terms = obj.RFQTerms.Where(li => li.RfqTermsid == item.VRfqTermsid).FirstOrDefault();
				if (terms != null)
				{
					terms.VendorResponse = item.VendorResponse;
					terms.Remarks = item.Remarks;
					obj.SaveChanges();
				}
			}
			return vscm.RemoteRfqTerms.Where(o => o.DeleteFlag != true && o.RfqRevisionId == rfqrevisionId).ToList();
		}


		public List<RemoteDocumentTypeMaster> GetMasterDocumentTypeList()
		{

			DocumentTypeMaster _Documenteachitem = null;
			List<DocumentTypeMaster> _listItem = new List<DocumentTypeMaster>();
			return vscm.RemoteDocumentTypeMasters.Where(o => o.IsActive == true).ToList();

		}

		public List<RemoteRFQCommunication> UpdateVendorCommunication(VendorCommunicaton model)
		{
			string msg = "";
			int RfqMasterId = vscm.RemoteRFQRevisions_N.Where(li => li.rfqRevisionId == model.RfqRevisionId).FirstOrDefault().rfqMasterId;

			try
			{
				if (model != null)
				{

					int rfqccid = 0;
					var remotedataforvendorcomm = new RemoteRFQCommunication();
					remotedataforvendorcomm.Remarks = model.Remarks;
					remotedataforvendorcomm.DeleteFlag = false;
					remotedataforvendorcomm.RemarksDate = System.DateTime.Now;
					remotedataforvendorcomm.RfqItemsId = model.RfqItemsId;
					remotedataforvendorcomm.RfqRevisionId = model.RfqRevisionId;
					remotedataforvendorcomm.RfqMasterId = RfqMasterId;
					remotedataforvendorcomm.RemarksFrom = model.RemarksFrom;
					vscm.RemoteRFQCommunications.Add(remotedataforvendorcomm);
					vscm.SaveChanges();
					rfqccid = remotedataforvendorcomm.RfqCCid;
					var remotedataforvendorcommyscm = new RFQCommunication();
					remotedataforvendorcommyscm.Remarks = model.Remarks;
					remotedataforvendorcommyscm.DeleteFlag = false;
					remotedataforvendorcommyscm.RemarksDate = System.DateTime.Now;
					remotedataforvendorcommyscm.RfqItemsId = model.RfqItemsId;
					remotedataforvendorcommyscm.RfqRevisionId = model.RfqRevisionId;
					remotedataforvendorcommyscm.RfqMasterId = RfqMasterId;
					remotedataforvendorcommyscm.RemarksFrom = model.RemarksFrom;
					remotedataforvendorcommyscm.RfqCCid = rfqccid;
					obj.RFQCommunications.Add(remotedataforvendorcommyscm);
					obj.SaveChanges();
					msg = "OK";
					emailTemplateDA.sendCommunicationmailtoRequestor(model.RfqRevisionId, model.Remarks);



				}
			}
			catch (Exception ex)
			{
				log.ErrorMessage("RFQDA", "UpdateVendorCommunication", ex.Message + "; " + ex.StackTrace.ToString());
			}

			List<RemoteRFQCommunication> RemoteRFQCommunications = vscm.RemoteRFQCommunications.Where(li => li.RfqMasterId == RfqMasterId).ToList();
			foreach (RemoteRFQCommunication item in RemoteRFQCommunications)
			{
				item.Employee = obj.VendorEmployeeViews.Where(li => li.EmployeeNo == item.RemarksFrom).FirstOrDefault();
			}
			return RemoteRFQCommunications;

			// throw new NotImplementedException();
		}

		public List<VendorCommunicaton> GetVendorCommunicationForRFQRevId(int RFQRevisionId)
		{
			List<VendorCommunicaton> listobj = new List<VendorCommunicaton>();
			VendorCommunicaton eachobj = null;
			try
			{
				var rfqcommdata = vscm.RemoteRFQCommunications.Where(x => x.RfqRevisionId == RFQRevisionId && x.DeleteFlag != true).ToList();
				foreach (var item in rfqcommdata)
				{
					eachobj = new VendorCommunicaton();
					eachobj.Remarks = item.Remarks;
					eachobj.RemarksFrom = item.RemarksFrom;
					eachobj.RemarksDate = item.RemarksDate;
					listobj.Add(eachobj);
				}
			}
			catch (Exception e)
			{
				throw;
			}
			return listobj;
		}

		public List<RFQTerms> GetTermsByRfqRevisionId(int RfqRevisionId)
		{
			var terms = vscm.RemoteRfqTerms.Where(x => x.RfqRevisionId == RfqRevisionId && x.DeleteFlag == false).ToList<RemoteRfqTerm>();
			List<RFQTerms> listobj = new List<RFQTerms>();
			RFQTerms eachobj = null;
			if (terms != null)
			{

				foreach (var item in terms)
				{
					eachobj = new RFQTerms();
					eachobj.Terms = item.Terms;
					eachobj.VendorResponse = item.VendorResponse;
					eachobj.VRfqTermsid = item.VRfqTermsid;
					eachobj.remarks = item.Remarks;
					listobj.Add(eachobj);
				}
			}
			return listobj;
		}

		public List<RemoteRFQDocument> GetRfqdocumentdetailsById(int revisionId)
		{
			List<RemoteRFQDocument> _listobj = new List<RemoteRFQDocument>();
			_listobj = vscm.RemoteRFQDocuments.Where(li => li.rfqRevisionId == revisionId && li.DocumentType != 1 && li.DeleteFlag == false).ToList();
			return _listobj;
		}

		public List<RemoteRFQDocument> GetRfqdocumentdetailsById(int revisionId, int rfqitemId)
		{
			List<RemoteRFQItems_N> _listobjs = new List<RemoteRFQItems_N>();
			List<RemoteRFQDocument> _listobj = new List<RemoteRFQDocument>();
			List<RemoteRFQDocument> _eachobj = null;
			_listobjs = vscm.RemoteRFQItems_N.Include(x => x.RemoteRFQDocuments).Where(li => li.RFQRevisionId == revisionId).ToList();
			foreach (var item in _listobjs)
			{
				_eachobj = new List<RemoteRFQDocument>();
				_eachobj = vscm.RemoteRFQDocuments.Where(li => li.rfqItemsid == item.RFQItemsId && li.rfqRevisionId == revisionId && li.DocumentType.Equals(1) && li.DeleteFlag.Equals(false)).ToList();
				_listobj.AddRange(_eachobj);

			}
			return _listobj;
		}

		List<VendorRegistrationModel> IRFQDA.InsertVendordata(VendorRegistrationModel obj)
		{
			throw new NotImplementedException();
		}

		public VendorRegistrationModel GetVendorDetails(int vendorId)
		{
			VendorRegistrationModel listobj = new VendorRegistrationModel();
			try
			{
				RemoteVendorRegisterMaster getdata = vscm.RemoteVendorRegisterMasters.Where(li => li.Vendorid == vendorId).FirstOrDefault();
				if (getdata != null)
				{
					listobj.UniqueId = getdata.Id;
					listobj.VendorId = getdata.Vendorid;
					listobj.Onetimevendor = Convert.ToBoolean(getdata.Onetimevendor);
					listobj.MSMERequired = Convert.ToBoolean(getdata.MSMERequired);
					listobj.PerformanceVerificationRequired = Convert.ToBoolean(getdata.PerformanceVerificationRequired);
					listobj.EvaluationRequired = Convert.ToBoolean(getdata.EvaluationRequired);
					listobj.BusinessArea = getdata.BusinessArea;
					listobj.VendorNoInSAP = getdata.VendorNoInSAP;
					listobj.VendorName = getdata.VendorName;
					listobj.Street = getdata.Street;
					listobj.City = getdata.City;
					listobj.PostalCode = getdata.PostalCode;
					listobj.StateId = Convert.ToInt32(getdata.StateId);
					listobj.State = getdata.State;

					listobj.PhoneAndExtn = getdata.PhoneAndExtn;
					listobj.LocalBranchOffice = getdata.LocalBranchOffice;
					listobj.Mobile = getdata.Mobile;
					listobj.Email = getdata.Email;
					listobj.AltEmail = getdata.AltEmail;
					listobj.Fax = getdata.Fax;
					listobj.ContactPerson = getdata.ContactPerson;
					listobj.Phone = getdata.Phone;
					listobj.ContactPersonForSales = getdata.ContactPersonForSales;
					listobj.PhoneNumberForSales = getdata.PhoneNumberForSales;
					listobj.EmailIdForSales = getdata.EmailIdForSales;
					listobj.AltEmailidForSales = getdata.AltEmailidForSales;

					listobj.ContactPersonForOperations = getdata.ContactPersonForOperations;
					listobj.PhoneNumberForOperations = getdata.PhoneNumberForOperations;
					listobj.EmailIdForOperations = getdata.EmailIdForOperations;
					listobj.AltEmailidForOperations = getdata.AltEmailidForOperations;

					listobj.ContactPersonForLogistics = getdata.ContactPersonForLogistics;
					listobj.PhoneNumberForLogistics = getdata.PhoneNumberForLogistics;
					listobj.EmailIdForLogistics = getdata.EmailIdForLogistics;
					listobj.AltEmailidForLogistics = getdata.AltEmailidForLogistics;

					listobj.ContactPersonForAccounts = getdata.ContactPersonForLogistics;
					listobj.PhoneNumberForAccounts = getdata.PhoneNumberForAccounts;
					listobj.EmailIdForAccounts = getdata.EmailIdForAccounts;
					listobj.AltEmailidForAccounts = getdata.AltEmailidForAccounts;
					listobj.GSTNo = getdata.GSTNo;
					listobj.NatureofBusiness = getdata.NatureofBusiness;
					listobj.SpecifyNatureOfBusiness = getdata.SpecifyNatureOfBusiness;
					listobj.PANNo = getdata.PANNo;
					listobj.CINNo = getdata.CINNo;
					listobj.TanNo = getdata.TanNo;
					listobj.PaymentTerms = getdata.PaymentTerms;
					listobj.SwiftCode = getdata.SwiftCode;
					listobj.CurrencyId = Convert.ToInt32(getdata.CurrencyId);
					listobj.CurrencyName = getdata.CurrencyName;
					listobj.VendorType = getdata.VendorType;
					listobj.Country = getdata.Country;

					RemoteBankDetailsForVendor bankData = vscm.RemoteBankDetailsForVendors.Where(li => li.VendorId == vendorId).FirstOrDefault();
					if (bankData != null)
					{
						listobj.BankDetails = bankData.BankDetails;
						listobj.BankerName = bankData.BankerName;
						listobj.IFSCCode = bankData.IFSCCode;
						listobj.IncoTerms = bankData.IncoTerms;
						listobj.AccountHolderName = bankData.AccountHolderName;
						listobj.LocationOrBranch = bankData.LocationOrBranch;
						listobj.AccNo = bankData.AccNo;
					}

				}
				listobj.DocDetailsLists = vscm.RemoteVendorRegisterDocumentDetails.Where(li => li.VendorId == vendorId && li.Deleteflag == false).ToList();
			}
			catch (Exception ex)
			{
				log.ErrorMessage("RFQDA", "GetVendorDetails", ex.Message + "; " + ex.StackTrace.ToString());
			}

			return listobj;
			// throw new NotImplementedException();
		}

		public string changepassword(Changepassword objs)
		{
			string msg = string.Empty;
			if (objs != null)
			{
				RemoteVendorUserMaster remotechnagepwdobj = vscm.RemoteVendorUserMasters.Where(li => li.VendorId == objs.VendorId && li.pwd == objs.CurrentPassword).FirstOrDefault();
				if (remotechnagepwdobj.pwd == objs.CurrentPassword)
				{
					remotechnagepwdobj.pwd = objs.NewPassword;
					vscm.SaveChanges();
					msg = "OK";
				}
				else
				{
					msg = "error";
				}

				VendorUserMaster chnagepwdobj = obj.VendorUserMasters.Where(li => li.VendorId == objs.VendorId && li.pwd == objs.CurrentPassword).FirstOrDefault();
				if (chnagepwdobj.pwd == objs.CurrentPassword)
				{
					chnagepwdobj.pwd = objs.NewPassword;
					obj.SaveChanges();
					msg = "OK";
				}
			}
			return msg;
			//throw new NotImplementedException();
		}

		public bool sendVendormail(int RFQRevisionId)
		{
			try
			{

				this.emailTemplateDA.sendQuotemailtoRequestor(RFQRevisionId);
			}
			catch (Exception ex)
			{
				throw ex;
			}
			return true;
		}
		//public bool sendEmail(EmailSend emlSndngList)
		//{
		//	emlSndngList.BCC = ConfigurationManager.AppSettings["BCC"];
		//	var SMTPServer = ConfigurationManager.AppSettings["SMTPServer"];
		//	// string smtp= ConfigurationManager.AppSettings["AttachedDocPath"];
		//	MailMessage mailMessage = new MailMessage(emlSndngList.FrmEmailId, emlSndngList.ToEmailId);
		//	SmtpClient client = new SmtpClient();
		//	if (!string.IsNullOrEmpty(emlSndngList.Subject))
		//		mailMessage.Subject = emlSndngList.Subject;
		//	if (!string.IsNullOrEmpty(emlSndngList.CC))
		//		mailMessage.CC.Add(emlSndngList.CC);
		//	if (!string.IsNullOrEmpty(emlSndngList.BCC))
		//		mailMessage.Bcc.Add(emlSndngList.BCC);
		//	mailMessage.Body = emlSndngList.Body;
		//	mailMessage.IsBodyHtml = true;
		//	mailMessage.BodyEncoding = Encoding.UTF8;
		//	//SmtpClient mailClient = new SmtpClient("localhost", 25);
		//	SmtpClient mailClient = new SmtpClient(SMTPServer, 25);
		//	//mailClient.EnableSsl = false;
		//	mailClient.DeliveryMethod = SmtpDeliveryMethod.Network;
		//	mailClient.Send(mailMessage);

		//	//MailMessage mailMessage = new MailMessage(emlSndngList.FrmEmailId, emlSndngList.ToEmailId);
		//	//SmtpClient client = new SmtpClient();
		//	//if (!string.IsNullOrEmpty(emlSndngList.Subject))
		//	//    mailMessage.Subject = emlSndngList.Subject;
		//	//if (!string.IsNullOrEmpty(emlSndngList.CC))
		//	//    mailMessage.CC.Add(emlSndngList.CC);
		//	//mailMessage.Body = emlSndngList.Body;
		//	//mailMessage.IsBodyHtml = true;
		//	//mailMessage.BodyEncoding = Encoding.UTF8;
		//	//SmtpClient mailClient = new SmtpClient("10.29.15.9", 25);
		//	//mailClient.EnableSsl = false;
		//	////mailClient.DeliveryMethod = SmtpDeliveryMethod.Network;
		//	//mailClient.Send(mailMessage);
		//	return true;
		//}

		public async Task<statuscheckmodel> DeleteRfqIteminfoByidformultiple(int id, int BOMid)
		{
			statuscheckmodel status = new statuscheckmodel();
			try
			{
				double? totalprice = 0;
				//vscm.Database.Connection.Open();
				var Remotedata = vscm.RemoteRFQItemsInfo_N.Where(x => x.RFQSplitItemId == id && x.DeleteFlag == false).FirstOrDefault();
				var remotebomdata = vscm.RemoteRfqVendorBOMs.Where(x => x.RFQVendorbomItemId == BOMid && x.DeleteFlag == false).FirstOrDefault();
				var bomdata = obj.RfqVendorBOMs.Where(x => x.RFQVendorbomItemId == BOMid && x.DeleteFlag == false).FirstOrDefault();
				//if (Remotedata != null)
				//{

				//    Remotedata.DeleteFlag = true;
				//    vscm.SaveChanges();
				//}
				if (remotebomdata != null)
				{

					remotebomdata.DeleteFlag = true;
					vscm.SaveChanges();

				}
				if (bomdata != null)
				{

					bomdata.DeleteFlag = true;
					obj.SaveChanges();

				}
				List<RemoteRfqVendorBOM> itemsforupdate = vscm.RemoteRfqVendorBOMs.Where(li => li.RfqItemsId == id && li.DeleteFlag == false).ToList<RemoteRfqVendorBOM>();

				foreach (var data1 in itemsforupdate)
				{
					double? subtotalprice = 0;
					subtotalprice = Convert.ToDouble(data1.Qty) * Convert.ToDouble(data1.UnitPrice);
					if (Convert.ToInt32(data1.Discount) != 0)
					{
						double? Discount = subtotalprice - Convert.ToDouble(data1.Discount);
						subtotalprice = Discount;
					}
					if (Convert.ToInt32(data1.DiscountPercentage) != 0)
					{
						double? DiscountPercentage = (subtotalprice - subtotalprice * Convert.ToDouble(data1.DiscountPercentage) / 100);
						subtotalprice = DiscountPercentage;
					}
					if (Convert.ToInt32(data1.FreightAmount) != 0)
					{
						double? FreightAmount = subtotalprice - Convert.ToDouble(data1.FreightAmount);
						subtotalprice = FreightAmount;
					}
					if (Convert.ToInt32(data1.FreightPercentage) != 0)
					{
						double? FreightPercentage = (subtotalprice - subtotalprice * Convert.ToDouble(data1.FreightPercentage) / 100);
						subtotalprice = FreightPercentage;
					}
					if (Convert.ToInt32(data1.PFAmount) != 0)
					{
						double? PFAmount = subtotalprice - Convert.ToDouble(data1.PFAmount);
						subtotalprice = PFAmount;
					}
					if (Convert.ToInt32(data1.PFPercentage) != 0)
					{

						double? PFPercentage = (subtotalprice * Convert.ToDouble(data1.PFPercentage) / 100);
						subtotalprice += PFPercentage;
					}
					totalprice += subtotalprice;
				}
				RemoteRFQItemsInfo_N itemsinfo = vscm.RemoteRFQItemsInfo_N.Where(li => li.RFQItemsId == id).FirstOrDefault();
				if (itemsinfo != null)
				{
					itemsinfo.UnitPrice = Convert.ToDecimal(totalprice);
					vscm.SaveChanges();
				}

				// vscm.Database.Connection.Close();

				//obj.Database.Connection.Open();
				var Localdata = obj.RFQItemsInfo_N.Where(x => x.RFQSplitItemId == id && x.DeleteFlag == false).FirstOrDefault();
				if (Localdata != null)
				{
					Localdata.DeleteFlag = true;
					Localdata.UnitPrice = Convert.ToDecimal(totalprice);
					obj.SaveChanges();
				}
				else
				{

				}
				//if (Remotedata == null || Localdata==null)
				//{
				//    status.Sid = 0;
				//}
				//status.Sid = Localdata.RFQItemsId;
				return status;
			}
			catch (Exception ex)
			{
				throw;
			}
		}

		public bool checkemail(Changepassword emailId)
		{
			bool exists = false;
			RemoteVendorUserMaster emailidexists = vscm.RemoteVendorUserMasters.Where(li => li.Vuserid == emailId.emailid && li.Active == true).FirstOrDefault();
			if (emailidexists != null)
			{
				exists = true;
			}
			else
			{
				exists = false;
			}
			return exists;
		}

		public bool sendLinkForForgetPassword(forgetpassword model)
		{
			try
			{
				string token = Guid.NewGuid().ToString();
				// DateTime expirydateandtime = System.DateTime.Now.AddHours(24);
				forgetpassword passwordobj = new forgetpassword();
				passwordobj.emailid = model.emailid;
				passwordobj.Expirydatetime = System.DateTime.Now.AddMinutes(Convert.ToInt32(model.tokenduration));
				passwordobj.URLCreatedOn = System.DateTime.Now;
				passwordobj.URL = model.baseURL + model.emailid + "/" + token;
				var obj = new RemoteForgetPassword();
				obj.Token = token;
				obj.emailId = model.emailid;
				obj.ExpirtyDateAndTime = System.DateTime.Now.AddMinutes(Convert.ToInt32(model.tokenduration));
				obj.URLGeneratedOn = System.DateTime.Now;
				obj.ResetURL = passwordobj.URL;
				obj.TokenUsed = false;
				vscm.RemoteForgetPasswords.Add(obj);
				vscm.SaveChanges();
				EmailSend emailobj = new EmailSend();
				emailobj.Subject = "Reset password";
				// emailobj.Body = "Please click the  link to reset the password  " + obj.ResetURL;
				emailobj.Body = "<html><meta charset=\"ISO-8859-1\"><head><link rel ='stylesheet' href ='https://maxcdn.bootstrapcdn.com/bootstrap/4.3.1/css/bootstrap.min.css'></head><body><div class='container'><p>Dear User,</p><p>Please use the following URL to  reset the password in Vendor Portal.</p><p style = 'font-weight:600;text-decoration:underline'> LINK </p><p id= 'linkid'>" + obj.ResetURL + "</p><p style='color:red;'> The above link will be active for 120 minutes from the time of receiving this email. </p><p style='margin-bottom:0px;'>Regards,</p><p> YIL CMM Team.</p></div></body></html>";
				emailobj.ToEmailId = model.emailid;
				emailobj.FrmEmailId = model.fromemail;

				this.emailTemplateDA.sendEmail(emailobj);
			}
			catch (Exception ex)
			{
				log.ErrorMessage("RFQDA", "sendLinkForForgetPassword", ex.Message + "; " + ex.StackTrace.ToString());

			}
			return true;
		}

		public bool sendMailForgetPassword(forgetpassword model)
		{

			EmailSend emailobj = new EmailSend();
			emailobj.Subject = "Password Reset Status";
			//emailobj.Body = "Reset password updated Successfully";
			emailobj.Body = "<html><meta charset=\"ISO-8859-1\"><head><link rel ='stylesheet' href ='https://maxcdn.bootstrapcdn.com/bootstrap/4.3.1/css/bootstrap.min.css'></head><body><div class='container'><p>Dear User,</p><p>Your Yokogawa Vendor Portal password has been changed successfully.</p><p>Thank you for partnering with YOKOGAWA INDIA LIMITED.</p><p style='margin-bottom:0px;'>Regards,</p><p>YIL CMM Team.</p></div></body></html>";
			emailobj.ToEmailId = model.emailid;
			emailobj.FrmEmailId = model.fromemail;
			this.emailTemplateDA.sendEmail(emailobj);
			return true;
		}

		public string Resetpassword(forgetpassword model)
		{
			string status = "";
			if (model != null)
			{
				RemoteForgetPassword tokenvalid = vscm.RemoteForgetPasswords.Where(li => li.Token == model.token && li.emailId == model.emailid && li.TokenUsed == false).FirstOrDefault();
				if (tokenvalid != null)
				{
					if (tokenvalid.ExpirtyDateAndTime >= System.DateTime.Now && tokenvalid.TokenUsed == false)
					{
						RemoteVendorUserMaster remoteresetpwd = vscm.RemoteVendorUserMasters.Where(li => li.Vuserid == model.emailid).FirstOrDefault();
						remoteresetpwd.pwd = model.ConfirmPassword;
						vscm.SaveChanges();
						VendorUserMaster resetpwd = obj.VendorUserMasters.Where(li => li.Vuserid == model.emailid).FirstOrDefault();
						resetpwd.pwd = model.ConfirmPassword;
						obj.SaveChanges();
						tokenvalid.TokenUsed = true;
						vscm.SaveChanges();
						status = "Updated Successfully";
						sendMailForgetPassword(model);
					}

				}
			}
			return status;
		}
		public bool CheckLinkExpiryOrNot(forgetpassword model)
		{
			bool status = false;

			RemoteForgetPassword tokenvalidcheck = vscm.RemoteForgetPasswords.Where(li => li.Token == model.token && li.emailId == model.emailid).FirstOrDefault();
			if (tokenvalidcheck != null)
			{
				if (tokenvalidcheck.ExpirtyDateAndTime < System.DateTime.Now)
				{
					status = true;
				}
				else
				{
					status = false;

				}

			}

			return status;
		}



		public int insertdata(RfqItemModel model)
		{
			int spiltitemid = 0;
			double? totalprice = 0;
			int RFQItemsId = 0;
			try
			{
				var rfqremoteitem = vscm.RemoteRFQItems_N.Where(x => x.RFQItemsId == model.RFQItemID).FirstOrDefault();
				rfqremoteitem.HSNCode = model.HSNCode;
				rfqremoteitem.QuotationQty = model.QuotationQty;
				rfqremoteitem.VendorModelNo = model.VendorModelNo;
				rfqremoteitem.IGSTPercentage = model.IGSTPercentage;
				rfqremoteitem.SGSTPercentage = model.SGSTPercentage;
				rfqremoteitem.CGSTPercentage = model.CGSTPercentage;
				rfqremoteitem.MfgPartNo = model.MfgPartNo;
				rfqremoteitem.MfgModelNo = model.MfgModelNo;
				rfqremoteitem.ManufacturerName = model.ManufacturerName;
				rfqremoteitem.PFAmount = model.PFAmount;
				rfqremoteitem.PFPercentage = model.PFPercentage;
				rfqremoteitem.FreightAmount = model.FreightAmount;
				rfqremoteitem.FreightPercentage = model.FreightPercentage;
				rfqremoteitem.CustomDuty = model.CustomDuty;
				rfqremoteitem.taxInclusiveOfDiscount = model.taxInclusiveOfDiscount;
				//int rfqitemsid = rfqremoteitem.RFQItemsId;
				vscm.SaveChanges();

				//yscm update
				var rfqitem = obj.RFQItems_N.Where(x => x.RFQItemsId == model.RFQItemID).FirstOrDefault();
				rfqitem.HSNCode = model.HSNCode;
				rfqitem.QuotationQty = model.QuotationQty;
				rfqitem.VendorModelNo = model.VendorModelNo;
				rfqitem.IGSTPercentage = model.IGSTPercentage;
				rfqitem.SGSTPercentage = model.SGSTPercentage;
				rfqitem.CGSTPercentage = model.CGSTPercentage;
				rfqitem.MfgPartNo = model.MfgPartNo;
				rfqitem.MfgModelNo = model.MfgModelNo;
				rfqitem.ManufacturerName = model.ManufacturerName;
				rfqitem.PFAmount = model.PFAmount;
				rfqitem.PFPercentage = model.PFPercentage;
				rfqitem.FreightAmount = model.FreightAmount;
				rfqitem.FreightPercentage = model.FreightPercentage;
				rfqitem.CustomDuty = model.CustomDuty;
				rfqitem.taxInclusiveOfDiscount = model.taxInclusiveOfDiscount;
				//int rfqitemsid = rfqremoteitem.RFQItemsId;
				obj.SaveChanges();
				if (model.multipleitem == "yes")
				{
					foreach (var item in model.iteminfo)
					{
						RFQItemsId = item.RFQItemsId;
						var info = new RemoteRfqVendorBOM();
						info.ItemDescription = item.ItemDescriptionForMultiple;
						info.ItemName = item.ItemNameForMultiple;
						info.RfqItemsId = item.RFQItemsId;
						info.HSNCode = model.HSNCode;
						info.QuotationQty = model.QuotationQty;
						info.VendorModelNo = model.VendorModelNo;
						info.IGSTPercentage = model.IGSTPercentage;
						info.SGSTPercentage = model.SGSTPercentage;
						info.CGSTPercentage = model.CGSTPercentage;
						info.MfgPartNo = model.MfgPartNo;
						info.MfgModelNo = model.MfgModelNo;
						info.ManufacturerName = model.ManufacturerName;
						info.PFPercentage = model.PFPercentage;
						info.PFAmount = model.PFAmount;
						info.PFPercentage = model.PFPercentage;
						info.FreightAmount = model.FreightAmount;
						info.CustomDuty = model.CustomDuty;
						//rfqremoteitem.taxInclusiveOfDiscount = model.taxInclusiveOfDiscount;
						info.Qty = item.Quantity;
						info.UOM = item.UOM;
						info.UnitPrice = item.UnitPrice;
						info.DiscountPercentage = item.DiscountPercentage;
						info.Discount = item.Discount;
						info.CurrencyId = item.CurrencyID;
						info.CurrencyValue = item.CurrencyValue;
						info.Remarks = item.Remarks;
						info.DeliveryDate = item.DeliveryDate;
						info.ItemDescription = model.ItemDescription;
						info.DeleteFlag = false;
						vscm.RemoteRfqVendorBOMs.Add(info);
						vscm.SaveChanges();
						int bomid = info.RFQVendorbomItemId;

						//in yscm

						var infos = new RfqVendorBOM();
						infos.RFQVendorbomItemId = bomid;
						infos.ItemDescription = item.ItemDescriptionForMultiple;
						infos.ItemName = item.ItemNameForMultiple;
						infos.RfqItemsId = item.RFQItemsId;
						infos.HSNCode = model.HSNCode;
						infos.QuotationQty = model.QuotationQty;
						infos.VendorModelNo = model.VendorModelNo;
						infos.IGSTPercentage = model.IGSTPercentage;
						infos.SGSTPercentage = model.SGSTPercentage;
						infos.CGSTPercentage = model.CGSTPercentage;
						infos.MfgPartNo = model.MfgPartNo;
						infos.MfgModelNo = model.MfgModelNo;
						infos.ManufacturerName = model.ManufacturerName;
						infos.PFPercentage = model.PFPercentage;
						infos.PFAmount = model.PFAmount;
						infos.PFPercentage = model.PFPercentage;
						infos.FreightAmount = model.FreightAmount;
						infos.CustomDuty = model.CustomDuty;
						//rfqremoteitem.taxInclusiveOfDiscount = model.taxInclusiveOfDiscount;
						infos.Qty = item.Quantity;
						infos.UOM = item.UOM;
						infos.UnitPrice = item.UnitPrice;
						infos.DiscountPercentage = item.DiscountPercentage;
						infos.Discount = item.Discount;
						infos.CurrencyId = item.CurrencyID;
						infos.CurrencyValue = item.CurrencyValue;
						infos.Remarks = item.Remarks;
						infos.DeliveryDate = item.DeliveryDate;
						infos.ItemDescription = model.ItemDescription;
						infos.DeleteFlag = false;
						obj.RfqVendorBOMs.Add(infos);
						obj.SaveChanges();


						List<RemoteRfqVendorBOM> itemsforupdate = vscm.RemoteRfqVendorBOMs.Where(li => li.RfqItemsId == RFQItemsId && li.DeleteFlag == false).ToList<RemoteRfqVendorBOM>();

						foreach (var data1 in itemsforupdate)
						{
							double? subtotalprice = 0;
							subtotalprice = Convert.ToDouble(data1.Qty) * Convert.ToDouble(data1.UnitPrice);
							if (Convert.ToInt32(data1.Discount) != 0)
							{
								double? Discount = subtotalprice - Convert.ToDouble(data1.Discount);
								subtotalprice = Discount;
							}
							if (Convert.ToInt32(data1.DiscountPercentage) != 0)
							{
								double? DiscountPercentage = (subtotalprice - subtotalprice * Convert.ToDouble(data1.DiscountPercentage) / 100);
								subtotalprice = DiscountPercentage;
							}
							if (Convert.ToInt32(data1.FreightAmount) != 0)
							{
								double? FreightAmount = subtotalprice - Convert.ToDouble(data1.FreightAmount);
								subtotalprice = FreightAmount;
							}
							if (Convert.ToInt32(data1.FreightPercentage) != 0)
							{
								double? FreightPercentage = (subtotalprice - subtotalprice * Convert.ToDouble(data1.FreightPercentage) / 100);
								subtotalprice = FreightPercentage;
							}
							if (Convert.ToInt32(data1.PFAmount) != 0)
							{
								double? PFAmount = subtotalprice - Convert.ToDouble(data1.PFAmount);
								subtotalprice = PFAmount;
							}
							if (Convert.ToInt32(data1.PFPercentage) != 0)
							{

								double? PFPercentage = (subtotalprice * Convert.ToDouble(data1.PFPercentage) / 100);
								subtotalprice += PFPercentage;
							}
							totalprice += subtotalprice;

						}
						RfqVendorBOM bompriceupdate = obj.RfqVendorBOMs.Where(li => li.RFQVendorbomItemId == bomid).FirstOrDefault();
						if (bompriceupdate != null)
						{
							bompriceupdate.UnitPrice = Convert.ToInt32(totalprice);
							obj.SaveChanges();
						}

						RemoteRFQItemsInfo_N itemsinfos = vscm.RemoteRFQItemsInfo_N.Where(li => li.RFQItemsId == RFQItemsId).FirstOrDefault();
						if (itemsinfos != null)
						{
							itemsinfos.UnitPrice = Convert.ToDecimal(totalprice);
							vscm.SaveChanges();
						}
						else
						{
							var remoteinfo = new RemoteRFQItemsInfo_N();
							remoteinfo.RFQItemsId = item.RFQItemsId;
							remoteinfo.Qty = 1;//item.Quantity;
							remoteinfo.UOM = item.UOM;
							remoteinfo.UnitPrice = item.UnitPrice;
							remoteinfo.DiscountPercentage = item.DiscountPercentage;
							remoteinfo.Discount = item.Discount;
							remoteinfo.CurrencyId = item.CurrencyID;
							remoteinfo.CurrencyValue = item.CurrencyValue;
							remoteinfo.Remarks = item.Remarks;
							remoteinfo.DeliveryDate = item.DeliveryDate;
							remoteinfo.SyncDate = System.DateTime.Now;

							vscm.RemoteRFQItemsInfo_N.Add(remoteinfo);
							vscm.SaveChanges();
							spiltitemid = remoteinfo.RFQSplitItemId;
						}

						RFQItemsInfo_N rfqinsertdata = obj.RFQItemsInfo_N.Where(li => li.RFQSplitItemId == spiltitemid).FirstOrDefault();
						if (itemsinfos != null)
						{
							itemsinfos.UnitPrice = Convert.ToDecimal(totalprice);
							vscm.SaveChanges();
						}
						else
						{
							var remoteinfo = new RFQItemsInfo_N();
							remoteinfo.RFQItemsId = item.RFQItemsId;
							remoteinfo.Qty = 1;//item.Quantity;
							remoteinfo.UOM = item.UOM;
							remoteinfo.UnitPrice = item.UnitPrice;
							remoteinfo.DiscountPercentage = item.DiscountPercentage;
							remoteinfo.Discount = item.Discount;
							remoteinfo.CurrencyId = item.CurrencyID;
							remoteinfo.CurrencyValue = item.CurrencyValue;
							remoteinfo.Remarks = item.Remarks;
							remoteinfo.DeliveryDate = item.DeliveryDate;
							//remoteinfo.SyncDate = System.DateTime.Now;

							obj.RFQItemsInfo_N.Add(remoteinfo);
							obj.SaveChanges();
							//spiltitemid = remoteinfo.RFQSplitItemId;
						}
					}
				}
				else if (model.multipleitem == "no")
				{
					foreach (var item in model.iteminfo)
					{
						RemoteRFQItemsInfo_N itemsinfos = vscm.RemoteRFQItemsInfo_N.Where(li => li.RFQItemsId == RFQItemsId).FirstOrDefault();
						if (itemsinfos != null)
						{
							itemsinfos.UnitPrice = Convert.ToDecimal(totalprice);
							vscm.SaveChanges();
						}
						else
						{
							var remoteinfo = new RemoteRFQItemsInfo_N();
							remoteinfo.RFQItemsId = item.RFQItemsId;
							remoteinfo.Qty = 1;//item.Quantity;
							remoteinfo.UOM = item.UOM;
							remoteinfo.UnitPrice = item.UnitPrice;
							remoteinfo.DiscountPercentage = item.DiscountPercentage;
							remoteinfo.Discount = item.Discount;
							remoteinfo.CurrencyId = item.CurrencyID;
							remoteinfo.CurrencyValue = item.CurrencyValue;
							remoteinfo.Remarks = item.Remarks;
							remoteinfo.DeliveryDate = item.DeliveryDate;
							remoteinfo.SyncDate = System.DateTime.Now;

							vscm.RemoteRFQItemsInfo_N.Add(remoteinfo);
							vscm.SaveChanges();
							spiltitemid = remoteinfo.RFQSplitItemId;
						}

						RFQItemsInfo_N rfqinsertdata = obj.RFQItemsInfo_N.Where(li => li.RFQSplitItemId == spiltitemid).FirstOrDefault();
						if (itemsinfos != null)
						{
							itemsinfos.UnitPrice = Convert.ToDecimal(totalprice);
							vscm.SaveChanges();
						}
						else
						{
							var remoteinfo = new RFQItemsInfo_N();
							remoteinfo.RFQItemsId = item.RFQItemsId;
							remoteinfo.Qty = 1;//item.Quantity;
							remoteinfo.UOM = item.UOM;
							remoteinfo.UnitPrice = item.UnitPrice;
							remoteinfo.DiscountPercentage = item.DiscountPercentage;
							remoteinfo.Discount = item.Discount;
							remoteinfo.CurrencyId = item.CurrencyID;
							remoteinfo.CurrencyValue = item.CurrencyValue;
							remoteinfo.Remarks = item.Remarks;
							remoteinfo.DeliveryDate = item.DeliveryDate;
							//remoteinfo.SyncDate = System.DateTime.Now;

							obj.RFQItemsInfo_N.Add(remoteinfo);
							obj.SaveChanges();
							//spiltitemid = remoteinfo.RFQSplitItemId;
						}

					}
				}
			}
			catch (Exception ex)
			{
				log.ErrorMessage("RFQDA", "insertdata", ex.Message + "; " + ex.StackTrace.ToString());
			}

			return 1;

		}


		/*Name of Function : <<rfqStatusUpdate>>  Author :<<Prasanna>>  
		Date of Creation <<28-10-2020>>
		Purpose : <<Update Rfq status in both vscm and yscm rfq status tables>>
		Review Date :<<>>   Reviewed By :<<>>*/
		public bool rfqStatusUpdate(RFQStatu rfqStatus)
		{
			try
			{
				int RfqMasterId = vscm.RemoteRFQRevisions_N.Where(li => li.rfqRevisionId == rfqStatus.RfqRevisionId).FirstOrDefault().rfqMasterId;

				RemoteRFQStatu statusobj = new RemoteRFQStatu();
				statusobj.RfqRevisionId = rfqStatus.RfqRevisionId;
				statusobj.RfqMasterId = RfqMasterId;
				statusobj.StatusId = rfqStatus.StatusId;
				statusobj.Remarks = rfqStatus.Remarks;
				statusobj.DeleteFlag = false;
				statusobj.updatedby = rfqStatus.updatedby;
				statusobj.updatedDate = System.DateTime.Now;
				vscm.RemoteRFQStatus.Add(statusobj);
				RemoteRFQRevisions_N remoteRfqRevision = vscm.RemoteRFQRevisions_N.Where(li => li.rfqRevisionId == rfqStatus.RfqRevisionId).FirstOrDefault();
				if (remoteRfqRevision != null)
				{
					remoteRfqRevision.StatusId = Convert.ToByte(rfqStatus.StatusId);
				}

				vscm.SaveChanges();

				int rfqstatusid = statusobj.RfqStatusId;
				RFQStatu statusobjs = new RFQStatu();
				statusobjs.RfqStatusId = rfqstatusid;
				statusobjs.RfqRevisionId = rfqStatus.RfqRevisionId;
				statusobjs.RfqMasterId = RfqMasterId;
				statusobjs.StatusId = rfqStatus.StatusId;
				statusobjs.Remarks = rfqStatus.Remarks;
				statusobjs.DeleteFlag = false;
				statusobjs.updatedby = rfqStatus.updatedby;
				statusobjs.updatedDate = System.DateTime.Now;
				obj.RFQStatus.Add(statusobjs);
				RFQRevisions_N localRfqRevision = obj.RFQRevisions_N.Where(li => li.rfqRevisionId == rfqStatus.RfqRevisionId).FirstOrDefault();
				if (localRfqRevision != null)
				{
					localRfqRevision.StatusId = Convert.ToByte(rfqStatus.StatusId);
				}
				obj.SaveChanges();
				string StatusTxt = "";
				if (rfqStatus.StatusId == 26)
					StatusTxt = "Acknowledged";
				else
					StatusTxt = "Regretted";
				this.emailTemplateDA.sendSatustoRequestor(rfqStatus.RfqRevisionId, StatusTxt, rfqStatus.Remarks);
			}
			catch (Exception e)
			{

				log.ErrorMessage("RFQDA", "rfqStatusUpdate", e.Message.ToString() + e.InnerException.ToString() + e.ToString());

			}
			return true;
		}

		public bool checkrfqitemexists(int rfqitemsid)
		{
			Boolean returndata = false;
			RemoteRfqVendorBOM obj = vscm.RemoteRfqVendorBOMs.Where(x => x.RfqItemsId == rfqitemsid && x.DeleteFlag == false).FirstOrDefault();
			if (obj != null)
			{
				returndata = true;
			}
			return returndata;
		}

		/*Name of Function : <<getDBMastersList>>  Author :<<Prasanna>>  
	    Date of Creation <<22-10-2020>>
	    Purpose : <<get table data dynamically by passing query as parameter>>
	    Review Date :<<>>   Reviewed By :<<>>*/
		public DataTable getDBMastersList(DynamicSearchResult Result)
		{
			Result.connectionString = obj.Database.Connection.ConnectionString;
			DataTable dtDBMastersList = new DataTable();
			string query = "";
			if (!string.IsNullOrEmpty(Result.tableName))
			{
				query = "select * from " + Result.tableName;
				if (Result.sortBy != null)
				{
					query += " order by " + Result.sortBy;
				}
			}
			else if (Result.query != "")
			{
				query = Result.query;
			}

			SqlConnection con = new SqlConnection(obj.Database.Connection.ConnectionString);
			SqlCommand cmd = new SqlCommand(query, con);
			con.Open();
			SqlDataAdapter da = new SqlDataAdapter(cmd);
			da.Fill(dtDBMastersList);
			con.Close();
			da.Dispose();

			return dtDBMastersList;
		}


	}

}

