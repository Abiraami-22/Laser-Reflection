using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class FadeManager : MonoBehaviour
{
    public static FadeManager Instance;

    public float fadeDuration = 2f;

    private CanvasGroup fadeGroup;
    private Coroutine currentFade;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        Time.timeScale = 1f;

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Always re-find CanvasGroup after scene load
        fadeGroup = FindFirstObjectByType<CanvasGroup>();

        if (fadeGroup == null)
            return;

        fadeGroup.alpha = 1f;
        fadeGroup.interactable = false;
        fadeGroup.blocksRaycasts = true;

        if (currentFade != null)
            StopCoroutine(currentFade);

        currentFade = StartCoroutine(FadeIn());
    }

    public void FadeToScene(string sceneName)
    {
        if (fadeGroup == null)
            return;

        if (currentFade != null)
            StopCoroutine(currentFade);

        currentFade = StartCoroutine(FadeOutAndLoad(sceneName));
    }

    IEnumerator FadeOutAndLoad(string sceneName)
    {
        yield return Fade(1f);
        SceneManager.LoadScene(sceneName);
    }

    IEnumerator FadeIn()
    {
        yield return Fade(0f);
        fadeGroup.blocksRaycasts = false;
    }

    IEnumerator Fade(float target)
    {
        float start = fadeGroup.alpha;
        float t = 0f;

        while (t < fadeDuration)
        {
            t += Time.unscaledDeltaTime;

            if (fadeGroup == null)
                yield break;

            fadeGroup.alpha = Mathf.Lerp(start, target, t / fadeDuration);
            yield return null;
        }

        fadeGroup.alpha = target;
    }
}
