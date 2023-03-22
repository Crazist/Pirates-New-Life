using GameInit.GameCyrcleModule;
using UnityEngine;
using GameInit.Pool;
using GameInit.DropAndCollectGold;
using GameInit.Hero;
using GameInit.Connector;

namespace GameInit.Builders
{
    public class HeroBuilder
    {
        public HeroComponent HeroComponent { get; private set; }

        public HeroBuilder(GameCyrcle GameCyrcle, Pools _pool, ResourceManager resources, AIConnector AIConnector)
        {
            HeroComponent hero = Object.FindObjectOfType<HeroComponent>();

            HeroComponent = hero.GetComponent<HeroComponent>();

            DropCoins dropCoins = new DropCoins(_pool, hero.transform, resources, HeroComponent, AIConnector);
            HeroMove move = new HeroMove(HeroComponent);

            GameCyrcle.Add(dropCoins);
            GameCyrcle.Add(move);
        }

        public HeroComponent GetHeroComponent()
        {
            return HeroComponent;
        }
        public HeroComponent GetHeroSettings()
        {
            return HeroComponent;
        }
    }
}
