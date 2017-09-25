using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using VirtualMachineSizeBrowser.Models;
using VirtualMachineSizeBrowser.Data;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Linq;

namespace VirtualMachineSizeBrowser.Controllers
{
    [Produces("application/json")]
    [Route("api/VmSize")]
    public class VmSizeController : Controller
    {
        private readonly VmSizeStorage storage;

        public VmSizeController(VmSizeStorage storage)
        {
            this.storage = storage;
        }

        [Route("regions")]
        public async Task<IEnumerable<string>> GetRegions()
        {
            return await storage.GetRegions();
        }

        [Route("region/{region}")]
        public async Task<IEnumerable<VmSize>> GetForRegion(string region)
        {
            return await storage.GetAllVmSizeForRegion(region);
        }
    }
}