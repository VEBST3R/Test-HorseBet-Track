using UnityEngine;
using HorseBet.Gameplay;
using UnityEngine.InputSystem.XR.Haptics;
using System.Collections.Generic;
using HorseBet.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    [SerializeField] private Horse[] horses = new Horse[0];
    [SerializeField] private UI_Manager uiManager;

    public int CurrentBet = 0;
    public int CurrentHorse = -1;
    private List<Horse> finishedHorses = new List<Horse>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            PlayerDataManager.InitializePlayerData();

            CurrentBet = PlayerDataManager.GetCurrentBet();
            CurrentHorse = PlayerDataManager.GetSelectedHorse();
        }
        else
        {
            Destroy(gameObject);
        }
        Application.targetFrameRate = 300;
    }

    public void StartRace()
    {
        Debug.Log("StartRace метод викликано");

        finishedHorses.Clear();

        if (horses == null || horses.Length == 0)
        {
            Debug.LogError("Масив коней порожній або не ініціалізований в інспекторі!");
            return;
        }

        if (CurrentHorse >= 0 && CurrentHorse < horses.Length)
        {
            Horse selectedHorse = horses[CurrentHorse];

            Debug.Log($"Selected horse index: {CurrentHorse}, Horse name: {selectedHorse.name}");

            HorseBet.Camera.CameraManager cameraManager = FindFirstObjectByType<HorseBet.Camera.CameraManager>();
            if (cameraManager != null)
            {
                cameraManager.SetRaceCameraTarget(selectedHorse.transform);
                Debug.Log($"Camera should now follow horse: {selectedHorse.name}");
            }
            else
            {
                Debug.LogError("CameraManager not found in the scene!");
            }
        }
        else
        {
            Debug.LogError($"Invalid CurrentHorse index: {CurrentHorse}. Cannot set camera target.");
        }

        foreach (Horse horse in horses)
        {
            horse.enabled = true;
            Animator animator = horse.gameObject.GetComponentInChildren<Animator>();
            if (animator != null)
            {
                animator.SetTrigger("Run");
            }
            else
            {
                Debug.LogWarning($"No animator found on horse: {horse.name}");
            }
        }
    }

    public void RegisterHorseFinish(Horse horse)
    {
        if (!finishedHorses.Contains(horse))
        {
            finishedHorses.Add(horse);
        }
    }

    public void EndRace()
    {
        ProcessBetResults();

        ShowResults();
    }

    public List<Horse> GetFinishedHorses()
    {
        return new List<Horse>(finishedHorses);
    }

    public int CalculateWinnings()
    {
        Debug.Log($"CalculateWinnings: CurrentHorse={CurrentHorse}, CurrentBet={CurrentBet}");

        if (CurrentHorse < 0 || CurrentHorse >= horses.Length || CurrentBet <= 0)
        {
            Debug.Log("Invalid bet parameters!");
            return 0;
        }

        Horse selectedHorse = horses[CurrentHorse];

        Debug.Log($"Checking bet result for horse: {selectedHorse.name}, index: {CurrentHorse}");

        int position = -1;
        for (int i = 0; i < finishedHorses.Count; i++)
        {
            Debug.Log($"Finished horse {i}: {finishedHorses[i].name}");

            if (finishedHorses[i] == selectedHorse)
            {
                position = i;
                Debug.Log($"Found match by reference! Horse {selectedHorse.name} finished at position {position}");
                break;
            }

            if (position == -1 && finishedHorses[i].name == selectedHorse.name)
            {
                position = i;
                Debug.Log($"Found match by name! Horse {selectedHorse.name} finished at position {position}");
                break;
            }
        }

        if (position == -1)
        {
            Debug.LogWarning($"Horse {selectedHorse.name} not found among finished horses!");
            return 0;
        }

        Debug.Log($"Horse {selectedHorse.name} finished in position {position}");

        if (position == 0)
        {
            Debug.Log("Horse won 1st place! 2x bet amount!");
            return CurrentBet * 2;
        }
        else
        {
            Debug.Log("Horse did not finish 1st. No winnings.");
            return 0;
        }
    }

    private void ProcessBetResults()
    {
        int winAmount = CalculateWinnings();

        if (winAmount > 0)
        {
            PlayerDataManager.AddMoney(winAmount);
            PlayerDataManager.AddWin();
            Debug.Log($"Player won! Added {winAmount}$. New balance: {PlayerDataManager.GetMoney()}$");
        }
        else if (CurrentBet > 0)
        {
            PlayerDataManager.AddLoss();
            Debug.Log($"Player lost. Current balance: {PlayerDataManager.GetMoney()}$");
        }
    }

    private void ShowResults()
    {
        Finish_UI_Manager resultsManager = FindFirstObjectByType<Finish_UI_Manager>();
        if (resultsManager != null && resultsManager.gameObject.activeSelf)
        {
            resultsManager.DisplayResults();
        }
    }

    public void SetBet(int amount, int horseIndex)
    {
        if (PlayerDataManager.TrySpendMoney(amount))
        {
            CurrentBet = amount;
            CurrentHorse = horseIndex;

            PlayerDataManager.SetCurrentBet(amount);
            PlayerDataManager.SetSelectedHorse(horseIndex);

            Debug.Log($"Bet placed: {amount}$ on horse {horseIndex}. Current money: {PlayerDataManager.GetMoney()}$");
        }
        else
        {
            Debug.LogError($"Not enough money to place bet: {amount}$");
        }
    }

    public void RestartRace()
    {
        Debug.Log("Restarting race...");

        finishedHorses.Clear();

        FinishCollider finishCollider = FindFirstObjectByType<FinishCollider>();
        if (finishCollider != null)
        {
            finishCollider.ResetFinishCollider();
            Debug.Log("Finish collider reset");
        }
        else
        {
            Debug.LogWarning("Could not find FinishCollider object!");
        }

        HorseBet.Camera.CameraManager cameraManager = FindFirstObjectByType<HorseBet.Camera.CameraManager>();
        if (cameraManager != null)
        {
            cameraManager.SetRaceCameraIdle();
        }
    }

    public void ReturnToMainMenu()
    {
        if (uiManager != null)
        {
            uiManager.MenuToHorseChoose();
        }
    }

    public void exitgame()
    {
        Application.Quit();
    }
}
