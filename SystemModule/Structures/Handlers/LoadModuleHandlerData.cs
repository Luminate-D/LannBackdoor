using Newtonsoft.Json;

namespace SystemModule.Structures; 

public class LoadModuleHandlerData {
    [JsonProperty("raw")]
    public string Raw { get; set; }
    
    [JsonProperty("id")]
    public int Id { get; set; }
}