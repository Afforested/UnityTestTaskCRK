using System.Threading;

namespace App.Presentation.Common
{
    internal static class CancellationSourceUtility
    {
        public static void CancelAndDispose(ref CancellationTokenSource source)
        {
            if (source == null)
                return;

            var current = source;
            source = null;
            current.Cancel();
            current.Dispose();
        }
    }
}
