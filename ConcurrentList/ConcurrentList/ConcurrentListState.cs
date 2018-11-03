using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

/**
* 命名空间: ConcurrentList 
* 类 名： ConcurrentListState
* CLR版本： 4.0.30319.42000
* 版本 ：v1.0
* Copyright (c) jinyu  
*/

namespace ConcurrentList
{
    public delegate void BagEntryRemove<T>(object sender, T[] entrys);
    /// <summary>
    /// 功能描述    ：ConcurrentListState  
    /// 创 建 者    ：jinyu
    /// 创建日期    ：2018/10/31 17:56:35 
    /// 最后修改者  ：jinyu
    /// 最后修改日期：2018/10/31 17:56:35 
    /// </summary>
    public class ConcurrentListState<T>  where T:IConcurrentBagEntry
    {
        private IBucketStack<T> stack = null;

        private T[] removeList = null;
        private int removeIndex = -1;//移除数据
        private int removeCount = 10;//移除数据批量推送
        private int size = 0;
        private object lock_obj = new object();

        /// <summary>
        /// 接收移除的元素
        /// 批量推送
        /// </summary>
        public event BagEntryRemove<T> ArrayEntryRemove = null;

        /// <summary>
        /// 是否有数据
        /// </summary>
        public bool IsEmpty { get { return stack.Count == 0; } }

        /// <summary>
        /// 元素个数
        /// </summary>
        public int Count { get { return stack.Count; } }

        /// <summary>
        /// 批量移除数据推送个数
        /// </summary>
        public int EntryRemoveCount { get { return removeCount; } set { removeCount = value; RefreshRemove(); } }

        /// <summary>
        /// 有效数据大小
        /// </summary>
        public int Size { get { return size; } set { size = value; } }

        public ConcurrentListState()
        {
            stack = new ConcurrentBucket<T>();
        }

        /// <summary>
        /// 添加移除的数据
        /// </summary>
        /// <param name="item"></param>
        private void AddRemove(T item)
        {
            if(item==null)
             { return; }
            removeList[++removeIndex] = item;
            if (removeIndex + 1 == removeCount)
            {
                if (ArrayEntryRemove != null)
                {
                    T[] tmp = new T[removeCount];
                    Array.Copy(removeList, tmp, tmp.Length);
                    Task.Factory.StartNew(() =>
                    {
                        ArrayEntryRemove(this, tmp);
                    });
                }
            }
        }

        /// <summary>
        /// 刷新移除数据
        /// </summary>
        private void RefreshRemove()
        {
            if (removeIndex > -1)
            {
                if (ArrayEntryRemove != null)
                {
                    T[] tmp = new T[removeIndex + 1];
                    Array.Copy(removeList, tmp, tmp.Length);
                    removeIndex = -1;
                    Task.Factory.StartNew(() =>
                    {
                        ArrayEntryRemove(this, tmp);
                    });
                }
            }
            removeList = new T[removeCount];
        }

        /// <summary>
        /// 添加底部元素
        /// </summary>
        /// <param name="item"></param>
        public void AddBottom(T item)
        {
            stack.AddBottom(item);
        }

        /// <summary>
        /// 添加顶部元素
        /// </summary>
        /// <param name="item"></param>
        public void Push(T item)
        {
            if(item==null||item.State==IConcurrentBagEntry.STATE_REMOVED)
            {
                AddRemove(item);
            }
            stack.Push(item);
            Interlocked.Increment(ref size);
            if (!item.IsRegister)
            {
                //只需一次
                item.StateUpdate += Item_StateUpdate;
            }
        }

        /// <summary>
        /// 修改状态
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="state"></param>
        private void Item_StateUpdate(object sender, int state)
        {
            if (IConcurrentBagEntry.STATE_REMOVED == state)
            {
                Interlocked.Decrement(ref size);
            }
        }

        /// <summary>
        /// 取出底部元素并移除
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public bool TryBottom(out T result)
        {
           return stack.TryBottom(out result);
        }

        /// <summary>
        /// 取出底部元素不移除
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public bool TryOut(out T result)
        {
           return stack.TryOut(out result);
        }

        /// <summary>
        /// 取出顶部元素不移除
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public bool TryPeek(out T result)
        {
            bool r = true;
            do
            {
                if (!stack.TryBottom(out result))
                {
                    r = false;
                    break;
                }
            } while (IConcurrentBagEntry.STATE_REMOVED == result.State);
            if(r)
            {
                Interlocked.Decrement(ref size);
            }
            return r;
        }

        /// <summary>
        /// 取出顶部元素
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public bool TryPop(out T result)
        {
            bool r = true;
            do
            {
                if (!stack.TryPop(out result))
                {
                    r = false;
                    break;
                }
            } while (IConcurrentBagEntry.STATE_REMOVED == result.State);
            if(r)
            {
                Interlocked.Decrement(ref size);
            }
            return r;
           
        }

        /// <summary>
        /// 移除,修改状态
        /// </summary>
        /// <param name="item"></param>
        public void Remove(T item)
        {
            //修改状态即可
            item.CompareAndSetState(IConcurrentBagEntry.STATE_NOT_IN_USE, IConcurrentBagEntry.STATE_REMOVED);
        }
    }
}
