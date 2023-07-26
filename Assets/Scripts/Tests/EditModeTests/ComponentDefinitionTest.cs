using System;
using Assets.Scripts.SceneState;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using static Assets.Scripts.SceneState.DclComponent.DclComponentProperty;

namespace Assets.Scripts.Tests.EditModeTests
{
    public class ComponentDefinitionTest
    {
        [Test]
        public void MakeNewComponentFromDefinition()
        {
            var definition1 = new DclComponent.ComponentDefinition("C1", "C1", true, null, new PropertyDefinition("P1", PropertyType.String, "Some string"));

            var component1 = new DclComponent(definition1);

            Assert.AreEqual("C1", component1.NameInCode);
            Assert.AreEqual("C1", component1.NameOfSlot);
            Assert.AreEqual(1, component1.Properties.Count);
            Assert.AreEqual("P1", component1.Properties[0].PropertyName);
            Assert.AreEqual("Some string", component1.Properties[0].GetConcrete<string>().Value);
            Assert.AreEqual("Some string", component1.Properties[0].GetConcrete<string>().FixedValue);

            var definition2 = new DclComponent.ComponentDefinition("C2", "C2", true, null, new PropertyDefinition("P2", PropertyType.Int, 1234));

            var component2 = new DclComponent(definition2);

            Assert.AreEqual("C2", component2.NameInCode);
            Assert.AreEqual("C2", component2.NameOfSlot);
            Assert.AreEqual(1, component2.Properties.Count);
            Assert.AreEqual("P2", component2.Properties[0].PropertyName);
            Assert.AreEqual(1234, component2.Properties[0].GetConcrete<int>().Value);
            Assert.AreEqual(1234, component2.Properties[0].GetConcrete<int>().FixedValue);

            var definition3 = new DclComponent.ComponentDefinition("C3", "C3", true, null, new PropertyDefinition("P3", PropertyType.Float, 1234.0f));

            var component3 = new DclComponent(definition3);

            Assert.AreEqual("C3", component3.NameInCode);
            Assert.AreEqual("C3", component3.NameOfSlot);
            Assert.AreEqual(1, component3.Properties.Count);
            Assert.AreEqual("P3", component3.Properties[0].PropertyName);
            Assert.AreEqual(1234.0f, component3.Properties[0].GetConcrete<float>().Value);
            Assert.AreEqual(1234.0f, component3.Properties[0].GetConcrete<float>().FixedValue);

            var definition4 = new DclComponent.ComponentDefinition("C4", "C4", true, null, new PropertyDefinition("P4", PropertyType.Boolean, true));

            var component4 = new DclComponent(definition4);

            Assert.AreEqual("C4", component4.NameInCode);
            Assert.AreEqual("C4", component4.NameOfSlot);
            Assert.AreEqual(1, component4.Properties.Count);
            Assert.AreEqual("P4", component4.Properties[0].PropertyName);
            Assert.AreEqual(true, component4.Properties[0].GetConcrete<bool>().Value);
            Assert.AreEqual(true, component4.Properties[0].GetConcrete<bool>().FixedValue);

            var definition5 = new DclComponent.ComponentDefinition("C5", "C5", true, null, new PropertyDefinition("P5", PropertyType.Vector3, new Vector3(1, 2, 3)));

            var component5 = new DclComponent(definition5);

            Assert.AreEqual("C5", component5.NameInCode);
            Assert.AreEqual("C5", component5.NameOfSlot);
            Assert.AreEqual(1, component5.Properties.Count);
            Assert.AreEqual("P5", component5.Properties[0].PropertyName);
            Assert.AreEqual(new Vector3(1, 2, 3), component5.Properties[0].GetConcrete<Vector3>().Value);
            Assert.AreEqual(new Vector3(1, 2, 3), component5.Properties[0].GetConcrete<Vector3>().FixedValue);

            var definition6 = new DclComponent.ComponentDefinition("C6", "C6", true, null, new PropertyDefinition("P6", PropertyType.Quaternion, new Quaternion(1, 2, 3, 4)));

            var component6 = new DclComponent(definition6);

            Assert.AreEqual("C6", component6.NameInCode);
            Assert.AreEqual("C6", component6.NameOfSlot);
            Assert.AreEqual(1, component6.Properties.Count);
            Assert.AreEqual("P6", component6.Properties[0].PropertyName);
            Assert.AreEqual(new Quaternion(1, 2, 3, 4), component6.Properties[0].GetConcrete<Quaternion>().Value);
            Assert.AreEqual(new Quaternion(1, 2, 3, 4), component6.Properties[0].GetConcrete<Quaternion>().FixedValue);

            var definition7 = new DclComponent.ComponentDefinition("C7", "C7", true, null, new PropertyDefinition("P7", PropertyType.Asset, Guid.Empty));

            var component7 = new DclComponent(definition7);

            Assert.AreEqual("C7", component7.NameInCode);
            Assert.AreEqual("C7", component7.NameOfSlot);
            Assert.AreEqual(1, component7.Properties.Count);
            Assert.AreEqual("P7", component7.Properties[0].PropertyName);
            Assert.AreEqual(Guid.Empty, component7.Properties[0].GetConcrete<Guid>().Value);
            Assert.AreEqual(Guid.Empty, component7.Properties[0].GetConcrete<Guid>().FixedValue);
        }

