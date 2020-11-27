using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

[RequireComponent(typeof(Image))]
public class MenuButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public MenuButtonGroup menuButtonGroup;

    public UnityEvent onMenuButtonSelected;
    public UnityEvent onMenuButtonDeselected;

    private Image background;

    void Start()
    {
        background = GetComponent<Image>();
        menuButtonGroup.Subscribe(this);
    }

    public void SetTint(Color _color)
    {
        background.color = _color;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        menuButtonGroup.OnTabSelected(this);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        menuButtonGroup.OnTabEnter(this);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        menuButtonGroup.OnTabExit(this);
    }

    public void Select()
    {
        if(onMenuButtonSelected != null)
        {
            onMenuButtonSelected.Invoke();
        }
    }

    public void Deselect()
    {
        if(onMenuButtonDeselected != null)
        {
            onMenuButtonDeselected.Invoke();
        }
    }
}
