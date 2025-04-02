using UnityEngine;
using TMPro;
using UnityEngine.UI;
using HorseBet.UI;
using HorseBet.Gameplay;

public class Bet_UI_Manager : MonoBehaviour
{
    [SerializeField] private TMP_InputField betInputField;
    [SerializeField] private TMP_Text PlayerMoneyText;
    [SerializeField] private TMP_Text errText;
    [SerializeField] private Button betButton;
    [SerializeField] private Button LoanButton;
    [SerializeField] private UI_Manager uiManager;

    private void OnEnable()
    {
        UpdateTextInfo(PlayerMoneyText, "Budget: " + PlayerDataManager.GetMoney() + "$");
        UpdateTextInfo(errText, "");
        betInputField.text = "";
        betButton.interactable = false;

        if (uiManager == null)
            uiManager = Object.FindFirstObjectByType<UI_Manager>();

        UpdateLoanButtonState();
    }

    public void UpdateTextInfo(TMP_Text _text, string _info)
    {
        _text.text = _info;
    }

    public void UpdateBetInputField()
    {
        if (int.TryParse(betInputField.text, out int betAmount))
        {
            int currentMoney = PlayerDataManager.GetMoney();

            if (betAmount > currentMoney)
            {
                UpdateTextInfo(errText, "You don't have enough money!");
                betButton.interactable = false;
            }
            else
            {
                UpdateTextInfo(errText, "");
                betButton.interactable = true;
            }
        }
        else
        {
            UpdateTextInfo(errText, "Invalid input!");
            betButton.interactable = false;
        }

        UpdateLoanButtonState();
    }

    public void PlaceBet()
    {
        try
        {
            if (int.TryParse(betInputField.text, out int betAmount))
            {
                int selectedHorseIndex = -1;

                if (uiManager != null)
                {
                    selectedHorseIndex = uiManager.GetSelectedHorseIndex();
                    Debug.Log($"Got selected horse index from UI_Manager: {selectedHorseIndex}");
                }
                else
                {
                    Debug.LogError("UI_Manager is null! Cannot get selected horse index.");
                }

                if (selectedHorseIndex >= 0)
                {
                    if (GameManager.Instance != null)
                    {
                        Debug.Log($"Setting bet in GameManager: amount={betAmount}, horseIndex={selectedHorseIndex}");
                        GameManager.Instance.SetBet(betAmount, selectedHorseIndex);

                        UpdateTextInfo(PlayerMoneyText, "Budget: " + PlayerDataManager.GetMoney() + "$");
                        Debug.Log($"Updated budget display to: {PlayerDataManager.GetMoney()}$");

                        if (uiManager != null)
                        {
                            uiManager.HorseBetToStartRaceTimer();
                            Debug.Log("Transitioning to race start timer screen through UI_Manager");
                        }
                        else
                        {
                            Debug.LogError("Cannot transition to race screen: UI_Manager is null!");
                        }
                    }
                    else
                    {
                        Debug.LogError("GameManager instance is null!");
                        UpdateTextInfo(errText, "Error: Game system error!");
                    }
                }
                else
                {
                    Debug.LogWarning("Неможливо розмістити ставку: не вибрано коня!");
                    UpdateTextInfo(errText, "Please select a horse first!");
                }
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Error in PlaceBet: {ex.Message}");
            UpdateTextInfo(errText, "Error processing bet. Try again.");
        }

        UpdateLoanButtonState();
    }

    public void GetLoan()
    {
        int loanAmount = Random.Range(2, 6) * 100;

        PlayerDataManager.AddMoney(loanAmount);

        UpdateTextInfo(PlayerMoneyText, "Budget: " + PlayerDataManager.GetMoney() + "$");

        UpdateLoanButtonState();

        Debug.Log($"Loan taken: +{loanAmount}$. New balance: {PlayerDataManager.GetMoney()}$");
    }

    private void UpdateLoanButtonState()
    {
        if (LoanButton != null)
        {
            int currentBalance = PlayerDataManager.GetMoney();

            LoanButton.interactable = currentBalance < 100;

            if (currentBalance < 100)
            {
                LoanButton.GetComponentInChildren<TMP_Text>()?.SetText("Take Loan");
            }
            else
            {
                LoanButton.GetComponentInChildren<TMP_Text>()?.SetText("Loan Unavailable");
            }
        }
    }
}
