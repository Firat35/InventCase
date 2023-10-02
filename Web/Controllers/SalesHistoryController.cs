using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;

using System.Data;
using System.Data.SqlClient;

using Web.Models;

namespace Web.Controllers
{
    public class SalesHistoryController : Controller
    {
        string connectionString = "Server = localhost\\SQLEXPRESS;Database=Invent;Trusted_Connection=True;";
        // GET: SalesHistoryController
        public ActionResult Index()
        {
            DataTable dtblProduct = new DataTable();
            using (SqlConnection sqlCon = new SqlConnection(connectionString))
            {
                sqlCon.Open();
                string query = "SELECT iss.Id, p.ProductName, p.Cost, p.SalesPrice, s.StoreName, iss.Date, iss.SalesQuantity, iss.Stock FROM InventorySales iss JOIN Products p ON iss.ProductId = p.Id  JOIN Stores s ON iss.StoreId = s.Id";
                SqlDataAdapter sqlDa = new SqlDataAdapter(query, sqlCon);
                sqlDa.Fill(dtblProduct);
            }
            return View(dtblProduct);
        }

        // GET: SalesHistoryController/Create
        public ActionResult Create()
        {
            var productOptions = new List<SelectListItem>();
            var storeOptions = new List<SelectListItem>();
            using (SqlConnection sqlCon = new SqlConnection(connectionString))
            {
                sqlCon.Open();
                using (var cmdProducts = new SqlCommand("SELECT Id, ProductName FROM Products", sqlCon))
                {
                    var rdrProducts = cmdProducts.ExecuteReader();
                    while (rdrProducts.Read())
                    {
                        var o = new SelectListItem
                        {
                            Value = rdrProducts.GetInt32(rdrProducts.GetOrdinal("Id")).ToString(),
                            Text = rdrProducts.GetString(rdrProducts.GetOrdinal("ProductName"))
                        };
                        productOptions.Add(o);
                    }
                }
            }
            using (SqlConnection sqlCon = new SqlConnection(connectionString))
            {
                sqlCon.Open();
                using (var cmdStores = new SqlCommand("SELECT Id, StoreName FROM Stores", sqlCon))
                {
                    var rdrStores = cmdStores.ExecuteReader();
                    while (rdrStores.Read())
                    {
                        var o = new SelectListItem
                        {
                            Value = rdrStores.GetInt32(rdrStores.GetOrdinal("Id")).ToString(),
                            Text = rdrStores.GetString(rdrStores.GetOrdinal("StoreName"))
                        };
                        storeOptions.Add(o);
                    }
                }
            }
            SalesHistoryModel salesHistoryModel = new SalesHistoryModel();
            salesHistoryModel.Products = productOptions;
            salesHistoryModel.Stores = storeOptions;
            return View(salesHistoryModel);
        }

        // POST: SalesHistoryController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(SalesHistoryModel salesHistoryModel)
        {
            using (SqlConnection sqlCon = new SqlConnection(connectionString))
            {
                sqlCon.Open();

                string commandInventorySales = "INSERT INTO InventorySales VALUES(@ProductId, @StoreId ,@Date,@SalesQuantity,@Stock)";
                SqlCommand sqlCmd2 = new SqlCommand(commandInventorySales, sqlCon);
                sqlCmd2.Parameters.AddWithValue("@ProductId", salesHistoryModel.ProductId);
                sqlCmd2.Parameters.AddWithValue("@StoreId", salesHistoryModel.StoreId);
                sqlCmd2.Parameters.AddWithValue("@Date", salesHistoryModel.Date);
                sqlCmd2.Parameters.AddWithValue("@SalesQuantity", salesHistoryModel.SalesQuantity);
                sqlCmd2.Parameters.AddWithValue("@Stock", salesHistoryModel.Stock);
                sqlCmd2.ExecuteNonQuery();
            }
            return RedirectToAction("Index");
        }

        // GET: SalesHistoryController/Edit/5
        public ActionResult Edit(int id)
        {
            SalesHistoryModel salesHistoryModel = new SalesHistoryModel();
            DataTable dtblProduct = new DataTable();
            using (SqlConnection sqlCon = new SqlConnection(connectionString))
            {
                sqlCon.Open();
                string query = "SELECT iss.Id, p.ProductName, p.Cost, p.SalesPrice, s.StoreName, iss.Date, iss.SalesQuantity, iss.Stock, p.Id, s.Id FROM InventorySales iss JOIN Products p ON iss.ProductId = p.Id  JOIN Stores s ON iss.StoreId = s.Id Where iss.Id = @SalesHistoryId";
                SqlDataAdapter sqlDa = new SqlDataAdapter(query, sqlCon);
                sqlDa.SelectCommand.Parameters.AddWithValue("@SalesHistoryId", id);
                sqlDa.Fill(dtblProduct);
            }
            if (dtblProduct.Rows.Count == 1)
            {
                salesHistoryModel.Id = int.Parse(dtblProduct.Rows[0][0].ToString());
                salesHistoryModel.ProductName = dtblProduct.Rows[0][1].ToString();
                salesHistoryModel.Cost = int.Parse(dtblProduct.Rows[0][2].ToString());
                salesHistoryModel.SalesPrice = int.Parse(dtblProduct.Rows[0][3].ToString());
                salesHistoryModel.StoreName = dtblProduct.Rows[0][4].ToString();
                salesHistoryModel.Date = DateTime.Parse(dtblProduct.Rows[0][5].ToString());
                salesHistoryModel.SalesQuantity = int.Parse(dtblProduct.Rows[0][6].ToString());
                salesHistoryModel.Stock = int.Parse(dtblProduct.Rows[0][7].ToString());
                salesHistoryModel.ProductId = int.Parse(dtblProduct.Rows[0][8].ToString());
                salesHistoryModel.StoreId = int.Parse(dtblProduct.Rows[0][9].ToString());
                return View(salesHistoryModel);
            }
            else
                return RedirectToAction("Index");
        }

        // POST: SalesHistoryController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(SalesHistoryModel salesHistoryModel)
        {
            using (SqlConnection sqlCon = new SqlConnection(connectionString))
            {
                sqlCon.Open();
                string query = "UPDATE InventorySales SET Date = @Date, SalesQuantity= @SalesQuantity , Stock = @Stock Where Id= @Id ;";
                SqlCommand sqlCmd = new SqlCommand(query, sqlCon);
                sqlCmd.Parameters.AddWithValue("@Id", salesHistoryModel.Id);
                sqlCmd.Parameters.AddWithValue("@Date", salesHistoryModel.Date.Date);
                sqlCmd.Parameters.AddWithValue("@SalesQuantity", salesHistoryModel.SalesQuantity);
                sqlCmd.Parameters.AddWithValue("@Stock", salesHistoryModel.Stock);
                sqlCmd.ExecuteNonQuery();
            }
            return RedirectToAction("Index");
        }

        // GET: SalesHistoryController/Delete/5
        public ActionResult Delete(int id)
        {
            using (SqlConnection sqlCon = new SqlConnection(connectionString))
            {
                sqlCon.Open();
                string query = "DELETE FROM InventorySales  Where Id= @Id ;";
                SqlCommand sqlCmd = new SqlCommand(query, sqlCon);
                sqlCmd.Parameters.AddWithValue("@Id", id);
                sqlCmd.ExecuteNonQuery();
            }
            return RedirectToAction("Index");
        }
    }
}
