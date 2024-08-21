namespace Opera
{
    public interface IJsonUtility
    {
        // Implementation requirements:
        // - If the JSON representation is missing any fields, they will be given their default values
        // - If the input is null or empty, FromJson returns null
        // 
        // In the future (when all the engines are ready) this function may use the new C# json utility.
        // After this, the json functionality may move from engine-specific folder into the common code.
        TResult FromJson<TResult>(string json);
    }
}
