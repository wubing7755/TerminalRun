using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.LetCode
{
    /// <summary>
    /// 001道题目
    /// </summary>
    public class Exam001
    {
        public void PrintInfo(string[] inputs)
        {
            Console.WriteLine("001道题目");
            foreach (var input in inputs)
            {
                Console.Write($"{input} ");
            }
        }
    }
}
