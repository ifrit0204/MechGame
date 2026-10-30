using UnityEngine;
using UnityEngine.InputSystem;

public class MechGarageInputController : MonoBehaviour
{
    [SerializeField]
    private MechGarageMenuUI garageMenu;

    [SerializeField]
    private MechPartListUI partListUI;

    public void OnCancel(InputAction.CallbackContext context)
    {
        if (!context.performed)
            return;

        // if (garageMenu.CurrentState == GarageState.PartList)
        // {
        //     partListUI.ClosePartList();
        // }

        garageMenu.GoBack();
    }
}