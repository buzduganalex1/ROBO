namespace Robo
{
    public static class RoboEnum
    {
        private const string baseURI = "http://www.semanticweb.org/ontology/robo#";

        public static string GetRoboUri(string text)
        {
            return $"{baseURI}{text}";
        }
    }
}