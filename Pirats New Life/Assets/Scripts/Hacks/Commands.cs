using GameInit.AI;
using GameInit.Hacks;
using System.Collections.Generic;

public class Commands
{
    private static DebugCommand RESTART_LEVEL;
    private static DebugCommand SPAWN_STRAY;
    private static DebugCommand SPAWN_BUILDER;
    private static DebugCommand SPAWN_FARMER;
    private static DebugCommand SPAWN_ARCHER;
    private static DebugCommand SPAWN_SWORDMAN;

    private static DebugCommand<int> SPAWN_STRAY_COUNT;
    private static DebugCommand<int> SPAWN_BUILDER_COUNT;
    private static DebugCommand<int> SPAWN_FARMER_COUNT;
    private static DebugCommand<int> SPAWN_ARCHER_COUNT;
    private static DebugCommand<int> SPAWN_SWORDMAN_COUNT;
    private static DebugCommand<int> SET_GOLD;

    private List<object> commandList;
    public Commands(ResourceManager _resourceManager, AIBuilder _aiBuilder)
    {
        CommandsSet(_resourceManager, _aiBuilder);
        CommandsSetSpawn(_aiBuilder);
    }
    private void CommandsSet(ResourceManager _resourceManager, AIBuilder _aiBuilder)
    {
        RESTART_LEVEL = new DebugCommand("restart_level", "Restart level from zero", "restart_level", () =>
        {
           
        });
        
        SET_GOLD = new DebugCommand<int>("set_gold", "Sets the amount of gold", "set_gold <gold_amount>", (x) =>
        {
            _resourceManager.SetResource(ResourceType.Gold, x);
        });

        commandList = new List<object>
        {
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

        SPAWN_STRAY_COUNT = new DebugCommand<int>("spawn_stray", "Spawn stray in vector.zero with count", "spawn_stray_count <gold_amount>", (x) =>
        {
            _aiBuilder.GetAISpawner().AllySpawnersList[0].SpawnAllyCommand(ItemsType.None, x);
        });

        SPAWN_BUILDER = new DebugCommand("spawn_builder", "Spawn builder in vector.zero", "spawn_builder", () =>
        {
            _aiBuilder.GetAISpawner().AllySpawnersList[0].SpawnAllyCommand(ItemsType.Hammer);
        });

        SPAWN_BUILDER_COUNT = new DebugCommand<int>("spawn_builder", "Spawn builder in vector.zero with count", "spawn_builder_count <gold_amount>", (x) =>
        {
            _aiBuilder.GetAISpawner().AllySpawnersList[0].SpawnAllyCommand(ItemsType.Hammer, x);
        });

        SPAWN_FARMER = new DebugCommand("spawn_farmer", "Spawn farmer in vector.zero", "spawn_farmer", () =>
        {
            _aiBuilder.GetAISpawner().AllySpawnersList[0].SpawnAllyCommand(ItemsType.Hoe);
        });

        SPAWN_FARMER_COUNT = new DebugCommand<int>("spawn_farmer", "Spawn farmer in vector.zero with count", "spawn_farmer_count <gold_amount>", (x) =>
        {
            _aiBuilder.GetAISpawner().AllySpawnersList[0].SpawnAllyCommand(ItemsType.Hoe, x);
        });

        SPAWN_ARCHER = new DebugCommand("spawn_archer", "Spawn archer in vector.zero", "spawn_archer", () =>
        {
            _aiBuilder.GetAISpawner().AllySpawnersList[0].SpawnAllyCommand(ItemsType.Bowl);
        });

        SPAWN_ARCHER_COUNT = new DebugCommand<int>("spawn_archer", "Spawn archer in vector.zero with count", "spawn_archer_count <gold_amount>", (x) =>
        {
            _aiBuilder.GetAISpawner().AllySpawnersList[0].SpawnAllyCommand(ItemsType.Bowl, x);
        });

        SPAWN_SWORDMAN = new DebugCommand("spawn_swordman", "Spawn swordman in vector.zero", "spawn_swordman", () =>
        {
            _aiBuilder.GetAISpawner().AllySpawnersList[0].SpawnAllyCommand(ItemsType.Sword);
        });

        SPAWN_SWORDMAN_COUNT = new DebugCommand<int>("spawn_swordman", "Spawn swordman in vector.zero with count", "spawn_swordman_count <gold_amount>", (x) =>
        {
            _aiBuilder.GetAISpawner().AllySpawnersList[0].SpawnAllyCommand(ItemsType.Sword, x);
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
        SPAWN_SWORDMAN_COUNT
        });
    }
    public List<object> GetList()
    {
        return commandList;
    }
}
