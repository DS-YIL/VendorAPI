using SCMModels;
using SCMModels.RemoteModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SCMAPI
{
    public class UserMasterRepository : IDisposable
    {
        // SECURITY_DBEntities it is your context class
        VSCMEntities db = new VSCMEntities();
        VendorModel vendor = null;
        //This method is used to check and validate the user credentials
        public VendorModel ValidateUser(string username, string password)
        {
            var person = (from p in db.RemoteVendorMasters
                          join e in db.RemoteVendorUserMasters
                          on p.Vendorid equals e.VendorId
                          join f in db.RemoteVendorMasters
                          on  p.Vendorid equals f.Vendorid
                          where e.VuniqueId == username && e.pwd == password
                          select new
                          {
                              e.Vuserid,
                              p.VendorName,
                              e.VuniqueId,
                              e.VendorId,
                              f.VendorCode
                          }).ToList();
            if(person.Count!=0)
            {
                foreach (var p in person)
                {
                  //  Console.WriteLine("{0} {1}", p.Vuserid, p.VendorName);
                    vendor = new VendorModel();
                    vendor.Vuserid = p.Vuserid!=null ? p.Vuserid:string.Empty;
                    vendor.UserName = p.VendorName != null ? p.VendorName : string.Empty;
                    vendor.VUniqueId = p.VuniqueId != null ? p.VuniqueId : string.Empty;
                    vendor.vendorId = p.VendorId;
                    
                        vendor.VendorCode =p.VendorCode!= null ? p.VendorCode : string.Empty;


                    // vendor.VendorCode = string.IsNullOrEmpty(p.VendorCode) ? DBNull.Value : p.VendorCode;//if(p.VendorCode==DBNull) ;
                }
            }
           
            return vendor;
        }

        public void Dispose()
        {
            db.Dispose();
        }
    }
}