using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameInit.Connector;
using GameInit.Animation;
using GameInit.Pool;
using GameInit.RandomWalk;

namespace GameInit.AI
{
    public class AISpawner
    {
        private float heightPosition = -8.48f;
        public AISpawner(CampComponent[] camps, AIConnector _AIConnector, Pools pool, CoinDropAnimation _coinDropAnimation, HeroComponent heroComponent)
        {
            foreach (var camp in camps)
            {
                for (int i = 0; i < camp.GetCount(); i++)
                {
                    var pos = camp.GetTransformSpawn().position;
                    var diffmax = camp.GetSpawnDiffMax();
                    var diffmin = camp.GetSpawnDiffMin();

                    var x = Random.Range(diffmin, diffmax);
                    var z = Random.Range(diffmin, diffmax);

                    if (Random.Range(0, 2) == 1)
                    {
                        x = -x;
                    }
                    if (Random.Range(0, 2) == 1)
                    {
                        z = -z;
                    }

                    var position = new Vector3(pos.x + x, heightPosition, pos.z + z);

                    var obj = GameObject.Instantiate(camp.GetCitizenPrefab(), position, Quaternion.identity);
                    var _AIComponent = obj.GetComponent<AIComponent>();

                    var randomWalker = new RandomWalker(); 
                    Stray stray = new Stray(_AIComponent, _AIConnector.GenerateId(), pool, _coinDropAnimation, heroComponent, randomWalker);
                    

                    _AIConnector.StrayList.Add(stray);
                }
            }
        }
    }
}