        [Test]
        public void ConstructPropertyDefinition()
        {
            // should not throw
            new PropertyDefinition("name 1", PropertyType.Int, 1234);
            new PropertyDefinition("name 2", PropertyType.Float, 1234.0f);
            new PropertyDefinition("name 3", PropertyType.Boolean, true);
            new PropertyDefinition("name 4", PropertyType.Vector3, new Vector3(1, 2, 3));
            new PropertyDefinition("name 5", PropertyType.Quaternion, new Quaternion(1, 2, 3, 4));
            new PropertyDefinition("name 6", PropertyType.Asset, Guid.NewGuid());

            // type none should always throw
            Assert.Throws<ArgumentException>(() => new PropertyDefinition("name 14", PropertyType.None, 1234));
            Assert.Throws<ArgumentException>(() => new PropertyDefinition("name 15", PropertyType.None, "1234"));
            Assert.Throws<ArgumentException>(() => new PropertyDefinition("name 16", PropertyType.None, 1234.0f));
            Assert.Throws<ArgumentException>(() => new PropertyDefinition("name 17", PropertyType.None, true));
            Assert.Throws<ArgumentException>(() => new PropertyDefinition("name 18", PropertyType.None, new Vector3(1, 2, 3)));
            Assert.Throws<ArgumentException>(() => new PropertyDefinition("name 19", PropertyType.None, new Quaternion(1, 2, 3, 4)));
            Assert.Throws<ArgumentException>(() => new PropertyDefinition("name 20", PropertyType.None, Guid.NewGuid()));

            // type mismatch should throw
            Assert.Throws<ArgumentException>(() => new PropertyDefinition("name 7", PropertyType.String, 1234));
            Assert.Throws<ArgumentException>(() => new PropertyDefinition("name 8", PropertyType.Int, "1234"));
            Assert.Throws<ArgumentException>(() => new PropertyDefinition("name 9", PropertyType.Float, "1234.0f"));
            Assert.Throws<ArgumentException>(() => new PropertyDefinition("name 10", PropertyType.Boolean, "true"));
            Assert.Throws<ArgumentException>(() => new PropertyDefinition("name 11", PropertyType.Vector3, "1,2,3"));
            Assert.Throws<ArgumentException>(() => new PropertyDefinition("name 12", PropertyType.Quaternion, "1,2,3,4"));
            Assert.Throws<ArgumentException>(() => new PropertyDefinition("name 13", PropertyType.Asset, "1234"));

            // null should throw
            Assert.Throws<ArgumentException>(() => new PropertyDefinition("name 21", PropertyType.String, null));
        }

