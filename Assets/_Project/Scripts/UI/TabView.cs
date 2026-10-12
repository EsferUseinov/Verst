using UnityEngine;
using UnityEngine.UI;

public class TabView : MonoBehaviour
{
    [SerializeField] private Button[] tabButtons;
    [SerializeField] private GameObject[] pages;
    [SerializeField] private int startPage;
    [SerializeField] private Color activeColor = new Color(0.95f, 0.55f, 0.16f);

    private ColorBlock[] defaultColors;
    private int currentPage;

    private void Awake()
    {
        currentPage = startPage;
        defaultColors = new ColorBlock[tabButtons.Length];
        for (int i = 0; i < tabButtons.Length; i++)
        {
            int index = i;
            defaultColors[i] = tabButtons[i].colors;
            tabButtons[i].onClick.AddListener(() => ShowPage(index));
        }
    }

    private void OnEnable()
    {
        ShowPage(currentPage);
    }

    public void ShowPage(int index)
    {
        currentPage = index;
        for (int i = 0; i < pages.Length; i++)
        {
            pages[i].SetActive(i == index);
        }

        for (int i = 0; i < tabButtons.Length; i++)
        {
            ColorBlock colors = defaultColors[i];
            if (i == index)
            {
                colors.normalColor = activeColor;
                colors.selectedColor = activeColor;
                colors.highlightedColor = Color.Lerp(activeColor, Color.white, 0.25f);
            }
            tabButtons[i].colors = colors;
        }
    }
}
