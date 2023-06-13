using System;
using System.Collections.Generic;
using System.Text;

namespace zbwebcore.API.Model
{
    public class DataBaseSetting : IDataBaseSetting
    {
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
    }
    public interface IDataBaseSetting
    {
        string ConnectionString { get; set; }
        /// <summary>
        /// 当是关系型数据库时，DatabaseName属性没用到
        /// </summary>
        string DatabaseName { get; set; }
    }
}


