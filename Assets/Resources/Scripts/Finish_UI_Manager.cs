using UnityEngine;
using TMPro;
using HorseBet.Gameplay;
using System.Collections.Generic;

public class Finish_UI_Manager : MonoBehaviour
{
    [Header("Horse Place Labels")]
    [SerializeField] private TMP_Text horse1PlaceText;
    [SerializeField] private TMP_Text horse2PlaceText;
    [SerializeField] private TMP_Text horse3PlaceText;

    [Header("Race Results")]
    [SerializeField] private TMP_Text resultsHeaderText;

    [Header("Player Results")]
    [SerializeField] private TMP_Text winningsText;
    [SerializeField] private TMP_Text playerWinsText;
    [SerializeField] private TMP_Text playerLosesText;
    [SerializeField] private TMP_Text moneyChangeText;
    [SerializeField] private TMP_Text playerBudgetText;

    [SerializeField] private Player player;

    private void OnEnable()
    {
        DisplayResults();
    }

    public void DisplayResults()
    {
        DisplayHorsePlaces();
        DisplayWinnings();
        UpdatePlayerStats();
    }

    private void DisplayHorsePlaces()
    {
        if (horse1PlaceText != null) horse1PlaceText.text = "";
        if (horse2PlaceText != null) horse2PlaceText.text = "";
        if (horse3PlaceText != null) horse3PlaceText.text = "";

        if (GameManager.Instance != null)
        {
            List<Horse> finishedHorses = GameManager.Instance.GetFinishedHorses();

            for (int i = 0; i < finishedHorses.Count; i++)
            {
                Horse horse = finishedHorses[i];
                int place = i + 1;

                if (horse.name.Contains("Horse_1") && horse1PlaceText != null)
                {
                    horse1PlaceText.text = place.ToString();
                }
                else if (horse.name.Contains("Horse_2") && horse2PlaceText != null)
                {
                    horse2PlaceText.text = place.ToString();
                }
                else if (horse.name.Contains("Horse_3") && horse3PlaceText != null)
                {
                    horse3PlaceText.text = place.ToString();
                }
            }
        }
    }

    private void DisplayWinnings()
    {
        if (GameManager.Instance != null)
        {
            int winAmount = GameManager.Instance.CalculateWinnings();
            int betAmount = PlayerDataManager.GetCurrentBet();

            Debug.Log($"Final result: winAmount={winAmount}, original bet={betAmount}");

            if (winAmount > 0)
            {
                resultsHeaderText.text = "Your horse finished 1st!";
                moneyChangeText.text = $"+{winAmount}$";
                moneyChangeText.color = Color.green;
            }
            else
            {
                resultsHeaderText.text = "Your horse didn't win!";
                moneyChangeText.text = $"You lost {betAmount}$";
                moneyChangeText.color = Color.red;
            }
        }
    }

    private void UpdatePlayerStats()
    {
        int money = PlayerDataManager.GetMoney();
        int wins = PlayerDataManager.GetWins();
        int losses = PlayerDataManager.GetLosses();

        playerWinsText.text = $"Wins: {wins}";
        playerLosesText.text = $"Losses: {losses}";
        playerBudgetText.text = $"Budget: {money}$";

        Debug.Log($"Displayed player stats - Money: {money}, Wins: {wins}, Losses: {losses}");
    }

    public void RestartRace()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.RestartRace();
        }
    }

    public void BackToMainMenu()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ReturnToMainMenu();
        }
    }
}
