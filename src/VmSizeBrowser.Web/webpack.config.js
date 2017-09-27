const path = require('path');

module.exports = {
    entry: './wwwroot/app/app.js',
    output: {
        filename: 'bundle.js',
        path: path.resolve('./wwwroot/dist')
    },  resolve: {
        alias: {
          'vue': 'vue/dist/vue.js'
        }
      },
    module: {
        rules: [{
            test: /\.js$/,
            exclude: /(node_modules|lib|dist)/,
            use: {
                loader: 'babel-loader',
                options: {
                    presets: ['env']
                }
            }
        },{
            test: /\.vue$/,
            exclude: /(node_modules|lib|dist)/,
            use: {
                loader: 'vue-loader'
            }
        }]
    }
};