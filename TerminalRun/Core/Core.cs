using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using TerminalRun.Input;

namespace TerminalRun.Core;

public static class Core
{
    #region 字段

    /// <summary>
    ///     控制台提示信息
    /// </summary>
    private const string _readPrompt = "console> ";

    /// <summary>
    ///     方法的命名空间
    /// </summary>
    private const string _commandNamespace = "TerminalRun.Cmd";

    /// <summary>
    ///     一个应用程序，支持多个命令库，每个命令库中包含多个命令，每个命令有其参数
    ///     [类空间名，[方法名,参数列表]]
    /// </summary>
    private static readonly Dictionary<string, Dictionary<string, IEnumerable<ParameterInfo>>>
        _commandLibraries = new();

    /// <summary>
    ///     类型映射字典
    /// </summary>
    private static readonly Dictionary<TypeCode, Func<string, object>> TypeParsers = new()
    {
        { TypeCode.String, ParseValue<string> },
        { TypeCode.Int16, input => ParseValue<short>(input) },
        { TypeCode.Int32, input => ParseValue<int>(input) },
        { TypeCode.Int64, input => ParseValue<long>(input) },
        { TypeCode.Boolean, input => ParseValue<bool>(input) },
        { TypeCode.Byte, input => ParseValue<byte>(input) },
        { TypeCode.Char, input => ParseValue<char>(input) },
        { TypeCode.DateTime, input => ParseValue<DateTime>(input) },
        { TypeCode.Decimal, input => ParseValue<decimal>(input) },
        { TypeCode.Double, input => ParseValue<double>(input) },
        { TypeCode.Single, input => ParseValue<float>(input) },
        { TypeCode.UInt16, input => ParseValue<ushort>(input) },
        { TypeCode.UInt32, input => ParseValue<uint>(input) },
        { TypeCode.UInt64, input => ParseValue<ulong>(input) }
    };

    #endregion

    #region 方法

    /// <summary>
    ///     初始化
    /// </summary>
    /// <remarks>将TerminalRun.Cmd命名空间下的所有静态类及静态类中的方法存储到_commandLibraries中</remarks>
    public static void InitialConsole()
    {
        // 控制台标题
        Console.Title = nameof(Program);

        /* ------------------------ Libraries-Start ------------------------ */

        /*
         * 通过反射动态地加载当前程序集中特定命名空间中的所有静态类，并将这些类中所有公共静态方法的信息存储在一个嵌套字典中
         */

        // 使用反射获取当前程序集的所有类 => IsClass && Namespace == "TerminalRun.Cmd"
        var commandClasses = Assembly.GetExecutingAssembly().GetTypes()
            .Where(t => t is { IsClass: true, Namespace: _commandNamespace }).ToList();

        foreach (var commandClass in commandClasses)
        {
            // 从当前程序集的所有类中，获取 public static修饰的方法
            var methods = commandClass.GetMethods(BindingFlags.Static | BindingFlags.Public);
            // 方法字典：[方法名，参数列表]
            var methodDictionary = new Dictionary<string, IEnumerable<ParameterInfo>>();
            // 将静态类中的方法填充到方法字典中
            foreach (var method in methods)
            {
                var commandName = method.Name;

                // 将方法名和获取的参数列表填充到方法字典中
                methodDictionary.Add(commandName, method.GetParameters());
            }

            // 类字典：[类名，方法名]
            _commandLibraries.Add(commandClass.Name, methodDictionary);
        }
        /* ------------------------ Libraries-End ------------------------ */
    }

    /// <summary>
    ///     运行
    /// </summary>
    public static void Run()
    {
        while (true)
        {
            // 获取控制台输入
            var consoleInput = ReadFromConsole();

            consoleInput = "CS.Transfer";

            if (string.IsNullOrWhiteSpace(consoleInput)) continue;

            // 清空控制台输入
            if (consoleInput == "cls")
            {
                Console.Clear();
                continue;
            }

            try
            {
                // Create a ConsoleCommand instance:
                var cmd = new ConsoleCommand(consoleInput);

                // Execute the command:
                var result = Execute(cmd);

                // Write out the result:
                WriteToConsole(result);
            }
            catch (Exception ex)
            {
                // OOPS! Something went wrong - Write out the problem:
                WriteToConsole(ex.Message);
            }
        }
    }

