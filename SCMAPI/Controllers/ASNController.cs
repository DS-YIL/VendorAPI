using BALayer.ASN;
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

		[HttpGet]
		[Route("getAsnList/{vendorId}")]
		public IHttpActionResult getAsnList(int vendorId)
		{
			return Ok(_asnBusinessAccess.getAsnList(vendorId));
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


		[HttpGet]
		[Route("getPOInvoiceDetailsbyVendor/{vendorId}")]
		public IHttpActionResult getPOInvoiceDetailsbyVendor(int vendorId)
		{
			return Ok(_asnBusinessAccess.getPOInvoiceDetailsbyVendor(vendorId));
		}

		[Route("updateASNComminications")]
		[HttpPost]
		public IHttpActionResult updateASNComminications([FromBody] RemoteASNCommunication model)
		{
			return Ok(_asnBusinessAccess.updateASNComminications(model));
		}


		[HttpPost]
		[Route("UpdateInvoice")]
		[ResponseType(typeof(statuscheckmodel))]
		public async Task<IHttpActionResult> UpdateInvoice(InvoiceDetail invoiceModel)
		{
			return Ok(await _asnBusinessAccess.UpdateInvoice(invoiceModel));
		}


		[HttpGet]
		[Route("GetInvoiceDetails/{InvoiceNo}")]
		public IHttpActionResult GetInvoiceDetails(string InvoiceNo)
		{
			return Ok(_asnBusinessAccess.GetInvoiceDetails(InvoiceNo));
		}

		[HttpGet]
		[Route("DeleteInvoiceFile/{DocumentId}")]
		public IHttpActionResult DeleteInvoiceFile(int DocumentId)
		{
			return Ok(_asnBusinessAccess.DeleteInvoiceFile(DocumentId));
		}
	}
}
