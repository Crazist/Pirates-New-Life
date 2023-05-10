using GameInit.AI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace GameInit.RandomWalk
{
    public class RandomWalker
    {
        private  float _radius = 5f; // the radius around the spawn point where the walker can move
        
        private NavMeshAgent _agent;
        private IWork _work;
        private IAnimal _animal;
        private Vector3 _spawnPoint;
        private bool _inPlay = false;
        private float _randomTime;

        private const float _randomTimeForRabbits = 4f;
        public void Init(NavMeshAgent agent, Vector3 pos, IWork work, float radius)
        {
            _radius = radius;
            _work = work;
            _agent = agent;
            _spawnPoint = pos;
            _inPlay = false;

            _randomTime = Random.Range(5, 15);
            _work.GetAiComponent().GetMonoBehaviour().StartCoroutine(CheckForStartRandomWallk());
        }
        public void Init(NavMeshAgent agent, Vector3 pos, IAnimal animal, float radius)
        {
            _radius = radius;
            _animal = animal;
            _agent = agent;
            _spawnPoint = pos;
            _inPlay = false;

            _randomTime = _randomTimeForRabbits;
            _animal.GetAiComponent().GetMonoBehaviour().StartCoroutine(CheckForStartRandomWallk());
        }
        public void ChangeOnlyPositionAndStartMove(Vector3 pos, float radius)
        {
            _spawnPoint = pos;
            _radius = radius;
            Move();
        }
        public void Move()
        {
            if (_work != null && !_work.InWork && !_work.InMove)
                {
                    Vector3 randomDirection = Random.insideUnitSphere * (_radius);
                    randomDirection += _spawnPoint;

                    NavMeshHit hit;
                    NavMesh.SamplePosition(randomDirection, out hit, _radius, NavMesh.AllAreas);
                    Vector3 finalPosition = hit.position;

                    _agent.SetDestination(finalPosition);
                }
                else if (_animal != null && !_animal.InMove)
                {
                    Vector3 randomDirection = Random.insideUnitSphere * (_radius);
                    randomDirection += _spawnPoint;

                    NavMeshHit hit;
                    NavMesh.SamplePosition(randomDirection, out hit, _radius, NavMesh.AllAreas);
                    Vector3 finalPosition = hit.position;

                    _animal.Move(finalPosition);
                }
        }
        private IEnumerator CheckForStartRandomWallk()
        {
            do
            {
                yield return new WaitForEndOfFrame();

                if (_work != null &&!_inPlay && !_work.InWork)
                {
                    _work.GetAiComponent().GetMonoBehaviour().StartCoroutine(StartRandomWallk());
                }
                else if (_animal != null && !_inPlay)
                {
                    _animal.GetAiComponent().GetMonoBehaviour().StartCoroutine(StartRandomWallkAnimal());
                }
            } while (true);
        }
        private IEnumerator StartRandomWallk()
        {
            _inPlay = true;
            yield return new WaitForSecondsRealtime(_randomTime);
            if (!_work.InMove)
            {
                Move();
            }
            _randomTime = Random.Range(8, 13);
            _inPlay = false;
        }
       
        private IEnumerator StartRandomWallkAnimal()
        {
            _inPlay = true;
            yield return new WaitForSecondsRealtime(_randomTime);
            if (!_animal.InMove)
            {
                Move();
            }
            _randomTime = _randomTimeForRabbits;
            _inPlay = false;
        }

    }
}