        [Test]
        public void MultipleProperties()
        {
            var component = new DclComponent(
                new DclComponent.ComponentDefinition(
                    "C1",
                    "S1",
                    true,
                    null,
                    new PropertyDefinition("P1", PropertyType.Int, 1234),
                    new PropertyDefinition("P2", PropertyType.Float, 1234.0f),
                    new PropertyDefinition("P3", PropertyType.Boolean, true),
                    new PropertyDefinition("P4", PropertyType.Vector3, new Vector3(1, 2, 3)),
                    new PropertyDefinition("P5", PropertyType.Quaternion, new Quaternion(1, 2, 3, 4)),
                    new PropertyDefinition("P6", PropertyType.Asset, Guid.Parse("00f5ef90-ef0e-4197-aa84-b73bfac4d22f"))));

            Assert.AreEqual("C1", component.NameInCode);
            Assert.AreEqual("S1", component.NameOfSlot);

            Assert.AreEqual(6, component.Properties.Count);

            Assert.AreEqual("P1", component.Properties[0].PropertyName);
            Assert.AreEqual(1234, component.Properties[0].GetConcrete<int>().Value);
            Assert.AreEqual(1234, component.Properties[0].GetConcrete<int>().FixedValue);

            Assert.AreEqual("P2", component.Properties[1].PropertyName);
            Assert.AreEqual(1234.0f, component.Properties[1].GetConcrete<float>().Value);
            Assert.AreEqual(1234.0f, component.Properties[1].GetConcrete<float>().FixedValue);

            Assert.AreEqual("P3", component.Properties[2].PropertyName);
            Assert.AreEqual(true, component.Properties[2].GetConcrete<bool>().Value);
            Assert.AreEqual(true, component.Properties[2].GetConcrete<bool>().FixedValue);

            Assert.AreEqual("P4", component.Properties[3].PropertyName);
            Assert.AreEqual(new Vector3(1, 2, 3), component.Properties[3].GetConcrete<Vector3>().Value);
            Assert.AreEqual(new Vector3(1, 2, 3), component.Properties[3].GetConcrete<Vector3>().FixedValue);

            Assert.AreEqual("P5", component.Properties[4].PropertyName);
            Assert.AreEqual(new Quaternion(1, 2, 3, 4), component.Properties[4].GetConcrete<Quaternion>().Value);
            Assert.AreEqual(new Quaternion(1, 2, 3, 4), component.Properties[4].GetConcrete<Quaternion>().FixedValue);

            Assert.AreEqual("P6", component.Properties[5].PropertyName);
            Assert.AreEqual(Guid.Parse("00f5ef90-ef0e-4197-aa84-b73bfac4d22f"), component.Properties[5].GetConcrete<Guid>().Value);
            Assert.AreEqual(Guid.Parse("00f5ef90-ef0e-4197-aa84-b73bfac4d22f"), component.Properties[5].GetConcrete<Guid>().FixedValue);
        }

