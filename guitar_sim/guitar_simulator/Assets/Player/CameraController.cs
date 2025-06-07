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

    private float rotationX = 0f;
    private float rotationY = 0f;
    private bool isControlling = false;

    private void Start()
    {
        // Сохраняем начальный поворот
        Vector3 angles = transform.eulerAngles;
        rotationX = angles.y;
        rotationY = angles.x;
    }

    private void Update()
    {
        // Проверяем нажатие средней кнопки мыши
        if (Input.GetMouseButtonDown(2)) // 2 = средняя кнопка (колесо)
        {
            isControlling = true;
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
        else if (Input.GetMouseButtonUp(2))
        {
            isControlling = false;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        // Управляем камерой только при зажатой средней кнопке
        if (isControlling)
        {
            HandleRotation();
            HandleMovement();
        }
    }

    private void HandleRotation()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * (invertY ? 1 : -1);

        rotationX += mouseX;
        rotationY = Mathf.Clamp(rotationY + mouseY, minVerticalAngle, maxVerticalAngle);

        transform.rotation = Quaternion.Euler(rotationY, rotationX, 0);
    }

    private void HandleMovement()
    {
        // Получаем ввод с клавиатуры
        float horizontal = Input.GetAxis("Horizontal"); // A/D
        float vertical = Input.GetAxis("Vertical");     // W/S

        // Создаем вектор движения в локальных координатах камеры
        Vector3 moveDirection = transform.right * horizontal + transform.forward * vertical;
        moveDirection.y = 0; // Убираем вертикальное движение, чтобы камера двигалась только в горизонтальной плоскости
        moveDirection = moveDirection.normalized;

        // Перемещаем камеру
        transform.position += moveDirection * moveSpeed * Time.deltaTime;
    }
} 