using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CardData", menuName = "Scriptable Object/CardData", order = int.MaxValue)]

public class Card : MonoBehaviour
{
    [SerializeField]
    private string zombieName;
    public string ZombieName { get { return zombieName; } }

    [SerializeField]
    private int hp;
    public int Hp { get { return hp; } }
    
}
