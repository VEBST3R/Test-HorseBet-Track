using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace HorseBet.UI
{
    [RequireComponent(typeof(Image))]
    public class FadeEffect : MonoBehaviour
    {
        [SerializeField] private float fadeDuration = 1.0f;
        [SerializeField] private AnimationCurve fadeCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

        private Image fadeImage;

        void Awake()
        {
            fadeImage = GetComponent<Image>();

            Color color = fadeImage.color;
            color.a = 0;
            fadeImage.color = color;
            gameObject.SetActive(false);
        }

        public void FadeOut()
        {
            gameObject.SetActive(true);
            StopAllCoroutines();
            StartCoroutine(FadeTo(1));
        }

        public void FadeIn()
        {
            gameObject.SetActive(true);
            StopAllCoroutines();
            StartCoroutine(FadeTo(0));
        }

        private IEnumerator FadeTo(float targetAlpha)
        {
            Color currentColor = fadeImage.color;
            float startAlpha = currentColor.a;

            float elapsedTime = 0;
            while (elapsedTime < fadeDuration)
            {
                elapsedTime += Time.deltaTime;
                float normalizedTime = elapsedTime / fadeDuration;
                float evaluatedTime = fadeCurve.Evaluate(normalizedTime);

                currentColor.a = Mathf.Lerp(startAlpha, targetAlpha, evaluatedTime);
                fadeImage.color = currentColor;
                yield return null;
            }

            currentColor.a = targetAlpha;
            fadeImage.color = currentColor;

            if (targetAlpha == 0)
            {
                gameObject.SetActive(false);
            }
        }
    }
}
