using SCMModels.SCMModels;
using System;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using SCMModels.RemoteModel;
using DALayer.Common;
using System.Collections.Generic;
using SCMModels;

namespace DALayer.Emails
{
	public class EmailTemplateDA : IEmailTemplateDA
	{
		private ErrorLog log = new ErrorLog();

		public bool sendQuotemailtoRequestor(int RFQRevisionId)
		{
			try
			{
				VSCMEntities yscmobj = new VSCMEntities();
				var db = new YSCMEntities();
				RemoteRFQRevisions_N rfqrevisiondetails = yscmobj.RemoteRFQRevisions_N.Where(li => li.rfqRevisionId == RFQRevisionId).FirstOrDefault<RemoteRFQRevisions_N>();
				RemoteRFQMaster rfqmasterDetails = yscmobj.RemoteRFQMasters.Where(li => li.RfqMasterId == rfqrevisiondetails.rfqMasterId).FirstOrDefault<RemoteRFQMaster>();
				RemoteVendorMaster vendor = yscmobj.RemoteVendorMasters.Where(li => li.Vendorid == rfqmasterDetails.VendorId).FirstOrDefault();

				var fromMail = ConfigurationManager.AppSettings["fromemail"];
				var mpripaddress = ConfigurationManager.AppSettings["UI_IpAddress"];
				mpripaddress = mpripaddress + "SCM/MPRForm/" + rfqmasterDetails.MPRRevisionId + "";
				var rfqipaddress = ConfigurationManager.AppSettings["UI_IpAddress"];
				rfqipaddress = rfqipaddress + "SCM/VendorQuoteView/" + rfqrevisiondetails.rfqRevisionId + "";
				var requestionId = db.MPRRevisions.Where(li => li.RevisionId == rfqmasterDetails.MPRRevisionId).FirstOrDefault().RequisitionId;
				var mprDocNo = db.MPRDetails.Where(li => li.RequisitionId == requestionId).FirstOrDefault().DocumentNo;

				EmailSend emlSndngList = new EmailSend();
				emlSndngList.Subject = "Vendor Responded to RFQNumber: " + rfqmasterDetails.RFQNo + " for " + mprDocNo;// + mprrevisionDetail.RemoteRFQMaster.RFQNo;

				emlSndngList.Body = "<html><meta charset=\"ISO-8859-1\"><head><link rel ='stylesheet' href ='https://maxcdn.bootstrapcdn.com/bootstrap/4.3.1/css/bootstrap.min.css'></head><body><div class='container'><p>Dear Sir,</p><p>Quote has been submitted for: " + rfqmasterDetails.RFQNo + "</p><br/><div><b  style='color:#40bfbf;'>TO View MPR: <a href='" + mpripaddress + "'>" + mpripaddress + "</a></b></div><br /><div><b  style='color:#40bfbf;'>TO View RFQ: <a href='" + rfqipaddress + "'>" + rfqipaddress + "</a></b><p style = 'margin-bottom:0px;' ><br/> Regards,</p><p> <b>" + vendor.VendorName + " - " + vendor.VendorCode + "</b></p></div></body></html>";
				Employee Email = db.Employees.Where(li => li.EmployeeNo == rfqrevisiondetails.BuyergroupEmail).FirstOrDefault<Employee>();
				emlSndngList.FrmEmailId = fromMail;
				var toEMail = Convert.ToString(rfqrevisiondetails.CreatedBy);
				emlSndngList.ToEmailId = (db.Employees.Where(li => li.EmployeeNo == toEMail).FirstOrDefault<Employee>()).EMail;
				emlSndngList.CC = (db.Employees.Where(li => li.EmployeeNo == rfqrevisiondetails.BuyergroupEmail).FirstOrDefault<Employee>()).EMail;

				if ((!string.IsNullOrEmpty(emlSndngList.FrmEmailId) && !string.IsNullOrEmpty(emlSndngList.FrmEmailId)) && (emlSndngList.FrmEmailId != "NULL" && emlSndngList.ToEmailId != "NULL"))
					this.sendEmail(emlSndngList);

			}
			catch (Exception ex)
			{
				log.ErrorMessage("EmailTemplate", "sendQuotemailtoRequestor", ex.Message + "; " + ex.StackTrace.ToString());
			}
			return true;
		}

