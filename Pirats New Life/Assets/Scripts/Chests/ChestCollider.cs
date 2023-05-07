using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using GameInit.Builders;
using GameInit.Pool;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using GameInit.Component;
using GameInit.Animation;

namespace GameInit.Chest
{
    [UnityEngine.Scripting.APIUpdating.MovedFrom(true, null, "Assembly-CSharp")]
    public class ChestCollider : IUpdate
    {
        private ChestComponent _chestComponent;
        private ChestBuilder _chestBuilder;
        private Pools _pool;
        private CoinDropAnimation _coinDropAnimation;
        private const bool canPickUp = false;

        public ChestCollider(ChestComponent chestComponent, ChestBuilder chestBuilder, ResourceManager resourceManager, Pools pool, CoinDropAnimation coinDropAnimation)
        {
            _coinDropAnimation = coinDropAnimation;
            _chestComponent = chestComponent;
            _chestBuilder = chestBuilder;
            _pool = pool;
        }

        public void OnUpdate()
        {
            if (_chestComponent.IsCollided)
            {
                _chestComponent.GetAnimator().SetTrigger("Open");
               _chestComponent.GetParticlePrefab().SetActive(true);
                 _coinDropAnimation.RandomCoinJump(_chestComponent.transform.localPosition, _chestComponent.GoldAmount, _chestComponent.transform.position, _pool, canPickUp);
                //  _chestComponent.GetMono().StartCoroutine(MoveCoins());
                _chestBuilder.RemoveChestCollider(this);
                _chestComponent.enabled = false;
            }
        }
    }
}