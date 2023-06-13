using Newtonsoft.Json;
using NPOI.HSSF.UserModel;
using NPOI.SS.Formula.Eval;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using zbwebcore.API.BllDal;

namespace zbwebcore.API.Config
{
    public class NpoiHelper
    {
        #region 公有方法
        //导出,默认为excel2007
        public static byte[] ExportByte(DataTable dt)
        {
            var bytes = RenderToExcelByte2007(dt);
            return bytes;
        }
        //导入,返回dict
        public static List<Dictionary<string,object>> ExcelToDictList(Stream sm, string filename)
        {
            IWorkbook workbook = null;
            var dt = new DataTable();
            var f = filename.Split(".");
            if (f.Length != 2) return null;
            if (f[1] == "xls")
            {
                workbook = new HSSFWorkbook(sm);
            }
            else
            {
                workbook = new XSSFWorkbook(sm);
            }

            var sheet = workbook.GetSheetAt(0);

            //列头
            foreach (var item in sheet.GetRow(sheet.FirstRowNum).Cells)
            {
                string colName = item.ToString();
                if (dt.Columns.Contains(colName))
                {
                    colName += item.ColumnIndex.ToString();
                }
                dt.Columns.Add(colName, typeof(string));
            }

            //写入内容
            var rows = sheet.GetRowEnumerator();
            IRow row = null;
            while (rows.MoveNext())
            {
                if (f[1]=="xls")
                {
                    row = (HSSFRow)rows.Current;
                }
                else
                {
                    row = (XSSFRow)rows.Current;
                }
                if (row.RowNum == sheet.FirstRowNum)
                {
                    continue;
                }

                var dr = dt.NewRow();
                foreach (var item in row.Cells)
                {
                    if (item.ColumnIndex >= dt.Columns.Count)
                    {
                        break;
                    }
                    if (item == null)
                    {
                        dr[item.ColumnIndex] = string.Empty;
                        continue;
                    }
                    switch (item.CellType)
                    {

                        case CellType.Boolean:
                            dr[item.ColumnIndex] = item.BooleanCellValue;
                            break;
                        case CellType.Error:
                            dr[item.ColumnIndex] = ErrorEval.GetText(item.ErrorCellValue);
                            break;
                        case CellType.Formula:
                            switch (item.CachedFormulaResultType)
                            {
                                case CellType.Boolean:
                                    dr[item.ColumnIndex] = item.BooleanCellValue;
                                    break;
                                case CellType.Error:
                                    dr[item.ColumnIndex] = ErrorEval.GetText(item.ErrorCellValue);
                                    break;
                                case CellType.Numeric:
                                    if (DateUtil.IsCellDateFormatted(item))
                                    {
                                        dr[item.ColumnIndex] = item.DateCellValue.ToString("yyyy-MM-dd hh:MM:ss");
                                    }
                                    else
                                    {
                                        dr[item.ColumnIndex] = item.NumericCellValue;
                                    }
                                    break;
                                case CellType.String:
                                    var str = item.StringCellValue;
                                    if (!string.IsNullOrEmpty(str))
                                    {
                                        dr[item.ColumnIndex] = str;
                                    }
                                    else
                                    {
                                        dr[item.ColumnIndex] = string.Empty;
                                    }
                                    break;
                                case CellType.Unknown:
                                case CellType.Blank:
                                default:
                                    dr[item.ColumnIndex] = string.Empty;
                                    break;
                            }
                            break;
                        case CellType.Numeric:
                            if (DateUtil.IsCellDateFormatted(item))
                            {
                                dr[item.ColumnIndex] = item.DateCellValue.ToString("yyyy-MM-dd hh:MM:ss");
                            }
                            else
                            {
                                dr[item.ColumnIndex] = item.NumericCellValue;
                            }
                            break;
                        case CellType.String:
                            var strValue = item.StringCellValue;
                            if (!string.IsNullOrEmpty(strValue))
                            {
                                dr[item.ColumnIndex] = strValue;
                            }
                            else
                            {
                                dr[item.ColumnIndex] = string.Empty;
                            }
                            break;
                        case CellType.Unknown:
                        case CellType.Blank:
                        default:
                            dr[item.ColumnIndex] = string.Empty;
                            break;
                    }
                }
                dt.Rows.Add(dr);
            }
            return BllPub.Instance.DtToDictList(dt);
        }
        //导入,返回dt
        public static DataTable ExcelToDT(Stream sm, string filename)
        {
            IWorkbook workbook = null;
            var dt = new DataTable();
            var f = filename.Split(".");
            if (f.Length != 2) return null;
            if (f[1] == "xls")
            {
                workbook = new HSSFWorkbook(sm);
            }
            else
            {
                workbook = new XSSFWorkbook(sm);
            }

            var sheet = workbook.GetSheetAt(0);

            //列头
            foreach (var item in sheet.GetRow(sheet.FirstRowNum).Cells)
            {
                string colName = item.ToString();
                if (dt.Columns.Contains(colName))
                {
                    colName += item.ColumnIndex.ToString();
                }
                dt.Columns.Add(colName, typeof(string));
            }

            //写入内容
            var rows = sheet.GetRowEnumerator();
            IRow row = null;
            while (rows.MoveNext())
            {
                if (f[1] == "xls")
                {
                    row = (HSSFRow)rows.Current;
                }
                else
                {
                    row = (XSSFRow)rows.Current;
                }
                if (row.RowNum == sheet.FirstRowNum)
                {
                    continue;
                }

                var dr = dt.NewRow();
                foreach (var item in row.Cells)
                {
                    if (item.ColumnIndex >= dt.Columns.Count)
                    {
                        break;
                    }
                    if (item == null)
                    {
                        dr[item.ColumnIndex] = string.Empty;
                        continue;
                    }
                    switch (item.CellType)
                    {

                        case CellType.Boolean:
                            dr[item.ColumnIndex] = item.BooleanCellValue;
                            break;
                        case CellType.Error:
                            dr[item.ColumnIndex] = ErrorEval.GetText(item.ErrorCellValue);
                            break;
                        case CellType.Formula:
                            switch (item.CachedFormulaResultType)
                            {
                                case CellType.Boolean:
                                    dr[item.ColumnIndex] = item.BooleanCellValue;
                                    break;
                                case CellType.Error:
                                    dr[item.ColumnIndex] = ErrorEval.GetText(item.ErrorCellValue);
                                    break;
                                case CellType.Numeric:
                                    if (DateUtil.IsCellDateFormatted(item))
                                    {
                                        dr[item.ColumnIndex] = item.DateCellValue.ToString("yyyy-MM-dd hh:MM:ss");
                                    }
                                    else
                                    {
                                        dr[item.ColumnIndex] = item.NumericCellValue;
                                    }
                                    break;
                                case CellType.String:
                                    var str = item.StringCellValue;
                                    if (!string.IsNullOrEmpty(str))
                                    {
                                        dr[item.ColumnIndex] = str;
                                    }
                                    else
                                    {
                                        dr[item.ColumnIndex] = string.Empty;
                                    }
                                    break;
                                case CellType.Unknown:
                                case CellType.Blank:
                                default:
                                    dr[item.ColumnIndex] = string.Empty;
                                    break;
                            }
                            break;
                        case CellType.Numeric:
                            if (DateUtil.IsCellDateFormatted(item))
                            {
                                dr[item.ColumnIndex] = item.DateCellValue.ToString("yyyy-MM-dd hh:MM:ss");
                            }
                            else
                            {
                                dr[item.ColumnIndex] = item.NumericCellValue;
                            }
                            break;
                        case CellType.String:
                            var strValue = item.StringCellValue;
                            if (!string.IsNullOrEmpty(strValue))
                            {
                                dr[item.ColumnIndex] = strValue;
                            }
                            else
                            {
                                dr[item.ColumnIndex] = string.Empty;
                            }
                            break;
                        case CellType.Unknown:
                        case CellType.Blank:
                        default:
                            dr[item.ColumnIndex] = string.Empty;
                            break;
                    }
                }
                dt.Rows.Add(dr);
            }
            return dt;
        }

