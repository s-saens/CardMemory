using System;
using UnityEngine;

public class Manager : MonoBehaviour
{
    private static Manager instance = null;
    public static Manager Instance
    {
        get
        {
            return instance;
        }
    }
    protected void UpdateSingleton()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    private SceneMover sceneMover;
    public static int score = 0;
    public static bool failed = false;

    public AudioSource flipFrontSound;
    public AudioSource flipBackSound;
    public AudioSource completeSound;
    public AudioSource failSound;

    private void Awake()
    {
        Application.targetFrameRate = 80;
        sceneMover = this.GetComponent<SceneMover>();
        sceneMover.FadeInOnLoad();
        UpdateSingleton();
    }

    public void SceneMove(int index)
    {
        sceneMover.FadeOutAndLoad(index, () => this.Awake());
    }

}