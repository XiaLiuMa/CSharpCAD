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
    /// 系统设置控制器
    /// </summary>
    [Authorize]
    public class SystemSetController : Controller
    {
        private readonly ILogger<SystemSetController> _logger;

        public SystemSetController(ILogger<SystemSetController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }


    }
}