        /// <summary>
        /// 将datatable导出到excel并输出浏览器端直接下载
        /// </summary>
        /// <param name="dt">数据源</param>
        /// <param name="isExcel2003">是不是输出2003的xls格式，否则输出2007的xlsx格式</param>
        public static Stream Export(DataTable dt, bool isExcel2003 = true)
        {
            var ms = isExcel2003 ? RenderToExcel2003(dt) : RenderToExcel2007(dt);
            //输出
            return ms;
        }
        
        public static Stream Export(DataTable dt, List<CellRangeAddressModel> cramlist,
            bool isExcel2003 = true)
        {
            var ms = isExcel2003 ? RenderToExcel2003(dt, cramlist) : RenderToExcel2007(dt);
            return ms;
        }

        /// <summary>
        ///     将对象列表导出到excel，并输出浏览器端直接下载
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="modelList">对象列表</param>
        /// <param name="isExcel2003">是不是输出2003的xls格式，否则输出2007的xlsx格式</param>
        public static Stream Export<T>(List<T> modelList, bool isExcel2003 = true) where T : class
        {
            var ms = isExcel2003 ? RenderToExcel2003(modelList) : RenderToExcel2007(modelList);
            //输出
            return ms;
        }

        /// <summary>
        ///     将动态对象列表导出到excel，并输出浏览器端直接下载
        /// </summary>
        /// <param name="obj">对象列表，必须是多条数据，单一对象将报错</param>
        /// <param name="isExcel2003">是不是输出2003的xls格式，否则输出2007的xlsx格式</param>
        public static Stream Export(dynamic obj, bool isExcel2003 = true)
        {
            var ms = isExcel2003 ? (MemoryStream)RenderToExcel2003(obj) : (MemoryStream)RenderToExcel2007(obj);
            //输出
            return ms;
        }

