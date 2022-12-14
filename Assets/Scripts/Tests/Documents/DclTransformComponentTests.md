<!-- omit in toc -->
# DclTransformComponent Tests 

<!-- omit in toc -->
## Table of Contents

- [Setup](#setup)
- [Tests](#tests)
  - [Basic Operations](#basic-operations)
  - [Special Cases](#special-cases)
- [Example](#example)

---

## Setup
Add a Testcube with an empty parent and an empty grandparent entity to the scene.

---

## Tests

### Basic Operations
* Scaling an entity should neither change the gizmo of that entity nor those of the children entities.
* Rotating an entity should rotate the gizmos of the children entities in the same way.
* Moving an entity should move the gizmos of the children entities in the same way.
* The TRS of parents should never change when the children/grandchildren etc. change. 

### Special Cases
* Scaling an entity below 0 should result in a negative scale, not in a rotation.
* Scaling an entity below 0 should never induce flickering
* Check if shearing appears to work as expected

---

## Example
If a child entity has an offset to its parent and the parent is scaled and rotated, the child should orbit around it at an angle, without shearing. Rotating/Scaling the child should only affect the child (and its children).
