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
		bool sendCommunicationmailtoRequestor(int RFQRevisionId, string Remarks);
		bool sendMailtoBuyer(int VendorId);
		bool sendTechNotificationMail(int RFQRevisionId);
	}
}
