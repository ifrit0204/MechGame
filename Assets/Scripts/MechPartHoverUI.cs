// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using UnityEngine.UI;
// using UnityEngine.EventSystems;

// public class MechPartHoverUI : MonoBehaviour, IPointerEnterHandler, ISelectHandler
// {
//     private MechPartData part;
//     private MechPartInfoUI infoUI;
//     [SerializeField] private Image iconImage;

//     void Start()
//     {
//         if(iconImage != null)
//         {
//             iconImage.sprite = part.icon;
//             iconImage.enabled = part.icon != null;
//         }

//     }

//     public void Initialize(MechPartData part, MechPartInfoUI infoUI)
//     {
//         this.part = part;
//         this.infoUI = infoUI;
//     }

//     public void OnPointerEnter(PointerEventData eventData)
//     {
//         Debug.Log("Hover : " + part.partName);
//         infoUI.Show();
//         infoUI.ShowPart(part);
//         // if (parts.Length > 0)
//         // {
//         //     infoUI.ShowPart(parts[0]);
//         // }
//     }

//     public void OnSelect(BaseEventData eventData)
//     {
//         Debug.Log("Select : " + part.partName);
//         infoUI.ShowPart(part);
//         // if (parts.Length > 0)
//         // {
//         //     infoUI.ShowPart(parts[0]);
//         // }
//     }
// }
