using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

namespace GuitarSimulator
{
    /// <summary>
    /// Скрипт для подключения штекера к вилке
    /// </summary>
    public class PlugConnector : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [Header("Connection Settings")]
        [SerializeField] private Transform targetPosition;
        [SerializeField] private float connectionSpeed = 2f;
        [SerializeField] private float connectionDistance = 0.1f;
        [SerializeField] private bool isConnected = false;
        [SerializeField] private bool isDraggable = true;
        
        [Header("Audio Settings")]
        [SerializeField] private AudioClip connectionSound;
        [SerializeField] private AudioClip disconnectSound;
        
        [Header("Step System Integration")]
        [SerializeField] private GuitarStepManager stepManager;
        [SerializeField] private int targetStepIndex = -1; // Индекс шага подключения
        
        [Header("Connection Manager Integration")]
        [SerializeField] private ConnectionType connectionType = ConnectionType.Guitar;
        
        [Header("Visual Feedback")]
        [SerializeField] private Material connectedMaterial;
        [SerializeField] private Material disconnectedMaterial;
        [SerializeField] private Renderer targetRenderer;
        
        private AudioSource audioSource;
        private Vector3 originalPosition;
        private Quaternion originalRotation;
        private Transform originalParent;
        private bool isDragging = false;
        private bool isConnecting = false;
        private Coroutine connectionCoroutine;
        
        public bool IsConnected => isConnected;
        public Transform TargetPosition => targetPosition;
        
        public enum ConnectionType
        {
            Guitar,     // Подключение гитары
            Amplifier   // Подключение усилителя
        }
        
        void Awake()
        {
            // Получаем или создаем AudioSource
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }
            
            // Сохраняем исходную позицию и поворот
            originalPosition = transform.position;
            originalRotation = transform.rotation;
            originalParent = transform.parent;
            
