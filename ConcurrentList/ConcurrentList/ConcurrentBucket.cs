using System;
using System.Collections.Generic;
using System.Threading;

/**
* 命名空间: ConcurrentList 
* 类 名： ConcurrentListStack
* CLR版本： 4.0.30319.42000
* 版本 ：v1.0
* Copyright (c) jinyu  
*/

namespace ConcurrentList
{
    /// <summary>
    /// 功能描述    ：ConcurrentBucket   实现2端操作的结构
    /// 创 建 者    ：jinyu
    /// 创建日期    ：2018/10/31 15:37:08 
    /// 最后修改者  ：jinyu
    /// 最后修改日期：2018/10/31 15:37:08 
    /// </summary>
    public class ConcurrentBucket<T> : IBucketStack<T>
    {
        /// <summary>
        /// 数据源
        /// </summary>
        private LinkedList<T> link = null;

        /// <summary>
        /// 移除索引
        /// </summary>
        private Dictionary<T, LinkedListNode<T>> dicIndex = null;

        /// <summary>
        /// 锁定对象
        /// </summary>
        private object lock_obj = new object();

        public ConcurrentBucket()
        {
            link = new LinkedList<T>();
            dicIndex = new Dictionary<T, LinkedListNode<T>>();
        }

        /// <summary>
        /// 元素个数
        /// </summary>
        public int Count { get { return link.Count; } }

        public bool IsEmpty { get { return Count == 0; } }

        /// <summary>
        /// 添加底部元素
        /// </summary>
        /// <param name="item"></param>
        public void AddBottom(T item)
        {
            bool token = false;
            try
            {
                Monitor.Enter(lock_obj, ref token);
                LinkedListNode<T> node = link.AddFirst(item);
                dicIndex[item] = node;
            }
            catch (Exception ex)
            {

            }
            finally
            {
                if (token) Monitor.Exit(lock_obj);
            }
        }

        /// <summary>
        /// 添加顶部元素
        /// </summary>
        /// <param name="item"></param>
        public void Push(T item)
        {
            bool token = false;
            try
            {
                Monitor.Enter(lock_obj,ref token);
                LinkedListNode<T> node= link.AddLast(item);
                dicIndex[item] = node;
               
            }
            catch(Exception ex)
            {

            }
            finally
            {
                if (token) Monitor.Exit(lock_obj);
            }

        }

        /// <summary>
        /// 取出底部元素
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public bool TryBottom(out T result)
        {
            bool token = false;
            result = default(T);
            if(link.First==null)
            {
                return false;
            }
            try
            {
                Monitor.Enter(lock_obj, ref token);
                if(link.First!=null)
                {
                    result = link.First.Value;
                    dicIndex.Remove(result);
                    link.RemoveFirst();
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
            finally
            {
                if (token) Monitor.Exit(lock_obj);
            }
           
        }

        /// <summary>
        /// 取出底部元素不移除
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public bool TryOut(out T result)
        {
            bool token = false;
            result = default(T);
            if (link.First == null)
            {
                return false;
            }
            try
            {
                Monitor.Enter(lock_obj, ref token);
                if (link.First != null)
                {
                    result = link.First.Value;
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
            finally
            {
                if (token) Monitor.Exit(lock_obj);
            }
           
        }

        /// <summary>
        /// 取出顶部元素不移除
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public bool TryPeek(out T result)
        {
            bool token = false;
            result = default(T);
            if (link.Last == null)
            {
                return false;
            }
            try
            {
                Monitor.Enter(lock_obj, ref token);
                if (link.Last != null)
                {
                    result = link.Last.Value;
                    return true;
                }
                return false;

            }
            catch (Exception ex)
            {
                return false;
            }
            finally
            {
                if (token) Monitor.Exit(lock_obj);
            }
         
        }

        /// <summary>
        /// 取出顶部元素
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public bool TryPop(out T result)
        {
            bool token = false;
            result = default(T);
            if (link.Last == null)
            {
                return false;
            }
            try
            {
                Monitor.Enter(lock_obj, ref token);
                if (link.Last != null)
                {
                    result = link.Last.Value;
                    link.RemoveLast();
                    dicIndex.Remove(result);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
            finally
            {
                if (token) Monitor.Exit(lock_obj);
            }
        }

        /// <summary>
        /// 移除数据
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Remove(T item)
        {
            LinkedListNode<T> node = null;
            if(dicIndex.TryGetValue(item,out node))
            {
                link.Remove(node);
                return true;
            }
            else
            {
               return link.Remove(item);
            }
        }

        /// <summary>
        /// 清除元素
        /// </summary>
        public void Clear()
        {
            link.Clear();
        }
    }
}
