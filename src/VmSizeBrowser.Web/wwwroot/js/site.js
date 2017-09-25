// var app = new Vue({
//     el: '#app',
//     data: {
//         message: 'Hello Vue!',
//         vmSizes: []
//     },
//     filters: {
//         divideBy1024: function megabytesToGigabytes(data) {
//             var result = (data / 1024);
//             return result == result.toFixed(0) ? result : result.toFixed(2);
//         }

//     },
//     mounted: function () {
//         var self = this;
//         $("#region").val('EastUS'); // set default initial value.
//         $.getJSON("/api/VmSize/region/" + $("#region").val()).then(function (data) {
//             self.vmSizes = data;
//         });

//     }
// });


$(document).ready(function () {
   $("#region").val('EastUS'); // set default initial value.
   $("#table").DataTable({
       "ajax": {
           "url": "/api/VmSize/region/" + $("#region").val(),
           "dataSrc": ""
       },
       "columns": [
           { "data": "friendlyName" },
           { "data": "tier" },
           { "data": "apiName" },
           { "data": "numberOfCores" },
           { "data": "memoryInMb", render: megabytesToGigabytes },
           { "data": "maxNICs" },
           { "data": "gpuCount" },
           { "data": "maxDataDiskCount" },
           { "data": "osDiskSizeInMb", render: megabytesToGigabytes },
           { "data": "resourceDiskSizeInMb", render: megabytesToGigabytes }
       ]
   });

   $("#region").on('input', function () {
       appInsights.trackEvent('regionChanged', { region: $(this).val() });
       $("#table").DataTable().ajax.url("/api/VmSize/region/" + $(this).val());
       $("#table").DataTable().ajax.reload();
   });
   $("#minimumVCore").on('input', function () {
       $("#selectedvCores").text($(this).val());

   });
   $("#minimumMemory").on('input', function () {
       $("#selectedMemory").text($(this).val());

   });

   $("#minimumVCore").on('change', function () {
       debugger;
       appInsights.trackEvent('vCoreChanged', { minVCores: $(this).val() });
       $("#table").DataTable().table().draw();
   });
   $("#minimumMemory").on('change', function () {
       debugger;
       appInsights.trackEvent('memoryChanged', { minMemory: $(this).val() });
       $("#table").DataTable().table().draw();
   });

   $.fn.dataTable.ext.search.push(
      function (settings, data, dataIndex) {
          var minMemory = parseInt($('#minimumMemory').val(), 0);
          var minVCore = parseInt($('#minimumVCore').val(), 0);
          var vcore = parseFloat(data[3]) || 0;
          var memory = parseFloat(data[4]) || 0;
   
          if (minVCore <= vcore && minMemory <= memory) {
              return true;
          }
          return false;
      }
   );
   function megabytesToGigabytes(data) {
      return data / 1024;
   }
});