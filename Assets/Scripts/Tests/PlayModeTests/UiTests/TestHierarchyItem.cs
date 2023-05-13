using System.Collections;
using Assets.Scripts.Tests.PlayModeTests.UiTests.Utility;
using Assets.Scripts.Visuals.UiBuilder;
using Assets.Scripts.Visuals.UiHandler;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using static UiTesterPrompt.Answer;

namespace Assets.Scripts.Tests.PlayModeTests.UiTests
{
    public class TestHierarchyItem
    {
        private bool sceneIsLoaded = false;
        private UiBuilder uiBuilder;

        [UnitySetUp]
        public IEnumerator SetUp()
        {
            if (!sceneIsLoaded)
            {
                SceneManager.LoadScene("UiTestScene", LoadSceneMode.Single);
                sceneIsLoaded = true;
                yield return null;
            }

            UiTester.instance.UpdateTestName();

            uiBuilder = UiTester.instance.MakeNewUiBuilder();
        }

        [UnityTearDown]
        public IEnumerator TearDown()
        {
            yield return UiTester.instance.ResultFeedback();

            UiTester.instance.DisposeUiBuilder(uiBuilder);
        }


        [UnityTest]
        public IEnumerator ShowHierarchyItem()
        {
            var testerPrompt = UiTester.instance.uiTesterPrompt;
            var mainPanel = UiBuilder.NewPanelData();

            mainPanel.AddHierarchyItem("This is some header", 0, true, true, false, TextHandler.TextStyle.Normal, false);
            mainPanel.AddHierarchyItem("This is some more header", 1, true, false, false, TextHandler.TextStyle.Normal, false);
            mainPanel.AddHierarchyItem("This is even more header", 1, false, true, false, TextHandler.TextStyle.Normal, false);

            uiBuilder.Update(mainPanel);

            yield return testerPrompt.WaitForQuestionPrompt("Do you see three hierarchy items under each other?", Yes);
        }

        [UnityTest]
        public IEnumerator Levels()
        {
            var testerPrompt = UiTester.instance.uiTesterPrompt;
            var mainPanel = UiBuilder.NewPanelData();

            mainPanel.AddHierarchyItem("Level 0", 0, false, false, false, TextHandler.TextStyle.Normal, false);
            mainPanel.AddHierarchyItem("Level 0", 0, true, false, false, TextHandler.TextStyle.Normal, false);
            mainPanel.AddHierarchyItem("Level 1", 1, false, false, false, TextHandler.TextStyle.Normal, false);
            mainPanel.AddHierarchyItem("Level 1", 1, true, false, false, TextHandler.TextStyle.Normal, false);
            mainPanel.AddHierarchyItem("Level 2", 2, false, false, false, TextHandler.TextStyle.Normal, false);
            mainPanel.AddHierarchyItem("Level 2", 2, true, false, false, TextHandler.TextStyle.Normal, false);
            mainPanel.AddHierarchyItem("Level 3", 3, false, false, false, TextHandler.TextStyle.Normal, false);
            mainPanel.AddHierarchyItem("Level 3", 3, true, false, false, TextHandler.TextStyle.Normal, false);
            mainPanel.AddHierarchyItem("Level 4", 4, false, false, false, TextHandler.TextStyle.Normal, false);
            mainPanel.AddHierarchyItem("Level 4", 4, true, false, false, TextHandler.TextStyle.Normal, false);
            mainPanel.AddHierarchyItem("Level 5", 5, false, false, false, TextHandler.TextStyle.Normal, false);
            mainPanel.AddHierarchyItem("Level 5", 5, true, false, false, TextHandler.TextStyle.Normal, false);
            mainPanel.AddHierarchyItem("Level 6", 6, false, false, false, TextHandler.TextStyle.Normal, false);
            mainPanel.AddHierarchyItem("Level 6", 6, true, false, false, TextHandler.TextStyle.Normal, false);
            mainPanel.AddHierarchyItem("Level 7", 7, false, false, false, TextHandler.TextStyle.Normal, false);
            mainPanel.AddHierarchyItem("Level 7", 7, true, false, false, TextHandler.TextStyle.Normal, false);

            uiBuilder.Update(mainPanel);

            yield return testerPrompt.WaitForQuestionPrompt("Do the indents look correct for the specified level?", Yes);
        }

