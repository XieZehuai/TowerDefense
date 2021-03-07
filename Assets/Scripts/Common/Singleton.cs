namespace TowerDefense
{
    public abstract class Singleton<T> where T : Singleton<T>, new()
    {
        private static T instance;

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

        protected virtual void OnInit() { }
    }
}
