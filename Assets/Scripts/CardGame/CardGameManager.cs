using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class CardGameManager : MonoBehaviour
{
    public Timer timer;
    public CardGameStageInfo stageInfo;
    private byte level ;
    private Vector2 xyCounts;
    public GameObject backGround;

    public GameObject cardPrefab;

    private void Start()
    {
        level = stageInfo.level;
        xyCounts = stageInfo.xyCounts;
        timer.limitTime = stageInfo.limitTime;
        timer.SetTimer();

        SetGamePanelSize();
        InstantiateCards();
    }
    private void EndGame()
    {

    }
    
    // Randomly Instantiate Cards on Canvas
    private void InstantiateCards()
    {
        int cardCount = (int)(xyCounts.x * xyCounts.y);
        if(cardCount > 104)
        {
            Debug.LogError("카드 종류는 52가지를 넘을 수 없습니다! 카드 종류 가지수 : " + cardCount/2);
            return;
        }
        if(cardCount % 2 != 0)
        {
            Debug.LogError("홀수개일 수 없습니다! 카드 개수 : " + cardCount);
            return;
        }
        if(cardCount == 0)
        {
            Debug.LogError("0개입니다");
            return;
        }

        // 중복 없이 '카드' 뽑기
        List<byte> randomCardList = new List<byte>();
        Stack<byte> randomPosList = new Stack<byte>();

        for (byte i = 0; i < cardCount / 2; ++i)
        {
            byte r = (byte)Random.Range(0, 52);
            // 중복된걸 뽑았다면 랜덤 다시 돌리기
            if(randomCardList.Contains<byte>(r))
            {
                --i;
                continue;
            }
            randomCardList.Add(r);
        }

        // 중복 없이 '자리' 뽑기
        for (byte i = 0; i < cardCount ; ++i)
        {
            // 가로 세로 개수 곱 만큼 뽑기
            byte r = (byte)Random.Range(0, cardCount);
            // 중복된걸 뽑았다면 랜덤 다시 돌리기
            if (randomPosList.Contains<byte>(r))
            {
                --i;
                continue;
            }
            randomPosList.Push(r);
        }

        // 랜덤으로 뽑힌 카드들을 초기화, 생성하고 멤버 리스트에 넣기
        foreach (byte sel in randomCardList)
        {
            // 같은 카드에 대해서 두번 해준다.
            InstantiateSelectedCard(sel);
            InstantiateSelectedCard(sel);
        }

        void InstantiateSelectedCard(byte sel)
        {
            byte num = (byte)(sel % 13 + 1);
            CardType type = (CardType)(sel / 13);
            GameObject cardObj = Instantiate(cardPrefab, Vector2.zero, Quaternion.Euler(0, 0, 0));
            // 게임백그라운드의 자식으로
            cardObj.transform.SetParent(backGround.transform);
            // 크기 및 위치 지정
            SetSizeAndPos(cardObj, randomPosList.Pop());

            Card cardComponent = cardObj.GetComponent<Card>();
            cardComponent.InitCard(num, type, this);
        }

        void SetSizeAndPos(GameObject c, byte pos)
        {
            RectTransform bgRect = backGround.GetComponent<RectTransform>();
            RectTransform cardRect = c.GetComponent<RectTransform>();

            float bgW = bgRect.rect.width;
            float bgH = bgRect.rect.height;
            
            float cardW = bgW / this.xyCounts.x;
            float cardH = bgH / this.xyCounts.y;

            byte posX = (byte)(pos%this.xyCounts.x);
            byte posY = (byte)(pos/this.xyCounts.x);

            float realX = (cardW/2) - (bgW/2) + posX * cardW;
            float realY = (cardH/2) - (bgH/2) + posY * cardH;

            cardRect.sizeDelta = new Vector2(cardW, cardH);
            cardRect.localPosition = new Vector2(realX, realY);
        }
    }

    private void SetGamePanelSize()
    {
        RectTransform bgRect = backGround.GetComponent<RectTransform>();

        float bgW = bgRect.rect.width;
        float bgH = bgRect.rect.height;

        float bgRatio = bgW / bgH;
        float cardCntRatio = this.xyCounts.x / this.xyCounts.y;
        Debug.Log(bgRatio);
        Debug.Log(cardCntRatio);

        if (bgRatio < cardCntRatio) bgH = bgW / cardCntRatio;
        else if( bgRatio > cardCntRatio) bgW = bgH * cardCntRatio;

        Debug.Log(bgW);
        Debug.Log(bgH);

        bgRect.sizeDelta = new Vector2(bgW, bgH);
    }

    // Game Logic
    public void OnCardClick(Card card)
    {
        Debug.Log(card.state);
        if(card.state == CardState.BACK) card.FlipFront();
    }


}