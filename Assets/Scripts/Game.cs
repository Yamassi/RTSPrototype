using UnityEngine;

namespace Rts
{
    public class Game : MonoBehaviour
    {
        [SerializeField] private Player _player;
        [SerializeField] private Building[] _buildings;
        [SerializeField] private PopUp _popUp;
        [SerializeField] private Settings _settings;
        [SerializeField] private AudioService _audioService;
        [SerializeField] private InputController _inputController;
        private DataService _dataService;

        private void Awake()
        {
            _inputController.Initialize(_player);

            _dataService = new DataService();
            _dataService.Initialize();

            _audioService.Initialize();

            _settings.Initialize(_audioService);

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