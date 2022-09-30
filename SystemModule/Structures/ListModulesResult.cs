using Newtonsoft.Json;

namespace SystemModule.Structures;

public class ListedModule {
    [JsonProperty("id")] public int Id;
    [JsonProperty("name")] public string Name;
}

public class ListModulesResult {
    [JsonProperty("list")] public ListedModule[] List;
}