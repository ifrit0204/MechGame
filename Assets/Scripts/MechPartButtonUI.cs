using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MechPartButtonUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler
{
    private MechPartData currentPart;
    private MechPartInfoUI infoUI;
    private MechStatsUI statsUI;
    private PartType partType;


    [Header("UI")]
    [SerializeField] private Image icon;

    [SerializeField] private Text partName;

    [SerializeField] private Text weight;

    [SerializeField] private Text energy;

    [SerializeField] private Text priceText;

    [SerializeField] private Sprite emptyIcon;

    // [SerializeField]
    // private GameObject selectedBackground;

    [SerializeField]
    private Image selectedBackground;

    private Color normalColor = new Color(1f,1f,1f,0.2f);

    private Color hoverColor = new Color(1f,1f,1f,0.5f);

    [Header("Equipped")]
    [SerializeField]
    private GameObject equippedMark;
    private bool isSelected;
    void Start()
    {
        SetSelected(false);
    }


    //==================================================
    // パーツ情報設定
    //==================================================
    // public void Initialize(MechPartData part, MechPartInfoUI infoUI)
    // {
    //     currentPart = part;
    //     this.infoUI = infoUI;
    // }
    public void Initialize(MechPartData part, PartType partType, MechPartInfoUI infoUI, MechStatsUI statsUI)
    {
        currentPart = part;
        this.partType = partType;
        this.infoUI = infoUI;
        this.statsUI = statsUI;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if(currentPart != null && infoUI != null)
        {
            infoUI.ShowPart(currentPart);
        }

        if(statsUI != null)
        {
            statsUI.ShowPreviewStats(partType, currentPart);
        }

        if(selectedBackground != null)
        {
            selectedBackground.color = hoverColor;
        }
    }
    // public void OnPointerEnter(PointerEventData eventData)
    // {
    // }
    public void OnPointerExit(PointerEventData eventData)
    {
        if (selectedBackground == null)
        {
            return;
        }

        selectedBackground.color = isSelected ? new Color(1f,1f,1f,0.4f) : new Color(1f,1f,1f,0f);
    }
    public void OnSelect(BaseEventData eventData)
    {
        if(currentPart == null)
            return;

        if(infoUI != null)
        {
            infoUI.ShowPart(currentPart);
        }

        if(statsUI != null)
        {
            statsUI.ShowPreviewStats(
                partType,
                currentPart);
        }
    }
    public void SetSelected(bool selected)
    {
        isSelected = selected;

        if (selectedBackground == null)
        {
            return;
        }

        selectedBackground.color = selected ? new Color(1f,1f,1f,0.4f) : new Color(1f,1f,1f,0f);
    }

    public void SetPart(MechPartData part)
    {
        Debug.Log("SetPart : " + part.partName);
        if (part == null)
        {
            Clear();
            return;
        }


        if (partName != null)
        {
            partName.text =
                part.partName;
        }


        if (icon != null)
        {
            icon.sprite =
                part.icon;

            icon.enabled =
                part.icon != null;
        }


        if (weight != null)
        {
            weight.text =
                "Weight : " +
                part.weight.ToString("0");
        }


        if (energy != null)
        {
            energy.text = "EN : " + part.energyConsumption.ToString("0");
        }
        
        if(priceText != null)
        {
            priceText.text = part.price.ToString();
        }
    }


    //==================================================
    // EQUIPPED表示
    //==================================================

    public void SetEquipped(bool equipped)
    {
        if (equippedMark != null)
        {
            equippedMark.SetActive(equipped);
        }
    }


    //==================================================
    // クリア
    //==================================================

    public void Clear()
    {
        if (icon != null)
        {
            icon.sprite = emptyIcon;
            icon.enabled = true;
        }


        if (partName != null)
        {
            partName.text = "-";
        }


        if (weight != null)
        {
            weight.text =
                "Weight : -";
        }


        if (energy != null)
        {
            energy.text =
                "EN : -";
        }


        SetEquipped(false);
        SetSelected(false);
    }
}
