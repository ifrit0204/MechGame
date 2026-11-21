using UnityEngine;

public static class MechStatText
{
    // "ラベル : 値 (+差)" を作る。良くなれば緑、悪くなれば赤
    // higherIsBetter = false のときは、値が小さくなるほうを緑にする
    public static string Line(string label, float value, float? compare, bool higherIsBetter = true, string format = "0.##")
    {
        string text = label + " : " + value.ToString(format);

        if (!compare.HasValue)
            return text;

        float diff = value - compare.Value;

        if (Mathf.Abs(diff) < 0.0001f)
            return text;

        bool good = higherIsBetter ? diff > 0f : diff < 0f;

        string color = good ? "lime" : "red";
        string sign = diff > 0f ? "+" : "-";

        return $"{text} <color={color}>({sign}{Mathf.Abs(diff).ToString(format)})</color>";
    }
}