        public static Stream Export(dynamic obj, List<CellRangeAddressModel> cramlist, bool isExcel2003 = true)
        {
            var ms = isExcel2003 ? (MemoryStream)RenderToExcel2003(obj, cramlist) : (MemoryStream)RenderToExcel2007(obj, cramlist);
            return ms;
        }

        /// <summary>
        ///     Excel文件导成Datatable
        /// </summary>
        /// <param name="strFilePath">Excel文件目录地址</param>
        /// <param name="strTableName">Datatable表名</param>
        /// <param name="iSheetIndex">Excel sheet index</param>
        /// <returns></returns>
        public static DataTable ExcelToDataTable(string strFilePath, string strTableName, int iSheetIndex)
        {
            var strExtName = Path.GetExtension(strFilePath);

            var dt = new DataTable();
            if (!string.IsNullOrEmpty(strTableName))
            {
                dt.TableName = strTableName;
            }

            if (strExtName.Equals(".xls"))
            {
                using (var file = new FileStream(strFilePath, FileMode.Open, FileAccess.Read))
                {
                    var workbook = new HSSFWorkbook(file);
                    var sheet = workbook.GetSheetAt(iSheetIndex);

                    //列头
                    foreach (var item in sheet.GetRow(sheet.FirstRowNum).Cells)
                    {
                        dt.Columns.Add(item.ToString(), typeof(string));
                    }

                    //写入内容
                    var rows = sheet.GetRowEnumerator();
                    while (rows.MoveNext())
                    {
                        IRow row = (HSSFRow)rows.Current;
                        if (row.RowNum == sheet.FirstRowNum)
                        {
                            continue;
                        }

                        var dr = dt.NewRow();
                        foreach (var item in row.Cells)
                        {
                            switch (item.CellType)
                            {
                                case CellType.Boolean:
                                    dr[item.ColumnIndex] = item.BooleanCellValue;
                                    break;
                                case CellType.Error:
                                    dr[item.ColumnIndex] = ErrorEval.GetText(item.ErrorCellValue);
                                    break;
                                case CellType.Formula:
                                    switch (item.CachedFormulaResultType)
                                    {
                                        case CellType.Boolean:
                                            dr[item.ColumnIndex] = item.BooleanCellValue;
                                            break;
                                        case CellType.Error:
                                            dr[item.ColumnIndex] = ErrorEval.GetText(item.ErrorCellValue);
                                            break;
                                        case CellType.Numeric:
                                            if (DateUtil.IsCellDateFormatted(item))
                                            {
                                                dr[item.ColumnIndex] = item.DateCellValue.ToString("yyyy-MM-dd hh:MM:ss");
                                            }
                                            else
                                            {
                                                dr[item.ColumnIndex] = item.NumericCellValue;
                                            }
                                            break;
                                        case CellType.String:
                                            var str = item.StringCellValue;
                                            if (!string.IsNullOrEmpty(str))
                                            {
                                                dr[item.ColumnIndex] = str;
                                            }
                                            else
                                            {
                                                dr[item.ColumnIndex] = null;
                                            }
                                            break;
                                        case CellType.Unknown:
                                        case CellType.Blank:
                                        default:
                                            dr[item.ColumnIndex] = string.Empty;
                                            break;
                                    }
                                    break;
                                case CellType.Numeric:
                                    if (DateUtil.IsCellDateFormatted(item))
                                    {
                                        dr[item.ColumnIndex] = item.DateCellValue.ToString("yyyy-MM-dd hh:MM:ss");
                                    }
                                    else
                                    {
                                        dr[item.ColumnIndex] = item.NumericCellValue;
                                    }
                                    break;
                                case CellType.String:
                                    var strValue = item.StringCellValue;
                                    if (!string.IsNullOrEmpty(strValue))
                                    {
                                        dr[item.ColumnIndex] = strValue;
                                    }
                                    else
                                    {
                                        dr[item.ColumnIndex] = null;
                                    }
                                    break;
                                case CellType.Unknown:
                                case CellType.Blank:
                                default:
                                    dr[item.ColumnIndex] = string.Empty;
                                    break;
                            }
                        }
                        dt.Rows.Add(dr);
                    }
                }
            }
            return dt;
        }

