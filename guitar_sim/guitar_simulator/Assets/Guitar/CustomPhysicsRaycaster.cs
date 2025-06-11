using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

[RequireComponent(typeof(Camera))]
public class CustomPhysicsRaycaster : PhysicsRaycaster
{
    protected override void Awake()
    {
        base.Awake();
        // Устанавливаем маску слоев, исключающую триггеры
        eventMask = Physics.DefaultRaycastLayers;
    }

    public override void Raycast(PointerEventData eventData, List<RaycastResult> resultAppendList)
    {
        if (eventCamera == null)
            return;

        var ray = eventCamera.ScreenPointToRay(eventData.position);
        var hits = Physics.RaycastAll(ray, eventCamera.farClipPlane - eventCamera.nearClipPlane, eventMask, QueryTriggerInteraction.Ignore);

        for (int i = 0; i < hits.Length; i++)
        {
            var hit = hits[i];
            var result = new RaycastResult
            {
                gameObject = hit.collider.gameObject,
                module = this,
                distance = hit.distance,
                worldPosition = hit.point,
                worldNormal = hit.normal,
                screenPosition = eventData.position,
                index = resultAppendList.Count,
                sortingLayer = 0,
                sortingOrder = 0
            };
            resultAppendList.Add(result);
        }
    }
}