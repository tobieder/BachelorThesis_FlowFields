using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public enum SceneIndexes
{
    MANAGER = 0,
    MENU = 1,
    SANDBOX = 2,
    BENCHMARK = 3
}

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public GameObject loadingScreen;
    public Slider progressBar;

    private SceneIndexes currentScene;

    private int selectedMapSize = 0;

    private void Awake()
    {
        instance = this;

        SceneManager.LoadSceneAsync((int)SceneIndexes.MENU, LoadSceneMode.Additive);
        currentScene = SceneIndexes.MENU;
    }

    List<AsyncOperation> scenesLoading = new List<AsyncOperation>();

    public void LoadMainMenu()
    {
        loadingScreen.SetActive(true);

        scenesLoading.Add(SceneManager.LoadSceneAsync((int)SceneIndexes.MENU, LoadSceneMode.Additive));
        scenesLoading.Add(SceneManager.UnloadSceneAsync((int)currentScene));
        currentScene = SceneIndexes.MENU;

        StartCoroutine(GetSceneLoadProgress());
    }

    public void LoadSandbox()
    {
        loadingScreen.SetActive(true);

        scenesLoading.Add(SceneManager.LoadSceneAsync((int)SceneIndexes.SANDBOX, LoadSceneMode.Additive));
        scenesLoading.Add(SceneManager.UnloadSceneAsync((int)currentScene));
        currentScene = SceneIndexes.SANDBOX;

        StartCoroutine(GetSceneLoadProgress());
    }

    public void LoadBenchmark()
    {
        loadingScreen.SetActive(true);

        scenesLoading.Add(SceneManager.LoadSceneAsync((int)SceneIndexes.BENCHMARK, LoadSceneMode.Additive));
        scenesLoading.Add(SceneManager.UnloadSceneAsync((int)currentScene));
        currentScene = SceneIndexes.BENCHMARK;

        StartCoroutine(GetSceneLoadProgress());
    }

    float totalSceneProgress;
    public IEnumerator GetSceneLoadProgress()
    {
        for(int i = 0; i < scenesLoading.Count; i++)
        {
            while(!scenesLoading[i].isDone)
            {
                totalSceneProgress = 0;

                foreach(AsyncOperation operation in scenesLoading)
                {
                    totalSceneProgress += operation.progress;
                }

                totalSceneProgress = (totalSceneProgress / (float)scenesLoading.Count);
                progressBar.value = totalSceneProgress;

                yield return null;
            }
        }

        loadingScreen.SetActive(false);
        scenesLoading.Clear();
    }

    #region Saved Data
    public void SetSelectedMapSize(int _mapSize)
    {
        selectedMapSize = _mapSize;
    }

    public int GetSelectedMapSize()
    {
        return selectedMapSize;
    }
    #endregion

}
