using System.Threading;
using Cysharp.Threading.Tasks;

namespace App.Presentation.Popup
{
    public interface IPopupService
    {
        UniTask ShowAsync(string title, string message, CancellationToken cancellationToken = default);
    }
}
