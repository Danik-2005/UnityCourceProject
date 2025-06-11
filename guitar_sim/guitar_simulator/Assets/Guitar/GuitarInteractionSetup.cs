using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Camera))]
public class GuitarInteractionSetup : MonoBehaviour
{
    private void Awake()
    {
        // Удаляем стандартный PhysicsRaycaster если он есть
        var existingRaycaster = GetComponent<PhysicsRaycaster>();
        if (existingRaycaster != null && !(existingRaycaster is CustomPhysicsRaycaster))
        {
            DestroyImmediate(existingRaycaster);
        }

        // Добавляем кастомный PhysicsRaycaster на камеру
        if (GetComponent<CustomPhysicsRaycaster>() == null)
        {
            var raycaster = gameObject.AddComponent<CustomPhysicsRaycaster>();
        }

        // Проверяем наличие EventSystem в сцене
        if (FindObjectOfType<EventSystem>() == null)
        {
            var eventSystem = new GameObject("EventSystem");
            eventSystem.AddComponent<EventSystem>();
            eventSystem.AddComponent<StandaloneInputModule>();
        }
    }
} 