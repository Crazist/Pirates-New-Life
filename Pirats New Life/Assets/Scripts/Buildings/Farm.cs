using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using GameInit.Connector;
using GameInit.Building;
using GameInit.GameCyrcleModule;
using GameInit.AI;
using GameInit.Optimization;
using GameInit.Pool;
using GameInit.Animation;

public class Farm : IBuilding, IDayChange, IUpdate
{
    private bool isBuilded = false;
    private bool inBuildProgress = false;

    private Action _startBuilding;
    private BuildingComponent _farmComponent;
    private ResourceManager _res;
    private GameCyrcle _cyrcle;
    private AIConnector _AIConnector;
    private IWork _curentlyWorker;
    private List<IWork> _curentlyWorkers;
    private float timeToBuild = 50.0f; // time to build in seconds
    private float progress = 0.0f; // current progress towards completing the wall
    private bool _coroutineInPlay = false;
    private int _curGold = 0;
    private HeroComponent _heroComponent;
    private bool _waitCoins = false;
    private Pools _pool;
    private CoinDropAnimation _coinDropAnimation;
    private bool _isWorking = false;
    private int _maxCountOfWorkers = 3;
    private int _curCountOfWorkers = 0;
    private int _index = -1;
    private int _curForm = 2;
    private Action<int> DropBeforePickUp;
    
    private const bool canPickUp = false;
    private const int _minimalDistanceToHero = 2;
    private const bool _canPickUp = true;
    private const int _minimalCountOfGoldToDrop= 20;
    private const int _ChangeFormFirstTime = 10;
    private const int _ChangeFormSecndTime = 20;
    private const int _standartForm = 2;

