namespace ShareDrawing.HttpClient.Http
{
    public class TokenBox
    {
        public static TokenBox Default { get; } = new();
        public        string   Token   { get; set; }
    }
}