using GameInit.AI;
using UnityEngine;

public interface IAnimal
{
    public bool Move(Vector3 position);
    public AIComponent GetAiComponent();
    public bool InMove { get; set; }
    public void ChangePositionForRandomWallk(Vector3 pos, float radius);
}
