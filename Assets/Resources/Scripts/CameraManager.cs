using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace HorseBet.Camera
{
    public class CameraManager : MonoBehaviour
    {
        [Header("Camera References")]
        [SerializeField] private GameObject raceCamera;
        [SerializeField] private GameObject winnerCamera;

        [Header("Canvas References")]
        [SerializeField] private Canvas raceCanvas;
        [SerializeField] private Canvas endRaceCanvas;

        [Header("Transition Settings")]
        [SerializeField] private float transitionDuration = 2.0f;
        [SerializeField] private AnimationCurve transitionCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

        [Header("Race Camera")]
        [SerializeField] private CameraSettings raceCameraSettings;

        [Header("Fade Effect")]
        [SerializeField] private Image fadeImage;
        [SerializeField] private float fadeDuration = 1.0f;
        [SerializeField] private AnimationCurve fadeCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

        private static CameraManager _instance;
        public static CameraManager Instance => _instance;

        private void Awake()
        {
            if (_instance == null)
                _instance = this;
            else
                Destroy(gameObject);

            if (raceCamera && winnerCamera)
            {
                raceCamera.SetActive(true);
                winnerCamera.SetActive(false);
            }

            if (raceCameraSettings != null)
            {
                raceCameraSettings.EnableIdleMode();
            }
        }

        public void TransitionToWinnerCamera()
        {
            if (raceCamera && winnerCamera)
            {
                StartCoroutine(TransitionCamerasWithFade(raceCamera, winnerCamera));

                StartCoroutine(SwitchCanvasesAfterDelay(raceCanvas, endRaceCanvas, fadeDuration));
            }
        }

        public void TransitionToRaceCamera()
        {
            if (raceCamera && winnerCamera)
            {
                StartCoroutine(TransitionCamerasWithFade(winnerCamera, raceCamera));

                StartCoroutine(SwitchCanvasesAfterDelay(endRaceCanvas, raceCanvas, fadeDuration));
            }

            if (raceCameraSettings != null)
            {
                raceCameraSettings.EnableIdleMode();
            }
        }

        public void SetRaceCameraTarget(Transform target)
        {
            if (raceCameraSettings != null && target != null)
            {
                Debug.Log($"Setting race camera to follow: {target.name}");
                raceCameraSettings.SetTarget(target);
            }
            else
            {
                if (raceCameraSettings == null)
                    Debug.LogError("raceCameraSettings is null! Assign it in the inspector.");
                if (target == null)
                    Debug.LogError("target is null! Cannot set camera target.");
            }
        }

        public void SetRaceCameraIdle()
        {
            if (raceCameraSettings != null)
            {
                raceCameraSettings.EnableIdleMode();
            }
        }

        private IEnumerator SwitchCanvasesAfterDelay(Canvas fromCanvas, Canvas toCanvas, float delay)
        {
            yield return new WaitForSeconds(delay * 0.5f);

            if (fromCanvas != null && fromCanvas.gameObject != null)
                fromCanvas.gameObject.SetActive(false);

            if (toCanvas != null && toCanvas.gameObject != null)
                toCanvas.gameObject.SetActive(true);
        }

        private IEnumerator TransitionCamerasWithFade(GameObject fromCamera, GameObject toCamera)
        {
            if (fadeImage == null)
            {
                Debug.LogWarning("Fade image is not set in CameraManager, falling back to simple transition");
                yield return StartCoroutine(TransitionCameras(fromCamera, toCamera));
                yield break;
            }

            fadeImage.gameObject.SetActive(true);
            Color fadeColor = fadeImage.color;
            fadeColor.a = 0;
            fadeImage.color = fadeColor;

            float elapsedTime = 0;
            while (elapsedTime < fadeDuration * 0.5f)
            {
                elapsedTime += Time.deltaTime;
                float normalizedTime = elapsedTime / (fadeDuration * 0.5f);
                float alpha = fadeCurve.Evaluate(normalizedTime);

                fadeColor.a = alpha;
                fadeImage.color = fadeColor;
                yield return null;
            }

            fadeColor.a = 1;
            fadeImage.color = fadeColor;

            toCamera.SetActive(true);
            fromCamera.SetActive(false);

            elapsedTime = 0;
            while (elapsedTime < fadeDuration * 0.5f)
            {
                elapsedTime += Time.deltaTime;
                float normalizedTime = elapsedTime / (fadeDuration * 0.5f);
                float alpha = fadeCurve.Evaluate(normalizedTime);

                fadeColor.a = 1 - alpha;
                fadeImage.color = fadeColor;
                yield return null;
            }

            fadeColor.a = 0;
            fadeImage.color = fadeColor;
            fadeImage.gameObject.SetActive(false);
        }

        private IEnumerator TransitionCameras(GameObject fromCamera, GameObject toCamera)
        {
            toCamera.SetActive(true);

            var fromCam = fromCamera.GetComponent<UnityEngine.Camera>();
            var toCam = toCamera.GetComponent<UnityEngine.Camera>();

            float fromWeight = 1f;
            float toWeight = 0f;

            if (fromCam && toCam)
            {
                float elapsedTime = 0;

                while (elapsedTime < transitionDuration)
                {
                    elapsedTime += Time.deltaTime;
                    float t = transitionCurve.Evaluate(elapsedTime / transitionDuration);

                    fromWeight = 1 - t;
                    toWeight = t;

                    fromCam.depth = toWeight < 0.5f ? 1 : 0;
                    toCam.depth = toWeight >= 0.5f ? 1 : 0;

                    yield return null;
                }
            }

            fromCamera.SetActive(false);
        }
    }
}
