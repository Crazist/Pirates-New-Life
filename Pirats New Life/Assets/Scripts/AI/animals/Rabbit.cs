using GameInit.Animation;
using GameInit.Connector;
using GameInit.Pool;
using GameInit.RandomWalk;
using System.Collections;
using UnityEngine;

namespace GameInit.AI
{
    public class Rabbit : IKDTree, IAnimal
    {
        public int HP { get; set; } = 1;
        public EntityType Type { get; } = EntityType.Animals;
        public SideType Side { get; set; } = SideType.None;
        public bool InMove { get; set; } = false;

        private AIComponent _AIComponent;
        private CoinDropAnimation _coinDropAnimation;
        private Pools _coinPool;
        private RandomWalker randomWalker;
        private AnimalSpawner _AnimalSpawner;
        private AIConnector _AIConnector;

        private bool _isDying;
        private int count = 1;

        private const float _height = 0.46f;
        private const bool canPickUp = true;
        public Rabbit(AIComponent component, CoinDropAnimation coinDropAnimation, Vector3 position, float radiusRandomWalk, Pools coinPool, AnimalSpawner _spawner, AIConnector AIConnector)
        {
            _AnimalSpawner = _spawner;
            _coinPool = coinPool;
            _AIComponent = component;
            _coinDropAnimation = coinDropAnimation;
            _AIConnector = AIConnector;

            randomWalker = new RandomWalker();

            _AIComponent.GetAnimator().SetBool("Iddle", true);

            randomWalker.Init(_AIComponent.GeNavMeshAgent(), position, this, radiusRandomWalk);
        }

        public void ChangePositionForRandomWallk(Vector3 _pos, float radius)
        {
            var pos = new Vector3(_pos.x, _height, _pos.z);
            randomWalker.ChangeOnlyPositionAndStartMove(pos, radius);
        }

        public bool Move(Vector3 position)
        {
            if (InMove == false)
            {
                InMove = true;
                _AIComponent.GetMonoBehaviour().StartCoroutine(Waiter());
                _AIComponent.GeNavMeshAgent().destination = position;
                return true;
            }
            return false;
        }
        private IEnumerator Waiter()
        {
            yield return new WaitForEndOfFrame();

            var agent = _AIComponent.GeNavMeshAgent();

            while (agent.remainingDistance == 0 || !agent.hasPath)
            {
                yield return null;
            }

            _AIComponent.GetAnimator().SetBool("Iddle", false);
            _AIComponent.GetAnimator().SetBool("Move", true);

            while (agent.remainingDistance > agent.stoppingDistance)
            {
                yield return null;
            }

            _AIComponent.GetAnimator().SetBool("Move", false);
            _AIComponent.GetAnimator().SetBool("Iddle", true);

            InMove = false;
        }

        public void Attack()
        {
            // can not
        }

        public bool CheckIfCanDamage()
        {
            return false;
        }

        public bool CheckIfEnemy()
        {
            return true;
        }

        public int CountOFDamage()
        {
            return 0; // can not
        }

        public void GetDamage(int damage)
        {
            if (HP - damage <= 0)
            {
                _AIComponent.GetBloodSplash().Play();
                _AIComponent.GetMonoBehaviour().StartCoroutine(BeforeDie());
            }
            else
            {
                HP = HP - damage;
            }
        }

        private IEnumerator BeforeDie()
        {
            _isDying = true;
            HP = 0; // kd tree look at this, do not forget
            yield return new WaitForSeconds(0.2f);
            Die();
            _isDying = false;
        }
        private void Die()
        {
            _AnimalSpawner.RemoveCurCount();
            var pos = new Vector3(_AIComponent.GetTransform().position.x, _height, _AIComponent.GetTransform().position.z);
            _coinDropAnimation.RandomCoinJump(pos, count, _coinPool, canPickUp);
            _AIComponent.GetGm().SetActive(false);
            _AIComponent.StopAllCoroutines();
            _AIConnector.CheckAndGoToCoin();
        }
        public Vector2 GetPositionVector2()
        {
            Vector2 _positionOnVector2;
            _positionOnVector2.x = _AIComponent.GetTransform().position.x;
            _positionOnVector2.y = _AIComponent.GetTransform().position.z;

            return _positionOnVector2;
        }

        public AIComponent GetAiComponent()
        {
            return _AIComponent;
        }
    }

}
