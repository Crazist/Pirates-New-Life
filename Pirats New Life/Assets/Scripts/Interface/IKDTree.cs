
using UnityEngine;

public interface IKDTree
{
    public int HP { get; set; }
    public EntityType Type { get; }
    public SideType Side { get; set; }
    public Vector2 GetPositionVector2();
    public bool CheckIfEnemy();
    public bool CheckIfCanDamage();
    public void Attack();
    public void GetDamage(int damage);
    public int CountOFDamage();
}
