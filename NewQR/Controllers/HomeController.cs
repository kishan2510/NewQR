using Microsoft.AspNetCore.Mvc;
using System.Drawing;
using QRCoder;
using NewQR.DBCONN;
using NewQR.Models;
using Microsoft.AspNetCore.Authorization;
using System;
using System.IO;
using System.Threading.Tasks;


namespace NewQR.Controllers
{

    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _db;
         public HomeController(ApplicationDbContext db)
        {
            _db = db;
        }
        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        public IActionResult Login(Auth user)
        {
            var MyUser =_db.Auths.Where(x=> x.Email == user.Email && x.Password==user.Password).FirstOrDefault();
            if(MyUser!= null)
            {

                
                string folderPath = "C:\\Users\\Kishan.Pathak\\source\\repos\\NewQR\\NewQR\\wwwroot\\qrr";
                //string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "qrr");
               
                string[] files = Directory.GetFiles("wwwroot/qrr");
                foreach (string file in files)
                {
                    var fileInfo = new FileInfo(file);
                    if (fileInfo.Exists)
                    {
                        fileInfo.Delete();
                    }
                }

                return RedirectToAction("Index");
            }
            else
            {
                ViewBag.Message = "Login Failed";
            }
            return View();
        }


        [AllowAnonymous]
        [HttpGet]
        public IActionResult Register()
        {
           
            return View();
        }
        [HttpPost]
        public IActionResult Register(Auth obj)
        {
            if (obj != null && obj.Password!=null)
            {
                _db.Auths.Add(obj);
                _db.SaveChanges();
                return View("Login");
            }
            else
            {
                ViewBag.Message = "Registration Failed";
            }
            return View();
        }
        
        public IActionResult Index()
        {
            return View();
        }


        [HttpPost]
        public IActionResult Index(string qrText)
        {
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(qrText, QRCodeGenerator.ECCLevel.Q);
            QRCode qrCode = new QRCode(qrCodeData);
            Bitmap qrCodeImage = qrCode.GetGraphic(20);

            return View(BitmapToBytes(qrCodeImage));
        }

        private static Byte[] BitmapToBytes(Bitmap img)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                img.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                return stream.ToArray();
            }
        }

    
        public IActionResult GenerateFile()
        {
            return View();
        }
       

        [HttpPost]
        public IActionResult GenerateFile(string qrText,string filename)
        {
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(qrText, QRCodeGenerator.ECCLevel.Q);
            //string fileGuid = Guid.NewGuid().ToString().Substring(0, 4);
            qrCodeData.SaveRawData("wwwroot/qrr/" + filename + ".qrr", QRCodeData.Compression.Uncompressed);

            QRCodeData qrCodeData1 = new QRCodeData("wwwroot/qrr/" + filename + ".qrr", QRCodeData.Compression.Uncompressed);
            QRCode qrCode = new QRCode(qrCodeData1);
            Bitmap qrCodeImage = qrCode.GetGraphic(20);
            return View(BitmapToBytes(qrCodeImage));
        }

       
        public IActionResult WifiPass()
        {
            return View();
        }
        [HttpPost]
        public IActionResult WifiPass(string ssid, string encryption, string password, string filename)
        {
            // Format the WiFi credentials
            string wifiFormat = $"WIFI:T:{encryption};S:{ssid};P:{password};;";

            // Generate QR code
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(wifiFormat, QRCodeGenerator.ECCLevel.Q);
            qrCodeData.SaveRawData("wwwroot/qrr/" + filename + ".qrr", QRCodeData.Compression.Uncompressed);

            QRCodeData qrCodeData1 = new QRCodeData("wwwroot/qrr/" + filename + ".qrr", QRCodeData.Compression.Uncompressed);
            QRCode qrCode = new QRCode(qrCodeData1);
            Bitmap qrCodeImage = qrCode.GetGraphic(20);

            return View(BitmapToBytes(qrCodeImage));
        }

     

        public IActionResult ViewFile()
        {
            List<KeyValuePair<string, Byte[]>> fileData = new List<KeyValuePair<string, byte[]>>();
            KeyValuePair<string, Byte[]> data;

            string[] files = Directory.GetFiles("wwwroot/qrr");
            foreach (   string  file in files)
            {
                QRCodeData qrCodeData = new QRCodeData(file, QRCodeData.Compression.Uncompressed);
                QRCode qrCode = new QRCode(qrCodeData);
                Bitmap qrCodeImage = qrCode.GetGraphic(20);

                Byte[] byteData = BitmapToBytes(qrCodeImage);
                data = new KeyValuePair<string, Byte[]>(Path.GetFileName(file), byteData);
                fileData.Add(data);
            }

            return View(fileData);
        }


    }
}
