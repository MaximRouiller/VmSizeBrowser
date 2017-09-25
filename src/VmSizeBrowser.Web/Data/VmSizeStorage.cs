using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VirtualMachineSizeBrowser.Models;

namespace VirtualMachineSizeBrowser.Data
{
    public class VmSizeStorage
    {
        private readonly string connectionString;
        private readonly TimeSpan defaultCacheLength = TimeSpan.FromHours(1);
        private readonly IMemoryCache cache;

        public VmSizeStorage(IOptions<AppSettings> appSettings, IMemoryCache cache)
        {
            connectionString = appSettings.Value.StorageConnectionString;
            this.cache = cache;
        }

        public async Task<IEnumerable<VmSize>> GetAllVmSizeForRegion(string region)
        {
            IEnumerable<VmSize> sizesForRegion;

            if (!cache.TryGetValue($"Region_{region}", out sizesForRegion))
            {
                CloudStorageAccount account;
                CloudStorageAccount.TryParse(connectionString, out account);

                var blobClient = account.CreateCloudBlobClient();
                var container = blobClient.GetContainerReference("default");
                var vmSizesDirectory = container.GetDirectoryReference("vmSizes");

                var file = vmSizesDirectory.GetBlockBlobReference($"{region}.json");

                if (!await file.ExistsAsync())
                {
                    return Enumerable.Empty<VmSize>();
                }


                var content = (await file.DownloadTextAsync(Encoding.Unicode, null, null, null)).Trim(new char[] { '\uFEFF', '\u200B' });
                sizesForRegion = JsonConvert.DeserializeObject<List<VmSize>>(content).Where(x => !x.Name.EndsWith("_Promo")).ToList();
                IDictionary<string, SerieInfo> seriesInfo = await GetSeriesInfo();

                foreach (var vmSize in sizesForRegion)
                {
                    if (seriesInfo.ContainsKey(vmSize.ApiName))
                    {
                        var currentSeries = seriesInfo[vmSize.ApiName];
                        vmSize.MaxNICs = currentSeries.MaxNICs;
                        vmSize.MaxIOPS = currentSeries.MaxIOPS;
                        vmSize.NetworkPerformance = currentSeries.NetworkPerformance;
                        vmSize.GpuCount = currentSeries.GpuCount;
                    }
                }
                cache.Set($"Region_{region}", sizesForRegion, defaultCacheLength);
            }
            return sizesForRegion;
        }
        public async Task<IEnumerable<string>> GetRegions()
        {
            IEnumerable<string> regions;
            if (!cache.TryGetValue("AllRegions", out regions))
            {
                CloudStorageAccount account;
                CloudStorageAccount.TryParse(connectionString, out account);

                var blobClient = account.CreateCloudBlobClient();
                var container = blobClient.GetContainerReference("default");
                var vmSizesDirectory = container.GetDirectoryReference("vmSizes");

                var files = await vmSizesDirectory.ListBlobsSegmentedAsync(true, BlobListingDetails.All, null, null, null, null);
                regions = files.Results.Cast<CloudBlockBlob>()
                    .Select(x => Path.GetFileNameWithoutExtension(x.Name))
                    .OrderBy(x => x)
                    .ToList();
                cache.Set("AllRegions", regions, defaultCacheLength);
            }
            return regions;
        }

        private async Task<IDictionary<string, SerieInfo>> GetSeriesInfo()
        {
            CloudStorageAccount account;
            CloudStorageAccount.TryParse(connectionString, out account);

            var blobClient = account.CreateCloudBlobClient();
            var container = blobClient.GetContainerReference("default");
            var staticDirectory = container.GetDirectoryReference("static");

            var file = staticDirectory.GetBlockBlobReference("series.json");

            if (!await file.ExistsAsync())
            {
                return new Dictionary<string, SerieInfo>();
            }
            var content = (await file.DownloadTextAsync(Encoding.Default, null, null, null)).Trim(new char[] { '\uFEFF', '\u200B' });
            IDictionary<string, SerieInfo> seriesInfo = JsonConvert.DeserializeObject<IDictionary<string, SerieInfo>>(content);

            return seriesInfo;
        }
    }
}
