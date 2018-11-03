using System;
using System.Collections.Generic;
using System.Text;

namespace ConcurrentList
{

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
  public  interface IBucketStack<T>
    {

        /// <summary>
        /// 元素个数
        /// </summary>
        int Count { get; }

        /// <summary>
        /// 是否为空
        /// </summary>
        bool IsEmpty { get; }

        /// <summary>
        /// 添加顶部元素
        /// </summary>
        /// <param name="item"></param>
        void Push(T item);

        /// <summary>
        /// 取出顶部元素
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        bool TryPop(out T result);

        /// <summary>
        /// 取出顶部元素不移除
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        bool TryPeek(out T result);

        /// <summary>
        /// 添加底部元素
        /// </summary>
        /// <param name="item"></param>
        void AddBottom(T item);

        /// <summary>
        /// 取出底部元素
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        bool TryBottom(out T result);

        /// <summary>
        /// 取出底部元素不移除
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        bool TryOut(out T result);

        /// <summary>
        /// 清除元素
        /// </summary>
        void Clear();



    }
}
