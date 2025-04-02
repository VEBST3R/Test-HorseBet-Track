using HorseBet.Gameplay;
using HorseBet.Camera;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FinishCollider : MonoBehaviour
{
    [Header("Race Settings")]
    [SerializeField] private int totalHorses = 0;

    private HashSet<Horse> finishedHorses = new HashSet<Horse>();
    private bool raceFinished = false;

    private void Start()
    {
        if (totalHorses <= 0)
        {
            Horse[] allHorses = FindObjectsByType<Horse>(FindObjectsSortMode.None);
            totalHorses = allHorses.Length;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (raceFinished)
        {
            Debug.Log("Race already finished, ignoring finish trigger for: " + other.name);
            return;
        }

        Debug.Log("Хтось попав у колайдер фінішу!");

        StartCoroutine(SmoothMoveToFinish(other.transform, new Vector3(other.transform.position.x, other.transform.position.y, transform.position.z), 1f));

        Animator childAnimator = other.GetComponentInChildren<Animator>();
        if (childAnimator != null)
        {
            childAnimator.SetTrigger("Stop");
        }

        Horse horseObject = other.GetComponent<Horse>();
        if (horseObject != null)
        {
            horseObject.enabled = false;

            if (GameManager.Instance != null)
            {
                GameManager.Instance.RegisterHorseFinish(horseObject);
            }

            if (!finishedHorses.Contains(horseObject))
            {
                finishedHorses.Add(horseObject);

                if (finishedHorses.Count >= totalHorses && !raceFinished)
                {
                    raceFinished = true;
                    Debug.Log("Last horse finished! Switching camera.");
                    TransitionToWinnerCamera();

                    if (GameManager.Instance != null)
                    {
                        GameManager.Instance.EndRace();
                    }
                }
            }
        }
    }

    public void ResetFinishCollider()
    {
        Debug.Log("Resetting finish collider state");
        finishedHorses.Clear();
        raceFinished = false;
    }

    private IEnumerator SmoothMoveToFinish(Transform target, Vector3 destination, float duration)
    {
        Vector3 startPosition = target.position;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            target.position = Vector3.Lerp(startPosition, destination, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        target.position = destination;
    }

    private void TransitionToWinnerCamera()
    {
        if (CameraManager.Instance != null)
        {
            CameraManager.Instance.TransitionToWinnerCamera();
        }
        else
        {
            Debug.LogWarning("CameraManager instance not found! Cannot transition camera.");
        }
    }
}