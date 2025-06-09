using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Camera Settings")]
    [SerializeField] private float mouseSensitivity = 2f;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private bool invertY = false;
    
    [Header("Rotation Limits")]
    [SerializeField] private float minVerticalAngle = -30f;
    [SerializeField] private float maxVerticalAngle = 60f;

    [Header("Crouch Settings")]
    [SerializeField] private float crouchHeight = 0.7f;    // Насколько низко опускается камера при приседании
    [SerializeField] private float crouchSpeed = 10f;      // Скорость приседания/вставания

    private float rotationX = 0f;
    private float rotationY = 0f;
    private bool isControlling = false;
    private float defaultHeight;                           // Изначальная высота камеры
    private float targetHeight;                            // Целевая высота для плавного перехода
    private Transform playerTransform;                     // Ссылка на объект player

    private void Start()
    {
        // Находим объект player (родитель камеры)
        playerTransform = transform.parent;
        if (playerTransform == null)
        {
            Debug.LogError("CameraController: Камера должна быть дочерней к объекту Player!");
            return;
        }

        Debug.Log("CameraController: Player найден - " + playerTransform.name);

        // Сохраняем начальный поворот камеры
        Vector3 angles = transform.localEulerAngles;
        rotationX = angles.y;
        rotationY = angles.x;

        // Сохраняем начальную высоту камеры
        defaultHeight = transform.localPosition.y;
        targetHeight = defaultHeight;
        
        Debug.Log("CameraController: Начальная высота камеры - " + defaultHeight);
    }

    private void Update()
    {
        // Проверяем нажатие средней кнопки мыши для управления камерой
        if (Input.GetMouseButtonDown(2)) // 2 = средняя кнопка (колесо)
        {
            isControlling = true;
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            Debug.Log("CameraController: Управление включено");
        }
        else if (Input.GetMouseButtonUp(2))
        {
            isControlling = false;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            Debug.Log("CameraController: Управление выключено");
        }

        // Вращение камеры только при зажатой средней кнопке
        if (isControlling)
        {
            HandleRotation();
        }

        // Движение работает всегда
        HandleMovement();

        // Обрабатываем приседание независимо от управления камерой
        HandleCrouching();
    }

    private void HandleRotation()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * (invertY ? 1 : -1);

        rotationX += mouseX;
        rotationY = Mathf.Clamp(rotationY + mouseY, minVerticalAngle, maxVerticalAngle);

        // Горизонтальный поворот (влево-вправо) - поворачиваем весь объект Player
        if (playerTransform != null)
        {
            playerTransform.rotation = Quaternion.Euler(0, rotationX, 0);
        }
        
        // Вертикальный поворот (вверх-вниз) - поворачиваем только камеру
        transform.localRotation = Quaternion.Euler(rotationY, 0, 0);
    }

    private void HandleMovement()
    {
        if (playerTransform == null) 
        {
            Debug.LogWarning("CameraController: Player не найден!");
            return;
        }

        // Получаем ввод с клавиатуры
        float horizontal = Input.GetAxis("Horizontal"); // A/D
        float vertical = Input.GetAxis("Vertical");     // W/S

        // Создаем вектор движения в локальных координатах камеры
        Vector3 moveDirection = transform.right * horizontal + transform.forward * vertical;
        moveDirection.y = 0; // Убираем вертикальное движение, чтобы player двигался только в горизонтальной плоскости
        moveDirection = moveDirection.normalized;

        // Перемещаем объект player, а не камеру
        Vector3 newPosition = playerTransform.position + moveDirection * moveSpeed * Time.deltaTime;
        newPosition.y = playerTransform.position.y; // Сохраняем текущую высоту player
        
        playerTransform.position = newPosition;
    }

    private void HandleCrouching()
    {
        // Определяем целевую высоту в зависимости от нажатия Ctrl
        targetHeight = Input.GetKey(KeyCode.LeftControl) ? defaultHeight - crouchHeight : defaultHeight;

        // Плавно меняем текущую высоту камеры
        Vector3 currentPos = transform.localPosition;
        currentPos.y = Mathf.Lerp(currentPos.y, targetHeight, Time.deltaTime * crouchSpeed);
        transform.localPosition = currentPos;
    }
} 