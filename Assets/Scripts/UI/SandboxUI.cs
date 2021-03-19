using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SandboxUI : MonoBehaviour
{
    [SerializeField]
    private GameObject m_GameOverlay;
    [SerializeField]
    private GameObject m_PauseMenu;

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            PauseGame();
        }
    }

    public void PauseGame()
    {
        m_GameOverlay.SetActive(false);
        m_PauseMenu.GetComponent<PauseMenu>().PauseGame();
    }

    public void ResumeGame()
    {
        m_GameOverlay.SetActive(true);
    }
}
