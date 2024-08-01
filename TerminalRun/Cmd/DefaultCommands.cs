

// All console commands must be in the sub-namespace Commands:
namespace TerminalRun.Cmd;

// Must be a public static class:
public static class DefaultCommands
{
    // Methods used as console commands must be public and must return a string

    /// <summary>
    ///     清空控制台
    /// </summary>
    /// <returns></returns>
    public static string CLS()
    {
        Console.Clear();
        return string.Empty;
    }
}