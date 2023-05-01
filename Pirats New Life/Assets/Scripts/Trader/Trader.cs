using GameInit.Animation;
using GameInit.GameCyrcleModule;
using GameInit.Pool;
using System;

namespace GameInit.TraderLogic
{
    public class Trader : IDayChange
    {
        private TraderComponent _TraderComponent;
        private Action CoinCollect;
        private Action CoinDrop;
        private Action<int> DropBeforePickUp;
        private CoinDropAnimation _coinDropAnimation;
        private GameCyrcle _cyrcle;
        private Pools _coinPool;
        private int _dayToWait;
        private int _curDay = 0;
        private bool _atHome = true;
        private float _money = 0;
        private bool _canDrop = true;

        private const bool canPickUp = false;
        public Trader(GameCyrcle cyrcle, CoinDropAnimation coinDropAnimation, Pools poolCoin)
        {
            _coinPool = poolCoin;
            _coinDropAnimation = coinDropAnimation;
            _cyrcle = cyrcle;
            _TraderComponent = UnityEngine.Object.FindObjectOfType<TraderComponent>();
            _dayToWait = _TraderComponent.DayToWait;
            CoinCollect += MoveBack;
            CoinDrop += DropCoins;
            DropBeforePickUp += DropBeforePickUpCoin;
            _TraderComponent.SetAction(CoinCollect, CoinDrop, DropBeforePickUp);
        }
        private void DropBeforePickUpCoin(int count)
        {
            _coinDropAnimation.RandomCoinJump(_TraderComponent.transform.position, count, _TraderComponent.transform.position, _coinPool, canPickUp);
        }
        private void DropCoins()
        {
            if(_canDrop && _money >= _TraderComponent.GoldCount)
            {
                _coinDropAnimation.RandomCoinJump(_TraderComponent.transform.position, (int)_money, _TraderComponent.transform.position, _coinPool, canPickUp);
                _money = 0;
                _TraderComponent.CanPick(true);
            }
        }
        private void MoveBack()
        {
            _canDrop = false;
            _TraderComponent.Agent.SetDestination(_TraderComponent.StartPoint.position);
            _money = _TraderComponent.GoldCount;
            _money = (int)_money * _TraderComponent.Modificator;
        }
        public void OnDayChange()
        {
            if (!_atHome)
            {
                 _atHome = true;
                _TraderComponent.Agent.SetDestination(_TraderComponent.StartPoint.position);
            }

            if (_cyrcle.ChekIfDay() && _curDay >= _dayToWait)
            {
                _curDay = 0;
                if (_atHome)
                {
                    if(_money == 0)
                    {
                        _TraderComponent.CanPick(true);
                    }
                    else
                    {
                        _canDrop = true;
                    }
                    _atHome = false;
                    _TraderComponent.Agent.SetDestination(_TraderComponent.EndPoint.position);
                }
            }
            else if (_cyrcle.ChekIfDay())
            {
                _curDay++;
            }
        }
    }
}

