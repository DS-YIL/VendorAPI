using DALayer.MPR;
using SCMModels;
using SCMModels.MPRMasterModels;
using SCMModels.RemoteModel;
using SCMModels.RFQModels;
using SCMModels.SCMModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Net.Mail;
using System.Net;
using System.Security.Cryptography;
using System.Configuration;

namespace DALayer.RFQ
{
    public class RFQDA : IRFQDA
    {
        private IMPRDA MPRDA = default(IMPRDA);
        public RFQDA(IMPRDA MPRDA)
        {
            this.MPRDA = MPRDA;
        }
        VSCMEntities vscm = new VSCMEntities();
        YSCMEntities obj = new YSCMEntities();

        public Expression<Func<RemoteRFQMaster, object>> MPRRevisionId { get; private set; }

        public List<RFQQuoteView> getRFQItems(int RevisionId)
        {
            using (YSCMEntities Context = new YSCMEntities())
            {
                string query = "select  *  from RFQQuoteView where MPRRevisionId=" + RevisionId + " ORDER BY  RFQQuoteView.ItemId, RFQQuoteView.UnitPrice";
                return Context.Database.SqlQuery<RFQQuoteView>(query).ToList();
                //return Context.RFQQuoteViews.Where(li => li.MPRRevisionId == RevisionId).ToList();
            }
        }
        public bool updateVendorQuotes(List<RFQQuoteView> RFQQuoteViewList, List<YILTermsandCondition> termsList)
        {
            List<RFQTermsModel> rfqList = new List<RFQTermsModel>();
            foreach (var data in termsList)
            {
                if (data.DefaultSelect == true)
                {
                    RFQTermsModel rfqterm = new RFQTermsModel();
                    rfqterm.termsid = data.TermId;
                    rfqterm.TermGroup = obj.YILTermsGroups.Where(li => li.TermGroupId == data.TermGroupId).FirstOrDefault<YILTermsGroup>().TermGroup;
                    rfqterm.Terms = data.Terms;
                    rfqterm.CreatedBy = "190455";
                    rfqterm.CreatedDate = DateTime.Now;
                    rfqList.Add(rfqterm);
                }

            }
            foreach (RFQQuoteView item in RFQQuoteViewList)
            {
                RfqRevisionModel rfqModel = new RfqRevisionModel();
                rfqModel.rfqmaster = new RFQMasterModel();
                rfqModel.rfqmaster.MPRRevisionId = Convert.ToInt32(item.MPRRevisionId);
                rfqModel.rfqmaster.VendorId = item.VendorId;
                rfqModel.rfqmaster.CreatedBy = "190455";
                rfqModel.rfqmaster.Created = DateTime.Now;
                rfqModel.CreatedBy = 190455;
                rfqModel.CreatedDate = DateTime.Now;
                //rfqModel.RfqValidDate = item.RFQValidDate;
                rfqModel.PackingForwading = item.PackingForwarding;
                rfqModel.ExciseDuty = item.ExciseDuty;
                rfqModel.salesTax = item.SalesTax;
                rfqModel.freight = item.Freight;
                rfqModel.Insurance = item.Insurance;
                rfqModel.CustomsDuty = item.CustomsDuty;
                rfqModel.ShipmentModeId = item.ShipmentModeid;
                rfqModel.PaymentTermDays = item.PaymentTermDays;
                rfqModel.PaymentTermRemarks = item.PaymentTermRemarks;
                rfqModel.BankGuarantee = item.BankGuarantee;
                rfqModel.DeliveryMinWeeks = item.DeliveryMinWeeks;
                rfqModel.DeliveryMaxWeeks = item.DeliveryMaxWeeks;
                RfqItemModel rfqitem = new RfqItemModel();
                // rfqitem.MRPItemsDetailsID = item.MPRItemDetailsid;
                rfqitem.ItemName = item.ItemName;
                rfqitem.ItemDescription = item.ItemDescription;
                //rfqitem.QuotationQty = item.QuotationQty;
                rfqModel.rfqitem.Add(rfqitem);
                rfqModel.RFQTerms = rfqList;
                CreateRfQ(rfqModel);
                MPRStatusTrack mPRStatusTrackDetails = new MPRStatusTrack();
                mPRStatusTrackDetails.RequisitionId = obj.MPRRevisions.Where(li => li.RevisionId == item.MPRRevisionId).FirstOrDefault().RequisitionId;
                mPRStatusTrackDetails.RevisionId = Convert.ToInt32(item.MPRRevisionId);
                mPRStatusTrackDetails.StatusId = 7;
                mPRStatusTrackDetails.UpdatedBy = "190455";
                mPRStatusTrackDetails.UpdatedDate = DateTime.Now;
                this.MPRDA.updateMprstatusTrack(mPRStatusTrackDetails);
            }
            return true;

        }

