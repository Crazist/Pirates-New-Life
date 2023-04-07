using System;
using UnityEngine;

public interface IEnemy
{
    public bool InMove { get; set; }
    public void Move(Vector3 position, Action action);
}
