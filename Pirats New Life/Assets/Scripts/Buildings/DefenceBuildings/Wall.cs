using System.Collections.Generic;
using System;
using GameInit.Connector;
using GameInit.Building;
using System.Collections;
using UnityEngine;
using GameInit.GameCyrcleModule;
using GameInit.AI;
using GameInit.Animation;
using GameInit.Pool;
using GameInit.Builders;

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
    public int CirclePosition { get; set; }
    public bool IsRight { get; set; }

    private const bool canPickUp = false;
    private const int _firstForm = 1;
    private const bool _isEnemy = false;
    private bool _isDay = false;
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
    }
    private void DropBeforePickUpCoin(int count)
    {
        _coinDropAnimation.RandomCoinJump(_wallComponent.GetBuildPositions()[1].position, count, _wallComponent.GetBuildPositions()[1].position, _coinPool, canPickUp);
    }

    public void SetBuilder(IWork worker)
    {
        worker.InWork = true;
        _curentlyWorker = worker;
    }
    public void Build()
    {
        if (!_wallComponent.ChekMaxLvl())
        {
            inBuildProgress = true;
            _wallComponent.SetInBuild(inBuildProgress);
            MoveBuilder();
            _wallComponent.SetCountForGold(_wallComponent.GetCurCountOFGold() * 3);
        }
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
        MoveBuilder();
    }

    private void MoveBuilder()
    {
        _isDay = _cyrcle.ChekIfDay();

        if (_isDay && inBuildProgress)
        {
            _curentlyWorker = _AIConnector.MoveToClosestAIBuilder(RandomBuildPosition(), StartBuilding, this);
            if(_curentlyWorker != null)
            _curentlyWorker.InWork = true;
        }
    }

    private void StartBuilding()
    {
        if (_isDay && inBuildProgress && _curentlyWorker != null)
        {
            _curentlyWorker.InWork = true;
            _wallComponent.GetMonoBehaviour().StartCoroutine(BuildingInProgress());
        }
    }
    private IEnumerator RandomBuildPositionCoroutine()
    {
        while (_isDay && progress < timeToBuild && _curentlyWorker != null)
        {
            yield return new WaitForSeconds(10.0f); // chek for build and randomPosition time, it will not work if it to similary
            if(_curentlyWorker != null)
            _curentlyWorker.Move(RandomBuildPosition(), null);
        }
    }
    private IEnumerator BuildingInProgress()
    {
        _coroutineInPlay = true;
        _curCoroutineWaitMove = _wallComponent.GetMonoBehaviour().StartCoroutine(RandomBuildPositionCoroutine());
        while (_isDay && progress < timeToBuild)
        {
            yield return new WaitForSeconds(1.0f); // wait for 1 second before checking progress again
            progress += 1.0f;
        }

        _wallComponent.GetMonoBehaviour().StopCoroutine(_curCoroutineWaitMove);

        if (progress >= timeToBuild)
        {
            if(_wallComponent.GetCurForm() == _firstForm)
            {
                foreach (var wall in _walls)
                {
                    if(wall.CirclePosition == CirclePosition + 1 && IsRight == wall.IsRight)
                    {
                        wall.GetWallComponent().SetCanProduce(true);
                    }
                }
            }
            // wall is built
            HP = HP + _hpPerLvl;
            _curentlyWorker.InWork = false;
            isBuilded = true;
            inBuildProgress = false;
            _wallComponent.UpdateBuild();
            _wallComponent.SetInBuild(false);
            Debug.Log("Wall built!");
            _AIConnector.MoveToClosest();
            _curentlyWorker.GetRandomWalker().Move();
            _curentlyWorker = null;
            _AIWarConnector.SetSwordManToNewPosition();
        }
        else
        {
            // wall building interrupted due to day/night cycle
            inBuildProgress = true;
            Debug.Log("Wall building interrupted.");
        }
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

    public void GetDamage(int damage)
    {
        if (HP - damage <= 0)
        {
            Die();
            HP = 0;
        }
        else
        {
            HP = HP - damage;
        }
    }
    private void Die()
    {
        _wallComponent.ResetForm();
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
