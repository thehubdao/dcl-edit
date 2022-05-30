#!/usr/bin/env node

let args = require("yargs")(process.argv.slice(2))
    .usage("Usage: dcl-edit [options]")
    .help("h").alias("h", "help").describe("h", "Show this help")
    .argv;


const getBinary = require("./getBinary");
getBinary().run();