    /// <summary>
    ///     执行DefaultCommands自定义的控制台命令
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public static string Execute(ConsoleCommand command)
    {
        // Validate the class name and command name:
        // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        var badCommandMessage = string.Format("Unrecognized command \'{0}.{1}\'. Please type a valid command.",
            command.LibraryClassName, command.Name);

        // 检查命令库字典中是否包含指定的类名和命令名
        // Validate the command name:
        if (!_commandLibraries.TryGetValue(command.LibraryClassName, out var methodDictionary
            ))
            return badCommandMessage;

        if (!methodDictionary.ContainsKey(command.Name)) return badCommandMessage;

        // Make sure the correct number of required arguments are provided:
        // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        var methodParameterValueList = new List<object>();
        IEnumerable<ParameterInfo> paramInfoList = methodDictionary[command.Name].ToList();

        // Validate proper # of required arguments provided. Some may be optional:
        var requiredParams = paramInfoList.Where(p => p.IsOptional == false);
        var optionalParams = paramInfoList.Where(p => p.IsOptional);
        var requiredCount = requiredParams.Count();
        var optionalCount = optionalParams.Count();
        var providedCount = command.Arguments.Count();

        if (requiredCount > providedCount)
            return string.Format(
                "Missing required argument. {0} required, {1} optional, {2} provided",
                requiredCount, optionalCount, providedCount);

        // Make sure all arguments are correct to the proper type, and that there is a 
        // value for every method parameter. The InvokeMember method fails if the number 
        // of arguments provided does not match the number of parameters in the 
        // method signature, even if some are optional:
        // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        if (paramInfoList.Any())
        {
            // Populate the list with default values:
            foreach (var param in paramInfoList)
                // This will either add a null object reference if the param is required 
                // by the method, or will set a default value for optional parameters. in 
                // any case, there will be a value or null for each method argument 
                // in the method signature:
                methodParameterValueList.Add(param.DefaultValue);

            // Now walk through all the arguments passed from the console and assign 
            // accordingly. Any optional arguments not provided have already been set to 
            // the default specified by the method signature:
            for (var i = 0; i < command.Arguments.Count(); i++)
            {
                var methodParam = paramInfoList.ElementAt(i);
                var typeRequired = methodParam.ParameterType;
                try
                {
                    // Coming from the Console, all of our arguments are passed in as 
                    // strings. Coerce to the type to match the method parameter:
                    var value = CoerceArgument(typeRequired, command.Arguments.ElementAt(i));
                    methodParameterValueList.RemoveAt(i);
                    methodParameterValueList.Insert(i, value);
                }
                catch (ArgumentException ex)
                {
                    var argumentName = methodParam.Name;
                    var argumentTypeName = typeRequired.Name;
                    var message =
                        string.Format(""
                                      + "The value passed for argument '{0}' cannot be parsed to type '{1}'",
                            argumentName, argumentTypeName);
                    throw new ArgumentException(message);
                }
            }
        }

        // 使用反射调用自定义 public static 方法
        // 将用户输入的文本解析为命令和参数
        // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        var current = typeof(Program).Assembly;

        // 命名空间.类名
        var commandLibraryClass =
            current.GetType(_commandNamespace + "." + command.LibraryClassName);

        // 参数列表
        object[] inputArgs = null;
        if (methodParameterValueList.Count > 0) inputArgs = methodParameterValueList.ToArray();

