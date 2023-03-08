using GameInit.GameCyrcleModule;
using UnityEngine;
using GameInit.Pool;
using GameInit.DropAndCollectGold;
using GameInit.Hero;

namespace GameInit.Builders
{
    public class HeroBuilder
    {
        public HeroComponent HeroComponent { get; private set; }

        public HeroBuilder(GameCyrcle GameCyrcle, Pools _pool, ResourceManager resources)
        {
            HeroComponent hero = Object.FindObjectOfType<HeroComponent>();

            HeroComponent = hero.GetComponent<HeroComponent>();

            DropCoins dropCoins = new DropCoins(_pool, hero.transform, resources, HeroComponent);
            HeroMove move = new HeroMove(HeroComponent);

            GameCyrcle.Add(dropCoins);
            GameCyrcle.Add(move);
        }

        public HeroComponent GetHeroSettings()
        {
            return HeroComponent;
        }
    }
}
