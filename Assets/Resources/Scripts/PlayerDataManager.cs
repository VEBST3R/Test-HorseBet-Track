using UnityEngine;

namespace HorseBet.Gameplay
{
    public static class PlayerDataManager
    {
        private const string MONEY_KEY = "money";
        private const string WINS_KEY = "wins";
        private const string LOSSES_KEY = "losses";
        private const string SELECTED_HORSE_KEY = "SelectedHorse";
        private const string CURRENT_BET_KEY = "CurrentBet";

        public static int GetMoney()
        {
            return PlayerPrefs.GetInt(MONEY_KEY, 300);
        }

        public static void SetMoney(int value)
        {
            if (value < 0) value = 0;
            PlayerPrefs.SetInt(MONEY_KEY, value);
            PlayerPrefs.Save();
            Debug.Log($"Player balance updated to: {value}$");
        }

        public static void AddMoney(int amount)
        {
            int currentMoney = GetMoney();
            SetMoney(currentMoney + amount);
            Debug.Log($"Added {amount}$ to player balance. New balance: {GetMoney()}$");
        }

        public static bool TrySpendMoney(int amount)
        {
            int currentMoney = GetMoney();
            if (amount <= currentMoney)
            {
                SetMoney(currentMoney - amount);
                Debug.Log($"Spent {amount}$. New balance: {GetMoney()}$");
                return true;
            }
            else
            {
                Debug.LogWarning($"Not enough money! Required: {amount}$, available: {currentMoney}$");
                return false;
            }
        }

        public static int GetWins()
        {
            return PlayerPrefs.GetInt(WINS_KEY, 0);
        }

        public static void AddWin()
        {
            int currentWins = GetWins();
            PlayerPrefs.SetInt(WINS_KEY, currentWins + 1);
            PlayerPrefs.Save();
            Debug.Log($"Win added. Total wins: {GetWins()}");
        }

        public static int GetLosses()
        {
            return PlayerPrefs.GetInt(LOSSES_KEY, 0);
        }

        public static void AddLoss()
        {
            int currentLosses = GetLosses();
            PlayerPrefs.SetInt(LOSSES_KEY, currentLosses + 1);
            PlayerPrefs.Save();
            Debug.Log($"Loss added. Total losses: {GetLosses()}");
        }

        public static void SetSelectedHorse(int index)
        {
            PlayerPrefs.SetInt(SELECTED_HORSE_KEY, index);
            PlayerPrefs.Save();
        }

        public static int GetSelectedHorse()
        {
            return PlayerPrefs.GetInt(SELECTED_HORSE_KEY, -1);
        }

        public static void SetCurrentBet(int amount)
        {
            Debug.Log($"Setting current bet to: {amount}$");
            PlayerPrefs.SetInt(CURRENT_BET_KEY, amount);
            PlayerPrefs.Save();
        }

        public static int GetCurrentBet()
        {
            return PlayerPrefs.GetInt(CURRENT_BET_KEY, 0);
        }

        public static void InitializePlayerData()
        {
            if (!PlayerPrefs.HasKey(MONEY_KEY))
            {
                SetMoney(300);
                PlayerPrefs.SetInt(WINS_KEY, 0);
                PlayerPrefs.SetInt(LOSSES_KEY, 0);
                PlayerPrefs.Save();
                Debug.Log("Player data initialized with default values");
            }
        }

        public static void ResetPlayerData()
        {
            SetMoney(300);
            PlayerPrefs.SetInt(WINS_KEY, 0);
            PlayerPrefs.SetInt(LOSSES_KEY, 0);
            PlayerPrefs.SetInt(SELECTED_HORSE_KEY, -1);
            PlayerPrefs.SetInt(CURRENT_BET_KEY, 0);
            PlayerPrefs.Save();
            Debug.Log("Player data reset to default values");
        }
    }
}
