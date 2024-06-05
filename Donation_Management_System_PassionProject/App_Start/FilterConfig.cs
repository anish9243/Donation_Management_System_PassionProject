using System.Web;
using System.Web.Mvc;

namespace Donation_Management_System_PassionProject
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
