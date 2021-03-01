using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SandboxUI : MonoBehaviour
{
    public GameObject gameOverlay;
    public GameObject pauseMenu;

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
        gameOverlay.SetActive(false);
        pauseMenu.GetComponent<PauseMenu>().PauseGame();
    }

    public void ResumeGame()
    {
        gameOverlay.SetActive(true);
    }
}
