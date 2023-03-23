using System;
using UnityEngine;

namespace GameInit.AI
{
    public interface IWork
    {
        public Transform getTransform();
        public void Move(Vector3 position, Action action, ItemsType type);
        public void Move(Vector3 position, Action action);
        public ItemsType GetItemType();
        public AIComponent GetAiComponent();
        public int GetId();
        public bool HasCoin();
        public void RemoveAllEveants();
    }
}

