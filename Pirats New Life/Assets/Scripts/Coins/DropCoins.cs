using UnityEngine;
using GameInit.Pool;
using GameInit.GameCyrcleModule;
using System.Threading.Tasks;
using System;


namespace GameInit.DropAndCollectGold
{
    public class DropCoins : IUpdate
    {
        private Pools pool;
        private Transform transform;
        private ResourceManager resourses;
        private HeroComponent _heroComponent;

        public DropCoins(Pools _pool, Transform _transform, ResourceManager _resourses, HeroComponent heroComponent)
        {
            pool = _pool;
            transform = _transform;
            resourses = _resourses;
            _heroComponent = heroComponent;
        }

        private async void DropCoin()
        {
            await Task.Delay(TimeSpan.FromSeconds(0.2f));
            pool.GetFreeElements(transform.position);
            resourses.SetResource(ResourceType.Gold, -1);
        }
        private void CollectGold()
        {
            if (_heroComponent.GetCoin() != null && _heroComponent.GetCoin().CanPickUp)
            {
                _heroComponent.GetCoin().Hide();
                resourses.SetResource(ResourceType.Gold, 1);
            }
        }

        public void OnUpdate()
        {
            if (Input.GetKeyDown(KeyCode.Space) && resourses.GetResource(ResourceType.Gold) != 0)
            {
                DropCoin();
            }
            CollectGold();
        }
    }
}