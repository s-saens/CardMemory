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
            // ???????????? ???????????? ?????? ?????? ?????????
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
            // ?????? ?????? ?????? ??? ?????? ??????
            byte r = (byte)Random.Range(0, cardCount);
            // ???????????? ???????????? ?????? ?????? ?????????
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
            Debug.LogError("?????? ????????? 52????????? ?????? ??? ????????????! ?????? ?????? ????????? : " + cardCount / 2);
            return;
        }
        if (cardCount % 2 != 0)
        {
            Debug.LogError("???????????? ??? ????????????! ?????? ?????? : " + cardCount);
            return;
        }
        if (cardCount == 0)
        {
            Debug.LogError("0????????????");
            return;
        }

        // ?????? ?????? '??????' ??????
        SetRandomCardNumbers();
        // ?????? ?????? '??????' ??????
        SetRandomCardPos();

        // ???????????? ?????? ???????????? ?????????, ???????????? ?????? ???????????? ??????
        foreach (byte sel in randomCardNumbersList)
        {
            // ?????? ????????? ????????? ?????? ?????????.
            InstantiateSelectedCard(sel);
            InstantiateSelectedCard(sel);
        }

        void InstantiateSelectedCard(byte sel)
        {
            byte num = (byte)(sel % 13 + 1);
            CardType type = (CardType)(sel / 13);
            GameObject cardObj = Instantiate(cardPrefab, Vector2.zero, Quaternion.Euler(0, 0, 0));
            // ???????????????????????? ????????????
            cardObj.transform.SetParent(gamePanel.transform);
            // ?????? ??? ?????? ??????
            SetSizeAndPos(cardObj, randomPosList.Pop());

            Card cardComponent = cardObj.GetComponent<Card>();
            cardComponent.InitCard(num, type, this.OnCardClick);
            cardList.Add(cardObj);
        }

        // GamePanel??? ????????? ?????? ????????? ??????
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

        // ?????????!
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

        // ?????????! ??????
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