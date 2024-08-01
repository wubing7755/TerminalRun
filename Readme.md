# TerminalRun

## 主题

这是一个方便测试方法的控制台应用。

## 用法

### 在Shared.ExpandFunc中添加测试的类或方法。

例如： 添加PrintInfo.cs文件，内容如下

```csharp
public class PrintInfo
{
    public PrintInfo() { }

    public void Print()
    {
        Console.WriteLine("Test");
    }
}
```

### 在TerminalRun.Cmd.CS文件中调用

例如：初始化类后，调用方法

```csharp
public static string Transfer(string[]? inputs = null)
{
    PrintInfo printInfo = new PrintInfo();
            
    printInfo.Print();

    return "Execute successfully!";
}
```

### 启动程序，在控制台中输入：CS.Transfer

```csharp
CS.Transfer
```

系统会自动调用类中引用的方法。
