using GameInit.Builders;
using UnityEngine;

namespace GameInit.Hero
{
    public class HeroMove : IUpdate, IKDTree
    {
        private HeroComponent _heroComponent;
        private RaycastHit _raycastHit;
        private LayerMask _layerMask;
        private UIBuilder __UIBuilder;

        private bool _RMBIsPressed;
        private const bool _canAttack = false;
        private const bool _isEnemy = false;
        private const int _damage = 0;
        //  private ParticleSystem _particleSystemMoveTo;
        public int HP { get; set; } = 1;

        public HeroMove(HeroComponent heroComponent, UIBuilder UIBuilder)
        {
            __UIBuilder = UIBuilder;
            _heroComponent = heroComponent;

         //   _particleSystemMoveTo = heroComponent.ParticleSystemMoveTo;

            _layerMask = 1 >> 0; // Default Layer
        }

       
        public void Attack()
        {
           //can not
        }

        public bool CheckIfCanDamage()
        {
          return  _canAttack;
        }

        public bool CheckIfEnemy()
        {
            return _isEnemy;
        }

        public int CountOFDamage()
        {
            return _damage;
        }

        public void GetDamage(int damage)
        {
            _heroComponent.gameObject.SetActive(false);
            __UIBuilder.GetLoseSceneAnimation().Lose();
        }

        public Vector2 GetPositionVector2()
        {
            Vector2 pos = new Vector2(_heroComponent.GetTransform().position.x, _heroComponent.GetTransform().position.z);
            return pos;
        }

        public void OnUpdate()
        {
            if (Input.GetMouseButtonDown(1))
                _RMBIsPressed = true;

            if (Input.GetMouseButtonUp(1))
                _RMBIsPressed = false;

            if (!_RMBIsPressed) return; 

            if (Physics.Raycast(
                    UnityEngine.Camera.main.ScreenPointToRay(Input.mousePosition),
                    out _raycastHit,
                    Mathf.Infinity,
                    _layerMask,
                    QueryTriggerInteraction.Ignore))
            {
                if(_heroComponent.isActiveAndEnabled)
                _heroComponent.Agent.SetDestination(_raycastHit.point);

               // _particleSystemMoveTo.transform.position = _raycastHit.point;
               // _particleSystemMoveTo.Play();
            }
        }
    }
}