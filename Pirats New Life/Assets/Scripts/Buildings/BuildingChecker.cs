using Cysharp.Threading.Tasks;
using GameInit.AI;
using GameInit.Building;
using GameInit.Connector;
using GameInit.GameCyrcleModule;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class BuildingChecker : IBuilding
{
    private IWork _curentlyWorker;
    private Vector3 _position;
    private AIConnector _AIConnector;
    private GameCyrcle _cyrcle;
    private  bool _inBuild = false;
    private Action _buildFinished;
    private float _progress = 0;
    private float _timeToBuild;
    private int _index;
    private List<Transform> _positions;
    private bool prepearForBuild = false;
    public BuildingChecker(Vector3 position, AIConnector connector, GameCyrcle cyrcle, Action buildFinished, float timeToBuild, List<Transform> positions = null) 
    {
        _positions = positions;
        _timeToBuild = timeToBuild;
        _position = position;
        _AIConnector = connector;
        _cyrcle = cyrcle;
        _buildFinished = buildFinished;
    }

    public void Build()
    {
        _inBuild = true;
        MoveBuilder();
    }
    public void CheckIfNeedBuild()
    {
        if (_cyrcle.ChekIfDay() && _inBuild)
        {
            MoveBuilder();
        }
    }

    public void SetBuilder(IWork worker)
    {
        if (worker != null)
            worker.InWork = true;
        _curentlyWorker = worker;
    }
    private void MoveBuilder()
    {
        if (_cyrcle.ChekIfDay() && _inBuild)
        {
            prepearForBuild = true;
            _AIConnector.MoveToClosestAIBuilder(_position, StartBuilding, this);
        }
    }
    private void StartBuilding()
    {
        if (!prepearForBuild)
        {
            _curentlyWorker.InWork = false;
            _inBuild = false;
            _AIConnector.MoveToClosest();
            _curentlyWorker.GetRandomWalker().Move();
            _curentlyWorker = null;
            _progress = 0;
            return;
        }

        if (_curentlyWorker != null)
        {
            _curentlyWorker.InWork = true;
            BuildingInProgress();
        }
    }
    private Vector3 RandomBuildPosition()
    {
        var positions = _positions;
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
    private async void RandomBuildPositionCoroutine(CancellationToken token)
    {
        while (_cyrcle.ChekIfDay() && (_progress < _timeToBuild) && _curentlyWorker != null)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(10), cancellationToken: token); // chek for build and randomPosition time, it will not work if it to similary
            if (_curentlyWorker != null)
                _curentlyWorker.Move(RandomBuildPosition(), null);
        }
    }
    private async void BuildingInProgress()
    {
        CancellationTokenSource cts = new CancellationTokenSource();
        CancellationToken token = cts.Token;
        if (_positions != null)
        {
            RandomBuildPositionCoroutine(token);
        }
        
        while (_cyrcle.ChekIfDay() && _progress < _timeToBuild && _curentlyWorker != null && prepearForBuild)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(1)); // chek for build and randomPosition time, it will not work if it to similary
            _progress += 1.0f;
        }

        cts.Cancel();

        if (_progress >= _timeToBuild && prepearForBuild)
        {
            _curentlyWorker.InWork = false;
            _inBuild = false;
            _AIConnector.MoveToClosest();
            _curentlyWorker.GetRandomWalker().Move();
            _curentlyWorker = null;
            _buildFinished.Invoke();
            _progress = 0;
        }
        else
        {
            // wall building interrupted due to day/night cycle
            _curentlyWorker.InWork = false;
            _inBuild = true;
            _AIConnector.MoveToClosest();
            _curentlyWorker.GetRandomWalker().Move();
            _curentlyWorker = null;
        }

        if (!prepearForBuild)
        {
            _inBuild = false;
            _progress = 0;
        }
     }
    public void CanselBuilding()
    {
        prepearForBuild = false;
    }
}
