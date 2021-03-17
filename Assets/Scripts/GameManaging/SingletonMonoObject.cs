using UnityEngine;

public class SingletonMonoObject : MonoBehaviour
{
    private static SingletonMonoObject instance = null;
    public static SingletonMonoObject Instance
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
}