using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace zbwebcore.API.Config
{
    public class Singleton
    {
        private volatile static Singleton _instance = null;
        private static readonly object lockHelper = new object();
        private Singleton() { }
        public static Singleton CreateInstance()
        {
            if (_instance == null)
            {
                lock (lockHelper)
                {
                    if (_instance == null)
                        _instance = new Singleton();
                }
            }
            return _instance;
        }
    }
}
