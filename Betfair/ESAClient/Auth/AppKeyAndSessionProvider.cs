using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Authentication;
using Newtonsoft.Json;


namespace Betfair.ESAClient.Auth
{
    public class AppKeyAndSessionProvider
    {
        private string _key;
        private string _host;
        private string _password;
        private string _username;

        private AppKeyAndSession? _session;

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

        public AppKeyAndSession GetSession()
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
                    HttpWebRequest loginRequest = (HttpWebRequest)WebRequest.Create(url);
                    loginRequest.Headers.Add("X-Application", _key);
                    loginRequest.Accept = "application/json";
                    loginRequest.Method = "POST";
                    loginRequest.Timeout = (int)Timeout.TotalMilliseconds;
                    WebResponse thePage = loginRequest.GetResponse();
                    using (StreamReader reader = new StreamReader(thePage.GetResponseStream()))
                    {
                        string response = reader.ReadToEnd();
                        Trace.TraceInformation("{0}: Response: {1}", _host, response);
                        sessionDetails = JsonConvert.DeserializeObject<SessionDetails>(response);
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

        public void ExpireTokenNow()
        {
            Trace.TraceInformation("SSO Login - expiring session token now");
            _session = null;
        }
    }
}

class SessionDetails
{
    public string? token;
    public string? product;
    public string? status;
    public string? error;
}
