using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameInit.Building;
using System;
using Cysharp.Threading.Tasks;

public class BuildingComponent : MonoBehaviour
{
    [SerializeField] private List<GameObject> formsList;
    [SerializeField] private List<Transform> _positionForBuildFirst;
    [SerializeField] private List<Transform> _positionForBuildSecond;
    [SerializeField] private List<Transform> _positionForBuildThird;
    [SerializeField] private List<Transform> _positionForBuildFourth;
    [SerializeField] private int countOfGold = 2;
    [SerializeField] private int maxLevel = 1;
    [SerializeField] BuildingsType type;
    [SerializeField] private bool _currentlyBuilding = false;
    [SerializeField] private bool _momentalBuild = true;
    [SerializeField] private BuildNeedGoldShow _BuildNeedGoldShow;

    public int BaseCount { get; set; } = 1;

    private bool canProduce = true;
    private Coin coin;
    private int curForm = 0;
    private bool checkForGold = false;
    private int curGold = 0;
    private Action _action;
    private bool inBuild = false;
    private Action<int> _dropBeforePickUp;
    
    public GameObject GetGm()
    {
        return gameObject;
    }

    private void Start()
    {
        BaseCount = countOfGold;
    }

    public List<Transform> GetBuildPositions()
    {
        switch (curForm - 1)
        {
            case 0:
                return _positionForBuildFirst;
            case 1:
                return _positionForBuildSecond;
            case 2:
                return _positionForBuildThird;
            case 3:
                return _positionForBuildFourth;
        }
        return _positionForBuildFirst;
    }
    
    public int GetCurForm()
    {
        return curForm;
    }
    public void SetInBuild(bool _inBuild)
    {
        if (_inBuild)
        {
            _BuildNeedGoldShow.DeactiveBuildingNeeds();
        }
        inBuild = _inBuild;
        _currentlyBuilding = _inBuild;
    }
    public void SetAction(Action action, Action<int> DropBeforePickUp)
    {
       _action = action;
       _dropBeforePickUp = DropBeforePickUp;
    }
    public List<GameObject> GetFormList()
    {
        return formsList;
    }
    public void IncreaseForm()
    {
        curForm++;
    }
    public void SetForm(int form)
    {
        curForm = form;
    }
    public void ResetForm()
    {
        formsList[curForm].gameObject.SetActive(false);
        curForm = 0;
        formsList[curForm].gameObject.SetActive(true);
    }
    public bool ChekMaxLvl()
    {
        if (curForm == maxLevel)
        {
            this.enabled = false;
            canProduce = false;
            _BuildNeedGoldShow.DeactiveBuildingNeeds();
            return true;
        }
        return false;
    }
    public MonoBehaviour GetMonoBehaviour()
    {
        return this;
    }
    public void UpdateBuild()
    {
        formsList[0].gameObject.SetActive(false);
        formsList[curForm - 1].gameObject.SetActive(false);
        formsList[curForm].gameObject.SetActive(true);
    }

    public void SetCanProduce(bool canProd)
    {
        if (!canProd && _BuildNeedGoldShow != null)
        {
            _BuildNeedGoldShow.DeactiveBuildingNeeds();
        }
        canProduce = canProd;
        inBuild = false;
    }
    public bool CanProduce()
    {
        return canProduce;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!canProduce || curGold == countOfGold || _currentlyBuilding)
        {
            return;
        }
       
        if (other.TryGetComponent(out HeroComponent hero))
        {
            _BuildNeedGoldShow.ActiveBuildingNeeds();
            _BuildNeedGoldShow.SetGoldText(curGold.ToString() + "/" + countOfGold.ToString());
            _BuildNeedGoldShow.SetNameText(type.ToString());
        }

        var _coin = other.gameObject.GetComponent<Coin>();
        if (_coin && !_coin.SecondTouch)
        {
            coin = _coin;
            coin.Hide();
            GoldCollects();
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (!canProduce || curGold == countOfGold || _currentlyBuilding)
        {
            return;
        }

        if (other.TryGetComponent(out HeroComponent hero))
        {
            _BuildNeedGoldShow.DeactiveBuildingNeeds();
        }
    }
   
    private void GoldCollects()
    {
        curGold++;
        _BuildNeedGoldShow.SetGoldText(curGold.ToString() + "/" + countOfGold.ToString());
        if (!inBuild)
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
            if (_momentalBuild)
            {
                _currentlyBuilding = true;
                StartCoroutine(BuildingInProgress());
            }
        }
        else
        {
            _dropBeforePickUp.Invoke(curGold);
        }
        curGold = 0;
        _BuildNeedGoldShow.SetGoldText(curGold.ToString() + "/" + countOfGold.ToString());
    }
    private IEnumerator BuildingInProgress()
    {
        yield return new WaitForSecondsRealtime(2);
        UpdateBuild();
        inBuild = false;
        _currentlyBuilding = false;
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }
}