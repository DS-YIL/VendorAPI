using BALayer.ASN;
using SCMModels;
using SCMModels.ASNModels;
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
        public  IHttpActionResult getPONumbersbyVendor(int vendorId)
        {
            return Ok(_asnBusinessAccess.getPONumbersbyVendor(vendorId));
        }

        [HttpPost]
        [Route("CreateInvoice")]
        [ResponseType(typeof(statuscheckmodel))]
        public async Task<IHttpActionResult> CreateInvoice(InvoiceModel invoiceModel)
        {
            statuscheckmodel status = new statuscheckmodel();
             status = await _asnBusinessAccess.CreateInvoice(invoiceModel);
            return Ok(status);
        }

        [HttpPost]
        [Route("EditInvoice")]
        [ResponseType(typeof(statuscheckmodel))]
        public async Task<IHttpActionResult> EditInvoice(InvoiceModel invoiceModel)
        {
            statuscheckmodel status = new statuscheckmodel();
            status = await _asnBusinessAccess.EditInvoice(invoiceModel);
            return Ok(status);
        }

        [HttpGet]
        [Route("GetInvoiceDetails/{InvoiceNo}")]
        public IHttpActionResult GetInvoiceDetails(string InvoiceNo)
        {
            return Ok(_asnBusinessAccess.GetInvoiceDetails(InvoiceNo));
        }


        [HttpPost]
        [Route("UploadFileInvoice")]
        public List<DocumentDetailsInvoice> UploadFile()
        {
            try
            {

            
            var filePath = "";
            List<DocumentDetailsInvoice> listobj = new List<DocumentDetailsInvoice>();
            var httpRequest = HttpContext.Current.Request;
            if (httpRequest.Files.Count > 0)
            {
                string filename = string.Empty;
                string[] listdata;
                    string docid, vendorid;
                    int DocumentId;
                foreach (string file in httpRequest.Files)
                {
                    filename = file;
                    break;
                }
                listdata = filename.Split('_');
                docid = listdata[0];
                
                //if(docid == "1")
                //{
                    vendorid = listdata[2];
                    DocumentId = Convert.ToInt32(listdata[3].Remove(0, 5));
                //}
                //else
                //{
                //    vendorid = listdata[2];
                //    DocumentId = Convert.ToInt32(listdata[3].Remove(0, 5));
                //}

                for (int i = 0; i <= httpRequest.Files.Count - 1; i++)
                {
                    var postedFile = httpRequest.Files[i];
                    filePath = ConfigurationManager.AppSettings["AttachedDocPath"] + "\\" + docid + "_" + vendorid;
                    string dbfilePath = docid + "_" + vendorid + "\\" + postedFile.FileName;
                    if (!Directory.Exists(filePath))
                        Directory.CreateDirectory(filePath);
                    filePath = Path.Combine(filePath, postedFile.FileName);
                    postedFile.SaveAs(filePath);
                    List<DocumentDetailsInvoice> obj = new List<DocumentDetailsInvoice>();
                    DocumentDetailsInvoice eachobj = new DocumentDetailsInvoice();
                    eachobj.DocumentName = docid;
                    eachobj.Path = dbfilePath;
                    eachobj.vendorId = Convert.ToInt32(vendorid);
                    eachobj.DocumentId = DocumentId;
                        if (docid == "Other")
                            eachobj.DocumentName = listdata[4].Remove(0, 7);
                    obj.Add(eachobj);
                    listobj = _asnBusinessAccess.InsertDocuments_Invoice(obj);
                }
            }
            return listobj;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

    }
}
