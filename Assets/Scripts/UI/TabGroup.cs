using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TabGroup : MonoBehaviour
{
    public Color m_TabIdle;
    public Color m_TabHover;
    public Color m_TabActive;

    public List<GameObject> m_TabPages;

    private List<TabButton> m_TabButtons;
    private TabButton m_SelectedTab;

    public void Subscribe(TabButton _tabButton)
    {
        if(m_TabButtons == null)
        {
            m_TabButtons = new List<TabButton>();
        }

        m_TabButtons.Add(_tabButton);
    }
    public void OnTabEnter(TabButton _tabButton)
    {
        ResetTabs();
        if (m_SelectedTab == null || _tabButton != m_SelectedTab)
        {
            _tabButton.SetTint(m_TabHover);
        }
    }

    public void OnTabExit(TabButton _tabButton)
    {
        ResetTabs();
    }

    public void OnTabSelected(TabButton _tabButton)
    {
        ResetTabs();
        if (m_SelectedTab == _tabButton)
        {
            m_SelectedTab = null;

            int selectedIndex = _tabButton.transform.GetSiblingIndex();
            m_TabPages[selectedIndex].SetActive(false);
        }
        else
        {
            m_SelectedTab = _tabButton;
            _tabButton.SetTint(m_TabActive);

            int selectedIndex = _tabButton.transform.GetSiblingIndex();
            for (int i = 0; i < m_TabPages.Count; i++)
            {
                if (i == selectedIndex)
                {
                    m_TabPages[i].SetActive(true);
                }
                else
                {
                    m_TabPages[i].SetActive(false);
                }
            }
        }
    }

    private void ResetTabs()
    {
        foreach(TabButton _tabButton in m_TabButtons)
        {
            if(m_SelectedTab != null && _tabButton == m_SelectedTab)
            { 
                continue; 
            }
            _tabButton.SetTint(m_TabIdle);
        }
    }
}

