using System.Collections;
using UnityEngine;
using UnityEngine.UI;

// 画面に字幕を表示する。バトルのシーンごとに1つ置く(Canvas の下)。
//
// 使い方(どこからでも):
//   MechSubtitleUI.Show("目標の撃破を確認。帰投してください", 3f);
//   MechSubtitleUI.Hide();
// このシーンに字幕UIがなければ、何も起きない(エラーにならない)。
//
// 注意: このスクリプトは、常に有効な(非アクティブにならない)オブジェクトに付けること。
//       表示・非表示を切り替えるのは、子の panel のほう。
public class MechSubtitleUI : MonoBehaviour
{
    private static MechSubtitleUI instance;

    // 字幕の背景と文字をまとめた子オブジェクト(表示のたびにオン・オフされる)
    [SerializeField] private GameObject panel;

    [SerializeField] private Text subtitleText;

    // 任意。付けるとフェードイン・フェードアウトする(panel に付ける)
    [SerializeField] private CanvasGroup canvasGroup;

    [SerializeField] private float fadeTime = 0.2f;

    private Coroutine routine;

    private void Awake()
    {
        instance = this;

        if (panel != null)
            panel.SetActive(false);
    }

    private void OnDestroy()
    {
        if (instance == this)
            instance = null;
    }

    // 字幕を表示する。duration は、文字を出し続ける時間(秒)。
    // 表示中に呼ぶと、前の字幕を置き換える。
    public static void Show(string text, float duration)
    {
        if (instance == null || string.IsNullOrEmpty(text))
            return;

        instance.ShowText(text, duration);
    }

    public static void Hide()
    {
        if (instance != null)
            instance.HideText();
    }

    private void ShowText(string text, float duration)
    {
        if (subtitleText == null)
            return;

        if (routine != null)
            StopCoroutine(routine);

        subtitleText.text = text;

        routine = StartCoroutine(ShowRoutine(duration));
    }

    private void HideText()
    {
        if (routine != null)
        {
            StopCoroutine(routine);
            routine = null;
        }

        if (panel != null)
            panel.SetActive(false);
    }

    private IEnumerator ShowRoutine(float duration)
    {
        if (panel != null)
            panel.SetActive(true);

        if (canvasGroup != null)
            canvasGroup.alpha = 0f;

        yield return StartCoroutine(Fade(0f, 1f));

        yield return new WaitForSecondsRealtime(Mathf.Max(0f, duration));

        yield return StartCoroutine(Fade(1f, 0f));

        if (panel != null)
            panel.SetActive(false);

        routine = null;
    }

    // ゲームが止まっていても動くように、時間は Time.unscaledDeltaTime で数える
    private IEnumerator Fade(float from, float to)
    {
        if (canvasGroup == null || fadeTime <= 0f)
        {
            if (canvasGroup != null)
                canvasGroup.alpha = to;

            yield break;
        }

        float t = 0f;

        while (t < fadeTime)
        {
            t += Time.unscaledDeltaTime;

            canvasGroup.alpha = Mathf.Lerp(from, to, t / fadeTime);

            yield return null;
        }

        canvasGroup.alpha = to;
    }
}
