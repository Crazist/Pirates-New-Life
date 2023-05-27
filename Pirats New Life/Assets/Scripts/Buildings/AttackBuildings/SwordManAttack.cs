using System;
using System.Collections.Generic;
using GameInit.GameCyrcleModule;
using GameInit.Builders;
using GameInit.Connector;
using GameInit.Building;
using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;
using GameInit.Animation;
using GameInit.Pool;

namespace GameInit.Attack
{
    public class SwordManAttack : IDayChange
    {
        private AttackComponent _sworManComponentRight;
        private AttackComponent _sworManComponentLeft;

        private AIWarConnector _AIWarConnector;
        private GameCyrcle _cyrcle;

        private Pools _coinPool;
        private CoinDropAnimation _CoinDropAnimation;

        private CancellationToken token;
        private int _curGold = 0;
        private bool checkForGold = false;

        private int _curCost = 1;

        private Action _rightaction;
        private Action _leftaction;

        private TownHall _townHall;

        private bool isBuild = false;

        private const bool _canPickUp = false;
        private const int _moneyWaitDelay = 1;
        private const int minDaysForAttack = 1;
        public SwordManAttack(List<AttackComponent> AttackComponent, GameCyrcle cyrcle, BuilderConnectors builderConnectors, TownHall townHall, CoinDropAnimation coinDropAnimation, Pools coinPool)
        {
            _CoinDropAnimation = coinDropAnimation;
            _coinPool = coinPool;

            _cyrcle = cyrcle;
            _AIWarConnector = builderConnectors.GetAIWarConnector();

            _townHall = townHall;

            CancellationTokenSource cts = new CancellationTokenSource();
            token = cts.Token;

            _rightaction += RightAction;
            _leftaction += LeftAction;

            SetAction(AttackComponent);
        }
        private void DropBeforePickUpCoin(int count, bool isRight)
        {
            Vector3 pos = Vector3.zero;

            if (isRight)
            {
                pos = _sworManComponentRight.transform.position;
            }
            else
            {
                pos = _sworManComponentLeft.transform.position;
            }

            _CoinDropAnimation.RandomCoinJump(pos, count, _coinPool, _canPickUp);
        }

        private void SetAction(List<AttackComponent> AttackComponent)
        {
            foreach (var item in AttackComponent)
            {
                if (item.IsRight)
                {
                    _sworManComponentRight = item;
                    _sworManComponentRight.SetAction(_rightaction);
                }
                else
                {
                    _sworManComponentLeft = item;
                    _sworManComponentLeft.SetAction(_leftaction);
                }
            }
        }
        private async void RightAction()
        {
            _curGold++;
            _curCost = _AIWarConnector.GetSideCalculation().ArcherRightSide.Count;
            checkForGold = true;
            int gold = 0;
            if (_cyrcle.DayCount > minDaysForAttack && isBuild && _townHall.IsMaxForm())
            {
                while (checkForGold && _curGold < _curCost)
                {
                    if (gold < _curGold)
                    {
                        gold++;
                        _sworManComponentRight.SetCurGoldText(gold);
                        await UniTask.Delay(TimeSpan.FromSeconds(_moneyWaitDelay), cancellationToken: token);
                    }
                    else
                    {
                        checkForGold = false;
                    }
                }

                if (_curGold == _curCost)
                {
                    _sworManComponentRight.SetCurGoldText(_curCost);
                    _AIWarConnector.UpdateAttack(_AIWarConnector.GetSideCalculation().SworManRightSide, true, AttackTypes.SwordManType);
                }
                else
                {
                    DropBeforePickUpCoin(_curGold, true);
                }
            }
            else
            {
                DropBeforePickUpCoin(_curGold, true);
            }
            _curGold = 0;
            _sworManComponentRight.SetCurGoldText(_curGold);
        }
        private async void LeftAction()
        {
            _curGold++;
            _curCost = _AIWarConnector.GetSideCalculation().ArcherLeftSide.Count;
            checkForGold = true;
            int gold = 0;
            if (_cyrcle.DayCount > minDaysForAttack && isBuild && _townHall.IsMaxForm())
            {
                while (checkForGold && _curGold < _curCost)
                {
                    if (gold < _curGold)
                    {
                        gold++;
                        _sworManComponentRight.SetCurGoldText(gold);
                        await UniTask.Delay(TimeSpan.FromSeconds(_moneyWaitDelay), cancellationToken: token);
                    }
                    else
                    {
                        checkForGold = false;
                    }
                }

                if (_curGold == _curCost)
                {
                    _sworManComponentRight.SetCurGoldText(_curCost);
                    _AIWarConnector.UpdateAttack(_AIWarConnector.GetSideCalculation().SworManLeftSide, false, AttackTypes.SwordManType);
                }
                else
                {
                    DropBeforePickUpCoin(_curGold, false);
                }
            }
            else
            {
                DropBeforePickUpCoin(_curGold, false);
            }
            _curGold = 0;
            _sworManComponentRight.SetCurGoldText(_curGold);
        }
        public void SetRightText(int count)
        {
            _sworManComponentRight.SetGoldText(count);
        }
        public void SetLeftText(int count)
        {
            _sworManComponentLeft.SetGoldText(count);
        }
        public void OnDayChange()
        {
            if (_cyrcle.ChekIfDay() && _cyrcle.DayCount >= minDaysForAttack && !isBuild && _townHall.IsMaxForm())
            {
                _sworManComponentLeft.gameObject.SetActive(true);
                _sworManComponentLeft.SetCanProduce(true);

                _sworManComponentRight.gameObject.SetActive(true);
                _sworManComponentRight.SetCanProduce(true);

                isBuild = true;
            }
        }
    }
}
