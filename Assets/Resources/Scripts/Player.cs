using UnityEngine;

namespace HorseBet.Gameplay
{
    public class Player : MonoBehaviour
    {
        public int money
        {
            get { return PlayerDataManager.GetMoney(); }
            set { PlayerDataManager.SetMoney(value); }
        }

        public int wins
        {
            get { return PlayerDataManager.GetWins(); }
            set { PlayerDataManager.AddWin(); }
        }

        public int losses
        {
            get { return PlayerDataManager.GetLosses(); }
            set { PlayerDataManager.AddLoss(); }
        }

        private void Start()
        {
            PlayerDataManager.InitializePlayerData();
        }

        [System.Obsolete("Use PlayerDataManager methods instead")]
        public void UpdateData(int money, int wins, int losses)
        {
            PlayerDataManager.SetMoney(money);

            PlayerPrefs.SetInt("wins", wins);
            PlayerPrefs.SetInt("losses", losses);
            PlayerPrefs.Save();

            Debug.LogWarning("UpdateData is obsolete. Use PlayerDataManager methods instead.");
        }

        private void OnApplicationQuit()
        {
            PlayerPrefs.Save();
        }
    }
}
