using Shared.Models;
using TerminalRun.Input;

namespace TerminalRun.Cmd;

/// <summary>
/// </summary>
/// <remarks>Users.cs使用了Models/SampleData.cs中的类</remarks>
public static class Users
{
    /// <summary>
    ///     按Id顺序添加一个新Item
    /// </summary>
    /// <param name="firstName"></param>
    /// <param name="lastName"></param>
    /// <returns></returns>
    public static string Create(string firstName, string lastName)
    {
        var newId = SampleData.Users.Select(u => u.Id).Max() + 1;
        var newUser = new SampleData.User { Id = newId, FirstName = firstName, LastName = lastName };

        SampleData.Users.Add(newUser);
        return "Create Successfully!";
    }

    /// <summary>
    /// </summary>
    /// <returns></returns>
    public static string Get()
    {
        var indent = ConsoleFormatting.Indent(2);

        return string.Join(Environment.NewLine,
            SampleData.Users.Select(user => $"{indent}Id:{user.Id} {user.FirstName} {user.LastName}"));
    }
}