		/*Name of Function : <<Technical clearance mail to CMM>>  Author :<<Prasanna>>  
		  Date of Creation <<09-10-2020>>
		  Purpose : <<Sending mail method>>
		  Review Date :<<>>   Reviewed By :<<>>*/
		public bool sendSatustoRequestor(int RFQRevisionId, string StatusTxt, string Remarks)
		{
			try
			{
				VSCMEntities yscmobj = new VSCMEntities();
				var db = new YSCMEntities();
				RemoteRFQRevisions_N rfqrevisiondetails = yscmobj.RemoteRFQRevisions_N.Where(li => li.rfqRevisionId == RFQRevisionId).FirstOrDefault<RemoteRFQRevisions_N>();
				RemoteRFQMaster rfqmasterDetails = yscmobj.RemoteRFQMasters.Where(li => li.RfqMasterId == rfqrevisiondetails.rfqMasterId).FirstOrDefault<RemoteRFQMaster>();
				RemoteVendorMaster vendor = yscmobj.RemoteVendorMasters.Where(li => li.Vendorid == rfqmasterDetails.VendorId).FirstOrDefault();

				var fromMail = ConfigurationManager.AppSettings["fromemail"];
				var mpripaddress = ConfigurationManager.AppSettings["UI_IpAddress"];
				mpripaddress = mpripaddress + "SCM/MPRForm/" + rfqmasterDetails.MPRRevisionId + "";
				var rfqipaddress = ConfigurationManager.AppSettings["UI_IpAddress"];
				rfqipaddress = rfqipaddress + "SCM/VendorQuoteView/" + rfqrevisiondetails.rfqRevisionId + "";
				var requestionId = db.MPRRevisions.Where(li => li.RevisionId == rfqmasterDetails.MPRRevisionId).FirstOrDefault().RequisitionId;
				var mprDocNo = db.MPRDetails.Where(li => li.RequisitionId == requestionId).FirstOrDefault().DocumentNo;

				EmailSend emlSndngList = new EmailSend();
				emlSndngList.Subject = "Vendor " + StatusTxt + " to RFQNumber: " + rfqmasterDetails.RFQNo + " for " + mprDocNo;// + mprrevisionDetail.RemoteRFQMaster.RFQNo;

				emlSndngList.Body = "<html><meta charset=\"ISO-8859-1\"><head><link rel ='stylesheet' href ='https://maxcdn.bootstrapcdn.com/bootstrap/4.3.1/css/bootstrap.min.css'></head><body><div class='container'><p>Dear Sir,</p><p>" + Remarks + "</p><br/><div><b  style='color:#40bfbf;'>TO View MPR: <a href='" + mpripaddress + "'>" + mpripaddress + "</a></b></div><br /><div><b  style='color:#40bfbf;'>TO View RFQ: <a href='" + rfqipaddress + "'>" + rfqipaddress + "</a></b><p style = 'margin-bottom:0px;' ><br/> Regards,</p><p> <b>" + vendor.VendorName + " - " + vendor.VendorCode + "</b></p></div></body></html>";
				Employee Email = db.Employees.Where(li => li.EmployeeNo == rfqrevisiondetails.BuyergroupEmail).FirstOrDefault<Employee>();
				emlSndngList.FrmEmailId = fromMail;
				var toEMail = Convert.ToString(rfqrevisiondetails.CreatedBy);
				emlSndngList.ToEmailId = (db.Employees.Where(li => li.EmployeeNo == toEMail).FirstOrDefault<Employee>()).EMail;
				emlSndngList.CC = (db.Employees.Where(li => li.EmployeeNo == rfqrevisiondetails.BuyergroupEmail).FirstOrDefault<Employee>()).EMail;

				if ((!string.IsNullOrEmpty(emlSndngList.FrmEmailId) && !string.IsNullOrEmpty(emlSndngList.FrmEmailId)) && (emlSndngList.FrmEmailId != "NULL" && emlSndngList.ToEmailId != "NULL"))
					this.sendEmail(emlSndngList);

			}
			catch (Exception ex)
			{
				log.ErrorMessage("EmailTemplate", "sendSatustoRequestor", ex.Message + "; " + ex.StackTrace.ToString());
			}
			return true;
		}
		/*Name of Function : <<Technical clearance mail to CMM>>  Author :<<Prasanna>>  
		  Date of Creation <<09-10-2020>>
		  Purpose : <<Sending mail method>>
		  Review Date :<<>>   Reviewed By :<<>>*/
		public bool sendTechNotificationMail(int RFQRevisionId)
		{
			try
			{
				VSCMEntities yscmobj = new VSCMEntities();
				var db = new YSCMEntities();
				RemoteRFQRevisions_N rfqrevisiondetails = yscmobj.RemoteRFQRevisions_N.Where(li => li.rfqRevisionId == RFQRevisionId).FirstOrDefault<RemoteRFQRevisions_N>();
				RemoteRFQMaster rfqmasterDetails = yscmobj.RemoteRFQMasters.Where(li => li.RfqMasterId == rfqrevisiondetails.rfqMasterId).FirstOrDefault<RemoteRFQMaster>();
				RemoteVendorMaster vendor = yscmobj.RemoteVendorMasters.Where(li => li.Vendorid == rfqmasterDetails.VendorId).FirstOrDefault();
				MPRRevision mprrevisionDetails = db.MPRRevisions.Where(li => li.RevisionId == rfqmasterDetails.MPRRevisionId && li.BoolValidRevision == true).FirstOrDefault();
				var mprDocNo = db.MPRDetails.Where(li => li.RequisitionId == mprrevisionDetails.RequisitionId).FirstOrDefault().DocumentNo;
				List<MPRIncharge> mprincharges = new List<MPRIncharge>();
				if (mprrevisionDetails != null)
					mprincharges = db.MPRIncharges.Where(li => li.RevisionId == mprrevisionDetails.RevisionId && li.RequisitionId == mprrevisionDetails.RequisitionId && li.CanClearTechnically == true).ToList();
				var fromMail = ConfigurationManager.AppSettings["fromemail"];
				var mpripaddress = ConfigurationManager.AppSettings["UI_IpAddress"];
				mpripaddress = mpripaddress + "SCM/MPRForm/" + rfqmasterDetails.MPRRevisionId + "";
				var rfqipaddress = ConfigurationManager.AppSettings["UI_IpAddress"];
				rfqipaddress = rfqipaddress + "SCM/VendorQuoteView/" + rfqrevisiondetails.rfqRevisionId + "";

				EmailSend emlSndngList = new EmailSend();
				emlSndngList.Subject = "Vendor Responded to RFQNumber: " + rfqmasterDetails.RFQNo + " for " + mprDocNo;// + mprrevisionDetail.RemoteRFQMaster.RFQNo;
				emlSndngList.Body = "<html><meta charset=\"ISO-8859-1\"><head><link rel ='stylesheet' href ='https://maxcdn.bootstrapcdn.com/bootstrap/4.3.1/css/bootstrap.min.css'></head><body><div class='container'><p>Dear Sir,</p><p>Vendor has reponded with technical documents. Plese review and confirm the documents</p><br/><div><b  style='color:#40bfbf;'>TO View MPR: <a href='" + mpripaddress + "'>" + mpripaddress + "</a></b></div><br /><div><b  style='color:#40bfbf;'>TO View RFQ: <a href='" + rfqipaddress + "'>" + rfqipaddress + "</a></b><p style = 'margin-bottom:0px;' ><br/> Regards,</p><p> <b>" + vendor.VendorName + " - " + vendor.VendorCode + "</b></p></div></body></html>";
				emlSndngList.FrmEmailId = fromMail;

				//To Emails
				string ToEmails = "";
				if (mprrevisionDetails != null)
				{
					ToEmails = (db.Employees.Where(li => li.EmployeeNo == mprrevisionDetails.CheckedBy).FirstOrDefault<Employee>()).EMail;
					ToEmails += "," + (db.Employees.Where(li => li.EmployeeNo == mprrevisionDetails.ApprovedBy).FirstOrDefault<Employee>()).EMail;
				}
				if (mprincharges.Count() > 0)
				{
					foreach (var item in mprincharges)
					{
						ToEmails += "," + (db.Employees.Where(li => li.EmployeeNo == item.Incharge).FirstOrDefault<Employee>()).EMail;
					}
				}
				emlSndngList.ToEmailId = ToEmails;
				//CC Mails
				var CC1 = Convert.ToString(rfqrevisiondetails.CreatedBy);
				string CCEmails = (db.Employees.Where(li => li.EmployeeNo == CC1).FirstOrDefault<Employee>()).EMail;
				CCEmails += "," + (db.Employees.Where(li => li.EmployeeNo == rfqrevisiondetails.BuyergroupEmail).FirstOrDefault<Employee>()).EMail;
				emlSndngList.CC = CCEmails;

				if ((!string.IsNullOrEmpty(emlSndngList.FrmEmailId) && !string.IsNullOrEmpty(emlSndngList.FrmEmailId)) && (emlSndngList.FrmEmailId != "NULL" && emlSndngList.ToEmailId != "NULL"))
					this.sendEmail(emlSndngList);

			}
			catch (Exception ex)
			{
				log.ErrorMessage("EmailTemplate", "sendTechNotificationMail", ex.Message + "; " + ex.StackTrace.ToString());
			}
			return true;
		}
		public bool sendCommunicationmailtoRequestor(int RFQRevisionId, string Remarks)
		{
			try
			{
				VSCMEntities yscmobj = new VSCMEntities();
				var db = new YSCMEntities();
				RemoteRFQRevisions_N rfqrevisiondetails = yscmobj.RemoteRFQRevisions_N.Where(li => li.rfqRevisionId == RFQRevisionId).FirstOrDefault<RemoteRFQRevisions_N>();
				RemoteRFQMaster rfqmasterDetails = yscmobj.RemoteRFQMasters.Where(li => li.RfqMasterId == rfqrevisiondetails.rfqMasterId).FirstOrDefault<RemoteRFQMaster>();
				var fromMail = ConfigurationManager.AppSettings["fromemail"];
				var mpripaddress = ConfigurationManager.AppSettings["UI_IpAddress"];
				mpripaddress = mpripaddress + "SCM/MPRForm/" + rfqmasterDetails.MPRRevisionId + "";
				var rfqipaddress = ConfigurationManager.AppSettings["UI_IpAddress"];
				rfqipaddress = rfqipaddress + "SCM/VendorQuoteView/" + rfqrevisiondetails.rfqRevisionId + "";

				EmailSend emlSndngList = new EmailSend();
				emlSndngList.Body = "<html><head></head><body><div class='container'><p>Comments by Vendor</p></div><br/>" + Remarks + "  <br/><br/></div><b  style='color:#40bfbf;'>TO View MPR: <a href='" + mpripaddress + "'>" + mpripaddress + "</a></b></div><br /><br/><div><b  style='color:#40bfbf;'>TO View RFQ: <a href='" + rfqipaddress + "'>" + rfqipaddress + "</a></b></body></html>";
				Employee Email = db.Employees.Where(li => li.EmployeeNo == rfqrevisiondetails.BuyergroupEmail).FirstOrDefault<Employee>();
				emlSndngList.Subject = "Communication From Vendor  on RFQNumber: " + rfqmasterDetails.RFQNo;// + mprrevisionDetail.RemoteRFQMaster.RFQNo;
				emlSndngList.FrmEmailId = fromMail;
				var toEMail = Convert.ToString(rfqrevisiondetails.CreatedBy);
				emlSndngList.ToEmailId = (db.Employees.Where(li => li.EmployeeNo == toEMail).FirstOrDefault<Employee>()).EMail;
				emlSndngList.CC = (db.Employees.Where(li => li.EmployeeNo == rfqrevisiondetails.BuyergroupEmail).FirstOrDefault<Employee>()).EMail;

				if ((!string.IsNullOrEmpty(emlSndngList.FrmEmailId) && !string.IsNullOrEmpty(emlSndngList.FrmEmailId)) && (emlSndngList.FrmEmailId != "NULL" && emlSndngList.ToEmailId != "NULL"))
					this.sendEmail(emlSndngList);
			}
			catch (Exception ex)
			{
				log.ErrorMessage("EmailTemplate", "sendmailtoRequestor", ex.Message + "; " + ex.StackTrace.ToString());
			}
			return true;
		}
		public bool sendMailtoBuyer(int VendorId)
		{
			try
			{
				using (var db = new YSCMEntities()) //ok
				{
					var mpripaddress = ConfigurationManager.AppSettings["UI_IpAddress"];
					mpripaddress = mpripaddress + "SCM/VendorRegInitiate/" + VendorId + "";
					var fromMail = ConfigurationManager.AppSettings["fromemail"];
					VendorRegApprovalProcess vendorProcessDetails = db.VendorRegApprovalProcesses.Where(li => li.Vendorid == VendorId).FirstOrDefault();
					EmailSend emlSndngList = new EmailSend();
					emlSndngList.Subject = "Vendor Registration response";
					emlSndngList.Body = "<html><head></head><body><div class='container'><p>Comments by Vendor</p></div><br/><div><b  style='color:#40bfbf;'>TO View Details: <a href='" + mpripaddress + "'>" + mpripaddress + "</a></b></div><br /><div><b  style='color:#40bfbf;'></a></b></body></html>";
					emlSndngList.FrmEmailId = fromMail;
					emlSndngList.ToEmailId = (db.Employees.Where(li => li.EmployeeNo == vendorProcessDetails.IntiatedBy).FirstOrDefault<Employee>()).EMail;
					if ((!string.IsNullOrEmpty(emlSndngList.FrmEmailId) && !string.IsNullOrEmpty(emlSndngList.FrmEmailId)) && (emlSndngList.FrmEmailId != "NULL" && emlSndngList.ToEmailId != "NULL"))
						this.sendEmail(emlSndngList);

				}

			}
			catch (Exception ex)
			{
				log.ErrorMessage("EmailTemplate", "sendMailtoBuyer", ex.Message + "; " + ex.StackTrace.ToString());
			}
			return true;
		}

