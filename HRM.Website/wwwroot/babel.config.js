module.exports = {
    presets: [
        ['@babel/preset-env', { targets: { node: 'current' } }],
        '@babel/preset-react',
    ],
    plugins: [
        '@babel/plugin-transform-class-properties',
        '@babel/plugin-transform-object-rest-spread',
        '@babel/plugin-syntax-dynamic-import',
        '@babel/plugin-transform-runtime',
    ],
};
