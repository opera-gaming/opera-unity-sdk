using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

namespace Opera
{
    [System.Serializable]
    public abstract class GmxApiResult<TData>
    {
        public TData Data => data;
        [Preserve] public TData data;
        [Preserve] public GmxApiError[] errors;
    }

    [System.Serializable]
    public sealed class GmxApiError
    {
        [Preserve] public string code;
    }
}
