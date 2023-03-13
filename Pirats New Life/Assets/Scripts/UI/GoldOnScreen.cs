using System.Collections;
using System.Collections.Generic;
using GameInit.Component;
using UnityEngine;

public class GoldOnScreen
{
    private GoldOnScreenComponent _goldOnScreenComponent;
    private ResourceManager _resources;

    public GoldOnScreen(GoldOnScreenComponent goldOnScreenComponent, ResourceManager resources)
    {
        _goldOnScreenComponent = goldOnScreenComponent;
        _resources = resources;
        resources.OnResourceChange += ResourceChange;
    }

    private void ResourceChange()
    {
        _goldOnScreenComponent.SetText(_resources.GetResource(ResourceType.Gold).ToString());
    }

}
