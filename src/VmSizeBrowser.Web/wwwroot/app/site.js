const $ = require('jquery');
const numeral = require('numeral');

let app = new Vue({
    el: '#app',
    data: {
        vmSizes: [],
        selectedRegion: 'EastUS'

    },
    filters: {
        divideBy1024: function megabytesToGigabytes(data) {
            return numeral(data*1024*1024).format('0.00b');
        }

    },
    mounted: function () {
        let self = this;
        $.getJSON("/api/VmSize/region/" + self.selectedRegion).then(function (data) {
            self.vmSizes = data;
        });

    }
});

module.exports = app;