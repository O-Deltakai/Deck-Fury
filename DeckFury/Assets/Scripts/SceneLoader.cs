using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;

public class SceneLoader : MonoBehaviour
{
    [SerializeField] Image loadingPanel;
    [SerializeField] Slider loadingBar;

    [SerializeField] float fadeToBlackTime = 1;
    [SerializeField] float fadeToTransparentTime = 1;

    GameManager gameManager;


    public bool fadingIn;
    public bool fadingOut;



    private void Start() 
    {
        gameManager = GameManager.Instance;    
    }

    private void Update() 
    {
        if(fadingIn)
        {
            FadeToBlack();
            fadingIn = false;
        }    
        if(fadingOut)
        {
            FadeToTransparent();
            fadingOut = false;
        }

    }

    public void LoadScene(int levelIndex)
    {
        StartCoroutine(LoadSceneAsynchronously(levelIndex));
    }
    public void LoadScene(string sceneName)
    {
        StartCoroutine(LoadSceneAsynchronously(sceneName));
    }
    public void LoadScene(SceneNames sceneName)
    {
        StartCoroutine(LoadSceneAsynchronously(sceneName.ToString()));
    }
    public void LoadSceneAdditive(string sceneName)
    {
        StartCoroutine(LoadSceneAsyncAdditive(sceneName));
    }

    /// <summary>
    /// Unload the currently active scene and set another scene by name be the active one.
    /// <para><b>Should only be used on scenes that were loaded additively</b>.</para>
    /// </summary>
    /// <param name="sceneToActivateName"></param>
    public void UnloadActiveSceneThenReturn(string sceneToActivateName)
    {
        StartCoroutine(UnloadSceneAsyncAdditive(SceneManager.GetActiveScene().name, sceneToActivateName));
    }

    public void ChangeSceneAdditive(string sceneToActivateName)
    {
        StartCoroutine(UnloadSceneAdditiveThenChange(SceneManager.GetActiveScene().name, sceneToActivateName));
    }

    public void ReloadCurrentScene()
    {
        gameManager.DisableMenuCanvasGraphicRaycaster();
        StartCoroutine(LoadSceneAsynchronously(SceneManager.GetActiveScene().buildIndex));     
    }


    IEnumerator LoadSceneAsynchronously(int levelIndex)
    {
        FadeToBlack();
        yield return new WaitForSecondsRealtime(fadeToBlackTime);

        AsyncOperation loadSceneOperation = SceneManager.LoadSceneAsync(levelIndex, LoadSceneMode.Single);

        loadSceneOperation.allowSceneActivation = false;

        loadingBar.gameObject.SetActive(true);
        while(!loadSceneOperation.isDone)
        {
            if(loadSceneOperation.progress >= 0.9f)
            {
                loadSceneOperation.allowSceneActivation = true; 
            }
            loadingBar.value = loadSceneOperation.progress;
            yield return null;
        }
        
        loadingBar.gameObject.SetActive(false);
        yield return new WaitForSecondsRealtime(fadeToBlackTime);
        FadeToTransparent();
    }
    IEnumerator LoadSceneAsynchronously(string sceneName)
    {
        FadeToBlack();
        yield return new WaitForSecondsRealtime(fadeToBlackTime);

        AsyncOperation loadSceneOperation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
        loadingBar.gameObject.SetActive(true);

        loadSceneOperation.allowSceneActivation = false;

        while(!loadSceneOperation.isDone)
        {
            if(loadSceneOperation.progress >= 0.9f)
            {
                loadSceneOperation.allowSceneActivation = true; 
            }

            loadingBar.value = loadSceneOperation.progress;
            yield return null;
        }

        loadingBar.gameObject.SetActive(false);
        yield return new WaitForSecondsRealtime(fadeToBlackTime);
        FadeToTransparent();
    }

    IEnumerator LoadSceneAsyncAdditive(string sceneToAdd)
    {
        FadeToBlack();
        yield return new WaitForSecondsRealtime(fadeToBlackTime);





        AsyncOperation loadSceneOperation = SceneManager.LoadSceneAsync(sceneToAdd, LoadSceneMode.Additive);
        loadSceneOperation.allowSceneActivation = false; 
        loadingBar.gameObject.SetActive(true);

        while(!loadSceneOperation.isDone)
        {
            if(loadSceneOperation.progress >= 0.9f)
            {
              

                loadSceneOperation.allowSceneActivation = true; 
            }

            loadingBar.value = loadSceneOperation.progress;
            yield return null;
        }

        Scene additiveScene = SceneManager.GetSceneByName(sceneToAdd);
        SceneManager.SetActiveScene(additiveScene);  

        DisableAllObjectsInScene("StageSelectionScene");



        loadingBar.gameObject.SetActive(false);
        yield return new WaitForSecondsRealtime(fadeToBlackTime);
        FadeToTransparent();

    }

    void DisableAllObjectsInScene(string sceneName)
    {
        Scene scene = SceneManager.GetSceneByName(sceneName);
        foreach (GameObject rootObject in scene.GetRootGameObjects())
            {

                rootObject.SetActive(false);
            }        
    }
    void EnableAllObjectsInScene(string sceneName)
    {
        Scene scene = SceneManager.GetSceneByName(sceneName);
        foreach (GameObject rootObject in scene.GetRootGameObjects())
            {

                rootObject.SetActive(true);
            }          
    }

    IEnumerator UnloadSceneAsyncAdditive(string unloadSceneName, string sceneToActivateName)
    {
        FadeToBlack();
        yield return new WaitForSecondsRealtime(fadeToBlackTime);

        AsyncOperation unloadSceneOperation = SceneManager.UnloadSceneAsync(unloadSceneName);

        loadingBar.gameObject.SetActive(true);

        while(!unloadSceneOperation.isDone)
        {
            loadingBar.value = unloadSceneOperation.progress;
            yield return null;
        }

        Scene activeSceneName = SceneManager.GetSceneByName(sceneToActivateName);
        SceneManager.SetActiveScene(activeSceneName);

        EnableAllObjectsInScene(sceneToActivateName);
        
        loadingBar.gameObject.SetActive(false);
        yield return new WaitForSecondsRealtime(fadeToBlackTime);
        FadeToTransparent();


    }

    IEnumerator UnloadSceneAdditiveThenChange(string unloadSceneName, string nextSceneToLoad)
    {
        FadeToBlack();
        yield return new WaitForSecondsRealtime(fadeToBlackTime);

        AsyncOperation unloadSceneOperation = SceneManager.UnloadSceneAsync(unloadSceneName);

        loadingBar.gameObject.SetActive(true);

        while(!unloadSceneOperation.isDone)
        {
            loadingBar.value = unloadSceneOperation.progress;
            yield return null;
        }

        yield return new WaitForSecondsRealtime(fadeToBlackTime);
        LoadSceneAdditive(nextSceneToLoad);



    }


    void FadeToBlack()
    {
        loadingPanel.DOFade(1, fadeToBlackTime).SetUpdate(true);   
    }

    void FadeToTransparent()
    {
        loadingPanel.DOFade(0, fadeToTransparentTime).SetUpdate(true);
    }


}
