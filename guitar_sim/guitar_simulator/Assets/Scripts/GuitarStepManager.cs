using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.Audio;

namespace GuitarSimulator
{
    /// <summary>
    /// Система шагов для гитары с поддержкой аудио и состояний
    /// </summary>
    public class GuitarStepManager : MonoBehaviour
    {
        [System.Serializable]
        public class GuitarStep
        {
            [Header("Step Info")]
            public string stepName = "Step";
            public string description = "";
            
            [Header("Step Settings")]
            public float delay = 0f;
            public bool waitForInput = false;
            public bool autoProceed = true;
            public bool isOptional = false;
        }
        
        [Header("Guitar Step System Settings")]
        [SerializeField] private bool autoStart = false;
        [SerializeField] private bool loopSteps = false;
        [SerializeField] private float stepDelay = 0.5f;
        
        [Header("UI References")]
        [SerializeField] private TextMeshProUGUI statusText;
        [SerializeField] private TextMeshProUGUI stepInfoText;
        
        [Header("Steps")]
        [SerializeField] private List<GuitarStep> steps = new List<GuitarStep>();
        
        private int currentStepIndex = -1;
        private bool isRunning = false;
        private bool isPaused = false;
        private Coroutine stepCoroutine;
        private HashSet<int> completedSteps = new HashSet<int>(); // Отслеживаем выполненные шаги
        private int lastCompletedStepIndex = -1; // Последний выполненный шаг
        
        public int CurrentStepIndex => currentStepIndex;
        public int TotalSteps => steps.Count;
        public bool IsRunning => isRunning;
        public bool IsPaused => isPaused;
        public GuitarStep CurrentStep => currentStepIndex >= 0 && currentStepIndex < steps.Count ? steps[currentStepIndex] : null;
        
        void Awake()
        {
            // Инициализация
        }
        
        void Start()
        {
            // Инициализируем шаги
            InitializeSteps();
            
            // Обновляем UI сразу при старте
            UpdateUI();
            
            if (autoStart)
            {
                StartSteps();
            }
        }
        
        /// <summary>
        /// Начать выполнение шагов (для OnClick)
        /// </summary>
        public void StartSteps()
        {
            if (isRunning)
            {
                Debug.LogWarning("GuitarStepManager: Шаги уже выполняются!");
                return;
            }
            
            if (steps.Count == 0)
            {
                Debug.LogWarning("GuitarStepManager: Нет шагов для выполнения!");
                return;
            }
            
            currentStepIndex = -1;
            isRunning = true;
            isPaused = false;
            completedSteps.Clear(); // Сбрасываем выполненные шаги
            lastCompletedStepIndex = -1; // Сбрасываем последний выполненный шаг
            
            stepCoroutine = StartCoroutine(ExecuteSteps());
            
            Debug.Log("GuitarStepManager: Начато выполнение шагов");
            UpdateUI();
        }
        
        /// <summary>
        /// Остановить выполнение шагов (для OnClick)
        /// </summary>
        public void StopSteps()
        {
            if (!isRunning) return;
            
            isRunning = false;
            isPaused = false;
            
            if (stepCoroutine != null)
            {
                StopCoroutine(stepCoroutine);
                stepCoroutine = null;
            }
            
            Debug.Log("GuitarStepManager: Выполнение шагов остановлено");
            UpdateUI();
        }
        
        /// <summary>
        /// Приостановить выполнение шагов (для OnClick)
        /// </summary>
        public void PauseSteps()
        {
            if (!isRunning) return;
            
            isPaused = true;
            Debug.Log("GuitarStepManager: Выполнение шагов приостановлено");
            UpdateUI();
        }
        
        /// <summary>
        /// Возобновить выполнение шагов (для OnClick)
        /// </summary>
        public void ResumeSteps()
        {
            if (!isRunning || !isPaused) return;
            
            isPaused = false;
            Debug.Log("GuitarStepManager: Выполнение шагов возобновлено");
            UpdateUI();
        }
        
        /// <summary>
        /// Перейти к следующему шагу (для OnClick)
        /// </summary>
        public void NextStep()
        {
            Debug.Log($"GuitarStepManager: NextStep called. IsRunning: {isRunning}, CurrentStepIndex: {currentStepIndex}, TotalSteps: {steps.Count}");
            
            if (!isRunning) 
            {
                Debug.LogWarning("GuitarStepManager: Cannot proceed - system is not running!");
                return;
            }
            
            if (currentStepIndex < steps.Count - 1)
            {
                currentStepIndex++;
                ExecuteCurrentStep();
                Debug.Log($"GuitarStepManager: Moved to step {currentStepIndex + 1}: {steps[currentStepIndex].stepName}");
            }
            else if (loopSteps)
            {
                currentStepIndex = 0;
                ExecuteCurrentStep();
                Debug.Log($"GuitarStepManager: Looped to step 1: {steps[currentStepIndex].stepName}");
            }
            else
            {
                CompleteSteps();
            }
            
            UpdateUI();
        }
        
