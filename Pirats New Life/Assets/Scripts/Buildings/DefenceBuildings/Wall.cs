using System.Collections.Generic;
using System;
using GameInit.Connector;
using System.Collections;
using UnityEngine;
using GameInit.GameCyrcleModule;
using GameInit.AI;
using GameInit.Animation;
using GameInit.Pool;
using GameInit.Builders;

namespace GameInit.Building
{
    public class Wall : IBuilding, IDayChange, IKDTree
    {
        public bool isBuilded { get; set; } = false;
        private bool inBuildProgress = false;

        private Action _startBuilding;
        private BuildingComponent _wallComponent;
        private ResourceManager _res;
        private GameCyrcle _cyrcle;
        private AIConnector _AIConnector;
        private AIWarConnector _AIWarConnector;
        private IWork _curentlyWorker;
        private List<Wall> _walls;
        private float timeToBuild = 10.0f; // time to build in seconds
        private float progress = 0.0f; // current progress towards completing the wall
        private bool _coroutineInPlay = false;
        private int _index = -1;
        private int _hpPerLvl = 100;
        private Action<int> DropBeforePickUp;
        private CoinDropAnimation _coinDropAnimation;
        private Pools _coinPool;
        private Coroutine _curCoroutineWaitMove;
        private bool _isDay = false;
        private bool _needToRepear = false;
        private Tower _towerFirst;
        private Tower _towerSecond;
        private bool prepearForBuild = false;
        private bool _rootWall = false;
        private bool _buildFirstTime = true;
        
        public int CirclePosition { get; set; }
        public bool IsRight { get; set; }
        public EntityType Type { get; } = EntityType.Wall;

        private const bool canPickUp = false;
        private const int _firstForm = 1;
        private const bool _isEnemy = false;
        private const bool _canDamage = false;

        public int HP { get; set; } = 0;
        public Wall(BuildingComponent buildingComponent, ResourceManager res, BuilderConnectors BuilderConnectors, GameCyrcle cyrcle, List<Wall> wall, Pools coinPool, CoinDropAnimation coinDropAnimation)
        {
            _coinDropAnimation = coinDropAnimation;
            _coinPool = coinPool;
            _walls = wall;
            _AIConnector = BuilderConnectors.GetAiConnector();
            _AIWarConnector = BuilderConnectors.GetAIWarConnector();
            _cyrcle = cyrcle;
            _wallComponent = buildingComponent;
            _res = res;
            DropBeforePickUp += DropBeforePickUpCoin;
            _startBuilding += Build;
            _wallComponent.SetAction(_startBuilding, DropBeforePickUp);

            var components = buildingComponent.gameObject.GetComponentsInChildren<TowerBuildingComponent>();

            _towerFirst = new Tower(components[0], coinDropAnimation, coinPool, BuilderConnectors, cyrcle);
            _towerSecond = new Tower(components[1], coinDropAnimation, coinPool, BuilderConnectors, cyrcle);
        }
        private void DropBeforePickUpCoin(int count)
        {
            _coinDropAnimation.RandomCoinJump(_wallComponent.GetBuildPositions()[1].position, count, _wallComponent.GetBuildPositions()[1].position, _coinPool, canPickUp);
        }

        public void SetBuilder(IWork worker)
        {
            if (worker != null)
                worker.InWork = true;
            _curentlyWorker = worker;
        }
        public void Build()
        {
            progress = 0;
            inBuildProgress = true;
            _wallComponent.SetInBuild(inBuildProgress);
            MoveBuilder();
        }

