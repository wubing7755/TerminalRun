using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Models
{
    /// <summary>
    /// C#链表节点定义
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class LinkNode<T>
    {
        public T? Value;

        /// <summary>
        /// 头节点
        /// </summary>
        public LinkNode<T>? Prev;

        /// <summary>
        /// 尾节点
        /// </summary>
        public LinkNode<T>? Next;

        public LinkNode(T value)
        {
            Value = value;
            Prev = null;
            Next = null;
        }
    }

    /// <summary>
    /// 双向链表
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DoubleLink<T>
    {
        /// <summary>
        /// 头节点
        /// </summary>
        public LinkNode<T> HeadNode { get; set; }

        /// <summary>
        /// 尾节点
        /// </summary>
        public LinkNode<T> EndNode { get; set; }

        /// <summary>
        /// 节点个数
        /// </summary>
        public int Count { get; set; }

        public DoubleLink()
        {
            HeadNode = new LinkNode<T>(default);
            EndNode = new LinkNode<T>(default);

            HeadNode.Next = EndNode;
            EndNode.Prev = HeadNode;

            Count = 0;
        }

        /// <summary>
        /// 头插法
        /// </summary>
        /// <param name="nodeValue"></param>
        public void AddAfterHead(T nodeValue)
        {
            var node = new LinkNode<T>(nodeValue);

            node.Prev = HeadNode;
            node.Next = HeadNode.Next;

            HeadNode.Next!.Prev = node;
            HeadNode.Next = node;

            Count++;
        }

        /// <summary>
        /// 尾插法
        /// </summary>
        /// <param name="nodeValue"></param>
        public void AddAfterEnd(T nodeValue)
        {
            var node = new LinkNode<T>(nodeValue);

            EndNode.Prev!.Next = node;
            node.Prev = EndNode.Prev;

            EndNode.Prev = node;
            node.Next = EndNode;

            Count++;
        }

        /// <summary>
        /// 打印输出
        /// </summary>
        /// <param name="node"></param>
        public void PrintNodeValue(LinkNode<T> node)
        {
            LinkNode<T> curr = new LinkNode<T>(default);

            curr.Next = node;

            while (curr.Next != null)
            {
                Console.WriteLine($"{curr.Next.Value}");

                curr.Next = curr.Next.Next;
            }
        }
    }

}
