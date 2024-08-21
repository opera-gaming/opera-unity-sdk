using System;

namespace Opera
{
    public interface ICachedData<TData>
    {
        TData Data { get; }
    }

    public interface IRefetchable
    {
        bool RefetchData();
    }

    public class CachedCloudData<TReqestResult, TResponseData, TOutputData> : ICachedData<TOutputData>, IRefetchable
        where TReqestResult : GmxApiResult<TResponseData>
    {
        private string serverURL => serverSettings.ServerUrl;

        private readonly IServerSettings serverSettings;
        private readonly IOperaGxGetRequest operaGxGetRequest;
        private readonly string requestPath;
        private readonly OperaAuthorization operaAuthorization;
        private readonly ISessionProperty<TOutputData> propertyInSessionStorage;
        private readonly Func<TResponseData, TOutputData> transformData;

        public TOutputData Data => propertyInSessionStorage.Value;

        public CachedCloudData(IOperaGxGetRequest operaGxGetRequest, OperaAuthorization operaAuthorization, IServerSettings serverSettings, string requestPath, ISessionProperty<TOutputData> propertyInSessionStorage, Func<TResponseData, TOutputData> transformData)
        {
            this.operaGxGetRequest = operaGxGetRequest ?? throw new ArgumentNullException(nameof(operaGxGetRequest));
            this.operaAuthorization = operaAuthorization ?? throw new ArgumentNullException(nameof(operaAuthorization));
            this.serverSettings = serverSettings ?? throw new ArgumentNullException(nameof(serverSettings));
            this.requestPath = requestPath ?? throw new ArgumentNullException(nameof(requestPath));
            this.propertyInSessionStorage = propertyInSessionStorage ?? throw new ArgumentNullException(nameof(propertyInSessionStorage));
            this.transformData = transformData ?? throw new ArgumentNullException(nameof(transformData));
        }

        public bool RefetchData()
        {
            var (success, newValue) = operaGxGetRequest.Get<TReqestResult, TResponseData>(operaAuthorization.OAUTH2_access_token, serverURL, requestPath);

            if (success)
            {
                propertyInSessionStorage.Value = transformData(newValue);
            }

            return success;
        }
    }
}
