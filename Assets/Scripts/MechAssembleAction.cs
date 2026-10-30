// using UnityEngine;
// using UnityEngine.UI;
// using UnityEngine.InputSystem;

// public class MechAssembleAction : MonoBehaviour
// {
//     private MechGarageMenuUI garageMenu;
//     private MechPartListUI partListUI;
//     private Text guideText;

//     public void Initialize(MechGarageMenuUI garageMenu, MechPartListUI partListUI, Text guideText)
//     {
//         this.garageMenu = garageMenu;
//         this.partListUI = partListUI;
//         this.guideText = guideText;
//     }

//     public void OnCancel(InputAction.CallbackContext context)
//     {
//         Debug.Log("Cancel");
        
//         if (!context.performed)
//             return;

//         if (partListUI.IsPartListOpen())
//         {
//             partListUI.HideParts();
//         }
//         else
//         {
//             garageMenu.CloseAssembly();
//         }
//     }
// }