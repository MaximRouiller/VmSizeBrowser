using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using VirtualMachineSizeBrowser.Models;
using VirtualMachineSizeBrowser.Data;

namespace VirtualMachineSizeBrowser.Controllers
{
    public class HomeController : Controller
    {
        private readonly VmSizeStorage storage;

        public HomeController(VmSizeStorage storage)
        {
            this.storage = storage;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.Regions = await storage.GetRegions();
            return View();
        }
        
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
