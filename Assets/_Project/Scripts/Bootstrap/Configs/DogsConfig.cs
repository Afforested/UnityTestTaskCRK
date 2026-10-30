using App.Application.Dogs;
using App.Infrastructure.Dogs;
using UnityEngine;

namespace App.Bootstrap.Configs
{
    [CreateAssetMenu(fileName = "DogsConfig", menuName = "App/Configs/Dogs")]
    public sealed class DogsConfig : ScriptableObject
    {
        [SerializeField] private string _baseUrl = "https://dogapi.dog/api/v2";
        [SerializeField, Min(1)] private int _breedsCount = 10;

        public DogsSettings ToSettings() => new DogsSettings(_breedsCount);

        public DogApiSettings ToApiSettings() => new DogApiSettings(_baseUrl);
    }
}
