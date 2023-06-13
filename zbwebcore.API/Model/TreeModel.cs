using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace zbwebcore.API.Model
{
    public class TreeModel
    {
        /// <summary>
        /// ID
        /// </summary>
        public string id { get; set; }

        /// <summary>
        /// 节点名称
        /// </summary>
        public string title { get; set; }

        /// <summary>
        /// 是否展开
        /// </summary>
        public string state { get; set; }

        /// <summary>
        /// 图标样式
        /// </summary>
        public string icon { get; set; }
        /// <summary>
        /// url
        /// </summary>
        public string href { get; set; }

        /// <summary>
        /// 子节点集合
        /// </summary>
        public List<TreeModel> childs { get; set; }
        
    }

}
