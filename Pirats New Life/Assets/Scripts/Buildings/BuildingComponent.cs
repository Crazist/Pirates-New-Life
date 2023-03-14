using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameInit.Building;
using System;

public class BuildingComponent : MonoBehaviour
{
    [SerializeField] private List<GameObject> formsList;
    [SerializeField] private int countOfGold = 1;
    [SerializeField] private int maxLevel = 2;
    [SerializeField] BuildingsType type;
    private Coin coin;
    private int curForm = 0;
    private bool checkForGold = false;
    private int curGold = 0;
    private List<Coin> curCoinsList;
    private Action _action;

    private void Start()
    {
        curCoinsList = new List<Coin>();
    }

    public void SetAction(Action action)
    {
       _action = action;
    }

    public void ChekMaxLvl()
    {
        if (curForm == maxLevel)
        {
            this.enabled = false;
        }
    }

    public void UpdateBuild()
    {
        formsList[curForm].gameObject.SetActive(false);
        curForm++;
        formsList[curForm].gameObject.SetActive(true);
    }

    private void OnTriggerEnter(Collider other)
    {
        var _coin = other.gameObject.GetComponent<Coin>();
        if (_coin)
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

        if (checkForGold)
        {
            StopCoroutine(GoldCollectorWaiter());
            StartCoroutine(GoldCollectorWaiter());
        }
        else
        {
            StartCoroutine(GoldCollectorWaiter());
        }
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
        yield return new WaitForSecondsRealtime(2);
        if(curGold == countOfGold)
        {
            _action.Invoke();
            StartCoroutine(BuildingInProgress());
        }
        else
        {
            foreach (var coin in curCoinsList)
            {
                coin.Active();
            }
        }
        curCoinsList.Clear();
        curGold = 0;
        checkForGold = false;
        StopCoroutine(GoldCollectorWaiter());
    }
    private IEnumerator BuildingInProgress()
    {
        yield return new WaitForSecondsRealtime(4);
        UpdateBuild();
        StopCoroutine(BuildingInProgress());
    }
}
