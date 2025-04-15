using System.Collections.Generic;
using UnityEngine;

namespace Rts
{
    public class DataService
    {
        private readonly Dictionary<ResourceType, int> _resources = new Dictionary<ResourceType, int>();

        public void Initialize()
        {
            _resources.Add(ResourceType.Gold, 0);
            _resources.Add(ResourceType.Silver, 0);
            _resources.Add(ResourceType.Bronze, 0);
            _resources.Add(ResourceType.Wood, 0);
            _resources.Add(ResourceType.Diamond, 0);
        }

        public void AddResource(ResourceType resourceType, int amount) => _resources[resourceType] += amount;
        public void RemoveResource(ResourceType resourceType, int amount) => _resources[resourceType] -= amount;
        public int GetResource(ResourceType resourceType) => _resources[resourceType];
        
    }
    public class Game : MonoBehaviour
    {
        [SerializeField] private Player _player;
        [SerializeField] private Building[] _buildings;
        [SerializeField] private PopUp _popUp;
        private InputController _inputController;
        private DataService _dataService;

        private void Awake()
        {
            _inputController = new InputController(_player);
            _inputController.Initialize();
            
            _dataService = new DataService();
            _dataService.Initialize();

            foreach (var building in _buildings)
            {
                building.Initialize(GetResource);
            }
        }

        private void GetResource(ReceivedResource receivedResource)
        {
            _dataService.AddResource(receivedResource.ResourceType, receivedResource.Amount);
            
            _popUp.Show(
                receivedResource.ResourceType.ToString(),
                _dataService.GetResource(receivedResource.ResourceType).ToString());
        }
    }
}