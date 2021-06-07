using SCMModels.RemoteModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCMModels.RFQModels
{
	public partial class VendorRegistrationModel
	{
		public int VendorId { get; set; }
		public string VendorCode { get; set; }
		public int UniqueId { get; set; }
		public Boolean Onetimevendor { get; set; }
		public Boolean EvaluationRequired { get; set; }
		public bool PerformanceVerificationRequired { get; set; }
		public bool MSMERequired { get; set; }

		public string VendorNoInSAP { get; set; }
		public DateTime? RequestedOn { get; set; }
		public string VendorName { get; set; }
		public string VendorAddress { get; set; }
		public string PostalCode { get; set; }
		public string City { get; set; }
		public string State { get; set; }
		public string LocalBranchOffice { get; set; }
		public string PhoneAndExtn { get; set; }
		public string Fax { get; set; }
		public string ContactPerson { get; set; }
		public string Phone { get; set; }
		public string ContactPersonForSales { get; set; }
		public string PhoneNumberForSales { get; set; }
		public string EmailIdForSales { get; set; }
		public string AltEmailidForSales { get; set; }
		public string ContactPersonForOperations { get; set; }
		public string PhoneNumberForOperations { get; set; }
		public string EmailIdForOperations { get; set; }
		public string AltEmailidForOperations { get; set; }
		public string ContactPersonForLogistics { get; set; }
		public string PhoneNumberForLogistics { get; set; }
		public string EmailIdForLogistics { get; set; }
		public string AltEmailidForLogistics { get; set; }
		public string ContactPersonForAccounts { get; set; }
		public string PhoneNumberForAccounts { get; set; }
		public string EmailIdForAccounts { get; set; }
		public string AltEmailidForAccounts { get; set; }
		public string GSTNo { get; set; }
		public int? NatureofBusiness { get; set; }
		public string SpecifyNatureOfBusiness { get; set; }
		public string PANNo { get; set; }
		public string CINNo { get; set; }
		public string TanNo { get; set; }
		public string PaymentTerms { get; set; }
		public string Street { get; set; }

		public string Location { get; set; }
		public string BusinessArea { get; set; }
		public string Mobile { get; set; }
		public string Email { get; set; }
		public string AltEmail { get; set; }
		public string contPhone { get; set; }
		public string BankDetails { get; set; }
		public string BankerName { get; set; }
		public string LocationOrBranch { get; set; }
		public string AccNo { get; set; }
		public string IFSCCode { get; set; }
		public string IncoTerms { get; set; }
		public string AccountHolderName { get; set; }
		public string PhysicalPath { get; set; }
		public int DocumentationTypeId { get; set; }
		public int StateId { get; set; }
		public string fileattach1 { get; set; }
		public string SwiftCode { get; set; }
		public Nullable<int> CurrencyId { get; set; }
		public string CurrencyName { get; set; }
		public Nullable<bool> VendorType { get; set; }
		public string Country { get; set; }
		public Nullable<bool> TermsAndConditions { get; set; }
		public Nullable<bool> Guidelines { get; set; }

		public List<RemoteVendorRegisterDocumentDetail> DocDetailsLists = new List<RemoteVendorRegisterDocumentDetail>();


	}
	public class documentDetails
	{
		public string PhysicalPath { get; set; }
		public int DocumentationTypeId { get; set; }
		public int VendorId { get; set; }
		public bool status { get; set; }
		public int Id { get; set; }
		public string DocumentName { get; set; }
		public string UploadedBy { get; set; }
		public Nullable<System.DateTime> UploadedOn { get; set; }
		public Nullable<bool> Status { get; set; }
		public Nullable<bool> Deleteflag { get; set; }


	}
	public class StateList
	{
		public string StateName { get; set; }
		public int StateId { get; set; }

	}
	public class NatureOfBusiness
	{
		public string NatureofbusinessName { get; set; }
		public int NaturofBusinessId { get; set; }
	}
	public class DocumentType
	{
		public string DocumentName { get; set; }
		public int DocId { get; set; }
	}
	public class RFQTerms
	{
		public string Terms { get; set; }
		public int VRfqTermsid { get; set; }
		public string remarks { get; set; }
		public string VendorResponse { get; set; }
		public string status { get; set; }
		public string errmsg { get; set; }
		public int RfqRevisionId { get; set; }

	}
	//public class DocumentTypeMaster
	//{
	//	public int DocumenTypeId { get; set; }
	//	public string DocumentTypeName { get; set; }
	//	public DateTime? CreatedOn { get; set; }
	//	public string CreatedBy { get; set; }
	//}

	public class VendorCommunicaton
	{
		public int RfqCCid { get; set; }
		public Nullable<int> RfqItemsId { get; set; }
		public int RfqRevisionId { get; set; }
		public string RemarksFrom { get; set; }
		public string RemarksTo { get; set; }
		public Nullable<bool> SendEmail { get; set; }
		public Nullable<bool> SetReminder { get; set; }
		public Nullable<System.DateTime> ReminderDate { get; set; }
		public System.DateTime? RemarksDate { get; set; }
		public string Remarks { get; set; }
		public bool DeleteFlag { get; set; }
		public List<VendorCommunicaton> GetRfqCommunication { get; set; }
	}
	public class Changepassword
	{
		public string CurrentPassword { get; set; }
		public string NewPassword { get; set; }
		public string ConfirmPassword { get; set; }
		public int VendorId { get; set; }
		public string emailid { get; set; }

	}
	public class forgetpassword
	{

		public string NewPassword { get; set; }
		public string ConfirmPassword { get; set; }
		public DateTime Expirydatetime { get; set; }
		public string URL { get; set; }
		public string emailid { get; set; }
		public DateTime URLCreatedOn { get; set; }
		public string token { get; set; }
		public string tokenduration { get; set; }
		public string baseURL { get; set; }
		public string fromemail { get; set; }

	}

}
