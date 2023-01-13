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
        public void TestMenuBarTreeA()
        {
            //construct system
            MenuBarSystem menuBarSystem = new MenuBarSystem();
            EditorEvents editorEvents = new EditorEvents();
            MenuBarState menuBarState = new MenuBarState();
            menuBarSystem.Construct(editorEvents, menuBarState);

            //prepare
            eventCounter = 0;
            editorEvents.onUpdateMenuBarEvent += CatchEvent;
            UnityAction testUnityAction = new UnityAction(TestUnityAction);
            MenuBarState.MenuBarItem menuBarItem;
            ContextMenuItem contextMenuItem;

            //add menu item
            menuBarSystem.AddMenuItem("A1/A2/A3", testUnityAction, -1);
            CheckEventCounter(1);

            //asserts
            Assert.AreEqual(menuBarState.menuItems.Count, 1);
            menuBarItem = menuBarState.menuItems[0];
            Assert.AreEqual(menuBarItem.title, "A1");

            Assert.AreEqual(menuBarItem.subItems.Count, 1);
            contextMenuItem = menuBarItem.subItems[0];
            Assert.AreEqual((contextMenuItem as ContextSubmenuItem).title, "A2");

            Assert.AreEqual((contextMenuItem as ContextSubmenuItem).items.Count, 1);
            contextMenuItem = (contextMenuItem as ContextSubmenuItem).items[0];
            Assert.AreEqual((contextMenuItem as ContextMenuTextItem).title, "A3");
            Assert.AreSame((contextMenuItem as ContextMenuTextItem).onClick, testUnityAction);

            //add menu item
            menuBarSystem.AddMenuItem("A1/A2/A4", testUnityAction, -1);
            CheckEventCounter(1);

            //asserts
            Assert.AreEqual(menuBarState.menuItems.Count, 1);
            menuBarItem = menuBarState.menuItems[0];
            Assert.AreEqual(menuBarItem.title, "A1");

            Assert.AreEqual(menuBarItem.subItems.Count, 1);
            contextMenuItem = menuBarItem.subItems[0];
            Assert.AreEqual((contextMenuItem as ContextSubmenuItem).title, "A2");

            Assert.AreEqual((contextMenuItem as ContextSubmenuItem).items.Count, 2);
            contextMenuItem = (contextMenuItem as ContextSubmenuItem).items[1];
            Assert.AreEqual((contextMenuItem as ContextMenuTextItem).title, "A4");
            Assert.AreSame((contextMenuItem as ContextMenuTextItem).onClick, testUnityAction);

            //add menu item
            menuBarSystem.AddMenuItem("A1/A2/A5", testUnityAction, 1);
            CheckEventCounter(1);

            //asserts
            Assert.AreEqual(menuBarState.menuItems.Count, 1);
            menuBarItem = menuBarState.menuItems[0];
            Assert.AreEqual(menuBarItem.title, "A1");

            Assert.AreEqual(menuBarItem.subItems.Count, 1);
            contextMenuItem = menuBarItem.subItems[0];
            Assert.AreEqual((contextMenuItem as ContextSubmenuItem).title, "A2");

            Assert.AreEqual((contextMenuItem as ContextSubmenuItem).items.Count, 3);
            contextMenuItem = (contextMenuItem as ContextSubmenuItem).items[1];
            Assert.AreEqual((contextMenuItem as ContextMenuTextItem).title, "A5");
            Assert.AreSame((contextMenuItem as ContextMenuTextItem).onClick, testUnityAction);

            //add menu item
            menuBarSystem.AddMenuItem("A1/A6/A7", testUnityAction, 1);
            CheckEventCounter(1);

            //asserts
            Assert.AreEqual(menuBarState.menuItems.Count, 1);
            menuBarItem = menuBarState.menuItems[0];
            Assert.AreEqual(menuBarItem.title, "A1");

            Assert.AreEqual(menuBarItem.subItems.Count, 2);
            contextMenuItem = menuBarItem.subItems[1];
            Assert.AreEqual((contextMenuItem as ContextSubmenuItem).title, "A6");

            Assert.AreEqual((contextMenuItem as ContextSubmenuItem).items.Count, 1);
            contextMenuItem = (contextMenuItem as ContextSubmenuItem).items[0];
            Assert.AreEqual((contextMenuItem as ContextMenuTextItem).title, "A7");
            Assert.AreSame((contextMenuItem as ContextMenuTextItem).onClick, testUnityAction);
        }

        [Test]
        public void TestMenuBarTreeB()
        {
            //construct system
            MenuBarSystem menuBarSystem = new MenuBarSystem();
            EditorEvents editorEvents = new EditorEvents();
            MenuBarState menuBarState = new MenuBarState();
            menuBarSystem.Construct(editorEvents, menuBarState);

            //prepare
            eventCounter = 0;
            editorEvents.onUpdateMenuBarEvent += CatchEvent;
            UnityAction testUnityAction = new UnityAction(TestUnityAction);

            //add menu items
            menuBarSystem.AddMenuItem("A1/A2/A3", testUnityAction, -1);
            menuBarSystem.AddMenuItem("B1/B2", testUnityAction);
            menuBarSystem.AddMenuItem("B1/B3/B4", testUnityAction);
            menuBarSystem.AddMenuItem("B1/B3/B5", testUnityAction);
            menuBarSystem.AddMenuItem("B1/B6", testUnityAction);
            CheckEventCounter(5);

            //asserts
            Assert.AreEqual(menuBarState.menuItems.Count, 2);
            MenuBarState.MenuBarItem b1 = menuBarState.menuItems[1];
            Assert.AreEqual(b1.title, "B1");

            Assert.AreEqual(b1.subItems.Count, 3);
            Assert.AreEqual((b1.subItems[0] as ContextMenuTextItem).title, "B2");
            Assert.AreEqual((b1.subItems[0] as ContextMenuTextItem).onClick, testUnityAction);
            Assert.AreEqual((b1.subItems[1] as ContextSubmenuItem).title, "B3");
            Assert.AreEqual((b1.subItems[2] as ContextMenuTextItem).title, "B6");
            Assert.AreEqual((b1.subItems[2] as ContextMenuTextItem).onClick, testUnityAction);

            ContextSubmenuItem b3 = b1.subItems[1] as ContextSubmenuItem;
            Assert.AreEqual(b3.items.Count, 2);
            Assert.AreEqual((b3.items[0] as ContextMenuTextItem).title, "B4");
            Assert.AreEqual((b3.items[0] as ContextMenuTextItem).onClick, testUnityAction);
            Assert.AreEqual((b3.items[1] as ContextMenuTextItem).title, "B5");
            Assert.AreEqual((b3.items[1] as ContextMenuTextItem).onClick, testUnityAction);
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
