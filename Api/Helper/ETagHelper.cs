namespace Api.Helper
{
    public static class ETagHelper
    {
        public static string ETagFromVersion(long version) => $"W/\"{version}\"";

        public static long? VersionFromIfMatch(string? ifMatch)
        {
            if (string.IsNullOrWhiteSpace(ifMatch)) return null;
            var t = ifMatch.Trim();

            // accept W/"{number}" or "{number}"
            if (t.StartsWith("W/\"") && t.EndsWith("\""))
            {
                var inner = t[3..^1];
                if (long.TryParse(inner, out var v)) return v;
            }
            else if (t.StartsWith("\"") && t.EndsWith("\""))
            {
                var inner = t[1..^1];
                if (long.TryParse(inner, out var v)) return v;
            }
            else if (long.TryParse(t, out var v2))
            {
                return v2;
            }
            return null;
        }
    }
}

