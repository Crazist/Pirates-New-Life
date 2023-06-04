using GameInit.GameCyrcleModule;
using GameInit.LoseAnim;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameInit.Hacks
{
    public class Console : IGuiUpdate, IUpdate
    {
        private bool _showConsole;

        private Commands _commands;

        private string _input;
        
        private List<object> commandList;
       
        public Console(Commands commands)
        {
            _commands = commands;
            commandList = _commands.GetList();
        }

        public void OnUpdate()
        {
            if (Input.GetKeyDown(KeyCode.BackQuote))
            {
               _showConsole = !_showConsole;
            }
       }

        public void OnGuiUpdate()
        {
            if (!_showConsole) { return; }

            float boxHeight = 30f;
            float y = Screen.height - boxHeight;

            GUI.Box(new Rect(0, y, Screen.width, 30), "");
            GUI.backgroundColor = new Color(0, 0, 0, 0);
            GUI.SetNextControlName("ConsoleInput"); // Установка имени контрола
            _input = GUI.TextField(new Rect(10f, y + 5f, Screen.width - 20f, 20f), _input);

            Event e = Event.current;
            if (e.type == EventType.KeyDown && (e.keyCode == KeyCode.Return || e.character == '\n'))
            {
                CheckCommand();
            }
            else if(e.type == EventType.KeyDown && Event.current.keyCode == KeyCode.BackQuote || Event.current.character == '`')
            {
                if (GUI.GetNameOfFocusedControl() == "ConsoleInput") // Проверка фокуса на другие элементы
                {
                    _showConsole = !_showConsole;
                    _input = "";
                    GUI.FocusControl(null);
                }
            }

            GUI.FocusControl("ConsoleInput");
        }
        private void CheckCommand()
        {
            if (_showConsole)
            {
                string[] properties = _input.Split(' ');

                for (int i = 0; i < commandList.Count; i++)
                {
                    DebugCommandBase commandBase = commandList[i] as DebugCommandBase;

                    if (_input != null && properties[0].Equals(commandBase.comandId))
                    {
                        if(commandList[i] as DebugCommand != null) 
                        {
                            if(properties.Length < 2)
                            (commandList[i] as DebugCommand).Invoke();
                        }
                        else if (_input != null && commandList[i] as DebugCommand<int> != null && properties.Length > 1)
                        {
                            if (int.TryParse(properties[1], out int value))
                            {
                                (commandList[i] as DebugCommand<int>).Invoke(value);
                            }
                        }
                    }
                }
                
                _input = "";
            }
        }
    }

}
