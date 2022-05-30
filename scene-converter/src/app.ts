#!/usr/bin/env node
import express from "express";
import bodyParser from "body-parser";
import * as http from 'http';
import * as WebSocket from 'ws';
import cors from 'cors';
import { ConnectionManager } from "./converterConnection";

const app = express();

app.use(cors())

app.get("/client.js",(req,res)=>{
    res.sendFile(__dirname+"/scene_client.js");
})

app.get("/",(req,res)=>{
    res.sendFile(__dirname+"/index.html");
})

app.listen(8887,()=>{
    console.log("Server started at port 8887");
    console.log("#####################################################")
    console.log("#### open http://localhost:8887/ in your browser ####")
    console.log("#####################################################")
})

// websocket
const server = http.createServer(app);

const wss = new WebSocket.Server({ server });

const connectionManager = new ConnectionManager(wss);

//start our server
server.listen(8888, () => {
    console.log(`Server started on port ${(server.address() as WebSocket.AddressInfo).port} :)`);
});


/**
 * Paste this into the game.ts file
;(async function(){
    let script = await (await fetch("http://localhost:8887/client.js")).text()
    eval(script)
})();
 */
