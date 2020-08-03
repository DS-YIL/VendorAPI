using DALayer.ASN;
using SCMModels;
using SCMModels.ASNModels;
using SCMModels.RemoteModel;
using SCMModels.RFQModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace BALayer.ASN
{
   public class ASNBA : IASNBA
    {
        // public readonly IASNDA _asnDataAcess;
        //public ASNBA(IASNDA ASNDA)
        //{
        //    this._asnDataAcess = ASNDA;
        //}
        ASNDA _asnDataAcess = new ASNDA();

        public  Task<statuscheckmodel> CreateAsn(ASNShipmentHeaderModel model)
        {
            return  _asnDataAcess.CreateAsn(model);
        }

        public List<ASNShipmentHeader> getAsnList()
        {
            return _asnDataAcess.getAsnList();
        }

        public ASNShipmentHeaderModel getAsnDetailsByAsnNo(int asnNo)
        {
            return _asnDataAcess.getAsnDetailsByAsnNo(asnNo);
        }

        public Task<statuscheckmodel> EditAsn(ASNShipmentHeaderModel model)
        {
            return _asnDataAcess.EditAsn(model);
        }

        public List<StagingPoSapModels> getPONumbersbyVendor(int vendorId)
        {
            return _asnDataAcess.getPONumbersbyVendor(vendorId);
        }
        public List<DocumentDetailsInvoice> InsertDocuments_Invoice(List<DocumentDetailsInvoice> model)
        {
            return _asnDataAcess.InsertDocuments_Invoice(model);
        }
            public Task<statuscheckmodel> CreateInvoice(InvoiceModel invoiceModel )
        {
        
            return _asnDataAcess.CreateInvoice(invoiceModel);

        }

        public Task<statuscheckmodel> EditInvoice(InvoiceModel invoiceModel)
        {
            return _asnDataAcess.EditInvoice(invoiceModel);
        }

        public InvoiceModel GetInvoiceDetails(string invoiceNo)
        {
            return _asnDataAcess.GetInvoiceDetails(invoiceNo);
        }


    }
}

