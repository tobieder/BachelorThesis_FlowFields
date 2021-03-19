using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

[RequireComponent(typeof(Image))]
public class MenuButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public MenuButtonGroup m_MenuButtonGroup;

    public UnityEvent onMenuButtonSelected;
    public UnityEvent onMenuButtonDeselected;

    private Image m_Background;

    void Start()
    {
        m_Background = GetComponent<Image>();
        m_MenuButtonGroup.Subscribe(this);
    }

    public void SetTint(Color _color)
    {
        m_Background.color = _color;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        m_MenuButtonGroup.OnTabSelected(this);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        m_MenuButtonGroup.OnTabEnter(this);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        m_MenuButtonGroup.OnTabExit(this);
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