        // 如果提供的参数数量与数量不匹配，则会抛出此操作
        // 方法签名是必需的，即使有些是可选的：
        try
        {
            // ++++++++++++ ******************** ++++++++++++
            // ++++++++++++ 整个Console中的核心代码 ++++++++++++
            // ++++++++++++ ******************** ++++++++++++
            var result = commandLibraryClass.InvokeMember(
                command.Name,
                BindingFlags.InvokeMethod | BindingFlags.Static | BindingFlags.Public,
                null, null, inputArgs);
            // ++++++++++++ ******************** ++++++++++++
            // // ++++++++++++ 整个Console中的核心代码 ++++++++++++
            // // ++++++++++++ ******************** ++++++++++++

            // 如果是 void 类型返回方法
            if (result == null) return "This is a void return value method.";

            // object => string
            result = FormatResult(result);

            return result.ToString() ?? string.Empty;
        }
        catch (TargetInvocationException ex)
        {
            Debug.Assert(ex.InnerException != null, "ex.InnerException != null");
            throw ex.InnerException;
        }
    }

    /// <summary>
    ///     将字符串表示的参数转换为特定类型
    /// </summary>
    /// <param name="requiredType"></param>
    /// <param name="inputValue"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public static object CoerceArgument(Type requiredType, string inputValue)
    {
        // 错误处理
        var exceptionMessage =
            string.Format("Cannot coerce the input argument {0} to required type {1}",
                inputValue, requiredType.Name);

        // 如果是数组类型
        if (requiredType.IsArray)
        {
            // 将inputValue转换为数组，并赋给result
            var elementType = requiredType.GetElementType();

            var stringValues = inputValue.Trim('[', ']').Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            var array = Array.CreateInstance(elementType ?? throw new InvalidOperationException(), stringValues.Length);

            for (var i = 0; i < stringValues.Length; i++)
                array.SetValue(CoerceArgument(elementType, stringValues[i].Trim()), i);

            return array;
        }

        // 获取参数的TypeCode
        var requiredTypeCode = Type.GetTypeCode(requiredType);
        if (TypeParsers.TryGetValue(requiredTypeCode, out var parser))
            try
            {
                return parser(inputValue);
            }
            catch (Exception)
            {
                throw new ArgumentException(exceptionMessage);
            }

        return "result=null";
    }

    /// <summary>
    ///     将结果格式化为字符串
    /// </summary>
    /// <param name="result">要格式化的结果</param>
    /// <returns>格式化后的字符串</returns>
    public static string FormatResult(object result)
    {
        // 如果是字符数组，则转换
        if (result.GetType().IsArray)
        {
            var array = (Array)result;
            var elements = array.Cast<object>().Select(e => e.ToString());
            return $"{string.Join(", ", elements)}";
        }

        return result.ToString() ?? string.Empty;
    }

    /// <summary>
    ///     解析函数，将字符串转换到指定类型
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="inputValue"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    private static T ParseValue<T>(string inputValue)
    {
        try
        {
            if (typeof(T) == typeof(char) && inputValue.Length == 1) return (T)(object)inputValue[0];
            if (TypeDescriptor.GetConverter(typeof(T)).IsValid(inputValue))
                return (T)TypeDescriptor.GetConverter(typeof(T)).ConvertFromString(inputValue)!;
        }
        catch (Exception)
        {
            // 在这里捕获异常并在下面抛出自定义异常
        }

        throw new ArgumentException($"Cannot convert '{inputValue}' to type {typeof(T).Name}");
    }

    /// <summary>
    ///     写入信息到控制台
    /// </summary>
    /// <param name="message"></param>
    public static void WriteToConsole(string message = "")
    {
        if (message.Length > 0) Console.WriteLine(message);
    }

    /// <summary>
    ///     从控制台读取信息
    /// </summary>
    /// <param name="promptMessage"></param>
    /// <returns></returns>
    public static string ReadFromConsole(string promptMessage = "")
    {
        // Show a prompt, and get input:
        Console.Write(_readPrompt + promptMessage);
        return Console.ReadLine() ?? string.Empty;
    }

    #endregion
}