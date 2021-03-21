namespace TowerDefense
{
    /// <summary>
    /// 优先级比较器接口，用于在优先队列中比较两个元素的优先级
    /// </summary>
    /// <typeparam name="T">可以比较优先级的元素类型</typeparam>
    public interface IPriorityComparer<T>
    {
        /// <summary>
        /// 比较两个元素的优先级
        /// </summary>
        /// <param name="a">元素a</param>
        /// <param name="b">元素b</param>
        /// <returns>a优先级大于b返回正值，a优先级小于b返回负值，a和b优先级相等返回0</returns>
        int Compare(T a, T b);
    }
}