        /// <summary>
        /// 上传Excel文件导成DataTable
        /// </summary>
        /// <param name="sm"></param>
        /// <param name="filename"></param>
        /// <param name="p_tableName"></param>
        /// <param name="p_sheetIndex"></param>
        /// <returns></returns>
        public static DataTable ExcelToDataTable(Stream sm, string p_tableName, int p_sheetIndex, bool isExcel2003 = true)
        {
            var strExtName = isExcel2003 ? ".xls" : ".xlsx";
            var dt = new DataTable();
            if (!string.IsNullOrEmpty(p_tableName))
            {
                dt.TableName = p_tableName;
            }
            IWorkbook workbook = null;
            if (strExtName.Equals(".xls"))
            {
                workbook = new HSSFWorkbook(sm);
            }
            else if (strExtName.Equals(".xlsx"))
            {
                workbook = new XSSFWorkbook(sm);
            }



            var sheet = workbook.GetSheetAt(p_sheetIndex);

            //列头
            foreach (var item in sheet.GetRow(sheet.FirstRowNum).Cells)
            {
                string colName = item.ToString();
                if (dt.Columns.Contains(colName))
                {
                    colName += item.ColumnIndex.ToString();
                }
                dt.Columns.Add(colName, typeof(string));
            }

            //写入内容
            var rows = sheet.GetRowEnumerator();
            IRow row = null;
            while (rows.MoveNext())
            {
                if (strExtName.Equals(".xls"))
                {
                    row = (HSSFRow)rows.Current;
                }
                else
                {
                    row = (XSSFRow)rows.Current;
                }
                if (row.RowNum == sheet.FirstRowNum)
                {
                    continue;
                }

                var dr = dt.NewRow();
                foreach (var item in row.Cells)
                {
                    if (item.ColumnIndex >= dt.Columns.Count)
                    {
                        break;
                    }
                    if (item == null)
                    {
                        dr[item.ColumnIndex] = string.Empty;
                        continue;
                    }
                    switch (item.CellType)
                    {

                        case CellType.Boolean:
                            dr[item.ColumnIndex] = item.BooleanCellValue;
                            break;
                        case CellType.Error:
                            dr[item.ColumnIndex] = ErrorEval.GetText(item.ErrorCellValue);
                            break;
                        case CellType.Formula:
                            switch (item.CachedFormulaResultType)
                            {
                                case CellType.Boolean:
                                    dr[item.ColumnIndex] = item.BooleanCellValue;
                                    break;
                                case CellType.Error:
                                    dr[item.ColumnIndex] = ErrorEval.GetText(item.ErrorCellValue);
                                    break;
                                case CellType.Numeric:
                                    if (DateUtil.IsCellDateFormatted(item))
                                    {
                                        dr[item.ColumnIndex] = item.DateCellValue.ToString("yyyy-MM-dd hh:MM:ss");
                                    }
                                    else
                                    {
                                        dr[item.ColumnIndex] = item.NumericCellValue;
                                    }
                                    break;
                                case CellType.String:
                                    var str = item.StringCellValue;
                                    if (!string.IsNullOrEmpty(str))
                                    {
                                        dr[item.ColumnIndex] = str;
                                    }
                                    else
                                    {
                                        dr[item.ColumnIndex] = string.Empty;
                                    }
                                    break;
                                case CellType.Unknown:
                                case CellType.Blank:
                                default:
                                    dr[item.ColumnIndex] = string.Empty;
                                    break;
                            }
                            break;
                        case CellType.Numeric:
                            if (DateUtil.IsCellDateFormatted(item))
                            {
                                dr[item.ColumnIndex] = item.DateCellValue.ToString("yyyy-MM-dd hh:MM:ss");
                            }
                            else
                            {
                                dr[item.ColumnIndex] = item.NumericCellValue;
                            }
                            break;
                        case CellType.String:
                            var strValue = item.StringCellValue;
                            if (!string.IsNullOrEmpty(strValue))
                            {
                                dr[item.ColumnIndex] = strValue;
                            }
                            else
                            {
                                dr[item.ColumnIndex] = string.Empty;
                            }
                            break;
                        case CellType.Unknown:
                        case CellType.Blank:
                        default:
                            dr[item.ColumnIndex] = string.Empty;
                            break;
                    }
                }
                dt.Rows.Add(dr);
            }
            return dt;
        }

       
        #endregion 公有方法


