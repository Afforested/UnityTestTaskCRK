using Newtonsoft.Json;
using UnityEngine.Networking;

namespace App.Infrastructure.Network.Http
{
    public sealed class JsonWebRequest<T> : WebRequest<T>
    {
        public JsonWebRequest(string url, NetworkSettings settings, string name = null)
            : base(url, settings, name)
        {
        }

        protected override T ParseResponse(DownloadHandler downloadHandler)
        {
            T result;
            try
            {
                result = JsonConvert.DeserializeObject<T>(downloadHandler.text);
            }
            catch (JsonException exception)
            {
                throw new WebRequestException(Url, "the response is not valid JSON.", exception);
            }

            if (result == null)
                throw new WebRequestException(Url, "the response body is empty.");

            return result;
        }
    }
}
