using Assets.Scripts.System;
using NUnit.Framework;

using Assets.Scripts.Events;
using Assets.Scripts.EditorState;
using UnityEngine.Events;
using System;

namespace Assets.Scripts.Tests.EditModeTests
{
    public class MenuBarSystemTest
    {
        int eventCounter;

        [Test]
        public void ExceptionMenuBarNullCallBack()
        {
            //construct system
            EditorEvents editorEvents = new EditorEvents();
            MenuBarState menuBarState = new MenuBarState();
            MenuBarSystem menuBarSystem = new MenuBarSystem();
            menuBarSystem.Construct(editorEvents, menuBarState);

            Assert.Throws<ArgumentException>(() => { menuBarSystem.AddMenuItem("A1/A2/A3", null); });
        }

        [Test]
        public void ExceptionMenuNoDepth()
        {
            //construct system
            EditorEvents editorEvents = new EditorEvents();
            MenuBarState menuBarState = new MenuBarState();
            MenuBarSystem menuBarSystem = new MenuBarSystem();
            menuBarSystem.Construct(editorEvents, menuBarState);

            Assert.Throws<ArgumentException>(() => { menuBarSystem.AddMenuItem("A1", TestUnityAction); });
        }

        [Test]
        public void ExceptionMenuBarEmptyElement()
        {
            //construct system
            EditorEvents editorEvents = new EditorEvents();
            MenuBarState menuBarState = new MenuBarState();
            MenuBarSystem menuBarSystem = new MenuBarSystem();
            menuBarSystem.Construct(editorEvents, menuBarState);

            Assert.Throws<ArgumentException>(() => { menuBarSystem.AddMenuItem("A1//A3", TestUnityAction); });
        }

        [Test]
        public void ExceptionMenuInvalid()
        {
            //construct system
            EditorEvents editorEvents = new EditorEvents();
            MenuBarState menuBarState = new MenuBarState();
            MenuBarSystem menuBarSystem = new MenuBarSystem();
            menuBarSystem.Construct(editorEvents, menuBarState);

            Assert.Throws<ArgumentException>(() => { menuBarSystem.AddMenuItem("A1/A2#1x/A3", TestUnityAction); });
        }

        [Test]
        public void AddSimpleItem()
        {
            //construct system
            EditorEvents editorEvents = new EditorEvents();
            MenuBarState menuBarState = new MenuBarState();
            MenuBarSystem menuBarSystem = new MenuBarSystem();
            menuBarSystem.Construct(editorEvents, menuBarState);

            //prepare
            eventCounter = 0;
            editorEvents.onUpdateMenuBarEvent += CatchEvent;
            UnityAction testUnityAction = new UnityAction(TestUnityAction);

            //add menu items
            menuBarSystem.AddMenuItem("A1/A2", testUnityAction);
            CheckEventCounter(1);

            //asserts
            Assert.AreEqual(1, menuBarState.menuItems.Count);

            MenuBarState.MenuBarItem a1 = menuBarState.menuItems[0];
            Assert.AreEqual("A1", a1.title);
            ContextMenuTextItem a2 = a1.subItems[0] as ContextMenuTextItem;
            Assert.AreEqual("A2", a2.title);
            Assert.AreEqual(testUnityAction, a2.onClick);
        }

        [Test]
        public void AddDeepItem()
        {
            //construct system
            EditorEvents editorEvents = new EditorEvents();
            MenuBarState menuBarState = new MenuBarState();
            MenuBarSystem menuBarSystem = new MenuBarSystem();
            menuBarSystem.Construct(editorEvents, menuBarState);

            //prepare
            eventCounter = 0;
            editorEvents.onUpdateMenuBarEvent += CatchEvent;
            UnityAction testUnityAction = new UnityAction(TestUnityAction);

            //add menu items
            menuBarSystem.AddMenuItem("A1/A2/A3/A4/A5/A6", testUnityAction);
            CheckEventCounter(1);

            //asserts
            Assert.AreEqual(1, menuBarState.menuItems.Count);
            MenuBarState.MenuBarItem a1 = menuBarState.menuItems[0];
            Assert.AreEqual("A1", a1.title);
            ContextSubmenuItem a2 = a1.subItems[0] as ContextSubmenuItem;
            Assert.AreEqual("A2", a2.title);
            ContextSubmenuItem a3 = a2.items[0] as ContextSubmenuItem;
            Assert.AreEqual("A3", a3.title);
            ContextSubmenuItem a4 = a3.items[0] as ContextSubmenuItem;
            Assert.AreEqual("A4", a4.title);
            ContextSubmenuItem a5 = a4.items[0] as ContextSubmenuItem;
            Assert.AreEqual("A5", a5.title);
            ContextMenuTextItem a6 = a5.items[0] as ContextMenuTextItem;
            Assert.AreEqual("A6", a6.title);
        }

