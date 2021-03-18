
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    public byte number; // 1~13
    public CardType type; // Spade, Heart, Diamond, Club // <- UpperCase
    public CardState state;

    private CardGameManager cardGameManager;
    private Sprite frontSprite;
    public Sprite backSprite;

    public void InitCard(byte number, CardType type, CardGameManager cardGM)
    {
        this.cardGameManager = cardGM;
        this.number = number;
        this.type = type;
        this.state = CardState.BACK;

        this.transform.localScale = Vector3.one;

        SetSprite();
        this.GetComponent<Image>().sprite = backSprite;
    }

    private void SetSprite()
    {
        string typeName = GetTypeName();
        string numberString = GetNumberString();
        string folderPath = Path.Combine("Images", typeName);
        string fileName = typeName + numberString;
        string filePath = Path.Combine(folderPath, fileName);
        frontSprite = Resources.Load<Sprite>(filePath);

        string GetTypeName() {
            switch(this.type) {
                case CardType.SPADE: return "Spade";
                case CardType.HEART: return "Heart";
                case CardType.DIAMOND: return "Diamond";
                case CardType.CLUB: return "Club";
            }
            return null;
        }

        string GetNumberString() {
            string ret = "";
            ret += (this.number/10).ToString(); // 첫째자리
            ret += (this.number%10).ToString(); // 둘째자리
            return ret;
        }
    }

    public void OnClick()
    {
        cardGameManager.OnCardClick(this);
    }

    public void FlipFront()
    {
        this.state = CardState.FRONT;
        this.GetComponent<Image>().sprite = frontSprite;
    }
    public void FlipBack()
    {
        this.state = CardState.BACK;
        this.GetComponent<Image>().sprite = backSprite;
    }

    public static bool CheckSame(Card c1, Card c2)
    {
        if(c1.number != c2.number) return false;
        if(c1.type != c2.type) return false;

        return true;
    }

    
}
