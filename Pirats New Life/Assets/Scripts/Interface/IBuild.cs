using GameInit.Building;
using System;
using UnityEngine;

public interface IBuild
{
    public bool Build(Vector3 position, Action action, IBuilding _building);
}
