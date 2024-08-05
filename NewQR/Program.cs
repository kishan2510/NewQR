using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using NewQR.DBCONN;

var builder = WebApplication.CreateBuilder(args); //Creating webapplicaion and storing its instance in the builder variable


//Adding authentication middleware so that user cant access view for which they are not authorize
//This is implemented using cookies of browser
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
})
.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
{
    options.LoginPath = "/Home/Login";
    options.LogoutPath = "/Home/Logout";
});


//Add services to the container.
builder.Services.AddAuthorization();
builder.Services.AddControllersWithViews();//Add services to use controllers and views in our application

//This is the middleware for dbcontext to get the connection string from appsetting.js 
//And stablish the connection to the database 
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));

});

//bulding web aoolication
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

//Adding different middlewares for static files , http redirection , authentication and routing 

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

//Configuring the default route as login page of home controller
//which displays login view when we open our web app
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Login}/{id?}");

app.Run();//Running our web application 
