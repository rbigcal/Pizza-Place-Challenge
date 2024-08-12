using Pizza_Place_Challenge.Core.Helper;

namespace Pizza_Place_Challenge.Core.Data.Base
{
    public abstract class EntityBase
    {
        public string Id { get; set; } = IDHelper.GenerateNewID();
    }
}
