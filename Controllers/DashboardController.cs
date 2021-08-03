using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using FantasyWealth.Areas.Identity.Data;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Linq;
using FantasyWealth.Models;
using Microsoft.AspNetCore.Mvc.Filters;
using FantasyWealth.Utilities;
using static FantasyWealth.Models.CompanyInfo;
using System.Collections.Generic;

namespace FantasyWealth.Controllers
{
   
    public class DashboardController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly Repository _repo;
        private readonly StockMasterDbContext _db;
        public DashboardController(Repository repo, UserManager<User> userManager, StockMasterDbContext db )
        {
            _repo = repo;
            _userManager = userManager;
            _db = db;
        }

        [ViewLayout("_Empty")]
        public async Task<IActionResult> Index()
        {
            User user = _repo.GetUser();
            int id = _repo.GetMyW(user);
            WatchListModel model = new WatchListModel();        
            
            List<CompanyPriceChange> list = _repo.GetMyWatchList(user, id);
            model.My_Commies = list;
            return View(model);
        }

            [ViewLayout("_Empty")]
            public async Task<IActionResult> Details(string symbol)
            {

               User user = _repo.GetUser();
               Company company = _repo.FindCompany(symbol);
                return View();
            }          
    }

    public class ViewLayoutAttribute : ResultFilterAttribute
    {
        private readonly string layout;
        public ViewLayoutAttribute(string layout)
        {
            this.layout = layout;
        }

        public override void OnResultExecuting(ResultExecutingContext context)
        {
            var viewResult = context.Result as ViewResult;
            if (viewResult != null)
            {
                viewResult.ViewData["Layout"] = layout;
            }
        }
    }
}