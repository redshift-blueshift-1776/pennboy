using System;
using TMPro;
using UnityEngine;

public class Clock : MonoBehaviour
{
    [SerializeField] private TMP_Text hour;
    [SerializeField] private CanvasGroup colon;
    [SerializeField] private TMP_Text minute;

    public void SetTime(DateTime dateTime) {
        hour.text = $"{dateTime:HH}";
        minute.text = $"{dateTime:mm}";
    }

    private void Update() {
        colon.alpha = Mathf.Clamp01(0.5f * Mathf.Sin(7f * Time.time) + 0.5f);
    }
}