        #region 私有方法
        private static byte[] RenderToExcelByte2003(DataTable dt)
        {
            var ms = new MemoryStream();
            using (dt)
            {
                IWorkbook workbook = new HSSFWorkbook(); //创建工作簿
                var sheet1 = workbook.CreateSheet("sheet1"); //创建工作表
                var headerRow = sheet1.CreateRow(0);

                foreach (DataColumn column in dt.Columns)
                {
                    headerRow.CreateCell(column.Ordinal).SetCellValue(column.Caption);
                }

                //If Caption not set, returns the ColumnName value

                var rowIndex = 1;

                foreach (DataRow row in dt.Rows)
                {
                    var dataRow = sheet1.CreateRow(rowIndex);

                    foreach (DataColumn column in dt.Columns)
                    {
                        dataRow.CreateCell(column.Ordinal).SetCellValue(row[column].ToString());
                    }

                    rowIndex++;
                }

                workbook.Write(ms);
                ms.Flush();
                ms.Position = 0;
            }
            return ms.GetBuffer();
        }
        private static byte[] RenderToExcelByte2007(DataTable dt)
        {
            var xssfworkbook = new XSSFWorkbook();
            var sheet = xssfworkbook.CreateSheet("sheet1");

            //表头  
            var row = sheet.CreateRow(0);
            for (var i = 0; i < dt.Columns.Count; i++)
            {
                var cell = row.CreateCell(i);
                cell.SetCellValue(dt.Columns[i].ColumnName);
            }

            //数据  
            for (var i = 0; i < dt.Rows.Count; i++)
            {
                var row1 = sheet.CreateRow(i + 1);
                for (var j = 0; j < dt.Columns.Count; j++)
                {
                    var cell = row1.CreateCell(j);
                    cell.SetCellValue(dt.Rows[i][j].ToString());
                }
            }

            //转为字节数组  
            var stream = new MemoryStream();
            xssfworkbook.Write(stream);
            return stream.GetBuffer();
        }



        /// <summary>
        /// 将dt转成excel2003文档的格式并输入字节流
        /// </summary>
        /// <param name="dt">数据源</param>
        /// <returns></returns>
        private static MemoryStream RenderToExcel2003(DataTable dt)
        {
            var ms = new MemoryStream();
            using (dt)
            {
                IWorkbook workbook = new HSSFWorkbook(); //创建工作簿
                var sheet1 = workbook.CreateSheet("sheet1"); //创建工作表
                var headerRow = sheet1.CreateRow(0);

                foreach (DataColumn column in dt.Columns)
                {
                    headerRow.CreateCell(column.Ordinal).SetCellValue(column.Caption);
                }

                //If Caption not set, returns the ColumnName value

                var rowIndex = 1;

                foreach (DataRow row in dt.Rows)
                {
                    var dataRow = sheet1.CreateRow(rowIndex);

                    foreach (DataColumn column in dt.Columns)
                    {
                        dataRow.CreateCell(column.Ordinal).SetCellValue(row[column].ToString());
                    }

                    rowIndex++;
                }

                workbook.Write(ms);
                ms.Flush();
                ms.Position = 0;
            }
            return ms;
        }
        
