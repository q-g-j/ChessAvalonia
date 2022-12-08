namespace ChessAvalonia.Helpers;

internal static class ChessDebug
{
    internal static void Write(string text)
    {
        System.Diagnostics.Debug.Write(text);
    }
    internal static void WriteLine(string text)
    {
        System.Diagnostics.Debug.WriteLine(text);
    }
    internal static void WriteLine(int number)
    {
        System.Diagnostics.Debug.WriteLine(number.ToString());
    }
}