        public void UpdateFast(int form)
        {
            prepearForBuild = false;

            _towerFirst.UpdateFast(form);
            _towerSecond.UpdateFast(form);

            _rootWall = true;
            
            progress = 0;
            _wallComponent.SetInBuild(false);
           
            _wallComponent.SetCountForGold(_wallComponent.GetCurCountOFGold() * 3);
            
           
            if (!isBuilded)
            {
                _wallComponent.SetForm(form - 1);
                return;
            }

            _wallComponent.SetForm(form);

            if (_wallComponent.ChekMaxLvl())
                _wallComponent.SetCanProduce(false);
            // wall is built
            HP = form * _hpPerLvl;
            _wallComponent.UpdateBuild();
            _AIWarConnector.SetSwordManToNewPosition();
            _AIWarConnector.RandomAnimalPosition();
        }
        public bool GetBuildingState()
        {
            return isBuilded;
        }
        private Vector3 RandomBuildPosition()
        {
            var positions = _wallComponent.GetBuildPositions();
            int index = UnityEngine.Random.Range(0, positions.Count);

            if (_index != -1)
            {
                do
                {
                    index = UnityEngine.Random.Range(0, positions.Count);
                }
                while (index == _index);
            }

            _index = index;
            Vector3 position = positions[index].position;
            return position;
        }
        public void OnDayChange()
        {
            _isDay = _cyrcle.ChekIfDay();
            Reapear();
            MoveBuilder();
            _towerFirst.OnDayChange();
            _towerSecond.OnDayChange();
        }

        private void Reapear()
        {
            if (_needToRepear && _isDay && isBuilded)
            {
                _AIConnector.MoveToClosestAIBuilder(RandomBuildPosition(), StartRepear, this);
            }
        }
        private void MoveBuilder()
        {
            prepearForBuild = true;

            _isDay = _cyrcle.ChekIfDay();

            if (_isDay && inBuildProgress && !_coroutineInPlay)
            {
                _AIConnector.MoveToClosestAIBuilder(RandomBuildPosition(), StartBuilding, this);
            }
        }
        private void StartRepear()
        {
            if (_isDay && !inBuildProgress && _curentlyWorker != null && !_coroutineInPlay && isBuilded)
            {
                _curentlyWorker.InWork = true;
                _wallComponent.GetMonoBehaviour().StartCoroutine(Repearing());
            }
        }
        private void StartBuilding()
        {
            if (!prepearForBuild)
            {
                prepearForBuild = false;
                _curentlyWorker.InWork = false;
                inBuildProgress = false;
                _AIConnector.MoveToClosest();
                _curentlyWorker.GetRandomWalker().Move();
                _curentlyWorker = null;
                return;
            }

            if (_isDay && inBuildProgress && _curentlyWorker != null && !_coroutineInPlay)
            {
                _curentlyWorker.InWork = true;
                _wallComponent.GetMonoBehaviour().StartCoroutine(BuildingInProgress());
            }
        }
        private IEnumerator RandomBuildPositionCoroutine()
        {
            while (_isDay && (progress < timeToBuild || HP < _hpPerLvl * _wallComponent.GetCurForm()) && _curentlyWorker != null)
            {
                yield return new WaitForSeconds(10.0f); // chek for build and randomPosition time, it will not work if it to similary
                if (_curentlyWorker != null)
                    _curentlyWorker.Move(RandomBuildPosition(), null);
            }
        }
        private IEnumerator Repearing()
        {
            _coroutineInPlay = true;
            _curCoroutineWaitMove = _wallComponent.GetMonoBehaviour().StartCoroutine(RandomBuildPositionCoroutine());
            while (_isDay && HP < _hpPerLvl * _wallComponent.GetCurForm())
            {
                yield return new WaitForSeconds(1.0f); // wait for 1 second before checking progress again

                if (HP < _hpPerLvl * _wallComponent.GetCurForm())
                {
                    HP += 1;
                }
            }

            if (_curCoroutineWaitMove != null)
                _wallComponent.GetMonoBehaviour().StopCoroutine(_curCoroutineWaitMove);

            if (HP >= _hpPerLvl * _wallComponent.GetCurForm())
            {
                _wallComponent.SetCanProduce(true);
            }

            _curentlyWorker.InWork = false;
            _AIConnector.MoveToClosest();
            _curentlyWorker.GetRandomWalker().Move();
            _coroutineInPlay = false;
        }
        private IEnumerator BuildingInProgress()
        {
            _coroutineInPlay = true;
            _curCoroutineWaitMove = _wallComponent.GetMonoBehaviour().StartCoroutine(RandomBuildPositionCoroutine());
            
            while (_isDay && progress < timeToBuild && prepearForBuild)
            {
                yield return new WaitForSeconds(1.0f); // wait for 1 second before checking progress again
                progress += 1.0f;
            }

            _wallComponent.GetMonoBehaviour().StopCoroutine(_curCoroutineWaitMove);

            if (progress >= timeToBuild && prepearForBuild)
            {
                if (_buildFirstTime)
                {
                    _towerFirst.SetProduce(true);
                    _towerSecond.SetProduce(true);

                    foreach (var wall in _walls)
                    {
                        if (wall.CirclePosition == CirclePosition + 1 && IsRight == wall.IsRight)
                        {
                            wall.GetWallComponent().SetCanProduce(true);
                        }
                    }
                }
                if (_wallComponent.ChekMaxLvl())
                    _wallComponent.SetCanProduce(false);
                // wall is built
                _buildFirstTime = false;
                _curentlyWorker.InWork = false;
                isBuilded = true;
                inBuildProgress = false;
                _wallComponent.UpdateBuild();
                HP = _hpPerLvl * _wallComponent.GetCurForm();
                _wallComponent.SetInBuild(false);
                Debug.Log("Wall built!");
                _AIConnector.MoveToClosest();
                _curentlyWorker.GetRandomWalker().Move();
                _curentlyWorker = null;
                _AIWarConnector.SetSwordManToNewPosition();
                _AIWarConnector.RandomAnimalPosition();
                
                if (_wallComponent.GetCurForm() < _wallComponent.GetFormList().Count - 1)
                {
                    _wallComponent.SetCountForGold(_wallComponent.GetCurCountOFGold() * 3);
                }
            }
            else
            {
                // wall building interrupted due to day/night cycle
                _curentlyWorker.InWork = false;
                inBuildProgress = true;
                _AIConnector.MoveToClosest();
                _curentlyWorker.GetRandomWalker().Move();
                _curentlyWorker = null;
                Debug.Log("Wall building interrupted.");
            }

            if (!prepearForBuild)
            {
                progress = 0;
                isBuilded = true;
                inBuildProgress = false;
            }

            prepearForBuild = false;
            _coroutineInPlay = false;
        }