        /// <summary>
        /// 导出 EXCEL
        /// </summary>
        /// <param name="dt">数据源</param>
        /// <param name="cramlist">要合并的信息</param>
        /// <returns></returns>
        private static MemoryStream RenderToExcel2003(DataTable dt, List<CellRangeAddressModel> cramlist)
        {
            var ms = new MemoryStream();
            using (dt)
            {
                IWorkbook workbook = new HSSFWorkbook(); //创建工作簿
                var sheet1 = workbook.CreateSheet("sheet1"); //创建工作表
                var headerRow = sheet1.CreateRow(0);
                foreach (DataColumn column in dt.Columns)
                {
                    headerRow.CreateCell(column.Ordinal).SetCellValue(column.Caption);
                }
                //创建合并行或者列的信息
                if (cramlist != null & cramlist.Any())
                {
                    foreach (CellRangeAddressModel cram in cramlist)
                    {
                        sheet1.AddMergedRegion(new CellRangeAddress(cram.StartRow, cram.EndRow, cram.StartCol,
                            cram.EndCol));
                    }
                }

                //If Caption not set, returns the ColumnName value

                var rowIndex = 1;

                foreach (DataRow row in dt.Rows)
                {
                    var dataRow = sheet1.CreateRow(rowIndex);

                    foreach (DataColumn column in dt.Columns)
                    {
                        dataRow.CreateCell(column.Ordinal).SetCellValue(row[column].ToString());
                    }

                    rowIndex++;
                }

                workbook.Write(ms);
                ms.Flush();
                ms.Position = 0;
            }
            return ms;
        }

        /// <summary>
        /// 将dt转成excel2007文档的格式并输入字节流
        /// </summary>
        /// <param name="dt"></param>
        private static MemoryStream RenderToExcel2007(DataTable dt)
        {
            var xssfworkbook = new XSSFWorkbook();
            var sheet = xssfworkbook.CreateSheet("sheet1");

            //表头  
            var row = sheet.CreateRow(0);
            for (var i = 0; i < dt.Columns.Count; i++)
            {
                var cell = row.CreateCell(i);
                cell.SetCellValue(dt.Columns[i].ColumnName);
            }

            //数据  
            for (var i = 0; i < dt.Rows.Count; i++)
            {
                var row1 = sheet.CreateRow(i + 1);
                for (var j = 0; j < dt.Columns.Count; j++)
                {
                    var cell = row1.CreateCell(j);
                    cell.SetCellValue(dt.Rows[i][j].ToString());
                }
            }

            //转为字节数组  
            var stream = new MemoryStream();
            xssfworkbook.Write(stream);
            //var bytes = BllPub.Instance.StreamToBytes(stream);
            //stream.Close();
            
            var _stream = new MemoryStream();
            var bb = stream.GetBuffer();
            //stream.CopyTo(_stream);
            //var aa = _stream.Length;
            return stream;
        }

        private static MemoryStream RenderToExcel2007(DataTable dt, List<CellRangeAddressModel> cramlist)
        {
            var xssfworkbook = new XSSFWorkbook();
            var sheet = xssfworkbook.CreateSheet("sheet1");

            //表头  
            var row = sheet.CreateRow(0);
            for (var i = 0; i < dt.Columns.Count; i++)
            {
                var cell = row.CreateCell(i);
                cell.SetCellValue(dt.Columns[i].ColumnName);
            }
            //创建合并行或者列的信息
            if (cramlist != null & cramlist.Any())
            {
                foreach (CellRangeAddressModel cram in cramlist)
                {
                    sheet.AddMergedRegion(new CellRangeAddress(cram.StartRow, cram.EndRow, cram.StartCol,
                        cram.EndCol));
                }
            }

            //数据  
            for (var i = 0; i < dt.Rows.Count; i++)
            {
                var row1 = sheet.CreateRow(i + 1);
                for (var j = 0; j < dt.Columns.Count; j++)
                {
                    var cell = row1.CreateCell(j);
                    cell.SetCellValue(dt.Rows[i][j].ToString());
                }
            }

            //转为字节数组  
            var stream = new MemoryStream();
            xssfworkbook.Write(stream);
            return stream;
        }

