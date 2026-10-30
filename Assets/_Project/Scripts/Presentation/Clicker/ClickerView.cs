using System;
using App.Presentation.Common;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using Random = UnityEngine.Random;

namespace App.Presentation.Clicker
{
    public sealed class ClickerView : ScreenView, IClickerView
    {
        [Header("Button")]
        [SerializeField] private ClickButtonView _button;
        [SerializeField] private Image _autoCollectRing;

        [Header("Counters")]
        [SerializeField] private CounterView _balanceCounter;
        [SerializeField] private TMP_Text _incomeHint;
        [SerializeField] private CounterView _energyCounter;
        [Tooltip("The maximum after the current energy is drawn smaller and in this color.")]
        [SerializeField, Range(0.2f, 1f)] private float _energyMaxScale = 0.55f;
        [SerializeField] private Color _energyMaxColor = Color.gray;
        [SerializeField] private Image _energyBar;
        [SerializeField] private TMP_Text _energyHint;

        [Header("Feedback")]
        [SerializeField] private ParticleSystem _burstParticles;
        [SerializeField, Min(1)] private int _particlesPerCollect = 16;
        [Tooltip("Button size the burst is tuned for: on a smaller button (e.g. landscape) the burst shrinks with it.")]
        [SerializeField, Min(1f)] private float _burstButtonSize = 816f;
        [SerializeField] private RectTransform _flyingCurrencyOrigin;
        [SerializeField] private RectTransform _flyingCurrencyTarget;
        [SerializeField] private AudioSource _audioSource;
        [SerializeField] private AudioClip _collectSound;
        [SerializeField] private AudioClip _notEnoughEnergySound;
        [SerializeField] private Vector2 _collectPitchRange = new Vector2(0.94f, 1.12f);

        private FlyingCurrencyView.Pool _flyingCurrencyPool;

        [Inject]
        public void Construct(FlyingCurrencyView.Pool flyingCurrencyPool) => _flyingCurrencyPool = flyingCurrencyPool;

        public IObservable<Unit> ButtonClicked => _button.Clicked;

        public void SetBalance(string text) => _balanceCounter.SetValue(text);

        public void SetIncomeHint(string text) => _incomeHint.text = text;

        public void SetEnergy(string current, string max, float fill)
        {
            var maxColor = ColorUtility.ToHtmlStringRGBA(_energyMaxColor);
            _energyCounter.SetValue($"{current}<size={_energyMaxScale * 100f:0}%><color=#{maxColor}>/{max}</color></size>");
            _energyBar.fillAmount = fill;
        }

        public void SetEnergyHint(string text) => _energyHint.text = text;

        public void SetAutoCollectProgress(float progress) => _autoCollectRing.fillAmount = progress;

        public void PlayCollect(string rewardText)
        {
            _button.PlayPress();
            EmitBurst();
            LaunchFlyingCurrency(rewardText);
            PlaySound(_collectSound, Random.Range(_collectPitchRange.x, _collectPitchRange.y));
        }

        public void PlayNotEnoughEnergy()
        {
            _button.PlayDenied();
            _energyCounter.PlayDenied();
            PlaySound(_notEnoughEnergySound, 1f);
        }

        private void EmitBurst()
        {
            var scale = ((RectTransform)_button.transform).rect.width / _burstButtonSize;
            _burstParticles.transform.localScale = new Vector3(scale, scale, 1f);
            _burstParticles.Emit(_particlesPerCollect);
        }

        private void LaunchFlyingCurrency(string text)
        {
            var item = _flyingCurrencyPool.Spawn();
            item.Fly(text, _flyingCurrencyOrigin.position, _flyingCurrencyTarget.position, () => OnCurrencyArrived(item));
        }

        private void OnCurrencyArrived(FlyingCurrencyView item)
        {
            _flyingCurrencyPool.Despawn(item);
            if (isActiveAndEnabled)
                _balanceCounter.Punch();
        }

        private void PlaySound(AudioClip clip, float pitch)
        {
            if (clip == null)
                return;

            _audioSource.pitch = pitch;
            _audioSource.PlayOneShot(clip);
        }
    }
}
