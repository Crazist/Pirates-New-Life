using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameInit.Connector;
using GameInit.Pool;
using GameInit.Animation;
using GameInit.GameCyrcleModule;
using GameInit.Builders;

namespace GameInit.AI
{
    public class WorkChecker : IUpdate
    {
        private AIConnector _connector;
        private CoinDropAnimation _coinDropAnimation;
        private Pools _pool;
        private HeroComponent _heroComponent;


        private Dictionary<int, ItemsType> _strayList;
        private Dictionary<int, ItemsType> _builderList;
        private Dictionary<int, ItemsType> _archerList;
        private Dictionary<int, ItemsType> _farmerList;
        private Dictionary<int, ItemsType> _swordManList;
        public WorkChecker(AIConnector connector, CoinDropAnimation coinDropAnimation,  Pools pool, GameCyrcle curcle, HeroBuilder heroBuilder)
        {
            _heroComponent = heroBuilder.GetHeroComponent();
            _connector = connector;
            _pool = pool;
            _coinDropAnimation = coinDropAnimation;

            _strayList = new Dictionary<int, ItemsType>();

            Init(curcle);
        }

        public void Init(GameCyrcle curcle)
        {
            _strayList = new Dictionary<int, ItemsType>();
            CopyToDictinary(_strayList, _connector.StrayList, curcle);

            _builderList = new Dictionary<int, ItemsType>();
            CopyToDictinary(_builderList, _connector.StrayList, curcle);

            _archerList = new Dictionary<int, ItemsType>();
            CopyToDictinary(_archerList, _connector.StrayList, curcle);

            _farmerList = new Dictionary<int, ItemsType>();
            CopyToDictinary(_farmerList, _connector.StrayList, curcle);

            _swordManList = new Dictionary<int, ItemsType>();
            CopyToDictinary(_swordManList, _connector.StrayList, curcle);
        }

        private Dictionary<int, ItemsType> CopyToDictinary(Dictionary<int, ItemsType> dictinary, List<IWork> list, GameCyrcle curcle)
        {
            foreach (var item in list)
            {
                curcle.Add((IUpdate)item);
                dictinary.Add(item.GetId(), item.GetItemType());
            }

            return dictinary;
        }

        public void CreateNewWork(IWork work)
        {
            switch (work.GetItemType())
            {
                case ItemsType.None:
                    CreateStray(work);
                    break;
                case ItemsType.Hammer:
                    CreateStray(work);
                    break;
            }
        }

        private IWork CreateStray(IWork work)
        {
            var stray = new Stray(work.GetComponent(), work.GetId(), _pool,  _coinDropAnimation, _heroComponent);
            _connector.StrayList.Add(stray);
            _connector.StrayList.Remove(work);
            return stray;
        }

        public void OnUpdate()
        {
            foreach (var stray in _connector.StrayList)
            {
                foreach (var _stray in _strayList)
                {
                    if(stray.GetId() == _stray.Key && stray.GetItemType() == ItemsType.None && stray.HasCoin())
                    {
                        //stray.RemoveAllEveants();
                        return;
                    }
                    if (stray.GetId() == _stray.Key && !stray.HasCoin() && stray.GetItemType() != _stray.Value)
                    {
                        stray.RemoveAllEveants();
                        CreateNewWork(stray);
                        return;
                    }
                }

            }
        }
    }
}