        /// <summary>
        /// 将对象转成excel2003文档的格式并输入字节流
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="modelList">对象</param>
        /// <returns></returns>
        private static MemoryStream RenderToExcel2003<T>(List<T> modelList) where T : class
        {
            var ms = new MemoryStream();
            IWorkbook workbook = new HSSFWorkbook(); //创建工作簿
            var sheet1 = workbook.CreateSheet("sheet1"); //创建工作表
            var headerRow = sheet1.CreateRow(0);
            //反射读取对象的成员
            var properts = Activator.CreateInstance(typeof(T)).GetType().GetProperties();
            for (var i = 0; i < properts.Length; i++)
            {
                headerRow.CreateCell(i).SetCellValue(properts[i].Name);
                //If Caption not set, returns the ColumnName value
            }

            var rowIndex = 1;

            foreach (var row in modelList)
            {
                var dataRow = sheet1.CreateRow(rowIndex);

                for (var i = 0; i < properts.Length; i++)
                {
                    dataRow.CreateCell(i).SetCellValue(properts[i].GetValue(row, null).ToString());
                }

                rowIndex++;
            }

            workbook.Write(ms);
            ms.Flush();
            ms.Position = 0;
            return ms;
        }

        /// <summary>
        /// 将对象转成excel2007文档的格式并输入字节流
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="modelList">对象</param>
        /// <returns></returns>
        private static MemoryStream RenderToExcel2007<T>(List<T> modelList)
        {
            var xssfworkbook = new XSSFWorkbook();
            var sheet = xssfworkbook.CreateSheet("sheet1");

            //表头  
            var headerRow = sheet.CreateRow(0);
            //反射读取对象的成员
            var properts = Activator.CreateInstance(typeof(T)).GetType().GetProperties();
            for (var i = 0; i < properts.Length; i++)
            {
                headerRow.CreateCell(i).SetCellValue(properts[i].Name);
                //If Caption not set, returns the ColumnName value
            }

            //数据 
            var rowIndex = 1;
            foreach (var row in modelList)
            {
                var dataRow = sheet.CreateRow(rowIndex);

                for (var i = 0; i < properts.Length; i++)
                {
                    dataRow.CreateCell(i).SetCellValue(properts[i].GetValue(row, null).ToString());
                }

                rowIndex++;
            }
            //转为字节数组  
            var stream = new MemoryStream();
            xssfworkbook.Write(stream);
            return stream;
        }

        /// <summary>
        /// 将动态对象转成excel2003文档的格式并输入字节流
        /// </summary>
        /// <param name="obj">对象</param>
        /// <returns></returns>
        private static MemoryStream RenderToExcel2003(dynamic obj)
        {
            var ms = new MemoryStream();
            IWorkbook workbook = new HSSFWorkbook(); //创建工作簿
            var sheet1 = workbook.CreateSheet("sheet1"); //创建工作表
            var headerRow = sheet1.CreateRow(0);
            string json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);

            var diclist =
                JsonConvert.DeserializeObject<List<IDictionary<string, object>>>(json);

            var dic = diclist.FirstOrDefault();
            var k = 0;
            foreach (var key in dic)
            {
                headerRow.CreateCell(k).SetCellValue(key.Key); //If Caption not set, returns the ColumnName value
                k++;
            }

            var rowIndex = 1;

            foreach (var dictionary in diclist)
            {
                var dataRow = sheet1.CreateRow(rowIndex);

                var r = 0;
                foreach (var str in dictionary.Keys)
                {
                    var s = "";
                    if (dictionary[str] != null)
                    {
                        s = dictionary[str].ToString();
                    }
                    dataRow.CreateCell(r).SetCellValue(s);
                    r++;
                }

                rowIndex++;
            }

            workbook.Write(ms);
            ms.Flush();
            ms.Position = 0;
            return ms;
        }

        private static MemoryStream RenderToExcel2003(dynamic obj, List<CellRangeAddressModel> cramlist)
        {
            var ms = new MemoryStream();
            IWorkbook workbook = new HSSFWorkbook(); //创建工作簿
            var sheet1 = workbook.CreateSheet("sheet1"); //创建工作表
            var headerRow = sheet1.CreateRow(0);
            string json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
            var diclist =
                JsonConvert.DeserializeObject<List<IDictionary<string, object>>>(json);

            var dic = diclist.FirstOrDefault();
            var k = 0;
            foreach (var key in dic)
            {
                headerRow.CreateCell(k).SetCellValue(key.Key); //If Caption not set, returns the ColumnName value
                k++;
            }
            //创建合并行或者列的信息
            if (cramlist != null & cramlist.Any())
            {
                foreach (CellRangeAddressModel cram in cramlist)
                {
                    sheet1.AddMergedRegion(new CellRangeAddress(cram.StartRow, cram.EndRow, cram.StartCol,
                        cram.EndCol));
                }
            }

            var rowIndex = 1;

            foreach (var dictionary in diclist)
            {
                var dataRow = sheet1.CreateRow(rowIndex);

                var r = 0;
                foreach (var str in dictionary.Keys)
                {
                    var s = "";
                    if (dictionary[str] != null)
                    {
                        s = dictionary[str].ToString();
                    }
                    dataRow.CreateCell(r).SetCellValue(s);
                    r++;
                }

                rowIndex++;
            }

            workbook.Write(ms);
            ms.Flush();
            ms.Position = 0;
            return ms;
        }

