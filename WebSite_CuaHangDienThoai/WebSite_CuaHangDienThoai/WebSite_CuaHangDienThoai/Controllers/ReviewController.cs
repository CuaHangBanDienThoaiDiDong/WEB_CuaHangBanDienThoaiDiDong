﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebSite_CuaHangDienThoai.Models;

namespace WebSite_CuaHangDienThoai.Controllers
{
    public class ReviewController : Controller
    {
        // GET: Review
        CUAHANGDIENTHOAIEntities db = new CUAHANGDIENTHOAIEntities();
        public ActionResult Index()
        {
            return View();
        }



        public ActionResult Partail_ReivewForProduct(int id)
        {

            var reviewIds = db.tb_Review
                    .Where(x => x.ProductDetailId == id)
                    .Select(x => x.ReviewId)
                    .ToList();
            if (reviewIds != null)
            {
                var reviewDetails = db.tb_ReviewDetail
    .Where(detail => reviewIds.Contains((int)detail.ReviewId))
    .OrderByDescending(detail => detail.ReviewDetailId)
    .ToList();


                if (reviewDetails != null)
                {
                    ViewBag.ProductDetailId = id;
                    ViewBag.Count = reviewDetails.Count;
                    return PartialView(reviewDetails);
                }
            }
          
            return PartialView();
        }
        public ActionResult Partail_ReviewDetailById(int id) 
        {
            var item = db.tb_ReviewDetail.Where(x => x.ReviewId == id).ToList();
            if (item != null) 
            {
                ViewBag.Count = item.Count;
                return PartialView(item);

            }
            return PartialView();
        }


        public ActionResult Partial_ThongTinKhach()
        {
            if (Session["CustomerId"] != null)
            {
                int idKhach = (int)Session["CustomerId"];
                var khachHang = db.tb_Customer.Find(idKhach);
                return PartialView(khachHang);
            }
            return PartialView();
        }


        public ActionResult Partial_AddReview(int id ) 
        {
            ViewBag.id = id;
            return PartialView();
        }    
        public ActionResult Add(int id ,string productName)
        {
            //var item = db.tb_ProductDetail.Find(id);
            //if (item != null) 
            //{
            //    ViewBag.id = id;
            //    return PartialView(item);
            //}
            ViewBag.id = id;
            ViewBag.productName = productName;  
            return PartialView();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(int productDetailId, Client_TokenReview req , tb_Review review , tb_ReviewDetail revDetail)
        {
            var code = new { Success = false, Code = -1, Url = "" }; 
            if (Session["CustomerId"] != null)
            {
                int idKhach = (int)Session["CustomerId"];
                var inforKhachHang = db.tb_Customer.FirstOrDefault(x => x.CustomerId == idKhach);
                if (req.Content != null && productDetailId > 0) 
                {
                    var checkReview = db.tb_Review.FirstOrDefault(x => x.ProductDetailId == productDetailId);
                    if (checkReview == null) 
                    {
                        review.CreatedDate = DateTime.Now;
                        review.ProductDetailId = productDetailId;
                        db.tb_Review.Add(review);
                        db.SaveChanges();


                        revDetail.CreatedDate=DateTime.Now;
                        revDetail.CreatedBy = inforKhachHang.CustomerName;

                        revDetail.Content=req.Content.Trim();

                        revDetail.CustomerId = idKhach;
                        revDetail.ReviewId = review.ReviewId;       
                        db.tb_ReviewDetail.Add(revDetail);
                        db.SaveChanges();

                        code = new { Success = true, Code =1, Url = "" };

                    }
                    else
                    {
                        revDetail.CreatedDate = DateTime.Now;
                        revDetail.CreatedBy = inforKhachHang.CustomerName;

                        revDetail.Content = req.Content.Trim();

                        revDetail.CustomerId = idKhach;
                        revDetail.ReviewId = checkReview.ReviewId;
                        db.tb_ReviewDetail.Add(revDetail);
                        db.SaveChanges();

                        code = new { Success = true, Code = 1, Url = "" };
                    }
                }
                else
                {
                    code = new { Success = false, Code = -3, Url = "" };//Ko bo trong
                }
            }
            else 
            {
                code = new { Success = false, Code = -2, Url = "" };//Ko tim thay tai khoan n
            }
            return Json(code);
        }


        [HttpPost]
        public JsonResult EditContent(int reviewDetailId, string content)
        {
            var code = new { Success = false, Code = -1, Url = "" };

            if (Session["CustomerId"] != null)
            {
                int idKhach = (int)Session["CustomerId"];
                if (idKhach > 0)
                {
                    var reviewDetail = db.tb_ReviewDetail.Find(reviewDetailId);
                    if (reviewDetail != null)
                    {
                        
                        if (reviewDetail.CustomerId == idKhach)
                        {
                            var khachHang = db.tb_Customer.Find(idKhach);
                            reviewDetail.Content = content;
                            reviewDetail.ModifiedDate = DateTime.Now;
                            reviewDetail.Modifeby = khachHang.CustomerName;

                            try
                            {
                                db.Entry(reviewDetail).State = EntityState.Modified;
                                db.SaveChanges();

                                code = new { Success = true, Code = 1, Url = "" };
                            }
                            catch (Exception ex)
                            {
                                db.Entry(reviewDetail).State = EntityState.Unchanged;

                                code = new { Success = false, Code = -6, Url = "" };
                            }
                        }
                        else
                        {
                            code = new { Success = false, Code = -5, Url = "" }; 
                        }
                    }
                    else
                    {
                        code = new { Success = false, Code = -3, Url = "" }; // Không tìm thấy đánh giá
                    }
                }
                else
                {
                    code = new { Success = false, Code = -2, Url = "" }; // Không tìm thấy tài khoản người dùng
                }
            }
            else
            {
                code = new { Success = false, Code = -4, Url = "" }; // Không có phiên làm việc
            }

            return Json(code);
        }


        //[HttpPost]
        //public JsonResult EditContent(int reviewDetailId, string content)
        //{
        //    var code = new { Success = false, Code = -1, Url = "" };

        //    if (Session["CustomerId"] != null)
        //    {
        //        int idKhach = (int)Session["CustomerId"];
        //        if (idKhach > 0)
        //        {
        //            var reviewDetail = db.tb_ReviewDetail.Find(reviewDetailId);
        //            if (reviewDetail != null)
        //            {
        //                var khachHang = db.tb_Customer.Find(idKhach);
        //                reviewDetail.Content = content;
        //                reviewDetail.ModifiedDate = DateTime.Now;
        //                reviewDetail.Modifeby = khachHang.CustomerName;

        //                db.Entry(reviewDetail).State = EntityState.Modified;
        //                db.SaveChanges();

        //                code = new { Success = true, Code = 1, Url = "" };
        //            }
        //            else
        //            {
        //                code = new { Success = false, Code = -3, Url = "" }; // Không tìm thấy đánh giá
        //            }
        //        }
        //        else
        //        {
        //            code = new { Success = false, Code = -2, Url = "" }; // Không tìm thấy tài khoản người dùng
        //        }
        //    }
        //    else
        //    {
        //        code = new { Success = false, Code = -4, Url = "" }; // Không có phiên làm việc
        //    }

        //    return Json(code);
        //}

    }
}