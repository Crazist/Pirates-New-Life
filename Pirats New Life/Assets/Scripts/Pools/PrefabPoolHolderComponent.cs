using UnityEngine;

namespace GameInit.PoolPrefabs
{
    public class PrefabPoolHolderComponent : MonoBehaviour
    {
        [SerializeField] private Coin coin;


        public Coin GetCoinPrefab()
        {
            return coin;
        }

    }
}

