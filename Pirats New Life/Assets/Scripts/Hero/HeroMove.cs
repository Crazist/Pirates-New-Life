using UnityEngine;

namespace GameInit.Hero
{
    public class HeroMove : IUpdate
    {
        private HeroComponent _heroComponent;
        private RaycastHit _raycastHit;
        private LayerMask _layerMask;

        private bool _RMBIsPressed;
      //  private ParticleSystem _particleSystemMoveTo;

        public HeroMove(HeroComponent heroComponent)
        {
            _heroComponent = heroComponent;

         //   _particleSystemMoveTo = heroComponent.ParticleSystemMoveTo;

            _layerMask = 1 >> 0; // Default Layer
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
                _heroComponent.Agent.SetDestination(_raycastHit.point);

           //     _particleSystemMoveTo.transform.position = _raycastHit.point;
           //     _particleSystemMoveTo.Play();
            }
        }
    }
}