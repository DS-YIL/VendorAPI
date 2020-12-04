using SCMModels;
using SCMModels.RFQModels;
using SCMModels.SCMModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Data.SqlClient;
using System.Linq;
using System.DirectoryServices.AccountManagement;
using System.Web;
using SCMModels.RemoteModel;

namespace DALayer.Login
{
    public class LoginDA : ILoginDA
    {
        YSCMEntities DB = new YSCMEntities();
        VSCMEntities vscm = new VSCMEntities();
        

        //[WebMethod(enableSession: true)]
        public EmployeeModel ValidateLoginCredentials(DynamicSearchResult Result)
        {
            //var session = HttpContext.Current.Session;
            EmployeeModel employee = new EmployeeModel();
            string[] UserCredentials = Result.columnValues.Split(',');
            string Id = UserCredentials[0].ToString();
            PrincipalContext ctx = new PrincipalContext(ContextType.Domain);

            UserPrincipal user = UserPrincipal.FindByIdentity(ctx, UserCredentials[0].Trim());
            if (user != null)
            {
                if (ctx.ValidateCredentials(UserCredentials[0], UserCredentials[1]))
                {
                    var data = DB.Employees.Where(li => li.DomainId == Id).FirstOrDefault();
                    if (data != null)
                    {
                        employee.EmployeeNo = data.EmployeeNo;
                        employee.Name = data.Name;
                        employee.EMail = data.EMail;
                        employee.DeptID = data.DepartmentId;
                        employee.RoleId = data.RoleId;
                    }
                }
            }
            else if (user == null)
            {
                var data = DB.Employees.Where(li => li.DomainId == Id).FirstOrDefault();
                if (data != null)
                {
                    employee.EmployeeNo = data.EmployeeNo;
                    employee.Name = data.Name;
                    employee.EMail = data.EMail;
                    employee.DeptID = data.DepartmentId;
                    employee.RoleId = data.RoleId;
                }



                //else
                //{
                //    InValidUser();
                //}
            }
            //if (session != null)
            //{
            //    session["name"] = employee.Name;
            //    session["id"] = employee.EmployeeNo.ToString();
            //    string SessionID = session.SessionID;
            //}
            return employee;
        }

        public VendorModel ValidateLoginCreds(VendorModel model)
        {
         
            VendorModel vendor = new VendorModel();
            string[] UserCredentials = model.columnValues.Split(',');
            string Id = UserCredentials[0].ToString();
            string password = UserCredentials[1].ToString();
           // PrincipalContext ctx = new PrincipalContext(ContextType.Domain);
            
            using (VSCMEntities db = new VSCMEntities())
            {

                var person = (from p in vscm.RemoteVendorMasters
                              join e in vscm.RemoteVendorUserMasters
                              on p.Vendorid equals e.VendorId
                              where e.VuniqueId == Id  && e.pwd== password
                              select new
                              {
                                  e.Vuserid,
                                  p.VendorName,
                                  e.VuniqueId,
                                  e.VendorId
                              }).ToList();
                foreach (var p in person)
                {
                    Console.WriteLine("{0} {1}", p.Vuserid, p.VendorName);
                    vendor.Vuserid = p.Vuserid;
                    vendor.UserName = p.VendorName;
                    vendor.VUniqueId = p.VuniqueId;
                    vendor.vendorId = p.VendorId;
                }
            }

            return vendor;
        }
    }

}
