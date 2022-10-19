using Newtonsoft.Json;

namespace SystemModule.Structures;

public class ListedModule {
    [JsonProperty("name")] public string Name;
}

public class ListModulesResult {
    [JsonProperty("list")] public ListedModule[] List;
}