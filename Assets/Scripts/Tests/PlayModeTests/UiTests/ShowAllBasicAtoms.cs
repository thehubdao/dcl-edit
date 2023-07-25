using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Tests.PlayModeTests.UiTests.Utility;
using Assets.Scripts.Visuals.UiBuilder;
using Assets.Scripts.Visuals.UiHandler;
using NUnit.Framework;
using NUnit.Framework.Constraints;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using static UiTesterPrompt.Answer;

namespace Assets.Scripts.Tests.PlayModeTests.UiTests
{
    public class ShowAllBasicAtoms
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
        public IEnumerator ShowComplexSetup()
        {
            var testerPrompt = UiTester.instance.uiTesterPrompt;

            var centerUiBuilder = UiTester.instance.MakeNewUiBuilder(UiTester.PanelToUse.Center);

            var mainPanel = UiBuilder.NewPanelData();

            mainPanel.AddTitle("A Title");
            mainPanel.AddText("This is a text");

            var firstPanel = mainPanel.AddPanelWithBorder();
            firstPanel.AddPanelHeader("First panel");
            firstPanel.AddVector3Property("Vector property", ("x", "y", "z"), new ValueBindStrategy<Vector3>(() => Vector3.zero));
            firstPanel.AddNumberProperty("Number property", "only takes numbers", new ValueBindStrategy<float>(() => 100));

            var horPanel = mainPanel.AddPanel(PanelHandler.LayoutDirection.Horizontal);

            var secondPanel = horPanel.AddPanelWithBorder();
            secondPanel.AddPanelHeader("Second panel", new ClickStrategy());
            secondPanel.AddStringProperty("String property", "write a string here", new ValueBindStrategy<string>(() => ""));
            secondPanel.AddBooleanProperty("Bool property", new ValueBindStrategy<bool>(() => true));

            var thirdPanel = horPanel.AddPanelWithBorder();
            thirdPanel.AddPanelHeader("Second panel", new ClickStrategy());
            thirdPanel.AddStringProperty("String property", "write a string here", new ValueBindStrategy<string>(() => ""));
            thirdPanel.AddBooleanProperty("Bool property", new ValueBindStrategy<bool>(() => true));

            mainPanel.AddSpacer(100);
            mainPanel.AddHierarchyItem("Hierarchy Item", 0, false, false, false, TextHandler.TextStyle.Normal, false);

            centerUiBuilder.Update(mainPanel);

            yield return testerPrompt.WaitForQuestionPrompt("Do you see the ui with the following setup? A title, a text," +
                                                            " a panel (with a header, a vector property, and a number property)," +
                                                            " another panel (with a header, a string property, and a bool property)," +
                                                            " then a space, and as last element a hierarchy item ", Yes);

            UiTester.instance.DisposeUiBuilder(centerUiBuilder);
        }

        [UnityTest]
        public IEnumerator ShowText()
        {
            var testerPrompt = UiTester.instance.uiTesterPrompt;
            var mainPanel = UiBuilder.NewPanelData();

            var centerTextValue = "This is some more text";
            var centerTextValueStrategy = new SetValueStrategy<string>(() => centerTextValue);


            mainPanel.AddText("This is some text");
            mainPanel.AddText(centerTextValueStrategy);
            mainPanel.AddText("This is even more text");

            uiBuilder.Update(mainPanel);

            yield return testerPrompt.WaitForQuestionPrompt("Do you see three texts under each other?", Yes);

            centerTextValue = "Some other text";
            uiBuilder.UpdateValues();

            yield return testerPrompt.WaitForQuestionPrompt("Did you see the center text change to \"Some other text\"?", Yes);
        }

        [UnityTest]
        public IEnumerator ShowTitle()
        {
            var testerPrompt = UiTester.instance.uiTesterPrompt;
            var mainPanel = UiBuilder.NewPanelData();

            mainPanel.AddTitle("This is some title");
            mainPanel.AddTitle("This is some more title");
            mainPanel.AddTitle("This is even more title");

            uiBuilder.Update(mainPanel);

            yield return testerPrompt.WaitForQuestionPrompt("Do you see three titles under each other?", Yes);
        }

        [UnityTest]
        public IEnumerator ShowButton()
        {
            var testerPrompt = UiTester.instance.uiTesterPrompt;
            var mainPanel = UiBuilder.NewPanelData();

            var completion = new UiTesterPrompt.CheckCompletionByEvent();

            mainPanel.AddButton("Don't press here", new LeftClickStrategy(_ => completion.Fail()));
            mainPanel.AddButton("Press here", new LeftClickStrategy(_ => completion.Success()));
            mainPanel.AddButton("Don't press here", new LeftClickStrategy(_ => completion.Fail()));

            uiBuilder.Update(mainPanel);

            yield return testerPrompt.WaitForTaskPrompt("Press the marked button", completion);
        }

