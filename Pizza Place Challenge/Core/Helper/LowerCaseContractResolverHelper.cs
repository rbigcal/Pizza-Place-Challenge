using Newtonsoft.Json.Serialization;

namespace Pizza_Place_Challenge.Core.Helper {
    public class LowerCaseContractResolverHelper : DefaultContractResolver {
        protected override string ResolvePropertyName(string propertyName) {
            return propertyName.ToLower();
        }
    }
}
