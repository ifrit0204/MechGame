using UnityEngine;
using UnityEngine.UI;

public class MechGarageHUDUI : MonoBehaviour
{
    [SerializeField] private Text creditsText;

    [SerializeField] private Text guideText;

    private void Update()
    {
        if (MechGameManager.Instance != null)
        {
            creditsText.text = "CREDITS : " + MechGameManager.Instance.Credits;
        }

        guideText.text = "左クリック : 決定\n" + "右クリック : 戻る";
    }
}