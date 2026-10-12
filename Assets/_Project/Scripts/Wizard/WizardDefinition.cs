using System;
using System.Collections.Generic;
using UnityEngine;

public enum WizardStepType
{
    Info,
    SingleChoice,
    Action
}

[Serializable]
public class WizardStep
{
    [SerializeField] private WizardStepType type;
    [SerializeField] private string title;
    [SerializeField, TextArea] private string text;
    [SerializeField] private string[] options = new string[0];
    [SerializeField] private int correctOption;
    [SerializeField, TextArea] private string explanation;
    [SerializeField] private string expectedEvent;
    [SerializeField] private string failEvent;
    [SerializeField, TextArea] private string failHint;
    [SerializeField] private string markerId;

    public WizardStepType Type => type;
    public string Title => title;
    public string Text => text;
    public IReadOnlyList<string> Options => options;
    public int CorrectOption => correctOption;
    public string Explanation => explanation;
    public string ExpectedEvent => expectedEvent;
    public string FailEvent => failEvent;
    public string FailHint => failHint;
    public string MarkerId => markerId;
}

[CreateAssetMenu(fileName = "Wizard_", menuName = "Verst/Wizard Definition")]
public class WizardDefinition : ScriptableObject
{
    [SerializeField] private string id;
    [SerializeField] private string displayName;
    [SerializeField, TextArea] private string intro;
    [SerializeField, Range(0f, 1f)] private float passScore = 0.8f;
    [SerializeField] private List<WizardStep> steps = new List<WizardStep>();

    public string Id => id;
    public string DisplayName => displayName;
    public string Intro => intro;
    public float PassScore => passScore;
    public IReadOnlyList<WizardStep> Steps => steps;
}
