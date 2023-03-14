using System;
using System.Collections.Generic;
using GameInit.Builders;

[Serializable]
public class ResourceManager
{
    private Dictionary<ResourceType, int> _resources;

    public Action OnResourceChange { get; set; }
    
    public ResourceManager()
    {
        _resources = new Dictionary<ResourceType, int>();
        foreach (var resType in Enum.GetNames(typeof(ResourceType)))
        {
            _resources.Add((ResourceType)Enum.Parse(typeof(ResourceType), resType), 0);
        }
    }

    public int GetResource(ResourceType type)
    {
        if (_resources.ContainsKey(type))
        {
            return _resources.GetValueOrDefault(type);
        }
        return 0;
    }

    public void SetResource(ResourceType type, int count)
    {
        if (_resources.ContainsKey(type))
        {
            _resources[type] += count;
            if (_resources[type] <= 0)
            {
                _resources[type] = 0;
            }
        }

        OnResourceChange.Invoke();
        System.Console.WriteLine("resourse " + _resources[type]);
    }
}