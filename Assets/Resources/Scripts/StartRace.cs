using UnityEngine;

public class StartRace : MonoBehaviour
{
    public void _StartRace()
    {
        if (GameManager.Instance == null)
        {
            Debug.LogError("GameManager.Instance є null! Перевірте, чи правильно ініціалізовано GameManager.");
            return;
        }

        if (GameManager.Instance.CurrentHorse < 0)
        {
            Debug.LogWarning("Не вибрано коня для ставки! CurrentHorse = " + GameManager.Instance.CurrentHorse);
        }

        Debug.Log($"Викликаю GameManager.StartRace() з CurrentHorse={GameManager.Instance.CurrentHorse}, CurrentBet={GameManager.Instance.CurrentBet}");
        GameManager.Instance.StartRace();
    }
}