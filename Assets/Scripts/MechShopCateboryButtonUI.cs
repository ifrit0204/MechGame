using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MechShopCategoryButtonUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField]
    private Image background;

    private bool isSelected;

    private Color normalColor = new Color(1f,1f,1f,0f);

    private Color hoverColor = new Color(1f,1f,1f,0.2f);

    private Color selectedColor = new Color(1f,1f,1f,0.4f);

    public void SetSelected(bool selected)
    {
        isSelected = selected;

        background.color = selected ? selectedColor : normalColor;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if(isSelected)
            return;

        background.color = hoverColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if(isSelected)
            return;

        background.color = normalColor;
    }
}