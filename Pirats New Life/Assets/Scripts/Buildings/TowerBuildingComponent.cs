using Cysharp.Threading.Tasks;
using GameInit.Building;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerBuildingComponent : MonoBehaviour
{
    [SerializeField] private BuildNeedGoldShow _BuildNeedGoldShow;
    [SerializeField] private Transform _position;
    [SerializeField] private List<GameObject> _forms;
    [SerializeField] private int countOfGold = 2;
    [SerializeField] private float timeToBuild = 10;
    [SerializeField] private string _name = "Tower";
    [SerializeField] private Transform _shootPosition;

    private int curGold = 0;
    private bool canProduce = false;
    private Action _action;
    private bool _checkOneTime = true;

    public Vector3 GetShootPosition()
    {
        return _shootPosition.position;
    }
    public List<GameObject> GetFormList()
    {
        return _forms;
    }
    public float GetTimeForBuild()
    {
        return timeToBuild;
    }
    public Vector3 GetBuildingsPosition()
    {
        return _position.position;
    }
    public int GetCountOfGold()
    {
        return countOfGold;
    }
    public void SetNewCountOfgold(int _gold)
    {
        countOfGold = _gold;
    }
    public void SetCurGold(int gold)
    {
        curGold = gold;
        _BuildNeedGoldShow.SetGoldText(curGold.ToString() + "/" + countOfGold.ToString());
    }
    public void SetAction(Action action)
    {
        _action = action;
    }
    public void SetCanProduce(bool canProd)
    {
        if (!canProd && _BuildNeedGoldShow != null)
        {
            _BuildNeedGoldShow.DeactiveBuildingNeeds();
        }
        canProduce = canProd;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (!canProduce)
        {
            return;
        }

        if (other.TryGetComponent(out HeroComponent hero))
        {
            _BuildNeedGoldShow.ActiveBuildingNeeds();
            _BuildNeedGoldShow.SetGoldText(curGold.ToString() + "/" + countOfGold.ToString());
            _BuildNeedGoldShow.SetNameText(_name);
        }

        var _coin = other.gameObject.GetComponent<Coin>();
        if (_coin && !_coin.SecondTouch)
        {
            _coin.Hide();
            _action.Invoke();
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (_checkOneTime && canProduce)
        {
            if (other.TryGetComponent(out HeroComponent hero))
            {
                WaitBeforeShow();
                _checkOneTime = false;
            }
        }
    }

    private async void WaitBeforeShow()
    {
        await UniTask.Delay(TimeSpan.FromSeconds(2));
        _BuildNeedGoldShow.ActiveBuildingNeeds();
        _BuildNeedGoldShow.SetGoldText(curGold.ToString() + "/" + countOfGold.ToString());
        _BuildNeedGoldShow.SetNameText(_name);
    }
    private void OnTriggerExit(Collider other)
    {
        if (!canProduce)
        {
            return;
        }

        if (other.TryGetComponent(out HeroComponent hero))
        {
            _BuildNeedGoldShow.DeactiveBuildingNeeds();
        }
    }
}
