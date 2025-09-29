using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
    public static LevelLoader loader;
    public enum Scenes { MainMenu, Game }


    [Header("LOADER OBJECT")]
    [SerializeField] CanvasGroup _loaderCanvas;
    [SerializeField] Image _progressBar;
    float _target;

    private void Awake()
    {
        if (loader == null)
        {
            loader = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (loader != this)
        {
            Destroy(this.gameObject);
        }
    }

    public static void LoadScene(Scenes newScene)
    {
        loader.LoadSceneLocal(newScene);
    }

    void LoadSceneLocal(Scenes newScene)
    {
        StartCoroutine(Load(newScene));
    }

    IEnumerator Load(Scenes newScene)
    {
        bool alphaAnimDone = false;
        _loaderCanvas.gameObject.SetActive(true);
        _loaderCanvas.alpha = 0;
        LeanTween.alphaCanvas(_loaderCanvas, 1, .5f).setEase(LeanTweenType.easeOutExpo).setOnComplete(done =>
        {
            alphaAnimDone = true;
        });
        while (!alphaAnimDone)
        {
            yield return null;
        }

        Debug.Log("Start Scene Loading");
        SceneManager.LoadScene((int)newScene);

        //var scene = SceneManager.LoadSceneAsync((int)newScene);
        //scene.allowSceneActivation = false;
        //do
        //{
        //    _target = scene.progress + 0.1f;
        //    Debug.Log("Loading: " + scene.progress);
        //    yield return null;
        //} while (scene.progress < 0.9f);

        //scene.allowSceneActivation = true;
        LeanTween.alphaCanvas(_loaderCanvas, 0, .5f).setEase(LeanTweenType.easeInExpo).setOnComplete(done =>
        {
            _loaderCanvas.gameObject.SetActive(false);
        });
    }

    private void Update()
    {
        if (_progressBar == null) return;
        _progressBar.fillAmount = Mathf.MoveTowards(_progressBar.fillAmount, _target, 3 * Time.deltaTime);
    }
}
