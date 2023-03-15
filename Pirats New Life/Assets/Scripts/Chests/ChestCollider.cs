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

namespace GameInit.Chest
{
    [UnityEngine.Scripting.APIUpdating.MovedFrom(true, null, "Assembly-CSharp")]
    public class ChestCollider : IUpdate
    {
        private ChestComponent _chestComponent;
        private HeroComponent _heroComponent;
        private ChestBuilder _chestBuilder;
        private ResourceManager _resourceManager;
        private Pools _pool;
        private List<GameObject> coins;
        private List<Vector3> positions;
        private float speedSplash = 0.5f;
        private const float heightPosition = -8.71f;

        private float _jumpHeight = 3f;
        private float _bounceTimes = 0.5f;
        private int _jumpCount = 1;
        private const float coefChangeBounce = 0.8f;
        private const float minBounce = -0.2f;
        private const float maxBounce = 0.2f;
        private const float minJumpHeightSecond = 0.5f;
        private const float maxJumpHeightSecond = 0.8f;
        private const float minJumpHeightThird = 0.2f;
        private const float maxJumpHeightThird = 0.4f;
        private const int randJumpZero = 0;
        private const float coefChangeBounceMax = 1.2f;

        public ChestCollider(ChestComponent chestComponent, ChestBuilder chestBuilder, ResourceManager resourceManager, Pools pool)
        {
            _chestComponent = chestComponent;
            _chestBuilder = chestBuilder;
            _resourceManager = resourceManager;
            _pool = pool;
            coins = new List<GameObject>();
            positions = new List<Vector3>();
        }

        public void OnUpdate()
        {
            if (_chestComponent.IsCollided)
            {
                var pos = _chestComponent.transform.localPosition;
                for (int i = 0; i < _chestComponent.GoldAmount; i++)
                {
                    var x = Random.Range(pos.x - 4f, pos.x + 4f);
                    var z = Random.Range(pos.z - 4f, pos.z + 4f);
                    positions.Add(new Vector3(x, heightPosition, z));
                    var coin = _pool.GetFreeElements(_chestComponent.transform.position);
                    JumpOut(new Vector3(x, heightPosition, z),coin.transform);
                }
              //  _chestComponent.GetMono().StartCoroutine(MoveCoins());
                _chestBuilder.RemoveChestCollider(this);
                _chestComponent.enabled = false;
            }
        }
        
        private void JumpOut(Vector3 targetPosition, Transform transform)
        {
            Vector3 newTargetPosition = Vector3.Lerp(transform.position, targetPosition, coefChangeBounce);

            // Define your initial jump tween with the new target position
            float randomJumpHeight1 = Random.Range(_jumpHeight * coefChangeBounce, _jumpHeight * coefChangeBounceMax); // Randomize jump height
            int randomJumpCount1 = _jumpCount + Random.Range(0, 2); // Randomize jump count
            float randomBounceTimes1 = _bounceTimes + Random.Range(minBounce, maxBounce); // Randomize bounce times

            // Define your second jump tween with updated jumpHeight value and original target position
            float randomJumpHeight2 = randomJumpHeight1 * Random.Range(minJumpHeightSecond, maxJumpHeightSecond); // Randomize jump height
            int randomJumpCount2 = randomJumpCount1 - 1; // Reduce jump count by 1
            float randomBounceTimes2 = randomBounceTimes1 + Random.Range(minBounce, maxBounce); // Randomize bounce times

            // Define your third jump tween with updated jumpHeight value and original target position
            float randomJumpHeight3 = randomJumpHeight2 * Random.Range(minJumpHeightThird, maxJumpHeightThird); // Randomize jump height
            int randomJumpCount3 = randomJumpCount2 - 1; // Reduce jump count by 1
            float randomBounceTimes3 = randomBounceTimes2 + Random.Range(minBounce, maxBounce); // Randomize bounce times

            // Choose either the 2nd or 3rd jump to use
            int randomJump = Random.Range(1, 4);
            Tween jumpTween;
            switch (randomJump)
            {
                case 1:
                    jumpTween = transform.DOJump(newTargetPosition, randomJumpHeight1, randomJumpCount1, randomBounceTimes1, false);
                    break;
                case 2:
                    jumpTween = transform.DOJump(newTargetPosition, randomJumpHeight2, randomJumpCount2, randomBounceTimes2, false);
                    break;
                default:
                    jumpTween = transform.DOJump(newTargetPosition, randomJumpHeight3, randomJumpCount3, randomBounceTimes3, false);
                    break;
            }

            jumpTween.SetEase(Ease.OutQuad)
                .SetDelay(Random.Range(0f, 0.1f)) // Add a random delay to each jump
                .OnComplete(() =>
                {
                    // Create a new jump tween with updated jumpHeight value and original target position
                    transform.DOJump(targetPosition, randomJumpHeight2 * coefChangeBounce, randomJumpCount2, randomBounceTimes2 * coefChangeBounce, false)
                        .SetEase(Ease.InOutQuad)
                        .SetDelay(Random.Range(0f, 0.1f)) // Add a random delay to each jump
                        .SetEase(Ease.OutQuad)
                        .SetEase(Ease.InQuad)
                        .SetEase(Ease.InCubic)
                        .SetEase(Ease.InOutSine)
                        .SetEase(Ease.InOutCubic)
                        .SetEase(Ease.InBounce)
                        .SetEase(Ease.InElastic)
                        .SetEase(Ease.OutElastic)
                        .SetEase(Ease.OutBounce);
                });
        }
    }
    
}