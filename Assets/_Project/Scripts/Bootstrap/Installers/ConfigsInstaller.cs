using App.Bootstrap.Configs;
using UnityEngine;
using Zenject;

namespace App.Bootstrap.Installers
{
    
    [CreateAssetMenu(fileName = "ConfigsInstaller", menuName = "App/Configs Installer")]
    public sealed class ConfigsInstaller : ScriptableObjectInstaller<ConfigsInstaller>
    {
        [SerializeField] private NetworkConfig _network;
        [SerializeField] private ClickerConfig _clicker;
        [SerializeField] private WeatherConfig _weather;
        [SerializeField] private DogsConfig _dogs;

        public override void InstallBindings()
        {
            Container.BindInstance(_network.ToSettings());
            Container.BindInstance(_clicker.ToSettings());
            Container.BindInstance(_weather.ToSettings());
            Container.BindInstance(_weather.ToApiSettings());
            Container.BindInstance(_dogs.ToSettings());
            Container.BindInstance(_dogs.ToApiSettings());
        }
    }
}
