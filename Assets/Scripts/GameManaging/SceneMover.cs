using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneMover : MonoBehaviour
{
    /* Scene Move Fade Effect */

    private RawImage blackPanel;
    private IEnumerator FadeInEffect;
    private bool isFadingOut = false;
    private float fadeSpeed = 1.5f;

    public void FadeInOnLoad()
    {
        isFadingOut = false;

        blackPanel = GameObject.FindWithTag("BlackPanel").GetComponent<RawImage>();
        blackPanel.color = Color.black;

        FadeInEffect = FadeIn();
        StartCoroutine(FadeInEffect);

        IEnumerator FadeIn()
        {
            while (blackPanel.color.a > 0)
            {
                blackPanel.color -= new Color(0, 0, 0, 1) * Time.fixedDeltaTime * fadeSpeed;
                yield return new WaitForFixedUpdate();
            }
            Debug.Log("Faded in!");
        }
    }

    public void FadeOutAndLoad(int sceneIndex, Action action)
    {
        if (isFadingOut) return;

        if (sceneIndex == SceneManager.GetActiveScene().buildIndex)
        {
            Debug.Log("already in that scene");
            return;
        }

        // FadeIn 중이었다면, 종료!
        StopCoroutine(FadeInEffect);

        StartCoroutine(FadeOut());

        IEnumerator FadeOut()
        {
            isFadingOut = true;
            while (blackPanel.color.a < 1)
            {
                blackPanel.color += new Color(0, 0, 0, 1) * Time.fixedDeltaTime * fadeSpeed;
                yield return new WaitForFixedUpdate();
            }
            SceneManager.LoadSceneAsync(sceneIndex);
            action();
        }
    }
}