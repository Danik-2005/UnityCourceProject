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
        }
        
        /// <summary>
        /// Автоматически выполняется при клике на кнопку
        /// </summary>
        private void OnButtonClick()
        {
            if (targetWindow == null) return;
            
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
        
        private void OnDestroy()
        {
            if (button != null)
            {
                button.onClick.RemoveListener(OnButtonClick);
            }
        }
    }
} 