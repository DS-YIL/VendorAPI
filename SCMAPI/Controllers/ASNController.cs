using BALayer.ASN;
using SCMModels;
using SCMModels.RemoteModel;
using SCMModels.RFQModels;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

namespace SCMAPI.Controllers
{
	[Authorize]
	[RoutePrefix("Api/ASN")]
	public class ASNController : ApiController
	{
		// private readonly IASNBA _asnBusinessAccess;

		private readonly ASNBA _asnBusinessAccess;
		public ASNController(ASNBA ASNBA)
		{
			this._asnBusinessAccess = ASNBA;
		}
		[Route("InsertandEditAsn")]
		[HttpPost]
		public IHttpActionResult InsertandEditAsn([FromBody] RemoteASNShipmentHeader model)
		{
			return Ok(_asnBusinessAccess.InsertandEditAsn(model));
		}

		[HttpPost]
		[Route("getAsnList")]
		public IHttpActionResult getAsnList(ASNfilters ASNfilters)
		{
			return Ok(_asnBusinessAccess.getAsnList(ASNfilters));
		}

		[HttpGet]
		[Route("getAsnDetails/{ASNId}")]
		public IHttpActionResult getAsnDetailsByAsnNo(int ASNId)
		{
			return Ok(_asnBusinessAccess.getAsnDetailsByAsnNo(ASNId));
		}


		[HttpGet]
		[Route("getPONumbersByVendor/{vendorId}")]
		public IHttpActionResult getPONumbersbyVendor(int vendorId)
		{
			return Ok(_asnBusinessAccess.getPONumbersbyVendor(vendorId));
		}
		[HttpGet]
		[Route("getItemDetailsByPoNo/{PONo}")]
		public IHttpActionResult getItemDetailsByPoNo(string PONo)
		{
			return Ok(_asnBusinessAccess.getItemDetailsByPoNo(PONo));
		}


		//[HttpGet]
		//[Route("getPOInvoiceDetailsbyVendor/{vendorId}")]
		//public IHttpActionResult getPOInvoiceDetailsbyVendor(int vendorId)
		//{
		//	return Ok(_asnBusinessAccess.getPOInvoiceDetailsbyVendor(vendorId));
		//}
		

		[Route("updateASNComminications")]
		[HttpPost]
		public IHttpActionResult updateASNComminications([FromBody] RemoteASNCommunication model)
		{
			return Ok(_asnBusinessAccess.updateASNComminications(model));
		}


		[HttpPost]
		[Route("UpdateInvoice")]
		[ResponseType(typeof(statuscheckmodel))]
		public async Task<IHttpActionResult> UpdateInvoice(RemoteInvoiceDetail invoiceModel)
		{
			return Ok(await _asnBusinessAccess.UpdateInvoice(invoiceModel));
		}


		[HttpPost]
		[Route("GetInvoiceDetails")]
		public IHttpActionResult GetInvoiceDetails(RemoteInvoiceDetail invoiceDetails)
		{
			return Ok(_asnBusinessAccess.GetInvoiceDetails(invoiceDetails));
		}

		[HttpGet]
		[Route("DeleteInvoiceFile/{DocumentId}")]
		public IHttpActionResult DeleteInvoiceFile(int DocumentId)
		{
			return Ok(_asnBusinessAccess.DeleteInvoiceFile(DocumentId));
		}
	}
}
