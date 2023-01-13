using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.SceneState;
using Zenject;

namespace Assets.Scripts.System
{
    public class EntityPresetSystem
    {
        public struct AvailablePreset
        {
            public int index;
            public string name;
        }

        //private readonly SortedList<int, EntityPreset> entityPresets = new SortedList<int, EntityPreset>();

        [Inject]
        public void Construct()
        {
            FillListWithInitialPresets();
        }

        private void FillListWithInitialPresets()
        {
            //// Empty entity (in its own category)
            //{
            //    var templateEntity = new DclEntity(Guid.Empty, "Empty Entity");
            //    templateEntity.AddComponent(new DclTransformComponent());
            //
            //    entityPresets.Add(1, new EntityPreset
            //    {
            //        name = "Empty Entity",
            //        templateEntity = templateEntity
            //    });
            //}
            //
            //// Category: Primitive shape entities
            //// Box Entity
            //{
            //    var templateEntity = new DclEntity(Guid.Empty, "Box Entity");
            //    templateEntity.AddComponent(new DclTransformComponent());
            //
            //    templateEntity.AddComponent(new DclComponent("BoxShape", "Shape"));
            //
            //    entityPresets.Add(1001, new EntityPreset
            //    {
            //        name = "Box Entity",
            //        templateEntity = templateEntity
            //    });
            //}
            //
            //// Sphere Entity
            //{
            //    var templateEntity = new DclEntity(Guid.Empty, "Sphere Entity");
            //    templateEntity.AddComponent(new DclTransformComponent());
            //
            //    templateEntity.AddComponent(new DclComponent("SphereShape", "Shape"));
            //
            //    entityPresets.Add(1002, new EntityPreset
            //    {
            //        name = "Sphere Entity",
            //        templateEntity = templateEntity
            //    });
            //}
            //
            //// Plane Entity
            //{
            //    var templateEntity = new DclEntity(Guid.Empty, "Plane Entity");
            //    templateEntity.AddComponent(new DclTransformComponent());
            //
            //    templateEntity.AddComponent(new DclComponent("PlaneShape", "Shape"));
            //
            //    entityPresets.Add(1003, new EntityPreset
            //    {
            //        name = "Plane Entity",
            //        templateEntity = templateEntity
            //    });
            //}
            //
            //// Cylinder Entity
            //{
            //    var templateEntity = new DclEntity(Guid.Empty, "Cylinder Entity");
            //    templateEntity.AddComponent(new DclTransformComponent());
            //
            //    templateEntity.AddComponent(new DclComponent("CylinderShape", "Shape"));
            //
            //    entityPresets.Add(1004, new EntityPreset
            //    {
            //        name = "Cylinder Entity",
            //        templateEntity = templateEntity
            //    });
            //}
            //
            //// Cone Entity
            //{
            //    var templateEntity = new DclEntity(Guid.Empty, "Cone Entity");
            //    templateEntity.AddComponent(new DclTransformComponent());
            //
            //    templateEntity.AddComponent(new DclComponent("ConeShape", "Shape"));
            //
            //    entityPresets.Add(1005, new EntityPreset
            //    {
            //        name = "Cone Entity",
            //        templateEntity = templateEntity
            //    });
            //}
        }

        public IEnumerable<AvailablePreset> availablePresets => throw new NotImplementedException(); // entityPresets.Select(preset => new AvailablePreset {index = preset.Key, name = preset.Value.name});

        public DclEntity GetTemplate(int index)
        {
            //return entityPresets[index].templateEntity;
            throw new NotImplementedException();
        }

        public string GetName(int index)
        {
            //return entityPresets[index].name;
            throw new NotImplementedException();
        }
    }
}
