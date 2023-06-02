using GameInit.AI;
using GameInit.GameCyrcleModule;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameInit.Hacks
{
    public class HacksBuilder
    {
        private Console _console;
        public HacksBuilder(GameCyrcle gameCyrcle, ResourceManager _resourceManager, AIBuilder _aiBuilder)
        {
            var commands = new Commands(_resourceManager, _aiBuilder);
            CreateConsole(gameCyrcle, commands);
        }
        public void CreateConsole(GameCyrcle gameCyrcle, Commands commands)
        {
            _console = new Console(commands);

            gameCyrcle.AddGuiUpdate(_console);
        }
    }

}
