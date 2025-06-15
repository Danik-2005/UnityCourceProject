using UnityEngine;
using System.Collections;

public class GuitarPickup : MonoBehaviour
{
    [Header("Настройки подбора")]
    [SerializeField] private float animationSpeed = 2f;
    [SerializeField] private float triggerZoneSize = 4f; // Размер триггерной зоны
    
    [Header("Объект рук")]
    [Tooltip("Перетащите сюда пустой объект, который выполняет функцию рук")]
    [SerializeField] private Transform handsObject;
    
    [Header("Дополнительная настройка позиции")]
    [Tooltip("Дополнительное смещение относительно объекта рук")]
    [SerializeField] private Vector3 offsetFromHands = Vector3.zero;
    
    [Header("Поворот в руках")]
    [Tooltip("X: наклон вперед/назад, Y: поворот влево/вправо, Z: поворот струнами вверх")]
    [SerializeField] private Vector3 handRotation = new Vector3(0f, 0f, 90f);
    
    private bool isInHands = false;
    private bool playerInRange = false;
    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private Transform originalParent;
    private GameObject triggerZoneObject;
    private Collider triggerZone;
    
    void Start()
    {
        // Запоминаем исходное положение гитары
        originalPosition = transform.position;
        originalRotation = transform.rotation;
        originalParent = transform.parent;
        
        // Создаем триггерную зону автоматически
        CreateTriggerZone();
        
        // Проверяем, назначен ли объект рук
        if (handsObject == null)
        {
            Debug.LogWarning("GuitarPickup: Не назначен объект рук! Назначьте пустой объект в поле 'Hands Object'");
        }
        
        // Проверяем наличие Player в сцене
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogWarning("GuitarPickup: В сцене нет объекта с тегом 'Player'!");
        }
        else
        {
            Collider playerCollider = player.GetComponent<Collider>();
            if (playerCollider == null)
            {
                Debug.LogWarning("GuitarPickup: У Player нет коллайдера!");
            }
        }
    }
    
    void CreateTriggerZone()
    {
        // Проверяем, есть ли уже триггерная зона
        Transform existingZone = transform.Find("GuitarTriggerZone");
        if (existingZone != null)
        {
            triggerZoneObject = existingZone.gameObject;
            triggerZone = triggerZoneObject.GetComponent<Collider>();
            return;
        }
        
        // Создаем объект для триггерной зоны
        triggerZoneObject = new GameObject("GuitarTriggerZone");
        triggerZoneObject.transform.SetParent(transform);
        triggerZoneObject.transform.localPosition = Vector3.zero;
        
        // Добавляем коллайдер-триггер
        SphereCollider sphereCollider = triggerZoneObject.AddComponent<SphereCollider>();
        sphereCollider.radius = triggerZoneSize;
        sphereCollider.isTrigger = true;
        
        // Сохраняем ссылку на триггерную зону
        triggerZone = sphereCollider;
    }
    
    void Update()
    {
        // Если гитара в руках, следуем за объектом рук
        if (isInHands && handsObject != null)
        {
            Vector3 targetPos = handsObject.position + offsetFromHands;
            Quaternion targetRot = handsObject.rotation * Quaternion.Euler(handRotation);
            
            // Плавно следуем за руками
            transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * animationSpeed * 2f);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRot, Time.deltaTime * animationSpeed * 2f);
        }
        
        // Проверяем состояние триггерной зоны
        if (triggerZone == null)
        {
            CreateTriggerZone();
        }
        
        // Принудительная проверка входа в зону
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null && triggerZone != null)
        {
            float distance = Vector3.Distance(player.transform.position, triggerZone.transform.position);
            float triggerRadius = (triggerZone as SphereCollider)?.radius ?? 2f;
            
            bool shouldBeInRange = distance <= triggerRadius;
            
            // Если состояние изменилось, обновляем
            if (shouldBeInRange != playerInRange)
            {
                playerInRange = shouldBeInRange;
            }
        }
        
        // Если игрок в зоне и нажал E
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            if (!isInHands)
            {
                // Подбираем гитару
                if (player != null)
                {
                    StartCoroutine(PickupGuitar(player.transform));
                }
            }
            else
            {
                // Возвращаем гитару на место
                StartCoroutine(ReturnGuitar());
            }
        }
    }
    
    // Срабатывает когда игрок входит в зону гитары
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }
    
    // Срабатывает когда игрок выходит из зоны гитары
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }
    
    IEnumerator PickupGuitar(Transform player)
    {
        isInHands = true;
        
        // Сохраняем начальные параметры
        Vector3 startPos = transform.position;
        Quaternion startRot = transform.rotation;
        
        // Вычисляем позицию в руках
        Vector3 targetPos;
        Quaternion targetRot;
        
        if (handsObject != null)
        {
            // Используем позицию объекта рук
            targetPos = handsObject.position + offsetFromHands;
            targetRot = handsObject.rotation * Quaternion.Euler(handRotation);
        }
        else
        {
            // Fallback - используем позицию игрока
            targetPos = player.position + new Vector3(0.3f, -0.2f, 0.5f);
            targetRot = player.rotation * Quaternion.Euler(handRotation);
        }
        
        // Анимация перемещения и поворота
        float time = 0;
        while (time < 1)
        {
            time += Time.deltaTime * animationSpeed;
            float t = Mathf.SmoothStep(0, 1, time);
            
            transform.position = Vector3.Lerp(startPos, targetPos, t);
            transform.rotation = Quaternion.Lerp(startRot, targetRot, t);
            
            yield return null;
        }
    }
    
    IEnumerator ReturnGuitar()
    {
        isInHands = false;
        
        // Сохраняем текущие параметры
        Vector3 startPos = transform.position;
        Quaternion startRot = transform.rotation;
        
        // Анимация возврата на место
        float time = 0;
        while (time < 1)
        {
            time += Time.deltaTime * animationSpeed;
            float t = Mathf.SmoothStep(0, 1, time);
            
            transform.position = Vector3.Lerp(startPos, originalPosition, t);
            transform.rotation = Quaternion.Lerp(startRot, originalRotation, t);
            
            yield return null;
        }
        
        // Устанавливаем точную позицию
        transform.position = originalPosition;
        transform.rotation = originalRotation;
    }
} 