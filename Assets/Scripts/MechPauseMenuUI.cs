using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class MechPauseMenuUI : MonoBehaviour
{
    // 他のスクリプトから「今ポーズ中か」を見るための目印
    public static bool IsPaused { get; private set; }

    [SerializeField] private MechShopConfirmDialogUI confirmDialog;
    [SerializeField] private string garageSceneName = "GarageScene";
    [SerializeField] private string message = "ガレージに戻りますか?";

    private static CursorLockMode prevLockState;
    private static bool prevCursorVisible;

    private void Start()
    {
        // 「いいえ」や別の方法でダイアログが閉じたときも再開する
        confirmDialog.OnClosed += Resume;
    }

    // ポーズ中なら閉じて再開する(Cancel ボタン用)
    public void CancelPause()
    {
        if (IsPaused)
            Resume();
    }

    private void OnDestroy()
    {
        if (confirmDialog != null)
            confirmDialog.OnClosed -= Resume;

        // シーンをまたいで時間停止が残らないようにする
        SetPaused(false);
    }

    // PlayerInput の Menu アクションのイベントから呼ぶ
    public void OnMenu(InputAction.CallbackContext context)
    {
        if (!context.performed)
            return;

        if (IsPaused) Resume();
        else Pause();
    }

    private void Pause()
    {
        // ミッション完了の演出中はポーズさせない(演出は実時間で進むため)
        if (MechGameManager.Instance != null && MechGameManager.Instance.IsMissionClearing)
            return;

        SetPaused(true);
        confirmDialog.Show(message, ReturnToGarage);
    }

    private void Resume()
    {
        if (!IsPaused)
            return;

        SetPaused(false);

        // Hide() 経由で OnClosed → Resume が再度呼ばれても、上の判定で止まる
        if (confirmDialog.gameObject.activeSelf)
            confirmDialog.Hide();
    }

    private void ReturnToGarage()
    {
        SetPaused(false);

        // ボイスや字幕が残らないようにする
        if (MechGameManager.Instance != null)
            MechGameManager.Instance.StopBriefing();

        SceneManager.LoadScene(garageSceneName);
    }

    private static void SetPaused(bool paused)
    {
        if (paused == IsPaused)
            return;

        if (paused)
        {
            prevLockState = Cursor.lockState;
            prevCursorVisible = Cursor.visible;

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = prevLockState;
            Cursor.visible = prevCursorVisible;
        }

        IsPaused = paused;
        Time.timeScale = paused ? 0f : 1f;
        AudioListener.pause = paused;
    }
}