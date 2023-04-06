const { Binary } = require('binary-install-with-args');
const os = require('os');
const version = require('./package.json').version;

function getBinary(){

    const type = os.type();
    const arch = os.arch();

    if (type == "Windows_NT") {
        platformName = "windows-x86"
        binaryName = "dcl-edit.exe"
    } else if (type == "Linux") {
        platformName = "linux"
        binaryName = "dcl-edit"
    } else if (type == "Darwin") {
        platformName = "macos"
        binaryName = "Contents/MacOS/dcl-edit"
    } else {
        console.error("dcl-edit is not available for your platform (" + type + ")")
        throw new Error(`Unsupported platform`)
    }
    
    return new Binary(binaryName,`https://github.com/cblech/dcl-edit-automation/releases/download/${version}/dcl-edit-${version}-${platformName}.tar.gz`)
}

module.exports = getBinary;
