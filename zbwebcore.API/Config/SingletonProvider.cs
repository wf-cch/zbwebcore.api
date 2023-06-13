using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace zbwebcore.API.Config
{
    /// <summary>
    /// 创建指定类型的新实例，加锁方式
    /// </summary>
    /// <typeparam name="T"></typeparam>
    // Token: 0x02000012 RID: 18
    public class SingletonProvider<T> where T : new()
    {
        // Token: 0x06000089 RID: 137 RVA: 0x00003A6D File Offset: 0x00001C6D
        private SingletonProvider()
        {
        }

        /// <summary>
        /// 创建实例
        /// </summary>
        // Token: 0x17000020 RID: 32
        // (get) Token: 0x0600008A RID: 138 RVA: 0x00003A78 File Offset: 0x00001C78
        public static T Instance
        {
            get
            {
                bool flag = SingletonProvider<T>._singleton == null;
                if (flag)
                {
                    object syncObject = SingletonProvider<T>.SyncObject;
                    lock (syncObject)
                    {
                        SingletonProvider<T>._singleton = Activator.CreateInstance<T>();
                    }
                }
                return SingletonProvider<T>._singleton;
            }
        }

        // Token: 0x0400001C RID: 28
        private static readonly object SyncObject = new object();

        // Token: 0x0400001D RID: 29
        private static T _singleton;
    }
}
