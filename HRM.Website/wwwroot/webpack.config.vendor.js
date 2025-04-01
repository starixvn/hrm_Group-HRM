const path = require('path');
const webpack = require('webpack');
const MiniCssExtractPlugin = require('mini-css-extract-plugin');
const merge = require('webpack-merge');
const OptimizeCssAssetsPlugin = require('optimize-css-assets-webpack-plugin');
const CleanWebpackPlugin = require('clean-webpack-plugin');

const { environment, projectPath } = require('./parts/constant');

let pathsToClean = ['dist/vendors/*.*'];

let cleanOptions = {
    root: path.resolve(__dirname, '.'),
    verbose: true,
    dry: false
};

module.exports = env => {
    env = environment[env] || environment.development;
    const isDevBuild = env !== environment.production;
    const relativeOutputRootPath = projectPath.clientOutputPath;

    const sharedConfig = {
        mode: env,
        stats: { modules: false },
        module: {
            rules: [
                {
                    test: /\.(png|woff|woff2|eot|ttf)(\?|$)/,
                    use: [
                        {
                            loader: 'url-loader',
                            options: {
                                limit: 4096
                            }
                        }
                    ]
                }
            ]
        },
        entry: {
            react: ['react', 'react-dom'],
            chosen: ['chosen-js/chosen.css']
        },
        output: {
            filename: '[name].js',
            library: '[name]_[hash]'
        },
        plugins: [
            new webpack.DefinePlugin({
                'process.env.NODE_ENV': isDevBuild ? '"development"' : '"production"'
            }),
            new OptimizeCssAssetsPlugin()
        ]
    };

    const clientBundleConfig = merge(sharedConfig, {
        output: { publicPath: 'dist/vendors', path: path.join(relativeOutputRootPath, 'vendors') },
        module: {
            rules: [
                {
                    test: /\.s?[ac]ss$/,
                    use: [MiniCssExtractPlugin.loader, 'css-loader', 'sass-loader']
                }
            ]
        },
        plugins: [
            new MiniCssExtractPlugin({
                filename: '[name].css'
            }),
            new webpack.DllPlugin({
                path: path.join(relativeOutputRootPath, 'vendors', '[name]-manifest.json'),
                name: '[name]_[hash]'
            })
        ].concat(isDevBuild ? [] : [new CleanWebpackPlugin(pathsToClean, cleanOptions)])
    });

    return [clientBundleConfig];
};