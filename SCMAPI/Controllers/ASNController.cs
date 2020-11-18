using BALayer.ASN;
using SCMModels;
using SCMModels.ASNModels;
using SCMModels.RemoteModel;
using SCMModels.RFQModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;

namespace SCMAPI.Controllers
{
	[Authorize]
	[RoutePrefix("Api/ASN")]
	public class ASNController : ApiController
	{
		// private readonly IASNBA _asnBusinessAccess;

		ASNBA _asnBusinessAccess = new ASNBA();
		//public ASNController(IASNBA asnBA)
		//{
		//    this._asnBusinessAccess = asnBA;
		//}

		[Route("CreateAsn")]
		[HttpPost]
		public IHttpActionResult CreateAsn([FromBody] ASNShipmentHeaderModel model)
		{
			return Ok( _asnBusinessAccess.CreateAsn(model));
		}

		[HttpGet]
		[Route("getAsnList")]
		public IHttpActionResult getAsnList()
		{
			return Ok(_asnBusinessAccess.getAsnList());
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
		[Route("getPOInvoiceDetailsbyVendor/{vendorId}")]
		public IHttpActionResult getPOInvoiceDetailsbyVendor(int vendorId)
		{
			return Ok(_asnBusinessAccess.getPOInvoiceDetailsbyVendor(vendorId));
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
