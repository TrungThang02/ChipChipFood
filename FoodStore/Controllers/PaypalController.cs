using FoodStore.helper;
using FoodStore.Models;
using PayPal.Api;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FoodStore.Controllers
{
    public class PaypalController : Controller
    {
        // GET: PayPal
        private PayPal.Api.Payment payment;



        GioHangController g = new GioHangController();



        public ActionResult Index()
        {
            return View();
        }



        public ActionResult PaymentWithPaypal()
        {
            APIContext apiContext = helper.Configuration.GetAPIContext();
            try
            {
                string payerId = Request.Params["PayerID"];
                if (string.IsNullOrEmpty(payerId))
                {
                    string baseURI = Request.Url.Scheme + "://" + Request.Url.Authority
                    +
                    "/Paypal/PaymentWithPayPal?";



                    //link trả về khi người dùng hủy thanh toán
                    string failedURI = Request.Url.Scheme + "://" + Request.Url.Authority + "/Paypal/FailureView";


                    var guid = Convert.ToString((new Random()).Next(100000));
                    var createdPayment = this.CreatePayment(apiContext, baseURI + "guid=" + guid, failedURI);
                    var links = createdPayment.links.GetEnumerator();
                    string paypalRedirectUrl = null;
                    while (links.MoveNext())
                    {
                        Links lnk = links.Current;
                        if (lnk.rel.ToLower().Trim().Equals("approval_url"))
                        {
                            paypalRedirectUrl = lnk.href;
                        }
                    }
                    Session.Add(guid, createdPayment.id);
                    return Redirect(paypalRedirectUrl);
                }
                else
                {
                    var guid = Request.Params["guid"];
                    var executedPayment = ExecutePayment(apiContext, payerId, Session[guid] as string);
                    if (executedPayment.state.ToLower() != "approved")
                    {
                        return View("FailureView");
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log("Error" + ex.Message);
                return View("FailureView");
            }
            Session["GioHang"] = null;
            return View("SuccessView");


        }


        private Payment CreatePayment(APIContext apiContext, string redirectUrl, string failedUrl)
        {

            var itemList = new ItemList() { items = new List<Item>(), shipping_address = new ShippingAddress() { recipient_name = "Tran Trung Thang", country_code = "VN", city = "Phu Yen", line1 = "An Ninh Dong, Tuy An", postal_code = "700000" } };
            //recipient_name: tên người đặt hàng
            //country_code: code quốc gia, tham khảo thêm tại: https://developer.paypal.com/docs/api/reference/country-codes/
            //city: thành phố shipping
            //line1: địa chỉ giao hàng
            //postal_code: code postal (ví dụ code ở Việt Nam: https://www.google.com/search?q=postal+code+vietnam)
            var payer = new Payer() { payment_method = "paypal" };
            var redirUrls = new RedirectUrls()
            {
                cancel_url = failedUrl, //url cancel
                return_url = redirectUrl //url return
            };

            List<GioHang> listGioHang = Session["GioHang"] as List<GioHang>;

            double tien = Convert.ToDouble(Math.Round((g.getTongTien() / 23000), 0));
            double tongtien = tien + 3;

            foreach (var cart in listGioHang)
            {
                itemList.items.Add(new Item()
                {
                    //Thông tin đơn hàng
                    name = cart.productName,
                    currency = "USD",
                    price = Math.Round(tien, 0).ToString(),
                    quantity = "1",
                    sku = "sku"
                });
            }

            var details = new Details()
            {
                tax = "1",
                shipping = "2",
                subtotal = Math.Round(tien, 0).ToString()
            };

            var amount = new Amount()
            {
                currency = "USD",
                total = Math.Round(tongtien, 0).ToString(),
                details = details
            };
            var transactionList = new List<Transaction>();

            transactionList.Add(new Transaction()
            {
                description = "Transaction description.", //nội dung thanh toán
                invoice_number = DateTime.Now.ToString(), //mã hóa đơn
                amount = amount,
                item_list = itemList
            });
            this.payment = new Payment()
            {
                intent = "sale",
                payer = payer,
                transactions = transactionList,
                redirect_urls = redirUrls
            };
            return this.payment.Create(apiContext);
        }

        private Payment ExecutePayment(APIContext apiContext, string payerId, string paymentId)
        {
            var paymentExecution = new PaymentExecution() { payer_id = payerId };
            this.payment = new Payment() { id = paymentId };
            return this.payment.Execute(apiContext, paymentExecution);
        }

        public ActionResult SuccessView()
        {
            return View();
        }



        public ActionResult FailureView()
        {
            return View();
        }
    }
}