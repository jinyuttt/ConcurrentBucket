using System;
using System.Collections.Concurrent;

/**
* 命名空间: ConcurrentList 
* 类 名： ConcurrentList
* CLR版本： 4.0.30319.42000
* 版本 ：v1.0
* Copyright (c) jinyu  
*/

namespace ConcurrentList
{
    /// <summary>
    /// 功能描述    ：ConcurrentList  这个只是实验品
    /// 创 建 者    ：jinyu
    /// 创建日期    ：2018/10/31 17:05:44 
    /// 最后修改者  ：jinyu
    /// 最后修改日期：2018/10/31 17:05:44 
    /// </summary>
    public class ConcurrentList<T> : IBucketStack<T>
    {
        private ConcurrentStack<T> stack = null;

        public int Count { get { return stack.Count; } }

        public bool IsEmpty { get { return stack.IsEmpty; } }

        public ConcurrentList()
        {
            stack = new ConcurrentStack<T>();
        }

        public void AddBottom(T item)
        {
            if(stack.IsEmpty)
            {
                stack.Push(item);
            }
            else
            {
                T[] array = new T[stack.Count+1];
                int r=stack.TryPopRange(array);
                if(r<array.Length)
                {
                    array[r] = item;
                    stack.PushRange(array);
                }
                else
                {
                    T[] tmp = new T[r + 1];
                    Array.Copy(array, 0, tmp,0, r);
                    tmp[r] = item;
                    stack.PushRange(tmp);
                }
            }
        }

        public void Push(T item)
        {
            stack.Push(item);
        }

        public bool TryBottom(out T result)
        {
            result = default(T);
            if (stack.IsEmpty)
            {
                return false;
            }
            T[] array = new T[stack.Count];
            int r=stack.TryPopRange(array);
            if(r>0)
            {
                T[] tmp = new T[r];
                Array.Copy(array, 0, tmp, 0, r);
                stack.PushRange(tmp);
                return true;
            }
            return false;
        }

        public bool TryOut(out T result)
        {
            result = default(T);
            if(stack.IsEmpty)
            {
                return false;
            }
            T[] ts= stack.ToArray();
            if (ts != null && ts.Length > 0)
            {
                result = ts[ts.Length - 1];
                return true;
            }
            return false;
           
        }

        public bool TryPeek(out T result)
        {
          return  stack.TryPeek(out result);
        }

        public bool TryPop(out T result)
        {
            return stack.TryPop(out result);
        }

        public void Clear()
        {
            stack.Clear();
        }
    }
}
