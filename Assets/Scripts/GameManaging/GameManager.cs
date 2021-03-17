using System;
using UnityEngine;

public class GameManager : SingletonMonoObject
{
    SceneMover sceneMover;

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