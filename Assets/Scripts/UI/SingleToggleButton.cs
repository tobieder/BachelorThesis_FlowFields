using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

[RequireComponent(typeof(Image))]
public class SingleToggleButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public Color buttonIdle;
    public Color buttonHover;
    public Color buttonActive;

    public UnityEvent onButtonSelected;
    public UnityEvent onButtonDeselected;


    private bool selected;
    private Image background;

    private void Start()
    {
        selected = false;
        background = GetComponent<Image>();
    }

    private void SetTint(Color _color)
    {
        background.color = _color;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if(selected)
        {
            Deselect();
            SetTint(buttonHover);
            selected = false;
        }
        else
        {
            Select();
            SetTint(buttonActive);
            selected = true;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!selected)
        {
            SetTint(buttonHover);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!selected)
        {
            SetTint(buttonIdle);
        }
    }

    public void Select()
    {
        if(onButtonSelected != null)
        {
            onButtonSelected.Invoke();
        }
    }

    public void Deselect()
    {
        if(onButtonDeselected != null)
        {
            onButtonDeselected.Invoke();
        }
    }
}