        [UnityTest]
        public IEnumerator ExpandArrow()
        {
            var testerPrompt = UiTester.instance.uiTesterPrompt;
            var mainPanel = UiBuilder.NewPanelData();

            mainPanel.AddHierarchyItem("Item", 0, false, false, false, TextHandler.TextStyle.Normal, false);
            mainPanel.AddHierarchyItem("Item", 1, false, true, false, TextHandler.TextStyle.Normal, false);
            mainPanel.AddHierarchyItem("Item", 2, false, false, false, TextHandler.TextStyle.Normal, false);

            uiBuilder.Update(mainPanel);

            yield return testerPrompt.WaitForQuestionPrompt("Does the expand arrow show up for any item?", No);
            yield return UiTester.instance.PositiveFeedback();

            mainPanel.childDates.Clear();

            mainPanel.AddHierarchyItem("Item", 0, true, false, false, TextHandler.TextStyle.Normal, false);
            mainPanel.AddHierarchyItem("Item", 1, true, false, false, TextHandler.TextStyle.Normal, false);
            mainPanel.AddHierarchyItem("Item", 2, true, false, false, TextHandler.TextStyle.Normal, false);

            uiBuilder.Update(mainPanel);

            yield return testerPrompt.WaitForQuestionPrompt("Does the expand arrow show up for all items?", Yes);
            yield return UiTester.instance.PositiveFeedback();
            yield return testerPrompt.WaitForQuestionPrompt("Does the expand arrow point to the right for all items?", Yes);
            yield return UiTester.instance.PositiveFeedback();


            mainPanel.childDates.Clear();

            mainPanel.AddHierarchyItem("Item", 0, true, true, false, TextHandler.TextStyle.Normal, false);
            mainPanel.AddHierarchyItem("Item", 1, true, true, false, TextHandler.TextStyle.Normal, false);
            mainPanel.AddHierarchyItem("Item", 2, true, true, false, TextHandler.TextStyle.Normal, false);

            uiBuilder.Update(mainPanel);

            yield return testerPrompt.WaitForQuestionPrompt("Does the expand arrow show up for all items?", Yes);
            yield return UiTester.instance.PositiveFeedback();
            yield return testerPrompt.WaitForQuestionPrompt("Does the expand arrow point to the right for any item?", No);
        }

        [UnityTest]
        public IEnumerator ClickActions()
        {
            var testerPrompt = UiTester.instance.uiTesterPrompt;

            {
                var clickChecker = new UiTesterPrompt.CheckCompletionByEvent();
                var mainPanel = UiBuilder.NewPanelData();

                mainPanel.AddHierarchyItem(
                    "<- Click here",
                    0,
                    true,
                    false,
                    false,
                    TextHandler.TextStyle.Normal,
                    false,
                    clickArrowStrategy: new LeftClickStrategy {onLeftClick = _ => clickChecker.Success()},
                    clickTextStrategy: new LeftClickStrategy {onLeftClick = _ => clickChecker.Fail("The user clicked on the text")},
                    rightClickStrategy: new RightClickStrategy {onRightClick = _ => clickChecker.Fail("The user right clicked")});

                uiBuilder.Update(mainPanel);

                yield return testerPrompt.WaitForTaskPrompt("Click on the marked arrow", clickChecker);
                yield return UiTester.instance.PositiveFeedback();
            }

            {
                var clickChecker = new UiTesterPrompt.CheckCompletionByEvent();
                var mainPanel = UiBuilder.NewPanelData();

                mainPanel.AddHierarchyItem(
                    "-> Click here <-",
                    0,
                    true,
                    false,
                    false,
                    TextHandler.TextStyle.Normal,
                    false,
                    clickArrowStrategy: new LeftClickStrategy {onLeftClick = _ => clickChecker.Fail("The user clicked on the arrow")},
                    clickTextStrategy: new LeftClickStrategy {onLeftClick = _ => clickChecker.Success()},
                    rightClickStrategy: new RightClickStrategy {onRightClick = _ => clickChecker.Fail("The user right clicked")});


                uiBuilder.Update(mainPanel);

                yield return testerPrompt.WaitForTaskPrompt("Click on the marked item", clickChecker);
                yield return UiTester.instance.PositiveFeedback();
            }

            {
                var clickChecker = new UiTesterPrompt.CheckCompletionByEvent();
                var mainPanel = UiBuilder.NewPanelData();

                mainPanel.AddHierarchyItem(
                    "-> Right click here <-",
                    0,
                    true,
                    false,
                    false,
                    TextHandler.TextStyle.Normal,
                    false,
                    clickArrowStrategy: new LeftClickStrategy {onLeftClick = _ => clickChecker.Fail("The user clicked on the arrow")},
                    clickTextStrategy: new LeftClickStrategy {onLeftClick = _ => clickChecker.Fail("The user clicked on the text")},
                    rightClickStrategy: new RightClickStrategy {onRightClick = _ => clickChecker.Success()});


                uiBuilder.Update(mainPanel);

                yield return testerPrompt.WaitForTaskPrompt("Right click on the marked item", clickChecker);
                yield return UiTester.instance.PositiveFeedback();
            }

            {
                var mainPanel = UiBuilder.NewPanelData();

                mainPanel.AddHierarchyItem(
                    "-> Right click here <-",
                    0,
                    true,
                    false,
                    false,
                    TextHandler.TextStyle.Normal,
                    false);

                uiBuilder.Update(mainPanel);

                yield return testerPrompt.WaitForQuestionPrompt("Try to click the arrow, the text, and right click the text. Did anything happen?", No);
                //yield return UiTester.instance.PositiveFeedback();
            }
        }
    }
}
