using Newtonsoft.Json;

namespace Alexandra.Common.Types
{
    public class Hex
    {
        [JsonProperty("value")]
        public string Value { get; set; }

        [JsonProperty("clean")]
        public string Clean { get; set; }
    }

    public class FractionRgb
    {
        [JsonProperty("r")]
        public double R { get; set; }
        
        [JsonProperty("g")]
        public double G { get; set; }
        
        [JsonProperty("b")]
        public double B { get; set; }
    }

    public class FractionHsl
    {
        [JsonProperty("h")]
        public double H { get; set; }

        [JsonProperty("s")]
        public double S { get; set; }

        [JsonProperty("l")]
        public double L { get; set; }
    }

    public class FractionHsv
    {
        [JsonProperty("h")]
        public double H { get; set; }

        [JsonProperty("s")]
        public double S { get; set; }
        
        [JsonProperty("v")]
        public double V { get; set; }
    }

    public class FractionCmyk
    {
        [JsonProperty("c")]
        public double C { get; set; }

        [JsonProperty("m")]
        public double M { get; set; }

        [JsonProperty("y")]
        public double Y { get; set; }

        [JsonProperty("k")]
        public double K { get; set; }
    }

    public class Rgb
    {
        [JsonProperty("fraction")]
        public FractionRgb Fraction { get; set; }

        [JsonProperty("r")]
        public int R { get; set; }

        [JsonProperty("g")]
        public int G { get; set; }

        [JsonProperty("b")]
        public int B { get; set; }

        [JsonProperty("value")]
        public string Value { get; set; }
    }

    public class Hsl
    {
        [JsonProperty("fraction")]
        public FractionHsl Fraction { get; set; }

        [JsonProperty("h")]
        public int H { get; set; }

        [JsonProperty("s")]
        public int S { get; set; }

        [JsonProperty("l")]
        public int L { get; set; }

        [JsonProperty("value")]
        public string Value { get; set; }
    }

    public class Hsv
    {
        [JsonProperty("fraction")]
        public FractionHsv Fraction { get; set; }

        [JsonProperty("h")]
        public int H { get; set; }

        [JsonProperty("s")]
        public int S { get; set; }

        [JsonProperty("value")]
        public string Value { get; set; }

        [JsonProperty("v")]
        public int V { get; set; }
    }

    public class Name
    {
        [JsonProperty("value")]
        public string Value { get; set; }

        [JsonProperty("closest_named_hex")]
        public string ClosestNamedHex { get; set; }

        [JsonProperty("exact_match_name")]
        public bool ExactMatchName { get; set; }

        [JsonProperty("distance")]
        public int Distance { get; set; }
    }

    public class Cmyk
    {
        [JsonProperty("fraction")]
        public FractionCmyk Fraction { get; set; }

        [JsonProperty("value")]
        public string Value { get; set; }

        [JsonProperty("c")]
        public int C { get; set; }

        [JsonProperty("m")]
        public int M { get; set; }

        [JsonProperty("y")]
        public int Y { get; set; }

        [JsonProperty("k")]
        public int K { get; set; }
    }

    public class Image
    {
        [JsonProperty("bare")]
        public string Bare { get; set; }

        [JsonProperty("named")]
        public string Named { get; set; }
    }

    public class Contrast
    {
        [JsonProperty("value")]
        public string Value { get; set; }
    }

    public class ColorInfoData
    {
        [JsonProperty("hex")]
        public Hex Hex { get; set; }

        [JsonProperty("rgb")]
        public Rgb Rgb { get; set; }

        [JsonProperty("hsl")]
        public Hsl Hsl { get; set; }

        [JsonProperty("hsv")]
        public Hsv Hsv { get; set; }

        [JsonProperty("name")]
        public Name Name { get; set; }

        [JsonProperty("cmyk")]
        public Cmyk Cmyk { get; set; }

        [JsonProperty("image")]
        public Image Image { get; set; }

        [JsonProperty("contrast")]
        public Contrast Contrast { get; set; }
    }
}