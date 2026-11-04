using UnityEngine;
using UnityEngine.InputSystem;

public class MechGarageInputController : MonoBehaviour
{
    [SerializeField]
    private MechGarageMenuUI garageMenu;

    [SerializeField]
    private MechPartListUI partListUI;
    
    [SerializeField]
    private MechShopConfirmDialogUI confirmDialog;

    // public void OnCancel(InputAction.CallbackContext context)
    // {
    //     if (!context.performed)
    //         return;

    //     garageMenu.GoBack();
    // }

    public void OnCancel(InputAction.CallbackContext context)
    {
        if (!context.performed)
            return;

        if(confirmDialog.gameObject.activeSelf)
        {
            confirmDialog.Hide();
            return;
        }

        garageMenu.GoBack();
    }
}