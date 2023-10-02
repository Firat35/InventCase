using Microsoft.AspNetCore.Mvc.Rendering;

namespace Web.Models
{
    public class StoreProfitModel
    {
        public int StoreId { get; set; }
        public int? StoreProfit { get; set; }
        public List<SelectListItem> Stores { set; get; }
    }
}
