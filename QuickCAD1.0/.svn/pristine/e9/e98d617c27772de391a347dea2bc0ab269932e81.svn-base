using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;
using WebApplication1.Models;
using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Controllers
{
    /// <summary>
    /// 菜单管理控制器
    /// </summary>
    [Authorize]
    public class MenuMgtController : Controller
    {
        private readonly ILogger<MenuMgtController> _logger;

        public MenuMgtController(ILogger<MenuMgtController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }
        
    }
}