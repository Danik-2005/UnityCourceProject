using UnityEngine;
using UnityEngine.Audio;
using System.Collections.Generic;

namespace GuitarSimulator
{
    /// <summary>
    /// Глобальный менеджер подключений для управления состоянием проводов и аудио
    /// </summary>
    public class ConnectionManager : MonoBehaviour
    {
        [Header("Audio Settings")]
        [SerializeField] private AudioMixer audioMixer;
        [SerializeField] private string guitarVolumeParameter = "GuitarVolume";
        [SerializeField] private string guitarMuteParameter = "GuitarMute";
        
        [Header("Volume Settings")]
        [SerializeField] private float connectedVolume = 0f; // Громкость когда подключено (в децибелах)
        [SerializeField] private float disconnectedVolume = -70f; // Громкость когда не подключено (в децибелах)
        [SerializeField] private float mutedVolume = -70f; // Приглушенная громкость
        
        [Header("Connection States")]
        [SerializeField] private bool isGuitarConnected = false;
        [SerializeField] private bool isAmplifierConnected = false;
        
        // События для уведомления других систем
        public System.Action<bool> OnGuitarConnectionChanged;
        public System.Action<bool> OnAmplifierConnectionChanged;
        public System.Action OnConnectionStateChanged;
        
        // Синглтон для глобального доступа
        public static ConnectionManager Instance { get; private set; }
        
        // Свойства для чтения состояния
        public bool IsGuitarConnected => isGuitarConnected;
        public bool IsAmplifierConnected => isAmplifierConnected;
        public bool IsFullyConnected => isGuitarConnected && isAmplifierConnected;
        
        void Awake()
        {
            // Настройка синглтона
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
                return;
            }
            
            // Автоматически найти AudioMixer если не назначен
            if (audioMixer == null)
            {
                audioMixer = Resources.Load<AudioMixer>("AudioMixer");
            }
            
            // Проверяем существование параметров
            VerifyMixerParameters();
            
            // Устанавливаем начальное состояние как отключенное
            isGuitarConnected = false;
            isAmplifierConnected = false;
            
            // Инициализируем начальное состояние - гитара должна быть замучена
            UpdateAudioState();
            
            Debug.Log("ConnectionManager: Awake - Initialized with muted guitar");
        }
        
        /// <summary>
        /// Проверить существование параметров в микшере
        /// </summary>
        private void VerifyMixerParameters()
        {
            if (audioMixer == null) return;

            float testValue;
            bool volumeExists = audioMixer.GetFloat(guitarVolumeParameter, out testValue);
            bool muteExists = audioMixer.GetFloat(guitarMuteParameter, out testValue);

            if (!volumeExists)
                Debug.LogError($"Parameter '{guitarVolumeParameter}' not found in AudioMixer! Check if it's exposed.");
            if (!muteExists)
                Debug.LogError($"Parameter '{guitarMuteParameter}' not found in AudioMixer! Check if it's exposed.");

            if (!volumeExists || !muteExists)
                Debug.LogError("Some AudioMixer parameters are missing! Check if they are exposed in the AudioMixer.");
        }
        
        void Start()
        {
            // Устанавливаем начальное состояние как отключенное
            isGuitarConnected = false;
            isAmplifierConnected = false;
            
            // Принудительно обновляем аудио состояние для мутирования
            UpdateAudioState();
            
            Debug.Log("ConnectionManager: Initialized with all connections disabled - guitar should be muted");
        }
        
        /// <summary>
        /// Установить состояние подключения гитары
        /// </summary>
        public void SetGuitarConnected(bool connected)
        {
            Debug.Log($"ConnectionManager: SetGuitarConnected called with {connected}. Current state: {isGuitarConnected}");
            
            if (isGuitarConnected != connected)
            {
                isGuitarConnected = connected;
                Debug.Log($"ConnectionManager: Guitar connection changed to {connected}. IsFullyConnected: {IsFullyConnected}");
                UpdateAudioState();
                OnGuitarConnectionChanged?.Invoke(connected);
                OnConnectionStateChanged?.Invoke();
                
                Debug.Log($"ConnectionManager: Guitar connection changed to {connected}");
            }
            else
            {
                Debug.Log($"ConnectionManager: Guitar connection already {connected}, no change needed");
            }
        }
        
