import JSZip from "jszip"
import * as uuid from 'uuid'
import { isUpperCase } from "is-upper-case"

export class Converter {
    public async convertScene(sceneJson: { children: SendableEntity[] }): Promise<{ zipB64: string, entityReport: string, assetReport: string }> {
        this._entityCounter = 1
        this._entities = []
        this._gltfAssets = []
        this._entityReport = h1("Entity Report")
        this._assetReport = h1("Asset Report")

        for (const entity of sceneJson.children) {
            this.convertEntity(entity)
        }

        let saveJson: SaveJsonType = {
            entityNumberCounter: this._entityCounter,
            entities: this._entities
        }
        let saveJsonFileContents = JSON.stringify(saveJson, null, " ")

        let assetJson: AssetsJsonType = {
            gltfAssets: this._gltfAssets
        }

        let zip = new JSZip()

        let savesFolder = zip.folder("dcl-edit").folder("saves")

        savesFolder.file("save.json", saveJsonFileContents)
        savesFolder.file("assets.json", JSON.stringify(assetJson))
        savesFolder.file("version.json", versionJson)
        savesFolder.file("project.json", projectJson)

        let generatedZip = await zip.generateAsync({ type: "base64" })

        return { zipB64: generatedZip, entityReport: this._entityReport, assetReport: this._assetReport }
    }

    private _entityCounter = 1;
    private _entities: SaveJsonEntityType[] = [];
    private _gltfAssets: AssetJsonGltfType[] = [];
    private _entityReport: string = h1("Entity Report");
    private _assetReport: string = h1("Asset Report");

    private convertEntity(entity: SendableEntity, parentNumber?: number) {
        let rawName = entity.name
        let name = ""

        if (!rawName) {
            rawName = "Entity"
        }

        for (let i = 0; i < rawName.length; i++) {
            let c = rawName[i]
            if (isLetter(c) || isNumber(c) || c === " ") {
                name += c
            } else if (c === "_") {
                name += " "
            }
        }

        if (isNumber(name[0])) {
            name = "Entity " + name
        }

        name = name.trim()


        this._entityReport += h2("Added entity: " + name)

        let jsonEntity = this.makeBasicSaveJsonEntity(name);

        this._entityReport += h3("Unique number: " + jsonEntity.uniqueNumber)

        if (parentNumber) {
            jsonEntity.parent = parentNumber;
            this._entityReport += h3("Parent: " + parentNumber)
        } else {
            this._entityReport += h3("No Parent")
        }

        this._entityReport += h3("Components:")

        for (const component of entity.components) {
            let saveJsonComponent = this.makeSaveJsonComponent(component.name, component.data)
            if (saveJsonComponent !== null) {
                jsonEntity.components.push(saveJsonComponent);
            }

        }

        this._entities.push(jsonEntity);

        for (const child of entity.children) {
            this.convertEntity(child, jsonEntity.uniqueNumber);
        }
    }

    private makeBasicSaveJsonEntity(name: string): SaveJsonEntityType {
        let jsonEntity = {
            hierarchyOrder: this._entityCounter,
            name: name,
            uniqueNumber: this._entityCounter,
            parent: -1,
            exposed: false,
            collapsedChildren: false,
            components: [],
        }

        this._entityCounter++;

        return jsonEntity;
    }

    private makeSaveJsonComponent(name: string, data: any): SaveJsonEntityComponentType | null {
        switch (name) {
            case "engine.transform":
                return this.makeTransformComponent(data);
            case "engine.shape":
                return this.makeGltfShapeComponent(data);
            default:
                this._entityReport += h4err("Component \"" + name + "\" is not supported and was ignored")
                return null;
        }
    }

    private makeTransformComponent(data: any): SaveJsonEntityComponentType {
        let transformData = data as SendableTransformComponentData

        this._entityReport += h4("Transform")

        let saveJsonTransformComponent: SaveJsonTransformComponentType = {
            pos: {
                x: transformData.position.x,
                y: transformData.position.y,
                z: transformData.position.z
            },
            rot: {
                x: transformData.rotation.x,
                y: transformData.rotation.y,
                z: transformData.rotation.z,
                w: transformData.rotation.w
            },
            scale: {
                x: transformData.scale.x,
                y: transformData.scale.y,
                z: transformData.scale.z
            }
        }

        this._entityReport += h5("Position: " + transformData.position.x + ", " + transformData.position.y + ", " + transformData.position.z)
        this._entityReport += h5("Rotation: " + transformData.rotation.x + ", " + transformData.rotation.y + ", " + transformData.rotation.z + ", " + transformData.rotation.w)
        this._entityReport += h5("Scale: " + transformData.scale.x + ", " + transformData.scale.y + ", " + transformData.scale.z)

        return {
            name: "transform",
            specifics: JSON.stringify(saveJsonTransformComponent)
        }
    }