        [Test]
        public void AddItemsWithPriority()
        {
            //construct system
            EditorEvents editorEvents = new EditorEvents();
            MenuBarState menuBarState = new MenuBarState();
            MenuBarSystem menuBarSystem = new MenuBarSystem();
            menuBarSystem.Construct(editorEvents, menuBarState);

            //prepare
            eventCounter = 0;
            editorEvents.onUpdateMenuBarEvent += CatchEvent;
            UnityAction testUnityAction = new UnityAction(TestUnityAction);

            //add menu items
            menuBarSystem.AddMenuItem("A1#5/A2#10", testUnityAction);
            menuBarSystem.AddMenuItem("A1#5/A3#9", testUnityAction);
            menuBarSystem.AddMenuItem("A1#5/A4#11", testUnityAction);
            menuBarSystem.AddMenuItem("B1#-10/B2", testUnityAction);
            menuBarSystem.AddMenuItem("C1#10/C2", testUnityAction);
            CheckEventCounter(5);

            //asserts
            Assert.AreEqual(3, menuBarState.menuItems.Count);

            MenuBarState.MenuBarItem a1 = menuBarState.menuItems[1];
            Assert.AreEqual("A1", a1.title);

            ContextMenuState.SortItems(a1.subItems);

            ContextMenuTextItem a2 = a1.subItems[1] as ContextMenuTextItem;
            Assert.AreEqual("A2", a2.title);
            ContextMenuTextItem a3 = a1.subItems[0] as ContextMenuTextItem;
            Assert.AreEqual("A3", a3.title);
            ContextMenuTextItem a4 = a1.subItems[2] as ContextMenuTextItem;
            Assert.AreEqual("A4", a4.title);

            MenuBarState.MenuBarItem b1 = menuBarState.menuItems[0];
            Assert.AreEqual("B1", b1.title);
            ContextMenuTextItem b2 = b1.subItems[0] as ContextMenuTextItem;
            Assert.AreEqual("B2", b2.title);

            MenuBarState.MenuBarItem c1 = menuBarState.menuItems[2];
            Assert.AreEqual("C1", c1.title);
            ContextMenuTextItem c2 = c1.subItems[0] as ContextMenuTextItem;
            Assert.AreEqual("C2", c2.title);
        }

        [Test]
        public void TestMenuBarTree()
        {
            //construct system
            EditorEvents editorEvents = new EditorEvents();
            MenuBarState menuBarState = new MenuBarState();
            MenuBarSystem menuBarSystem = new MenuBarSystem();
            menuBarSystem.Construct(editorEvents, menuBarState);

            //prepare
            eventCounter = 0;
            editorEvents.onUpdateMenuBarEvent += CatchEvent;
            UnityAction testUnityAction = new UnityAction(TestUnityAction);

            //add menu items
            menuBarSystem.AddMenuItem("A1/A2/A3", testUnityAction);
            menuBarSystem.AddMenuItem("B1/B2", testUnityAction);
            menuBarSystem.AddMenuItem("B1/B3/B4", testUnityAction);
            menuBarSystem.AddMenuItem("B1/B3/B5", testUnityAction);
            menuBarSystem.AddMenuItem("B1/B6", testUnityAction);
            CheckEventCounter(5);

            //asserts
            Assert.AreEqual(2, menuBarState.menuItems.Count);

            MenuBarState.MenuBarItem a1 = menuBarState.menuItems[0];
            Assert.AreEqual("A1", a1.title);
            ContextSubmenuItem a2 = a1.subItems[0] as ContextSubmenuItem;
            Assert.AreEqual("A2", a2.title);
            ContextMenuTextItem a3 = a2.items[0] as ContextMenuTextItem;
            Assert.AreEqual("A3", a3.title);

            MenuBarState.MenuBarItem b1 = menuBarState.menuItems[1];
            Assert.AreEqual("B1", b1.title);
            ContextMenuTextItem b2 = b1.subItems[0] as ContextMenuTextItem;
            Assert.AreEqual("B2", b2.title);
            ContextSubmenuItem b3 = b1.subItems[1] as ContextSubmenuItem;
            Assert.AreEqual("B3", b3.title);
            ContextMenuTextItem b4 = b3.items[0] as ContextMenuTextItem;
            Assert.AreEqual("B4", b4.title);
            ContextMenuTextItem b5 = b3.items[1] as ContextMenuTextItem;
            Assert.AreEqual("B5", b5.title);
            ContextMenuTextItem b6 = b1.subItems[2] as ContextMenuTextItem;
            Assert.AreEqual("B6", b6.title);
        }

        //utility
        private void TestUnityAction() { }

        private void CatchEvent()
        {
            eventCounter++;
        }

        private void CheckEventCounter(int count)
        {
            Assert.AreEqual(eventCounter, count);
            eventCounter = 0;
        }
    }
}
