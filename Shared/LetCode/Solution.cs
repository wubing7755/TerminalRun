using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.Models;

namespace Shared.LetCode
{
    /// <summary>
    /// 系列解决方案
    /// </summary>
    public class Solution
    {
        /// <summary>
        /// 最长公共前缀
        /// </summary>
        /// <param name="strs"></param>
        /// <returns></returns>
        public string LongestCommonPrefix(string[] strs)
        {
            // 如果字符数组为空
            if (strs.Length == 0) return "";

            // 找出最短字符串
            string minStr = strs[0];
            foreach (var str in strs)
            {
                if(str.Length < minStr.Length) minStr = str;
            }

            StringBuilder sb = new StringBuilder(minStr);
            int minIndex = sb.Length;

            // 找出最长公共前缀
            foreach (var str in strs)
            {
                // 不和自身比较
                if(str == sb.ToString()) continue;
                for (int i = 0; i < sb.ToString().Length; i++)
                {
                    if (str[i] != sb[i])
                    {
                        if (i < minIndex) minIndex = i;
                        break;
                    }
                    
                }
            }

            return sb.ToString().Substring(0,minIndex);
        }

        /// <summary>
        /// 有效的括号
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public bool IsValid(string s)
        {
            Dictionary<char, char> brackets = new Dictionary<char, char> { { '(', ')' }, { '{', '}' }, { '[', ']' } };

            Stack<char> stack = new Stack<char>();

            foreach (var ch in s)
            {
                // 左闭括号入栈
                if(brackets.ContainsKey(ch)) stack.Push(ch);
                else
                {
                    // 右开括号出栈配对
                    if (stack.Count == 0 || brackets[stack.Pop()] != ch) return false;
                }
            }

            return stack.Count == 0;
        }

        ///// <summary>
        ///// 合并两个有序链表
        ///// </summary>
        ///// <param name="list1"></param>
        ///// <param name="list2"></param>
        ///// <returns></returns>
        ///// <remarks>
        ///// Input: list1 = [1,2,4], list2 = [1,3,4]
        ///// Output: [1, 1, 2, 3, 4, 4]
        ///// </remarks>
        //public ListNode MergeTwoLists(ListNode list1, ListNode list2)
        //{
        //    if(list1 == null) return list2;
        //    if(list2 == null) return list1;

        //    if (list1.val < list2.val)
        //    {
        //        list1.next = MergeTwoLists(list1.next, list2);
        //        return list1;
        //    }
        //    else
        //    {
        //        list2.next = MergeTwoLists(list1, list2.next);
        //        return list2;
        //    }
        //}

        /// <summary>
        /// 雇佣 K 位工人的总代价
        /// </summary>
        /// <param name="costs">每人的代价</param>
        /// <param name="k">雇佣人数</param>
        /// <param name="candidates">雇佣区间</param>
        /// <returns></returns>Console.WriteLine($"Selected worker index: {minIndex} -- cost: {minCost}");
        public long TotalCost(int[] costs, int k, int candidates)
        {
            // 总代价 
            long cost = 0;

            // 优先队列，用于存储候选工人的代价和索引
            SortedList<int, Queue<int>> candidateQueue = new SortedList<int, Queue<int>>();

            // 初始化优先队列
            // 前 candidates 个和后 candidates 个元素
            for (int i = 0; i < candidates && i < costs.Length; i++)
            {
                if (!candidateQueue.ContainsKey(costs[i]))
                {
                    candidateQueue[costs[i]] = new Queue<int>();
                }
                candidateQueue[costs[i]].Enqueue(i);
            }
            for (int i = costs.Length - 1; i >= costs.Length - candidates && i >= 0; i--)
            {
                if (!candidateQueue.ContainsKey(costs[i]))
                {
                    candidateQueue[costs[i]] = new Queue<int>();
                }
                candidateQueue[costs[i]].Enqueue(i);
            }

            // 已选中集合
            HashSet<int> selectedIndices = new HashSet<int>();

            // k 轮
            for (int i = 0; i < k; i++)
            {
                if (candidateQueue.Count == 0) break;
                // 从优先队列中选取最小代价的工人
                var minElement = candidateQueue.First();
                // 最小代价
                int minCost = minElement.Key;
                // 最小下标
                int minIndex = minElement.Value.Dequeue();

                // 移除value已经为空的队列
                if (minElement.Value.Count == 0) candidateQueue.Remove(minCost);

                cost += minCost;
                selectedIndices.Add(minIndex);
                Console.WriteLine($"Selected worker index: {minIndex} -- cost: {minCost}");

                // 更新优先队列
                // 前
                if (minIndex < costs.Length - candidates)
                {
                    int nextIndex = k + candidates;
                    if (nextIndex < costs.Length && !selectedIndices.Contains(nextIndex))
                    {
                        if (!candidateQueue.ContainsKey(costs[nextIndex]))
                        {
                            candidateQueue[costs[nextIndex]] = new Queue<int>();
                        }
                        candidateQueue[costs[nextIndex]].Enqueue(nextIndex);
                    }
                }

                // 后
                if (minIndex >= candidates)
                {
                    int preIndex = costs.Length - selectedIndices.Count - candidates - k;
                    if (preIndex >= 0 && !selectedIndices.Contains(preIndex))
                    {
                        if (!candidateQueue.ContainsKey(costs[preIndex]))
                        {
                            candidateQueue[costs[preIndex]] = new Queue<int>();
                        }
                        candidateQueue[costs[preIndex]].Enqueue(preIndex);
                    }
                }
            }

            return cost;
        }

