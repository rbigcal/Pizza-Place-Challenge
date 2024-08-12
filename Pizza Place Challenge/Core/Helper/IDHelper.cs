namespace Pizza_Place_Challenge.Core.Helper
{
    public static class IDHelper
    {
        public static string GenerateNewID() => Guid.NewGuid().ToString("n");
    }
}