        /// <summary>
        /// Перейти к предыдущему шагу (для OnClick)
        /// </summary>
        public void PreviousStep()
        {
            if (!isRunning || currentStepIndex <= 0) 
            {
                Debug.LogWarning("GuitarStepManager: Cannot go back - system not running or already at first step!");
                return;
            }
            
            currentStepIndex--;
            ExecuteCurrentStep();
            UpdateUI();
        }
        
        /// <summary>
        /// Перейти к конкретному шагу (для OnClick)
        /// </summary>
        public void GoToStep(int stepIndex)
        {
            if (!isRunning || stepIndex < 0 || stepIndex >= steps.Count) return;
            
            currentStepIndex = stepIndex;
            ExecuteCurrentStep();
            UpdateUI();
        }
        
        /// <summary>
        /// Пропустить текущий шаг (для OnClick)
        /// </summary>
        public void SkipCurrentStep()
        {
            if (!isRunning || currentStepIndex < 0 || currentStepIndex >= steps.Count) return;
            
            NextStep();
        }
        
        /// <summary>
        /// Сбросить систему шагов (для OnClick)
        /// </summary>
        public void ResetSteps()
        {
            StopSteps();
            currentStepIndex = -1;
            completedSteps.Clear();
            UpdateUI();
        }
        
        /// <summary>
        /// Завершить выполнение шагов
        /// </summary>
        private void CompleteSteps()
        {
            isRunning = false;
            isPaused = false;
            
            if (stepCoroutine != null)
            {
                StopCoroutine(stepCoroutine);
                stepCoroutine = null;
            }
            
            UpdateUI();
        }
        
        /// <summary>
        /// Получить прогресс выполнения шагов
        /// </summary>
        public float GetProgress()
        {
            if (steps.Count == 0) return 0f;
            return (float)(currentStepIndex + 1) / steps.Count;
        }
        
        /// <summary>
        /// Обновить UI
        /// </summary>
        private void UpdateUI()
        {
            // Обновляем текстовые поля
            if (statusText != null)
            {
                if (!isRunning)
                {
                    statusText.text = "Mironov Danila";
                }
                else if (isPaused)
                {
                    statusText.text = "Paused";
                }
                else
                {
                    statusText.text = $"Current step: \n{CurrentStep.stepName}";
                }
            }
            else
            {
                Debug.LogWarning("Status Text не назначен!");
            }
            
            if (stepInfoText != null && CurrentStep != null)
            {
                stepInfoText.text = $"{CurrentStep.stepName}\n{CurrentStep.description}";
                Debug.Log($"Step Info: {CurrentStep.stepName} - {CurrentStep.description}");
            }
            else if (stepInfoText != null)
            {
                stepInfoText.text = "Нет активного шага";
                Debug.Log("Step Info: Нет активного шага");
            }
            else
            {
                Debug.LogWarning("Step Info Text не назначен!");
            }
        }
        
        private IEnumerator ExecuteSteps()
        {
            Debug.Log("ExecuteSteps started");
            
            while (isRunning)
            {
                if (isPaused)
                {
                    yield return null;
                    continue;
                }
                
                // Если это первый запуск, начинаем с первого шага
                if (currentStepIndex == -1)
                {
                    currentStepIndex = 0;
                    ExecuteCurrentStep();
                    UpdateUI();
                    Debug.Log($"First step executed: {currentStepIndex}");
                    
                    // Ждем действия пользователя для первого шага
                    GuitarStep currentStep = steps[currentStepIndex];
                    if (currentStep.waitForInput || !currentStep.autoProceed)
                    {
                        Debug.Log("Waiting for user input on first step");
                        while (isRunning && isPaused == false)
                        {
                            yield return null;
                        }
                    }
                }
                
                if (currentStepIndex < steps.Count - 1)
                {
                    currentStepIndex++;
                    ExecuteCurrentStep();
                    UpdateUI();
                    
                    GuitarStep currentStep = steps[currentStepIndex];
                    
                    // Ждем указанную задержку
                    if (currentStep.delay > 0)
                    {
                        yield return new WaitForSeconds(currentStep.delay);
                    }
                    
                    // Если шаг требует ожидания ввода или автопродолжение выключено
                    if (currentStep.waitForInput || !currentStep.autoProceed)
                    {
                        Debug.Log($"Waiting for user input on step {currentStepIndex}: {currentStep.stepName}");
                        while (isRunning && isPaused == false)
                        {
                            yield return null;
                        }
                    }
                    else if (currentStep.autoProceed)
                    {
                        // Автоматическое продолжение только если включено
                        yield return new WaitForSeconds(stepDelay);
                    }
                }
                else if (loopSteps)
                {
                    currentStepIndex = 0;
                    ExecuteCurrentStep();
                    UpdateUI();
                    yield return new WaitForSeconds(stepDelay);
                }
                else
                {
                    CompleteSteps();
                    break;
                }
            }
        }
        