        [Test]
        public void FollowingDefinition()
        {
            // Two examples of components that follow their definitions
            var component1 = new DclComponent("Name1", "Slot1");
            component1.Properties.Add(new DclComponent.DclComponentProperty<string>("Property1", "Some string"));
            component1.Properties.Add(new DclComponent.DclComponentProperty<int>("Property2", 1234));
            component1.Properties.Add(new DclComponent.DclComponentProperty<float>("Property3", 1234.0f));

            var definition1 = new DclComponent.ComponentDefinition(
                "Name1",
                "Slot1",
                true,
                null,
                new PropertyDefinition("Property1", PropertyType.String, "Some string"),
                new PropertyDefinition("Property2", PropertyType.Int, 1234),
                new PropertyDefinition("Property3", PropertyType.Float, 1234.0f));

            Assert.IsTrue(component1.IsFollowingDefinition(definition1));

            var component2 = new DclComponent("Name2", "Slot2");
            component2.Properties.Add(new DclComponent.DclComponentProperty<Vector3>("Property1", Vector3.one));
            component2.Properties.Add(new DclComponent.DclComponentProperty<Quaternion>("Property2", Quaternion.identity));
            component2.Properties.Add(new DclComponent.DclComponentProperty<Guid>("Property3", Guid.NewGuid()));

            var definition2 = new DclComponent.ComponentDefinition(
                "Name2",
                "Slot2",
                true,
                null,
                new PropertyDefinition("Property1", PropertyType.Vector3, Vector3.one),
                new PropertyDefinition("Property2", PropertyType.Quaternion, Quaternion.identity),
                new PropertyDefinition("Property3", PropertyType.Asset, Guid.NewGuid()));

            Assert.IsTrue(component2.IsFollowingDefinition(definition2));

            // Example of a component that has a different name than its definition
            var component3 = new DclComponent("NotTheCorrectName", "Slot3");
            component3.Properties.Add(new DclComponent.DclComponentProperty<Vector3>("Property1", Vector3.one));
            component3.Properties.Add(new DclComponent.DclComponentProperty<Quaternion>("Property2", Quaternion.identity));
            component3.Properties.Add(new DclComponent.DclComponentProperty<Guid>("Property3", Guid.NewGuid()));

            var definition3 = new DclComponent.ComponentDefinition(
                "Name3",
                "Slot3",
                true,
                null,
                new PropertyDefinition("Property1", PropertyType.Vector3, Vector3.one),
                new PropertyDefinition("Property2", PropertyType.Quaternion, Quaternion.identity),
                new PropertyDefinition("Property3", PropertyType.Asset, Guid.NewGuid()));

            Assert.IsFalse(component3.IsFollowingDefinition(definition3));

            // Example of a component that has a different slot than its definition
            var component4 = new DclComponent("Name4", "NotTheCorrectSlot");
            component4.Properties.Add(new DclComponent.DclComponentProperty<Vector3>("Property1", Vector3.one));
            component4.Properties.Add(new DclComponent.DclComponentProperty<Quaternion>("Property2", Quaternion.identity));
            component4.Properties.Add(new DclComponent.DclComponentProperty<Guid>("Property3", Guid.NewGuid()));

            var definition4 = new DclComponent.ComponentDefinition(
                "Name4",
                "Slot4",
                true,
                null,
                new PropertyDefinition("Property1", PropertyType.Vector3, Vector3.one),
                new PropertyDefinition("Property2", PropertyType.Quaternion, Quaternion.identity),
                new PropertyDefinition("Property3", PropertyType.Asset, Guid.NewGuid()));

            Assert.IsFalse(component4.IsFollowingDefinition(definition4));

            // Example of a component that has a property with a different name than its definition
            var component5 = new DclComponent("Name5", "Slot5");
            component5.Properties.Add(new DclComponent.DclComponentProperty<float>("PropertyFloat", 0.0f));

            var definition5 = new DclComponent.ComponentDefinition(
                "Name5",
                "Slot5",
                true,
                null,
                new PropertyDefinition("PropertyInt", PropertyType.Int, 0));

            Assert.IsFalse(component5.IsFollowingDefinition(definition5));

            // Example of a component that has a property with a different type but the same name than its definition
            var component6 = new DclComponent("Name6", "Slot6");
            component6.Properties.Add(new DclComponent.DclComponentProperty<float>("Property", 0.0f));

            var definition6 = new DclComponent.ComponentDefinition(
                "Name6",
                "Slot6",
                true,
                null,
                new PropertyDefinition("Property", PropertyType.Int, 0));

            Assert.IsFalse(component6.IsFollowingDefinition(definition6));

            // Example of a component that has a property to much
            var component7 = new DclComponent("Name7", "Slot7");
            component7.Properties.Add(new DclComponent.DclComponentProperty<float>("Property1", 0.0f));
            component7.Properties.Add(new DclComponent.DclComponentProperty<float>("Property2", 0.0f));

            var definition7 = new DclComponent.ComponentDefinition(
                "Name7",
                "Slot7",
                true,
                null,
                new PropertyDefinition("Property1", PropertyType.Float, 0.0f));

            Assert.IsFalse(component7.IsFollowingDefinition(definition7));

            // Example of a component that has a property missing
            var component8 = new DclComponent("Name8", "Slot8");
            component8.Properties.Add(new DclComponent.DclComponentProperty<float>("Property1", 0.0f));

            var definition8 = new DclComponent.ComponentDefinition(
                "Name8",
                "Slot8",
                true,
                null,
                new PropertyDefinition("Property1", PropertyType.Float, 0.0f),
                new PropertyDefinition("Property2", PropertyType.Float, 0.0f));

            Assert.IsFalse(component8.IsFollowingDefinition(definition8));

            // Example of a component that has the correct properties but in a different order
            var component9 = new DclComponent("Name9", "Slot9");
            component9.Properties.Add(new DclComponent.DclComponentProperty<float>("Property1", 0.0f));
            component9.Properties.Add(new DclComponent.DclComponentProperty<float>("Property2", 0.0f));

            var definition9 = new DclComponent.ComponentDefinition(
                "Name9",
                "Slot9",
                true,
                null,
                new PropertyDefinition("Property2", PropertyType.Float, 0.0f),
                new PropertyDefinition("Property1", PropertyType.Float, 0.0f));

            Assert.IsFalse(component9.IsFollowingDefinition(definition9));
        }
    }
}
