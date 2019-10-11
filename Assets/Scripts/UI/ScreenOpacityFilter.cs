using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScreenOpacityFilter : MonoBehaviour
{
    Image filter;

    void Start()
    {
        filter = GetComponent<Image>();
        filter.color = new Color(filter.color.r, filter.color.g, filter.color.b, 1f);
        FadeIn();
    }

    public void FadeOut(float duration = 1f)
    {
        StopAllCoroutines();
        StartCoroutine(TweenAlpha(1f, duration));
    }

    public void FadeIn(float duration = 1f)
    {
        StopAllCoroutines();
        StartCoroutine(TweenAlpha(0f, duration));
    }

    IEnumerator TweenAlpha(float target, float duration)
    {
        float delta = filter.color.a < target ? 0.01f : -0.01f;
        while (Mathf.Abs(filter.color.a - target) >= 0.01f)
        {
            filter.color = new Color(filter.color.r, filter.color.g, filter.color.b, filter.color.a + delta);
            yield return new WaitForSecondsRealtime(0.01f * duration);
        }
    }
}
