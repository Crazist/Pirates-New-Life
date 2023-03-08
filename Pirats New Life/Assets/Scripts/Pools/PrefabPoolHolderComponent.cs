using UnityEngine;

namespace GameInit.PoolPrefabs
{
    public class PrefabPoolHolderComponent : MonoBehaviour
    {
        [SerializeField] private GameObject coin;


        public GameObject GetCoinPrefab()
        {
            return coin;
        }

    }
}

