using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;

namespace RemindONServer.Auth
{
    public class AuthenticatedDevice : IIdentity
    {
        public AuthenticatedDevice(string authenticationType, bool isAuthenticated, string name)
        {
            AuthenticationType = authenticationType;
            IsAuthenticated = isAuthenticated;
            Name = name;
        }

        public string AuthenticationType { get; }

        public bool IsAuthenticated { get; }

        public string Name { get; }
    }
}
