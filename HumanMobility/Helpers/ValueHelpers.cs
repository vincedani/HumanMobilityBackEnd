using System.Text;

namespace HumanMobility.Helpers
{
    public enum ExportType
    {
        Database,
        MergedData,
        CoherentData
    }

    public enum Fill
    {
        LastData,
        LinearInterpolation
    }

    public enum Flags
    {
        K, // OK
        E, // Error
        U, // Up
        F, // Fix
        M, // Miss
        J  // Jump
    }

    public static class EnumDisplayStringHelper
    {
        public static string GetDisplayStringFromName(string name)
        {
            var builder = new StringBuilder();

            foreach (char c in name)
            {
                if(char.IsUpper(c))
                    builder.Append(' ');

                builder.Append(c);
            }
            return builder.ToString();
        }
    }
}
