using System;

[Serializable]
public class OperatorProfile
{
    public const int OperatorIdLength = 4;

    public string operatorId = "";
    public bool showInfoCards = true;
    public bool showWizardHints = true;
    public float tabletDistance = 0.45f;

    public OperatorProfile Clone()
    {
        return (OperatorProfile)MemberwiseClone();
    }

    public bool SameAs(OperatorProfile other)
    {
        return other != null
            && operatorId == other.operatorId
            && showInfoCards == other.showInfoCards
            && showWizardHints == other.showWizardHints
            && Math.Abs(tabletDistance - other.tabletDistance) < 0.001f;
    }

    public bool HasValidOperatorId()
    {
        if (operatorId == null || operatorId.Length != OperatorIdLength)
        {
            return false;
        }

        foreach (char c in operatorId)
        {
            if (!char.IsDigit(c))
            {
                return false;
            }
        }
        return true;
    }
}
