﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Betfair.ESAClient.Auth
{
    public class AppKeyAndSession
    {
        public string AppKey { get; private set; }
        public DateTime CreationTime  {get; private set;}
        public string Session { get; private set; }

        public AppKeyAndSession(string appKey, string sessionKey)
        {
            AppKey = appKey;
            Session = sessionKey;
            CreationTime = DateTime.Now;

        }
    }
}
