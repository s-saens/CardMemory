
using System.IO;
using System.Collections;
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
        if(this.state == CardState.BACK)
        {
            FlipFront();
        }
    }

    private bool isFlipping = false;

    public void FlipFront()
    {
        if(isFlipping) return;
        this.state = CardState.FRONT;
        StartCoroutine(FlipCoroutine(frontSprite));
    }

    public void FlipBack()
    {
        this.state = CardState.BACK;
        StartCoroutine(FlipCoroutine(backSprite));
    }

    IEnumerator FlipCoroutine(Sprite sprite)
    {
        const byte speed = 3;
        isFlipping = true;
        for (int i = 0; i < 90 / speed; i++)
        {
            isFlipping = true;
            this.transform.Rotate(Vector2.up * speed);
            float rot = this.transform.rotation.eulerAngles.y;
            yield return 0;
        }
        
        this.GetComponent<Image>().sprite = sprite;

        for (int i = 0; i < 90 / speed; i++)
        {
            isFlipping = true;
            this.transform.Rotate(-Vector2.up * speed);
            float rot = this.transform.rotation.eulerAngles.y;
            yield return 0;
        }

        if(this.state == CardState.FRONT) cardGameManager.OnCardClick(this);
        isFlipping = false;
    }

    public static bool CheckSame(Card c1, Card c2)
    {
        if(c1.number != c2.number) return false;
        if(c1.type != c2.type) return false;

        return true;
    }

    
}
