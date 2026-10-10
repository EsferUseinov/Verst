using System;
using System.Collections.Generic;

[Serializable]
public class WizardAnswer
{
    public int step;
    public int selected;
    public bool correct;
}

[Serializable]
public class WizardResult
{
    public string operatorId;
    public string wizardId;
    public string startedAt;
    public string finishedAt;
    public float durationSec;
    public int questions;
    public int correct;
    public int errors;
    public float score;
    public bool passed;
    public List<WizardAnswer> answers = new List<WizardAnswer>();
}
