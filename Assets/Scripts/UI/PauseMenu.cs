using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public void PauseGame()
    {
        gameObject.SetActive(true);
        Time.timeScale = 0.0f;
    }

    public void ResumeGame()
    {
        gameObject.SetActive(false);
        transform.parent.GetComponent<SandboxUI>().ResumeGame();
        Time.timeScale = 1.0f;
    }

    public void LoadMainMenu()
    {
        Time.timeScale = 1.0f;
        GameManager.instance.LoadMainMenu();
    }
}
