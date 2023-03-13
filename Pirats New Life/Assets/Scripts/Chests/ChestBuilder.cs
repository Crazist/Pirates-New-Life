using GameInit.Chest;
using GameInit.GameCyrcleModule;
using GameInit.Pool;
using UnityEngine;
using GameInit.Component; 

namespace GameInit.Builders
{
    public class ChestBuilder
    {
        private GameCyrcle _cycle;
        private ChestComponent[] _chestSettings;

        public ChestBuilder(GameCyrcle cycle,ResourceManager resourceManager, Pools pool)
        {
            _cycle = cycle;
            _chestSettings = Object.FindObjectsOfType<ChestComponent>();

            foreach (var settings in _chestSettings)
            {
                ChestCollider chestCollider = new ChestCollider(settings, this, resourceManager, pool);

                cycle.Add(chestCollider);
            }
        }

        public void RemoveChestCollider(IUpdate update)
        {
            _cycle.Remove(update);
        }
    }
}