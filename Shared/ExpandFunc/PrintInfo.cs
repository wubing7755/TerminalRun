using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.Interface;

namespace Shared.ExpandFunc
{
    public class PrintInfo
    {
        public PrintInfo() { }

        public void Print()
        {
            Console.WriteLine("Test");
        }
    }

    public class Common : ICommon
    {
        public string PrintInfo { get; set; } = string.Empty;

        public void Print()
        {
            try
            {
                Console.WriteLine($"{PrintInfo}");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw new NotImplementedException(); ;
            }
        }

        public Common():this(0, 0)
        {

        }

        public Common(int a, int b)
        {
            this.a = a;
            this.b = b;
        }

        private int a;

        private int b;
    }

    public class Bus
    {
        protected static readonly DateTime globalStartTime;

        protected int RouteNumber { get; set; }

        static Bus()
        {
            globalStartTime = DateTime.Now;

            Console.WriteLine($"{globalStartTime.ToLongTimeString()}");
        }

        public Bus(int routeNum)
        {
            RouteNumber = routeNum;
            Console.WriteLine($"{RouteNumber}");
        }

        public void Drive()
        {
            TimeSpan elapsedTime = DateTime.Now - globalStartTime;
            Console.WriteLine($"{this.RouteNumber} - {elapsedTime.Microseconds} - {globalStartTime.ToShortTimeString()}");
        }
    }

    public sealed class Person:IDisposable
    {
        public Person(string name)
        {
            Name = name;
        }

        public Person(Person person)
        {
            Name = person.Name;
        }

        private string _name = "undefined";

        public string Name 
        { 
            get => _name; 
            set => _name = value;
        }

        public string GetName()
        {
            return $"'{_name}'";
        }

        /// <summary>
        /// 链式调用
        /// </summary>
        /// <returns></returns>
        public Person ChangePerson(Person person)
        {
            person.Name = "Hello";
            return person;
        }

        ~Person()
        {
            Console.WriteLine("\n释放Person对象");
        }

        public void Dispose()
        {
            Console.WriteLine("Dispose Person!");
        }
    }

}
