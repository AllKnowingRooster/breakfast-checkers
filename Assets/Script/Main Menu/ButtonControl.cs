using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonControl : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Color idleBackgroundColor;
    [SerializeField] private Color idleContentColor;
    [SerializeField] private Color hoverBackgroundColor;
    [SerializeField] private Color hoverContentColor;
    [SerializeField] private TextMeshProUGUI buttonText;
    [SerializeField] private Image buttonIcon;
    private Image img;

    private void Awake()
    {
        img = GetComponent<Image>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        GameManager.instance.NotifyObserver(UserAction.Click);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        img.color = hoverBackgroundColor;
        if (buttonIcon != null)
        {
            buttonIcon.color = hoverContentColor;
        }
        else
        {
            buttonText.color = hoverContentColor;
        }
        GameManager.instance.NotifyObserver(UserAction.Hover);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        img.color = idleBackgroundColor;
        if (buttonIcon != null)
        {
            buttonIcon.color = idleContentColor;
        }
        else
        {
            buttonText.color = idleContentColor;
        }
    }
}
