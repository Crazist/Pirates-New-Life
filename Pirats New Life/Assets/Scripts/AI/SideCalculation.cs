using GameInit.Building;
using GameInit.Connector;
using System.Collections.Generic;
using UnityEngine;
using GameInit.GameCyrcleModule;
using GameInit.Attack;

namespace GameInit.AI
{
    public class SideCalculation
    {
        public List<IKDTree> ArcherLeftSide { get; set; }
        public List<IKDTree> ArcherRightSide { get; set; }
        public List<IKDTree> SworManLeftSide { get; set; }
        public List<IKDTree> SworManRightSide { get; set; }

        private ArcherAttack _ArcherAttack;
        private SwordManAttack _SwordManAttack;

        private Wall lastRightWall = null;
        private Wall lastLeftWall = null;
        private AIWarConnector _AIWarConnector;
        private GameCyrcle _gameCyrcle;

        private float radiusRandomWalkInDay = 10;
        private float radiusRandomWalkInNight = 3;

        private const float _wallOffset = 22f;
        private const float radiusRandomWalkAnimal = 20f;
        private const int offsetSwordMan = 4;
        private const int offsetArcher = 7;
        private const int offsetArcherInday = 10;

        public SideCalculation(AIWarConnector aIWarConnector, GameCyrcle gameCyrcle)
        {
            _AIWarConnector = aIWarConnector;
            _gameCyrcle = gameCyrcle;

            ArcherLeftSide = new List<IKDTree>();
            ArcherRightSide = new List<IKDTree>();
            SworManLeftSide = new List<IKDTree>();
            SworManRightSide = new List<IKDTree>();
        }
        public void SetAttackComponentsArcher(ArcherAttack ArcherAttack)
        {
            _ArcherAttack = ArcherAttack;
        }
        public void SetAttackComponentsSwordMan(SwordManAttack SwordManAttack)
        {
            _SwordManAttack = SwordManAttack;
        }
        public void SetSwordManToNewPosition()
        {
            Wall _lastRightWall = null;
            Wall _lastLeftWall = null;

            // Находим последнюю незавершенную стену справа и слева
            for (int i = 0; i < _AIWarConnector.RightWall.Count; i++)
            {
                if (_AIWarConnector.RightWall[i].isBuilded)
                {
                    _lastRightWall = _AIWarConnector.RightWall[i];
                }
            }
            for (int i = 0; i < _AIWarConnector.LeftWall.Count; i++)
            {
                if (_AIWarConnector.LeftWall[i].isBuilded)
                {
                    _lastLeftWall = _AIWarConnector.LeftWall[i];
                }
            }

            lastRightWall = _lastRightWall;
            lastLeftWall = _lastLeftWall;

            MoveSwordMans(_lastRightWall, _lastLeftWall);
            MoveArcher(_lastRightWall, _lastLeftWall);
        }
        private void MoveSwordMans(Wall lastRightWall, Wall lastLeftWall)
        {
            SworManLeftSide.Clear();
            SworManRightSide.Clear();

            int rightCount = 0;
            int leftCount = 0;
            var swordManList = _AIWarConnector.SwordManList;
            List<IWork> noneType = new List<IWork>();
            List<IWork> withType = new List<IWork>();

            foreach (var archer in swordManList)
            {
                if (archer is IKDTree kdTreeSwordMan)
                {
                    if (kdTreeSwordMan.Side == SideType.RightFar)
                    {
                        rightCount++;
                        withType.Add(archer);
                    }
                    else if (kdTreeSwordMan.Side == SideType.LeftFar)
                    {
                        leftCount++;
                        withType.Add(archer);
                    }
                    else if (kdTreeSwordMan.Side == SideType.None)
                    {
                        noneType.Add(archer);
                    }
                }
            }

            for (int i = 0; i < noneType.Count; i++)
            {
                var swordMan = (IKDTree)noneType[i];
                var _attackSwordMan = (IAttack)noneType[i];
                Vector3 targetWall = Vector3.zero;
                // Если есть пустое место справа, перемещаем мечника туда
                if ((rightCount <= leftCount || lastLeftWall == null) && lastRightWall != null)
                {
                    targetWall = new Vector3(lastRightWall.GetPositionVector3().x - offsetSwordMan, lastRightWall.GetPositionVector3().y, lastRightWall.GetPositionVector3().z);
                    swordMan.Side = SideType.RightFar;
                    rightCount++;
                }
                // Если есть пустое место слева, перемещаем мечника туда
                else if ((leftCount <= rightCount || lastRightWall == null) && lastLeftWall != null)
                {
                    targetWall = new Vector3(lastLeftWall.GetPositionVector3().x + offsetSwordMan, lastLeftWall.GetPositionVector3().y, lastLeftWall.GetPositionVector3().z);
                    swordMan.Side = SideType.LeftFar;
                    leftCount++;
                }

                if (lastRightWall != null && !_attackSwordMan.InAttack)
                {
                    SworManRightSide.Add(swordMan);
                    noneType[i].GetRandomWalker().ChangeOnlyPositionAndStartMove(targetWall, radiusRandomWalkInNight);
                }
                else if (lastLeftWall != null && !_attackSwordMan.InAttack)
                {
                    SworManLeftSide.Add(swordMan);
                    noneType[i].GetRandomWalker().ChangeOnlyPositionAndStartMove(targetWall, radiusRandomWalkInNight);
                }
            }

            for (int i = 0; i < withType.Count; i++)
            {
                var swordMan = (IKDTree)withType[i];
                Vector3 targetWall = Vector3.zero;
                var type = swordMan.Side;

                if (swordMan.Side != SideType.None && swordMan.Side == SideType.RightFar && lastRightWall != null && (Mathf.Abs(leftCount - rightCount) == 1 || leftCount - rightCount == 0))
                {
                    targetWall = new Vector3(lastRightWall.GetPositionVector3().x - offsetSwordMan, lastRightWall.GetPositionVector3().y, lastRightWall.GetPositionVector3().z);
                    swordMan.Side = SideType.RightFar;
                }
                else if (swordMan.Side != SideType.None && swordMan.Side == SideType.LeftFar && lastLeftWall != null && (Mathf.Abs(rightCount - leftCount) == 1 || rightCount - leftCount == 0))
                {
                    targetWall = new Vector3(lastLeftWall.GetPositionVector3().x + offsetSwordMan, lastLeftWall.GetPositionVector3().y, lastLeftWall.GetPositionVector3().z);
                    swordMan.Side = SideType.LeftFar;
                }
                else
                {
                    // Если есть пустое место справа, перемещаем мечника туда
                    if (leftCount > rightCount && lastRightWall != null)
                    {
                        targetWall = new Vector3(lastRightWall.GetPositionVector3().x - offsetSwordMan, lastRightWall.GetPositionVector3().y, lastRightWall.GetPositionVector3().z);
                        swordMan.Side = SideType.RightFar;
                        rightCount++;
                    }
                    // Если есть пустое место слева, перемещаем мечника туда
                    else if (rightCount > leftCount && lastLeftWall != null)
                    {
                        targetWall = new Vector3(lastLeftWall.GetPositionVector3().x + offsetSwordMan, lastLeftWall.GetPositionVector3().y, lastLeftWall.GetPositionVector3().z);
                        swordMan.Side = SideType.LeftFar;
                        leftCount++;
                    }
                    // Если нет пустых мест или их количество равно, перемещаем мечника к доступной стене
                    else if (lastLeftWall != null)
                    {
                        targetWall = new Vector3(lastLeftWall.GetPositionVector3().x + offsetSwordMan, lastLeftWall.GetPositionVector3().y, lastLeftWall.GetPositionVector3().z);
                        swordMan.Side = SideType.LeftFar;
                        leftCount++;
                    }
                    else if (lastRightWall != null)
                    {
                        targetWall = new Vector3(lastRightWall.GetPositionVector3().x - offsetSwordMan, lastRightWall.GetPositionVector3().y, lastRightWall.GetPositionVector3().z);
                        swordMan.Side = SideType.RightFar;
                        rightCount++;
                    }
                }

                if (swordMan.Side != type)
                {
                    if (type == SideType.LeftFar)
                    {
                        leftCount--;
                    }
                    else
                    {
                        rightCount--;
                    }
                }

                var _attackSwordMan = (IAttack)withType[i];

                if (lastRightWall != null && !_attackSwordMan.InAttack)
                {
                    SworManRightSide.Add(swordMan);
                    withType[i].GetRandomWalker().ChangeOnlyPositionAndStartMove(targetWall, radiusRandomWalkInNight);
                }
                else if (lastLeftWall != null && !_attackSwordMan.InAttack)
                {
                    SworManLeftSide.Add(swordMan);
                    withType[i].GetRandomWalker().ChangeOnlyPositionAndStartMove(targetWall, radiusRandomWalkInNight);
                }
            }
            _SwordManAttack.SetRightText(SworManRightSide.Count);
            _SwordManAttack.SetLeftText(SworManLeftSide.Count);
        }
        private void MoveArcher(Wall lastRightWall, Wall lastLeftWall)
        {
            ArcherLeftSide.Clear();
            ArcherRightSide.Clear();

            int rightCount = 0;
            int leftCount = 0;
            var ArcherList = _AIWarConnector.ArcherList;
            List<IWork> noneType = new List<IWork>();
            List<IWork> withType = new List<IWork>();

            foreach (var archer in ArcherList)
            {
                if (archer is IKDTree kdTreeSwordMan)
                {
                    if (kdTreeSwordMan.Side == SideType.RightFar)
                    {
                        rightCount++;
                        withType.Add(archer);
                    }
                    else if (kdTreeSwordMan.Side == SideType.LeftFar)
                    {
                        leftCount++;
                        withType.Add(archer);
                    }
                    else if (kdTreeSwordMan.Side == SideType.None)
                    {
                        noneType.Add(archer);
                    }
                }
            }

            for (int i = 0; i < noneType.Count; i++)
            {
                var archer = (IKDTree)noneType[i];
                var _attackArcher = (IAttack)archer;
                Vector3 targetWall = Vector3.zero;
                // Если есть пустое место справа, перемещаем мечника туда
                if ((rightCount <= leftCount || lastLeftWall == null) && lastRightWall != null)
                {
                    if (_gameCyrcle.ChekIfDay())
                    {
                        targetWall = new Vector3(lastRightWall.GetPositionVector3().x + offsetArcherInday, lastRightWall.GetPositionVector3().y, lastRightWall.GetPositionVector3().z);
                    }
                    else
                    {
                        targetWall = new Vector3(lastRightWall.GetPositionVector3().x - offsetArcher, lastRightWall.GetPositionVector3().y, lastRightWall.GetPositionVector3().z);
                    }

                    archer.Side = SideType.RightFar;
                    rightCount++;
                }
                // Если есть пустое место слева, перемещаем мечника туда
                else if ((leftCount <= rightCount || lastRightWall == null) && lastLeftWall != null)
                {
                    if (_gameCyrcle.ChekIfDay())
                    {
                        targetWall = new Vector3(lastLeftWall.GetPositionVector3().x - offsetArcherInday, lastLeftWall.GetPositionVector3().y, lastLeftWall.GetPositionVector3().z);
                    }
                    else
                    {
                        targetWall = new Vector3(lastLeftWall.GetPositionVector3().x + offsetArcher, lastLeftWall.GetPositionVector3().y, lastLeftWall.GetPositionVector3().z);
                    }
                    archer.Side = SideType.LeftFar;
                    leftCount++;
                }
                
                if (_gameCyrcle.ChekIfDay() && !_attackArcher.InAttack)
                {
                    if (lastRightWall != null)
                    {
                        ArcherRightSide.Add(archer);
                        noneType[i].GetRandomWalker().ChangeOnlyPositionAndStartMove(targetWall, radiusRandomWalkInDay);
                    }
                    else if(lastLeftWall != null && !_attackArcher.InAttack)
                    {
                        noneType[i].GetRandomWalker().ChangeOnlyPositionAndStartMove(targetWall, radiusRandomWalkInDay);
                    }
                } 
                else
                {
                    if (lastRightWall != null && !_attackArcher.InAttack)
                    {
                        ArcherRightSide.Add(archer);
                        noneType[i].GetRandomWalker().ChangeOnlyPositionAndStartMove(targetWall, radiusRandomWalkInNight);
                    }
                    else if (lastLeftWall != null && !_attackArcher.InAttack)
                    {
                        ArcherLeftSide.Add(archer);
                        noneType[i].GetRandomWalker().ChangeOnlyPositionAndStartMove(targetWall, radiusRandomWalkInNight);
                    }
                }
            }

            for (int i = 0; i < withType.Count; i++)
            {
                var archer = (IKDTree)withType[i];
                Vector3 targetWall = Vector3.zero;
                var type = archer.Side;

                if (archer.Side != SideType.None && archer.Side == SideType.RightFar && lastRightWall != null && (Mathf.Abs(leftCount - rightCount) == 1 || leftCount - rightCount == 0))
                {
                    if (_gameCyrcle.ChekIfDay())
                    {
                        targetWall = new Vector3(lastRightWall.GetPositionVector3().x + offsetArcherInday, lastRightWall.GetPositionVector3().y, lastRightWall.GetPositionVector3().z);
                    }
                    else
                    {
                        targetWall = new Vector3(lastRightWall.GetPositionVector3().x - offsetArcher, lastRightWall.GetPositionVector3().y, lastRightWall.GetPositionVector3().z);
                    }

                    archer.Side = SideType.RightFar;
                }
                else if (archer.Side != SideType.None && archer.Side == SideType.LeftFar && lastLeftWall != null && (Mathf.Abs(rightCount - leftCount) == 1 || rightCount - leftCount == 0))
                {
                    if (_gameCyrcle.ChekIfDay())
                    {
                        targetWall = new Vector3(lastLeftWall.GetPositionVector3().x - offsetArcherInday, lastLeftWall.GetPositionVector3().y, lastLeftWall.GetPositionVector3().z);
                    }
                    else
                    {
                        targetWall = new Vector3(lastLeftWall.GetPositionVector3().x + offsetArcher, lastLeftWall.GetPositionVector3().y, lastLeftWall.GetPositionVector3().z);
                    }

                    archer.Side = SideType.LeftFar;
                }
                else
                {
                    // Если есть пустое место справа, перемещаем мечника туда
                    if (leftCount > rightCount && lastRightWall != null)
                    {
                        if (_gameCyrcle.ChekIfDay())
                        {
                            targetWall = new Vector3(lastRightWall.GetPositionVector3().x + offsetArcherInday, lastRightWall.GetPositionVector3().y, lastRightWall.GetPositionVector3().z);
                        }
                        else
                        {
                            targetWall = new Vector3(lastRightWall.GetPositionVector3().x - offsetArcher, lastRightWall.GetPositionVector3().y, lastRightWall.GetPositionVector3().z);
                        }
                        archer.Side = SideType.RightFar;
                        rightCount++;
                    }
                    // Если есть пустое место слева, перемещаем мечника туда
                    else if (rightCount > leftCount && lastLeftWall != null)
                    {
                        if (_gameCyrcle.ChekIfDay())
                        {
                            targetWall = new Vector3(lastLeftWall.GetPositionVector3().x - offsetArcherInday, lastLeftWall.GetPositionVector3().y, lastLeftWall.GetPositionVector3().z);
                        }
                        else
                        {
                            targetWall = new Vector3(lastLeftWall.GetPositionVector3().x + offsetArcher, lastLeftWall.GetPositionVector3().y, lastLeftWall.GetPositionVector3().z);
                        }
                        archer.Side = SideType.LeftFar;
                        leftCount++;
                    }
                    // Если нет пустых мест или их количество равно, перемещаем мечника к доступной стене
                    else if (lastLeftWall != null)
                    {
                        if (_gameCyrcle.ChekIfDay())
                        {
                            targetWall = new Vector3(lastLeftWall.GetPositionVector3().x - offsetArcherInday, lastLeftWall.GetPositionVector3().y, lastLeftWall.GetPositionVector3().z);
                        }
                        else
                        {
                            targetWall = new Vector3(lastLeftWall.GetPositionVector3().x + offsetArcher, lastLeftWall.GetPositionVector3().y, lastLeftWall.GetPositionVector3().z);
                        }
                        archer.Side = SideType.LeftFar;
                        leftCount++;
                    }
                    else if (lastRightWall != null)
                    {
                        if (_gameCyrcle.ChekIfDay())
                        {
                            targetWall = new Vector3(lastRightWall.GetPositionVector3().x + offsetArcherInday, lastRightWall.GetPositionVector3().y, lastRightWall.GetPositionVector3().z);
                        }
                        else
                        {
                            targetWall = new Vector3(lastRightWall.GetPositionVector3().x - offsetArcher, lastRightWall.GetPositionVector3().y, lastRightWall.GetPositionVector3().z);
                        }
                        archer.Side = SideType.RightFar;
                        rightCount++;
                    }
                }

                if(archer.Side != type)
                {
                    if(type == SideType.LeftFar)
                    {
                        leftCount--;
                    }
                    else
                    {
                        rightCount--;
                    }
                }

                var _attackArcher = (IAttack)archer;

                if (lastRightWall != null && !_attackArcher.InAttack)
                {
                    ArcherRightSide.Add(archer);
                    withType[i].GetRandomWalker().ChangeOnlyPositionAndStartMove(targetWall, radiusRandomWalkInNight);
                }
                else if (lastLeftWall != null && !_attackArcher.InAttack)
                {
                    ArcherLeftSide.Add(archer);
                    withType[i].GetRandomWalker().ChangeOnlyPositionAndStartMove(targetWall, radiusRandomWalkInNight);
                }
            }
           
            _ArcherAttack.SetRightText(ArcherRightSide.Count);
            _ArcherAttack.SetLeftText(ArcherLeftSide.Count);
        }
        public void RandomAnimalPosition()
        {
            var AnimalList = _AIWarConnector.AnimalList;

            List<IAnimal> AnimalListSorted = new List<IAnimal>(AnimalList.Count);

            Vector3 wallPos = Vector3.zero;

            if (lastRightWall != null)
            {
                wallPos = lastRightWall.GetPositionVector3();
            }
            else if (lastLeftWall != null)
            {
                wallPos = lastLeftWall.GetPositionVector3();
            }
            else
            {
                return;
            }

            // Sort animals by distance to the wall position
            for (int i = 0; i < AnimalList.Count; i++)
            {
                IAnimal closestAnimal = null;
                float closestDistance = Mathf.Infinity;

                foreach (IAnimal animal in AnimalList)
                {
                    if (AnimalListSorted.Contains(animal))
                    {
                        continue;
                    }

                    float distance = Vector2.SqrMagnitude(new Vector2(animal.GetAiComponent().GetTransform().position.x, animal.GetAiComponent().GetTransform().position.z) - new Vector2(wallPos.x, wallPos.z));

                    if (distance < closestDistance)
                    {
                        closestAnimal = animal;
                        closestDistance = distance;
                    }
                }

                AnimalListSorted.Add(closestAnimal);
            }

            Vector3 targetWall = Vector3.zero;
            // Перемещаем Archer по последовательности стен
            if (lastRightWall != null)
            {
                for (int i = 0; i < AnimalListSorted.Count / 2; i++)
                {
                    targetWall = new Vector3(lastRightWall.GetPositionVector3().x + _wallOffset, lastRightWall.GetPositionVector3().y, lastRightWall.GetPositionVector3().z);


                    if (targetWall != Vector3.zero)
                    {
                        AnimalListSorted[i].ChangePositionForRandomWallk(targetWall, radiusRandomWalkAnimal);
                    }
                }
            }
            if (lastLeftWall != null)
            {
                int sort = 0;
                int count = 0;

                if (lastRightWall != null)
                {
                    sort = AnimalListSorted.Count / 2;
                    count = AnimalListSorted.Count;
                }
                else
                {
                    count = AnimalListSorted.Count / 2;
                }

                for (int i = sort; i < count; i++)
                {

                    targetWall = new Vector3(lastLeftWall.GetPositionVector3().x - _wallOffset, lastLeftWall.GetPositionVector3().y, lastLeftWall.GetPositionVector3().z);

                    if (targetWall != Vector3.zero)
                    {
                        AnimalListSorted[i].ChangePositionForRandomWallk(targetWall, radiusRandomWalkAnimal);
                    }
                }
            }
        }
    }

}