		/*Name of Function : <<sendASNMailtoBuyer>  Author :<<Prasanna>>  
		  Date of Creation <<27-11-2020>>
		  Purpose : <<Sending mail method>>
		  Review Date :<<>>   Reviewed By :<<>>*/
		public bool sendASNMailtoBuyer(int ASNId)
		{
			try
			{
				VSCMEntities vscm = new VSCMEntities();
				var mpripaddress = ConfigurationManager.AppSettings["UI_IpAddress"];
				mpripaddress = mpripaddress + "SCM/ASNView/" + ASNId + "";
				using (var db = new YSCMEntities()) //ok
				{
					RemoteASNShipmentHeader ASNHeader = vscm.RemoteASNShipmentHeaders.Where(li => li.ASNId == ASNId).FirstOrDefault();
					EmailSend emlSndngList = new EmailSend();
					emlSndngList.FrmEmailId = ConfigurationManager.AppSettings["fromemail"];
					emlSndngList.Subject = "ASNCreated";
					emlSndngList.Body = "<html><head></head><body><div class='container'><p>Click below link to view details</p></div><br/><div><b  style='color:#40bfbf;'>TO View Details: <a href='" + mpripaddress + "'>" + mpripaddress + "</a></b></div><br /><div><b  style='color:#40bfbf;'></a></b></body></html>";
					if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["ASNToEmail"]))
						emlSndngList.ToEmailId = ConfigurationManager.AppSettings["ASNToEmail"];
					if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["ASNCCEmail"]))
						emlSndngList.CC = ConfigurationManager.AppSettings["ASNCCEmail"];

