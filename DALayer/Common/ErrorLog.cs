using SCMModels.RemoteModel;
using System;
using System.Data.SqlClient;
using System.Web;

namespace DALayer.Common
{
	public class ErrorLog
	{
		public void ErrorMessage(string controllername, string methodname, string exception)
		{
			exception = exception.Replace("'", String.Empty);
			VSCMEntities DB = new VSCMEntities();
			string query = "insert into dbo.RemoteApiErrorLog(ControllerName,MethodName,ExceptionMsg,OccuredDate,URL)values('" + controllername+"', '"+methodname+"', '"+exception+ "','"+ DateTime.Now + "','" + HttpContext.Current.Request.Url + "')";
			SqlConnection con = new SqlConnection(DB.Database.Connection.ConnectionString);
			SqlCommand cmd = new SqlCommand(query, con);
			con.Open();
			cmd.ExecuteNonQuery();
			con.Close();
			

		}
	}
}

