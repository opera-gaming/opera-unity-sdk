using UnityEngine;

namespace Opera
{
    public sealed class JsonUtilityAdapter : IJsonUtility
    {
        public TResult FromJson<TResult>(string json) => JsonUtility.FromJson<TResult>(json);
    }
}
