﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Betfair.ESAClient.Auth
{
    internal class AppKeyAndSession
    {
        public string AppKey { get; private set; }
        public DateTime CreationTime  {get; private set;}
        public string Session { get; private set; }
        AppKeyAndSession(string appKey, string sessionKey)
        {
            AppKey = appKey;
            Session = sessionKey;
            CreationTime = DateTime.Now;

        }
    }
}