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
                        <vue-slider id="selectedVCores" v-model="VCoreValues" :piecewise="true" :lazy="true" :min="0" :max="64" :interval="2" tooltip-dir="bottom"
                            tooltip="always"></vue-slider>
                    </div>
                </div>

                <div class="col-xs-12 col-md-6 col-lg-2">
                    <div class="form-group">
                        <label for="">Minimum Memory (GB)</label>
                        <vue-slider id="selectedMemory" v-model="VMemoryValues" :piecewise="true" :lazy="true" :min="0" :max="512" :interval="32" tooltip-dir="bottom"
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
            selectedRegion: this.$route.params.region || 'EastUS',
            VCoreValues: [this.$route.params.minVCore || 0, this.$route.params.maxVCore || 64],
            VMemoryValues: [this.$route.params.minMemory || 0, this.$route.params.maxMemory || 512]
        };
    },
    watch: {
        VCoreValues: function(current, previous){
            if(current[0] != previous[0] || current[1] != previous[1]){
                if(document.appInsights) {
                    appInsights.trackEvent('vCoreChanged', { minVCores: current[0], maxVCores: current[1] });
                }
                this.$router.push({name:'home', params: {region: this.selectedRegion, minVCore: current[0], maxVCore: current[1], minMemory: this.VMemoryValues[0], maxMemory: this.VMemoryValues[1]}});
            }
            
        },
        VMemoryValues: function(current, previous){
            if(current[0] != previous[0] || current[1] != previous[1]){
                if(document.appInsights) {
                    appInsights.trackEvent('memoryChanged', { minMemory: current[0], maxMemory: current[1] });
                }
                this.$router.push({name:'home', params: {region: this.selectedRegion, minVCore: this.VCoreValues[0], maxVCore: this.VCoreValues[1], minMemory: current[0], maxMemory: current[1]}});
            }
        },
        selectedRegion: function(current, previous){
            if(current != previous){
                if(document.appInsights) {
                    appInsights.trackEvent('regionChanged', { region: current });
                }
                this.$router.push({name:'home', params: {region: current, minVCore: this.VCoreValues[0], maxVCore: this.VCoreValues[1], minMemory: this.VMemoryValues[0], maxMemory: this.VMemoryValues[1]}});
            }
        },    
        '$route': function(route){
            var params = route.params;
            if(params.region !== this.selectedRegion){
                console.debug('region changed');
                this.selectedRegion = params.region;
            }

            if(params.minVCore !== this.VCoreValues[0] || params.maxVCore !== this.VCoreValues[1]){
                console.debug('cores changed');
                this.VCoreValues = [params.minVCore, params.maxVCore];
            }
            
            if(params.minMemory !== this.VMemoryValues[0] || params.maxMemory !== this.VMemoryValues[1]){
                console.debug('memory changed');
                this.VMemoryValues = [params.minMemory, params.maxMemory];
            }
        }    
    },
    methods: {
        vcoreChanged: function(event){
            console.debug('vcoreChanged', event);
        },
        memoryChanged: function(event){
            console.debug('memoryChanged', event);
        }
    },
    computed: {
        computedVmSizes: function () {
            var self = this;
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
        var self = this;
        vmSizes.getAllVmSizesForRegionPromise(self.selectedRegion).then(function (data) {
            self.vmSizes = data;
        });

        vmSizes.getAllRegions().then(function (data) {
            self.availableRegions = data;
        })

    }
}
</script>

