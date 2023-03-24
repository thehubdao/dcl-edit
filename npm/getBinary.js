const { Binary } = require('binary-install');
const os = require('os');
const version = require('./package.json').version;

function getBinary(){

    const type = os.type();
    const arch = os.arch();

    if (type !== 'Windows_NT') throw new Error(`Unsupported platform: Currently only for Windows`);

    return new Binary("dcl-edit.exe",`https://github.com/cblech/dcl-edit-automation/releases/download/${version}/dcl-edit-${version}-windows-x86.tar.gz`)
}

module.exports = getBinary;
