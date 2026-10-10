using UnityEngine;

public class ProfileSettingsApplier : MonoBehaviour
{
    [SerializeField] private ProfileStore store;
    [SerializeField] private TabletPresenter tablet;

    private void OnEnable()
    {
        store.Changed += Apply;
    }

    private void OnDisable()
    {
        store.Changed -= Apply;
    }

    private void Start()
    {
        Apply(store.Current);
    }

    private void Apply(OperatorProfile profile)
    {
        tablet.Distance = profile.tabletDistance;
        if (InfoCard.Current != null)
        {
            InfoCard.Current.CardsEnabled = profile.showInfoCards;
        }
    }
}
