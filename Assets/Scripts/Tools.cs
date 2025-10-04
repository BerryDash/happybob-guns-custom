using System.Numerics;

public static class Tools
{
    public static string FormatWithCommas(BigInteger number)
    {
        return string.Format("{0:N0}", number);
    }
}