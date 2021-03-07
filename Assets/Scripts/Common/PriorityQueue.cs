using System;
using System.Collections.Generic;

namespace TowerDefense
{
    /// <summary>
    /// 优先队列
    /// </summary>
    /// <typeparam name="T">元素的类型，必须实现IComparable接口</typeparam>
    public class PriorityQueue<T> where T : IComparable<T>
    {
        private T[] datas; // 保存所有数据
        private readonly HashSet<T> set; // 用一个HashSet来保存所有元素，提高Contains判断的速度

        /// <summary>
        /// 创建一个容量为capacity的优先队列
        /// </summary>
        /// <param name="capacity">容量</param>
        public PriorityQueue(int capacity)
        {
            Capacity = capacity;
            datas = new T[Capacity + 1];
            set = new HashSet<T>();
            Count = 0;
        }

        public bool IsEmpty => Count == 0;

        public int Count { get; private set; }

        public int Capacity { get; private set; }

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

        public T Max()
        {
            return datas[1];
        }

        public T DeleteMax()
        {
            T max = datas[1];
            Swap(1, Count);
            datas[Count--] = default;
            Sink(1);
            set.Remove(max);

            return max;
        }

        public bool Contains(T value)
        {
            return set.Contains(value);
        }

        private void Swim(int k)
        {
            int front = k / 2;
            while (k > 1 && datas[front].CompareTo(datas[k]) < 0)
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

                if (j < Count && datas[j].CompareTo(datas[j + 1]) < 0)
                {
                    j++;
                }

                if (datas[k].CompareTo(datas[j]) >= 0) break;

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
