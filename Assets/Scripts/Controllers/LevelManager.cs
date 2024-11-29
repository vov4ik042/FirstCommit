using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum Scenes
{
    MainMenu,
    Game
};

public class LevelManager : MonoBehaviour
{
    private static float _fadeSpeed = 0.02f;
    private static Color _fadeTransperancy = new Color(0,0,0,0.1f);
    private static AsyncOperation _asyncOperation;

    public static LevelManager Instance;
    public GameObject _faderObj;
    public Image _faderImg;
    void Start()
    {
        DontDestroyOnLoad(this);
        Instance = this;
        SceneManager.sceneLoaded += OnLevelFionishedLoading;
        PlayScene(Scenes.MainMenu);
    }

    public static void PlayScene(Scenes sceneEnum)
    {
        Instance.LoadScene(sceneEnum.ToString());
    }

    private void LoadScene(string sceneName)
    {
        Instance.StartCoroutine(Load(sceneName));
        Instance.StartCoroutine(FadeOut(Instance._faderObj, Instance._faderImg));
    }

    private static IEnumerator FadeOut(GameObject faderObject, Image faderImg)
    {
        faderObject.SetActive(true);//Занавес прозрачный
        while(faderImg.color.a < 1)
        {
            faderImg.color += _fadeTransperancy;
            yield return new WaitForSeconds(_fadeSpeed);
        }

        ActivateScene();
    }

    private void OnLevelFionishedLoading(Scene scene, LoadSceneMode mode)
    {
        Instance.StartCoroutine(FadeIn(Instance._faderObj, Instance._faderImg));
    }

    private static IEnumerator FadeIn(GameObject faderObject, Image faderImg)
    {
        faderObject.SetActive(true);//Занавес прозрачный
        while (faderImg.color.a > 0)
        {
            faderImg.color -= _fadeTransperancy;
            yield return new WaitForSeconds(_fadeSpeed);
        }
        faderObject.SetActive(false);
    }

    private static IEnumerator Load(string sceneName)
    {
        _asyncOperation = SceneManager.LoadSceneAsync(sceneName);
        _asyncOperation.allowSceneActivation = false;
        yield return _asyncOperation;
    }

    private static void ActivateScene()
    {
        _asyncOperation.allowSceneActivation = true;
    }
}