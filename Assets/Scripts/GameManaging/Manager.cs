using System;
using UnityEngine;

public class Manager : SingletonMonoObject
{
    SceneMover sceneMover;
    HighscoreManager highscoreManager;

    private void Awake()
    {
        sceneMover = this.GetComponent<SceneMover>();
        sceneMover.FadeInOnLoad();
        UpdateSingleton();
    }

    public void SceneMove(int index) {
        sceneMover.FadeOutAndLoad(index, () => this.Awake());
    }

}