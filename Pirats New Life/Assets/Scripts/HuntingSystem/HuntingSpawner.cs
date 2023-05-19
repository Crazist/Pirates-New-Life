using GameInit.Animation;
using GameInit.Builders;
using GameInit.Connector;
using GameInit.Pool;
using UnityEngine;
using GameInit.GameCyrcleModule;
using GameInit.Building;

namespace GameInit.AI
{
    public class HuntingSpawner : IDayChange
    {
        private AnimalSpawner[] _AnimalSpawners;
        private AIWarConnector _AIWarConnector;
        private AIConnector _AIConnector;
        private CoinDropAnimation _CoinDropAnimation;
        private GameCyrcle _cyrcle;
        private Pools pool;
        private Wall _lastRighrWall;
        private Wall _lastLeftWall;

        private const float _wallOffset = 2f;
        private const float heightPosition = 0.46f;
        private const float _maxCount = 3;

        public HuntingSpawner(CoinDropAnimation coinDropAnimation, Pools coinPool, BuilderConnectors _conectorsBuilder, GameCyrcle cyrcle)
        {
            _AnimalSpawners = UnityEngine.Object.FindObjectsOfType<AnimalSpawner>();

            _cyrcle = cyrcle;
            pool = coinPool;
            _CoinDropAnimation = coinDropAnimation;
            _AIWarConnector = _conectorsBuilder.GetAIWarConnector();
            _AIConnector = _conectorsBuilder.GetAiConnector();
        }

        private void SpawnRabits()
        {
            foreach (var spawner in _AnimalSpawners)
            {
                if(spawner.GetCurCount() < _maxCount)
                {
                    var count = spawner.GetCountRabbits() - spawner.GetCurCount();

                    for (int i = 0; i < count; i++)
                    {
                        Vector3 _position = Vector3.zero;
                        Vector3 _positionToMove = Vector3.zero;
                        var pos = spawner.GetTransform().position;
                        var diffmaxX = spawner.GetSpawnDiffMaxX();
                        var diffminX = spawner.GetSpawnDiffMinX();
                        var diffmaxZ = spawner.GetSpawnDiffMaxZ();
                        var diffminZ = spawner.GetSpawnDiffMinZ();

                        var x = UnityEngine.Random.Range(diffminX, diffmaxX);
                        var z = UnityEngine.Random.Range(diffminZ, diffmaxZ);

                        if (UnityEngine.Random.Range(0, 2) == 1)
                        {
                            x = -x;
                        }
                        if (UnityEngine.Random.Range(0, 2) == 1)
                        {
                            z = -z;
                        }

                        if (pos.x > 0 && _lastRighrWall != null)
                        {
                            var _pos = _lastRighrWall.GetPositionVector3();
                            _positionToMove = new Vector3(_pos.x + _wallOffset, _pos.y, _pos.z);
                            _position = new Vector3(_pos.x + spawner.GetOffsetFromTheWall(), _pos.y, _pos.z);
                        }
                        else if (pos.x > 0 && _lastLeftWall != null)
                        {
                            var _pos = _lastLeftWall.GetPositionVector3();
                            _positionToMove = new Vector3(_pos.x - _wallOffset, _pos.y, _pos.z);
                            _position = new Vector3(_pos.x - spawner.GetOffsetFromTheWall(), _pos.y, _pos.z);
                        }
                        else
                        {
                            _positionToMove = pos;
                            _position = pos;
                        }

                        _positionToMove = new Vector3(_positionToMove.x, heightPosition, _positionToMove.z);
                        var position = new Vector3(_position.x + x, heightPosition, _position.z + z);

                        var obj = GameObject.Instantiate(spawner.GetEnemy(), position, Quaternion.identity);

                        Rabbit rabbit = new Rabbit(obj, _CoinDropAnimation, _positionToMove, spawner.GetRadius(), pool, spawner, _AIConnector);

                        spawner.AddCurCount();

                        _AIWarConnector.PointsInWorld.Add(rabbit);
                        _AIWarConnector.AnimalList.Add(rabbit);
                        _AIWarConnector.UpdateTree();
                    }
                }
            }
              
        }

        public void OnDayChange()
        {
            if (!_cyrcle.ChekIfDay())
            {
                SpawnRabits();
            }
        }
    }
}

