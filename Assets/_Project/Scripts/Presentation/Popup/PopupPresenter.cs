using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UniRx;
using Zenject;

namespace App.Presentation.Popup
{
    public sealed class PopupPresenter : IPopupService, IInitializable, IDisposable
    {
        private readonly IPopupView _view;
        private readonly CompositeDisposable _disposables = new CompositeDisposable();

        private UniTaskCompletionSource _current;

        public PopupPresenter(IPopupView view)
        {
            _view = view ?? throw new ArgumentNullException(nameof(view));
        }

        public void Initialize() => _view.CloseClicked.Subscribe(_ => Close()).AddTo(_disposables);

        public async UniTask ShowAsync(string title, string message, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            _current?.TrySetResult();
            var completion = new UniTaskCompletionSource();
            _current = completion;
            _view.Show(title, message);

            using (cancellationToken.Register(() => Cancel(completion)))
            {
                await completion.Task;
            }
        }

        public void Dispose()
        {
            _disposables.Dispose();
            _current?.TrySetCanceled();
            _current = null;
        }

        private void Close()
        {
            var completion = _current;
            if (completion == null)
                return;

            _current = null;
            _view.Hide();
            completion.TrySetResult();
        }

        private void Cancel(UniTaskCompletionSource completion)
        {
            if (_current == completion)
            {
                _current = null;
                _view.Hide();
            }

            completion.TrySetCanceled();
        }
    }
}
