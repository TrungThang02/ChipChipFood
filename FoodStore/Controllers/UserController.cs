using FoodStore.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace FoodStore.Controllers
{
    public class UserController : Controller
    {
        private FoodStoreEntities db = new FoodStoreEntities();

        public static string Encrypt(string password)
        {
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            byte[] encrypt;
            UTF8Encoding encode = new UTF8Encoding();
            encrypt = md5.ComputeHash(Encoding.UTF8.GetBytes(password));
            StringBuilder encryptdata = new StringBuilder();
            for (int i = 0; i < encrypt.Length; i++)
            {
                encryptdata.Append(encrypt[i].ToString());

            }
            return encryptdata.ToString();

        }




        // GET: User/Details/5


        // POST: User/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.

        public ActionResult DangKy()
        {
            return View();
        }

        [HttpPost]
        public ActionResult DangKy(FormCollection collection, Customer kh)
        {
            var sHoTen = collection["HoTen"];
            var sTenDN = collection["TenDN"];
            var sMatKhau = collection["MatKhau"];
            var sNhapLaiMatKhau = collection["MatKhauNL"];
            var sDiaChi = collection["DiaChi"];
            var sEmail = collection["Email"];
            var sDienThoai = collection["DienThoai"];
            var dNgaySinh = String.Format("{0:MM/dd/yyyy}", collection["NgaySinh"]);

            if (String.IsNullOrEmpty(sHoTen))
            {
                ViewData["err1"] = "Họ tên không được để trống";
            }
            else if (String.IsNullOrEmpty(sTenDN))
            {
                ViewData["err2"] = "Tên đăng nhập không được để trống";
            }
            else if (String.IsNullOrEmpty(sMatKhau))
            {
                ViewData["err3"] = "Mật khẩu không được để trống";
            }
            else if (String.IsNullOrEmpty(sNhapLaiMatKhau))
            {
                ViewData["err4"] = "Phải nhập lại mật khẩu";
            }
            else if (String.IsNullOrEmpty(sDiaChi))
            {
                ViewData["err5"] = "Địa chỉ không được để trống";
            }
            else if (String.IsNullOrEmpty(sEmail))
            {
                ViewData["err6"] = "Email không được để trống";
            }
            else if (String.IsNullOrEmpty(sDienThoai))
            {
                ViewData["err7"] = "Điện thoại không được để trống";
            }
            else if (db.Customers.SingleOrDefault(n => n.UserName == sTenDN) != null)
            {
                ViewData["err8"] = "Tên đăng nhâp đã tồn tại";
            }
            else if (db.Customers.SingleOrDefault(n => n.Email == sEmail) != null)
            {
                ViewData["err9"] = "Email đã được sử dụng";
            }
            else if (String.IsNullOrEmpty(dNgaySinh))
            {
                ViewData["err10"] = "Chọn ngày sinh hợp lệ";
            }
            else
            {
                try
                {
                    kh.CustomerName = sHoTen;
                    kh.UserName = sTenDN;
                    kh.Password = Encrypt(sMatKhau);
                    kh.Email = sEmail;
                    kh.Address = sDiaChi;
                    kh.BirthDay = DateTime.Parse(dNgaySinh);
                    kh.Phone = sDienThoai;
                    db.Customers.Add(kh);
                    ViewData["success"] = "Đăng ký thành công";
                    db.SaveChanges();
                }
                catch (DbEntityValidationException e)
                {
                    Console.WriteLine(e);
                }
            }
            return View();
        }

        [HttpGet]
        public ActionResult DangNhap()
        {

            return View();
        }

        [HttpPost]
        public ActionResult DangNhap(FormCollection collection)
        {
            //var response = Request["g-recaptcha-response"];
            //string secretKey = "6LexpB0jAAAAAPFmXhwAmu2GGeV1DWOV5yH1Q1BK";
            //var client = new WebClient();

            //var result = client.DownloadString(string.Format("https://www.google.com/recaptcha/api/siteverify?secret={0}&response={1}", secretKey, response));

            //var obj = JObject.Parse(result);
            //var status = (bool)obj.SelectToken("success");
            //ViewBag.Message = status ? "Xác thực Google reCaptcha thành công !" : "Bạn chưa xác thực Google reCaptcha";


            if (ModelState.IsValid)
            {
                int state = Convert.ToInt32(Request.QueryString["id"]);
                var sTenDN = collection["TenDN"];
                var sMatKhau = Encrypt(collection["MatKhau"].ToString());
                if (String.IsNullOrEmpty(sTenDN))
                {
                    ViewData["err"] = "Tài khoản tên không được để trống";
                }
                else if (String.IsNullOrEmpty(sMatKhau))
                {
                    ViewData["err"] = "Mật khẩu không được để trống";
                }
                else
                {
                    Customer kh = db.Customers.SingleOrDefault(n => n.UserName == sTenDN && n.Password == sMatKhau);
                    if (kh != null)
                    {
                        ViewBag.ThongBao = "Chúc mừng bạn đăng nhập thành công";
                        Session["TaiKhoan"] = kh.CustomerName;

                        Session["TaiKhoan2"] = kh.CustomerId;
                        Session["cmt"] = kh;

                        if (state == 1)
                        {
                            return RedirectToAction("Index", "Products");
                        }
                        else if (state >= 2 && state < db.Products.Max(p => p.ProductId))
                        {
                            return RedirectToAction("ChiTIetSanPham/" + state, "Products");
                        }
                        else
                        {
                            return RedirectToAction("DatHang", "GioHang");
                        }
                    }
                    else
                    {
                        ViewBag.ThongBao = "Tên đăng nhập hoặc mật khẩu không đúng";
                        //ViewBag.Message = status ? "Xác thực Google reCaptcha thành công !" : "Bạn chưa xác thực Google reCaptcha";

                    }
                }
            }
            
                  
            return View();
        }
        public ActionResult LoginLogOutPartial()
        {

            return PartialView();
        }

        public ActionResult DangXuat()
        {
            Session.Remove("TaiKhoan");
            Session.Remove("TaiKhoan2");
            return RedirectToAction("Index", "Products");
        }


        [NonAction]
        public void SendVerificationLinkEmail(string email, string activationCode, string emailFor = "VerifyAccount")
        {
            var verifyUrl = "/User/" + emailFor + "/" + activationCode;
            var link = Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, verifyUrl);

            var fromEmail = new MailAddress("chuhaist123@gmail.com", "Chip Chip Food");
            var toEmail = new MailAddress(email);

            string subject = "";
            string body = "";

            if (emailFor == "VerifyAccount")
            {
                subject = "Your account is successfully created";
                body = "<br /><br /> We are  <a href='" + link + "'>" + link + "</a>";
            }
            else if (emailFor == "ResetPassword")
            {
                subject = "Reset Password";
                body = "Chúng tôi xác nhận tài khoản này thuộc về bạn, vui lòng ấn vào <a href='" + link + "'> Khôi phục mật khẩu </a>";
            }

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = true,
                Credentials = new NetworkCredential(fromEmail.Address, "synmlgiwemgarjyu")
            };
            using (var message = new MailMessage(fromEmail, toEmail)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            })
                smtp.Send(message);



        }
        
        public ActionResult HttpNotFound()
        {
            return View();
        }


       
       
        public ActionResult ThayDoiThongTin(int? id)
        {

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Customer customer = db.Customers.Find(id);
            if (customer == null)
            {
                return HttpNotFound();
            }
            return View(customer);
        }

    

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ThayDoiThongTin([Bind(Include = "CustomerId,CustomerName,Address,BirthDay,Password,UserName,Email,Phone,ResetPasswordCode")] Customer customer)
        {
            if (ModelState.IsValid)
            {
                db.Entry(customer).State = EntityState.Modified;
                
                db.SaveChanges();
                Session.Remove("TaiKhoan");
                Session["TaiKhoan"] = customer.CustomerName;
                return RedirectToAction("Index","Products");
            }
            return View(customer);
        }

        public ActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ForgotPassword(string Email)
        {
            string message = "";
            try
            {
                bool status = false;
                var account = db.Customers.Where(a => a.Email == Email).FirstOrDefault();
                if (account != null)
                {
                    string resetCode = Guid.NewGuid().ToString();
                    SendVerificationLinkEmail(account.Email, resetCode, "ResetPassword");
                    account.ResetPasswordCode = resetCode;
                    db.SaveChanges();
                    message = "Liên kết khôi phục mật khẩu đã được gửi đến email của bạn <3";
                }
                else
                {
                    message = "Không tìm thấy email";
                }
            } catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            ViewBag.Message = message;
            return View();
        }

        public ActionResult ResetPassword(string id)
        {
            var user = db.Customers.Where(a => a.ResetPasswordCode == id).FirstOrDefault();
            if (user != null)
            {
                ResetPassword model = new ResetPassword();
                model.ResetCode = id;
                return View(model);
            }
            else
            {
                return HttpNotFound();
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ResetPassword(ResetPassword model)
        {
            var message = "";
            if (ModelState.IsValid)
            {
                var user = db.Customers.Where(a => a.ResetPasswordCode == model.ResetCode).FirstOrDefault();
                if (user != null)
                {
                    user.Password = Encrypt(model.NewPassword);
                    user.ResetPasswordCode = "";
                    db.Configuration.ValidateOnSaveEnabled = false;
                    db.SaveChanges();
                    message = "Thay đổi mật khẩu thành công";

                }
            }
            else
            {
                message = "Something invalid";
            }
            ViewBag.Message = message;
            return View(model);
        }

    }
}
