const path = require('path');

module.exports = {
    entry: './wwwroot/app/site.js',
    output: {
        filename: 'bundle.js',
        path: path.resolve('./wwwroot/dist')
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
        }]
    }
};