using UnityEngine;

namespace Rts
{
    public struct ReceivedResource
    {
        public ResourceType ResourceType;
        public int Amount;

        public ReceivedResource(ResourceType resourceType, int amount)
        {
            ResourceType = resourceType;
            Amount = amount;
        }
    }
}