    private bool _isDay = false;
    public Farm(BuildingComponent buildingComponent, ResourceManager res, AIConnector AIConnector, GameCyrcle cyrcle, HeroComponent heroComponent, Pools pool, CoinDropAnimation coinDropAnimation)
    {
        _pool = pool;
        _coinDropAnimation = coinDropAnimation;
        _heroComponent = heroComponent;
        _AIConnector = AIConnector;
        _cyrcle = cyrcle;
        _farmComponent = buildingComponent;
        _res = res;
        _curentlyWorkers = new List<IWork>();

        DropBeforePickUp += DropBeforePickUpCoin;
        _startBuilding += Build;

        _farmComponent.SetAction(_startBuilding, DropBeforePickUp);
    }
    private void DropBeforePickUpCoin(int count)
    {
        _coinDropAnimation.RandomCoinJump(_farmComponent.GetBuildPositions()[0].position, count, _farmComponent.GetBuildPositions()[0].position, _pool, _canPickUp);
    }
    public void SetBuilder(IWork worker)
    {
        if (worker != null)
            worker.InWork = true;
        _curentlyWorker = worker;
       
        if (_curentlyWorker != null && isBuilded)
        {
            _curentlyWorker.InWork = true;
            _curentlyWorkers.Add(_curentlyWorker);
        }
    }
    public void Build()
    {
        if (!_farmComponent.ChekMaxLvl())
        {
            inBuildProgress = true;
            _farmComponent.SetInBuild(inBuildProgress);
            MoveBuilder();
            _farmComponent.SetCountForGold(_farmComponent.GetCurCountOFGold() * 3);
        }
    }
    private void DropGold()
    {
        if (_curGold >= _minimalCountOfGoldToDrop)
        {
            var formList = _farmComponent.GetFormList();

            formList[_curForm - 1].SetActive(false);
            _curForm = _standartForm;
            formList[_curForm - 1].SetActive(true);
            _coinDropAnimation.RandomCoinJump(_heroComponent.transform.position, _curGold - 1, _heroComponent.transform.position, _pool, canPickUp);
            _curGold = 0;
        }
    }
    public bool GetBuildingState()
    {
        return isBuilded;
    }
    private Vector3 RandomBuildPosition()
    {
        var positions = _farmComponent.GetBuildPositions();
        int index = 0;

        do
        {
            index = UnityEngine.Random.Range(0, positions.Count);
        } 
        while (_index == index);
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
            _AIConnector.MoveToClosestAIBuilder(RandomBuildPosition(), StartBuilding, this);
        }
        CheckIfNeedGoWork();
    }

    private void CheckIfNeedGoWork()
    {
        if(_isDay && isBuilded)
        {
            for (int i = 0; i < _maxCountOfWorkers; i++)
            {
                _AIConnector.MoveToClosestAIFarmer(_farmComponent.GetBuildPositions()[i].position, StartWorking, this);
                if (_curentlyWorker != null)
                {
                    _curentlyWorker.InWork = true;
                    _curentlyWorkers.Add(_curentlyWorker);
                }
            }
        }
    }
    private void StartBuilding()
    {
        if (_isDay && inBuildProgress && _curentlyWorker != null && !_coroutineInPlay)
        {
            _farmComponent.GetMonoBehaviour().StartCoroutine(BuildingInProgress());
        }
    }
    private void StartWorking()
    {
        if (_isDay && isBuilded && !_isWorking)
        {
            for (int i = 0; i < _curentlyWorkers.Count; i++)
            {
                if (_curentlyWorkers[i].InWork)
                {
                    _curCountOfWorkers++;
                }
                else
                {
                    _curentlyWorkers.Remove(_curentlyWorkers[i]);
                }
            }
            
            _farmComponent.GetMonoBehaviour().StartCoroutine(WorkingInProgress());
        }
    }
    private IEnumerator RandomBuildPositionCoroutine()
    {
        while (_isDay && (progress < timeToBuild))
        {
             yield return new WaitForSeconds(5.0f); // wait for 1 second before checking progress again
            _curentlyWorker?.GetAiComponent().GeNavMeshAgent().SetDestination(RandomBuildPosition());
        }
    }
    private IEnumerator BuildingInProgress()
    {
        _coroutineInPlay = true;
        var _curCoroutineWaitMove = _farmComponent.GetMonoBehaviour().StartCoroutine(RandomBuildPositionCoroutine());
        while (_isDay && progress < timeToBuild)
        {
            yield return new WaitForSeconds(1.0f); // wait for 1 second before checking progress again
            progress += 1.0f;
        }

        _farmComponent.GetMonoBehaviour().StopCoroutine(_curCoroutineWaitMove);

        if (progress >= timeToBuild)
        {
            // wall is built
            _curentlyWorker.InWork = false;
            isBuilded = true;
            inBuildProgress = false;
            _farmComponent.UpdateBuild();
            _farmComponent.SetInBuild(false);
            _farmComponent.IncreaseForm();
            _AIConnector.MoveToClosest();
            _curentlyWorker.GetRandomWalker().Move();
            _curentlyWorker = null;
            CheckIfNeedGoWork();
            Debug.Log("Wall built!");
        }
        else
        {
            // wall building interrupted due to day/night cycle
            inBuildProgress = true;
            Debug.Log("Wall building interrupted.");
        }
        _curentlyWorkers.Clear();
        _coroutineInPlay = false;
    }
    private IEnumerator WorkingInProgress()
    {
        _isWorking = true;
        
        while (_isDay)
        {
            yield return new WaitForSeconds(20.0f / _curCountOfWorkers); // wait for 1 second before checking progress again
           
            if(_curentlyWorkers.Count > _curCountOfWorkers)
            {
                _curCountOfWorkers = _curentlyWorkers.Count;
            }

            if(_curGold < 20)
            _curGold++;
            
            if (_curGold == _ChangeFormFirstTime || _curGold == _ChangeFormSecndTime)
            {
                _curForm++;
                UpdateWorking();
            }
        }

        _AIConnector.MoveToClosest();
        foreach (var item in _curentlyWorkers)
        {
            item.GetRandomWalker().Move();
        }
        _curentlyWorkers.Clear();
        _curCountOfWorkers = 0;
        _isWorking = false;
    }
    private void CheckIfPlayerWaitForCoins()
    {
        if (_waitCoins == false)
        {
            _waitCoins = true;
            _farmComponent.GetMonoBehaviour().StartCoroutine(Waiter());
        }
    }
    private void UpdateWorking()
    {
        var formList = _farmComponent.GetFormList();

        formList[_curForm - 1].SetActive(true);
        formList[_curForm - 2].SetActive(false);
    }
    private IEnumerator Waiter()
    {
        yield return new WaitForSecondsRealtime(3);
        var i = Vector3.Distance(_heroComponent.Transform.position, _farmComponent.transform.position);
        if (Vector3.Distance(_heroComponent.Transform.position, _farmComponent.transform.position) < _minimalDistanceToHero)
        {
            DropGold();
        }

        _waitCoins = false;
    }
    public void OnUpdate()
    {
       if (_heroComponent == null || _waitCoins) return;

       if (Distance.Manhattan(_heroComponent.Transform.position, _farmComponent.transform.position) < _minimalDistanceToHero)
       {
         CheckIfPlayerWaitForCoins();
       }
    }
}
