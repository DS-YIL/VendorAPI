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
					this.sendEmail(emlSndngList);

				}

			}
			catch (Exception ex)
			{
				log.ErrorMessage("EmailTemplate", "sendMailtoBuyer", ex.Message + "; " + ex.StackTrace.ToString());
			}
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
				mailMessage.From = new MailAddress(emlSndngList.FrmEmailId); //From Email Id
				string[] ToMuliId = emlSndngList.ToEmailId.Split(',');
				foreach (string ToEMailId in ToMuliId)
				{
					mailMessage.To.Add(new MailAddress(ToEMailId)); //adding multiple TO Email Id
				}
				SmtpClient client = new SmtpClient();
				if (!string.IsNullOrEmpty(emlSndngList.Subject))
					mailMessage.Subject = emlSndngList.Subject;

				if (!string.IsNullOrEmpty(emlSndngList.CC))
				{
					string[] CCId = emlSndngList.CC.Split(',');

					foreach (string CCEmail in CCId)
					{
						mailMessage.CC.Add(new MailAddress(CCEmail)); //Adding Multiple CC email Id
					}
				}

				if (!string.IsNullOrEmpty(emlSndngList.BCC))
				{
					string[] bccid = emlSndngList.BCC.Split(',');


					foreach (string bccEmailId in bccid)
					{
						mailMessage.Bcc.Add(new MailAddress(bccEmailId)); //Adding Multiple BCC email Id
					}
				}

				if (!string.IsNullOrEmpty(BCC))
					mailMessage.Bcc.Add(new MailAddress(BCC));
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
