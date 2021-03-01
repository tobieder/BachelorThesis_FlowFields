using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public void LoadBenchmark()
    {
        GameManager.instance.LoadBenchmark();
    }

    public void OpenBachelorThesis()
    {
        Application.OpenURL("https://github.com/tobieder");
    }

    public void QuitApplication()
    {
        Application.Quit();
    }
}
