using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GameInit.GameCyrcleModule
{
    [DisallowMultipleComponent]
    public class GameCyrcle : MonoBehaviour
    {
        //  private readonly Dictionary<CyrcleMethod, List<ICallable>> _classesToUpdate = new Dictionary<CyrcleMethod, List<ICallable>>();
        private bool dayChange = true;
        // ====== TEST ======
        private readonly List<IUpdate> _updates = new List<IUpdate>(100);
        private readonly List<ILateUpdate> _lateUpdates = new List<ILateUpdate>(20);
        private readonly List<IDayChange> _dayUpdates = new List<IDayChange>();

        [SerializeField] private DayChange _dayChange;

        private bool isDay = true;
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
        public void Add(IUpdate update)
        {
            _updates.Add(update);
        }

        public void Add(ILateUpdate lateUpdate)
        {
            _lateUpdates.Add(lateUpdate);
        }

        public void Add(IDayChange dayUpdates)
        {
            _dayUpdates.Add(dayUpdates);
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

            if (dayChange)
            {
                StartCoroutine(DayChange());
            }
            // ==================
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

        private void OnDisable()
        {
            StopCoroutine(DayChange());
        }
        private IEnumerator DayChange()
        {
            dayChange = false;
            foreach (var day in _dayUpdates)
            {
                day.OnDayChange();
            }
            yield return new WaitForSeconds(300f);
            isDay = !isDay;
            dayChange = true;
        }

        public bool ChekIfDay()
        {
            return isDay;
        }
    }
}