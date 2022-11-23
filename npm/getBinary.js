const { Binary } = require('binary-install-with-args');
const os = require('os');
 
function getBinary() {
 
    const type = os.type();
    const arch = os.arch();
 
    const version = "0.4.5"
    let platformName = ""
    let binaryName = ""
 
    //if (type !== 'Windows_NT') throw new Error(`Unsupported platform: Currently only for Windows`);
 
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
 
    return new Binary(binaryName, "https://github.com/metagamehub/dcl-edit/releases/download/" + version + "/dcl-edit-" + version + "-" + platformName + ".tar.gz")
}
 
module.exports = getBinary;