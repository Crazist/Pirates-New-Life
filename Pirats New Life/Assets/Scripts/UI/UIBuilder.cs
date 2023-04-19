using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameInit.GameCyrcleModule;
using GameInit.PoolPrefabs;
using GameInit.Component;
using GameInit.LoseAnim;

namespace GameInit.Builders
{
    public class UIBuilder
    {
        private LoseSceneAnimation _LoseSceneAnimation;
      public UIBuilder(ResourceManager resources)
      {
         var _goldOnScreenComponent = Object.FindObjectOfType<GoldOnScreenComponent>();

         GoldOnScreen _goldOnScreen = new GoldOnScreen(_goldOnScreenComponent, resources);
         _LoseSceneAnimation = new LoseSceneAnimation();
      }
        public LoseSceneAnimation GetLoseSceneAnimation()
        {
            return _LoseSceneAnimation;
        }
    }
}

