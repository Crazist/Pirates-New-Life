using GameInit.Animation;
using GameInit.Builders;
using GameInit.Pool;
using System.Collections;
using UnityEngine;

namespace GameInit.Hero
{
    public class HeroMove : IUpdate, IKDTree
    {
        private HeroComponent _heroComponent;
        private RaycastHit _raycastHit;
        private LayerMask _layerMask;
        private UIBuilder __UIBuilder;
        private ResourceManager _ResourceManager;
        private CoinDropAnimation _CoinDropAnimation;
        private Pools _coinPool;
        private CameraControl _CameraControl;
        private Animator _Animator;

        private const int _resiveDamage = 5;

        private bool _RMBIsPressed;
        private bool _isWalking;
        private const bool _canAttack = false;
        private const bool _isEnemy = false;
        private const int _damage = 0;
        private int _currentAnimationIndex = 0;
        
        [SerializeField] private string[] _animationNames = { "iddle", "iddle2", "iddle3" };
        [SerializeField] private float _animationSwitchDelay = 20f;
        //  private ParticleSystem _particleSystemMoveTo;
        public int HP { get; set; } = 1;
        public EntityType Type { get; } = EntityType.Hero;

        public HeroMove(HeroComponent heroComponent, UIBuilder UIBuilder, CoinDropAnimation CoinDropAnimation, ResourceManager ResourceManager, Pools coinPool)
        {
            _CameraControl = Object.FindObjectOfType<CameraControl>();

            _coinPool = coinPool;
            _CoinDropAnimation = CoinDropAnimation;
            _ResourceManager = ResourceManager;
            __UIBuilder = UIBuilder;
            _heroComponent = heroComponent;

         //   _particleSystemMoveTo = heroComponent.ParticleSystemMoveTo;

            _layerMask = 1 >> 0; // Default Layer
            _Animator = heroComponent.GetAnimator();
            heroComponent.GetMono().StartCoroutine(Animate());
        }

        private IEnumerator Animate()
        {
            while (true && HP > 0)
            {
                // Ждем _animationSwitchDelay секунд
                yield return new WaitForSeconds(_animationSwitchDelay);

                // Выбираем следующую анимацию
                _currentAnimationIndex++;
                if (_currentAnimationIndex >= _animationNames.Length)
                    _currentAnimationIndex = 0;

                // Меняем анимацию
                switch (_currentAnimationIndex)
                {
                    case 0:
                           _Animator.SetBool(_animationNames[0], true);
                        break;
                    case 1:
                        _Animator.SetBool(_animationNames[1], true);
                        break;
                    case 2:
                        _Animator.SetBool(_animationNames[2], true);
                        break;
                }
            }
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
            int _countGold = -1;

            //we have constant getdamage
            if (_ResourceManager.GetDamage(_resiveDamage, out _countGold))
            {
                if (HP <= 0) return;
                // _heroComponent.gameObject.SetActive(false);
                _Animator.SetTrigger("die");
                __UIBuilder.GetLoseSceneAnimation().Lose();
                _heroComponent.Agent.enabled = false;

                if (_countGold == 0) return;

                if (_countGold > 0)
                {
                    _countGold = 0;
                    _CoinDropAnimation.RandomCoinJump(_heroComponent.transform.position, _countGold, _heroComponent.transform.position, _coinPool, false);
                }
                HP = 0;
                return;
            }
            
            _CoinDropAnimation.RandomCoinJump(_heroComponent.transform.position, _resiveDamage, _heroComponent.transform.position, _coinPool, false);
            _CameraControl.ShakeCameraAnimation();
        }
        
        public Vector2 GetPositionVector2()
        {
            Vector2 pos = new Vector2(_heroComponent.GetTransform().position.x, _heroComponent.GetTransform().position.z);
            return pos;
        }

        public void OnUpdate()
        {
            if(HP > 0)
            {
                if (Input.GetMouseButtonDown(1))
                    _RMBIsPressed = true;

                if (Input.GetMouseButtonUp(1))
                    _RMBIsPressed = false;

                if (_heroComponent.Agent.velocity.magnitude <= 0.1f)
                {
                    if (_isWalking)
                    {
                        _isWalking = false;
                        _Animator.SetBool("walk", false);
                        _Animator.SetBool("iddle", true);
                    }
                }
                else
                {
                    if (!_isWalking)
                    {
                        _isWalking = true;
                        _Animator.SetBool("walk", true);
                        _Animator.SetBool("iddle", false);
                    }
                }

                if (!_RMBIsPressed) return;

                if (Physics.Raycast(
                        UnityEngine.Camera.main.ScreenPointToRay(Input.mousePosition),
                        out _raycastHit,
                        Mathf.Infinity,
                        _layerMask,
                        QueryTriggerInteraction.Ignore))
                {
                    if (_heroComponent.isActiveAndEnabled)
                    {
                        _heroComponent.Agent.SetDestination(_raycastHit.point);
                    }
                    // _particleSystemMoveTo.transform.position = _raycastHit.point;
                    // _particleSystemMoveTo.Play();
                }
            }
        }
    }
}