
using GameInit.AI;

namespace GameInit.Building
{
    public interface IBuilding
    {
       public void Build();
       public void SetBuilder(IWork worker);
    }
}

