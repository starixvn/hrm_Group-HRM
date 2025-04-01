const path = require('path');
const webpack = require('webpack');
const MiniCssExtractPlugin = require('mini-css-extract-plugin');
const OptimizeCSSAssetsPlugin = require('optimize-css-assets-webpack-plugin');
const merge = require('webpack-merge');
const CleanWebpackPlugin = require('clean-webpack-plugin');

let pathsToClean = ['dist/js/*.*', 'dist/styles/*.*'];

let cleanOptions = {
    root: path.resolve(__dirname, '.'),
    verbose: true,
    dry: false
};

const { environment, projectPath } = require('./parts/constant');
const entry = require('./parts/entry');

module.exports = env => {
    env = environment[env] || environment.development;
    const isDevBuild = env !== environment.production;

    const sharedConfig = () => ({
        mode: env,
        stats: { modules: false },
        resolve: {
            extensions: ['.js', '.jsx']
        },
        output: {
            filename: '[name].js',
            chunkFilename: 'js/[name].[chunkhash].js',
            publicPath: `/dist/`
        },
        module: {
            rules: [
                {
                    test: /\.(js|jsx)$/,
                    use: 'babel-loader',
                    exclude: /(node_modules|bower_components)/
                },
                {
                    test: /.(ttf|otf|eot|svg|woff(2)?)(\?[a-z0-9]+)?$/,
                    use: [{
                        loader: 'file-loader',
                        options: {
                            name: '[name].[ext]',
                            outputPath: 'styles/fonts/',
                            publicPath: 'fonts/'
                        }
                    }]
                },
                {
                    test: /\.(png|jpg|jpeg|gif)$/,
                    use: [
                        {
                            loader: 'url-loader',
                            options: {
                                limit: 8192,
                                name: '[name].[ext]',
                                outputPath: 'styles/images/',
                                publicPath: 'images/'
                            }
                        }
                    ]
                }
            ]
        }
    });

    // Configuration for client-side bundle suitable for running in browsers
    const clientBundleOutputDir = projectPath.clientOutputPath;
    const clientBundleConfig = merge(sharedConfig(), {
        entry: entry.client,
        module: {
            rules: [
                {
                    test: /\.s?[ac]ss$/,
                    use: [
                        'style-loader',
                        MiniCssExtractPlugin.loader,
                        'css-loader',
                        'sass-loader'
                    ]
                }
            ]
        },
        output: { path: clientBundleOutputDir },
        plugins: [
            new webpack.DefinePlugin({
                'process.env.NODE_ENV': isDevBuild ? '"development"' : '"production"'
            }),
            new MiniCssExtractPlugin({
                filename: '[name].css'
            }),
            new webpack.ProvidePlugin({
                jQuery: 'jquery',
                $: 'jquery',
                jquery: 'jquery',
                'window.$': 'jquery',
                "window.jQuery": 'jquery'
            })
        ].concat(
            isDevBuild ? [] : [new CleanWebpackPlugin(pathsToClean, cleanOptions), new OptimizeCSSAssetsPlugin()]
        ),
        devtool: 'source-map'
    });

    return [clientBundleConfig];
};