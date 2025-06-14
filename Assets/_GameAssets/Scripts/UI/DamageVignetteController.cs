using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DamageVignetteController : MonoBehaviour
{
    [SerializeField] private Image vignetteImage;
    [SerializeField] private float fadeDuration = 1f;
    [SerializeField] private float maxAlpha = 0.5f;

    private Coroutine fadeCoroutine;

    private void Start()
    {
        SetAlpha(0f);
    }

    public void ShowVignette()
    {
        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        fadeCoroutine = StartCoroutine(FadeOut());
    }

    private IEnumerator FadeOut()
    {
        SetAlpha(maxAlpha);

        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            float newAlpha = Mathf.Lerp(maxAlpha, 0f, elapsed / fadeDuration);
            SetAlpha(newAlpha);
            elapsed += Time.deltaTime;
            yield return null;
        }

        SetAlpha(0f);
    }

    private void SetAlpha(float a)
    {
        if (vignetteImage != null)
        {
            Color c = vignetteImage.color;
            c.a = a;
            vignetteImage.color = c;
        }
    }
}
