using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;

public class DaysMenu : MonoBehaviour
{
    public Canvas canvas;
    public Image canvasBackground;
    public TMP_Text canvasText;
    public float fadeDuration = 4f;
    public float displayTime = 0.5f;

    private Coroutine fadeCoroutine;

    private void Awake()
    {
        SetAlpha(0f);
        canvas.enabled = false;
    }

    public void ShowDay(DayID dayID)
    {
        string rawText = dayID.ToString();
        string formatted = Regex.Replace(rawText, "(\\D)(\\d)", "$1 $2");

        canvasText.text = formatted;
        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);

        canvas.enabled = true;
        fadeCoroutine = StartCoroutine(FadeOutRoutine());
    }

    private IEnumerator FadeOutRoutine()
    {
        SetAlpha(1f);

        yield return new WaitForSeconds(displayTime);

        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / fadeDuration;
            float alpha = Mathf.SmoothStep(1f, 0f, t); 
            SetAlpha(alpha);
            yield return null;
        }

        SetAlpha(0f);
        canvas.enabled = false;
    }

    private void SetAlpha(float alpha)
    {
        if (canvasBackground != null)
        {
            Color bgColor = canvasBackground.color;
            bgColor.a = alpha;
            canvasBackground.color = bgColor;
        }

        if (canvasText != null)
        {
            Color textColor = canvasText.color;
            textColor.a = alpha;
            canvasText.color = textColor;
        }
    }
}
