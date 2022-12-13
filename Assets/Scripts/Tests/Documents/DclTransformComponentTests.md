Test Structure: Grandparent->Parent->Cube
Checklist:
•	Rotating/Scaling an Entity should rotate/scale the following entities.
•	Rotating an Entity should rotate the gizmos of the following entities.
•	Scaling an Entity (on one axis) below -1 should not lead to a flipping scale.
•	Having an offset between parent and child should result in a moved gizmo. If the child has an offset, scaling and rotating the parent should let the child orbit around it at an angle without shearing. Rotating/Scaling the child should only affect the child (and its children)
•	Check if shearing appears to be working correctly.