        private void ExecuteCurrentStep()
        {
            if (currentStepIndex < 0 || currentStepIndex >= steps.Count) return;
            
            GuitarStep step = steps[currentStepIndex];
            
            Debug.Log($"GuitarStepManager: Выполняется шаг {currentStepIndex + 1}/{steps.Count}: {step.stepName}");
        }

        private void InitializeSteps()
        {
            steps = new List<GuitarStep>
            {
                
                new GuitarStep
                {
                    stepName = "Enable amplifier",
                    description = "Connect the amplifier to the power",
                    autoProceed = false,
                    waitForInput = true,
                    delay = 0f
                },
                new GuitarStep
                {
                    stepName = "Connect guitar",
                    description = "Take the Jack cabel and connect it to the guitar",
                    autoProceed = false,
                    waitForInput = true,
                    delay = 0f
                },
                new GuitarStep
                {
                    stepName = "Start playing",
                    description = "Yeee, you are ready to play",
                    autoProceed = false,
                    waitForInput = true,
                    delay = 0f
                }
            };
        }

        /// <summary>
        /// Зарегистрировать подключение (для PlugConnector)
        /// </summary>
        public void RegisterConnection(int stepIndex)
        {
            Debug.Log($"GuitarStepManager: RegisterConnection called for step {stepIndex}, current step: {currentStepIndex}");
            
            // Отмечаем шаг как выполненный
            completedSteps.Add(stepIndex);
            lastCompletedStepIndex = Mathf.Max(lastCompletedStepIndex, stepIndex);
            
            Debug.Log($"GuitarStepManager: Step {stepIndex} marked as completed. Last completed: {lastCompletedStepIndex}");
            
            // Проверяем, можем ли перейти к следующему шагу
            CheckAndProceedToNextStep();
        }
        
        /// <summary>
        /// Проверить и перейти к следующему шагу если возможно
        /// </summary>
        private void CheckAndProceedToNextStep()
        {
            // Проверяем, выполнен ли текущий шаг
            if (completedSteps.Contains(currentStepIndex))
            {
                // Ищем следующий невыполненный шаг
                for (int i = currentStepIndex + 1; i < steps.Count; i++)
                {
                    if (!completedSteps.Contains(i))
                    {
                        // Переходим к следующему невыполненному шагу
                        currentStepIndex = i;
                        ExecuteCurrentStep();
                        UpdateUI();
                        Debug.Log($"GuitarStepManager: Moved to next uncompleted step {i}");
                        return;
                    }
                }
                
                // Если все шаги выполнены
                CompleteSteps();
                Debug.Log("GuitarStepManager: All steps completed!");
            }
            else
            {
                Debug.Log($"GuitarStepManager: Current step {currentStepIndex} not completed yet, staying here");
            }
        }
        
        /// <summary>
        /// Зарегистрировать отключение (для PlugConnector)
        /// </summary>
        public void RegisterDisconnection(int stepIndex)
        {
            Debug.Log($"GuitarStepManager: RegisterDisconnection called for step {stepIndex}, current step: {currentStepIndex}");
            
            // Убираем шаг из выполненных
            completedSteps.Remove(stepIndex);
            
            Debug.Log($"GuitarStepManager: Step {stepIndex} removed from completed");
            
            // Находим первый невыполненный шаг
            int firstUncompletedStep = -1;
            for (int i = 0; i < steps.Count; i++)
            {
                if (!completedSteps.Contains(i))
                {
                    firstUncompletedStep = i;
                    break;
                }
            }
            
            // Переходим к первому невыполненному шагу
            if (firstUncompletedStep >= 0)
            {
                currentStepIndex = firstUncompletedStep;
                ExecuteCurrentStep();
                UpdateUI();
                Debug.Log($"GuitarStepManager: Returned to first uncompleted step {firstUncompletedStep}");
            }
            else
            {
                // Если все шаги выполнены, остаемся на текущем
                Debug.Log("GuitarStepManager: All steps still completed, staying on current step");
            }
        }
    }
} 