        /// <summary>
        /// 将动态对象转成excel2007文档的格式并输入字节流
        /// </summary>
        /// <param name="obj">对象</param>
        /// <returns></returns>
        private static MemoryStream RenderToExcel2007(dynamic obj, List<CellRangeAddressModel> cramlist)
        {
            var xssfworkbook = new XSSFWorkbook();
            var sheet = xssfworkbook.CreateSheet("sheet1");

            //表头  
            var headerRow = sheet.CreateRow(0);
            string json = JsonConvert.SerializeObject(obj);
            var diclist =
                JsonConvert.DeserializeObject<List<IDictionary<string, object>>>(json);

            var dic = diclist.FirstOrDefault();
            var k = 0;
            if (dic != null)
                foreach (var key in dic)
                {
                    headerRow.CreateCell(k).SetCellValue(key.Key); //If Caption not set, returns the ColumnName value
                    k++;
                }

            //创建合并行或者列的信息
            if (cramlist != null & cramlist.Any())
            {
                foreach (CellRangeAddressModel cram in cramlist)
                {
                    sheet.AddMergedRegion(new CellRangeAddress(cram.StartRow, cram.EndRow, cram.StartCol,
                        cram.EndCol));
                }
            }
            //数据 
            var rowIndex = 1;
            foreach (var dictionary in diclist)
            {
                var dataRow = sheet.CreateRow(rowIndex);

                var r = 0;
                foreach (var str in dictionary.Keys)
                {
                    var s = "";
                    if (dictionary[str] != null)
                    {
                        s = dictionary[str].ToString();
                    }
                    dataRow.CreateCell(r).SetCellValue(s);
                    r++;
                }

                rowIndex++;
            }
            //转为字节数组  
            var stream = new MemoryStream();
            xssfworkbook.Write(stream);
            return stream;
        }

        private static MemoryStream RenderToExcel2007(dynamic obj)
        {
            var xssfworkbook = new XSSFWorkbook();
            var sheet = xssfworkbook.CreateSheet("sheet1");

            //表头  
            var headerRow = sheet.CreateRow(0);
            string json = JsonConvert.SerializeObject(obj);
            var diclist =
                JsonConvert.DeserializeObject<List<IDictionary<string, object>>>(json);

            var dic = diclist.FirstOrDefault();
            var k = 0;
            if (dic != null)
                foreach (var key in dic)
                {
                    headerRow.CreateCell(k).SetCellValue(key.Key); //If Caption not set, returns the ColumnName value
                    k++;
                }

            //数据 
            var rowIndex = 1;
            foreach (var dictionary in diclist)
            {
                var dataRow = sheet.CreateRow(rowIndex);

                var r = 0;
                foreach (var str in dictionary.Keys)
                {
                    var s = "";
                    if (dictionary[str] != null)
                    {
                        s = dictionary[str].ToString();
                    }
                    dataRow.CreateCell(r).SetCellValue(s);
                    r++;
                }

                rowIndex++;
            }
            //转为字节数组  
            var stream = new MemoryStream();
            xssfworkbook.Write(stream);
            return stream;
        }



        #endregion 私有方法

    }

    /// <summary>
    /// 合并的信息对象 {0，1，0，1} 表示第一列和第二列的第一行、第二行合并，{0，0，0，1}表示第一列和第二列合并
    /// </summary>
    public class CellRangeAddressModel
    {
        /// <summary>
        /// 合并的起始行
        /// </summary>
        public int StartRow { get; set; } = 0;
        /// <summary>
        /// 合并的结束行
        /// </summary>
        public int EndRow { get; set; } = 0;
        /// <summary>
        /// 合并的起始列
        /// </summary>
        public int StartCol { get; set; } = 0;
        /// <summary>
        /// 合并的结束列
        /// </summary>
        public int EndCol { get; set; } = 0;
    }
}
