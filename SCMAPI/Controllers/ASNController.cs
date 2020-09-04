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
		[ResponseType(typeof(statuscheckmodel))]
		[HttpPost]
		public async Task<IHttpActionResult> CreateAsn(ASNShipmentHeaderModel model)
		{
			statuscheckmodel status = new statuscheckmodel();
			status = await _asnBusinessAccess.CreateAsn(model);
			return Ok(status);
		}

		[HttpGet]
		[Route("getAsnList")]
		public IHttpActionResult getAsnList()
		{
			return Ok(_asnBusinessAccess.getAsnList());
		}

		[HttpGet]
		[Route("getAsnDetails/{AsnNo}")]
		public IHttpActionResult getAsnDetailsByAsnNo(int asnNo)
		{
			return Ok(_asnBusinessAccess.getAsnDetailsByAsnNo(asnNo));
		}

		[Route("EditAsn")]
		[ResponseType(typeof(statuscheckmodel))]
		[HttpPost]
		public async Task<IHttpActionResult> editAsn(ASNShipmentHeaderModel model)
		{
			statuscheckmodel status = new statuscheckmodel();
			status = await _asnBusinessAccess.EditAsn(model);
			return Ok(status);
		}

		[HttpGet]
		[Route("getPONumbersByVendor/{vendorId}")]
		public IHttpActionResult getPONumbersbyVendor(int vendorId)
		{
			return Ok(_asnBusinessAccess.getPONumbersbyVendor(vendorId));
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
	}
}
