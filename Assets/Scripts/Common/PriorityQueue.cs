using System;
using System.Collections.Generic;

namespace TowerDefense
{
    /// <summary>
    /// 优先队列
    /// </summary>
    /// <typeparam name="T">元素的类型，必须实现IComparable接口</typeparam>
    public class PriorityQueue<T>
    {
        protected T[] datas; // 保存所有数据
        protected readonly HashSet<T> set; // 用一个HashSet来保存所有元素，提高Contains判断的速度
        protected IPriorityComparer<T> comparer;

        /// <summary>
        /// 创建一个默认大小为8的优先队列
        /// </summary>
        /// <param name="comparer">优先级比较器</param>
        public PriorityQueue(IPriorityComparer<T> comparer) : this(8, comparer)
        {
        }

        /// <summary>
        /// 创建一个自定义大小的优先队列
        /// </summary>
        /// <param name="capacity">大小</param>
        /// <param name="comparer">优先级比较器</param>
        public PriorityQueue(int capacity, IPriorityComparer<T> comparer)
        {
            this.comparer = comparer;
            Capacity = capacity;
            datas = new T[Capacity + 1];
            set = new HashSet<T>();
            Count = 0;
        }

        /// <summary>
        /// 队列是否为空
        /// </summary>
        public bool IsEmpty => Count == 0;

        /// <summary>
        /// 当前元素个数
        /// </summary>
        public int Count { get; private set; }

        /// <summary>
        /// 最大容量
        /// </summary>
        public int Capacity { get; private set; }

        /// <summary>
        /// 向队列中添加一个元素
        /// </summary>
        /// <param name="value">要添加的元素</param>
        public void Add(T value)
        {
            if (Count >= Capacity)
            {
                Resize(Count * 2);
            }

            datas[++Count] = value;
            set.Add(value);
            Swim(Count);
        }

        /// <summary>
        /// 获取队列中优先级最高的元素
        /// </summary>
        public T Max()
        {
            return datas[1];
        }

        /// <summary>
        /// 获取并删除队列中优先级最高的元素
        /// </summary>
        public T DeleteMax()
        {
            T max = datas[1];
            Swap(1, Count);
            datas[Count--] = default;
            Sink(1);
            set.Remove(max);

            return max;
        }

        /// <summary>
        /// 判断队列中是否包含目标元素
        /// </summary>
        public bool Contains(T value)
        {
            return set.Contains(value);
        }

        private void Swim(int k)
        {
            int front = k / 2;
            while (k > 1 && comparer.Compare(datas[front], datas[k]) < 0)
            {
                Swap(front, k);
                k = front;
            }
        }

        private void Sink(int k)
        {
            while (2 * k <= Count)
            {
                int j = 2 * k;

                if (j < Count && comparer.Compare(datas[j], datas[j + 1]) < 0)
                {
                    j++;
                }

                if (comparer.Compare(datas[k], datas[j]) >= 0) break;

                Swap(k, j);
                k = j;
            }
        }

        private void Swap(int i, int j)
        {
            T temp = datas[i];
            datas[i] = datas[j];
            datas[j] = temp;
        }

        private void Resize(int newSize)
        {
            Capacity = newSize;
            T[] temp = new T[newSize + 1];
            Array.Copy(datas, 1, temp, 1, datas.Length - 1);
            datas = temp;
        }
    }
}
