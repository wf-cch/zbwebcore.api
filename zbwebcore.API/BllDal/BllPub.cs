using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using zbwebcore.API.Config;
using zbwebcore.API.Model;

namespace zbwebcore.API.BllDal
{
    public class BllPub
    {
        public static BllPub Instance
        {
            get { return SingletonProvider<BllPub>.Instance; }
        }
        public string getCxtj(string json)
        {
            string cxtj = "";
            try
            {
                var obj = Dbservice.DB.Utilities.DeserializeObject<Dictionary<string, object>>(json);
                for (int i = 0; i < obj.Count; i++)
                {
                    var item = obj.ElementAt(i);
                    string key = item.Key.ToString();
                    string value = item.Value.ToString();
                    if (!string.IsNullOrEmpty(value))
                    {
                        if (key == "ksjsrq")
                        {
                            var ksjsrq = value.Split(" - ");
                            var ksrq = ksjsrq[0].Trim();
                            var jsrq = ksjsrq[1].Trim();
                            cxtj = cxtj + " and rq>='" + ksrq + "' and rq<='" + jsrq + "'";
                        }else if (key == "ksrq")
                        {
                            cxtj = cxtj + " and rq>='" + value+"'";
                        }
                        else if (key == "jsrq")
                        {
                            cxtj = cxtj + " and rq<='" + value + "'";
                        }
                        else if (key == "fdbh")
                        {
                            var fdbh = "'" + value.Replace(",", "','") + "'";
                            cxtj = cxtj + " and fdbh in (" + fdbh + ")";
                        }
                        else
                        {
                            cxtj = cxtj + " and " + key + " like '%" + value + "%'";
                        }
                    }
                    
                    
                }


            }
            catch (Exception ex)
            {
                
            }
            return cxtj;
        }
        public bool SqlFilter(string InText)
        {
            string word = ConfigInfo.Info.SqlFilterWord;
            if (InText == null)
                return false;
            foreach (string i in word.Split('|'))
            {
                if ((InText.ToLower().IndexOf(i + " ") > -1) || (InText.ToLower().IndexOf(" " + i) > -1))
                {
                    return true;
                }
            }
            return false;
        }
        public byte[] StreamToBytes(Stream stream)
        {
            byte[] bytes = new byte[stream.Length];
            stream.Read(bytes, 0, bytes.Length);

            // 设置当前流的位置为流的开始 
            stream.Seek(0, SeekOrigin.Begin);
            return bytes;
        }
        public List<Dictionary<string,object>> DtToDictList(DataTable dt)
        {
            var listdic = new List<Dictionary<string, object>>();
            foreach (DataRow dr in dt.Rows)
            {
                var dct = new Dictionary<string, object>();
                foreach (DataColumn dc in dt.Columns)
                {
                    dct.Add(dc.ColumnName, dr[dc.ColumnName].ToString());
                }
                listdic.Add(dct);
            }
            return listdic;
        }
    }
}
