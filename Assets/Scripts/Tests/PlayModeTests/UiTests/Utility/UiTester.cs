using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.EditorState;
using Assets.Scripts.Visuals.UiBuilder;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Tests.PlayModeTests.UiTests.Utility
{
    public class UiTester : MonoBehaviour
    {
        [SerializeField]
        private GameObject leftPanelGameObject;

        [SerializeField]
        private GameObject centerPanelGameObject;

        [SerializeField]
        private UnityState unityState;

        [SerializeField]
        private UiTesterPrompt uiTesterPromptInternal;

        [SerializeField]
        private Image background;

        [SerializeField]
        private TextMeshProUGUI testTitleText;

        public UiTesterPrompt uiTesterPrompt => uiTesterPromptInternal;

        // Singleton
        public static UiTester instance { get; private set; }

        private void Start()
        {
            instance = this;
        }

        public enum PanelToUse
        {
            Left,
            Center,
        }

        public void UpdateTestName()
        {
            var test = TestContext.CurrentContext.Test;
            testTitleText.text = test.Name;
        }

        public IEnumerator ResultFeedback()
        {
            var isSuccessful = TestContext.CurrentContext.Result.Outcome.Status == TestStatus.Passed;
            yield return ShowFeedback(isSuccessful);
        }

        public IEnumerator PositiveFeedback()
        {
            yield return ShowFeedback(true);
        }

        public IEnumerator NegativeFeedback()
        {
            yield return ShowFeedback(false);
        }

        private IEnumerator ShowFeedback(bool isPositive)
        {
            var oldColor = background.color;
            background.color = isPositive ? Color.green : Color.red;
            yield return new WaitForSeconds(0.1f);
            background.color = oldColor;
        }

        public UiBuilder MakeNewUiBuilder(PanelToUse panelToUse = PanelToUse.Left)
        {
            var go = new GameObject("UiBuilder", typeof(RectTransform));

            leftPanelGameObject.SetActive(false);
            centerPanelGameObject.SetActive(false);

            switch (panelToUse)
            {
                case PanelToUse.Left:
                    go.transform.SetParent(leftPanelGameObject.transform);
                    leftPanelGameObject.SetActive(true);
                    break;
                case PanelToUse.Center:
                    go.transform.SetParent(centerPanelGameObject.transform);
                    centerPanelGameObject.SetActive(true);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(panelToUse), panelToUse, null);
            }


            var rectTransform = go.GetComponent<RectTransform>();

            rectTransform.localScale = Vector3.one;

            rectTransform.anchorMin = new Vector2(0, 1);
            rectTransform.anchorMax = new Vector2(1, 1);

            rectTransform.pivot = new Vector2(0.5f, 1);

            rectTransform.offsetMin = Vector2.zero;
            rectTransform.offsetMax = Vector2.zero;

            var uiBuilder = new UiBuilder(go);
            uiBuilder.Constructor(unityState);

            return uiBuilder;
        }

        public void DisposeUiBuilder(UiBuilder uiBuilder)
        {
            Destroy(uiBuilder.parentObject);
        }
    }
}