        public DataTable getRFQCompareItems(int RevisionId)
        {
            DataTable table = new DataTable();

            using (YSCMEntities Context = new YSCMEntities())
            {

                string query = "select mprdet.DocumentNo,mprdet.DocumentDescription,mprdet.IssuePurposeId,mprdet.DepartmentName,mprdet.ProjectManagerName,mprdet.JobCode,mprdet.JobName,mprdet.GEPSApprovalId,mprdet.SaleOrderNo,mprdet.ClientName,mprdet.PlantLocation,mprdet.BuyerGroupName, * from RFQQuoteView inner join MPRRevisionDetails mprdet on mprdet.RevisionId = RFQQuoteView.MPRRevisionId where (Status not like '%Approved%' or Status is null) and MPRRevisionId=" + RevisionId + "";
                var cmd = Context.Database.Connection.CreateCommand();
                cmd.CommandText = query;

                cmd.Connection.Open();
                table.Load(cmd.ExecuteReader());
                cmd.Connection.Close();
            }
            return table;
        }
        public DataTable gerRFQVendorQuoteDetails(int rfqRevisionId)
        {
            DataTable table = new DataTable();

            using (YSCMEntities Context = new YSCMEntities())
            {

                string query = "select mprdet.DocumentNo,mprdet.DocumentDescription,mprdet.IssuePurposeId,mprdet.DepartmentName,mprdet.ProjectManagerName,mprdet.JobCode,mprdet.JobName,mprdet.GEPSApprovalId,mprdet.SaleOrderNo,mprdet.ClientName,mprdet.PlantLocation,mprdet.BuyerGroupName, * from RFQQuoteView inner join MPRRevisionDetails mprdet on mprdet.RevisionId = RFQQuoteView.MPRRevisionId where (Status not like '%Approved%' or Status is null) and rfqRevisionId=" + rfqRevisionId + "";
                var cmd = Context.Database.Connection.CreateCommand();
                cmd.CommandText = query;

                cmd.Connection.Open();
                table.Load(cmd.ExecuteReader());
                cmd.Connection.Close();
            }
            return table;
        }
        public bool rfqStatusUpdate(List<RFQQuoteView> vendorList)
        {
            using (YSCMEntities Context = new YSCMEntities())
            {
                foreach (var item in vendorList)
                {
                    RFQItem rfqItem = Context.RFQItems.Where(li => li.RFQItemsId == item.RFQItemsId).FirstOrDefault<RFQItem>();
                    rfqItem.Status = "Approved";
                    rfqItem.StatusUpdatedBy = "190455";
                    rfqItem.StatusUpdateddate = DateTime.Now;
                    Context.SaveChanges();

                }
            }
            return true;
        }
        public async Task<statuscheckmodel> CreateRfQ(RfqRevisionModel model)
        {
            statuscheckmodel status = new statuscheckmodel();
            try
            {
                //server data
                var rfqremote = new RemoteRFQMaster();
                if (model != null)
                {

                    vscm.Database.Connection.Open();
                    if (model.rfqmaster.RfqMasterId == 0)
                    {
                        //string unique = obj.RFQMasters.Select(x => x.RFQNo).FirstOrDefault();
                        rfqremote.RFQNo = "rfq/" + DateTime.Now.ToString("MMyy") + "/";
                        rfqremote.MPRRevisionId = model.rfqmaster.MPRRevisionId;
                        rfqremote.RFQUniqueNo = model.rfqmaster.RfqUniqueNo;
                        rfqremote.CreatedBy = model.rfqmaster.CreatedBy;
                        rfqremote.CreatedDate = model.rfqmaster.Created;
                        rfqremote.VendorId = model.rfqmaster.VendorId;
                        vscm.RemoteRFQMasters.Add(rfqremote);
                        vscm.SaveChanges();
                    }
                    else
                    {
                        rfqremote.RFQUniqueNo = model.rfqmaster.RfqUniqueNo;
                        //rfqdomain.RfqMasterId = model.RfqMasterId;
                        rfqremote.MPRRevisionId = model.rfqmaster.MPRRevisionId;
                        rfqremote.VendorId = model.rfqmaster.VendorId;
                        rfqremote.CreatedBy = model.rfqmaster.CreatedBy;
                        rfqremote.CreatedDate = model.rfqmaster.Created;
                        vscm.RemoteRFQMasters.Add(rfqremote);
                        vscm.SaveChanges();
                    }

                    int id = rfqremote.RfqMasterId;

                    RemoteRFQRevision revision = new RemoteRFQRevision();
                    if (model.RfqRevisionId == 0)
                    {
                        revision.rfqMasterId = id;
                        revision.RevisionNo = model.RfqRevisionNo;
                        revision.CreatedBy = model.CreatedBy;
                        revision.CreatedDate = model.CreatedDate;
                        revision.RFQValidDate = model.RfqValidDate;
                        revision.SalesTax = model.salesTax;
                        revision.PaymentTermRemarks = model.PaymentTermRemarks;
                        revision.PackingForwarding = model.PackingForwading;
                        revision.PaymentTermDays = model.PaymentTermDays;
                        revision.Insurance = model.Insurance;
                        revision.ExciseDuty = model.ExciseDuty;
                        revision.ShipmentModeid = model.ShipmentModeId;
                        revision.BankGuarantee = model.BankGuarantee;
                        revision.DeliveryMaxWeeks = model.DeliveryMaxWeeks;
                        revision.DeliveryMinWeeks = model.DeliveryMinWeeks;
                        //revision.RemoteRFQStatus.Select(x => new Remotedata.RFQStatu()
                        //{
                        //    StatusId = Convert.ToInt32(Enum.GetName(typeof(RFQStatusType), RFQStatusType.requested))
                        //});

                        vscm.RemoteRFQRevisions.Add(revision);
                        vscm.SaveChanges();
                    }
                    else
                    {
                        revision.RevisionNo = model.RfqRevisionNo;
                        revision.CreatedBy = model.CreatedBy;
                        revision.CreatedDate = model.CreatedDate;
                        revision.RFQValidDate = model.RfqValidDate;
                        revision.SalesTax = model.salesTax;
                        revision.PaymentTermRemarks = model.PaymentTermRemarks;
                        revision.PackingForwarding = model.PackingForwading;
                        revision.PaymentTermDays = model.PaymentTermDays;
                        revision.Insurance = model.Insurance;
                        revision.ExciseDuty = model.ExciseDuty;
                        revision.ShipmentModeid = model.ShipmentModeId;
                        revision.BankGuarantee = model.BankGuarantee;
                        revision.DeliveryMaxWeeks = model.DeliveryMaxWeeks;
                        revision.DeliveryMinWeeks = model.DeliveryMinWeeks;

                        vscm.RemoteRFQRevisions.Add(revision);
                        vscm.SaveChanges();
                    }


                    int revisionid = revision.rfqRevisionId;

                    foreach (var data in model.rfqitem)
                    {

                        var rfitems = new RemoteRFQItems_N()
                        {
                            RFQRevisionId = revisionid,
                            MPRItemDetailsid = data.MRPItemsDetailsID,
                            QuotationQty = data.QuotationQty,
                            VendorModelNo = data.VendorModelNo,
                            HSNCode = data.HSNCode,
                            RequestRemarks = data.RequsetRemarks,
                            ItemName = data.ItemName,
                            ItemDescription = data.ItemDescription,

                        };
                        vscm.RemoteRFQItems_N.Add(rfitems);
                        vscm.SaveChanges();
                    }
                }
                int masterid = rfqremote.RfqMasterId;
                int remoterevisionid = (from x in vscm.RemoteRFQRevisions orderby x.rfqRevisionId descending select x.rfqRevisionId).First();
                int Ritemid = (from x in vscm.RemoteRFQItems_N orderby x.RFQItemsId descending select x.RFQItemsId).First();
                vscm.Database.Connection.Close();
                if (model != null)
                {
                    // obj.Database.Connection.Open();
                    var rfqlocal = new RFQMaster();
                    if (model.rfqmaster.RfqMasterId == 0)
                    {
                        //string unique = obj.RFQMasters.Select(x => x.RFQNo).FirstOrDefault();
                        rfqlocal.RFQNo = "rfq/" + DateTime.Now.ToString("MMyy") + "/";
                        rfqlocal.MPRRevisionId = model.rfqmaster.MPRRevisionId;
                        rfqlocal.RfqMasterId = masterid;
                        rfqlocal.RFQUniqueNo = model.rfqmaster.RfqUniqueNo;
                        rfqlocal.CreatedBy = model.rfqmaster.CreatedBy;
                        rfqlocal.CreatedDate = model.rfqmaster.Created;
                        rfqlocal.VendorId = model.rfqmaster.VendorId;
                        obj.RFQMasters.Add(rfqlocal);
                        obj.SaveChanges();
                    }
                    else
                    {
                        rfqlocal.RFQUniqueNo = model.rfqmaster.RfqUniqueNo;
                        rfqlocal.MPRRevisionId = model.rfqmaster.MPRRevisionId;
                        //rfqdomain.RfqMasterId = model.RfqMasterId;
                        rfqlocal.VendorId = model.rfqmaster.VendorId;
                        rfqlocal.CreatedBy = model.rfqmaster.CreatedBy;
                        rfqlocal.CreatedDate = model.rfqmaster.Created;
                        obj.RFQMasters.Add(rfqlocal);
                        obj.SaveChanges();
                    }

                    RFQRevision revision = new RFQRevision();
                    if (model.RfqRevisionId == 0)
                    {
                        revision.rfqRevisionId = remoterevisionid;
                        revision.rfqMasterId = masterid;
                        revision.RevisionNo = model.RfqRevisionNo;
                        revision.CreatedBy = model.CreatedBy;
                        revision.CreatedDate = model.CreatedDate;
                        revision.RFQValidDate = model.RfqValidDate;
                        revision.SalesTax = model.salesTax;
                        revision.PaymentTermRemarks = model.PaymentTermRemarks;
                        revision.PackingForwarding = model.PackingForwading;
                        revision.PaymentTermDays = model.PaymentTermDays;
                        revision.Insurance = model.Insurance;
                        revision.ExciseDuty = model.ExciseDuty;
                        revision.ShipmentModeid = model.ShipmentModeId;
                        revision.BankGuarantee = model.BankGuarantee;
                        revision.DeliveryMaxWeeks = model.DeliveryMaxWeeks;
                        revision.DeliveryMinWeeks = model.DeliveryMinWeeks;
                        //revision.RFQStatus.Select(x => new RFQStatu()
                        //{
                        //    StatusId = Convert.ToInt32(Enum.GetName(typeof(RFQStatusType), RFQStatusType.requested))
                        //});

                        obj.RFQRevisions.Add(revision);
                        obj.SaveChanges();
                    }
                    else
                    {
                        revision.RevisionNo = model.RfqRevisionNo;
                        revision.CreatedBy = model.CreatedBy;
                        revision.CreatedDate = model.CreatedDate;
                        revision.RFQValidDate = model.RfqValidDate;
                        revision.SalesTax = model.salesTax;
                        revision.PaymentTermRemarks = model.PaymentTermRemarks;
                        revision.PackingForwarding = model.PackingForwading;
                        revision.PaymentTermDays = model.PaymentTermDays;
                        revision.Insurance = model.Insurance;
                        revision.ExciseDuty = model.ExciseDuty;
                        revision.ShipmentModeid = model.ShipmentModeId;
                        revision.BankGuarantee = model.BankGuarantee;
                        revision.DeliveryMaxWeeks = model.DeliveryMaxWeeks;
                        revision.DeliveryMinWeeks = model.DeliveryMinWeeks;

                        obj.RFQRevisions.Add(revision);
                        obj.SaveChanges();
                    }


                    int revisionid = revision.rfqRevisionId;

                    foreach (var data in model.rfqitem)
                    {
                        var rfitems = new RFQItem()
                        {
                            RFQItemsId = Ritemid,
                            RFQRevisionId = remoterevisionid,
                            MPRItemDetailsid = data.MRPItemsDetailsID,
                            QuotationQty = data.QuotationQty,
                            VendorModelNo = data.VendorModelNo,
                            HSNCode = data.HSNCode,
                            RequestRemarks = data.RequsetRemarks,
                            DeleteFlag = false
                        };
                        revision.RFQItems.Add(rfitems);
                        obj.SaveChanges();
                    }
                    status.Sid = rfqlocal.RfqMasterId;
                    foreach (RFQTermsModel terms in model.RFQTerms)
                    {
                        terms.RFQrevisionId = revisionid;
                        InsertRFQTerms(terms);
                    }
                }
                return status;
            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                           eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                               ve.PropertyName, ve.ErrorMessage);
                    }
                }
                throw;
            }

        }
        public async Task<statuscheckmodel> CreateNewRfq(RFQMasterModel model)
        {
            statuscheckmodel status = new statuscheckmodel();
            try
            {
                var rfqremote = new RemoteRFQMaster();
                if (model != null)
                {
                    //var remoteconnection = obj.Database.Connection.ConnectionString;
                    var remote = vscm.Database.Connection.ConnectionString;
                    vscm.Database.Connection.Open();
                    //vscm.Database.Connection.Close();
                    var result = obj.Database.Connection.ConnectionString;

                    rfqremote.RFQNo = model.RfqNo;
                    rfqremote.RFQUniqueNo = model.RfqUniqueNo;
                    rfqremote.MPRRevisionId = model.MPRRevisionId;
                    rfqremote.VendorId = model.VendorId;
                    rfqremote.CreatedBy = model.CreatedBy;
                    rfqremote.CreatedDate = model.Created;
                    vscm.RemoteRFQMasters.Add(rfqremote);
                    vscm.SaveChanges();
                }
                int masterid = rfqremote.RfqMasterId;
                vscm.Database.Connection.Close();
                var rfqlocal = new RFQMaster();
                if (model != null)
                {
                    var localconnection = vscm.Database.Connection.ConnectionString;
                    obj.Database.Connection.Open();
                    rfqlocal.RfqMasterId = masterid;
                    rfqlocal.RFQNo = model.RfqNo;
                    rfqlocal.RFQUniqueNo = model.RfqUniqueNo;
                    rfqlocal.MPRRevisionId = model.MPRRevisionId;
                    rfqlocal.VendorId = model.VendorId;
                    rfqlocal.CreatedBy = model.CreatedBy;
                    rfqlocal.CreatedDate = model.Created;
                    obj.RFQMasters.Add(rfqlocal);
                    obj.SaveChanges();
                    obj.Database.Connection.Close();
                }

                return status;
            }

            catch (Exception ex)
            {

                throw;
            }

        }
        public async Task<List<RFQMasterModel>> getallrfqlist()
        {
            List<RFQMasterModel> master = new List<RFQMasterModel>();
            try
            {
                var result = obj.RFQMasters.Where(x => x.DeleteFlag == false).Include(x => x.RFQRevisions_N).ToList();
                if (result != null)
                {
                    foreach (var item in result)
                    {
                        master = result.ConvertAll(x => new RFQMasterModel()
                        {
                            RfqMasterId = x.RfqMasterId,
                            VendorId = x.VendorId,
                            RfqNo = x.RFQNo,
                            RfqUniqueNo = x.RFQUniqueNo,
                            Created = x.CreatedDate,
                            CreatedBy = x.CreatedBy,
                            Revision = x.RFQRevisions_N.Where(y => y.rfqMasterId == x.RfqMasterId).Select(y => new RfqRevisionModel()
                            {
                                RfqRevisionId = y.rfqRevisionId,
                                RfqValidDate = y.RFQValidDate,
                                RfqMasterId = y.rfqMasterId,
                                RfqRevisionNo = y.RevisionNo
                            }).ToList()
                        });

                    }

                }
                //remote data
                //var remotedata = vscm.RemoteRFQMasters.Where(x => x.DeleteFlag == false).Include(x => x.RemoteRFQRevisions).ToList();
                //if (remotedata != null)
                //{
                //    foreach (var item in remotedata)
                //    {
                //        master = result.ConvertAll(x => new RFQMasterModel()
                //        {
                //            RfqMasterId = x.RfqMasterId,
                //            VendorId = x.VendorId,
                //            RfqNo = x.RFQNo,
                //            RfqUniqueNo = x.RFQUniqueNo,
                //            Created = x.CreatedDate,
                //            CreatedBy = x.CreatedBy,
                //            Revision = x.RFQRevisions.Where(y => y.rfqMasterId == x.RfqMasterId).Select(y => new RfqRevisionModel()
                //            {
                //                RfqRevisionId = y.rfqRevisionId,
                //                RfqValidDate = y.RFQValidDate,
                //                RfqMasterId = y.rfqMasterId,
                //                RfqRevisionNo = y.RevisionNo
                //            }).ToList()
                //        });

                //    }

                //}
                return master;
            }
            catch (Exception)
            {

                throw;
            }
        }
        //public async Task<List<RfqRevisionModel>> GetAllRFQs()
        //{
        //    List<RfqRevisionModel> model;

        //    try
        //    {
        //        obj.Configuration.ProxyCreationEnabled = false;
        //        model = obj.RFQRevisions.Where(x => x.DeleteFlag == false).Include(x => x.RFQMaster).ToList();
        //        //foreach (var item in revision)
        //        //{
        //        //    model = revision.Select(x => new RfqRevisionModel()
        //        //    {
        //        //        RfqRevisionId = x.rfqRevisionId,
        //        //        RfqRevisionNo = x.RevisionNo,
        //        //        RfqValidDate = x.RFQValidDate,
        //        //        CreatedBy = x.CreatedBy,
        //        //        CreatedDate = x.CreatedDate,
        //        //    }).ToList();
        //        //    foreach (var items in revision)
        //        //    {
        //        //        foreach (var masters in revision)
        //        //        {

        //        //        }
        //        //    }

        //        //}   




        //        //foreach (var item in revision)
        //        //{
        //        //    revision.Select(x => new RfqRevisionModel()
        //        //    {
        //        //        RfqRevisionId = item.rfqRevisionId,
        //        //        CreatedDate = item.CreatedDate,
        //        //        RfqValidDate = item.RFQValidDate,
        //        //    }).ToList();
        //        //    revision.Select(x => new RFQMasterModel()
        //        //    {
        //        //        RfqMasterId=item.RFQMaster.RfqMasterId,
        //        //        RfqUniqueNo=item.RFQMaster.RFQUniqueNo,
        //        //        RfqNo=item.RFQMaster.RFQNo
        //        //    }).ToList();

        //        //}
        //        return model;
        //    }
        //    catch (Exception)
        //    {

        //        throw;
        //    }
        //}
        //public async Task<RFQMasterDataModel> GetAllRFQs()
        //{
        //    RFQMasterDataModel rfqitems = new RFQMasterDataModel();
        //    //var rfqs = from x in obj.RFQMasters where x.DeleteFlag=="false"  select x;
        //    try
        //    {
        //        var rfqs = obj.RFQMasters.Where(x => x.DeleteFlag == false).Select(x => x).ToList();
        //        //var rfqs = obj.RFQMasters.SqlQuery("select * from RFQMaster where DeleteFlag=0");
        //        // var rfqs = obj.RFQMasters.Where(x => x.DeleteFlag = false).ToList();
        //        //List<RFQMasterModel> rfqs = (from x in obj.RFQMasters where x.DeleteFlag == false select x)
        //        RFQMasterModel model = new RFQMasterModel();
        //        foreach (var item in rfqs)
        //        {
        //            model.Created = item.CreatedDate;
        //            model.CreatedBy = item.CreatedBy;
        //            model.RfqMasterId = item.RfqMasterId;
        //            model.RfqNo = item.RFQNo;
        //            model.RfqUniqueNo = item.RFQUniqueNo;
        //            model.VendorId = item.VendorId;
        //            rfqitems.RFQlist.Add(model);
        //        }
        //        return rfqitems;
        //    }
        //    catch (Exception ex)
        //    {

        //        throw;
        //    }

        //}
        ////public async Task<RFQMasterDataModel> GetAllRFQs()
        //{
        //    RFQMasterDataModel rfqitems = new RFQMasterDataModel();
        //    List< RFQMasterModel> rfqs = new List< RFQMasterModel>();
        //    //var rfqs = from x in obj.RFQMasters where x.DeleteFlag=="false"  select x;
        //    try
        //    {
        //         rfqs = (from x in obj.RFQMasters
        //                    where x.DeleteFlag == false
        //                    select new RFQMasterModel
        //                    {
        //                       RfqMasterId=x.RfqMasterId,
        //                       RfqUniqueNo=x.RFQUniqueNo,
        //                       RfqNo=x.RFQNo,
        //                       VendorId=x.VendorId,
        //                       CreatedBy=x.CreatedBy,
        //                       Created=x.CreatedDate
        //                    }).ToList();
        //        rfqitems.RFQlist.Add(rfqs);
        //    }
        //    catch (Exception ex)
        //    {

        //        throw;
        //    }

        //}
        public async Task<List<RfqRevisionModel>> GetAllrevisionRFQs()
        {
            List<RfqRevisionModel> rfq = new List<RfqRevisionModel>();
            try
            {
                return (from x in obj.RFQRevisions_N
                        where x.DeleteFlag == false
                        select new RfqRevisionModel()
                        {
                            RfqRevisionId = x.rfqRevisionId,
                            RfqMasterId = x.rfqMasterId,
                            CreatedBy = x.CreatedBy,
                            CreatedDate = x.CreatedDate,
                            RfqRevisionNo = x.RevisionNo,
                            RfqValidDate = x.RFQValidDate
                        }).ToList();

            }

            catch (Exception ex)
            {

                throw;
            }

        }
        public async Task<RFQMasterModel> GetRFQById(int masterID)
        {
            RFQMasterModel model = new RFQMasterModel();
            RFQMaster rfq = new RFQMaster();
            RFQItem rfqitem = new RFQItem();
            try
            {
                rfq = obj.RFQMasters.Where(x => x.RfqMasterId == masterID).Include(x => x.RFQRevisions_N).SingleOrDefault<RFQMaster>();
                //rfq.RFQRevisions = obj.RFQRevisions.Where(x => x.rfqMasterId == id).Include(x=>x.RFQItems).ToList<RFQRevision>();
                if (rfq != null)
                {
                    model.RfqMasterId = rfq.RfqMasterId;
                    model.RfqNo = rfq.RFQNo;
                    model.RfqUniqueNo = rfq.RFQUniqueNo;
                    model.VendorId = rfq.VendorId;
                    model.CreatedBy = rfq.CreatedBy;


                    foreach (var item in rfq.RFQRevisions_N)
                    {
                        RfqRevisionModel RfqRevisionModel = new RfqRevisionModel();
                        RfqRevisionModel.RfqMasterId = item.rfqMasterId;
                        RfqRevisionModel.RfqRevisionId = item.rfqRevisionId;
                        RfqRevisionModel.RfqValidDate = item.RFQValidDate;
                        RfqRevisionModel.CreatedBy = item.CreatedBy;
                        RfqRevisionModel.CreatedDate = item.CreatedDate;
                        RfqRevisionModel.salesTax = item.SalesTax;
                        RfqRevisionModel.PaymentTermDays = item.PaymentTermDays;
                        RfqRevisionModel.freight = item.Freight;
                        RfqRevisionModel.Insurance = item.Insurance;
                        RfqRevisionModel.IsDeleted = item.DeleteFlag;
                        RfqRevisionModel.DeliveryMaxWeeks = item.DeliveryMaxWeeks;
                        RfqRevisionModel.DeliveryMinWeeks = item.DeliveryMinWeeks;
                        RfqRevisionModel.CustomsDuty = item.CustomsDuty;
                        RfqRevisionModel.BankGuarantee = item.BankGuarantee;
                        RfqRevisionModel.ExciseDuty = item.ExciseDuty;

                        model.Revision.Add(RfqRevisionModel);


                        //var result = obj.RFQItems.Where(x => x.RFQRevisionId == RfqRevisionModel.RfqRevisionId).ToList<RFQItem>();
                        //if (result != null)
                        //{
                        //    RfqItemModel rfqs = new RfqItemModel();
                        //    foreach (RFQItem items in item.RFQItems)
                        //    {
                        //        rfqs.RFQItemID = items.RFQItemsId;
                        //        rfqs.HSNCode = items.HSNCode;
                        //        rfqs.RFQRevisionId = items.RFQRevisionId;
                        //        rfqs.MRPItemsDetailsID = items.MPRItemDetailsid;
                        //        rfqs.QuotationQty = items.QuotationQty;
                        //        rfqs.VendorModelNo = items.VendorModelNo;
                        //        rfqs.RequsetRemarks = items.RequestRemarks;

                        //        RfqRevisionModel.rfqitem.Add(rfqs);
                        //    }

                        //}

                    }
                }
                //var gets = from x in obj.RFQMasters where x.RfqMasterId == id select x;

                return model;
            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                           eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                               ve.PropertyName, ve.ErrorMessage);
                    }
                }
                throw;
            }

        }
        public statuscheckmodel DeleteRfqById(int rfqmasterid)
        {
            statuscheckmodel status = new statuscheckmodel();
            var domainentity = obj.Set<RFQMaster>().Where(x => x.RfqMasterId == rfqmasterid && x.DeleteFlag == false).Include(x => x.RFQRevisions_N).SingleOrDefault();

            if (domainentity != null)
            {
                domainentity.DeleteFlag = true;
                obj.SaveChanges();
                var revision = obj.RFQRevisions.Where(l => l.rfqMasterId == domainentity.RfqMasterId && l.DeleteFlag == false).ToList();
                if (revision != null)
                {
                    foreach (var item in revision)
                    {
                        item.DeleteFlag = true;
                        obj.SaveChanges();
                    }

                    var ids = revision.Select(x => x.rfqRevisionId).ToList();
                    var itemlist = obj.RFQItems.Where(x => ids.Contains(x.RFQRevisionId) && x.DeleteFlag == false).ToList();
                    if (itemlist != null)
                    {
                        foreach (var item in itemlist)
                        {
                            item.DeleteFlag = true;
                            obj.SaveChanges();
                        }
                    }
                }
            }
            return status;
        }

        //public async Task<statusmodel> UpdateRfqRevision(RFQMasterModel model)
        //{
        //    statusmodel status = new statusmodel();
        //    var result = obj.RFQMasters.Where(x => x.RfqMasterId == model.RfqMasterId).Include(x=>x.RFQRevisions).FirstOrDefault<RFQMaster>();
        //    try
        //    {
        //        if (result!=null)
        //        {
        //            foreach (var item in model.Revision)
        //            {
        //                // var revision = new RFQRevision();
        //                var existing = result.RFQRevisions.Where(x => x.rfqMasterId == item.RfqMasterId).ToList<RFQRevision>();
        //                if (existing!=null)
        //                {
        //                    obj.Entry(existing).CurrentValues.SetValues(item);
        //                }

        //                //revision.SalesTax = item.salesTax;
        //                //revision.RFQValidDate = item.RfqValidDate;
        //                //revision.CreatedBy = item.CreatedBy;
        //                //revision.PaymentTermDays = item.PaymentTermDays;
        //                //revision.DeliveryMaxWeeks = item.DeliveryMaxWeeks;
        //                //revision.DeliveryMinWeeks = item.DeliveryMinWeeks;
        //                //revision.BankGuarantee = item.BankGuarantee;
        //                //revision.ExciseDuty = item.ExciseDuty;
        //                //revision.CustomsDuty = item.CustomsDuty;
        //                //revision.Insurance = item.Insurance;
        //                //revision.Freight = item.freight;
        //                //revision.PackingForwarding = item.PackingForwading;
        //                //revision.CreatedDate = item.CreatedDate;
        //            }

        //            await obj.SaveChangesAsync();
        //        }
        //        status.Sid = model.RfqMasterId;
        //        return  status;
        //    }
        //    catch (Exception ex)
        //    {

        //        throw;
        //    }
        //}
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
                return status;
            }
            catch (Exception ex)
            {

                throw;
            }
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
                return status;
            }
            catch (Exception ex)
            {

                throw;
            }
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
                return status;
            }
            catch (Exception ex)
            {
                throw;
            }
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


                return rfq;
            }

            catch (Exception ex)
            {
                throw;
            }
        }

        //public async Task<List<RfqItemModel>> GetItemsByRevisionId(int revisionid)
        //{
        //    List<RfqItemModel> rfq = new List<RfqItemModel>();
        //    try
        //    {
        //        return (from x in vscm.RemoteRFQItems_N
        //                where x.RFQRevisionId == revisionid
        //                select new RfqItemModel
        //                {
        //                    HSNCode = x.HSNCode,
        //                    QuotationQty = x.QuotationQty,
        //                    RFQRevisionId = x.RFQRevisionId,
        //                    VendorModelNo = x.VendorModelNo,
        //                    RequsetRemarks = x.RequestRemarks,
        //                    RFQItemID = x.RFQItemsId,
        //                    ItemName = x.ItemName,
        //                    ItemDescription = x.ItemDescription
        //                }).ToList();

        //    }

        //    catch (Exception ex)
        //    {
        //        throw;
        //    }
        //}

        //public RFQMasterModel getItemsbyRevisionno(string id)
        // {
        //     RFQMasterModel master = new RFQMasterModel();
        //     try
        //     {
        //         var masters = obj.RFQMasters.Where(x => x.RFQNo == id && x.DeleteFlag == false).Include(x => x.RFQRevisions).Select(x => x).SingleOrDefault();
        //         if (masters!=null)
        //         {

        //         }
        //     }
        //     catch (Exception)
        //     {

        //         throw;
        //     }
        // }
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
                return status;
            }
            catch (Exception ex)
            {
                throw;
            }
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
                return staus;
            }
            catch (Exception ex)
            {

                throw;
            }
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
                return status;
            }
            catch (Exception ex)
            {
                throw;
            }
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

        //public statusmodel InsertDocument(RfqDocumentsModel model)
        //{
        //    if (true)
        //    {
        //        try
        //        {
        //            string PhotoPath = Convert.ToString(ConfigurationManager.AppSettings["ImagePath"]);
        //            RFQDocument newObj = new RFQDocument();
        //            newObj.DocumentName = model.DocumentName;
        //            newObj.DocumentType = model.DocumentType;
        //            newObj.Path = model.Path;
        //            newObj.UploadedBy = model.UploadedBy;
        //            newObj.uploadedDate = model.UploadedDate;
        //            newObj.Status = model.Status;
        //            newObj.StatusBy = model.StatusBy;
        //            newObj.StatusDate = model.Statusdate;

        //            if (String.IsNullOrEmpty(newObj.Path))
        //            {

        //            }
        //            else
        //            {
        //               // string startingFilePath = PhotoPath;

        //                string FilePath = SaveImage(newObj.Path, startingFilePath, newObj.DocumentName);

        //                FileInfo fInfo = new FileInfo(FilePath);

        //                newObj.Content = fInfo.Name;
        //            }



        //            return Request.CreateResponse(HttpStatusCode.Created, newArticle);
        //        }
        //        catch (Exception ex)
        //        {
        //            return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
        //        }
        //    }
        //    else
        //    {
        //        return Request.CreateResponse(HttpStatusCode.BadRequest, ModelState);
        //    }
        //}
        private string SaveImage(string base64, string FilePath, string ImageName)
        {
            //Get the file type to save in
            var FilePathWithExtension = "";
            string localBase64 = "";

            if (base64.Contains("data:image/jpeg;base64,"))
            {
                FilePathWithExtension = FilePath + ImageName + ".jpg";
                localBase64 = base64.Replace("data:image/jpeg;base64,", "");
            }
            else if (base64.Contains("data:image/png;base64,"))
            {
                FilePathWithExtension = FilePath + ImageName + ".png";
                localBase64 = base64.Replace("data:image/png;base64,", "");
            }
            else if (base64.Contains("data:image/bmp;base64"))
            {
                FilePathWithExtension = FilePath + ImageName + ".bmp";
                localBase64 = base64.Replace("data:image/bmp;base64", "");
            }
            else if (base64.Contains("data:application/msword;base64,"))
            {
                FilePathWithExtension = FilePath + ImageName + ".doc";
                localBase64 = base64.Replace("data:application/msword;base64,", "");
            }
            else if (base64.Contains("data:application/vnd.openxmlformats-officedocument.wordprocessingml.document;base64,"))
            {
                FilePathWithExtension = FilePath + ImageName + ".docx";
                localBase64 = base64.Replace("data:application/vnd.openxmlformats-officedocument.wordprocessingml.document;base64,", "");
            }
            else if (base64.Contains("data:application/pdf;base64,"))
            {
                FilePathWithExtension = FilePath + ImageName + ".pdf";
                localBase64 = base64.Replace("data:application/pdf;base64,", "");
            }

            using (MemoryStream ms = new MemoryStream(Convert.FromBase64String(localBase64)))
            {
                using (FileStream fs = new FileStream(FilePathWithExtension, FileMode.Create, FileAccess.Write))
                {
                    //Create the specified directory if it does not exist
                    var photofolder = System.IO.Path.GetDirectoryName(FilePathWithExtension);
                    if (!Directory.Exists(photofolder))
                    {
                        Directory.CreateDirectory(photofolder);
                    }

                    ms.WriteTo(fs);
                    fs.Close();
                    ms.Close();
                }
            }

            return FilePathWithExtension;
        }
        public statuscheckmodel CommunicationAdd(RfqCommunicationModel model)
        {
            statuscheckmodel status = new statuscheckmodel();
            var revision = obj.RFQRevisions.Where(x => x.rfqRevisionId == model.RfqRevision.RfqRevisionId && x.DeleteFlag == false).Include(x => x.RFQItems).SingleOrDefault();
            var item = revision.RFQItems.Where(x => x.RFQItemsId == model.RfqItem.RFQItemID && x.DeleteFlag == false).Select(x => x);
            // var result=from x in obj.MPRRevisions where x.RevisionId==model.re
            if (item != null)
            {
                RFQCommunication communication = new RFQCommunication();
                communication.RemarksFrom = model.RemarksFrom;
                communication.RemarksTo = model.RemarksTo;
                communication.ReminderDate = model.ReminderDate;
                communication.SendEmail = model.SendEmail;
                communication.SetReminder = model.SetReminder;
                communication.RemarksDate = model.RemarksDate;
                communication.Remarks = model.Remarks;
                communication.RfqItemsId = model.RfqItem.RFQItemID;
                communication.RfqRevisionId = model.RfqRevision.RfqRevisionId;

                obj.RFQCommunications.Add(communication);
                obj.SaveChanges();
            }
            status.Sid = model.Rfqccid;
            //status.StatusMesssage = model.StatusMesssage;
            return status;
        }
        public async Task<statuscheckmodel> InsertCommunicationAgainstRevision(RfqCommunicationModel model)
        {
            statuscheckmodel status = new statuscheckmodel();
            try
            {
                vscm.Database.Connection.Open();
                RemoteRFQCommunication remotecomm = new RemoteRFQCommunication();
                if (model != null)
                {
                    remotecomm.RfqRevisionId = model.RfqRevisionId;
                    remotecomm.RemarksFrom = model.RemarksFrom;
                    remotecomm.RemarksTo = model.RemarksTo;
                    remotecomm.SendEmail = model.SendEmail;
                    remotecomm.SetReminder = model.SetReminder;
                    remotecomm.ReminderDate = model.ReminderDate;
                    remotecomm.RemarksDate = model.RemarksDate;
                    remotecomm.Remarks = model.Remarks;
                    vscm.RemoteRFQCommunications.Add(remotecomm);
                    vscm.SaveChanges();
                }
                int cid = remotecomm.RfqCCid;
                vscm.Database.Connection.Close();

                RFQCommunication localcomm = new RFQCommunication();
                obj.Database.Connection.Open();
                if (model != null)
                {
                    localcomm.RfqCCid = cid;
                    localcomm.RfqRevisionId = model.RfqRevisionId;
                    localcomm.RemarksFrom = model.RemarksFrom;
                    localcomm.RemarksTo = model.RemarksTo;
                    localcomm.SendEmail = model.SendEmail;
                    localcomm.SetReminder = model.SetReminder;
                    localcomm.ReminderDate = model.ReminderDate;
                    localcomm.RemarksDate = model.RemarksDate;
                    localcomm.DeleteFlag = false;
                    localcomm.Remarks = model.Remarks;
                    obj.RFQCommunications.Add(localcomm);
                    obj.SaveChanges();
                }
                obj.Database.Connection.Close();
                status.Sid = cid;
                return status;
            }
            catch (Exception ex)
            {

                throw;
            }
        }
        public async Task<RfqCommunicationModel> GetCommunicationByItemID(int itemid)
        {
            RfqCommunicationModel communication = new RfqCommunicationModel();
            try
            {
                var Remotecommunication = vscm.RemoteRFQCommunications.Where(x => x.RfqItemsId == itemid && x.DeleteFlag == false).FirstOrDefault();
                if (Remotecommunication != null)
                {
                    communication.Rfqccid = Remotecommunication.RfqCCid;
                    communication.RfqRevisionId = Remotecommunication.RfqRevisionId;
                    communication.RemarksFrom = Remotecommunication.RemarksFrom;
                    communication.Remarks = Remotecommunication.Remarks;
                    communication.RemarksTo = Remotecommunication.RemarksTo;
                    communication.SendEmail = Remotecommunication.SendEmail;
                    communication.SetReminder = Remotecommunication.SetReminder;
                    communication.ReminderDate = Remotecommunication.ReminderDate;
                    communication.RemarksDate = Remotecommunication.RemarksDate;
                }
                return communication;
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
                throw;
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
        public async Task<VendormasterModel> GetvendorById(int id)
        {
            VendormasterModel vendor = new VendormasterModel();
            try
            {
                var data = obj.VendorMasters.Where(x => x.Vendorid == id && x.Deleteflag == false).SingleOrDefault();
                if (data != null)
                {
                    vendor.ContactNo = data.ContactNo;
                    vendor.Emailid = data.Emailid;
                    vendor.VendorCode = data.VendorCode;
                    vendor.VendorName = data.VendorName;
                }
                return vendor;
            }
            catch (Exception)
            {

                throw;
            }
        }
        public async Task<statuscheckmodel> InsertVendorterms(VendorRfqtermModel vendor)
        {
            statuscheckmodel status = new statuscheckmodel();
            try
            {
                if (vendor != null)
                {
                    var data = new VendorRFQTerm();
                    data.VendorTermsid = vendor.VendorTermsid;
                    data.VendorID = vendor.VendorID;
                    data.Terms = vendor.Terms;
                    data.Indexno = vendor.Indexno;
                    obj.VendorRFQTerms.Add(data);
                    obj.SaveChanges();
                }
                return status;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<statuscheckmodel> UpdateRfqStatus(int id)
        {
            statuscheckmodel status = new statuscheckmodel();
            try
            {
                var data = obj.RFQStatus.Select(x => x.RfqStatusId).ToList();

                return status;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<List<RfqRevisionModel>> GetAllRFQs()
        {
            throw new NotImplementedException();
        }

        public List<RfqItemModel> InsertRfqItemInfo(RfqItemModel model)
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
                    var Remotedata = new RemoteRFQItem();
                    var items = new RemoteRFQItemsInfo();
                    int bomid = 0;
                    foreach (var item in model.iteminfo)
                    {

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
                        bomid = info.RFQVendorbomItemId;
                        List <RemoteRfqVendorBOM> itemsforupdate = vscm.RemoteRfqVendorBOMs.Where(li => li.RfqItemsId == item.RFQItemsId && li.DeleteFlag==false).ToList<RemoteRfqVendorBOM>();
                       
                        foreach (var data1 in itemsforupdate)
                        {
                            double? subtotalprice = 0;
                            subtotalprice = Convert.ToDouble(data1.Qty) * Convert.ToDouble(data1.UnitPrice);
                            if (Convert.ToInt32(data1.Discount)!=0)
                            {
                              double? Discount = subtotalprice - Convert.ToDouble(data1.Discount);
                                subtotalprice = Discount;
                            }
                             if (Convert.ToInt32(data1.DiscountPercentage) !=0)
                            {
                               double? DiscountPercentage = (subtotalprice - subtotalprice * Convert.ToDouble(data1.DiscountPercentage)/100);
                                subtotalprice = DiscountPercentage;
                            }
                             if (Convert.ToInt32(data1.FreightAmount)!=0)
                            {
                                double? FreightAmount = subtotalprice - Convert.ToDouble(data1.FreightAmount);
                                subtotalprice = FreightAmount;
                            }
                             if (Convert.ToInt32(data1.FreightPercentage) != 0)
                            {
                                double? FreightPercentage= (subtotalprice - subtotalprice * Convert.ToDouble(data1.FreightPercentage) / 100);
                                subtotalprice = FreightPercentage;
                            }
                             if (Convert.ToInt32(data1.PFAmount) != 0)
                            {
                                double? PFAmount = subtotalprice - Convert.ToDouble(data1.PFAmount);
                                subtotalprice = PFAmount;
                            }
                             if (Convert.ToInt32(data1.PFPercentage) != 0)
                            {

                                double? PFPercentage = (subtotalprice* Convert.ToDouble(data1.PFPercentage) / 100);
                                subtotalprice += PFPercentage;
                            }
                            totalprice += subtotalprice; 
                               
                        }
                        RemoteRFQItemsInfo_N itemsinfo = vscm.RemoteRFQItemsInfo_N.Where(li => li.RFQItemsId == item.RFQItemsId).FirstOrDefault();
                        if(itemsinfo!=null)
                        {
                            itemsinfo.UnitPrice = Convert.ToDecimal(totalprice);
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
                    }
                    foreach (var item in model.iteminfo)
                    {

                        var info = new RfqVendorBOM();
                        info.RFQVendorbomItemId = bomid;
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
                        info.PFPercentage = model.PFPercentage;
                        info.PFAmount = model.PFAmount;
                        info.PFPercentage = model.PFPercentage;
                        info.FreightAmount = model.FreightAmount;
                        info.CustomDuty = model.CustomDuty;
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
                        obj.RfqVendorBOMs.Add(info);
                        obj.SaveChanges();
                       
                    }

                        foreach (var item in model.iteminfo)
                    {
                       RFQItemsInfo_N itemsinfo = obj.RFQItemsInfo_N.Where(li => li.RFQItemsId == item.RFQItemsId).FirstOrDefault();
                        if(itemsinfo!=null)
                        {
                            itemsinfo.UnitPrice = Convert.ToDecimal(totalprice);
                            obj.SaveChanges();
                        }
                        else
                        {
                            var info = new RFQItemsInfo_N();
                            info.RFQSplitItemId = spiltitemid;
                            info.RFQItemsId = item.RFQItemsId;
                            info.Qty = 1;//item.Quantity;
                            info.UOM = item.UOM;
                            info.UnitPrice = Convert.ToDecimal(totalprice);
                            info.DiscountPercentage = item.DiscountPercentage;
                            info.Discount = item.Discount;
                            info.CurrencyId = item.CurrencyID;
                            info.CurrencyValue = item.CurrencyValue;
                            info.Remarks = item.Remarks;
                            info.DeliveryDate = item.DeliveryDate;
                            info.DeleteFlag = false;

                            obj.RFQItemsInfo_N.Add(info);
                            obj.SaveChanges();
                        }
                        eachobj.errormsg = "Success";
                    }
                    var rfqitems = obj.MPRRfqItems.Where(x => x.RfqItemsid == model.RFQItemID).FirstOrDefault();
                    var infos=new MPRRfqItemInfo();
                    if (rfqitems != null)
                         infos = obj.MPRRfqItemInfos.Where(li => li.MPRRFQitemId == rfqitems.MPRRFQitemId).FirstOrDefault();
                    if (infos.Mprrfqsplititemid!= 0)
                    {
                        infos.rfqsplititemid = spiltitemid;
                        infos.MPRRFQitemId = rfqitems.MPRRFQitemId;
                        obj.SaveChanges();
                    }
                    _listobj.Add(eachobj);

                }
                return _listobj;

            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public List<RfqItemModel> editRfqItemInfo(RfqItemModel model)
        {
            statuscheckmodel status = new statuscheckmodel();
            RfqItemModel eachobj = new RfqItemModel();
            List<RfqItemModel> _listobj = new List<RfqItemModel>();
            int spiltitemid = 0;
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
                        rfqremoteitem.PFAmount = model.PFAmount;
                        rfqremoteitem.PFPercentage = model.PFPercentage;
                        rfqremoteitem.FreightAmount = model.FreightAmount;
                        rfqremoteitem.FreightPercentage = model.FreightPercentage;
                        rfqremoteitem.CustomDuty = model.CustomDuty;
                       
                        rfqremoteitem.taxInclusiveOfDiscount = model.taxInclusiveOfDiscount;
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
                            remoteiteminfo.DeleteFlag = false;
                            // vscm.RemoteRFQItemsInfoes.Add(remoteinfo);
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
                        rfqitem.PFAmount = model.PFAmount;
                        rfqitem.PFPercentage = model.PFPercentage;
                        rfqitem.FreightAmount = model.FreightAmount;
                        rfqitem.FreightPercentage = model.FreightPercentage;
                        rfqitem.CustomDuty = model.CustomDuty;

                        rfqitem.taxInclusiveOfDiscount = model.taxInclusiveOfDiscount;
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

                            //obj.RFQItemsInfo_N.Add(inforrfq);
                            obj.SaveChanges();
                            eachobj.errormsg = "Success";
                        }
                      
                    }
                    _listobj.Add(eachobj);

                }
                return _listobj;

            }
            catch (Exception ex)
            {
                throw;
            }
        }



        //delete
        public async Task<statuscheckmodel> DeleteRfqIteminfoByid(int id,int rfqitemid)
        {
            statuscheckmodel status = new statuscheckmodel();
            try
            {
                double? totalprice = 0;
                //vscm.Database.Connection.Open();
                var Remotedata = vscm.RemoteRFQItemsInfo_N.Where(x => x.RFQSplitItemId == id && x.DeleteFlag == false).FirstOrDefault();
                var remotebomdata = vscm.RemoteRfqVendorBOMs.Where(x => x.RfqItemsId == rfqitemid && x.DeleteFlag==false).ToList();
                var bomdata = obj.RfqVendorBOMs.Where(x => x.RfqItemsId == rfqitemid && x.DeleteFlag == false).ToList();
                if (Remotedata != null)
                {

                    Remotedata.DeleteFlag = true;
                    vscm.SaveChanges();
                }
                if(remotebomdata!=null)
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
                    iteminfo.DeliveryDate = item.DeliveryDate;
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
            RemoteRFQRevisions_N rev = vscm.RemoteRFQRevisions_N.Include(x => x.RemoteRFQItems_N).Where(li => li.rfqRevisionId == revisionId && li.DeleteFlag==false).FirstOrDefault();
            rev.RemoteRFQItems_N = rev.RemoteRFQItems_N.Where(li=>li.DeleteFlag==false).ToList();
            foreach (RemoteRFQItems_N item in rev.RemoteRFQItems_N)
            {
                item.RemoteRFQItemsInfo_N = vscm.RemoteRFQItemsInfo_N.Where(li => li.RFQItemsId == item.RFQItemsId && li.DeleteFlag==false).ToList();
                item.RemoteRFQDocuments = vscm.RemoteRFQDocuments.Where(li => li.rfqItemsid == item.RFQItemsId && li.rfqRevisionId == rev.RevisionNo  && li.DocumentType.Equals(1) && li.DeleteFlag==false).ToList();
                item.RemoteRfqVendorBOMs = vscm.RemoteRfqVendorBOMs.Where(li => li.RfqItemsId==item.RFQItemsId).ToList();
                item.RemoteRfqVendorBOMs = item.RemoteRfqVendorBOMs.Where(li => li.DeleteFlag == false).ToList();
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
                var data = obj.CurrencyMasters.Where(x => x.CurrencyId == model.CurrenyId && x.DeleteFlag == false).FirstOrDefault();
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
                    CurrenyId = x.CurrencyId,
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
        public async Task<List<MPRApproversViewModel>> GetAllMPRApprovers()
        {
            List<MPRApproversViewModel> model = new List<MPRApproversViewModel>();
            try
            {
                var data = obj.MPRApproversViews.SqlQuery("select * from MPRApproversView");
                model = data.Select(x => new MPRApproversViewModel()
                {
                    EmployeeNo = x.EmployeeNo,
                    Name = x.Name,
                    //DeactivatedBy = x.DeactivatedBy,
                    //DeactivatedOn = x.DeactivatedOn
                }).ToList();
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
        public async Task<List<V_RFQList>> GetRfqByVendorId(rfqFilterParams obj)
         {
            List<RFQMasterModel> model = new List<RFQMasterModel>();
            try
            {
                List<V_RFQList> data = null;
                int vendorid = Convert.ToInt32(obj.VendorId);
                if ((obj.typeOfFilter=="other")) {
                    data = new List<V_RFQList>();
                     data = vscm.V_RFQList.Where(x => x.VendorId == vendorid && x.DeleteFlag == false && x.VendorVisiblity == true).ToList();

                }
                else if(obj.typeOfFilter=="true")
                {
                    data = vscm.V_RFQList.Where(x => x.VendorId == vendorid  && x.DeleteFlag == false && x.VendorVisiblity == true && (x.RFQValidDate>=obj.FromDate && x.RFQValidDate <= obj.ToDate) ||  x.RFQNo == obj.RFQNo).OrderByDescending(x => x.MPRRevisionId).ToList();
                }
                else if(obj.typeOfFilter=="false")
                {
                    data = vscm.V_RFQList.Where(x => x.VendorId == vendorid  && x.DeleteFlag == false && x.VendorVisiblity == true && (x.CreatedDate >= obj.FromDate && x.CreatedDate <= obj.ToDate) || x.RFQNo == obj.RFQNo).OrderByDescending(x => x.MPRRevisionId).ToList();
                    //data = vscm.RemoteRFQMasters.Where(x => x.VendorId == vendorid && x.RFQNo == obj.RFQNo && x.DeleteFlag == false && x.VendorVisiblity == true).Include(x => x.RemoteRFQRevisions_N.Select(li => li.CreatedDate >= obj.FromDate && li.CreatedDate <= obj.ToDate)).OrderByDescending(x => x.MPRRevisionId).ToList();
                }
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
                return data;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public List<RFQListView> getRFQList(rfqFilterParams rfqfilterparams)
        {
            List<RFQListView> mprRevisionDetails;
            using (var db = new YSCMEntities()) //ok
            {
                obj.Configuration.ProxyCreationEnabled = false;
                if (!string.IsNullOrEmpty(rfqfilterparams.typeOfFilter))
                    mprRevisionDetails = obj.RFQListViews.Where(li => li.RFQValidDate <= rfqfilterparams.ToDate && li.RFQValidDate >= rfqfilterparams.FromDate).ToList();
                else
                    mprRevisionDetails = obj.RFQListViews.Where(li => li.CreatedDate <= rfqfilterparams.ToDate && li.CreatedDate >= rfqfilterparams.FromDate).ToList();

            }
            return mprRevisionDetails;
        }

        List<VendorRegistrationModel>InsertVendordata(VendorRegistrationModel model)
        {
            try
            {
                List<VendorRegistrationModel> obj = new List<VendorRegistrationModel>();
                VendorRegistrationModel objid = new VendorRegistrationModel();
                if (model != null)
                {

                    vscm.Database.Connection.Open();
                    // var Remotedata = new RemoteVendorRegisterMaster();
                    int id = 0;
                    if (model.UniqueId == 0)
                    {
                        var Remotedata = new RemoteVendorRegisterMaster();
                        // Remotedata.Id = model.UniqueId;
                        Remotedata.Onetimevendor = model.Onetimevendor;
                        Remotedata.EvaluationRequired = model.EvaluationRequired;
                        Remotedata.VendorNoInSAP = model.VendorNoInSAP;
                        Remotedata.RequestedOn = model.RequestedOn;
                        Remotedata.VendorName = model.VendorName;
                        Remotedata.Address = model.VendorAddress;
                        Remotedata.PostalCode = model.PostalCode;
                        Remotedata.City = model.City;
                        Remotedata.LocalBranchOffice = model.LocalBranchOffice;
                        Remotedata.PhoneAndExtn = model.PhoneAndExtn;
                        Remotedata.Fax = model.Fax;
                        Remotedata.ContactPerson = model.ContactPerson;
                        Remotedata.Phone = model.Phone;
                        Remotedata.GSTNo = model.GSTNo;
                        Remotedata.NatureofBusiness = model.NatureofBusiness;
                        Remotedata.PANNo = model.PANNo;
                        Remotedata.CINNo = model.CINNo;
                        Remotedata.TanNo = model.TanNo;
                        Remotedata.RequestedOn = DateTime.Now;
                        Remotedata.PaymentTerms = model.PaymentTerms;
                        Remotedata.Address = model.street;
                        Remotedata.City = model.City;
                        Remotedata.PhoneAndExtn = model.PhoneAndExtn;
                        Remotedata.TanNo = model.TanNo;
                        Remotedata.Onetimevendor = model.Onetimevendor;
                        Remotedata.EvaluationRequired = model.EvaluationRequired;
                        vscm.RemoteVendorRegisterMasters.Add(Remotedata);
                        vscm.SaveChanges();
                        id = Remotedata.Id;
                        objid.UniqueId = Remotedata.Id;
                        obj.Add(objid);
                    }
                    else
                    {
                        RemoteVendorRegisterMaster Remotedata = vscm.RemoteVendorRegisterMasters.Where(li => li.Id == model.UniqueId).FirstOrDefault<RemoteVendorRegisterMaster>();
                        Remotedata.Id = model.UniqueId;
                        Remotedata.Onetimevendor = model.Onetimevendor;
                        Remotedata.EvaluationRequired = model.EvaluationRequired;
                        Remotedata.VendorNoInSAP = model.VendorNoInSAP;
                        Remotedata.RequestedOn = model.RequestedOn;
                        Remotedata.VendorName = model.VendorName;
                        Remotedata.Address = model.VendorAddress;
                        Remotedata.PostalCode = model.PostalCode;
                        Remotedata.City = model.City;
                        Remotedata.LocalBranchOffice = model.LocalBranchOffice;
                        Remotedata.PhoneAndExtn = model.PhoneAndExtn;
                        Remotedata.Fax = model.Fax;
                        Remotedata.ContactPerson = model.ContactPerson;
                        Remotedata.Phone = model.Phone;
                        Remotedata.GSTNo = model.GSTNo;
                        Remotedata.NatureofBusiness = model.NatureofBusiness;
                        Remotedata.PANNo = model.PANNo;
                        Remotedata.CINNo = model.CINNo;
                        Remotedata.TanNo = model.TanNo;
                        Remotedata.RequestedOn = DateTime.Now;
                        Remotedata.PaymentTerms = model.PaymentTerms;
                        Remotedata.Address = model.street;
                        Remotedata.City = model.City;
                        Remotedata.PhoneAndExtn = model.PhoneAndExtn;
                        Remotedata.TanNo = model.TanNo;
                        Remotedata.Onetimevendor = model.Onetimevendor;
                        Remotedata.EvaluationRequired = model.EvaluationRequired;
                        vscm.SaveChanges();
                        id = Remotedata.Id;
                        objid.UniqueId = Remotedata.Id;
                        obj.Add(objid);
                    }


                    if (id != 0)
                    {

                        //var remotedataforbankdetails = new RemoteBankDetailsForVendor();
                        RemoteBankDetailsForVendor remotedataforbankdetail = vscm.RemoteBankDetailsForVendors.Where(li => li.VendorId == id).FirstOrDefault<RemoteBankDetailsForVendor>();
                        if (remotedataforbankdetail == null)
                        {
                            var remotedataforbankdetails = new RemoteBankDetailsForVendor();
                            remotedataforbankdetails.IFSCCode = model.IFSCCode;
                            remotedataforbankdetails.BankDetails = model.BankDetails;
                            remotedataforbankdetails.BankerName = model.BankerName;
                            remotedataforbankdetails.AccNo = model.AccNo;
                            remotedataforbankdetails.AccountHolderName = model.AccountHolderName;
                            remotedataforbankdetails.VendorId = id;
                            remotedataforbankdetails.LocationOrBranch = model.LocationOrBranch;
                            vscm.RemoteBankDetailsForVendors.Add(remotedataforbankdetails);
                            vscm.SaveChanges();
                        }
                        else
                        {
                            //var remotedataforbankdetail = new RemoteBankDetailsForVendor();
                            remotedataforbankdetail.IFSCCode = model.IFSCCode;
                            remotedataforbankdetail.BankDetails = model.BankDetails;
                            remotedataforbankdetail.BankerName = model.BankerName;
                            remotedataforbankdetail.AccNo = model.AccNo;
                            remotedataforbankdetail.AccountHolderName = model.AccountHolderName;
                            remotedataforbankdetail.VendorId = id;
                            remotedataforbankdetail.LocationOrBranch = model.LocationOrBranch;
                            // vscm.RemoteBankDetailsForVendors.Add(remotedataforbankdetails);
                            vscm.SaveChanges();
                        }


                        //  var remotedataforDocumentDetails = new RemoteVendorRegisterDocumentDetail();

                        RemoteVendorRegisterDocumentDetail remotedataforDocumentDetails = vscm.RemoteVendorRegisterDocumentDetails.Where(li => li.VendorId == id).FirstOrDefault<RemoteVendorRegisterDocumentDetail>();
                        if (remotedataforDocumentDetails == null)
                        {
                            var remotedataforDocumentDetail = new RemoteVendorRegisterDocumentDetail();
                            remotedataforDocumentDetail.VendorId = id;
                            remotedataforDocumentDetail.PhysicalPath = model.PhysicalPath;
                            remotedataforDocumentDetail.DocumentationTypeId = model.DocumentationTypeId;
                            vscm.RemoteVendorRegisterDocumentDetails.Add(remotedataforDocumentDetail);
                            vscm.SaveChanges();
                        }
                        else
                        {
                            remotedataforDocumentDetails.VendorId = id;
                            remotedataforDocumentDetails.PhysicalPath = model.PhysicalPath;
                            remotedataforDocumentDetails.DocumentationTypeId = model.DocumentationTypeId;
                            //vscm.RemoteVendorRegisterDocumentDetails.Add(remotedataforDocumentDetails);
                            vscm.SaveChanges();
                        }

                    }
                    else
                    {

                    }


                }
                return obj;
            }
            catch (Exception ex)
            {
                throw;
            }

            //throw new NotImplementedException();
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

        public List<VendorRegistrationModel> SaveVendorDetails(VendorRegistrationModel model)
        {


            try
            {
                List<VendorRegistrationModel> RegistrationModelobj = new List<VendorRegistrationModel>();
                VendorRegistrationModel objid = new VendorRegistrationModel();
                if (model != null)
                {

                    //vscm.Database.Connection.Open();
                    // var Remotedata = new RemoteVendorRegisterMaster();
                    int id = 0;
                    int bankdetailsid = 0;
                    if (model.VendorCode == "")
                    {
                        var Remotedata = new RemoteVendorRegisterMaster();
                        // Remotedata.Id = model.UniqueId;
                        Remotedata.Onetimevendor = model.Onetimevendor;
                        Remotedata.EvaluationRequired = model.EvaluationRequired;
                        Remotedata.VendorNoInSAP = model.VendorNoInSAP;
                        Remotedata.RequestedOn = model.RequestedOn;
                        Remotedata.VendorName = model.VendorName;
                        Remotedata.Address = model.VendorAddress;
                        Remotedata.PostalCode = model.PostalCode;
                        Remotedata.City = model.City;
                        Remotedata.LocalBranchOffice = model.LocalBranchOffice;
                        Remotedata.PhoneAndExtn = model.PhoneAndExtn;
                        Remotedata.Fax = model.Fax;
                        Remotedata.ContactPerson = model.ContactPerson;
                        Remotedata.Phone = model.Phone;
                        Remotedata.GSTNo = model.GSTNo;
                        Remotedata.NatureofBusiness = model.NatureofBusiness;
                        Remotedata.PANNo = model.PANNo;
                        Remotedata.CINNo = model.CINNo;
                        Remotedata.TanNo = model.TanNo;
                        Remotedata.RequestedOn = DateTime.Now;
                        Remotedata.PaymentTerms = model.PaymentTerms;
                        Remotedata.Address = model.street;
                        Remotedata.City = model.City;
                        Remotedata.PhoneAndExtn = model.PhoneAndExtn;
                        Remotedata.TanNo = model.TanNo;
                        Remotedata.Onetimevendor = model.Onetimevendor;
                        Remotedata.EvaluationRequired = model.EvaluationRequired;

                        vscm.RemoteVendorRegisterMasters.Add(Remotedata);
                        vscm.SaveChanges();
                        id = Remotedata.Id;
                        objid.UniqueId = Remotedata.Id;
                        RegistrationModelobj.Add(objid);
                    }
                    else
                    {
                        RemoteVendorRegisterMaster Remotedata = vscm.RemoteVendorRegisterMasters.Where(li => li.Vendorid == model.VendorId).FirstOrDefault<RemoteVendorRegisterMaster>();
                        if (Remotedata != null)
                        {
                           // Remotedata.Id = model.UniqueId;
                            Remotedata.Onetimevendor = model.Onetimevendor;
                            Remotedata.EvaluationRequired = model.EvaluationRequired;
                            Remotedata.VendorNoInSAP = model.VendorNoInSAP;
                            Remotedata.RequestedOn = model.RequestedOn;
                            Remotedata.VendorName = model.VendorName;
                            Remotedata.Address = model.VendorAddress;
                            Remotedata.PostalCode = model.PostalCode;
                            Remotedata.City = model.City;
                            Remotedata.LocalBranchOffice = model.LocalBranchOffice;
                            Remotedata.PhoneAndExtn = model.PhoneAndExtn;
                            Remotedata.Fax = model.Fax;
                            Remotedata.ContactPerson = model.ContactPerson;
                            Remotedata.Phone = model.Phone;
                            Remotedata.GSTNo = model.GSTNo;
                            Remotedata.NatureofBusiness = model.NatureofBusiness;
                            Remotedata.PANNo = model.PANNo;
                            Remotedata.CINNo = model.CINNo;
                            Remotedata.TanNo = model.TanNo;
                            Remotedata.RequestedOn = DateTime.Now;
                            Remotedata.PaymentTerms = model.PaymentTerms;
                            Remotedata.Address = model.street;
                            //Remotedata.City = model.City;
                            Remotedata.PhoneAndExtn = model.PhoneAndExtn;
                            Remotedata.TanNo = model.TanNo;
                            Remotedata.Onetimevendor = model.Onetimevendor;
                            Remotedata.EvaluationRequired = model.EvaluationRequired;

                            vscm.SaveChanges();
                            id = Remotedata.Id;
                            objid.UniqueId = Remotedata.Id;
                            RegistrationModelobj.Add(objid);
                        }
                    }


                    if (id != 0)
                    {

                        //var remotedataforbankdetails = new RemoteBankDetailsForVendor();
                        RemoteBankDetailsForVendor remotedataforbankdetail = vscm.RemoteBankDetailsForVendors.Where(li => li.VendorId == id).FirstOrDefault<RemoteBankDetailsForVendor>();
                        if (remotedataforbankdetail == null)
                        {
                            var remotedataforbankdetails = new RemoteBankDetailsForVendor();
                            remotedataforbankdetails.IFSCCode = model.IFSCCode;
                            remotedataforbankdetails.BankDetails = model.BankDetails;
                            remotedataforbankdetails.BankerName = model.BankerName;
                            remotedataforbankdetails.AccNo = model.AccNo;
                            remotedataforbankdetails.AccountHolderName = model.AccountHolderName;
                            remotedataforbankdetails.VendorId = id;
                            remotedataforbankdetails.LocationOrBranch = model.LocationOrBranch;
                            vscm.RemoteBankDetailsForVendors.Add(remotedataforbankdetails);
                            vscm.SaveChanges();
                            bankdetailsid = remotedataforbankdetails.Id;
                        }
                        else
                        {
                            //var remotedataforbankdetail = new RemoteBankDetailsForVendor();
                            remotedataforbankdetail.IFSCCode = model.IFSCCode;
                            remotedataforbankdetail.BankDetails = model.BankDetails;
                            remotedataforbankdetail.BankerName = model.BankerName;
                            remotedataforbankdetail.AccNo = model.AccNo;
                            remotedataforbankdetail.AccountHolderName = model.AccountHolderName;
                            remotedataforbankdetail.VendorId = id;
                            remotedataforbankdetail.LocationOrBranch = model.LocationOrBranch;
                            // vscm.RemoteBankDetailsForVendors.Add(remotedataforbankdetails);
                            vscm.SaveChanges();
                            bankdetailsid = remotedataforbankdetail.Id;
                        }

                    }
                    else
                    {

                    }
                    //yscm
                    
                    if (model.VendorCode == "")
                    {
                        var yscmdata = new VendorRegisterMaster();
                        // Remotedata.Id = model.UniqueId;
                        yscmdata.Onetimevendor = model.Onetimevendor;
                        yscmdata.EvaluationRequired = model.EvaluationRequired;
                        yscmdata.VendorNoInSAP = model.VendorNoInSAP;
                        yscmdata.RequestedOn = model.RequestedOn;
                        yscmdata.VendorName = model.VendorName;
                        yscmdata.Address = model.VendorAddress;
                        yscmdata.PostalCode = model.PostalCode;
                        yscmdata.City = model.City;
                        yscmdata.LocalBranchOffice = model.LocalBranchOffice;
                        yscmdata.PhoneAndExtn = model.PhoneAndExtn;
                        yscmdata.Fax = model.Fax;
                        yscmdata.ContactPerson = model.ContactPerson;
                        yscmdata.Phone = model.Phone;
                        yscmdata.GSTNo = model.GSTNo;
                        yscmdata.NatureofBusiness = model.NatureofBusiness;
                        yscmdata.PANNo = model.PANNo;
                        yscmdata.CINNo = model.CINNo;
                        yscmdata.TanNo = model.TanNo;
                        yscmdata.RequestedOn = DateTime.Now;
                        yscmdata.PaymentTerms = model.PaymentTerms;
                        yscmdata.Address = model.street;
                       // yscmdata.City = model.City;
                        yscmdata.PhoneAndExtn = model.PhoneAndExtn;
                        yscmdata.TanNo = model.TanNo;
                        yscmdata.Onetimevendor = model.Onetimevendor;
                        yscmdata.EvaluationRequired = model.EvaluationRequired;
                        yscmdata.Id = id;
                        obj.VendorRegisterMasters.Add(yscmdata);
                        obj.SaveChanges();
                        //yscmid = Remotedata.Id;
                        objid.UniqueId = yscmdata.Id;
                        RegistrationModelobj.Add(objid);
                    }
                    else
                    {
                        VendorRegisterMaster yscmdata = obj.VendorRegisterMasters.Where(li => li.Vendorid == model.VendorId).FirstOrDefault<VendorRegisterMaster>();
                        //yscmdata.Id = model.UniqueId;
                        yscmdata.Onetimevendor = model.Onetimevendor;
                        yscmdata.EvaluationRequired = model.EvaluationRequired;
                        yscmdata.VendorNoInSAP = model.VendorNoInSAP;
                        yscmdata.RequestedOn = model.RequestedOn;
                        yscmdata.VendorName = model.VendorName;
                        yscmdata.Address = model.VendorAddress;
                        yscmdata.PostalCode = model.PostalCode;
                        yscmdata.City = model.City;
                        yscmdata.LocalBranchOffice = model.LocalBranchOffice;
                        yscmdata.PhoneAndExtn = model.PhoneAndExtn;
                        yscmdata.Fax = model.Fax;
                        yscmdata.ContactPerson = model.ContactPerson;
                        yscmdata.Phone = model.Phone;
                        yscmdata.GSTNo = model.GSTNo;
                        yscmdata.NatureofBusiness = model.NatureofBusiness;
                        yscmdata.PANNo = model.PANNo;
                        yscmdata.CINNo = model.CINNo;
                        yscmdata.TanNo = model.TanNo;
                        yscmdata.RequestedOn = DateTime.Now;
                        yscmdata.PaymentTerms = model.PaymentTerms;
                        yscmdata.Address = model.street;
                        yscmdata.City = model.City;
                        yscmdata.PhoneAndExtn = model.PhoneAndExtn;
                        yscmdata.TanNo = model.TanNo;
                        yscmdata.Onetimevendor = model.Onetimevendor;
                        yscmdata.EvaluationRequired = model.EvaluationRequired;
                        obj.SaveChanges();
                        //id = yscmdata.Id;
                        objid.UniqueId = yscmdata.Id;
                        RegistrationModelobj.Add(objid);
                    }


                    if (id != 0)
                    {

                        //var remotedataforbankdetails = new RemoteBankDetailsForVendor();
                        BankDetailsForVendor yscmdataforbankdetail = obj.BankDetailsForVendors.Where(li => li.VendorId == model.VendorId).FirstOrDefault<BankDetailsForVendor>();
                        if (yscmdataforbankdetail == null)
                        {
                            var dataforbankdetails = new BankDetailsForVendor();
                            dataforbankdetails.Id = bankdetailsid;
                            dataforbankdetails.IFSCCode = model.IFSCCode;
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
                            yscmdataforbankdetail.BankDetails = model.BankDetails;
                            yscmdataforbankdetail.BankerName = model.BankerName;
                            yscmdataforbankdetail.AccNo = model.AccNo;
                            yscmdataforbankdetail.AccountHolderName = model.AccountHolderName;
                            yscmdataforbankdetail.VendorId = id;
                            yscmdataforbankdetail.LocationOrBranch = model.LocationOrBranch;
                            // vscm.RemoteBankDetailsForVendors.Add(remotedataforbankdetails);
                            vscm.SaveChanges();
                        }

                    }
                }
                   
                    return RegistrationModelobj;
                
            }
            catch (Exception ex)
            {
                throw;
            }
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

        public List<documentDetails> InsertDocuments(List<documentDetails> model)
        {
            List<documentDetails> Listobj = new List<documentDetails>();
            var eachobj = new documentDetails();
            try
            {

                if (model != null)
                {

                    foreach (var item in model)
                    {
                        var remotedataforDocumentdetails = new RemoteVendorRegisterDocumentDetail();
                        remotedataforDocumentdetails.DocumentationTypeId = item.DocumentationTypeId;
                        remotedataforDocumentdetails.PhysicalPath = item.PhysicalPath;
                        remotedataforDocumentdetails.VendorId = item.VendorId;
                        remotedataforDocumentdetails.UploadedOn = DateTime.Now;
                        remotedataforDocumentdetails.Status = false;
                        vscm.RemoteVendorRegisterDocumentDetails.Add(remotedataforDocumentdetails);
                        vscm.SaveChanges();
                        eachobj.Id = remotedataforDocumentdetails.Id;
                        Listobj.Add(eachobj);

                    }


                }
            }
            catch
            {

            }
            return Listobj;
        }

        public bool DeletefileAttached(documentDetails model)
        {
            Boolean deletestatus = false;
            RemoteVendorRegisterDocumentDetail remotedatafordelete = vscm.RemoteVendorRegisterDocumentDetails.Where(li => li.VendorId == model.VendorId && li.PhysicalPath == model.PhysicalPath && li.DocumentationTypeId == model.DocumentationTypeId).FirstOrDefault<RemoteVendorRegisterDocumentDetail>();
            if (remotedatafordelete != null)
            {
                remotedatafordelete.Status = true;
                vscm.SaveChanges();
                deletestatus = true;
            }
            else
            {
                deletestatus = false;
            }
            return deletestatus;
        }

        public bool DeletefileAttachedforDocuments(RFQDocument model)
        {
            Boolean deletestatus = false;
            RemoteRFQDocument remotedatafordelete = vscm.RemoteRFQDocuments.Where(li => li.rfqRevisionId == model.rfqRevisionId && li.DocumentType == model.DocumentType && li.UploadedBy == model.UploadedBy && li.Path == model.DocumentName && li.rfqItemsid == model.rfqItemsid).FirstOrDefault();
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

            RFQDocument remotedatafordeleteYSCM = obj.RFQDocuments.Where(li => li.rfqRevisionId == model.rfqRevisionId && li.Path == model.DocumentName && li.DocumentType == model.DocumentType && li.UploadedBy == model.UploadedBy && li.rfqItemsid == model.rfqItemsid).FirstOrDefault();
            if (remotedatafordelete != null)
            {
                remotedatafordeleteYSCM.DeleteFlag = true;
                obj.SaveChanges();
                deletestatus = true;
            }
            else
            {
                deletestatus = false;
            }
            return deletestatus;
        }

        public List<RFQTerms> GetTermMaster(int rfqrevisionId)
        {
            RFQTerms _stateEachItem = null;
            List<RFQTerms> _listItem = new List<RFQTerms>();
            var data = vscm.RemoteRfqTerms.Where(o => o.DeleteFlag != true && o.RfqRevisionId == rfqrevisionId).ToList();
            foreach (var info in data)
            {
                _stateEachItem = new RFQTerms();
                _stateEachItem.VRfqTermsid = info.VRfqTermsid;
                _stateEachItem.Terms = info.Terms.Trim();
                _listItem.Add(_stateEachItem);

            }
            return _listItem;
            //throw new NotImplementedException();
        }

        public List<RFQTerms> UpdateVendorTerms(List<RFQTerms> model)
        {

            List<RFQTerms> listobj = new List<RFQTerms>();
            RFQTerms eachobj = new RFQTerms();
            foreach (var item in model)
            {
                RemoteRfqTerm remotedataforupdateterm = vscm.RemoteRfqTerms.Where(li => li.VRfqTermsid == item.VRfqTermsid).FirstOrDefault();
                if (remotedataforupdateterm != null)
                {
                    remotedataforupdateterm.VendorResponse = item.VendorResponse;
                    remotedataforupdateterm.Remarks = item.remarks;
                    //vscm.RemoteRfqTerms.Add(remotedataforupdateterm);
                    vscm.SaveChanges();

                }


                //RemoteRFQStatus rfqstatus = vscm.RemoteRFQStatus();

                //else
                //{
                //    RemoteRfqTerm rfqtermsadd = new RemoteRfqTerm();
                //    rfqtermsadd.VendorResponse = item.VendorResponse;
                //    rfqtermsadd.Remarks = item.remarks;
                //    vscm.RemoteRfqTerms.Add(rfqtermsadd);
                //    vscm.SaveChanges();
                //}
            }


            foreach (var items in model)
            {
                RFQTerm remotedataforupdatetermYSCM = obj.RFQTerms.Where(li => li.RfqTermsid == items.VRfqTermsid).FirstOrDefault();
                if (remotedataforupdatetermYSCM != null)
                {
                    remotedataforupdatetermYSCM.VendorResponse = items.VendorResponse;
                    remotedataforupdatetermYSCM.Remarks = items.remarks;
                    //vscm.RemoteRfqTerms.Add(remotedataforupdateterm);
                    obj.SaveChanges();
                    eachobj.errmsg = "OK";
                    listobj.Add(eachobj);
                }
                //    else
                //    {
                //        RFQTerm rfqtermsaddyscm = new RFQTerm();
                //        rfqtermsaddyscm.VendorResponse = items.VendorResponse;
                //        rfqtermsaddyscm.Remarks = items.remarks;
                //    //rfqtermsaddyscm.RFQrevisionId=
                //        obj.RFQTerms.Add(rfqtermsaddyscm);
                //        obj.SaveChanges();
                //    eachobj.errmsg = "OK";
                //    listobj.Add(eachobj);
                //}

            }
            //if (model != null)
            //{

            //}

            return listobj;
            // throw new NotImplementedException();
        }


        public List<DocumentTypeMaster> GetMasterDocumentTypeList()
        {

            DocumentTypeMaster _Documenteachitem = null;
            List<DocumentTypeMaster> _listItem = new List<DocumentTypeMaster>();
            var data = vscm.RemoteDocumentTypeMasters.Where(o => o.IsActive == true && o.UsedBYVendor == true).ToList();
            foreach (var info in data)
            {
                _Documenteachitem = new DocumentTypeMaster();
                _Documenteachitem.DocumenTypeId = info.DocumenTypeId;
                _Documenteachitem.DocumentTypeName = info.DocumentTypeName;
                _listItem.Add(_Documenteachitem);

            }
            return _listItem;
            // throw new NotImplementedException();
        }

        public string UpdateVendorCommunication(VendorCommunicaton model)
        {
            string msg = "";
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
                    remotedataforvendorcommyscm.RemarksFrom = model.RemarksFrom;
                    remotedataforvendorcommyscm.RfqCCid = rfqccid;
                    obj.RFQCommunications.Add(remotedataforvendorcommyscm);
                    obj.SaveChanges();

                    msg = "OK";


                }
            }
            catch (Exception e)
            {
                throw;
            }

            return msg;

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
            _listobj = vscm.RemoteRFQDocuments.Where(li => li.rfqRevisionId == revisionId && li.rfqItemsid.Equals(null) && li.DocumentType != 1 && li.DeleteFlag.Equals(false)).ToList();
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
            RemoteVendorRegisterMaster getdata = vscm.RemoteVendorRegisterMasters.Where(li => li.Vendorid == vendorId).FirstOrDefault();

            listobj.VendorName = getdata.VendorName;
            listobj.VendorNoInSAP = getdata.VendorNoInSAP;
            listobj.VendorName = getdata.VendorName;
            listobj.street = getdata.Address;
            return listobj;
            // throw new NotImplementedException();
        }

        public string changepassword(Changepassword obj)
        {
            string msg = string.Empty;
            if (obj != null)
            {
                RemoteVendorUserMaster chnagepwdobj = vscm.RemoteVendorUserMasters.Where(li => li.VendorId == obj.VendorId && li.pwd==obj.CurrentPassword).FirstOrDefault();
                if (chnagepwdobj.pwd == obj.CurrentPassword)
                {
                    chnagepwdobj.pwd = obj.NewPassword;
                    vscm.SaveChanges();
                    msg = "OK";
                }
                else 
                {
                    msg = "error";
                }
            }
            return msg;
            //throw new NotImplementedException();
        }

        public bool sendVendormail(int RFQRevisionId)
        {
            try
            {
                VSCMEntities yscmobj = new VSCMEntities();
                RemoteRFQRevisions_N mprrevisionDetail = yscmobj.RemoteRFQRevisions_N.Include(li=>li.RemoteRFQMaster).Where(li => li.rfqRevisionId == RFQRevisionId).FirstOrDefault<RemoteRFQRevisions_N>();
                using (var db = new YSCMEntities()) //ok
                {

                    RFQRevisions_N mprrevisionDetails = db.RFQRevisions_N.Where(li => li.rfqRevisionId == RFQRevisionId).FirstOrDefault<RFQRevisions_N>();
                    //var issueOfPurpose = mprrevisionDetail.IssuePurposeId == 1 ? "For Enquiry" : "For Issuing PO";
                    EmailSend emlSndngList = new EmailSend();

                    emlSndngList.Body = "<html><meta charset=\"ISO-8859-1\"><head><link rel = 'stylesheet' href = 'https://maxcdn.bootstrapcdn.com/bootstrap/4.3.1/css/bootstrap.min.css' ></head><body><div class='container'><table border='1' class='table table-bordered table-sm'><tr><td><b>RFQ Number</b></td><td>" + mprrevisionDetail.RemoteRFQMaster.RFQNo + "</td></tr></table><br/><br/><span>" + "</span><br/><br/><b>Click here to redirect : </b>&nbsp<a href='http://10.29.15.183:85/'>http://10.29.15.183:85/</a></div></body></html>";
                    // emlSndngList.Body = "<html><meta charset=\"ISO-8859-1\"><head><link rel = 'stylesheet' href = 'https://maxcdn.bootstrapcdn.com/bootstrap/4.3.1/css/bootstrap.min.css' ></head><body><div class='container'><table border='1' class='table table-bordered table-sm'><tr><td><b>MPR Number</b></td><td>" +"<b>Click here to redirect : </b>&nbsp<a href='http://10.29.15.165:99/'>http://10.29.15.165:99/</a></div></body></html>";
                    Employee Email = db.Employees.Where(li => li.EmployeeNo == mprrevisionDetails.BuyergroupEmail).FirstOrDefault<Employee>();
                    emlSndngList.Subject = "Response From Vendor  on RFQNumber:";// + mprrevisionDetail.RemoteRFQMaster.RFQNo;
                    emlSndngList.ToEmailId = "shashikala.k@in.yokogawa.com";//(db.Employees.Where(li => li.EmployeeNo == mprrevisionDetails.BuyergroupEmail).FirstOrDefault<Employee>()).EMail;
                    emlSndngList.FrmEmailId = "yilinfo@in.yokogawa.com";
                    //emlSndngList.ToEmailId = "shashikala.k@in.yokogawa.com";
                    this.sendEmail(emlSndngList);
                    
                   

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return true;
        }
        public bool sendEmail(EmailSend emlSndngList)
        {
            emlSndngList.BCC = "shashikala.k@in.yokogawa.com";
            // string smtp= ConfigurationManager.AppSettings["AttachedDocPath"];
            MailMessage mailMessage = new MailMessage(emlSndngList.FrmEmailId, emlSndngList.ToEmailId);
            SmtpClient client = new SmtpClient();
            if (!string.IsNullOrEmpty(emlSndngList.Subject))
                mailMessage.Subject = emlSndngList.Subject;
            if (!string.IsNullOrEmpty(emlSndngList.CC))
                mailMessage.CC.Add(emlSndngList.CC);
            if (!string.IsNullOrEmpty(emlSndngList.BCC))
                mailMessage.CC.Add(emlSndngList.BCC);
            mailMessage.Body = emlSndngList.Body;
            mailMessage.IsBodyHtml = true;
            mailMessage.BodyEncoding = Encoding.UTF8;
            //SmtpClient mailClient = new SmtpClient("localhost", 25);
            SmtpClient mailClient = new SmtpClient("10.29.15.9", 25);
            //mailClient.EnableSsl = false;
            mailClient.DeliveryMethod = SmtpDeliveryMethod.Network;
            mailClient.Send(mailMessage);
           
            //MailMessage mailMessage = new MailMessage(emlSndngList.FrmEmailId, emlSndngList.ToEmailId);
            //SmtpClient client = new SmtpClient();
            //if (!string.IsNullOrEmpty(emlSndngList.Subject))
            //    mailMessage.Subject = emlSndngList.Subject;
            //if (!string.IsNullOrEmpty(emlSndngList.CC))
            //    mailMessage.CC.Add(emlSndngList.CC);
            //mailMessage.Body = emlSndngList.Body;
            //mailMessage.IsBodyHtml = true;
            //mailMessage.BodyEncoding = Encoding.UTF8;
            //SmtpClient mailClient = new SmtpClient("10.29.15.9", 25);
            //mailClient.EnableSsl = false;
            ////mailClient.DeliveryMethod = SmtpDeliveryMethod.Network;
            //mailClient.Send(mailMessage);
            return true;
        }

       

        public class EmailSend
        {
            public string FrmEmailId { get; set; }
            public string ToEmailId { get; set; }
            public string CC { get; set; }
            public string Subject { get; set; }
            public string Body { get; set; }
            public string BCC { get; set; }
        }
        public async Task<statuscheckmodel> DeleteRfqIteminfoByidformultiple(int id, int BOMid)
        {
            statuscheckmodel status = new statuscheckmodel();
            try
            {
                double? totalprice = 0;
                //vscm.Database.Connection.Open();
                var Remotedata = vscm.RemoteRFQItemsInfo_N.Where(x => x.RFQSplitItemId == id && x.DeleteFlag == false).FirstOrDefault();
                var remotebomdata = vscm.RemoteRfqVendorBOMs.Where(x => x.RFQVendorbomItemId == BOMid && x.DeleteFlag==false).FirstOrDefault();
                var bomdata = obj.RfqVendorBOMs.Where(x => x.RFQVendorbomItemId == BOMid&& x.DeleteFlag==false).FirstOrDefault();
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
            RemoteVendorUserMaster emailidexists = vscm.RemoteVendorUserMasters.Where(li => li.Vuserid == emailId.emailid && li.Active==true).FirstOrDefault();
            if(emailidexists!=null)
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
           
            string token = Guid.NewGuid().ToString();
           // DateTime expirydateandtime = System.DateTime.Now.AddHours(24);
            forgetpassword passwordobj = new forgetpassword();
            passwordobj.emailid = model.emailid;
            passwordobj.Expirydatetime = System.DateTime.Now.AddMinutes(Convert.ToInt32(model.tokenduration));
            passwordobj.URLCreatedOn = System.DateTime.Now;
            passwordobj.URL = model.baseURL + model.emailid+"/"+token;
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
            emailobj.Body = "<html><meta charset=\"ISO-8859-1\"><head><link rel ='stylesheet' href ='https://maxcdn.bootstrapcdn.com/bootstrap/4.3.1/css/bootstrap.min.css'></head><body><div class='container'><p>Dear User,</p><p>Please use the following URL to  reset the password in Vendor Portal.</p><p style = 'font-weight:600;text-decoration:underline'> LINK </p><p id= 'linkid'>"+ obj.ResetURL+ "</p><p style='color:red;'> The above link will be active for 120 minutes from the time of receiving this email. </p><p style='margin-bottom:0px;'>Regards,</p><p> YIL CMM Team.</p></div></body></html>";
            emailobj.ToEmailId = model.emailid;
            emailobj.FrmEmailId = model.fromemail;

            sendEmail(emailobj);
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
            sendEmail(emailobj);
            return true;
        }

        public string Resetpassword(forgetpassword model)
        {
            string status = "";
            if (model != null)
            {
                RemoteForgetPassword tokenvalid = vscm.RemoteForgetPasswords.Where(li => li.Token == model.token && li.emailId==model.emailid && li.TokenUsed==false).FirstOrDefault();
                if(tokenvalid != null)
                {
                    if(tokenvalid.ExpirtyDateAndTime >= System.DateTime.Now && tokenvalid.TokenUsed == false)
                    {
                        RemoteVendorUserMaster remoteresetpwd = vscm.RemoteVendorUserMasters.Where(li=>li.Vuserid==model.emailid).FirstOrDefault();
                        remoteresetpwd.pwd = model.ConfirmPassword;
                        vscm.SaveChanges();
                        VendorUserMaster resetpwd = obj.VendorUserMasters.Where(li=>li.Vuserid==model.emailid).FirstOrDefault();
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
                else {
                    status = false;

                }
                   
                }
            
            return status;
        }
    }

}

