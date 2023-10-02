using Microsoft.AspNetCore.Mvc.Rendering;

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Web.Models
{
    public class SalesHistoryModel
    {
        public int Id { get; set; }
        //[DisplayName("Product Name")]
        public string ProductName { get; set; }
        public int Cost { get; set; }
        public int SalesPrice { get; set; }
        public string StoreName { get; set; }
        public DateTime Date { get; set; }
        public int SalesQuantity { get; set; }
        public int Stock { get; set; }
        public int ProductId { get; set; }
        public int StoreId { get; set; }
        public List<SelectListItem> Products { set; get; }
        public List<SelectListItem> Stores { set; get; }

    }
}
