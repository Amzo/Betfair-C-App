using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Security.Authentication;
using Newtonsoft.Json;


namespace Betfair.ESAClient.Auth
{
    internal class AppKeyAndSessionProvider
    {
        private string _key;
        private string _host;
        private string _password;
        private string _username;

        private AppKeyAndSession _session;

        public const string HOST_COM = "identitysso.netfair.com";

        public string Appkey
        {
            get { return _key; }
        }

        public TimeSpan ExpireSessionTime { get; set; }
        public TimeSpan Timeout { get; set; }

        public AppKeyAndSessionProvider(string host, string appKey, string username, string password)
        {
            _host = host;
            _key = appKey;
            _username = username;
            _password = password;
            Timeout = TimeSpan.FromSeconds(20);
            ExpireSessionTime = TimeSpan.FromHours(3);
        }

        public async Task<AppKeyAndSession> GetSession()
        {
            if (_session != null )
            {
                if ((_session.CreationTime + ExpireSessionTime) > DateTime.UtcNow)
                {
                    Trace.TraceInformation("Login: Reusing unexpired Session");
                    return _session;
                }
                else
                    Trace.TraceInformation("Login: Session  Expired");
            }

            SessionDetails sessionDetails;

            try
            {
                string url = string.Format("https://{0}/api/login?username={1}&password={2}", _host, _username, _password);

                using (HttpClient client = new HttpClient())
                {
                    client.Timeout = Timeout;
                    HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, url);
                    request.Headers.Add("X-Application", _key);
                    request.Headers.Accept.ParseAdd("application/json");

                    HttpResponseMessage response = await client.SendAsync(request);

                    if (response.IsSuccessStatusCode)
                    {
                        string responseBody = await response.Content.ReadAsStringAsync();
                        Trace.TraceInformation("{0}: Response: {1}", _host, responseBody);
                        sessionDetails = JsonConvert.DeserializeObject<SessionDetails>(responseBody);
                    }
                    else
                    {
                        Trace.TraceInformation("Received HTTP {0}", response.StatusCode);
                        throw new IOException($"SSO Authentication - call failed, status code: {response.StatusCode}");
                    }

                }
            }
            catch (Exception e)
            {
                throw new IOException("SSO Authentication - call failed:", e);
            }

            if (sessionDetails != null && "SUCCESS".Equals(sessionDetails.status))
                _session = new AppKeyAndSession(_key, sessionDetails.token);
            else
                throw new InvalidCredentialException("SSO Authentication - response is fail: " + sessionDetails?.error);

            return _session;
        }
    }
}

class SessionDetails
{
    public string token;
    public string product;
    public string status;
    public string error;
}
