using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class MechGarageInputController : MonoBehaviour
{
    [SerializeField] private MechGarageMenuUI garageMenu;
    [SerializeField] private MechPartListUI partListUI;
    [SerializeField] private MechShopConfirmDialogUI confirmDialog;      // 既存(購入確認)
    [SerializeField] private MechShopConfirmDialogUI quitDialog;         // 新規(終了確認・別のオブジェクト)

    private GameObject previousSelected;

    public void OnCancel(InputAction.CallbackContext context)
    {
        if (!context.performed)
            return;

        if (quitDialog.gameObject.activeSelf)
        {
            quitDialog.Hide();
            return;
        }

        if (confirmDialog.gameObject.activeSelf)
        {
            confirmDialog.Hide();
            return;
        }

        garageMenu.GoBack();
    }

    public void OnMenu(InputAction.CallbackContext context)
    {
        if (!context.performed)
            return;

        // 開いていれば閉じる(= いいえ)
        if (quitDialog.gameObject.activeSelf)
        {
            quitDialog.Hide();
            return;
        }

        // 購入確認が出ている間は何もしない
        if (confirmDialog.gameObject.activeSelf)
            return;

        previousSelected = EventSystem.current.currentSelectedGameObject;

        // 閉じたあと、元のボタンの選択に戻す(コントローラー操作が止まらないように)
        quitDialog.OnClosed = () => EventSystem.current.SetSelectedGameObject(previousSelected);

        quitDialog.Show("ゲームをやめますか?", QuitGame);
    }

    private void QuitGame()
    {
        #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
        #else
                Application.Quit();
        #endif
    }
}