            // Автоматически найти StepManager если не назначен
            if (stepManager == null)
            {
                stepManager = FindObjectOfType<GuitarStepManager>();
            }
        }
        
        void Start()
        {
            // Устанавливаем начальный материал
            UpdateVisualState();
        }
        
        /// <summary>
        /// Обработчик клика по штекеру
        /// </summary>
        public void OnPointerClick(PointerEventData eventData)
        {
            if (!isDragging && !isConnecting)
            {
                if (isConnected)
                {
                    Disconnect();
                }
                else
                {
                    Connect();
                }
            }
        }
        
        /// <summary>
        /// Начало перетаскивания
        /// </summary>
        public void OnBeginDrag(PointerEventData eventData)
        {
            if (!isDraggable || isConnecting) return;
            
            isDragging = true;
            
            // Отключаем от родителя для свободного перемещения
            if (isConnected)
            {
                Disconnect();
            }
        }
        
        /// <summary>
        /// Перетаскивание
        /// </summary>
        public void OnDrag(PointerEventData eventData)
        {
            if (!isDragging) return;
            
            // Получаем позицию в мировых координатах
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(new Vector3(
                eventData.position.x, 
                eventData.position.y, 
                Camera.main.WorldToScreenPoint(transform.position).z
            ));
            
            transform.position = worldPosition;
        }
        
        /// <summary>
        /// Конец перетаскивания
        /// </summary>
        public void OnEndDrag(PointerEventData eventData)
        {
            if (!isDragging) return;
            
            isDragging = false;
            
            // Проверяем, близко ли к цели
            if (targetPosition != null && Vector3.Distance(transform.position, targetPosition.position) < connectionDistance)
            {
                Connect();
            }
        }
        
        /// <summary>
        /// Подключить штекер
        /// </summary>
        public void Connect()
        {
            if (isConnected || isConnecting || targetPosition == null) return;
            
            isConnecting = true;
            
            if (connectionCoroutine != null)
            {
                StopCoroutine(connectionCoroutine);
            }
            
            connectionCoroutine = StartCoroutine(ConnectCoroutine());
        }
        
        /// <summary>
        /// Отключить штекер
        /// </summary>
        public void Disconnect()
        {
            if (!isConnected || isConnecting) return;
            
            isConnecting = true;
            
            if (connectionCoroutine != null)
            {
                StopCoroutine(connectionCoroutine);
            }
            
            connectionCoroutine = StartCoroutine(DisconnectCoroutine());
        }
        
        /// <summary>
        /// Корутина подключения
        /// </summary>
        private IEnumerator ConnectCoroutine()
        {
            Vector3 startPos = transform.position;
            Quaternion startRot = transform.rotation;
            float elapsedTime = 0f;
            
            // Воспроизводим звук подключения
            if (connectionSound != null && audioSource != null)
            {
                audioSource.clip = connectionSound;
                audioSource.Play();
            }
            
            while (elapsedTime < 1f)
            {
                elapsedTime += Time.deltaTime * connectionSpeed;
                float progress = Mathf.SmoothStep(0f, 1f, elapsedTime);
                
                transform.position = Vector3.Lerp(startPos, targetPosition.position, progress);
                transform.rotation = Quaternion.Lerp(startRot, targetPosition.rotation, progress);
                
                yield return null;
            }
            
            // Устанавливаем точную позицию и поворот
            transform.position = targetPosition.position;
            transform.rotation = targetPosition.rotation;
            
            // Делаем дочерним объектом
            transform.SetParent(targetPosition);
            
            isConnected = true;
            isConnecting = false;
            
            UpdateVisualState();
            
            // Уведомляем систему шагов
            NotifyStepSystem();
            
            Debug.Log("PlugConnector: Штекер подключен");
        }
        
        /// <summary>
        /// Корутина отключения
        /// </summary>
        private IEnumerator DisconnectCoroutine()
        {
            Vector3 startPos = transform.position;
            Quaternion startRot = transform.rotation;
            float elapsedTime = 0f;
            
            // Воспроизводим звук отключения
            if (disconnectSound != null && audioSource != null)
            {
                audioSource.clip = disconnectSound;
                audioSource.Play();
            }
            
            // Возвращаем к исходному родителю
            transform.SetParent(originalParent);
            
            while (elapsedTime < 1f)
            {
                elapsedTime += Time.deltaTime * connectionSpeed;
                float progress = Mathf.SmoothStep(0f, 1f, elapsedTime);
                
                transform.position = Vector3.Lerp(startPos, originalPosition, progress);
                transform.rotation = Quaternion.Lerp(startRot, originalRotation, progress);
                
                yield return null;
            }
            
            // Устанавливаем точную позицию и поворот
            transform.position = originalPosition;
            transform.rotation = originalRotation;
            
            isConnected = false;
            isConnecting = false;
            
            UpdateVisualState();
            
            // Уведомляем систему шагов об отключении
            NotifyStepSystemDisconnect();
            
            Debug.Log("PlugConnector: Штекер отключен");
        }
        
        /// <summary>
        /// Обновить визуальное состояние
        /// </summary>
        private void UpdateVisualState()
        {
            if (targetRenderer != null)
            {
                if (isConnected && connectedMaterial != null)
                {
                    targetRenderer.material = connectedMaterial;
                }
                else if (!isConnected && disconnectedMaterial != null)
                {
                    targetRenderer.material = disconnectedMaterial;
                }
            }
        }
        
        /// <summary>
        /// Уведомить систему шагов о подключении
        /// </summary>
        private void NotifyStepSystem()
        {
            Debug.Log($"PlugConnector: NotifyStepSystem called. StepManager: {(stepManager != null ? "Found" : "Not Found")}, TargetStepIndex: {targetStepIndex}");
            
            // Уведомляем ConnectionManager
            NotifyConnectionManager(true);
            
            if (stepManager != null)
            {
                if (targetStepIndex >= 0)
                {
                    // Используем новый метод регистрации подключения
                    stepManager.RegisterConnection(targetStepIndex);
                }
                else
                {
                    // Если targetStepIndex не установлен, просто переходим к следующему шагу
                    Debug.Log("PlugConnector: No target step index, moving to next step");
                    stepManager.NextStep();
                }
            }
            else
            {
                Debug.LogWarning("PlugConnector: StepManager not found!");
            }
        }
        
        /// <summary>
        /// Уведомляем систему шагов об отключении
        /// </summary>
        private void NotifyStepSystemDisconnect()
        {
            Debug.Log($"PlugConnector: NotifyStepSystemDisconnect called. StepManager: {(stepManager != null ? "Found" : "Not Found")}, TargetStepIndex: {targetStepIndex}");
            
            // Уведомляем ConnectionManager
            NotifyConnectionManager(false);
            
            if (stepManager != null)
            {
                if (targetStepIndex >= 0)
                {
                    // Используем новый метод регистрации отключения
                    stepManager.RegisterDisconnection(targetStepIndex);
                }
                else
                {
                    // Если targetStepIndex не установлен, просто переходим к следующему шагу
                    Debug.Log("PlugConnector: No target step index, moving to next step");
                    stepManager.NextStep();
                }
            }
            else
            {
                Debug.LogWarning("PlugConnector: StepManager not found!");
            }
        }
        
        /// <summary>
        /// Уведомить ConnectionManager о изменении состояния подключения
        /// </summary>
        private void NotifyConnectionManager(bool connected)
        {
            Debug.Log($"PlugConnector: NotifyConnectionManager called with {connected}. ConnectionType: {connectionType}");
            
            if (ConnectionManager.Instance != null)
            {
                Debug.Log($"PlugConnector: ConnectionManager found, calling Set...");
                
                switch (connectionType)
                {
                    case ConnectionType.Guitar:
                        ConnectionManager.Instance.SetGuitarConnected(connected);
                        break;
                    case ConnectionType.Amplifier:
                        ConnectionManager.Instance.SetAmplifierConnected(connected);
                        break;
                }
                
                Debug.Log($"PlugConnector: Notified ConnectionManager - {connectionType} is now {(connected ? "connected" : "disconnected")}");
            }
            else
            {
                Debug.LogError("PlugConnector: ConnectionManager not found! Check if ConnectionManager exists in scene.");
            }
        }
        
        /// <summary>
        /// Установить целевое положение
        /// </summary>
        public void SetTargetPosition(Transform target)
        {
            targetPosition = target;
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
        /// Принудительно установить состояние подключения
        /// </summary>
        public void ForceConnectionState(bool connected)
        {
            if (connected && !isConnected)
            {
                Connect();
            }
            else if (!connected && isConnected)
            {
                Disconnect();
            }
        }
        
        /// <summary>
        /// Сбросить к исходному состоянию
        /// </summary>
        public void ResetToOriginal()
        {
            if (isConnecting)
            {
                if (connectionCoroutine != null)
                {
                    StopCoroutine(connectionCoroutine);
                }
                isConnecting = false;
            }
            
            if (isConnected)
            {
                transform.SetParent(originalParent);
                transform.position = originalPosition;
                transform.rotation = originalRotation;
                isConnected = false;
            }
            
            UpdateVisualState();
        }
        
        /// <summary>
        /// Установить тип подключения
        /// </summary>
        public void SetConnectionType(ConnectionType type)
        {
            connectionType = type;
        }
        
        /// <summary>
        /// Получить тип подключения
        /// </summary>
        public ConnectionType GetConnectionType()
        {
            return connectionType;
        }
    }
} 