        [UnityTest]
        public IEnumerator ShowStringProperty()
        {
            var testerPrompt = UiTester.instance.uiTesterPrompt;
            var mainPanel = UiBuilder.NewPanelData();

            mainPanel.AddStringProperty("This is some property", "text", new ValueBindStrategy<string>(() => "text"));
            mainPanel.AddStringProperty("This is some more property", "text", new ValueBindStrategy<string>(() => "text"));
            mainPanel.AddStringProperty("This is even more property", "text", new ValueBindStrategy<string>(() => "text"));

            uiBuilder.Update(mainPanel);

            yield return testerPrompt.WaitForQuestionPrompt("Do you see three string properties under each other?", Yes);
        }

        [UnityTest]
        public IEnumerator ShowNumberProperty()
        {
            var testerPrompt = UiTester.instance.uiTesterPrompt;
            var mainPanel = UiBuilder.NewPanelData();

            mainPanel.AddNumberProperty("This is some property", "text", new ValueBindStrategy<float>(() => 0f));
            mainPanel.AddNumberProperty("This is some more property", "text", new ValueBindStrategy<float>(() => 0f));
            mainPanel.AddNumberProperty("This is even more property", "text", new ValueBindStrategy<float>(() => 0f));

            uiBuilder.Update(mainPanel);

            yield return testerPrompt.WaitForQuestionPrompt("Do you see three number properties under each other?", Yes);
        }

        [UnityTest]
        public IEnumerator ShowBooleanProperty()
        {
            var testerPrompt = UiTester.instance.uiTesterPrompt;
            var mainPanel = UiBuilder.NewPanelData();

            mainPanel.AddBooleanProperty("This is some property", new ValueBindStrategy<bool>(() => false));
            mainPanel.AddBooleanProperty("This is some more property", new ValueBindStrategy<bool>(() => false));
            mainPanel.AddBooleanProperty("This is even more property", new ValueBindStrategy<bool>(() => false));

            uiBuilder.Update(mainPanel);

            yield return testerPrompt.WaitForQuestionPrompt("Do you see three boolean properties under each other?", Yes);
        }

        [UnityTest]
        public IEnumerator ShowVector3Property()
        {
            var testerPrompt = UiTester.instance.uiTesterPrompt;
            var mainPanel = UiBuilder.NewPanelData();

            mainPanel.AddVector3Property("This is some property", ("x", "y", "z"), new ValueBindStrategy<Vector3>(() => Vector3.zero));
            mainPanel.AddVector3Property("This is some more property", ("x", "y", "z"), new ValueBindStrategy<Vector3>(() => Vector3.zero));
            mainPanel.AddVector3Property("This is even more property", ("x", "y", "z"), new ValueBindStrategy<Vector3>(() => Vector3.zero));

            uiBuilder.Update(mainPanel);

            yield return testerPrompt.WaitForQuestionPrompt("Do you see three vector 3 properties under each other?", Yes);
        }

        [UnityTest]
        public IEnumerator ShowPanelHeader()
        {
            var testerPrompt = UiTester.instance.uiTesterPrompt;
            var mainPanel = UiBuilder.NewPanelData();

            mainPanel.AddPanelHeader("This is some header");
            mainPanel.AddPanelHeader("This is some more header");
            mainPanel.AddPanelHeader("This is even more header");

            uiBuilder.Update(mainPanel);

            yield return testerPrompt.WaitForQuestionPrompt("Do you see three panel headers under each other?", Yes);
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
        public IEnumerator ShowPromptSystemDialog()
        {
            var testerPrompt = UiTester.instance.uiTesterPrompt;
            var mainPanel = UiBuilder.NewPanelData();
            var buttonCommands = new PromptSystem.Action[] { new PromptSystem.Yes(() => Debug.Log("Yes")), new PromptSystem.No(() => Debug.Log("No")) };
            var notInWindowCommand = new PromptSystem.Cancel(() => Debug.Log("Cancel"));
            UiPromptVisuals.Data data = new(null, "Test the Prompt System", buttonCommands, notInWindowCommand);
            var promptPanel = mainPanel.AddPanel();
            UiPromptVisuals.AssignActions(data);
            UiPromptVisuals.CreateText(promptPanel, data);
            UiPromptVisuals.AddButtons(promptPanel, data);
            uiBuilder.Update(mainPanel);

            string promptText = "Click the same anser in the prompt system and here to success?";
            testerPrompt.SetPromptText(promptText);

            yield return new WaitUntil(() => data.taskCompleted.Task.IsCompleted);
            UiTesterPrompt.Answer correctAnswer = data.taskCompleted.Task.Result.name == "Yes" ? Yes : No;

            yield return testerPrompt.WaitForQuestionPrompt(promptText, correctAnswer);
        }
    }
}