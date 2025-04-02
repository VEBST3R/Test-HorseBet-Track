using UnityEngine;
using UnityEngine.UI;
using TMPro;
using HorseBet.Gameplay;
using HorseBet.Camera;

namespace HorseBet.UI
{
    public class UI_Manager : MonoBehaviour
    {
        private static UI_Manager _instance;
        public static UI_Manager Instance { get { return _instance; } }

        [SerializeField] private GameObject MainMenu;
        [SerializeField] private GameObject HorseChooseMenu;
        [SerializeField] private GameObject HorseBetMenu;
        [SerializeField] private GameObject HorseBetResultMenu;
        [SerializeField] private GameObject HorseRaceStartTimer;

        [SerializeField] private Animator MainMenuAnimator;
        [SerializeField] private Animator HorseChooseMenuAnimator;
        [SerializeField] private Animator HorseBetMenuAnimator;
        [SerializeField] private Animator HorseBetResultMenuAnimator;

        [Header("Horse Selection")]
        [SerializeField] private Button horse1Button;
        [SerializeField] private Button horse2Button;
        [SerializeField] private Button horse3Button;
        [SerializeField] private Color selectedHorseColor = Color.green;
        [SerializeField] private Color defaultHorseColor = Color.white;

        [SerializeField] private TMP_InputField betAmountInput;

        private int selectedHorseIndex = -1;

