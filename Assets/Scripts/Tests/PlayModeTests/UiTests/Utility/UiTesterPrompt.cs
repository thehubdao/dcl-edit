using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UiTesterPrompt : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI Text;

    [SerializeField]
    private GameObject buttonsParent;

    [SerializeField]
    private Button yesButton;

    [SerializeField]
    private Button noButton;

    private bool wasYesPressed;
    private bool wasNoPressed;

    private bool wasAnyPressed => wasYesPressed || wasNoPressed;


    void Start()
    {
        yesButton.onClick.AddListener(() => wasYesPressed = true);
        noButton.onClick.AddListener(() => wasNoPressed = true);
    }

    private void ResetWasPressed()
    {
        wasYesPressed = false;
        wasNoPressed = false;
    }

    public void SetPromptText(string text)
    {
        Text.text = text;
    }

    private void SetButtonsActive(bool active)
    {
        buttonsParent.SetActive(active);
    }

    public enum Answer
    {
        Yes,
        No
    }

    public IEnumerator WaitForQuestionPrompt(string question, Answer expectedAnswer)
    {
        // Setup question
        SetPromptText(question);
        SetButtonsActive(true);

        var actualAnswer = Answer.No;

        // Wait for press
        while (true)
        {
            if (wasYesPressed || Input.GetKeyDown(KeyCode.Y))
            {
                actualAnswer = Answer.Yes;
                break;
            }

            if (wasNoPressed || Input.GetKeyUp(KeyCode.N))
            {
                actualAnswer = Answer.No;
                break;
            }

            yield return null;
        }

        // Set answer and reset
        ResetWasPressed();
        SetPromptText("");
        SetButtonsActive(false);

        if (actualAnswer != expectedAnswer)
        {
            Assert.Fail($"For the Question \"{question}\" the answer was {actualAnswer} but {expectedAnswer} was expected");
        }

        yield return null;
    }

    public struct Completion
    {
        public bool isCompleted;

        public bool isCorrect;

        [CanBeNull]
        public string errorMessage;
    }

    public IEnumerator WaitForTaskPrompt(string task, ICompletionChecker checker)
    {
        yield return WaitForTaskPrompt(task, checker.CheckCompletion);
    }

    public IEnumerator WaitForTaskPrompt(string task, Func<Completion> checkCompletion)
    {
        // Setup question
        SetPromptText(task);
        SetButtonsActive(false);

        // Wait for press
        while (true)
        {
            var c = checkCompletion();
            if (c.isCompleted)
            {
                // Reset
                SetPromptText("");

                if (!c.isCorrect)
                {
                    Assert.Fail($"The Task \"{task}\" was not correctly completed. " + (c.errorMessage ?? ""));
                }

                yield return null;
                yield break;
            }

            yield return null;
        }
    }

    public interface ICompletionChecker
    {
        public Completion CheckCompletion();
    }

    public class CheckCompletionByEvent : ICompletionChecker
    {
        private bool isCompleted = false;
        private bool isCorrect = false;
        private string errorMessage = null;

        public Completion CheckCompletion()
        {
            return new Completion
            {
                isCompleted = isCompleted,
                isCorrect = isCorrect,
                errorMessage = errorMessage
            };
        }

        public void Complete(bool correct, string message = null)
        {
            isCompleted = true;
            isCorrect = correct;
            errorMessage = message;
        }

        public void Success()
        {
            Complete(true);
        }

        public void Fail(string message = null)
        {
            Complete(false, message);
        }

        public void Reset()
        {
            isCompleted = false;
            isCorrect = false;
            errorMessage = null;
        }
    }
}