    private makeGltfShapeComponent(data: any): SaveJsonEntityComponentType | null {
        let gltfData = data as SendableGltfShapeComponentData

        if (gltfData.src === undefined) {
            this._entityReport += h4err("Only GLTFShapes are supported. Shape was ignored")
            return null
        }



        // check if src already exists
        let gltfAsset = this._gltfAssets.find(asset => asset.gltfPath === gltfData.src)

        if (gltfAsset === undefined) {
            let name = gltfData.src.split("/").pop()

            let guid = uuid.v4()
            gltfAsset = {
                gltfPath: gltfData.src,
                id: guid,
                name: this.beautifyName(name)
            }
            this._gltfAssets.push(gltfAsset)

            this._assetReport += h2("Added asset: " + gltfAsset.name)
            this._assetReport += h3("Id: " + gltfAsset.id)
            this._assetReport += h3("GltfPath: " + gltfAsset.gltfPath)
        }

        this._entityReport += h4("GLTF Shape: " + gltfAsset.name)
        this._entityReport += h5("Id: " + gltfAsset.id)
        this._entityReport += h5("GltfPath: " + gltfAsset.gltfPath)

        let shapeSpesifics: SaveJsonGltfShapeComponentType = { assetID: gltfAsset.id }

        return {
            name: "GLTFShape",
            specifics: JSON.stringify(shapeSpesifics)
        }
    }

    private beautifyName(name: string): string {
        name = name.replace("_", " ")
        name = name.replace(".gltf", "")
        name = name.replace(".glb", "")

        let beautyName = ""

        for (let i = 0; i < name.length; i++) {
            let currentChar = name[i]
            beautyName += currentChar

            if (i < name.length - 1) {
                let nextChar = name[i + 1]
                let nextNextChar = "A"
                if (i < name.length - 2) {
                    nextNextChar = name[i + 2]
                }
                if (currentChar !== " " && (!isUpperCase(currentChar) || (!isUpperCase(nextNextChar) && !(nextNextChar === " "))) && isUpperCase(nextChar)) {
                    beautyName += " "
                }
            }
        }

        return beautyName
    }


}

const letters = ["A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z", "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z"];
const numbers = ["0", "1", "2", "3", "4", "5", "6", "7", "8", "9"];

function isLetter(char: string): boolean {
    return letters.includes(char)
}

function isNumber(char: string): boolean {
    return numbers.includes(char)
}

function h1(text: string): string {
    return `<h1>${text}</h1>`
}
function h2(text: string): string {
    return `<h2>${text}</h2>`
}
function h3(text: string): string {
    return `<h3>${text}</h3>`
}
function h4(text: string): string {
    return `<h4>${text}</h4>`
}
function h4err(text: string): string {
    return `<h4 class="error">${text}</h4>`
}
function h5(text: string): string {
    return `<h5>${text}</h5>`
}

function br(text: string): string {
    return `${text}<br/>`
}

type SendableEntity = {
    name: string,
    children: SendableEntity[]
    components: {
        name: string,
        data: any
    }[]
}

type SendableTransformComponentData = {
    position: {
        x: number,
        y: number,
        z: number
    },
    rotation: {
        x: number,
        y: number,
        z: number,
        w: number
    },
    scale: {
        x: number,
        y: number,
        z: number
    }
}

type SendableGltfShapeComponentData = {
    src: string
}

type SaveJsonType = {
    entityNumberCounter: number,
    entities: SaveJsonEntityType[]
}

type SaveJsonEntityType = {
    hierarchyOrder: number,
    name: string,
    uniqueNumber: number,
    parent: number,
    exposed: boolean,
    collapsedChildren: boolean,
    components: SaveJsonEntityComponentType[]
}

type SaveJsonEntityComponentType = {
    name: string,
    specifics: string
}

type SaveJsonTransformComponentType = {
    pos: {
        x: number,
        y: number,
        z: number
    },
    rot: {
        x: number,
        y: number,
        z: number,
        w: number
    },
    scale: {
        x: number,
        y: number,
        z: number
    }
}

type SaveJsonGltfShapeComponentType = {
    assetID: string
}

type AssetsJsonType = {
    gltfAssets: AssetJsonGltfType[]
}

type AssetJsonGltfType = {
    name: string,
    id: string,
    gltfPath: string
}


const versionJson = JSON.stringify({ version: 1 })

const projectJson = JSON.stringify({
    translateSnapStep: 0.25,
    rotateSnapSte: 30.0,
    scaleSnapStep: 0.25,
    generateScriptLocation: "src/scene.ts"
})
