using System.Web;
using System.Web.Mvc;

namespace DynamicsNAV365_StaffPortal
{
	public class FilterConfig
	{
		public static void RegisterGlobalFilters(GlobalFilterCollection filters)
		{
			filters.Add(new HandleErrorAttribute());
		}
	}
}
