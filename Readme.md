# TerminalRun

## ����

����һ��������Է����Ŀ���̨Ӧ�á�

## �÷�

### ��Shared.ExpandFunc����Ӳ��Ե���򷽷���

���磺 ���PrintInfo.cs�ļ�����������

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

### ��TerminalRun.Cmd.CS�ļ��е���

���磺��ʼ����󣬵��÷���

```csharp
public static string Transfer(string[]? inputs = null)
{
    PrintInfo printInfo = new PrintInfo();
            
    printInfo.Print();

    return "Execute successfully!";
}
```

### ���������ڿ���̨�����룺CS.Transfer

```csharp
CS.Transfer
```

ϵͳ���Զ������������õķ�����
