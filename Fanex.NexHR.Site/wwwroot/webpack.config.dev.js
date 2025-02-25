const { environment } = require('./parts/constant');
const config = require('./webpack.config');

const devConfig = config(environment.development)[0];
devConfig.devServer = {
    compress: true,
    port: 9000,
    hot: true,
    watchContentBase: true,
    historyApiFallback: true
};

module.exports = devConfig;