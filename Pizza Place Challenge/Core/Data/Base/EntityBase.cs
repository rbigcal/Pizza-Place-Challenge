using Pizza_Place_Challenge.Core.Helper;
using System.Text.Json.Serialization;

namespace Pizza_Place_Challenge.Core.Data.Base
{
    public abstract class EntityBase
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = IDHelper.GenerateNewID();
    }
}
