using System;
using UnityEngine;

public enum WizardState
{
    Intro,
    Step,
    Feedback,
    Result
}

public class WizardController : MonoBehaviour
{
    private const string ResultPrefsKey = "Verst.LastWizardResult";

    [SerializeField] private WizardDefinition definition;
    [SerializeField] private ProfileStore profileStore;

    public event Action Changed;

    public WizardDefinition Definition => definition;
    public WizardState State { get; private set; } = WizardState.Intro;
    public int StepIndex { get; private set; }
    public WizardStep CurrentStep => definition.Steps[StepIndex];
    public bool LastAnswerCorrect { get; private set; }
    public WizardResult CurrentResult { get; private set; }
    public WizardResult LastSavedResult { get; private set; }
    public bool CanBegin => profileStore.Current.HasValidOperatorId();
    public bool ShowHints => profileStore.Current.showWizardHints;
    public string OperatorId => profileStore.Current.operatorId;

    private float startTime;

    private void Awake()
    {
        LastSavedResult = LoadResult();
    }

    public void Begin()
    {
        if (State != WizardState.Intro || !CanBegin)
        {
            return;
        }

        CurrentResult = new WizardResult
        {
            operatorId = OperatorId,
            wizardId = definition.Id,
            startedAt = DateTime.UtcNow.ToString("o"),
            questions = CountQuestions()
        };
        startTime = Time.time;
        StepIndex = 0;
        GoTo(WizardState.Step);
    }

    public void Next()
    {
        if (State != WizardState.Step || CurrentStep.Type != WizardStepType.Info)
        {
            return;
        }
        Advance();
    }

    public void Answer(int option)
    {
        if (State != WizardState.Step || CurrentStep.Type != WizardStepType.SingleChoice)
        {
            return;
        }

        LastAnswerCorrect = option == CurrentStep.CorrectOption;
        CurrentResult.answers.Add(new WizardAnswer { step = StepIndex, selected = option, correct = LastAnswerCorrect });
        if (LastAnswerCorrect)
        {
            CurrentResult.correct++;
        }
        else
        {
            CurrentResult.errors++;
        }
        GoTo(WizardState.Feedback);
    }

    public void Continue()
    {
        if (State != WizardState.Feedback)
        {
            return;
        }
        Advance();
    }

    public void Restart()
    {
        if (State != WizardState.Result)
        {
            return;
        }
        GoTo(WizardState.Intro);
    }

    private void Advance()
    {
        StepIndex++;
        if (StepIndex < definition.Steps.Count)
        {
            GoTo(WizardState.Step);
        }
        else
        {
            Finish();
        }
    }

    private void Finish()
    {
        CurrentResult.finishedAt = DateTime.UtcNow.ToString("o");
        CurrentResult.durationSec = Mathf.Round((Time.time - startTime) * 10f) / 10f;
        CurrentResult.score = CurrentResult.questions > 0 ? (float)CurrentResult.correct / CurrentResult.questions : 1f;
        CurrentResult.passed = CurrentResult.score >= definition.PassScore;

        string json = JsonUtility.ToJson(CurrentResult);
        PlayerPrefs.SetString(ResultPrefsKey, json);
        PlayerPrefs.Save();
        LastSavedResult = CurrentResult;
        Debug.Log($"Wizard result: {json}");

        GoTo(WizardState.Result);
    }

    private void GoTo(WizardState state)
    {
        State = state;
        Changed?.Invoke();
    }

    private int CountQuestions()
    {
        int count = 0;
        foreach (WizardStep step in definition.Steps)
        {
            if (step.Type == WizardStepType.SingleChoice)
            {
                count++;
            }
        }
        return count;
    }

    private static WizardResult LoadResult()
    {
        string json = PlayerPrefs.GetString(ResultPrefsKey, "");
        if (string.IsNullOrEmpty(json))
        {
            return null;
        }

        try
        {
            return JsonUtility.FromJson<WizardResult>(json);
        }
        catch (ArgumentException)
        {
            return null;
        }
    }
}
