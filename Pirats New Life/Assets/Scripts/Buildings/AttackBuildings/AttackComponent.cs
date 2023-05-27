using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

public class AttackComponent : MonoBehaviour
{
    public AttackTypes Type { get { return type; } }
    public bool IsRight { get { return isRight; } }

    [SerializeField] private BuildNeedGoldShow _BuildNeedGoldShow;
    [SerializeField] private int countOfGold = 1;
    [SerializeField] private string _name = "Attack";
    [SerializeField] private AttackTypes type = AttackTypes.ArcherType;
    [SerializeField] private bool isRight;


    private int curGold = 0;
    private bool canProduce = false;
    private Action _action;
    private bool _checkOneTime = true;

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
    public void SetCurGoldText(int gold)
    {
        curGold = gold;
        _BuildNeedGoldShow.SetGoldText(curGold.ToString() + "/" + countOfGold.ToString());
    }
    public void SetGoldText(int gold)
    {
        countOfGold = gold;
        _BuildNeedGoldShow.SetGoldText(curGold.ToString() + "/" + countOfGold.ToString());
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
