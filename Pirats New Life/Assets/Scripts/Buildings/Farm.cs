using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using GameInit.Connector;
using GameInit.Building;
using GameInit.GameCyrcleModule;
using GameInit.AI;

public class Farm : IBuilding, IDayChange
{
    private bool isBuilded = false;
    private bool inBuildProgress = false;

    private Action _startBuilding;
    private BuildingComponent _wallComponent;
    private ResourceManager _res;
    private GameCyrcle _cyrcle;
    private AIConnector _AIConnector;
    private IWork _curentlyWorker;
    private float timeToBuild = 100.0f; // time to build in seconds
    private float progress = 0.0f; // current progress towards completing the wall
    private bool _coroutineInPlay = false;

    private bool _isDay = false;
    public Farm(BuildingComponent buildingComponent, ResourceManager res, AIConnector AIConnector, GameCyrcle cyrcle)
    {
        _AIConnector = AIConnector;
        _cyrcle = cyrcle;
        _wallComponent = buildingComponent;
        _res = res;
        _startBuilding += Build;
        _wallComponent.SetAction(_startBuilding);
    }

    public void SetBuilder(IWork worker)
    {
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
            if (_curentlyWorker != null)
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
        while (_isDay && progress < timeToBuild)
        {
            yield return new WaitForSeconds(5.0f); // wait for 1 second before checking progress again
            RandomBuildPosition();
        }
    }
    private IEnumerator BuildingInProgress()
    {
        _coroutineInPlay = true;
        _wallComponent.GetMonoBehaviour().StopCoroutine(RandomBuildPositionCoroutine());
        while (_isDay && progress < timeToBuild)
        {
            yield return new WaitForSeconds(1.0f); // wait for 1 second before checking progress again
            progress += 1.0f;
        }

        _wallComponent.GetMonoBehaviour().StopCoroutine(RandomBuildPositionCoroutine());

        if (progress >= timeToBuild)
        {
            // wall is built
            _curentlyWorker.InWork = false;
            isBuilded = true;
            inBuildProgress = false;
            _wallComponent.UpdateBuild();
            _wallComponent.SetInBuild(false);
            Debug.Log("Wall built!");
        }
        else
        {
            // wall building interrupted due to day/night cycle
            inBuildProgress = true;
            Debug.Log("Wall building interrupted.");
        }
        _coroutineInPlay = false;
    }
}