        [Header("Race Reset")]
        [SerializeField] private Vector3[] horseStartPositions;
        [SerializeField] private Transform horsesParent;

        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
            }
            else if (_instance != this)
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            ResetHorseSelection();
        }

        public void SelectHorse1()
        {
            Debug.Log("SelectHorse1 called");
            SelectHorse(0);
        }

        public void SelectHorse2()
        {
            Debug.Log("SelectHorse2 called");
            SelectHorse(1);
        }

        public void SelectHorse3()
        {
            Debug.Log("SelectHorse3 called");
            SelectHorse(2);
        }

        private void SelectHorse(int index)
        {
            PlayerDataManager.SetSelectedHorse(index);

            if (GameManager.Instance != null)
            {
                GameManager.Instance.CurrentHorse = index;
            }

            selectedHorseIndex = index;

            ResetHorseButtonColors();
            Button selectedButton = null;
            switch (index)
            {
                case 0: selectedButton = horse1Button; break;
                case 1: selectedButton = horse2Button; break;
                case 2: selectedButton = horse3Button; break;
            }

            if (selectedButton != null)
            {
                ColorBlock colors = selectedButton.colors;
                colors.normalColor = selectedHorseColor;
                colors.selectedColor = selectedHorseColor;
                selectedButton.colors = colors;
            }
        }

        private void ResetHorseButtonColors()
        {
            SetButtonColor(horse1Button, defaultHorseColor);
            SetButtonColor(horse2Button, defaultHorseColor);
            SetButtonColor(horse3Button, defaultHorseColor);
        }

        private void SetButtonColor(Button button, Color color)
        {
            if (button != null)
            {
                ColorBlock colors = button.colors;
                colors.normalColor = color;
                colors.selectedColor = color;
                button.colors = colors;
            }
        }

        private void ResetHorseSelection()
        {
            selectedHorseIndex = -1;
            ResetHorseButtonColors();
        }

        public int GetSelectedHorseIndex()
        {
            if (selectedHorseIndex >= 0)
            {
                return selectedHorseIndex;
            }

            int savedIndex = PlayerDataManager.GetSelectedHorse();
            if (savedIndex >= 0)
            {
                selectedHorseIndex = savedIndex;
                return savedIndex;
            }

            return -1;
        }

        public void PlaceBet()
        {
            Debug.Log($"PlaceBet called with selectedHorseIndex={selectedHorseIndex}");

            if (selectedHorseIndex >= 0 && betAmountInput != null && int.TryParse(betAmountInput.text, out int betAmount))
            {
                if (GameManager.Instance != null)
                {
                    Debug.Log($"Setting bet: {betAmount}$ on horse index {selectedHorseIndex}");

                    GameManager.Instance.SetBet(betAmount, selectedHorseIndex);

                    HorseBetToStartRaceTimer();
                }
            }
            else
            {
                string errorMessage = "";
                if (selectedHorseIndex < 0)
                    errorMessage += "No horse selected. ";
                if (betAmountInput == null)
                    errorMessage += "Bet input field is null. ";
                if (betAmountInput != null && !int.TryParse(betAmountInput.text, out _))
                    errorMessage += $"Invalid bet amount: '{betAmountInput.text}'. ";

                Debug.LogWarning($"Cannot place bet: {errorMessage}");
            }
        }

        public void MenuToHorseChoose()
        {
            MainMenuAnimator.SetTrigger("Exit");
            HorseChooseMenu.SetActive(true);
        }

        public void HorseChooseToMainMenu()
        {
            HorseChooseMenuAnimator.SetTrigger("Exit");
            MainMenu.SetActive(true);
        }

        public void HorseChooseToHorseBet()
        {
            ResetHorseSelection();

            HorseChooseMenuAnimator.SetTrigger("Exit");
            HorseBetMenu.SetActive(true);
        }

        public void HorseBetToHorseChoose()
        {
            HorseBetMenuAnimator.SetTrigger("Exit");
            HorseChooseMenu.SetActive(true);
        }

        public void HorseBetToStartRaceTimer()
        {
            HorseBetMenuAnimator.SetTrigger("Exit");
            HorseRaceStartTimer.SetActive(true);
        }

        public void ShowRaceResults()
        {
            if (HorseBetResultMenu != null)
                HorseBetResultMenu.SetActive(true);
        }

        public void RestartRace()
        {
            Debug.Log("Restarting race...");

            if (CameraManager.Instance != null)
            {
                CameraManager.Instance.TransitionToRaceCamera();
            }

            if (GameManager.Instance != null)
            {
                GameManager.Instance.RestartRace();
            }

            ResetHorsePositions();

            if (HorseChooseMenu != null)
            {
                if (MainMenu != null) MainMenu.SetActive(false);
                if (HorseBetMenu != null) HorseBetMenu.SetActive(false);
                if (HorseRaceStartTimer != null) HorseRaceStartTimer.SetActive(false);

                HorseChooseMenu.SetActive(true);
            }

            ResetHorseSelection();
        }

        public void ReturnToMainMenu()
        {
            Debug.Log("Returning to main menu...");

            if (CameraManager.Instance != null)
            {
                CameraManager.Instance.TransitionToRaceCamera();
            }

            if (GameManager.Instance != null)
            {
                GameManager.Instance.RestartRace();
            }

            ResetHorsePositions();

            if (HorseBetResultMenu != null)
                HorseBetResultMenu.SetActive(false);

            if (MainMenu != null)
            {
                if (HorseChooseMenu != null) HorseChooseMenu.SetActive(false);
                if (HorseBetMenu != null) HorseBetMenu.SetActive(false);
                if (HorseRaceStartTimer != null) HorseRaceStartTimer.SetActive(false);

                MainMenu.SetActive(true);
            }

            ResetHorseSelection();
        }

        private void ResetHorsePositions()
        {
            Horse[] horses = FindObjectsByType<Horse>(FindObjectsSortMode.None);

            Debug.Log($"Resetting positions for {horses.Length} horses");

            foreach (Horse horse in horses)
            {
                horse.enabled = false;

                string horseName = horse.gameObject.name;
                int horseIndex = -1;

                if (horseName.Contains("Horse_1")) horseIndex = 0;
                else if (horseName.Contains("Horse_2")) horseIndex = 1;
                else if (horseName.Contains("Horse_3")) horseIndex = 2;

                if (horseIndex >= 0 && horseIndex < horseStartPositions.Length)
                {
                    Vector3 startPosition = horseStartPositions[horseIndex];

                    startPosition.y = horse.transform.position.y;

                    Debug.Log($"Resetting {horseName} to position: {startPosition}");
                    horse.transform.position = startPosition;

                    horse.transform.rotation = Quaternion.Euler(0, 0, 0);
                }
                else
                {
                    Debug.LogWarning($"No starting position found for horse: {horseName}");
                }

                Animator animator = horse.gameObject.GetComponentInChildren<Animator>();
                if (animator != null)
                {
                    animator.Rebind();
                    animator.Update(0f);
                    if (animator.parameters.Length > 0)
                    {
                        foreach (var param in animator.parameters)
                        {
                            if (param.name == "Idle" || param.name == "Reset")
                            {
                                animator.SetTrigger(param.name);
                                break;
                            }
                        }
                    }
                }
            }

            FinishCollider finishCollider = FindFirstObjectByType<FinishCollider>();
            if (finishCollider != null)
            {
                finishCollider.ResetFinishCollider();
            }

            if (CameraManager.Instance != null)
            {
                CameraManager.Instance.SetRaceCameraIdle();
            }
        }

        public void ResetHorsePositionsPublic()
        {
            ResetHorsePositions();
        }
    }
}