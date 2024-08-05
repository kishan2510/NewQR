using Microsoft.AspNetCore.Mvc;
using System.Drawing;
using QRCoder;
using NewQR.DBCONN;
using NewQR.Models;
using Microsoft.AspNetCore.Authorization;
using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Humanizer.Bytes;


namespace NewQR.Controllers
{

    public class HomeController : Controller
    {
        //Created a instance of db context 
        private readonly ApplicationDbContext _db;

        //constructor of home controller to initalize the instance of dbcontext
         public HomeController(ApplicationDbContext db)
        {
            _db = db;
        }
        [AllowAnonymous] // This will make the login view to access by any user 
        //This is action method of login view by default it is a get method it  will return login view 
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        //This is post function of login view which will take input submitted by the user and checks 
        // weather the user credential is valid or not 
        //if the user is valid then reirect it to homepage i.e; Index view
        public async Task<IActionResult> Login(Auth user)
        {
            var MyUser = await _db.Auths
         .Where(x => x.Email == user.Email && x.Password == user.Password)
         .FirstOrDefaultAsync(); 
            if(MyUser!= null)
            {
                //This code gives authorization to the logined user to have access of authorize views
                var claims = new List<Claim>
                 {
                     new Claim(ClaimTypes.Name, MyUser.Email) // Use a unique claim for identification
                 };
                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));
                return RedirectToAction("Index");

                //Int this code we find the path of all the generated qrr and after login delete all the files make qrr empty 
                string folderPath = "C:\\Users\\Kishan.Pathak\\source\\repos\\NewQR\\NewQR\\wwwroot\\qrr";
                //string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "qrr");
                string[] files = Directory.GetFiles("wwwroot/qrr");

                //Deleting previously stored qrr
                foreach (string file in files)
                {
                    var fileInfo = new FileInfo(file);
                    if (fileInfo.Exists)
                    {
                        fileInfo.Delete();
                    }
                }
                //Return to Index page
                return RedirectToAction("Index");
            }
            else
            {
                //This is dynamic object which is used to transfer small information to the 
                //view from controller                                      
                ViewBag.Message = "Login Failed";
            }
            return View();
        }


        [AllowAnonymous] //Its visible for any user 
        [HttpGet]

        //Action method of register page which display register view on clicking register button
        public IActionResult Register()
        {
           
            return View();
        }
        [HttpPost]
        //This is the http post method of register view which get the register page data and store it in the database 
        //using object of db context 
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

        [Authorize] // Authorization is applied so that only valid user can have access of this Action method 
        public IActionResult Index()
        {
            return View();
        }

        
        [HttpPost]
        [Authorize]
        //This is the post method of index view which takes the data submitted by the user and 
        //convert the data into the qr code by using methods of qr coder package
        public IActionResult Index(string qrText)
        {
            if (string.IsNullOrEmpty(qrText))
            {
                return BadRequest("No data entered.");
            }
            else
            {
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(qrText, QRCodeGenerator.ECCLevel.Q);
            QRCode qrCode = new QRCode(qrCodeData);
                //Here qr code data is converted to bitmap
            Bitmap qrCodeImage = qrCode.GetGraphic(20);
                //Here we convert the bit mapped data to byte data using bitmaptobytes function to display the qr code image 
            byte[] imgData = BitmapToBytes(qrCodeImage);
                ViewBag.QRCodeText = qrText;
                return View(imgData);
            }
        }
        //this is bitmap to byte function which convert the bitmap data to bytes so that it can be displayed 
        private static Byte[] BitmapToBytes(Bitmap img)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                img.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                return stream.ToArray();
            }
        }

        [Authorize]
        //This is the action methos of generate file view which displays the view of generate file 
       
        public IActionResult GenerateFile()
        {
            return View();
        }
       

        [HttpPost]
        [Authorize]

        // this is the post method of generate file which takes the text in string and also name of the file 
        // Then after generating the qr code it will save the file in qrr folder which we can in from view files
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

        [Authorize]
        //This is the action method which will return the view where we can create qr code for our wifi connectivity
        public IActionResult WifiPass()
        {
            return View();
        }
        [HttpPost]
        [Authorize]
        // this is the post method of wifipass where we are accepting the data and converting it to the qr code so that 
        // user candirectly connect to the network without entering password
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


        [Authorize]

        //This will show the all the files in qrr folder at one place 
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

        //This will logout the user by checking the cookies of browser
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

        [Authorize]

        //This will download the file in png format
        public IActionResult DownloadFile(string qrText)
        {
            if (string.IsNullOrEmpty(qrText))
            {
                return BadRequest("Filename or data not provided.");
            }

            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(qrText, QRCodeGenerator.ECCLevel.Q);
            QRCode qrCode = new QRCode(qrCodeData);

            // Convert QR code data to Bitmap
            Bitmap qrCodeImage = qrCode.GetGraphic(20);
            byte[] imgData = BitmapToBytes(qrCodeImage);

            string contentType = "image/png";

            return File(imgData, contentType, "QrCode.png");
        }
    }
}
