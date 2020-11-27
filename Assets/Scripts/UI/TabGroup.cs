using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TabGroup : MonoBehaviour
{
    public Color tabIdle;
    public Color tabHover;
    public Color tabActive;

    public List<GameObject> tabPages;

    private List<TabButton> tabButtons;
    private TabButton selectedTab;

    public void Subscribe(TabButton _tabButton)
    {
        if(tabButtons == null)
        {
            tabButtons = new List<TabButton>();
        }

        tabButtons.Add(_tabButton);
    }
    public void OnTabEnter(TabButton _tabButton)
    {
        ResetTabs();
        if (selectedTab == null || _tabButton != selectedTab)
        {
            _tabButton.SetTint(tabHover);
        }
    }

    public void OnTabExit(TabButton _tabButton)
    {
        ResetTabs();
    }

    public void OnTabSelected(TabButton _tabButton)
    {
        ResetTabs();
        if (selectedTab == _tabButton)
        {
            selectedTab = null;

            int selectedIndex = _tabButton.transform.GetSiblingIndex();
            tabPages[selectedIndex].SetActive(false);
        }
        else
        {
            selectedTab = _tabButton;
            _tabButton.SetTint(tabActive);

            int selectedIndex = _tabButton.transform.GetSiblingIndex();
            for (int i = 0; i < tabPages.Count; i++)
            {
                if (i == selectedIndex)
                {
                    tabPages[i].SetActive(true);
                }
                else
                {
                    tabPages[i].SetActive(false);
                }
            }
        }
    }

    private void ResetTabs()
    {
        foreach(TabButton _tabButton in tabButtons)
        {
            if(selectedTab != null && _tabButton == selectedTab)
            { 
                continue; 
            }
            _tabButton.SetTint(tabIdle);
        }
    }
}

