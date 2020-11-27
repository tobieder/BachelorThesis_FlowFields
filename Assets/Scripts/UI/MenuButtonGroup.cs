using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuButtonGroup : MonoBehaviour
{
    public Color tabIdle;
    public Color tabHover;
    public Color tabActive;

    private List<MenuButton> menuButtons;
    private MenuButton selectedButton;

    public void Subscribe(MenuButton _menuButton)
    {
        if (menuButtons == null)
        {
            menuButtons = new List<MenuButton>();
        }

        menuButtons.Add(_menuButton);
    }

    public void OnTabEnter(MenuButton _menuButton)
    {
        ResetTabs();
        if (selectedButton == null || _menuButton != selectedButton)
        {
            _menuButton.SetTint(tabHover);
        }
    }

    public void OnTabExit(MenuButton _menuButton)
    {
        ResetTabs();
    }

    public void OnTabSelected(MenuButton _menuButton)
    {
        ResetTabs();
        if(selectedButton != null)
        {
            selectedButton.Deselect();
        }

        if (selectedButton == _menuButton)
        {
            selectedButton = null;
        }
        else
        {
            selectedButton = _menuButton;
            selectedButton.Select();
            _menuButton.SetTint(tabActive);
        }
    }

    private void ResetTabs()
    {
        foreach (MenuButton _menuButton in menuButtons)
        {
            if (selectedButton != null && _menuButton == selectedButton)
            {
                continue;
            }
            _menuButton.SetTint(tabIdle);
        }
    }

    private void OnDisable()
    {
        if (selectedButton != null)
        {
            selectedButton.Deselect();
            selectedButton = null;
        }
        ResetTabs();
    }
}
