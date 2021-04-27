namespace TowerDefense
{
    /// <summary>
    /// 普通单例基类
    /// </summary>
    public abstract class Singleton<T> where T : Singleton<T>, new()
    {
        private static T instance;

        /// <summary>
        /// 单例，如果没有对应的实例，会自动创建一个
        /// </summary>
        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new T();
                    instance.OnInit();
                }

                return instance;
            }
        }

        /// <summary>
        /// 在单例初始化时调用
        /// </summary>
        protected virtual void OnInit() { }
    }
}
