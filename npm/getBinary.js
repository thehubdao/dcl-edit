const { Binary } = require('binary-install');
const os = require('os');

function getBinary(){

    const type = os.type();
    const arch = os.arch();

    if (type !== 'Windows_NT') throw new Error(`Unsupported platform: Currently only for Windows`);

    return new Binary("dcl-edit.exe","https://github.com/cblech/dcl-edit/releases/download/0.2.3/dcl-edit-0.2.3-windows-x86.tar.gz")
}

module.exports = getBinary;
