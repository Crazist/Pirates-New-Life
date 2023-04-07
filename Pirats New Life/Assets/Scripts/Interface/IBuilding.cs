
using GameInit.AI;
using UnityEngine;

namespace GameInit.Building
{
    public interface IBuilding
    {
       public void Build();
       public void SetBuilder(IWork worker);
    }
}

