using GameInit.AI;
using GameInit.Builders;
using GameInit.Hacks;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class Commands
{
    private static DebugCommand RESTART_LEVEL;
   
    private static DebugCommand UNTOUCHABLE_PLAYER;
    private static DebugCommand TOUCHABLE_PLAYER;

    private static DebugCommand SPAWN_STRAY;
    private static DebugCommand SPAWN_BUILDER;
    private static DebugCommand SPAWN_FARMER;
    private static DebugCommand SPAWN_ARCHER;
    private static DebugCommand SPAWN_SWORDMAN;
    private static DebugCommand SPAWN_DEFAULT_ENEMY;

    private static DebugCommand<int> SET_GOLD;

    private static DebugCommand<int> SPAWN_STRAY_COUNT;
    private static DebugCommand<int> SPAWN_BUILDER_COUNT;
    private static DebugCommand<int> SPAWN_FARMER_COUNT;
    private static DebugCommand<int> SPAWN_ARCHER_COUNT;
    private static DebugCommand<int> SPAWN_SWORDMAN_COUNT;
    private static DebugCommand<int> SPAWN_DEFAULT_ENEMY_COUNT;
    private static DebugCommand<int> SPAWN_ENEMY_WAVE;
    
    private List<object> commandList;
    public Commands(ResourceManager _resourceManager, AIBuilder _aiBuilder, HeroBuilder _heroBuilder, BuilderConnectors _builderConnectors)
    {
        CommandsSet(_resourceManager, _aiBuilder, _heroBuilder, _builderConnectors);
        CommandsSetSpawn(_aiBuilder);
    }
    private void CommandsSet(ResourceManager _resourceManager, AIBuilder _aiBuilder, HeroBuilder _heroBuilder, BuilderConnectors _builderConnectors)
    {
        RESTART_LEVEL = new DebugCommand("restart_level", "Restart level from zero", "restart_level", () =>
        {
            Scene scene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(scene.name);
        });

        UNTOUCHABLE_PLAYER = new DebugCommand("untouchable", "Make player untouchable for enemy", "untouchable", () =>
        {
            _builderConnectors.GetAIWarConnector().PointsInWorld.Remove(_heroBuilder.GetHeroMove());
            _builderConnectors.GetAIWarConnector().UpdateTree();
        });

        TOUCHABLE_PLAYER = new DebugCommand("touchable", "Make player touchable for enemy", "touchable", () =>
        {
            _builderConnectors.GetAIWarConnector().PointsInWorld.Add(_heroBuilder.GetHeroMove());
            _builderConnectors.GetAIWarConnector().UpdateTree();
        });

        SET_GOLD = new DebugCommand<int>("set_gold", "Sets the amount of gold", "set_gold <gold_amount>", (x) =>
        {
            _resourceManager.SetResource(ResourceType.Gold, x);
        });

        commandList = new List<object>
        {
          UNTOUCHABLE_PLAYER,
          TOUCHABLE_PLAYER,
          RESTART_LEVEL,
          SET_GOLD,
        };
    }

    private void CommandsSetSpawn(AIBuilder _aiBuilder)
    {
        SPAWN_STRAY = new DebugCommand("spawn_stray", "Spawn stray in vector.zero", "spawn_stray", () =>
        {
            _aiBuilder.GetAISpawner().AllySpawnersList[0].SpawnAllyCommand(ItemsType.None);
        });

        SPAWN_STRAY_COUNT = new DebugCommand<int>("spawn_stray_count", "Spawn stray on mouse or in in vector.zero with count", "spawn_stray_count <gold_amount>", (x) =>
        {
            _aiBuilder.GetAISpawner().AllySpawnersList[0].SpawnAllyCommand(ItemsType.None, x);
        });

        SPAWN_BUILDER = new DebugCommand("spawn_builder", "Spawn builder on mouse or in in vector.zero", "spawn_builder", () =>
        {
            _aiBuilder.GetAISpawner().AllySpawnersList[0].SpawnAllyCommand(ItemsType.Hammer);
        });

        SPAWN_BUILDER_COUNT = new DebugCommand<int>("spawn_builder_count", "Spawn builder on mouse or in in vector.zero with count", "spawn_builder_count <gold_amount>", (x) =>
        {
            _aiBuilder.GetAISpawner().AllySpawnersList[0].SpawnAllyCommand(ItemsType.Hammer, x);
        });

        SPAWN_FARMER = new DebugCommand("spawn_farmer", "Spawn farmer on mouse or in in vector.zero", "spawn_farmer", () =>
        {
            _aiBuilder.GetAISpawner().AllySpawnersList[0].SpawnAllyCommand(ItemsType.Hoe);
        });

        SPAWN_FARMER_COUNT = new DebugCommand<int>("spawn_farmer_count", "Spawn farmer on mouse or in in vector.zero with count", "spawn_farmer_count <gold_amount>", (x) =>
        {
            _aiBuilder.GetAISpawner().AllySpawnersList[0].SpawnAllyCommand(ItemsType.Hoe, x);
        });

        SPAWN_ARCHER = new DebugCommand("spawn_archer", "Spawn archer on mouse or in in vector.zero", "spawn_archer", () =>
        {
            _aiBuilder.GetAISpawner().AllySpawnersList[0].SpawnAllyCommand(ItemsType.Bowl);
        });

        SPAWN_ARCHER_COUNT = new DebugCommand<int>("spawn_archer_count", "Spawn archer on mouse or in in vector.zero with count", "spawn_archer_count <gold_amount>", (x) =>
        {
            _aiBuilder.GetAISpawner().AllySpawnersList[0].SpawnAllyCommand(ItemsType.Bowl, x);
        });

        SPAWN_SWORDMAN = new DebugCommand("spawn_swordman", "Spawn swordman on mouse or in in vector.zero", "spawn_swordman", () =>
        {
            _aiBuilder.GetAISpawner().AllySpawnersList[0].SpawnAllyCommand(ItemsType.Sword);
        });

        SPAWN_SWORDMAN_COUNT = new DebugCommand<int>("spawn_swordman_count", "Spawn swordman on mouse or in in vector.zero with count", "spawn_swordman_count <gold_amount>", (x) =>
        {
            _aiBuilder.GetAISpawner().AllySpawnersList[0].SpawnAllyCommand(ItemsType.Sword, x);
        });

        SPAWN_DEFAULT_ENEMY = new DebugCommand("spawn_default_enemy", "Spawn default enemy on mouse or in vector.zero", "spawn_default_enemy", () =>
        {
            _aiBuilder.GetAISpawner()._EnemySpawnersPrivateList[0].SpawnDefaultEnemy();
        });

        SPAWN_DEFAULT_ENEMY_COUNT = new DebugCommand<int>("spawn_default_enemy_count", "Spawn default enemy on mouse or in vector.zero with count", "spawn_default_enemy_count <gold_amount>", (x) =>
        {
            _aiBuilder.GetAISpawner()._EnemySpawnersPrivateList[0].SpawnDefaultEnemy(x);
        });

        SPAWN_ENEMY_WAVE = new DebugCommand<int>("spawn_enemy_wave", "Spawn default enemy on mouse or in vector.zero with count", "spawn_enemy_wave <gold_amount>", (x) =>
        {
            _aiBuilder.GetAISpawner()._EnemySpawnersPrivateList[0].SpawnEnemyWaveCommand(x);
        });

        commandList.AddRange(new List<object>
        {
        SPAWN_STRAY,
        SPAWN_STRAY_COUNT,
        SPAWN_BUILDER,
        SPAWN_BUILDER_COUNT,
        SPAWN_FARMER,
        SPAWN_FARMER_COUNT,
        SPAWN_ARCHER,
        SPAWN_ARCHER_COUNT,
        SPAWN_SWORDMAN,
        SPAWN_SWORDMAN_COUNT,
        SPAWN_DEFAULT_ENEMY,
        SPAWN_DEFAULT_ENEMY_COUNT,
        SPAWN_ENEMY_WAVE
        });
    }
    public List<object> GetList()
    {
        return commandList;
    }
}
