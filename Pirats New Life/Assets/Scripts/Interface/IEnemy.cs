using GameInit.AI;
using System;
using UnityEngine;

public interface IEnemy
{
    public bool InMove { get; set; }
    public bool RefreshSkill { get; set; }
    public float DistanceForStartSpell { get; }
    public void Move(Vector3 position);
    public void MoveToBase(Vector3 position, Action action);
    public void UseSpell(Vector3 position, bool runAway);
    public void StopSpell();
    public void Disable();
    public AIComponent GetAiComponent();
}
