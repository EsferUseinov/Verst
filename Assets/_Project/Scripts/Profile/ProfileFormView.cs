using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ProfileFormView : MonoBehaviour
{
    [SerializeField] private ProfileStore store;
    [SerializeField] private TMP_InputField operatorIdField;
    [SerializeField] private Toggle infoCardsToggle;
    [SerializeField] private Toggle wizardHintsToggle;
    [SerializeField] private Slider distanceSlider;
    [SerializeField] private TMP_Text distanceLabel;
    [SerializeField] private Button saveButton;
    [SerializeField] private Button resetButton;
    [SerializeField] private TMP_Text statusText;
    [SerializeField] private Color okColor = new Color(0.45f, 0.85f, 0.45f);
    [SerializeField] private Color pendingColor = new Color(0.95f, 0.55f, 0.16f);
    [SerializeField] private Color errorColor = new Color(0.9f, 0.3f, 0.25f);

    private OperatorProfile draft;

    private void Awake()
    {
        operatorIdField.characterLimit = OperatorProfile.OperatorIdLength;
        operatorIdField.characterValidation = TMP_InputField.CharacterValidation.Digit;

        operatorIdField.onValueChanged.AddListener(value => { draft.operatorId = value; RefreshStatus(); });
        infoCardsToggle.onValueChanged.AddListener(value => { draft.showInfoCards = value; RefreshStatus(); });
        wizardHintsToggle.onValueChanged.AddListener(value => { draft.showWizardHints = value; RefreshStatus(); });
        distanceSlider.onValueChanged.AddListener(value => { draft.tabletDistance = value; UpdateDistanceLabel(); RefreshStatus(); });
        saveButton.onClick.AddListener(Save);
        resetButton.onClick.AddListener(ResetToDefaults);
    }

    private void OnEnable()
    {
        ShowProfile(store.Current);
        RefreshStatus();
    }

    private void Save()
    {
        if (!draft.HasValidOperatorId())
        {
            SetStatus($"ID оператора – {OperatorProfile.OperatorIdLength} цифри", errorColor);
            return;
        }

        store.Save(draft);
        RefreshStatus();
    }

    private void ResetToDefaults()
    {
        ShowProfile(new OperatorProfile());
        RefreshStatus();
    }

    private void ShowProfile(OperatorProfile profile)
    {
        draft = profile.Clone();
        operatorIdField.SetTextWithoutNotify(draft.operatorId);
        infoCardsToggle.SetIsOnWithoutNotify(draft.showInfoCards);
        wizardHintsToggle.SetIsOnWithoutNotify(draft.showWizardHints);
        distanceSlider.SetValueWithoutNotify(draft.tabletDistance);
        UpdateDistanceLabel();
    }

    private void UpdateDistanceLabel()
    {
        distanceLabel.text = $"Відстань планшета: {draft.tabletDistance:0.00} м";
    }

    private void RefreshStatus()
    {
        if (draft.SameAs(store.Current))
        {
            SetStatus("Збережено", okColor);
        }
        else
        {
            SetStatus("Є незбережені зміни", pendingColor);
        }
    }

    private void SetStatus(string text, Color color)
    {
        statusText.text = text;
        statusText.color = color;
    }
}
