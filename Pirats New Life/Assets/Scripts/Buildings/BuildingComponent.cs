using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameInit.Building;
using System;

public class BuildingComponent : MonoBehaviour
{
    [SerializeField] private List<GameObject> formsList;
    [SerializeField] private int countOfGold = 2;
    [SerializeField] private int maxLevel = 1;
    [SerializeField] BuildingsType type;
    private bool canProduce = true;
    private Coin coin;
    private int curForm = 0;
    private bool checkForGold = false;
    private int curGold = 0;
    private List<Coin> curCoinsList;
    private Action _action;
    private bool inBuild = false;

    private void Start()
    {
        curCoinsList = new List<Coin>();
    }

    public void SetAction(Action action)
    {
       _action = action;
    }

    public bool ChekMaxLvl()
    {
        if (curForm == maxLevel)
        {
            this.enabled = false;
            canProduce = false;
            return true;
        }
        return false;
    }

    public void UpdateBuild()
    {
        formsList[curForm - 1].gameObject.SetActive(false);
        formsList[curForm].gameObject.SetActive(true);
    }

    public bool CanProduce()
    {
        return canProduce;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!canProduce || curGold == countOfGold) return;

        var _coin = other.gameObject.GetComponent<Coin>();
        if (_coin && !_coin.SecondTouch)
        {
            coin = other.gameObject.GetComponent<Coin>();
            curCoinsList.Add(coin);
            coin.Hide();
            GoldCollects();
        }
    }

    private void GoldCollects()
    {
        curGold++;
        if(!inBuild)
        StartCoroutine(GoldCollectorWaiter());
    }
    
    public BuildingsType getType()
    {
        return type;
    }

    public int GetCurCountOFGold()
    {
        return countOfGold;
    }

    public void SetCountForGold(int goldCount)
    {
        countOfGold = goldCount;
    }

    private IEnumerator GoldCollectorWaiter()
    {
        checkForGold = true;
        inBuild = true;
        int gold = 0;
        do
         {
            if(gold < curGold)
            {
                gold++;
                yield return new WaitForSecondsRealtime(1);
            }
            else
            {
                checkForGold = false;
            }
        }
        while (checkForGold);

        if(curGold == countOfGold)
        {
            curForm++;
            _action.Invoke();
            StartCoroutine(BuildingInProgress());
        }
        else
        {
            foreach (var coin in curCoinsList)
            {
                coin.Active();
                coin.SecondTouch = true;
                inBuild = false;
            }
        }
        curCoinsList.Clear();
        curGold = 0;
        StopCoroutine(GoldCollectorWaiter());
    }
    private IEnumerator BuildingInProgress()
    {
        yield return new WaitForSecondsRealtime(2);
        UpdateBuild();
        inBuild = false;
        StopCoroutine(BuildingInProgress());
    }
}
