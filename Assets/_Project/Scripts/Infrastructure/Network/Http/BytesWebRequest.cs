using UnityEngine.Networking;

namespace App.Infrastructure.Network.Http
{
    public sealed class BytesWebRequest : WebRequest<byte[]>
    {
        public BytesWebRequest(string url, NetworkSettings settings, string name = null)
            : base(url, settings, name)
        {
        }

        protected override byte[] ParseResponse(DownloadHandler downloadHandler)
        {
            var data = downloadHandler.data;
            if (data == null || data.Length == 0)
                throw new WebRequestException(Url, "the response body is empty.");

            return data;
        }
    }
}