        public BuildingComponent GetWallComponent()
        {
            return _wallComponent;
        }
        public Vector2 GetPositionVector2()
        {
            Vector2 _positionOnVector2;
            _positionOnVector2.x = _wallComponent.GetBuildPositions()[0].position.x;
            _positionOnVector2.y = _wallComponent.GetBuildPositions()[0].position.z;

            return _positionOnVector2;
        }
        public Vector3 GetPositionVector3()
        {
            return _wallComponent.GetBuildPositions()[0].position;
        }

        private void CheckIfNeedRepear()
        {
            if (isBuilded && HP < _hpPerLvl * _wallComponent.GetCurForm() && !inBuildProgress && HP > 0)
            {
                _needToRepear = true;
                _wallComponent.SetCanProduce(false);
            }
        }
        public void GetDamage(int damage)
        {
            if (isBuilded &&  HP - damage <= 0)
            {
                Die();
                HP = 0;
            }
            else
            {
                HP = HP - damage;
                CheckIfNeedRepear();
            }
        }
        private void Die()
        {
            _buildFirstTime = true;
            isBuilded = false;
            foreach (var wall in _walls)
            {
                if (wall.CirclePosition == CirclePosition + 1 && IsRight == wall.IsRight)
                {
                    wall.GetWallComponent().SetCanProduce(false);
                }
            }

            _wallComponent.enabled = true;
            _AIWarConnector.SetSwordManToNewPosition();
            _AIWarConnector.RandomAnimalPosition();
            _wallComponent.StopAllCoroutines();
            _wallComponent.SetCanProduce(true);
            var _curform = _wallComponent.GetCurForm();
            _wallComponent.ResetForm();
            _wallComponent.SetForm(_curform - 1);
            _towerFirst.Destroy();
            _towerSecond.Destroy();
            
            if (_rootWall)
            {
                _wallComponent.SetCountForGold(_wallComponent.GetCurCountOFGold() / 3);
            }
            else
            {
                _wallComponent.SetCountForGold(_wallComponent.BaseCount);
            }
        }
        public bool CheckIfEnemy()
        {
            return _isEnemy;
        }

        public bool CheckIfCanDamage()
        {
            return _canDamage;
        }

        public int CountOFDamage()
        {
            return 0; // can not damage
        }

        public void Attack()
        {
            //can not damage
        }
    }

}
