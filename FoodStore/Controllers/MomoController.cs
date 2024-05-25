using FoodStore.Controllers;
using FoodStore.Models;
using FoodStore.Momo;
using MoMo;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FoodStore.Controllers
{
    public class MomoController : Controller
    {
        GioHangController g = new GioHangController();
        FoodStoreEntities db = new FoodStoreEntities();
        // GET: MomoGio
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Payment()
        {
            //request params need to request to MoMo system
            string endpoint = "https://test-payment.momo.vn/gw_payment/transactionProcessor";
            string partnerCode = "MOMOOJOI20210710";
            string accessKey = "iPXneGmrJH0G8FOP";
            string serectkey = "sFcbSGRSJjwGxwhhcEktCHWYUuTuPNDB";
            string orderInfo = "Thanh toán";
            string returnUrl = "https://localhost:44317/Momo/ConfirmPaymentClient";
            string notifyurl = "https://4c8d-2001-ee0-5045-50-58c1-b2ec-3123-740d.ap.ngrok.io/Home/SavePayment"; //lưu ý: notifyurl không được sử dụng localhost, có thể sử dụng ngrok để public localhost trong quá trình test

            string amount =  g.getTongTien().ToString();
            string orderid = DateTime.Now.Ticks.ToString(); //mã đơn hàng
            string requestId = DateTime.Now.Ticks.ToString();
            string extraData = "";

            //Before sign HMAC SHA256 signature
            string rawHash = "partnerCode=" +
                partnerCode + "&accessKey=" +
                accessKey + "&requestId=" +
                requestId + "&amount=" +
                amount + "&orderId=" +
                orderid + "&orderInfo=" +
                orderInfo + "&returnUrl=" +
                returnUrl + "&notifyUrl=" +
                notifyurl + "&extraData=" +
                extraData;

            MoMoSecurity crypto = new MoMoSecurity();
            //sign signature SHA256
            string signature = crypto.signSHA256(rawHash, serectkey);

            //build body json request
            JObject message = new JObject
            {
                { "partnerCode", partnerCode },
                { "accessKey", accessKey },
                { "requestId", requestId },
                { "amount", amount },
                { "orderId", orderid },
                { "orderInfo", orderInfo },
                { "returnUrl", returnUrl },
                { "notifyUrl", notifyurl },
                { "extraData", extraData },
                { "requestType", "captureMoMoWallet" },
                { "signature", signature }

            };

            string responseFromMomo = PaymentRequest.sendPaymentRequest(endpoint, message.ToString());

            JObject jmessage = JObject.Parse(responseFromMomo);

            return Redirect(jmessage.GetValue("payUrl").ToString());
        }

        //Khi thanh toán xong ở cổng thanh toán Momo, Momo sẽ trả về một số thông tin, trong đó có errorCode để check thông tin thanh toán
        //errorCode = 0 : thanh toán thành công (Request.QueryString["errorCode"])
        //Tham khảo bảng mã lỗi tại: https://developers.momo.vn/#/docs/aio/?id=b%e1%ba%a3ng-m%c3%a3-l%e1%bb%97i
        public ActionResult ConfirmPaymentClient(Result result, FormCollection f)
        {
            //lấy kết quả Momo trả về và hiển thị thông báo cho người dùng (có thể lấy dữ liệu ở đây cập nhật xuống db)
            string rMessage = result.message;
            string rOrderId = result.orderId;
            string rErrorCode = result.errorCode; // = 0: thanh toán thành công
           

            ViewBag.rMessage = result.message;
            ViewBag.rOrderId = result.orderId;
            ViewBag.rErrorCode = result.errorCode;
            Product p = new Product();

            Order ddh = new Order();
            Customer kh = (Customer)Session["cmt"];
            List<GioHang> lstGioHang = (List<GioHang>)Session["GioHang"];


            if (kh.CustomerId != null)
            {


                ddh.CustomerId = kh.CustomerId;

                ddh.OrderDate = DateTime.Now;
                ddh.Address = kh.Address;
                ddh.RecipientPhone = kh.Phone;

                //var NgayGiao = DateTime.Now.ToString() + 1;
                //ddh.DeliveryDate = DateTime.Parse(NgayGiao);


                var giatien = (decimal)g.getTongTien();
                ddh.OrderPrice = giatien;
                ddh.DeliveryDate = DateTime.Now.AddDays(3);





                db.Orders.Add(ddh);
                db.SaveChanges();
            }
            foreach (var item in lstGioHang)
            {
                OrderDetail ctdh = new OrderDetail();
                ctdh.OrderId = ddh.OrderId;
                ctdh.ProductId = item.productId;
                ctdh.Quantity = item.productQuantity;
                ctdh.Price = (decimal)item.productPrice * item.productQuantity;
                db.OrderDetails.Add(ctdh);
                db.SaveChanges();

            }
            Session["GioHang"] = null;
            return RedirectToAction("XacNhanDonHang", "GioHang");
        }

        [HttpPost]
        public void SavePayment(FormCollection f)
        {
            Order ddh = new Order();
            OrderDetail ct = new OrderDetail();
            Customer kh = (Customer)Session["cmt"];

            if (kh.CustomerId != null)
            {
              

                    ddh.CustomerId = kh.CustomerId;

                    ddh.OrderDate = DateTime.Now;
                    ddh.Address = kh.Address;
                    ddh.RecipientPhone = kh.Phone;

                    var NgayGiao = String.Format("{0:MM/mm/yyyy}", f["NgayGiao"]);
                    ddh.DeliveryDate = DateTime.Parse(NgayGiao);


                    var giatien = ct.Price;
                    ddh.OrderPrice = giatien;





                    db.Orders.Add(ddh);
                    db.SaveChanges();
                }



        }
    }
}
