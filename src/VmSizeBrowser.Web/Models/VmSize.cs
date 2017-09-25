using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VirtualMachineSizeBrowser.Models
{
    public class VmSize
    {
        public string FriendlyName
        {
            get { return String.Join('_', Name.Split('_').Skip(1).ToList()); }
        }
        public string Tier
        {
            get
            {
                return Name.Split('_')[0];
            }
        }
        public string ApiName
        {
            get { return Name; }
        }

        public int MaxDataDiskCount { get; set; }
        public int MemoryInMb { get; set; }
        public string Name { get; set; }
        public int NumberOfCores { get; set; }
        public int OSDiskSizeInMb { get; set; }
        public int ResourceDiskSizeInMb { get; set; }

        public int? MaxNICs { get; internal set; }
        public int? MaxIOPS { get; internal set; }
        public string NetworkPerformance { get; internal set; }
        public int GpuCount { get; internal set; }
    }
}
