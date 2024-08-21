using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Opera
{
    public interface IServerSettings
    {
        string ServerUrl { get; }
        string OauthServer { get; }
    }
}
