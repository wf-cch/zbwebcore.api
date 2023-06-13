using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace zbwebcore.API.Model
{
    public class qxgl_menu
    {
        /// <summary>
        /// ID
        /// </summary>
        public string id { get; set; }
        public string zybh { get; set; }

        /// <summary>
        /// 节点名称
        /// </summary>
        public string title { get; set; }

        public string fcode { get; set; }
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
        public List<qxgl_menu> childs { get; set; }
    }
}
