using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuButtonGroup : MonoBehaviour
{
    public Color m_TabIdle;
    public Color m_TabHover;
    public Color m_TabActive;

    private List<MenuButton> m_MenuButtons;
    private MenuButton m_SelectedButton;

    public void Subscribe(MenuButton _menuButton)
    {
        if (m_MenuButtons == null)
        {
            m_MenuButtons = new List<MenuButton>();
        }

        m_MenuButtons.Add(_menuButton);
    }

    public void OnTabEnter(MenuButton _menuButton)
    {
        ResetTabs();
        if (m_SelectedButton == null || _menuButton != m_SelectedButton)
        {
            _menuButton.SetTint(m_TabHover);
        }
    }

    public void OnTabExit(MenuButton _menuButton)
    {
        ResetTabs();
    }

    public void OnTabSelected(MenuButton _menuButton)
    {
        ResetTabs();
        if(m_SelectedButton != null)
        {
            m_SelectedButton.Deselect();
        }

        if (m_SelectedButton == _menuButton)
        {
            m_SelectedButton = null;
        }
        else
        {
            m_SelectedButton = _menuButton;
            m_SelectedButton.Select();
            _menuButton.SetTint(m_TabActive);
        }
    }

    private void ResetTabs()
    {
        foreach (MenuButton _menuButton in m_MenuButtons)
        {
            if (m_SelectedButton != null && _menuButton == m_SelectedButton)
            {
                continue;
            }
            _menuButton.SetTint(m_TabIdle);
        }
    }

    private void OnDisable()
    {
        if (m_SelectedButton != null)
        {
            m_SelectedButton.Deselect();
            m_SelectedButton = null;
        }
        ResetTabs();
    }
}
