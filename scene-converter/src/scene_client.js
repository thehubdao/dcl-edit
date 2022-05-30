
let ws = new WebSocket("ws://localhost:8888")
ws.onopen = () => {
    log("Connected to server")
    ws.send(JSON.stringify({ client: "scene" }))
}

ws.onclose = () => {
    log("Disconnected from server")
}

ws.onmessage = (evt) => {
    try {
        let msg = JSON.parse(evt.data)
        log("Message from Conversion Server: " , msg)
        if (msg.action == "send_scene") {
            sendScene()
        }

    } catch (e) { log(e) }
}



function getEntityWorldPosition(entity) {
    let entityPosition = entity.hasComponent(Transform)
        ? entity.getComponent(Transform).position.clone()
        : Vector3.Zero()
    let parentEntity = entity.getParent()

    if (parentEntity != null && parentEntity.uuid != '0') {
        if (parentEntity.uuid == 'FirstPersonCameraEntityReference') {
            //log('ATTACHED TO CAMERA')
            let parentRotation = Camera.instance.rotation.clone()
            return Camera.instance.position
                .clone()
                .add(entityPosition.rotate(parentRotation))
        } else if (parentEntity.uuid == 'AvatarEntityReference') {
            //log('ATTACHED TO AVATAR')
            let camRotation = Camera.instance.rotation
            let parentRotation = Quaternion.Euler(0, camRotation.eulerAngles.y, 0)
            //log(Camera.instance.rotation.eulerAngles.y)
            return Camera.instance.position
                .clone()
                .add(entityPosition.rotate(parentRotation))
                .add(new Vector3(0, -0.875, 0))
        } else {
            let parentRotation = parentEntity.hasComponent(Transform)
                ? parentEntity.getComponent(Transform).rotation
                : Quaternion.Identity
            return getEntityWorldPosition(parentEntity).add(
                entityPosition.rotate(parentRotation)
            )
        }
    }
    return entityPosition
}

function getEntityWorldRotation(entity) {
    let entityRotation = entity.hasComponent(Transform)
        ? entity.getComponent(Transform).rotation.clone()
        : Quaternion.Zero()
    // log("GOT A ROTATION VALUE: ", entityRotation.eulerAngles)
    let parentEntity = entity.getParent()
    //   log("PARENT ENTITY: ", parentEntity.uuid)
    if (parentEntity != null && parentEntity.uuid != '0') {
        if (parentEntity.uuid == 'FirstPersonCameraEntityReference') {
            //log('ATTACHED TO CAMERA')
            let parentRotation = Camera.instance.rotation.clone()
            return entityRotation.multiply(parentRotation)
        } else if (parentEntity.uuid == 'AvatarEntityReference') {
            //log('ATTACHED TO AVATAR')
            let parentRotation = Quaternion.Euler(
                0,
                Camera.instance.rotation.eulerAngles.y,
                0
            )
            return entityRotation.multiply(parentRotation)
        } else {
            let parentRotation = getEntityWorldRotation(parentEntity)
            return entityRotation.multiply(parentRotation)
        }
    }
    return entityRotation
}

function constructSendableEntity(ent) {

    let sendableEnt = {}

    sendableEnt.name = ent.name

    var sendableComps = []
    for (let [key, value] of Object.entries(ent.components)) {
        let sendableComp = {
            name: key,
            data: { ...value.data } // clone the data
        }

        if (sendableComp.name === "engine.transform") {
            sendableComp.data.position = getEntityWorldPosition(ent)
            sendableComp.data.rotation = getEntityWorldRotation(ent)
        }

        sendableComps.push(sendableComp)
    }

    sendableEnt.components = sendableComps

    sendableEnt.children = []

    for (let [key, value] of Object.entries(ent.children)) {
        if (key === "AvatarEntityReference" || key === "FirstPersonCameraEntityReference") {
            continue
        }
        sendableEnt.children.push(constructSendableEntity(value))
    }

    return sendableEnt
}

function sendScene() {
    let sendableScene = constructSendableEntity(engine.rootEntity)

    sendableScene.action = "sending_scene"

    let sceneJson = JSON.stringify(sendableScene)
    log(sceneJson)

    ws.send(sceneJson)
}