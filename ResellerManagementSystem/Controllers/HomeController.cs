﻿using ResellerManagementSystem.Helper;
using ResellerManagementSystem.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace ResellerManagementSystem.Controllers
{
    public class HomeController : Controller
    {
        [AuthorizationFilter]
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult AccessDenied()
        {
            return View();
        }

        [AuthorizationFilter]
        public ActionResult UserCreate()
        {
            return View();
        }

        public ActionResult Login()
        {
            return View();
        }


        public JsonResult CheckLogin(string username, string password)
        {
            ResellerDbEntities2 db = new ResellerDbEntities2();
            string md5StringPassword = AppHelper.GetMd5Hash(password);
            var dataItem = db.Users.Where(x => x.Username == username && x.Password == md5StringPassword).SingleOrDefault();
            bool isLogged = true;
            if (dataItem != null)
            {
                Session["Username"] = dataItem.Username;
                Session["Role"] = dataItem.Role;
                isLogged = true;
            }
            else
            {
                isLogged = false;
            }
            return Json(isLogged, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult SaveUser(User user)
        {
            ResellerDbEntities2 db = new ResellerDbEntities2();
            bool isSuccess = true;

            if (user.UserId > 0)
            {
                db.Entry(user).State = EntityState.Modified;
            }
            else
            {
                user.Status = 1;
                user.Password = AppHelper.GetMd5Hash(user.Password);
                db.Users.Add(user);
            }
            try
            {
                db.SaveChanges();
            }
            catch (Exception)
            {
                isSuccess = false;
            }

            return Json(isSuccess, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetAllUser()
        {
            ResellerDbEntities2 db = new ResellerDbEntities2();
            var dataList = db.Users.Where(x => x.Status == 1).ToList();
            return Json(dataList, JsonRequestBehavior.AllowGet);
        }

        [AuthorizationFilter]
        public ActionResult Category()
        {
            return View();
        }

        [HttpPost]
        public JsonResult SaveCategory(Category cat)
        {
            ResellerDbEntities2 db = new ResellerDbEntities2();
            bool isSuccess = true;
            if (cat.CategoryId > 0)
            {
                db.Entry(cat).State = EntityState.Modified;
            }
            else
            {
                cat.Status = 1;
                db.Categories.Add(cat);
            }
            try
            {
                db.SaveChanges();
            }
            catch (Exception)
            {
                isSuccess = false;
            }
            return Json(isSuccess, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetAllGetegory()
        {
            ResellerDbEntities2 db = new ResellerDbEntities2();
            var dataList = db.Categories.Where(x => x.Status == 1).ToList();
            var data = dataList.Select(x => new
            {
                CategoryId = x.CategoryId,
                Name = x.Name,
                Status = x.Status
            });
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Logout()
        {
            Session["Username"] = null;
            Session["Role"] = null;
            return RedirectToAction("Login");
        }

        [AuthorizationFilter]
        public ActionResult Product()
        {
            return View();
        }

        [HttpPost]
        public JsonResult SaveProduct(Product product)
        {
            ResellerDbEntities2 db = new ResellerDbEntities2();
            bool isSuccess = true;

            if (product.ProductId > 0)
            {
                db.Entry(product).State = EntityState.Modified;
            }
            else
            {
                db.Products.Add(product);
            }
            try
            {
                db.SaveChanges();
            }
            catch (Exception)
            {
                isSuccess = false;
            }

            return Json(isSuccess, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetAllProduct()
        {

            ResellerDbEntities2 db = new ResellerDbEntities2();
            var dataList = db.Products.Include("Category")
                                      .Where(prd => prd.Status == 1) // Assuming Status 1 means active
                                      .Select(prd => new
                                      {
                                          ProductId = prd.ProductId,
                                          Name = prd.Name,
                                          CategoryName = prd.Category.Name,
                                          Status = prd.Status
                                      })
                                      .ToList();

            return Json(dataList, JsonRequestBehavior.AllowGet);
           

            /*
            ResellerDbEntities db = new ResellerDbEntities();
            var dataList = db.Products.Select(prd => new
            {
                ProductId = prd.ProductId,
                Name = prd.Name
            }).ToList();

            return Json(dataList, JsonRequestBehavior.AllowGet);
            


            
            ResellerDbEntities db = new ResellerDbEntities();
            var dataList = (from prd in db.Products.Include("Category").ToList()
                            join stk in db.ProductStocks on prd.ProductId equals stk.ProductId
                            where stk.Quantity > 0
                            select new
                            {
                                ProductId = prd.ProductId,
                                CategoryId = prd.CategoryId,
                                Name = prd.Name,
                                Status = prd.Status,
                                CategoryName = prd.Category.Name,
                                PurchasePrice = stk.PurchasePrice,
                                SalesPrice = stk.SalesPrice
                            }).ToList();


            return Json(dataList, JsonRequestBehavior.AllowGet);*/ 
        }

        [AuthorizationFilter]
        public ActionResult Batch()
        {
            return View();
        }

        [HttpPost]
        public JsonResult SaveBatch(Batch batch)
        {
            ResellerManagementSystem.Helper.AppHelper.ReturnMessage retMessage = new AppHelper.ReturnMessage();
            ResellerDbEntities2 db = new ResellerDbEntities2();
            retMessage.IsSuccess = true;

            if (batch.BatchId > 0)
            {
                db.Entry(batch).State = EntityState.Modified;
                retMessage.Messagae = "Update Success!";
            }
            else
            {
                batch.BatchName = batch.BatchName + db.Batches.Count();
                var batchData = db.Batches.Where(x => x.BatchName.Equals(batch.BatchName)).SingleOrDefault();
                if (batchData == null)
                {
                    db.Batches.Add(batch);
                    retMessage.Messagae = "Save Success!";
                }
                else
                {
                    retMessage.IsSuccess = false;
                    retMessage.Messagae = "This batch already exist!Please refresh and again try!";
                }
            }
            try
            {
                db.SaveChanges();
            }
            catch (Exception)
            {
                retMessage.IsSuccess = false;
            }

            return Json(retMessage, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetAllBatch()
        {

            ResellerDbEntities2 db = new ResellerDbEntities2();
            var dataList = db.Batches.Select(batch => new
            {
                BatchId = batch.BatchId,
                BatchName = batch.BatchName
            }).ToList();
            return Json(dataList, JsonRequestBehavior.AllowGet);


            /*
            ResellerDbEntities db = new ResellerDbEntities();
            var dataList = db.Batches.ToList();
            var modefiedData = dataList.Select(x => new
            {
                BatchId = x.BatchId,
                BatchName = x.BatchName,
            }).ToList();
            return Json(modefiedData, JsonRequestBehavior.AllowGet);*/
        }

        [AuthorizationFilter]
        public ActionResult ProductStock()
        {
            return View();
        }
        [HttpPost]
        public JsonResult SaveProductStock(ProductStock stock)
        {
            ResellerManagementSystem.Helper.AppHelper.ReturnMessage retMessage = new AppHelper.ReturnMessage();
            ResellerDbEntities2 db = new ResellerDbEntities2();
            retMessage.IsSuccess = true;

            if (stock.ProductQtyId > 0)
            {
                db.Entry(stock).State = EntityState.Modified;
                retMessage.Messagae = "Update Success!";
            }
            else
            {
                db.ProductStocks.Add(stock);
                retMessage.Messagae = "Save Success!";
            }
            try
            {
                db.SaveChanges();
            }
            catch (Exception)
            {
                retMessage.IsSuccess = false;
            }

            return Json(retMessage, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult GetAllProductStocks()
        {
            ResellerDbEntities2 db = new ResellerDbEntities2();
            var dataList = db.ProductStocks.Include("Product").Include("Batch").ToList();
            var modefiedData = dataList.Select(x => new
            {
                ProductQtyId = x.ProductQtyId,
                ProductId = x.ProductId,
                ProductName = x.Product.Name,
                Quantity = x.Quantity,
                BatchId = x.BatchId,
                BatchName = x.Batch.BatchName,
                PurchasePrice = x.PurchasePrice,
                SalesPrice = x.SalesPrice
            }).ToList();
            return Json(modefiedData, JsonRequestBehavior.AllowGet);
        }
        [AuthorizationFilter]
        public ActionResult Invoice()
        {
            ViewBag.InvoiceNum = Guid.NewGuid();
            return View();
        }
        [HttpPost]
        public JsonResult SaveInvoiceSale(Sale sale, List<SalesDetail> salesDetails)
        {
            ResellerManagementSystem.Helper.AppHelper.ReturnMessage retMessage = new AppHelper.ReturnMessage();
            ResellerDbEntities2 db = new ResellerDbEntities2();
            retMessage.IsSuccess = true;

            foreach (var item in salesDetails)
            {
                sale.SalesDetails.Add(new SalesDetail { ProductId = item.ProductId, UnitPrice = item.UnitPrice, Quantity = item.Quantity, LineTotal = item.LineTotal });
                var prd = db.ProductStocks.Where(x => x.ProductId == item.ProductId && x.Quantity > 0).FirstOrDefault();
                prd.Quantity = prd.Quantity - item.Quantity;
                db.Entry(prd).State = EntityState.Modified;
            }
            db.Sales.Add(sale);
            retMessage.Messagae = "Save Success!";
            try
            {
                db.SaveChanges();
            }
            catch (Exception)
            {
                retMessage.IsSuccess = false;
            }

            return Json(retMessage, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult EditInvoiceSale(Sale sale, List<SalesDetail> salesDetails, List<int> deleted)
        {
            ResellerManagementSystem.Helper.AppHelper.ReturnMessage retMessage = new AppHelper.ReturnMessage();
            ResellerDbEntities2 db = new ResellerDbEntities2();
            retMessage.IsSuccess = true;

            if (deleted != null)
            {
                foreach (var item in deleted)
                {
                    var data = db.SalesDetails.Where(x => x.SalesDetailId == item).SingleOrDefault(); ;
                    db.SalesDetails.Remove(data);
                }
            }

            foreach (var item in salesDetails)
            {
                if (item.SalesDetailId > 0)
                {
                    db.Entry(item).State = EntityState.Modified;
                    retMessage.Messagae = "Update Success!";
                }
                else
                {
                    sale.SalesDetails.Add(new SalesDetail { ProductId = item.ProductId, UnitPrice = item.UnitPrice, Quantity = item.Quantity, LineTotal = item.LineTotal });
                    var prd = db.ProductStocks.Where(x => x.ProductId == item.ProductId && x.Quantity > 0).FirstOrDefault();
                    prd.Quantity = prd.Quantity - item.Quantity;
                    db.Entry(prd).State = EntityState.Modified;
                    db.Sales.Add(sale);
                    retMessage.Messagae = "Save Success!";
                }
            }


            try
            {
                db.SaveChanges();
            }
            catch (Exception)
            {
                retMessage.IsSuccess = false;
            }

            return Json(retMessage, JsonRequestBehavior.AllowGet);
        }

        [AuthorizationFilter]
        public ActionResult InvoiceList()
        {
            return View();
        }
                
        [HttpGet]
        public JsonResult GetAllSales()
        {
            ResellerDbEntities2 db = new ResellerDbEntities2();
            var dataList = db.Sales.ToList();
            var modefiedData = dataList.Select(x => new
            {
                SalesId = x.SalesId,
                OrderNo = x.OrderNo,
                CustomerName = x.CustomerName,
                CustomerPhone = x.CustomerPhone,
                CustomerAddress = x.CustomerAddress,
                OrderDate = x.OrderDate,
                TotalAmount = x.TotalAmount
            }).ToList();
            return Json(modefiedData, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult GetInvoiceBySalesId(int salesId)
        {
            ResellerDbEntities2 db = new ResellerDbEntities2();
            List<Sale> dataList = (from sd in db.SalesDetails.ToList()
                                   join s in db.Sales on sd.SalesId equals s.SalesId
                                   where sd.SalesId == salesId
                                   select new Sale
                                   {
                                       SalesId = (int)sd.SalesId,
                                       OrderNo = s.OrderNo,
                                       CustomerName = s.CustomerName,
                                       CustomerPhone = s.CustomerPhone,
                                       CustomerAddress = s.CustomerAddress,
                                       OrderDate = s.OrderDate,
                                       PaymentMethod = s.PaymentMethod,
                                       TotalAmount = s.TotalAmount,
                                       SalesDetailId = sd.SalesDetailId,
                                       ProductId = sd.ProductId,
                                       UnitPrice = sd.UnitPrice,
                                       Subtotal = s.Subtotal,
                                       Quantity = sd.Quantity,
                                       LineTotal = sd.LineTotal,
                                       DiscountPercentage = s.DiscountPercentage,
                                       VatPercentage = s.VatPercentage
                                   }).ToList();
            return Json(dataList, JsonRequestBehavior.AllowGet);
        }
    }
}