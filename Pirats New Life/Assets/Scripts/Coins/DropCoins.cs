using UnityEngine;
using GameInit.Pool;
using GameInit.GameCyrcleModule;
using System.Threading.Tasks;
using System;
using GameInit.Connector;

namespace GameInit.DropAndCollectGold
{
    public class DropCoins : IUpdate
    {
        private Pools pool;
        private Transform transform;
        private ResourceManager resourses;
        private HeroComponent _heroComponent;
        private AIConnector _AIConnector;

        private float dropTimer = 0.0f;
        private float dropInterval = 0.5f;

        public DropCoins(Pools _pool, Transform _transform, ResourceManager _resourses, HeroComponent heroComponent, AIConnector AIConnector)
        {
            _AIConnector = AIConnector;
            pool = _pool;
            transform = _transform;
            resourses = _resourses;
            _heroComponent = heroComponent;
        }

        private async void DropCoin()
        {
            await Task.Delay(TimeSpan.FromSeconds(0.2f));
            var coin = pool.GetFreeElements(transform.position);
            _AIConnector.CheckAndGoToCoin();
            resourses.SetResource(ResourceType.Gold, -1);
        }
        private void CollectGold()
        {
            if (_heroComponent.GetCoin() != null && _heroComponent.GetCoin().CanPickUp)
            {
                _heroComponent.GetCoin().Hide();
                _heroComponent.ForgetCoin();
                resourses.SetResource(ResourceType.Gold, 1);
            }
        }

        public void OnUpdate()
        {
            if (Input.GetKeyDown(KeyCode.Space) && resourses.GetResource(ResourceType.Gold) != 0)
            {
                DropCoin();
            }
            if (Input.GetKey(KeyCode.Space) && resourses.GetResource(ResourceType.Gold) != 0)
            {
                dropTimer += Time.deltaTime;
                if (dropTimer >= dropInterval)
                {
                    DropCoin();
                    dropTimer = 0.0f;
                }
            }
            else
            {
                dropTimer = 0.0f;
            }
            CollectGold();
        }
    }
}