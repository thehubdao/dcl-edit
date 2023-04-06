#!/usr/bin/env node
const os = require("os");

const type = os.type();

let availableGraphicsAPIs;

switch (type) {
    case "Windows_NT":
        availableGraphicsAPIs = ["auto","d11","d12","OpenGL","Vulkan"]
        break;
    case "Linux":
        availableGraphicsAPIs = ["auto","OpenGL","Vulkan"]
        break;
    case "Darwin":
        availableGraphicsAPIs = ["auto","Metal","OpenGL"]
        break;
    default:
        throw new Error(`Unsupported platform: ${type}`)
}


let args = require("yargs")(process.argv.slice(2))
    .usage("Usage: dcl-edit [options]")
    .help("h").alias("h", "help").describe("h", "Show this help")
    .alias("v", "version")
    .alias("p", "project-path").describe("p", "Set the decentraland project path. This is the path, where also the scene.json file lives. Defaults to current working directory")
    .alias("g", "graphics-api").describe("g", "Set the graphics API to use. Defaults to auto. Options: " + availableGraphicsAPIs.join(", "))
    .argv;


let projectDir = args.p ?? process.cwd()

let binaryArgs = [
    "--projectPath",
    projectDir
]


// check graphics api
// @type {string}
let graphicsAPI = args.g.toLowerCase();

if (graphicsAPI) {
    switch (graphicsAPI) {
        case "auto":
            break;
        case "d11":
            if(type != "Windows_NT") throw new Error(`DirectX 11 is only supported on Windows`)
            binaryArgs.push("-force-d3d11");
            break;
        case "d12":
            if(type != "Windows_NT") throw new Error(`DirectX 12 is only supported on Windows`)
            binaryArgs.push("-force-d3d12");
            break;
        case "opengl":
            // supports all platforms
            binaryArgs.push("-force-glcore");
            break;
        case "vulkan":
            if(type != "Linux" && type != "Windows_NT") throw new Error(`Vulkan is only supported on Linux and Windows`)
            binaryArgs.push("-force-vulkan");
            break;
        case "metal":
            if(type != "Darwin") throw new Error(`Metal is only supported on macOS`)
            binaryArgs.push("-force-metal");
            break;
        default:
            throw new Error(`Unsupported graphics API: ${graphicsAPI}. Supported graphics APIs: ${availableGraphicsAPIs.join(", ")}`)
    }
}


const getBinary = require("./getBinary");
getBinary().runWithArgs(binaryArgs);