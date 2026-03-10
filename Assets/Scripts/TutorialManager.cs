using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace ZyroX
{
    public class TutorialManager : MonoBehaviour
    {
        public static TutorialManager Instance { get; private set; }
        public List<TutorialStep> Steps ;

        private const string PlayerPrefsKey = "CompletedTutorials";
        public GameObject TutorialGroup;
        public Transform Pointer;
        public TextMeshProUGUI DescriptionText;
        [SerializeField] private int currentStepIndex = 0;
        private Action currentOnComplete;
        private int currentTutorialId = 0;

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                Load();
            }
            else if (Instance != this)
            {
                Destroy(gameObject);
            }
            TutorialGroup.SetActive(false);
        }

        private void Load()
        {
            currentStepIndex = PlayerPrefs.GetInt(PlayerPrefsKey , -1);
        }

        public void MarkCompleted()
        {
            PlayerPrefs.SetInt(PlayerPrefsKey , currentTutorialId);
            PlayerPrefs.Save();
            TutorialGroup.SetActive(false);
            currentOnComplete?.Invoke();
        }

        public bool IsCompleted(int id)
        {
            Debug.Log($"Checking if tutorial {id} is completed. Last completed: {PlayerPrefs.GetInt(PlayerPrefsKey , -1)}");
            return id <= PlayerPrefs.GetInt(PlayerPrefsKey , -1);
        }

        public void ShowTutorial(int id, Action onComplete = null)
        {
            var step = Steps.FirstOrDefault(s => s.Id == id);
            if (step.Position != null)
            {
                Pointer.position = step.Position.position;
            }
            DescriptionText.text = step.Description;
            TutorialGroup.SetActive(true);
            currentOnComplete = onComplete;
            currentTutorialId = id;
        }

        public void NextTutorial()
        {
            ShowTutorial(currentStepIndex + 1);
        }
    }

    [System.Serializable]
    public struct TutorialStep
    {
        public int Id;
        public string Description;
        public Transform Position;
    }
}
