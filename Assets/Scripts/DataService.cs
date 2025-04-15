using System.Collections.Generic;

namespace Rts
{
    public class DataService
    {
        private readonly Dictionary<ResourceType, int> _resources = new Dictionary<ResourceType, int>();

        public void Initialize()
        {
            _resources.Add(ResourceType.Gold, default);
            _resources.Add(ResourceType.Silver, default);
            _resources.Add(ResourceType.Bronze, default);
            _resources.Add(ResourceType.Wood, default);
            _resources.Add(ResourceType.Diamond, default);
        }

        public void AddResource(ResourceType resourceType, int amount) => _resources[resourceType] += amount;
        public void RemoveResource(ResourceType resourceType, int amount) => _resources[resourceType] -= amount;
        public int GetResource(ResourceType resourceType) => _resources[resourceType];
    }
}