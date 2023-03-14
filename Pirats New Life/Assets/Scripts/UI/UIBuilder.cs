using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameInit.GameCyrcleModule;
using GameInit.PoolPrefabs;
using GameInit.Component;

namespace GameInit.Builders
{
    public class UIBuilder
    {
      public UIBuilder(ResourceManager resources)
      {
         var _goldOnScreenComponent = Object.FindObjectOfType<GoldOnScreenComponent>();

         GoldOnScreen _goldOnScreen = new GoldOnScreen(_goldOnScreenComponent, resources);

      }
    }
}

