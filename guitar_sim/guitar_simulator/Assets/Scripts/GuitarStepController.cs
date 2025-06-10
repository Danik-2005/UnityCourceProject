using UnityEngine;
using UnityEngine.UI;

namespace GuitarSimulator
{
    /// <summary>
    /// Контроллер для управления системой шагов гитары
    /// </summary>
    public class GuitarStepController : MonoBehaviour
    {
        [Header("Controller Settings")]
        [SerializeField] private GuitarStepManager stepManager;
        [SerializeField] private StepAction action = StepAction.StartSteps;
        [SerializeField] private int targetStepIndex = 0;
        
        private Button button;
        
        public enum StepAction
        {
            StartSteps,      // Начать выполнение шагов
            StopSteps,       // Остановить выполнение шагов
            PauseSteps,      // Приостановить выполнение шагов
            ResumeSteps,     // Возобновить выполнение шагов
            NextStep,        // Следующий шаг
            PreviousStep,    // Предыдущий шаг
            GoToStep,        // Перейти к конкретному шагу
            SkipStep,        // Пропустить текущий шаг
            ResetSteps       // Сбросить шаги
        }
        
        void Awake()
        {
            button = GetComponent<Button>();
            
            if (button != null)
            {
                button.onClick.AddListener(OnButtonClick);
            }
            
            // Автоматически найти StepManager если не назначен
            if (stepManager == null)
            {
                stepManager = FindObjectOfType<GuitarStepManager>();
            }
        }
        
        void Start()
        {
            if (stepManager == null)
            {
                Debug.LogError("GuitarStepController: GuitarStepManager не найден!");
            }
        }
        
        /// <summary>
        /// Обработчик клика по кнопке
        /// </summary>
        private void OnButtonClick()
        {
            ExecuteAction();
        }
        
        /// <summary>
        /// Выполнить действие контроллера
        /// </summary>
        private void ExecuteAction()
        {
            if (stepManager == null)
            {
                Debug.LogError("GuitarStepController: GuitarStepManager недоступен!");
                return;
            }
            
            switch (action)
            {
                case StepAction.StartSteps:
                    stepManager.StartSteps();
                    break;
                    
                case StepAction.StopSteps:
                    stepManager.StopSteps();
                    break;
                    
                case StepAction.PauseSteps:
                    stepManager.PauseSteps();
                    break;
                    
                case StepAction.ResumeSteps:
                    stepManager.ResumeSteps();
                    break;
                    
                case StepAction.NextStep:
                    stepManager.NextStep();
                    break;
                    
                case StepAction.PreviousStep:
                    stepManager.PreviousStep();
                    break;
                    
                case StepAction.GoToStep:
                    stepManager.GoToStep(targetStepIndex);
                    break;
                    
                case StepAction.SkipStep:
                    stepManager.SkipCurrentStep();
                    break;
                    
                case StepAction.ResetSteps:
                    stepManager.ResetSteps();
                    break;
            }
        }
        
        /// <summary>
        /// Установить действие контроллера
        /// </summary>
        public void SetAction(StepAction newAction)
        {
            action = newAction;
        }
        
        /// <summary>
        /// Установить индекс целевого шага
        /// </summary>
        public void SetTargetStepIndex(int index)
        {
            targetStepIndex = index;
        }
        
        /// <summary>
        /// Установить StepManager
        /// </summary>
        public void SetStepManager(GuitarStepManager manager)
        {
            stepManager = manager;
        }
        
        /// <summary>
        /// Обновить состояние кнопки в зависимости от состояния системы шагов
        /// </summary>
        public void UpdateButtonState()
        {
            if (button == null || stepManager == null) return;
            
            bool interactable = true;
            
            switch (action)
            {
                case StepAction.StartSteps:
                    // Кнопка StartSteps всегда активна (для Freeplay)
                    interactable = true;
                    break;
                    
                case StepAction.StopSteps:
                case StepAction.PauseSteps:
                    interactable = stepManager.IsRunning && !stepManager.IsPaused;
                    break;
                    
                case StepAction.ResumeSteps:
                    interactable = stepManager.IsRunning && stepManager.IsPaused;
                    break;
                    
                case StepAction.NextStep:
                    interactable = stepManager.IsRunning && stepManager.CurrentStepIndex < stepManager.TotalSteps - 1;
                    break;
                    
                case StepAction.PreviousStep:
                    interactable = stepManager.IsRunning && stepManager.CurrentStepIndex > 0;
                    break;
                    
                case StepAction.GoToStep:
                    interactable = stepManager.IsRunning && targetStepIndex >= 0 && targetStepIndex < stepManager.TotalSteps;
                    break;
                    
                case StepAction.SkipStep:
                    interactable = stepManager.IsRunning && stepManager.CurrentStep != null;
                    break;
                    
                case StepAction.ResetSteps:
                    interactable = stepManager.TotalSteps > 0;
                    break;
            }
            
            button.interactable = interactable;
        }
        
        void Update()
        {
            UpdateButtonState();
        }
        
        private void OnDestroy()
        {
            if (button != null)
            {
                button.onClick.RemoveListener(OnButtonClick);
            }
        }
    }
} 