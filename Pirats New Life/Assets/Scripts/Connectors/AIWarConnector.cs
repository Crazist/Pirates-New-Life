using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameInit.AI;
using System;
using GameInit.Pool;
using GameInit.RandomWalk;
using GameInit.Optimization;
using GameInit.GameCyrcleModule;
using GameInit.Building;

public class AIWorkConnector : IUpdate, IDayChange
{
    public List<IWar> SwordManList { get; set; }
    public List<IWar> ArcherList { get; set; }
    public List<IWar> EnemyList { get; set; }
   

    public List<List<IWar>> ListOfLists { get; set; }

    private Pools _pool;
    private HeroComponent _heroComponent;
    private List<Action> lateMove;
    private GameCyrcle _gameCyrcle;
   
    private const int _minDistance = 5;
    private const float _minimalDistanceToHero = 1f;

    public AIWorkConnector(Pools pool, GameCyrcle cyrcle)
    {
        ListOfLists = new List<List<IWar>>();
        
        ListOfLists.Add(SwordManList = new List<IWar>());
        ListOfLists.Add(ArcherList = new List<IWar>());
        EnemyList = new List<IWar>();
        

        _gameCyrcle = cyrcle;
        _pool = pool;
    }

    public void GetHeroComponent(HeroComponent heroComponent)
    {
        _heroComponent = heroComponent;
    }

    public void OnUpdate()
    {
    }

    public void OnDayChange()
    {
    }
}
