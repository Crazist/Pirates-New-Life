using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GameInit.GameCyrcleModule
{
    [DisallowMultipleComponent]
    public class GameCyrcle : MonoBehaviour
    {
        public int DayCount { get; set; }
        //  private readonly Dictionary<CyrcleMethod, List<ICallable>> _classesToUpdate = new Dictionary<CyrcleMethod, List<ICallable>>();
        private bool dayChange = true;
        // ====== TEST ======
        private readonly List<IUpdate> _updates = new List<IUpdate>(100);
        private readonly List<ILateUpdate> _lateUpdates = new List<ILateUpdate>(20);
        private readonly List<IDayChange> _dayUpdates = new List<IDayChange>();
        private readonly List<IGuiUpdate> _GuiUpdates = new List<IGuiUpdate>();

        [SerializeField] private DayChange _dayChange;

        private Action _dayChangeAction;
        
        [SerializeField] private bool isDay = false;
        // ==================

        /* public void Init()
         {
             _classesToUpdate[CyrcleMethod.Update] = new List<ICallable>();
             _classesToUpdate[CyrcleMethod.LateUpdate] = new List<ICallable>();
         }
         public void Add(CyrcleMethod method, ICallable callable)
         {
             if (!_classesToUpdate[method].Contains(callable))
             {
                 _classesToUpdate[method].Add(callable);
             }
         }
         public void Remove(CyrcleMethod method, ICallable callable)
         {
             if (_classesToUpdate[method].Contains(callable))
             {
                 _classesToUpdate[method].Remove(callable);
             }
         }*/

        // ====== TEST ======
        private void Start()
        {
            _dayChangeAction += DayChange;
            _dayChange.SetDayChangeAction(_dayChangeAction);
        }
        public void Add(IUpdate update)
        {
            _updates.Add(update);
        }

        public void Add(ILateUpdate lateUpdate)
        {
            _lateUpdates.Add(lateUpdate);
        }

        public void AddDayChange(IDayChange dayUpdates)
        {
            _dayUpdates.Add(dayUpdates);
        }
        public void AddGuiUpdate(IGuiUpdate guiUpdates)
        {
            _GuiUpdates.Add(guiUpdates);
        }
        public void Remove(IUpdate update)
        {
            _updates.Remove(update);
        }

        public void Remove(ILateUpdate lateUpdate)
        {
            _lateUpdates.Remove(lateUpdate);
        }
        public void Remove(IDayChange dayUpdates)
        {
            _dayUpdates.Remove(dayUpdates);
        }
        public void Remove(IGuiUpdate guiUpdates)
        {
            _GuiUpdates.Remove(guiUpdates);
        }
        // ==================

        private void Update()
        {
            /*foreach (var item in _classesToUpdate[CyrcleMethod.Update].ToArray())
            {
                item.UpdateCall();
            }*/

            // ====== TEST ======
            foreach (var update in _updates.ToArray())
            {
                update?.OnUpdate();
            }
        }
        private void OnGUI()
        {
            foreach (var update in _GuiUpdates.ToArray())
            {
                update?.OnGuiUpdate();
            }
        }
        private void LateUpdate()
        {
            /*foreach (var item in _classesToUpdate[CyrcleMethod.LateUpdate].ToArray())
            {
                item.UpdateCall();
            }*/

            // ====== TEST ======
            foreach (var lateUpdate in _lateUpdates.ToArray())
            {
                lateUpdate.OnLateUpdate();
            }
            // ==================
        }
        public void DayChange()
        {
            dayChange = false;
            isDay = !isDay;
            foreach (var day in _dayUpdates)
            {
                day.OnDayChange();
            }
            dayChange = true;
          
            if (isDay)
            {
                DayCount++;
            }

        }
        public bool ChekIfDay()
        {
            return isDay;
        }
       
    }
}