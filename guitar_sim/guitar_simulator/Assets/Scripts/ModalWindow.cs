using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

namespace GuitarSimulator
{
    /// <summary>
    /// Простой скрипт для модальных окон
    /// </summary>
    public class ModalWindow : MonoBehaviour, IPointerClickHandler
    {
        [Header("Modal Settings")]
        [SerializeField] private bool startVisible = false;
        [SerializeField] private bool useAnimation = true;
        [SerializeField] private float animationDuration = 0.3f;
        [SerializeField] private bool closeOnBackgroundClick = true;
        [SerializeField] private bool closeOnEscape = true;
        
        private CanvasGroup canvasGroup;
        private bool isVisible = false;
        private bool isAnimating = false;
        
        void Awake()
        {
            // Получаем или создаем CanvasGroup
            canvasGroup = GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = gameObject.AddComponent<CanvasGroup>();
            }
            
            // Устанавливаем начальное состояние
            SetInitialState();
        }
        
        void Start()
        {
            if (startVisible)
            {
                Show();
            }
            else
            {
                Hide();
            }
        }
        
        void Update()
        {
            // Закрытие по Escape
            if (closeOnEscape && isVisible && Input.GetKeyDown(KeyCode.Escape))
            {
                Hide();
            }
        }
        
        /// <summary>
        /// Показать модальное окно (для OnClick)
        /// </summary>
        public void Show()
        {
            if (isVisible || isAnimating) return;
            
            if (useAnimation)
            {
                StartCoroutine(ShowAnimation());
            }
            else
            {
                SetVisible(true);
            }
        }
        
        /// <summary>
        /// Скрыть модальное окно (для OnClick)
        /// </summary>
        public void Hide()
        {
            if (!isVisible || isAnimating) return;
            
            if (useAnimation)
            {
                StartCoroutine(HideAnimation());
            }
            else
            {
                SetVisible(false);
            }
        }
        
        /// <summary>
        /// Переключить видимость модального окна (для OnClick)
        /// Если окно видимо - скрыть, если скрыто - показать
        /// </summary>
        public void Toggle()
        {
            if (isVisible)
                Hide();
            else
                Show();
        }
        
        /// <summary>
        /// Умный переключатель - показывает окно, если оно скрыто
        /// </summary>
        public void SmartToggle()
        {
            Toggle();
        }
        
        /// <summary>
        /// Обработчик клика по модальному окну
        /// </summary>
        public void OnPointerClick(PointerEventData eventData)
        {
            // Если клик был по фону (не по дочерним элементам)
            if (closeOnBackgroundClick && eventData.pointerCurrentRaycast.gameObject == gameObject)
            {
                Hide();
            }
        }
        
        /// <summary>
        /// Закрыть окно при клике на фон (если включено)
        /// </summary>
        public void OnBackgroundClick()
        {
            if (closeOnBackgroundClick)
            {
                Hide();
            }
        }
        
        /// <summary>
        /// Установить возможность закрытия по клику на фон
        /// </summary>
        public void SetCloseOnBackgroundClick(bool value)
        {
            closeOnBackgroundClick = value;
        }
        
        /// <summary>
        /// Установить возможность закрытия по Escape
        /// </summary>
        public void SetCloseOnEscape(bool value)
        {
            closeOnEscape = value;
        }
        
        private void SetInitialState()
        {
            if (startVisible)
            {
                canvasGroup.alpha = 1f;
                canvasGroup.interactable = true;
                canvasGroup.blocksRaycasts = true;
                isVisible = true;
            }
            else
            {
                canvasGroup.alpha = 0f;
                canvasGroup.interactable = false;
                canvasGroup.blocksRaycasts = false;
                isVisible = false;
            }
        }
        
        private void SetVisible(bool visible)
        {
            isVisible = visible;
            canvasGroup.alpha = visible ? 1f : 0f;
            canvasGroup.interactable = visible;
            canvasGroup.blocksRaycasts = visible;
        }
        
        private IEnumerator ShowAnimation()
        {
            isAnimating = true;
            float elapsedTime = 0f;
            float startAlpha = canvasGroup.alpha;
            
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
            
            while (elapsedTime < animationDuration)
            {
                elapsedTime += Time.deltaTime;
                float progress = elapsedTime / animationDuration;
                canvasGroup.alpha = Mathf.Lerp(startAlpha, 1f, progress);
                yield return null;
            }
            
            canvasGroup.alpha = 1f;
            isVisible = true;
            isAnimating = false;
        }
        
        private IEnumerator HideAnimation()
        {
            isAnimating = true;
            float elapsedTime = 0f;
            float startAlpha = canvasGroup.alpha;
            
            while (elapsedTime < animationDuration)
            {
                elapsedTime += Time.deltaTime;
                float progress = elapsedTime / animationDuration;
                canvasGroup.alpha = Mathf.Lerp(startAlpha, 0f, progress);
                yield return null;
            }
            
            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
            isVisible = false;
            isAnimating = false;
        }
    }
} 