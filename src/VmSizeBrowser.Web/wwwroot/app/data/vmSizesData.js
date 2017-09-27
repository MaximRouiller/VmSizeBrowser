import $ from 'jquery';

function getAllVmSizesForRegionPromise(regionCode) {
    return $.getJSON("/api/VmSize/region/" + regionCode);
}

function getAllRegions() {
    return $.getJSON("/api/VmSize/regions");
}

module.exports = {
    getAllVmSizesForRegionPromise,
    getAllRegions
};