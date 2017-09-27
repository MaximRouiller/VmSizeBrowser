import Vue from 'vue';
import VueRouter from 'vue-router'
Vue.use(VueRouter);

import $ from 'jquery';

import home from './components/home.vue'

const routes = [{
    name: 'home',
    path: '/:region?/:minVCore?/:maxVCore?/:minMemory?/:maxMemory?',
    component: home
}];

const router = new VueRouter({
    routes
});

const app = new Vue({
    router
}).$mount('#app');

module.exports = app;