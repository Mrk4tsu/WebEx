using System.Web;
using System.Web.Mvc;

namespace CNTT_CLC2_NguyenDucThang63135414
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
