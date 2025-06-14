using UnityEngine;
using UnityEngine.EventSystems;

public class HoverTextTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [TextArea]
    public string message;

    public void OnPointerEnter(PointerEventData eventData)
    {
        HoverTextManager.Instance?.ShowText(message);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        HoverTextManager.Instance?.ClearText();
    }
}
