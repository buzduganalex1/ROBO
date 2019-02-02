using FileHelpers;

namespace Robo
{
    [DelimitedRecord(",")]
    public class King
    {
        public string Ruler { get; set; }

        public string Reigned { get; set; }

        public string Comment { get; set; }
    }
}