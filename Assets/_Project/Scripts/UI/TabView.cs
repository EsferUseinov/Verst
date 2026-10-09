using UnityEngine;
using UnityEngine.UI;

public class TabView : MonoBehaviour
{
    [SerializeField] private Button[] tabButtons;
    [SerializeField] private GameObject[] pages;
    [SerializeField] private int startPage;

    private void Awake()
    {
        for (int i = 0; i < tabButtons.Length; i++)
        {
            int index = i;
            tabButtons[i].onClick.AddListener(() => ShowPage(index));
        }
    }

    private void OnEnable()
    {
        ShowPage(startPage);
    }

    public void ShowPage(int index)
    {
        for (int i = 0; i < pages.Length; i++)
        {
            pages[i].SetActive(i == index);
        }

        for (int i = 0; i < tabButtons.Length; i++)
        {
            tabButtons[i].interactable = i != index;
        }
    }
}
