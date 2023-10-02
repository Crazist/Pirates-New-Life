using System;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using GameInit.Animation;
using GameInit.Pool;
using GameInit.Connector;
using GameInit.GameCyrcleModule;
using GameInit.Builders;

namespace GameInit.Building
{
    public class TownHall : IDayChange
    {
        private BuildingTownHallComponent _BuildingTownHallComponent;
        private Action _action;
        private Action _playerCollectMoney;
        private GameCyrcle _cyrcle;
        private CoinDropAnimation _coinDropAnimation;
        private Pools _coinPool;
        private AIWarConnector _AIWarConnector;
        private CancellationToken token;
        private int _curGold = 0;
        private bool checkForGold = false;
        private int curForm = 0;
        private bool _dropMoney = true;

        private Action townHallFinishBuild;

        private const float _heightPosition = 0.46f;
        private const bool _canPickUp = false;
        private const int _firstForm = 1;
        private const int _moneyWaitDelay = 1;
        public TownHall(BuildingTownHallComponent BuildingTownHallComponent, CoinDropAnimation coinDropAnimation, Pools coinPool, GameCyrcle cyrcle, AIWarConnector aIWarConnector)
        {
            _cyrcle = cyrcle;
            _AIWarConnector = aIWarConnector;
            _BuildingTownHallComponent = BuildingTownHallComponent;
            _coinPool = coinPool;
            _coinDropAnimation = coinDropAnimation;
            CancellationTokenSource cts = new CancellationTokenSource();
            token = cts.Token;

            _action += CountMoney;
            _playerCollectMoney += DropMoneyPerDay;
            _BuildingTownHallComponent.SetAction(_action, _playerCollectMoney);
        }
        private void UpdateClosestWalls()
        {
            if(curForm > _firstForm)
            {
                _AIWarConnector.LeftWall[0].UpdateFast(curForm);
                _AIWarConnector.RightWall[0].UpdateFast(curForm);
            }
        }
        private void DropBeforePickUpCoin(int count)
        {
            var pos = new Vector3(_BuildingTownHallComponent.transform.position.x, _heightPosition, _BuildingTownHallComponent.transform.position.z);
            _coinDropAnimation.RandomCoinJump(pos, count, _coinPool, _canPickUp);
        }
        private void DropMoneyPerDay()
        {
            if(curForm < 1)
            {
                _dropMoney = false;
                return;
            }

            if (_dropMoney)
            {
                _BuildingTownHallComponent.GetChestAnim().SetBool("Open", true);
                _BuildingTownHallComponent.GetParticlePrefab().SetActive(true);
                _dropMoney = !_dropMoney;
                _BuildingTownHallComponent.GetChestAnim();
                _coinDropAnimation.RandomCoinJump(_BuildingTownHallComponent.GetChestPos(), _BuildingTownHallComponent.GetCountMoneyPerDay() * curForm * 2,  _coinPool, _canPickUp);
            }
        }
        private async void CountMoney()
        {
            _curGold++;
            _BuildingTownHallComponent.SetCurGold(_curGold);
            checkForGold = true;
            int gold = 0;

            while (checkForGold && _curGold < _BuildingTownHallComponent.GetCountOfGold())
            {
                if (gold < _curGold)
                {
                    gold++;
                    await UniTask.Delay(TimeSpan.FromSeconds(_moneyWaitDelay), cancellationToken: token);
                }
                else
                {
                    checkForGold = false;
                }
            }

            if (_curGold == _BuildingTownHallComponent.GetCountOfGold())
            {
                _BuildingTownHallComponent.SetCanProduce(false);
                BuildingFinish();
            }
            else
            {
                DropBeforePickUpCoin(_curGold);
            }
            _curGold = 0;
        }
        private async void BuildingFinish()
        {
            await UniTask.Delay(TimeSpan.FromSeconds(_moneyWaitDelay), cancellationToken: token);

            _BuildingTownHallComponent.GetFormList()[curForm].gameObject.SetActive(false);
            curForm++;
            _BuildingTownHallComponent.GetFormList()[curForm].gameObject.SetActive(true);
            //MoveBuilder();
            if (curForm < _BuildingTownHallComponent.GetFormList().Count - 1)
            {
                _BuildingTownHallComponent.SetCanProduce(true);
                _BuildingTownHallComponent.SetNewCountOfgold(_BuildingTownHallComponent.GetCountOfGold() * 3);
            }
            else
            {
                _BuildingTownHallComponent.SetCanProduce(false);
                townHallFinishBuild.Invoke();
            }
            UpdateClosestWalls();
        }
        public bool IsMaxForm()
        {
            if (curForm == _BuildingTownHallComponent.GetFormList().Count - 1)
            {
                return true;
            }

            return false;
        }
        public void SetAction(Action act)
        {
            townHallFinishBuild += act;
        }
        public void OnDayChange()
        {
            if (_cyrcle.ChekIfDay())
            {
                _BuildingTownHallComponent.GetParticlePrefab().SetActive(false);
                _BuildingTownHallComponent.GetChestAnim().SetBool("Open", false);
                _dropMoney = true;
            }
        }
    }

}
