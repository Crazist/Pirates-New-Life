using GameInit.AI;
using GameInit.Animation;
using GameInit.Builders;
using GameInit.Connector;
using GameInit.Pool;
using GameInit.RandomWalk;
using UnityEngine;

public class AllySpawner
{
    private CampComponent _CampComponent;
    private AIConnector _AIConnector;
    private Pools _pool;
    private CoinDropAnimation _coinDropAnimation;
    private HeroComponent _heroComponent;

    private int _countOfDays = 1;
    private int _spawnCountOfStrayPerDays = 2;

    private const float heightPosition = 0.46f;
    private const int _minDayToSpawnStray = 3;

    public AllySpawner(CampComponent campComponent, BuilderConnectors builderConnectors, Pools pool, CoinDropAnimation coinDropAnimation, HeroComponent heroComponent)
    {
        _CampComponent = campComponent;
        _AIConnector = builderConnectors.GetAiConnector();
        _pool = pool;
        _coinDropAnimation = coinDropAnimation;
        _heroComponent = heroComponent;


        SpawnStray();
    }
    private void SpawnStrayPerDays()
    {
        if (_countOfDays < _minDayToSpawnStray)
        {
            return;
        }
        else
        {
            for (int i = 0; i < _spawnCountOfStrayPerDays; i++)
            {
                var pos = _CampComponent.GetTransformSpawn().position;
                var diffmax = _CampComponent.GetSpawnDiffMax();
                var diffmin = _CampComponent.GetSpawnDiffMin();

                var x = UnityEngine.Random.Range(diffmin, diffmax);
                var z = UnityEngine.Random.Range(diffmin, diffmax);

                if (UnityEngine.Random.Range(0, 2) == 1)
                {
                    x = -x;
                }
                if (UnityEngine.Random.Range(0, 2) == 1)
                {
                    z = -z;
                }

                var position = new Vector3(pos.x + x, heightPosition, pos.z + z);

                var _AIComponent = GameObject.Instantiate(_CampComponent.GetCitizenPrefab(), position, Quaternion.identity);

                var randomWalker = new RandomWalker();
                Stray stray = new Stray(_AIComponent, _AIConnector.GenerateId(), _pool, _coinDropAnimation, _heroComponent, randomWalker);


                _AIConnector.StrayList.Add(stray);
            }
        }
    }
    private void SpawnStray()
    {
        for (int i = 0; i < _CampComponent.GetCount(); i++)
        {
            var pos = _CampComponent.GetTransformSpawn().position;
            var diffmax = _CampComponent.GetSpawnDiffMax();
            var diffmin = _CampComponent.GetSpawnDiffMin();

            var x = UnityEngine.Random.Range(diffmin, diffmax);
            var z = UnityEngine.Random.Range(diffmin, diffmax);

            if (UnityEngine.Random.Range(0, 2) == 1)
            {
                x = -x;
            }
            if (UnityEngine.Random.Range(0, 2) == 1)
            {
                z = -z;
            }

            var position = new Vector3(pos.x + x, heightPosition, pos.z + z);

            var _AIComponent = GameObject.Instantiate(_CampComponent.GetCitizenPrefab(), position, Quaternion.identity);

            var randomWalker = new RandomWalker();
            Stray stray = new Stray(_AIComponent, _AIConnector.GenerateId(), _pool, _coinDropAnimation, _heroComponent, randomWalker);


            _AIConnector.StrayList.Add(stray);
        }
    }
    public void DayChange()
    {
        _countOfDays++;
        SpawnStrayPerDays();
    }
}
