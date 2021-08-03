using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using FantasyWealth.Models;
using FantasyWealth.Utilities;
using Microsoft.AspNetCore.SignalR;

namespace FantasyWealth.Controllers
{
    public class HomeController : Controller
    {


      
        [HttpGet]
        public IActionResult Index(string SearchSymbol)
        {            
            return View();
        }

        //[ViewLayout("_Panel")]
        //[HttpGet]
        //public IActionResult Test()
        //{
        //    var data = iexTrading.getFunds("nio");
        //    WallViewModel model = new WallViewModel()
        //    {
        //        fundNames = data
        //    };
        //    return View(model);
        //}
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }


    public class ChatHub : Hub
    {

    }
}
