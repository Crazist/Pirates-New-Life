using System;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using GameInit.Animation;
using GameInit.Pool;
using GameInit.Connector;
using GameInit.GameCyrcleModule;
using GameInit.Builders;

public class Tower :  IKDTree
{
    public int HP { get; set; } = 0;
    public EntityType Type { get;} = EntityType.Tower;
    public SideType Side { get; set; } = SideType.None;

    private TowerBuildingComponent _TowerBuildingComponent;
    private Action _action;
    private Action _onBuildFinish;
    private CoinDropAnimation _coinDropAnimation;
    private Pools _coinPool;
    private CancellationToken token;
    private BuildingChecker _BuildingChecker;
    private AIWarConnector _AIWarConnector;
    private bool _isBuild = false;
    private int _curGold = 0;
    private bool checkForGold = false;
    private int curForm = 0;
    private int _damage = 0;
    private float _delayForAttack = 4f;
    private bool _canDamage = true;
    private bool _rootTower = false;
    private bool firstBuild = true;

    private const bool _canPickUp = false;
    private const int _moneyWaitDelay = 1;
    public Tower(TowerBuildingComponent towerBuildingComponent, CoinDropAnimation coinDropAnimation,Pools coinPool, BuilderConnectors _BuilderConnector, GameCyrcle cyrcle)
    {
        _TowerBuildingComponent = towerBuildingComponent;
        _coinPool = coinPool;
        _coinDropAnimation = coinDropAnimation;
        CancellationTokenSource cts = new CancellationTokenSource();
        token = cts.Token;
        _AIWarConnector = _BuilderConnector.GetAIWarConnector();

        _onBuildFinish += BuildingFinish;
        _action += CountMoney;
        _TowerBuildingComponent.SetAction(_action);

        _BuildingChecker = new BuildingChecker(towerBuildingComponent.GetBuildingsPosition(), _BuilderConnector.GetAiConnector(), cyrcle, _onBuildFinish, towerBuildingComponent.GetTimeForBuild());
    }
    private void DropBeforePickUpCoin(int count)
    {
        _coinDropAnimation.RandomCoinJump(_TowerBuildingComponent.transform.position, count, _coinPool, _canPickUp);
    }
    private async void CountMoney()
    {
        _curGold++;
        _TowerBuildingComponent.SetCurGold(_curGold);
        checkForGold = true;
        int gold = 0;

        while (checkForGold && _curGold < _TowerBuildingComponent.GetCountOfGold())
        {
            if (gold < _curGold)
            {
                gold++;
               await UniTask.Delay(TimeSpan.FromSeconds(_moneyWaitDelay), cancellationToken: token);
            }
            else
            {
                checkForGold = false;
            }
        }

        if (_curGold == _TowerBuildingComponent.GetCountOfGold())
        {
            _TowerBuildingComponent.SetCanProduce(false);
            _BuildingChecker.Build();
        }
        else
        {
            DropBeforePickUpCoin(_curGold);
        }
        _curGold = 0;
    }
    private void BuildingFinish()
    {
        _isBuild = true;

        if (firstBuild)
        {
            firstBuild = false;
            _AIWarConnector.TowerList.Add(this);
        }
       
        _TowerBuildingComponent.GetFormList()[curForm].gameObject.SetActive(false);
        curForm++;
        _TowerBuildingComponent.GetFormList()[curForm].gameObject.SetActive(true);

        _damage = curForm;
        _delayForAttack = _delayForAttack - ((curForm) * 0.5f);

        //MoveBuilder();
        if (curForm < _TowerBuildingComponent.GetFormList().Count - 1)
        {
            _TowerBuildingComponent.SetCanProduce(true);
            _TowerBuildingComponent.SetNewCountOfgold(_TowerBuildingComponent.GetCountOfGold() * 3);
        }
        else
        {
            _TowerBuildingComponent.SetCanProduce(false);
        }
    }
    public void UpdateFast(int form)
    {
        _rootTower = true;
        _BuildingChecker.CanselBuilding();

        _rootTower = true;

        _TowerBuildingComponent.SetNewCountOfgold(_TowerBuildingComponent.GetCountOfGold() * 3);
        
        if (!_isBuild)
        {
            curForm = form - 1;
            _TowerBuildingComponent.SetCanProduce(true);
            return;
        }
       
        _TowerBuildingComponent.GetFormList()[curForm].gameObject.SetActive(false);
        
        curForm = form;

        if (curForm < _TowerBuildingComponent.GetFormList().Count - 1)
        {
            _TowerBuildingComponent.SetCanProduce(true);
        }
        else
        {
            _TowerBuildingComponent.SetCanProduce(false);
        }

        _TowerBuildingComponent.GetFormList()[curForm].gameObject.SetActive(true);

        _damage = curForm;
        _delayForAttack = _delayForAttack - ((curForm) * 0.5f);

        _AIWarConnector.GetSideCalculation().SetSwordManToNewPosition();
        _AIWarConnector.GetSideCalculation().RandomAnimalPosition();
    }
    private async void AttackDelay()
    {
        _canDamage = false;
        await UniTask.Delay(TimeSpan.FromSeconds(_delayForAttack), cancellationToken: token);
        _canDamage = true;
    }
    public void Destroy()
    {
        firstBuild = true;
       _AIWarConnector.TowerList.Remove(this);
       _TowerBuildingComponent.GetFormList()[curForm].gameObject.SetActive(false);
       _TowerBuildingComponent.SetCanProduce(false);
       _TowerBuildingComponent.GetFormList()[0].gameObject.SetActive(true);
        if (_isBuild)
        {
            _TowerBuildingComponent.SetNewCountOfgold(_TowerBuildingComponent.GetCountOfGold() / 3);
        }
     
        _isBuild = false;

        if (!_rootTower)
        {
            curForm = 0;
        }
        else
        {
            curForm--;
        }
        _damage = 0;
        _delayForAttack = 4;
    }
    public void Attack()
    {
        if (_canDamage)
        {
            AttackDelay();
        }
    }

    public bool CheckIfCanDamage()
    {
        return _canDamage;
    }

    public bool CheckIfEnemy()
    {
        return false;
    }

    public int CountOFDamage()
    {
        return _damage;
    }

    public void GetDamage(int damage)
    {
       // die only with wall
    }

    public Vector2 GetPositionVector2()
    {
        Vector2 _positionOnVector2;
        _positionOnVector2.x = _TowerBuildingComponent.GetShootPosition().x;
        _positionOnVector2.y = _TowerBuildingComponent.GetShootPosition().z;

        return _positionOnVector2;
    }
    public void SetProduce(bool canProduce)
    {
        _TowerBuildingComponent.SetCanProduce(canProduce);
    }

    public void OnDayChange()
    {
        _BuildingChecker.CheckIfNeedBuild();
    }
}
