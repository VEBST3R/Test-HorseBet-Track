using UnityEditor;
using UnityEngine;

namespace HorseBet.Gameplay
{
    public class Horse : MonoBehaviour
    {
        [Header("Horse Identity")]
        public string horseName = "Unnamed Horse";

        [Header("Speed Settings")]
        [SerializeField] float baseCurveDuration = 2f;
        [SerializeField] float curveDurationVariance = 1f;
        [SerializeField] float baseMinSpeed = 1f;
        [SerializeField] float baseMaxSpeed = 10f;
        [SerializeField] float speedVariance = 2f;
        [SerializeField] float transitionBlendTime = 0.5f;

        AnimationCurve currentSpeedCurve;
        AnimationCurve nextSpeedCurve;
        float curveDuration;
        float nextCurveDuration;
        float minSpeed;
        float maxSpeed;
        float nextMinSpeed;
        float nextMaxSpeed;
        float currentSpeed = 0f;
        float elapsedTime = 0f;
        bool isTransitioning = false;
        float transitionStartTime = 0f;

        void Start()
        {
            currentSpeedCurve = GenerateRandomCurve();
            RandomizeSpeedParameters();

            PrepareNextCycle();
        }

        AnimationCurve GenerateRandomCurve()
        {
            AnimationCurve curve = new AnimationCurve();

            curve.AddKey(new Keyframe(0f, Random.Range(0f, 0.3f)));

            int keyCount = Random.Range(2, 5);
            for (int i = 0; i < keyCount; i++)
            {
                float time = Random.Range(0.2f, 0.9f);
                float value = Random.Range(0.1f, 1.0f);
                Keyframe key = new Keyframe(time, value);

                key.inTangent = Random.Range(-0.5f, 0.5f);
                key.outTangent = Random.Range(-0.5f, 0.5f);
                curve.AddKey(key);
            }

            curve.AddKey(new Keyframe(1f, Random.Range(0f, 0.3f)));

            for (int i = 0; i < curve.keys.Length; i++)
            {
#if UNITY_EDITOR
                AnimationUtility.SetKeyBroken(curve, i, false);
                AnimationUtility.SetKeyLeftTangentMode(curve, i, AnimationUtility.TangentMode.Auto);
                AnimationUtility.SetKeyRightTangentMode(curve, i, AnimationUtility.TangentMode.Auto);
#endif
            }

            return curve;
        }

        void RandomizeSpeedParameters()
        {
            curveDuration = baseCurveDuration + Random.Range(-curveDurationVariance, curveDurationVariance);
            curveDuration = Mathf.Max(0.8f, curveDuration);

            minSpeed = baseMinSpeed + Random.Range(-speedVariance * 0.5f, speedVariance * 0.5f);
            minSpeed = Mathf.Max(0.5f, minSpeed);

            maxSpeed = baseMaxSpeed + Random.Range(-speedVariance * 0.5f, speedVariance * 0.5f);
            maxSpeed = Mathf.Max(minSpeed + 1f, maxSpeed);
        }

        void PrepareNextCycle()
        {
            nextSpeedCurve = GenerateRandomCurve();

            nextCurveDuration = baseCurveDuration + Random.Range(-curveDurationVariance, curveDurationVariance);
            nextCurveDuration = Mathf.Max(0.8f, nextCurveDuration);

            nextMinSpeed = baseMinSpeed + Random.Range(-speedVariance * 0.5f, speedVariance * 0.5f);
            nextMinSpeed = Mathf.Max(0.5f, nextMinSpeed);

            nextMaxSpeed = baseMaxSpeed + Random.Range(-speedVariance * 0.5f, speedVariance * 0.5f);
            nextMaxSpeed = Mathf.Max(nextMinSpeed + 1f, nextMaxSpeed);
        }

        void Update()
        {
            elapsedTime += Time.deltaTime;

            if (!isTransitioning && elapsedTime >= (curveDuration - transitionBlendTime))
            {
                isTransitioning = true;
                transitionStartTime = elapsedTime;
            }

            if (!isTransitioning)
            {
                float normalizedTime = Mathf.Clamp01(elapsedTime / curveDuration);
                float curveValue = currentSpeedCurve.Evaluate(normalizedTime);
                currentSpeed = Mathf.Lerp(minSpeed, maxSpeed, curveValue);
            }
            else
            {
                float transitionProgress = (elapsedTime - transitionStartTime) / transitionBlendTime;
                transitionProgress = Mathf.Clamp01(transitionProgress);

                float normalizedTime1 = Mathf.Clamp01(elapsedTime / curveDuration);
                float curveValue1 = currentSpeedCurve.Evaluate(normalizedTime1);
                float speed1 = Mathf.Lerp(minSpeed, maxSpeed, curveValue1);

                float normalizedTime2 = Mathf.Clamp01((elapsedTime - transitionStartTime) / transitionBlendTime);
                float curveValue2 = nextSpeedCurve.Evaluate(normalizedTime2);
                float speed2 = Mathf.Lerp(nextMinSpeed, nextMaxSpeed, curveValue2);

                currentSpeed = Mathf.Lerp(speed1, speed2, transitionProgress);

                if (transitionProgress >= 1.0f)
                {
                    SwitchToNextCycle();
                }
            }

            transform.position += new Vector3(0, 0, 1) * Time.deltaTime * currentSpeed;
        }

        void SwitchToNextCycle()
        {
            currentSpeedCurve = nextSpeedCurve;
            curveDuration = nextCurveDuration;
            minSpeed = nextMinSpeed;
            maxSpeed = nextMaxSpeed;

            elapsedTime = 0f;
            isTransitioning = false;

            PrepareNextCycle();
        }
    }
}