					if ((!string.IsNullOrEmpty(emlSndngList.FrmEmailId) && !string.IsNullOrEmpty(emlSndngList.FrmEmailId)) && (emlSndngList.FrmEmailId != "NULL" && emlSndngList.ToEmailId != "NULL"))
						this.sendEmail(emlSndngList);
				}

			}
			catch (Exception ex)
			{
				log.ErrorMessage("EmailTemplate", "sendASNMailtoBuyer", ex.Message + "; " + ex.StackTrace.ToString());
			}
			return true;
		}

		/*Name of Function : <<sendInvoiceMailtoBuyer>  Author :<<Prasanna>>  
		  Date of Creation <<1412-11-2020>>
		  Purpose : <<Sending mail to YIL memebers>>
		  Review Date :<<>>   Reviewed By :<<>>*/
		public bool sendInvoiceMailtoBuyer(string invoiceNo)
		{
			try
			{
				VSCMEntities vscm = new VSCMEntities();
				var mpripaddress = ConfigurationManager.AppSettings["UI_IpAddress"];

				using (var db = new YSCMEntities()) //ok
				{
					RemoteASNShipmentHeader ASNHeader = vscm.RemoteASNShipmentHeaders.Where(li => li.InvoiceNo == invoiceNo).FirstOrDefault();
					mpripaddress = mpripaddress + "SCM/ASNView/" + ASNHeader.ASNId + "";
					EmailSend emlSndngList = new EmailSend();
					emlSndngList.FrmEmailId = ConfigurationManager.AppSettings["fromemail"];
					emlSndngList.Subject = "Invoice Submitted For ASNNo:" + ASNHeader.ASNNo + "";
					emlSndngList.Body = "<html><head></head><body><div class='container'><p>Click below link to view details</p></div><br/><div><b  style='color:#40bfbf;'>TO View Details: <a href='" + mpripaddress + "'>" + mpripaddress + "</a></b></div><br /><div><b  style='color:#40bfbf;'></a></b></body></html>";
					if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["ASNToEmail"]))
						emlSndngList.ToEmailId = ConfigurationManager.AppSettings["ASNToEmail"];
					if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["ASNCCEmail"]))
						emlSndngList.CC = ConfigurationManager.AppSettings["ASNCCEmail"];

					if ((!string.IsNullOrEmpty(emlSndngList.FrmEmailId) && !string.IsNullOrEmpty(emlSndngList.FrmEmailId)) && (emlSndngList.FrmEmailId != "NULL" && emlSndngList.ToEmailId != "NULL"))
						this.sendEmail(emlSndngList);
				}

			}
			catch (Exception ex)
			{
				log.ErrorMessage("EmailTemplate", "sendInvoiceMailtoBuyer", ex.Message + "; " + ex.StackTrace.ToString());
			}
			return true;
		}


		/*Name of Function : <<sendASNCommunicationMail>  Author :<<Prasanna>>  
		  Date of Creation <<14-12-2020>>
		  Purpose : <<Sending mail to YIL members>>
		  Review Date :<<>>   Reviewed By :<<>>*/
		public bool sendASNCommunicationMail(int ASNId, string Remarks)
		{
			try
			{
				VSCMEntities vscm = new VSCMEntities();
				using (var db = new YSCMEntities()) //ok
				{
					var mpripaddress = ConfigurationManager.AppSettings["UI_IpAddress"];
					mpripaddress = mpripaddress + "SCM/ASN/" + ASNId + "";
					var fromMail = ConfigurationManager.AppSettings["fromemail"];
					RemoteASNShipmentHeader ASNHeader = vscm.RemoteASNShipmentHeaders.Where(li => li.ASNId == ASNId).FirstOrDefault();

					EmailSend emlSndngList = new EmailSend();
					emlSndngList.Subject = "ASN Response";
					emlSndngList.Body = "<html><head></head><body><div class='container'><p>Comments by Vendor</p></div><br/>" + Remarks + "  <br/><br/></div><b  style='color:#40bfbf;'>TO View Details: <a href='" + mpripaddress + "'>" + mpripaddress + "</a></b></div><br /><div><b  style='color:#40bfbf;'></a></b></body></html>";
					emlSndngList.FrmEmailId = fromMail;
					if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["ASNToEmail"]))
						emlSndngList.ToEmailId = ConfigurationManager.AppSettings["ASNToEmail"];
					if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["ASNCCEmail"]))
						emlSndngList.CC = ConfigurationManager.AppSettings["ASNCCEmail"];

					if ((!string.IsNullOrEmpty(emlSndngList.FrmEmailId) && !string.IsNullOrEmpty(emlSndngList.FrmEmailId)) && (emlSndngList.FrmEmailId != "NULL" && emlSndngList.ToEmailId != "NULL"))
						this.sendEmail(emlSndngList);
				}

			}
			catch (Exception ex)
			{
				log.ErrorMessage("EmailTemplate", "sendASNCommunicationMail", ex.Message + "; " + ex.StackTrace.ToString());
			}
			return true;
		}

		/*Name of Function : <<sendBGmail>  Author :<<Prasanna>>  
		  Date of Creation <<16-01-2021>>
		  Purpose : <<Sending mail to YIL members>>
		  Review Date :<<>>   Reviewed By :<<>>*/
		public bool sendBGmail(int bgId)
		{
			try
			{
				VSCMEntities vscm = new VSCMEntities();
				using (var db = new YSCMEntities()) //ok
				{
					var mpripaddress = ConfigurationManager.AppSettings["UI_IpAddress"];
					mpripaddress = mpripaddress + "SCM/BGView/" + bgId + "";
					var fromMail = ConfigurationManager.AppSettings["fromemail"];
					RemoteBankGuarantee BGDeatils = vscm.RemoteBankGuarantees.Where(li => li.BGId == bgId).FirstOrDefault();
					var mailData = (db.Employees.Where(li => li.EmployeeNo == BGDeatils.CreatedBy).FirstOrDefault<Employee>());
					EmailSend emlSndngList = new EmailSend();
					emlSndngList.Subject = "BG Submitted For PONo:" + BGDeatils.PONo + "; BGNo:" + BGDeatils.BGNo + " ";
					emlSndngList.Body = "<html><head></head><body><div class='container'><p>Click below link to view details</p></div><br/><div><b  style='color:#40bfbf;'>TO View Details: <a href='" + mpripaddress + "'>" + mpripaddress + "</a></b></div><br /><div><b  style='color:#40bfbf;'></a></b><p style = 'margin-bottom:0px;' ><br/> Regards,</p><p> <b>" + BGDeatils.VendorName + "</b></p></body></html>";
					emlSndngList.FrmEmailId = fromMail;
					emlSndngList.ToEmailId = mailData.EMail + ",";
					if (BGDeatils.BuyerManger != null)
						emlSndngList.ToEmailId += (db.Employees.Where(li => li.EmployeeNo == BGDeatils.BuyerManger).FirstOrDefault<Employee>()).EMail;
					if ((!string.IsNullOrEmpty(emlSndngList.FrmEmailId) && !string.IsNullOrEmpty(emlSndngList.FrmEmailId)) && (emlSndngList.FrmEmailId != "NULL" && emlSndngList.ToEmailId != "NULL"))
						this.sendEmail(emlSndngList);
				}

			}
			catch (Exception ex)
			{
				log.ErrorMessage("EmailTemplate", "sendASNCommunicationMail", ex.Message + "; " + ex.StackTrace.ToString());
			}
			return true;
		}
		/*Name of Function : <<sendErrorLog>>  Author :<<Prasanna>>  
		  Date of Creation <<04-03-2021>>
		  Purpose : <<sendErrorLog>>
		  Review Date :<<>>   Reviewed By :<<>>*/
		public bool sendErrorLogEmail(string controllername, string methodname, string exception, Uri url)
		{
			EmailSend emlSndngList = new EmailSend();
			emlSndngList.Subject = "Error Log Created";
			emlSndngList.Body = "<html><head></head><body><div class='container'><b  style='color:#40bfbf;'>Controller Name:</b>" + controllername + "</div><br/><div><b  style='color:#40bfbf;'>Method Name:" + methodname + "</b></div><br /><div><b  style='color:#40bfbf;'>URL:" + url + "</b></div><div><b  style='color:#40bfbf;'>Exception Details:" + exception + "</b></div></body></html>";
			emlSndngList.FrmEmailId = ConfigurationManager.AppSettings["fromemail"];
			emlSndngList.ToEmailId = ConfigurationManager.AppSettings["ErrorToEmail"];
			if ((!string.IsNullOrEmpty(emlSndngList.FrmEmailId) && !string.IsNullOrEmpty(emlSndngList.FrmEmailId)) && (emlSndngList.FrmEmailId != "NULL" && emlSndngList.ToEmailId != "NULL"))
				this.sendEmail(emlSndngList);
			return true;
		}
		public bool sendEmail(EmailSend emlSndngList)
		{
			//bool validEmail = IsValidEmail(emlSndngList.ToEmailId);
			if (!string.IsNullOrEmpty(emlSndngList.ToEmailId) && !string.IsNullOrEmpty(emlSndngList.FrmEmailId))
			{
				var BCC = ConfigurationManager.AppSettings["BCC"];
				var SMTPServer = ConfigurationManager.AppSettings["SMTPServer"];
				MailMessage mailMessage = new MailMessage();
				mailMessage.From = new MailAddress(emlSndngList.FrmEmailId.Trim(), ""); //From Email Id
				string[] ToMuliId = emlSndngList.ToEmailId.Split(',');
				foreach (string ToEMailId in ToMuliId)
				{
					mailMessage.To.Add(new MailAddress(ToEMailId.Trim(), "")); //adding multiple TO Email Id
				}
				SmtpClient client = new SmtpClient();
				if (!string.IsNullOrEmpty(emlSndngList.Subject))
					mailMessage.Subject = emlSndngList.Subject;

				if (!string.IsNullOrEmpty(emlSndngList.CC))
				{
					string[] CCId = emlSndngList.CC.Split(',');

					foreach (string CCEmail in CCId)
					{
						mailMessage.CC.Add(new MailAddress(CCEmail.Trim(), "")); //Adding Multiple CC email Id
					}
				}

				if (!string.IsNullOrEmpty(emlSndngList.BCC))
				{
					string[] bccid = emlSndngList.BCC.Split(',');


					foreach (string bccEmailId in bccid)
					{
						mailMessage.Bcc.Add(new MailAddress(bccEmailId.Trim(), "")); //Adding Multiple BCC email Id
					}
				}

				if (!string.IsNullOrEmpty(BCC))
					mailMessage.Bcc.Add(new MailAddress(BCC.Trim(), ""));
				mailMessage.Body = emlSndngList.Body;
				mailMessage.IsBodyHtml = true;
				mailMessage.BodyEncoding = Encoding.UTF8;
				SmtpClient mailClient = new SmtpClient(SMTPServer, 25);
				//SmtpClient mailClient = new SmtpClient("10.29.15.9", 25);
				//mailClient.EnableSsl = true;
				mailClient.DeliveryMethod = SmtpDeliveryMethod.Network;
				mailClient.Send(mailMessage);
			}
			return true;
		}

		/*Name of Function : <<IsValidEmail>>  Author :<<Prasanna>>  
		  Date of Creation <<07-08-2020>>
		  Purpose : <<validate mail>>
		  Review Date :<<>>   Reviewed By :<<>>*/
		bool IsValidEmail(string email)
		{
			try
			{
				var addr = new System.Net.Mail.MailAddress(email);
				return addr.Address == email;
			}
			catch
			{
				return false;
			}
		}
	}


}
