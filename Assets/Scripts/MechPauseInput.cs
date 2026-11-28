using UnityEngine;
using UnityEngine.InputSystem;

public class MechPauseInput : MonoBehaviour
{
    private MechPauseMenuUI pauseMenu;

    private MechPauseMenuUI GetPauseMenu()
    {
        if (pauseMenu == null)
            pauseMenu = FindFirstObjectByType<MechPauseMenuUI>(FindObjectsInactive.Include);

        if (pauseMenu == null)
            Debug.LogWarning("このシーンに MechPauseMenuUI がありません", this);

        return pauseMenu;
    }

    public void OnMenu(InputAction.CallbackContext context)
    {
        if (!context.performed)
            return;

        MechPauseMenuUI menu = GetPauseMenu();

        if (menu != null)
            menu.OnMenu(context);
    }

    public void OnCancel(InputAction.CallbackContext context)
    {
        if (!context.performed)
            return;

        MechPauseMenuUI menu = GetPauseMenu();

        if (menu != null)
            menu.CancelPause();
    }
}