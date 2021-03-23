using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class CardGameManager : MonoBehaviour
{
    public Timer timer;
    public CardGameStageInfo stageInfo;
    private byte level;
    private Vector2 xyCounts;
    private int cardCount;

    public GameObject gamePanel;
    public GameObject cardPrefab;

    private byte remainCardPairs;
    private Card selectedCard = null;

    List<byte> randomCardNumbersList = new List<byte>();
    Stack<byte> randomPosList = new Stack<byte>();
    List<GameObject> cardList = new List<GameObject>();

    private void Start()
    {
        Debug.Log("start");
        level = stageInfo.level;
        xyCounts = stageInfo.xyCounts;
        cardCount = (int)(xyCounts.x * xyCounts.y);
        timer.StartTimer(stageInfo.limitTime, () => {
            EndGame();
            Manager.failed = true;
        } );
        remainCardPairs = (byte)(cardCount / 2);

        SetGamePanelSize();
        InstantiateCards();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            Hint();
        }
        else
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            RefreshCards();
        }
    }

    private void Hint()
    {
        foreach(GameObject c in cardList)
        {
            if (selectedCard != null)
            {
                selectedCard = null;
            }
            c.GetComponent<Card>().HintFlip();
            Manager.score -= 20;
        }
    }

    private void RefreshCards()
    {
        if(cardList[0].GetComponent<Card>().isHinting) return;

        SetRandomCardNumbers();
        SetRandomCardPos();
        selectedCard = null;
        remainCardPairs = (byte)(cardCount / 2);

        Manager.Instance.flipBackSound.Play();
        for(int i=0 ; i < cardCount/2 ; ++i)
        {
            byte randomNum = randomCardNumbersList[i];
            byte newNum = (byte) (randomNum % 13 + 1);
            CardType newType = (CardType)(randomNum / 13);
            cardList[randomPosList.Pop()].GetComponent<Card>().UpdateCard(newNum,newType);
            cardList[randomPosList.Pop()].GetComponent<Card>().UpdateCard(newNum,newType);
        }
    }

    private void SetRandomCardNumbers()
    {
        randomCardNumbersList.Clear();
        for (byte i = 0; i < cardCount / 2; ++i)
        {
            byte r = (byte)Random.Range(0, 52);
            // 중복된걸 뽑았다면 랜덤 다시 돌리기
            if (randomCardNumbersList.Contains<byte>(r))
            {
                --i;
                continue;
            }
            randomCardNumbersList.Add(r);
        }
    }

    private void SetRandomCardPos()
    {
        for (byte i = 0; i < cardCount; ++i)
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
            Debug.Log(i);
        }
    }

    // Fixed Ratio
    private void SetGamePanelSize()
    {
        RectTransform bgRect = gamePanel.GetComponent<RectTransform>();

        float bgW = bgRect.rect.width;
        float bgH = bgRect.rect.height;

        float bgRatio = bgW / bgH;
        float cardCntRatio = this.xyCounts.x / this.xyCounts.y;

        if (bgRatio < cardCntRatio) bgH = bgW / cardCntRatio;
        else if (bgRatio > cardCntRatio) bgW = bgH * cardCntRatio;

        bgRect.sizeDelta = new Vector2(bgW, bgH);
    }

    // Randomly Instantiate Cards on Canvas
    private void InstantiateCards()
    {
        if (cardCount > 104)
        {
            Debug.LogError("카드 종류는 52가지를 넘을 수 없습니다! 카드 종류 가지수 : " + cardCount / 2);
            return;
        }
        if (cardCount % 2 != 0)
        {
            Debug.LogError("홀수개일 수 없습니다! 카드 개수 : " + cardCount);
            return;
        }
        if (cardCount == 0)
        {
            Debug.LogError("0개입니다");
            return;
        }

        // 중복 없이 '카드' 뽑기
        SetRandomCardNumbers();
        // 중복 없이 '자리' 뽑기
        SetRandomCardPos();

        // 랜덤으로 뽑힌 카드들을 초기화, 생성하고 멤버 리스트에 넣기
        foreach (byte sel in randomCardNumbersList)
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
            cardObj.transform.SetParent(gamePanel.transform);
            // 크기 및 위치 지정
            SetSizeAndPos(cardObj, randomPosList.Pop());

            Card cardComponent = cardObj.GetComponent<Card>();
            cardComponent.InitCard(num, type, this.OnCardClick);
            cardList.Add(cardObj);
        }

        // GamePanel의 크기에 맞춘 크기와 위치
        void SetSizeAndPos(GameObject c, byte pos)
        {
            RectTransform bgRect = gamePanel.GetComponent<RectTransform>();
            RectTransform cardRect = c.GetComponent<RectTransform>();

            float bgW = bgRect.rect.width;
            float bgH = bgRect.rect.height;

            float cardW = bgW / this.xyCounts.x;
            float cardH = bgH / this.xyCounts.y;

            byte posX = (byte)(pos % this.xyCounts.x);
            byte posY = (byte)(pos / this.xyCounts.x);

            float realX = (cardW / 2) - (bgW / 2) + posX * cardW;
            float realY = (cardH / 2) - (bgH / 2) + posY * cardH;

            cardRect.sizeDelta = new Vector2(cardW, cardH);
            cardRect.localPosition = new Vector2(realX, realY);
        }
    }

    // Game Logic
    public void OnCardClick(Card card)
    {

        if (selectedCard == null)
        {
            selectedCard = card;
            return;
        }

        // 맞췄다!
        if (Card.CheckSame(card, selectedCard))
        {
            card.state = CardState.DONE;
            selectedCard.state = CardState.DONE;
            selectedCard = null;
            remainCardPairs--;
            Manager.score += level * 100;
            if (remainCardPairs <= 0)
            {
                this.NextLevel();
            }
            return;
        }

        // 틀렸다! ㅠㅠ
        else
        {
            Manager.score -= 10;
            card.FlipBack();
            selectedCard.FlipBack();
            selectedCard = null;
            return;
        }
    }


    // Ending //
    private void EndGame()
    {
        const int endSceneNumber = 6;
        Manager.Instance.SceneMove(endSceneNumber);
    }
    private void NextLevel()
    {
        Manager.score += (int)(timer.StopTimer()) * 100;
        Manager.Instance.SceneMove(level + 1);
        Debug.Log("YEAH!");
    }

}