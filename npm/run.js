#!/usr/bin/env node

let args = require("yargs")(process.argv.slice(2))
    .usage("Usage: dcl-edit [options]")
    .help("h").alias("h", "help").describe("h", "Show this help")
    //.alias("p", "projectPath").describe("p", "Set the decentraland project path. This is the path, where also the scene.json file lives. Defaults to current working directory")
    .argv;

let projectDir = process.cwd()

let binaryArgs = [
    "--projectPath",
    projectDir
]

const getBinary = require("./getBinary");
getBinary().runWithArgs(binaryArgs);

