using System.Collections;
using UnityEngine;
using GameInit.Optimization;
using GameInit.Optimization.KDTree;
using System;
using System.Collections.Generic;

namespace GameInit.Projectiles
{
    public class Arrow : IKDTree
    {
        private float arrowSpeed = 10f;
        private Transform _arrow;
        private MonoBehaviour _mono;
        private int _damage = 1;
        private ArrowComponent _component;

        private const float _earthY = 0.46f;

        private Vector3[] path;
        private int currentPoint = 0;

        public int HP { get; set; } = -1;

        public Arrow(ArrowComponent component)
        {
            _component = component;
            _mono = component.GetMono();
            _arrow = component.GetTransform();
        }
        public void Shoot(IKDTree queryPosition, Vector3 endPoint, KDTree tree, List<IKDTree> PointsInWorld, List<IEnemy> EnemyList, KDQuery _KDQuery)
        {
            Vector3 startPosition = new Vector3(queryPosition.GetPositionVector2().x, _earthY, queryPosition.GetPositionVector2().y);

            ProjectileMotionCalculator _projectileMotionCalculator = new ProjectileMotionCalculator();
            path = _projectileMotionCalculator.CalculateParabola(startPosition, endPoint);

            _mono.StartCoroutine(ArrowFly(tree, PointsInWorld, EnemyList,  _KDQuery));
        }
        private IEnumerator ArrowFly(KDTree tree, List<IKDTree> PointsInWorld, List<IEnemy> EnemyList, KDQuery _KDQuery)
        {
            while (currentPoint < path.Length)
            {
                _arrow.position = Vector3.MoveTowards(_arrow.position, path[currentPoint], arrowSpeed * Time.deltaTime);

                // поворачиваем стрелу в сторону следующей точки
                if (currentPoint < path.Length - 1)
                {
                    Vector3 direction = (path[currentPoint + 1] - _arrow.position).normalized;
                    _arrow.rotation = Quaternion.LookRotation(direction);
                }

                if (_arrow.position == path[currentPoint]) currentPoint++;

                yield return null;
            }

            _KDQuery.DamageClosest(tree, this, PointsInWorld, EnemyList);

            tree.Build(PointsInWorld);

            _component.GetGameObj().SetActive(false);
        }
       
        public ArrowComponent GetGm()
        {
            return _component;
        }
        public void Clear()
        {
            if(path != null)
            Array.Clear(path, 0, path.Length);
            currentPoint = 0;
            _mono.StopAllCoroutines();
        }

        public Vector2 GetPositionVector2()
        {
            Vector2 _pos = new Vector2(_component.GetTransform().position.x, _component.GetTransform().position.z);
            return _pos;
        }

        public bool CheckIfEnemy()
        {
            return false;
        }

        public bool CheckIfCanDamage()
        {
            return true;
        }

        public void Attack()
        {
            
        }

        public void GetDamage(int damage)
        {
           //can not
        }

        public int CountOFDamage()
        {
            return _damage;
        }
    }

}
