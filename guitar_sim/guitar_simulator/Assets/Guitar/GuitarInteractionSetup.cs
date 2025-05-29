using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Camera))]
public class GuitarInteractionSetup : MonoBehaviour
{
    private void Awake()
    {
        // Добавляем PhysicsRaycaster на камеру
        if (GetComponent<PhysicsRaycaster>() == null)
        {
            var raycaster = gameObject.AddComponent<PhysicsRaycaster>();
            Debug.Log("Added PhysicsRaycaster to camera");
        }

        // Проверяем наличие EventSystem в сцене
        if (FindObjectOfType<EventSystem>() == null)
        {
            var eventSystem = new GameObject("EventSystem");
            eventSystem.AddComponent<EventSystem>();
            eventSystem.AddComponent<StandaloneInputModule>();
            Debug.Log("Created EventSystem");
        }
    }
} 