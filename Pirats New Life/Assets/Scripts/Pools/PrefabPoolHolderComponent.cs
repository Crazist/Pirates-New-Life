using GameInit.AI;
using GameInit.Enemy;
using UnityEngine;

namespace GameInit.PoolPrefabs
{
    public class PrefabPoolHolderComponent : MonoBehaviour
    {
        [SerializeField] private Coin coin;
        [SerializeField] private AIComponent enemy;
        [SerializeField] private ArrowComponent _arrow;

        public Coin GetCoinPrefab()
        {
            return coin;
        }
        public AIComponent GetEnemyPrefab()
        {
            return enemy;
        }
        public ArrowComponent GetArrowPrefab()
        {
            return _arrow;
        }

    }
}

