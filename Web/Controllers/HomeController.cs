using Microsoft.AspNetCore.Mvc;

using System.Data.SqlClient;
using System.Data;
using System.Diagnostics;

using Web.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Identity;

namespace Web.Controllers
{
    public class HomeController : Controller
    {
        string connectionString = "Server = localhost\\SQLEXPRESS;Database=Invent;Trusted_Connection=True;";

        public IActionResult Index()
        {
            var res = new DataModel();
            DataTable dtbl = new DataTable();
            using (SqlConnection sqlCon = new SqlConnection(connectionString))
            {
                sqlCon.Open();
                var query = "SELECT TOP 1 s.StoreName, Sum( (p.SalesPrice - p.Cost ) * (iss.SalesQuantity + iss.Stock ) )  as potentialProfit FROM InventorySales iss JOIN Products p ON iss.ProductId = p.Id JOIN Stores s ON iss.StoreId = s.Id GROUP BY s.StoreName ORDER BY potentialProfit DESC";
                SqlDataAdapter sqlDa = new SqlDataAdapter(query, sqlCon);
                sqlDa.Fill(dtbl);
            }
            if (dtbl.Rows.Count == 1)
            {
                res.MostProfitableStore = dtbl.Rows[0][0].ToString();
            }

            DataTable dtbl1 = new DataTable();
            using (SqlConnection sqlCon = new SqlConnection(connectionString))
            {
                sqlCon.Open();
                var query = "SELECT TOP 1 p.ProductName, Sum( iss.SalesQuantity ) as totalSalesQuantity FROM InventorySales iss JOIN Products p ON iss.ProductId = p.Id GROUP BY p.ProductName ORDER BY totalSalesQuantity DESC";
                SqlDataAdapter sqlDa = new SqlDataAdapter(query, sqlCon);
                sqlDa.Fill(dtbl1);
            }
            if (dtbl1.Rows.Count == 1)
            {
                res.BestSellerProduct = dtbl1.Rows[0][0].ToString();
            }
            return View(res);
        }

        // GET: ProfitForStore
        public ActionResult ProfitForStore()
        {
            var storeOptions = new List<SelectListItem>();
            using (SqlConnection sqlCon = new SqlConnection(connectionString))
            {
                sqlCon.Open();
                using (var cmdProducts = new SqlCommand("SELECT Id, StoreName FROM Stores", sqlCon))
                {
                    var rdrStores = cmdProducts.ExecuteReader();
                    while (rdrStores.Read())
                    {
                        var o = new SelectListItem
                        {
                            //Value = rdrProducts.GetInt32(rdrProducts.GetOrdinal("Id")).ToString(),
                            Value = rdrStores.GetInt32(rdrStores.GetOrdinal("Id")).ToString(),
                            Text = rdrStores.GetString(rdrStores.GetOrdinal("StoreName"))
                        };
                        storeOptions.Add(o);
                    }
                }
            }
            StoreProfitModel storeProfitModel = new StoreProfitModel();
            storeProfitModel.Stores = storeOptions;
            return View(storeProfitModel);
        }

        //POST: SalesHistoryController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ProfitForStore(StoreProfitModel storeProfitModel)
        {
            var storeOptions = new List<SelectListItem>();
            using (SqlConnection sqlCon = new SqlConnection(connectionString))
            {
                sqlCon.Open();
                using (var cmdProducts = new SqlCommand("SELECT Id, StoreName FROM Stores", sqlCon))
                {
                    var rdrStores = cmdProducts.ExecuteReader();
                    while (rdrStores.Read())
                    {
                        var o = new SelectListItem
                        {
                            //Value = rdrProducts.GetInt32(rdrProducts.GetOrdinal("Id")).ToString(),
                            Value = rdrStores.GetInt32(rdrStores.GetOrdinal("Id")).ToString(),
                            Text = rdrStores.GetString(rdrStores.GetOrdinal("StoreName"))
                        };
                        storeOptions.Add(o);
                    }
                }
            }
            storeProfitModel.Stores = storeOptions;

            DataTable dtblProfit = new DataTable();
            using (SqlConnection sqlCon = new SqlConnection(connectionString))
            {
                sqlCon.Open();
                var query = " SELECT s.Id , Sum((p.SalesPrice - p.Cost) * iss.SalesQuantity) FROM InventorySales iss JOIN Products p ON iss.ProductId = p.Id JOIN Stores s ON iss.StoreId = s.Id Where s.Id= @Id GROUP BY s.Id";
                SqlDataAdapter sqlDa = new SqlDataAdapter(query, sqlCon);
                sqlDa.SelectCommand.Parameters.AddWithValue("@Id", storeProfitModel.StoreId);
                sqlDa.Fill(dtblProfit);
            }
            if (dtblProfit.Rows.Count == 1)
            {
                storeProfitModel.StoreProfit = int.Parse(dtblProfit.Rows[0][1].ToString());
                return View(storeProfitModel);
            }
            else
                return RedirectToAction("Index");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}