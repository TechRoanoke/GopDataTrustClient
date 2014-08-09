
namespace TechRoanoke.DataTrustClient
{
    public enum Sex
    {
        Unknown = 0,
        Male,
        Female
    }

    public static class SexExtensions
    {
        public static Sex Parse(string value)
        {
            switch (value)
            {
                case "M":
                case "m":
                    return Sex.Male;
                case "F":
                case "f":
                    return Sex.Female;
                default:
                    return Sex.Unknown;
            }
        }

        public static string ToDbValue(this Sex sex)
        {
            switch (sex)
            {
                case Sex.Male: return "M";
                case Sex.Female: return "F";
                default:
                    return "U";

            }
        }
    }
}