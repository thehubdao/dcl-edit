using System;
using Assets.Scripts.SceneState;
using NUnit.Framework;

namespace Assets.Scripts.Tests.EditModeTests
{
    [TestFixture]
    public class ChangeComponentTest
    {
        private DclEntity entity;
        private DclComponent component;
        private DclComponent component2;
        private DclEntity entity2;
        private DclComponent component2Entity;
        private DclComponent component2Entity2;

        [SetUp]
        public void Setup()
        {
            //First Entity
            entity = new DclEntity(Guid.NewGuid(), "TestName", Guid.Empty, false);
            
            component = new DclComponent("TestComponentName", "TestComponentSlotName");
            component2 = new DclComponent("TestComponentName2", "TestComponentSlotName2");
            
            var stringProperty = new DclComponent.DclComponentProperty<string>("Test String", "default string");
            var stringProperty2 = new DclComponent.DclComponentProperty<string>("Test String", "default string");
            
            component.Properties.Add(stringProperty);
            component2.Properties.Add(stringProperty2);
            
            component.Entity = entity;
            component2.Entity = entity;
            
            entity.AddComponent(component);
            entity.AddComponent(component2);
            
            
            //Second Entity
            entity2 = new DclEntity(Guid.NewGuid(), "TestName", Guid.Empty, false);
            
            component2Entity = new DclComponent("TestComponentName", "TestComponentSlotName");
            component2Entity2 = new DclComponent("TestComponentName2", "TestComponentSlotName2");
            
            var stringProperty2Entity = new DclComponent.DclComponentProperty<string>("Test String", "default string");
            var stringProperty2Entity2 = new DclComponent.DclComponentProperty<string>("Test String", "default string");
            
            component2Entity.Properties.Add(stringProperty2Entity);
            component2Entity2.Properties.Add(stringProperty2Entity2);
            
            component2Entity.Entity = entity2;
            component2Entity2.Entity = entity2;

            entity2.AddComponent(component2Entity);
            entity2.AddComponent(component2Entity2);
        }
        
        [Test]
        public void TestRemoveAndReAddComponent()
        {
            //Test
            entity.RemoveComponent(component);

            //Assertions
            Assert.IsTrue(component.Entity == null);
            Assert.IsTrue(entity.HasComponent(component.NameInCode) == false);
            Assert.IsFalse(component2.Entity == null);
            Assert.IsFalse(entity.HasComponent(component2.NameInCode) == false);
            
            Assert.IsFalse(component2Entity.Entity == null);
            Assert.IsFalse(entity2.HasComponent(component2Entity.NameInCode) == false);
            Assert.IsFalse(component2Entity2.Entity == null);
            Assert.IsFalse(entity2.HasComponent(component2Entity2.NameInCode) == false);
            
            //Test
            entity.AddComponent(component);

            //Assertions
            Assert.IsTrue(component.Entity == entity);
            Assert.IsTrue(entity.HasComponent(component.NameInCode) == true);
            Assert.IsTrue(component2.Entity == entity);
            Assert.IsTrue(entity.HasComponent(component2.NameInCode) == true);
            
            Assert.IsTrue(component2Entity.Entity == entity2);
            Assert.IsTrue(entity2.HasComponent(component2Entity.NameInCode) == true);
            Assert.IsTrue(component2Entity2.Entity == entity2);
            Assert.IsTrue(entity2.HasComponent(component2Entity2.NameInCode) == true);
        }
        
        [Test]
        public void TestRemoveAndReAddComponent2()
        {
            //Test
            entity.RemoveComponent(component2);

            //Assertions
            Assert.IsTrue(component2.Entity == null);
            Assert.IsTrue(entity.HasComponent(component2.NameInCode) == false);
            Assert.IsFalse(component.Entity == null);
            Assert.IsFalse(entity.HasComponent(component.NameInCode) == false);
            
            Assert.IsFalse(component2Entity2.Entity == null);
            Assert.IsFalse(entity2.HasComponent(component2Entity2.NameInCode) == false);
            Assert.IsFalse(component2Entity.Entity == null);
            Assert.IsFalse(entity2.HasComponent(component2Entity.NameInCode) == false);
            
            //Test
            entity.AddComponent(component2);

            //Assertions
            Assert.IsTrue(component2.Entity == entity);
            Assert.IsTrue(entity.HasComponent(component2.NameInCode) == true);
            Assert.IsTrue(component.Entity == entity);
            Assert.IsTrue(entity.HasComponent(component.NameInCode) == true);
            
            Assert.IsTrue(component2Entity2.Entity == entity2);
            Assert.IsTrue(entity2.HasComponent(component2Entity2.NameInCode) == true);
            Assert.IsTrue(component2Entity.Entity == entity2);
            Assert.IsTrue(entity2.HasComponent(component2Entity.NameInCode) == true);
        }
    }
}