        /// <summary>
        /// 删除有序数组中的重复项
        /// </summary>
        /// <param name="nums"></param>
        /// <returns></returns>
        public int RemoveDuplicates(int[] nums)
        {
            HashSet<int> exist = new HashSet<int>();

            int index = 0;

            for (int i = 0; i < nums.Length; i++)
            {
                if (exist.Add(nums[i]))
                {
                    nums[index++] = nums[i];
                }
            }

            return index;
        }
        
        /// <summary>
        /// 移除元素
        /// </summary>
        /// <param name="nums"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        /// num-27
        public int RemoveElement(int[] nums, int val)
        {
            // nums中不等于val的数量
            int k = 0;
            
            for (int i = 0; i < nums.Length; i++)
            {
                if (nums[i] == val)
                {
                    continue;
                }
                
                nums[k++] = nums[i];
            }
            
            return k;
        }
        
        /// <summary>
        /// 找出字符串中第一个匹配项的下标
        /// </summary>
        /// <param name="haystack"></param>
        /// <param name="needle"></param>
        /// <returns></returns>
        public int StrStr(string haystack, string needle)
        {
            // 判断是否完整地遍历完needle
            int index = 1;
            
            // 对于haystack，第一个匹配项的下标
            int backIndex = 0;

            if ((haystack.Length == needle.Length) && haystack == needle)
            {
                return 0;
            }

            for (int i = 0; i <= haystack.Length - needle.Length; i++)
            {
                index = i;
                for (int j = 0; j < needle.Length; j++)
                {
                    if (haystack[index ++] != needle[j]) break;

                    if ((j + 1) == needle.Length) return i;
                }
            }
            
            return -1;
        }
        
        /// <summary>
        /// 搜索插入位置
        /// </summary>
        /// <param name="nums"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public int SearchInsert(int[] nums, int target)
        {
            for (int i = 0; i < nums.Length; i++)
            {
                if (nums[i] >= target) return i;
            }
            return nums.Length;
        }
        
        /// <summary>
        /// 最后一个单词的长度
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public int LengthOfLastWord(string s)
        {
            string str = s.Trim().Split(' ').Last();
            return str.Length;
        }
        
        /// <summary>
        /// 加一
        /// </summary>
        /// <param name="digits"></param>
        /// <returns></returns>
        public int[] PlusOne(int[] digits) 
        {
            for (int i = digits.Length - 1; i >= 0; i--)
            {
                if (digits[i] + 1 > 10)
                {
                    digits[i] = (digits[i] + 1) % 10;
                }
                else if (digits[i] + 1 == 10)
                {
                    digits[i] = 0;
                }
                else
                {
                    digits[i] += 1;
                    break;
                }
            }
            
            // 第一位
            if (digits[0] == 0)
            {
                // 加一位在最后
                digits = digits.Append(0).ToArray();
                for (int i = digits.Length - 1; i > 0; i--)
                {
                    digits[i] = digits[i - 1];
                }

                digits[0] = 1;
                digits[1] = 0;
            }
            return digits;
        }
        
        /// <summary>
        /// 二进制求和
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        /// <remarks>两个二进制字符串 a 和 b ，以二进制字符串的形式返回它们的和。</remarks>
        public string AddBinary(string a, string b)
        {
            // 将二进制字符串转为为整形列表
            List<int> longStr = ((a.Length > b.Length) ? a : b).Select(c => c - '0').ToList();
            List<int> shortStr = ((a.Length > b.Length) ? b : a).Select(c => c - '0').ToList();
            // 长度
            int longStrLength = longStr.Count;
            int shortStrLength = shortStr.Count;
            
            for (int i = 1; i <= shortStrLength; i++)
            {
                int bitSum = longStr[longStrLength - i] + shortStr[shortStrLength - i];
                // 字符串每位相加，若有进位
                if (bitSum == 2)
                {
                    // 进位
                    int carryBit = 1;
                    // longStr进位加一
                    for (int j = longStrLength - i; j >= 0; j--)
                    {
                        if(longStr[j] + carryBit == 2)
                        {
                            carryBit = 1;
                            longStr[j] = 0;
                        }
                        else
                        {
                            carryBit = 0;
                            longStr[j] = 1;
                            break;
                        }
                    }

                    // 还多一个进位
                    if (carryBit == 1)
                    {
                        longStr.Insert(0, 1);
                        longStrLength = longStr.Count;
                    }
                }
                else if(bitSum == 1)
                {
                    longStr[longStrLength - i] = 1;
                }
                else
                {
                    longStr[longStrLength - i] = 0;
                }
            }
            
            return string.Concat(longStr.Select(n => n.ToString()));
        }
        
        /// <summary>
        /// x的平方根
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        /// <remarks>num.69</remarks>
        public int MySqrt(int x)
        {
            return -1;
        }
        
        /// <summary>
        /// 爬楼梯
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        /// <remarks>每次可以爬1阶或2阶楼梯</remarks>
        public int ClimbStairs(int n)
        {
            // 斐波那契数列
            int cur = 1, pre = 1;

            // 到第二阶楼梯时，有两种爬法
            for (int i = 2; i <= n; i++)
            {
                int next = cur + pre;
                pre = cur;
                cur = next;
            }
            
            return -1;
        }
        
        /// <summary>
        /// 快乐数
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        public bool IsHappy(int n)
        {
            
            return false;
        }

        /// <summary>
        /// 链表测试
        /// </summary>
        public void LinkNodeTest()
        {
            DoubleLink<string> list = new DoubleLink<string>();

            list.AddAfterHead("Hello");
            list.AddAfterHead("World");

            list.PrintNodeValue(list.HeadNode);
        }
    }
}

