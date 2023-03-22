using GameInit.Chest;
using GameInit.GameCyrcleModule;
using GameInit.Pool;
using UnityEngine;
using GameInit.Component;
using GameInit.Animation;

namespace GameInit.Builders
{
    public class ChestBuilder
    {
        private GameCyrcle _cycle;
        private ChestComponent[] _chestSettings;

        public ChestBuilder(GameCyrcle cycle,ResourceManager resourceManager, Pools pool, CoinDropAnimation coinDropAnimation)
        {
            _cycle = cycle;
            _chestSettings = Object.FindObjectsOfType<ChestComponent>();

            foreach (var settings in _chestSettings)
            {
                ChestCollider chestCollider = new ChestCollider(settings, this, resourceManager, pool, coinDropAnimation);

                cycle.Add(chestCollider);
            }
        }

        public void RemoveChestCollider(IUpdate update)
        {
            _cycle.Remove(update);
        }
    }
}