using App.Infrastructure.Network;
using UnityEngine;

namespace App.Bootstrap.Configs
{
    [CreateAssetMenu(fileName = "NetworkConfig", menuName = "App/Configs/Network")]
    public sealed class NetworkConfig : ScriptableObject
    {
        [SerializeField, Min(1)] private int _timeoutSeconds = 15;
        [Tooltip("api.weather.gov asks clients to identify themselves with a User-Agent.")]
        [SerializeField] private string _userAgent = "UnityTestTaskCRK/1.0 (Unity test task)";
        [Tooltip("Log every request queue step to the console.")]
        [SerializeField] private bool _logRequests = true;

        public NetworkSettings ToSettings() => new NetworkSettings(_timeoutSeconds, _userAgent, _logRequests);
    }
}