        /// <summary>
        /// Установить состояние подключения усилителя
        /// </summary>
        public void SetAmplifierConnected(bool connected)
        {
            Debug.Log($"ConnectionManager: SetAmplifierConnected called with {connected}. Current state: {isAmplifierConnected}");
            
            if (isAmplifierConnected != connected)
            {
                isAmplifierConnected = connected;
                Debug.Log($"ConnectionManager: Amplifier connection changed to {connected}. IsFullyConnected: {IsFullyConnected}");
                UpdateAudioState();
                OnAmplifierConnectionChanged?.Invoke(connected);
                OnConnectionStateChanged?.Invoke();
                
                Debug.Log($"ConnectionManager: Amplifier connection changed to {connected}");
            }
            else
            {
                Debug.Log($"ConnectionManager: Amplifier connection already {connected}, no change needed");
            }
        }
        
        /// <summary>
        /// Обновить аудио состояние в зависимости от подключений
        /// </summary>
        private void UpdateAudioState()
        {
            Debug.Log($"ConnectionManager: UpdateAudioState called. AudioMixer: {(audioMixer != null ? "Found" : "NOT FOUND")}");
            
            if (audioMixer == null)
            {
                Debug.LogError("ConnectionManager: AudioMixer not assigned! Cannot control audio.");
                return;
            }
            
            Debug.Log($"ConnectionManager: Parameters - Volume: '{guitarVolumeParameter}', Mute: '{guitarMuteParameter}'");
            Debug.Log($"ConnectionManager: Connection state - Guitar: {isGuitarConnected}, Amplifier: {isAmplifierConnected}, FullyConnected: {IsFullyConnected}");
            
            float targetVolume;
            
            if (IsFullyConnected)
            {
                // Оба провода подключены - нормальная громкость
                targetVolume = connectedVolume;
                Debug.Log($"ConnectionManager: Both connections active - normal volume: {targetVolume} dB");
            }
            else
            {
                // Не все провода подключены - отключенная/очень тихая громкость
                targetVolume = disconnectedVolume;
                Debug.Log($"ConnectionManager: Not all connections active - muted volume: {targetVolume} dB");
            }
            
            // Применяем громкость к микшеру (как в GuitarKnobsController)
            if (!audioMixer.SetFloat(guitarVolumeParameter, targetVolume))
            {
                Debug.LogError($"ConnectionManager: Failed to set {guitarVolumeParameter} to {targetVolume}");
            }
            else
            {
                Debug.Log($"ConnectionManager: SUCCESS - Set {guitarVolumeParameter} to {targetVolume} dB");
            }
            
            // Также можно управлять параметром мута
            bool isMuted = !IsFullyConnected;
            if (!audioMixer.SetFloat(guitarMuteParameter, isMuted ? 1f : 0f))
            {
                Debug.LogError($"ConnectionManager: Failed to set {guitarMuteParameter} to {(isMuted ? 1f : 0f)}");
            }
            else
            {
                Debug.Log($"ConnectionManager: SUCCESS - Set {guitarMuteParameter} to {(isMuted ? 1f : 0f)}");
            }
            
            // Проверяем текущие значения в микшере
            float currentVolume, currentMute;
            if (audioMixer.GetFloat(guitarVolumeParameter, out currentVolume) && 
                audioMixer.GetFloat(guitarMuteParameter, out currentMute))
            {
                Debug.Log($"ConnectionManager: Current values - Volume: {currentVolume} dB, Mute: {currentMute}");
            }
            else
            {
                Debug.LogError("ConnectionManager: Failed to read current parameter values");
            }
        }
        
        /// <summary>
        /// Получить текущее состояние подключений в виде строки
        /// </summary>
        public string GetConnectionStatus()
        {
            return $"Guitar: {(isGuitarConnected ? "Connected" : "Disconnected")}, " +
                   $"Amplifier: {(isAmplifierConnected ? "Connected" : "Disconnected")}";
        }
        
        /// <summary>
        /// Сбросить все подключения
        /// </summary>
        public void ResetAllConnections()
        {
            SetGuitarConnected(false);
            SetAmplifierConnected(false);
            Debug.Log("ConnectionManager: All connections reset");
        }
        
        /// <summary>
        /// Установить все подключения
        /// </summary>
        public void SetAllConnections()
        {
            SetGuitarConnected(true);
            SetAmplifierConnected(true);
            Debug.Log("ConnectionManager: All connections set");
        }
    }
} 