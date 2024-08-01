using System.Text.RegularExpressions;

namespace TerminalRun.Input;

/// <summary>
///     管理命令和参数的集合
/// </summary>
public class ConsoleCommand
{
    /// <summary>
    ///     私有的命令-参数列表
    /// </summary>
    private readonly List<string> _arguments = new();

    public ConsoleCommand(string input)
    {
        FormatInput(input);
    }

    /// <summary>
    ///     命名名
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    ///     类名
    /// </summary>
    public string LibraryClassName { get; set; }

    /// <summary>
    ///     命令-参数列表
    /// </summary>
    public IEnumerable<string> Arguments => _arguments;

    /// <summary>
    ///     规范化输入参数
    /// </summary>
    /// <param name="input"></param>
    public void FormatInput(string input)
    {
        // 在出现空格的地方拆分文本字符串，但保留带引号的文本字符串不变。
        var stringArray = Regex.Split(input,
            "(?<=^[^\"]*(?:\"[^\"]*\"[^\"]*)*) (?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)");

        for (var i = 0; i < stringArray.Length; i++)
            // 我们假设第一个元素始终是命令
            if (i == 0)
            {
                LibraryClassName = "DefaultCommands";
                Name = stringArray[0];

                var s = stringArray[0].Split('.');
                if (s.Length == 2)
                {
                    LibraryClassName = s[0];
                    Name = s[1];
                }
            }
            else
            {
                // Assume that most of the time, the input argument is NOT quoted text:
                var argument = stringArray[i];

                /*
                 * ["a","b","c"]
                 * 或"abc"
                 */
                if (argument.StartsWith("["))
                {
                    // 替换匹配到的双引号为单引号
                    var pattern = @"\""(\w+)\""";

                    argument = Regex.Replace(argument, pattern, "'$1'");
                }
                else
                {
                    // Is the argument a quoted text string?
                    var regex = new Regex("\"(.*?)\"", RegexOptions.Singleline);

                    var match = regex.Match(argument);

                    // If it IS quoted, there will be at least one capture:
                    if (match.Captures.Count > 0)
                    {
                        // Get the unquoted text from within the quotes:
                        var captureQuotedText = new Regex("[^\"]*[^\"]");
                        var quoted = captureQuotedText.Match(match.Captures[0].Value);

                        // The argument should include all text from between the quotes
                        // as a single string:
                        argument = quoted.Captures[0].Value;
                    }
                }

                _arguments.Add(argument);
            }
    }
}