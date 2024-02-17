using Microsoft.AspNetCore.Authentication.Cookies;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        #region Add services to the container.
        builder.Services.AddControllersWithViews();
        builder.Services.AddSession();//添加session                       
        builder.Services.AddMemoryCache();//使用缓存                            
        builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, o =>
            {
                //登录路径：这是当用户试图访问资源但未经过身份验证时，程序将会将请求重定向到这个相对路径
                o.LoginPath = new PathString("/Home/Login");
                //禁止访问路径：当用户试图访问资源时，但未通过该资源的任何授权策略，请求将被重定向到这个相对路径。
                o.AccessDeniedPath = new PathString("/Home/Login");
            });//添加授权
        builder.Services.AddRazorPages().AddRazorRuntimeCompilation();//添加实时预览 
        #endregion

        var app = builder.Build();
        #region Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
        }
        app.UseStaticFiles();
        app.UseRouting();
        app.UseAuthentication();//认证
        app.UseAuthorization();//授权
        app.UseCookiePolicy();
        app.UseSession();
        app.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}"); 
        #endregion
        app.Run();
    }
}