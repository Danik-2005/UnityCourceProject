using UnityEngine;

public class FretTrigger : MonoBehaviour
{
    public int stringNumber;
    public int fretNumber;
    public GuitarStringVisual parentString;

    private void OnMouseDown()
    {
        parentString.OnFretPressed(fretNumber);
    }

    private void OnMouseUp()
    {
        parentString.OnFretReleased(fretNumber);
    }

    private void OnMouseExit()
    {
        // Если мышь покидает коллайдер, тоже останавливаем звук
        parentString.OnFretReleased(fretNumber);
    }
} 