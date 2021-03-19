using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Image))]
public class TabButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public TabGroup m_TabGroup;

    private Image m_Background;

    void Start()
    {
        m_Background = GetComponent<Image>();
        m_TabGroup.Subscribe(this);
    }

    public void SetTint(Color _color)
    {
        m_Background.color = _color;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        m_TabGroup.OnTabSelected(this);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        m_TabGroup.OnTabEnter(this);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        m_TabGroup.OnTabExit(this);
    }
}
