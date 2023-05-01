using GameInit.RandomWalk;
using System;
using UnityEngine;

namespace GameInit.AI
{
    public interface IWork
    {
        public bool InMove { get; set; }
        public bool InWork { get; set; }
        public bool GoingForCoin { get; set; }
        public Transform getTransform();
        public bool Move(Vector3 position, Action action, ItemsType type);
        public bool Move(Vector3 position, Action action);
        public ItemsType GetItemType();
        public AIComponent GetAiComponent();
        public int GetId();
        public bool HasCoin();
        public void RemoveAllEveants();
        public void CheckIfPlayerWaitForCoins();
        public RandomWalker GetRandomWalker();
        public void CollectGold();
    }
}

