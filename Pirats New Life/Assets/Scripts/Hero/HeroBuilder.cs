using GameInit.GameCyrcleModule;
using UnityEngine;
using GameInit.Pool;
using GameInit.DropAndCollectGold;
using GameInit.Hero;
using GameInit.Connector;
using GameInit.Animation;

namespace GameInit.Builders
{
    public class HeroBuilder
    {
        public HeroComponent HeroComponent { get; private set; }
        private HeroMove _move;
        public HeroBuilder(GameCyrcle GameCyrcle, Pools _pool, ResourceManager resources, BuilderConnectors builderConnectors, BuilderConnectors _BuilderConnectors, UIBuilder _UIBuilder, CoinDropAnimation _CoinDropAnimation, ResourceManager _resourceManager)
        {
            HeroComponent hero = Object.FindObjectOfType<HeroComponent>();

            HeroComponent = hero.GetComponent<HeroComponent>();

            DropCoins dropCoins = new DropCoins(_pool, hero.transform, resources, HeroComponent, builderConnectors.GetAiConnector());
            _move = new HeroMove(HeroComponent, _UIBuilder, _CoinDropAnimation, _resourceManager, _pool);
            _BuilderConnectors.GetAIWarConnector().PointsInWorld.Add(_move);

            GameCyrcle.Add(dropCoins);
            GameCyrcle.Add(_move);
        }

        public HeroComponent GetHeroComponent()
        {
            return HeroComponent;
        }
        public HeroMove GetHeroMove()
        {
            return _move;
        }
        public HeroComponent GetHeroSettings()
        {
            return HeroComponent;
        }
    }
}
