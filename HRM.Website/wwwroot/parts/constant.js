const path = require('path');

const inputDir = 'sources';
const outputDir = 'dist';

const rootPath = path.resolve(__dirname, '..');

module.exports = {
    environment: {
        development: 'development',
        production: 'production'
    },
    projectPath: {
        root: rootPath,
        clientInputPath: path.join(rootPath, inputDir),
        clientOutputPath: path.join(rootPath, outputDir)
    }
};