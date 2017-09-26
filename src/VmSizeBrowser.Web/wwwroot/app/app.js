const $ = require('jquery');
const numeral = require('numeral');
const vmSizes = require('./data/vmSizesData');
const vueSlider = require('vue-slider-component')

let app = new Vue({
    el: '#app',
    components: {
        vueSlider
    },
    data: {
        // data to preload
        vmSizes: [],
        availableRegions: [],
        // default values
        selectedRegion: 'EastUS',
        VCoreValues: [0, 64],
        VMemoryValues: [0, 512]
    },
    computed: {
        computedVmSizes: function () {
            let self = this;
            return this.vmSizes.filter(function (item) {
                var memoryInGb = item.memoryInMb / 1024;
                return item.numberOfCores >= self.VCoreValues[0] && item.numberOfCores <= self.VCoreValues[1] &&
                    memoryInGb >= self.VMemoryValues[0] && memoryInGb <= self.VMemoryValues[1];
            });
        }
    },
    filters: {
        formatByteSizes: function (data) {
            return numeral(data / 1024).format('0[.]00');
        }

    },
    mounted: function () {
        let self = this;
        vmSizes.getAllVmSizesForRegionPromise(self.selectedRegion).then(function (data) {
            self.vmSizes = data;
        });

        vmSizes.getAllRegions().then(function (data) {
            self.availableRegions = data;
        })

    }
});
console.debug(app);
module.exports = app;