<template>
    <div>
        <div class="row">
            <form>
                <div class="col-xs-12 col-md-6 col-lg-2">
                    <div class="form-group">
                        <label for="region">Region</label>
                        <select class="form-control" id="region" name="selectedRegion" v-model="selectedRegion">
                            <option v-for="region in availableRegions" v-bind:key="region">{{region}}</option>
                        </select>
                    </div>
                </div>

                <div class="col-xs-12 col-md-6 col-lg-2">
                    <div class="form-group">
                        <label for="selectedVCores">vCores</label>
                        <vue-slider id="selectedVCores" v-model="VCoreValues" :piecewise="true" :min="0" :max="64" :interval="2" tooltip-dir="bottom"
                            tooltip="always"></vue-slider>
                    </div>
                </div>

                <div class="col-xs-12 col-md-6 col-lg-2">
                    <div class="form-group">
                        <label for="">Minimum Memory (GB)</label>
                        <vue-slider id="selectedMemory" v-model="VMemoryValues" :piecewise="true" :min="0" :max="512" :interval="32" tooltip-dir="bottom"
                            tooltip="always"></vue-slider>
                    </div>
                </div>

            </form>
        </div>

        <div class="row" style="padding-top: 20px;">
            <div class="col-xs-12">
                <table-component :data="computedVmSizes" table-class="table">
                    <table-column show="friendlyName" label="Friendly name"></table-column>
                    <table-column show="tier" label="Tier"></table-column>
                    <table-column show="apiName" label="API Name"></table-column>
                    <table-column show="numberOfCores" label="vCores" data-type="numeric"></table-column>
                    <table-column show="memoryInMb" label="Memory (GiB)" data-type="numeric">
                        <template scope="row">
                            {{row.memoryInMb | formatByteSizes}}
                        </template>
                    </table-column>
                    <table-column show="maxNICs" label="Max NICs" data-type="numeric"></table-column>
                    <table-column show="gpuCount" label="GPUs" data-type="numeric"></table-column>
                    <table-column show="maxDataDiskCount" label="Maximum data disk count" data-type="numeric"></table-column>
                    <table-column show="osDiskSizeInMb" label="OS Disk Size (GiB)" data-type="numeric">
                        <template scope="row">
                            {{row.osDiskSizeInMb | formatByteSizes}}
                        </template>
                    </table-column>
                    <table-column show="resourceDiskSizeInMb" label="Temp storage Size (GiB)" data-type="numeric">
                        <template scope="row">
                            {{row.resourceDiskSizeInMb | formatByteSizes}}
                        </template>
                    </table-column>
                </table-component>
            </div>
        </div>
    </div>
</template>

<script>
import vmSizes  from '../data/vmSizesData';
import vueSlider  from 'vue-slider-component';
import { TableComponent, TableColumn} from 'vue-table-component';
import numeral from 'numeral';

export default  {
    components:{
        vueSlider,
        TableComponent,
        TableColumn
    },
    data: function(){
        return {
            // data to preload
            vmSizes: [],
            availableRegions: [],
            // default values
            selectedRegion: 'EastUS',
            VCoreValues: [0, 64],
            VMemoryValues: [0, 512]
        };
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
}
</script>

