using UnityEngine;
using UnityEngine.UI;

namespace GuitarSimulator
{
    /// <summary>
    /// Максимально простой скрипт для кнопок модальных окон
    /// </summary>
    public class SimpleModalButton : MonoBehaviour
    {
        [Header("Button Settings")]
        [SerializeField] private ModalWindow targetWindow;
        [SerializeField] private ButtonAction action = ButtonAction.Toggle;
        [SerializeField] private bool keepButtonActive = true; // Держать кнопку активной
        
        private Button button;
        
        public enum ButtonAction
        {
            Show,       // Показать окно
            Hide,       // Скрыть окно
            Toggle      // Переключить окно (открыть/закрыть)
        }
        
        void Awake()
        {
            button = GetComponent<Button>();
            
            if (button != null)
            {
                button.onClick.AddListener(OnButtonClick);
            }
        }
        
        void Start()
        {
            if (targetWindow == null)
            {
                Debug.LogWarning("SimpleModalButton: Не назначено целевое окно!");
            }
            
            // Убеждаемся, что кнопка активна при старте
            EnsureButtonActive();
        }
        
        void Update()
        {
            // Постоянно проверяем, что кнопка активна (если включена опция)
            if (keepButtonActive)
            {
                EnsureButtonActive();
            }
        }
        
        /// <summary>
        /// Автоматически выполняется при клике на кнопку
        /// </summary>
        private void OnButtonClick()
        {
            if (targetWindow == null) return;
            
            Debug.Log($"[SimpleModalButton] Button clicked on {gameObject.name}, action: {action}");
            
            switch (action)
            {
                case ButtonAction.Show:
                    targetWindow.Show();
                    break;
                    
                case ButtonAction.Hide:
                    targetWindow.Hide();
                    break;
                    
                case ButtonAction.Toggle:
                    targetWindow.Toggle();
                    break;
            }
            
            // Убеждаемся, что кнопка остается активной после клика
            if (keepButtonActive)
            {
                StartCoroutine(EnsureButtonActiveAfterFrame());
            }
        }
        
        /// <summary>
        /// Убедиться, что кнопка активна
        /// </summary>
        private void EnsureButtonActive()
        {
            if (button != null && !button.interactable)
            {
                Debug.LogWarning($"[SimpleModalButton] Button {gameObject.name} was disabled, re-enabling...");
                button.interactable = true;
            }
        }
        
        /// <summary>
        /// Убедиться, что кнопка активна после кадра (для случаев, когда что-то отключает её)
        /// </summary>
        private System.Collections.IEnumerator EnsureButtonActiveAfterFrame()
        {
            yield return null; // Ждем один кадр
            EnsureButtonActive();
        }
        
        /// <summary>
        /// Установить целевое окно
        /// </summary>
        public void SetTargetWindow(ModalWindow window)
        {
            targetWindow = window;
        }
        
        /// <summary>
        /// Установить действие кнопки
        /// </summary>
        public void SetAction(ButtonAction newAction)
        {
            action = newAction;
        }
        
        /// <summary>
        /// Принудительно активировать кнопку
        /// </summary>
        public void ForceActivate()
        {
            if (button != null)
            {
                button.interactable = true;
                Debug.Log($"[SimpleModalButton] Button {gameObject.name} force activated");
            }
        }
        
        /// <summary>
        /// Получить текущее состояние кнопки
        /// </summary>
        public bool IsButtonActive()
        {
            return button != null && button.interactable;
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