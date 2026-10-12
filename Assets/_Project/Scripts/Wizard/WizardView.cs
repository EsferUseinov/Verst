using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WizardView : MonoBehaviour
{
    [SerializeField] private WizardController controller;

    [Header("Intro")]
    [SerializeField] private GameObject introScreen;
    [SerializeField] private TMP_Text introTitle;
    [SerializeField] private TMP_Text introText;
    [SerializeField] private TMP_Text introStatus;
    [SerializeField] private Button startButton;

    [Header("Step")]
    [SerializeField] private GameObject stepScreen;
    [SerializeField] private TMP_Text progressText;
    [SerializeField] private TMP_Text stepTitle;
    [SerializeField] private TMP_Text stepText;
    [SerializeField] private Button[] optionButtons;
    [SerializeField] private Button nextButton;

    [Header("Feedback")]
    [SerializeField] private GameObject feedbackScreen;
    [SerializeField] private TMP_Text verdictText;
    [SerializeField] private TMP_Text explanationText;
    [SerializeField] private Button continueButton;

    [Header("Result")]
    [SerializeField] private GameObject resultScreen;
    [SerializeField] private TMP_Text resultText;
    [SerializeField] private Button restartButton;

    [Header("Colors")]
    [SerializeField] private Color goodColor = new Color(0.45f, 0.85f, 0.45f);
    [SerializeField] private Color badColor = new Color(0.9f, 0.3f, 0.25f);
    [SerializeField] private Color hintColor = new Color(0.725f, 0.65f, 0.91f);

    private void Awake()
    {
        startButton.onClick.AddListener(controller.Begin);
        nextButton.onClick.AddListener(controller.Next);
        continueButton.onClick.AddListener(controller.Continue);
        restartButton.onClick.AddListener(controller.Restart);
        for (int i = 0; i < optionButtons.Length; i++)
        {
            int index = i;
            optionButtons[i].onClick.AddListener(() => controller.Answer(index));
        }
    }

    private void OnEnable()
    {
        controller.Changed += Render;
        Render();
    }

    private void OnDisable()
    {
        controller.Changed -= Render;
    }

    private void Render()
    {
        WizardState state = controller.State;
        introScreen.SetActive(state == WizardState.Intro);
        stepScreen.SetActive(state == WizardState.Step);
        feedbackScreen.SetActive(state == WizardState.Feedback);
        resultScreen.SetActive(state == WizardState.Result);

        switch (state)
        {
            case WizardState.Intro:
                RenderIntro();
                break;
            case WizardState.Step:
                RenderStep();
                break;
            case WizardState.Feedback:
                RenderFeedback();
                break;
            case WizardState.Result:
                resultText.text = FormatResult(controller.CurrentResult);
                break;
        }
    }

    private void RenderIntro()
    {
        WizardDefinition definition = controller.Definition;
        introTitle.text = definition.DisplayName;
        introText.text = definition.Intro;

        WizardResult last = controller.LastSavedResult;
        if (last != null && last.wizardId == definition.Id)
        {
            introText.text += $"\n\nОстанній результат: {last.correct} з {last.checkedSteps}, {(last.passed ? "пройдено" : "не пройдено")}";
        }

        startButton.interactable = controller.CanBegin;
        introStatus.text = controller.CanBegin
            ? $"Оператор: {controller.OperatorId}"
            : "Вкажи й збережи ID оператора у вкладці «Профіль»";
    }

    private void RenderStep()
    {
        WizardStep step = controller.CurrentStep;
        progressText.text = $"Крок {controller.StepIndex + 1} з {controller.Definition.Steps.Count}";
        stepTitle.text = step.Title;
        stepText.text = step.Type == WizardStepType.Action ? FormatAction(step) : step.Text;

        bool isQuestion = step.Type == WizardStepType.SingleChoice;
        for (int i = 0; i < optionButtons.Length; i++)
        {
            bool visible = isQuestion && i < step.Options.Count;
            optionButtons[i].gameObject.SetActive(visible);
            if (visible)
            {
                optionButtons[i].GetComponentInChildren<TMP_Text>().text = step.Options[i];
            }
        }
        nextButton.gameObject.SetActive(step.Type == WizardStepType.Info);
    }

    private string FormatAction(WizardStep step)
    {
        string text = step.Text;
        if (controller.StepErrors > 0 && !string.IsNullOrEmpty(step.FailHint))
        {
            text += $"\n\n<color=#{ColorUtility.ToHtmlStringRGB(badColor)}>{step.FailHint}</color>";
        }
        return text + $"\n\n<color=#{ColorUtility.ToHtmlStringRGB(hintColor)}>Крок зарахується сам, щойно зробиш це в цеху.</color>";
    }

    private void RenderFeedback()
    {
        WizardStep step = controller.CurrentStep;
        bool correct = controller.LastAnswerCorrect;
        verdictText.text = correct ? "Правильно" : "Неправильно";
        verdictText.color = correct ? goodColor : badColor;

        if (!controller.ShowHints)
        {
            explanationText.text = "";
        }
        else if (correct)
        {
            explanationText.text = step.Explanation;
        }
        else
        {
            explanationText.text = $"Правильна відповідь: {step.Options[step.CorrectOption]}\n{step.Explanation}";
        }
    }

    private static string FormatResult(WizardResult result)
    {
        return $"Оператор: {result.operatorId}\n"
            + $"Кроків без помилок: {result.correct} з {result.checkedSteps}\n"
            + $"Помилок: {result.errors}\n"
            + $"Час: {result.durationSec:0.0} с\n"
            + $"Результат: {result.score * 100f:0}% – {(result.passed ? "пройдено" : "не пройдено")}";
    }
}
