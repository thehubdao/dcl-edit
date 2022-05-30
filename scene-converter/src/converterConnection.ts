import { Send } from 'express';
import * as WebSocket from 'ws';
import { Converter } from './converter';

export class ConnectionManager {
    private _wss: WebSocket.Server<WebSocket.WebSocket>;

    private _controlClients: WebSocket.WebSocket[] = [];
    private _sceneClient: WebSocket.WebSocket = null;

    constructor(server: WebSocket.Server<WebSocket.WebSocket>) {
        this._wss = server;


        this._wss.on('connection', (ws: WebSocket) => {
            ws.on('message', (message: string) => {
                try {
                    let json = JSON.parse(message);
                    console.log(json);

                    // register as control client
                    if (json.client === "control") {
                        console.log("control client connected");
                        this._controlClients.push(ws);

                        this.sendSceneConnectionState(ws)

                        return
                    }

                    // register as scene client
                    if (json.client === "scene") {
                        console.log("scene client connected");

                        // close existing scene client
                        if (this._sceneClient) {
                            this._sceneClient.close();
                        }

                        this._sceneClient = ws;

                        this.sendSceneConnectionStateToAll();

                        return
                    }

                    // convert action command from control client
                    if (json.action === "convert") {
                        this._sceneClient.send(JSON.stringify({ action: "send_scene" }));

                        return
                    }

                    // receive scene from scene client
                    if (json.action === "sending_scene") {
                        console.log("scene received");

                        new Converter().convertScene(json).then(({zipB64,entityReport,assetReport}) => {
                            this._controlClients
                                .forEach(
                                    ws => {
                                        ws.send(
                                            JSON.stringify(
                                                { action: "send_zip", zip: zipB64 }
                                            )
                                        )
                                        ws.send(
                                            JSON.stringify(
                                                { action: "send_report", report: entityReport+assetReport }
                                            )
                                        )
                                        
                                    }
                                );
                        });

                        return
                    }

                    console.log("unknown message");

                } catch (e) { console.log(e) }
            });

            ws.on("close", (code, reason) => {
                console.log("client disconnected");
                this._controlClients = this._controlClients.filter(client => client !== ws);
                if (this._sceneClient === ws) {
                    this._sceneClient = null;
                }

                this.sendSceneConnectionStateToAll();
            })

            console.log("WebSocket connection established");
        });

    }

    private sendSceneConnectionStateToAll() {
        this._controlClients.forEach(ws => this.sendSceneConnectionState(ws));
    }

    private sendSceneConnectionState(ws: WebSocket.WebSocket) {
        ws.send(JSON.stringify({ scene_connected: this._sceneClient !== null }));
    }

}
