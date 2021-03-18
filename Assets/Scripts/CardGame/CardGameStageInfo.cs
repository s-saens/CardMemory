using UnityEngine;

[CreateAssetMenu(fileName = "CardGameStageInfo", menuName = "Scriptable Object Asset/CardGameStageInfo")]

public class CardGameStageInfo : ScriptableObject
{
    public byte level;
    public Vector2 xyCounts;
    public int limitTime;
}
