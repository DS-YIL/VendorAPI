using SCMModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DALayer.Emails
{
	public interface IEmailTemplateDA
	{
		bool sendQuotemailtoRequestor(int RFQRevisionId);
		bool sendSatustoRequestor(int RFQRevisionId, string StatusTxt, string Remarks);
		bool sendCommunicationmailtoRequestor(int RFQRevisionId, string Remarks);
		bool sendMailtoBuyer(int VendorId);
		bool sendTechNotificationMail(int RFQRevisionId);
		bool sendASNMailtoBuyer(int ASNId);
		bool sendInvoiceMailtoBuyer(string invoiceNo);
		bool sendEmail(EmailSend emailobj);
		bool sendBGmail(int bgId);
		bool sendErrorLogEmail(string controllername, string methodname, string exception, Uri URL);
	}
}
