using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Web;

namespace Max.BaseKit.Utils
{
    public class ExcelUtil
    {
        #region 导出Excel
        /// <summary>
        /// 导出Excel
        /// </summary>
        /// <param name="lists"></param>
        /// <param name="head">中文列名对照</param>
        /// <param name="workbookFile">保存路径</param>
        public static void LstToExcel<T>(List<T> lists, Hashtable head, string workbookFile)
        {
            try
            {
                HSSFWorkbook workbook = new HSSFWorkbook();
                using (MemoryStream ms = GetListToMemoryStream(lists, head, workbookFile))
                {
                    workbook.Write(ms);
                    ms.Flush();
                    ms.Position = 0;
                    workbook = null;
                    using (FileStream fs = new FileStream(workbookFile, FileMode.Create, FileAccess.Write))
                    {
                        byte[] data = ms.ToArray();
                        fs.Write(data, 0, data.Length);
                        fs.Flush();
                        fs.Close();
                    }
                }
            }
            catch (Exception ee)
            {
                string see = ee.Message;
            }
        }
        #endregion

        #region 导入Excel
        /// <summary>
        /// 导入Excel
        /// </summary>
        /// <param name="lists"></param>
        /// <param name="head">中文列名对照</param>
        /// <param name="workbookFile">Excel所在路径</param>
        /// <returns></returns>
        public static List<T> ExcelToList<T>(Hashtable head, string workbookFile)
        {
            try
            {
                HSSFWorkbook hssfworkbook;
                List<T> lists = new List<T>();
                using (FileStream file = new FileStream(workbookFile, FileMode.Open, FileAccess.Read))
                {
                    hssfworkbook = new HSSFWorkbook(file);
                }
                HSSFSheet sheet = hssfworkbook.GetSheetAt(0) as HSSFSheet;
                IEnumerator rows = sheet.GetRowEnumerator();
                HSSFRow headerRow = sheet.GetRow(0) as HSSFRow;
                int cellCount = headerRow.LastCellNum;
                PropertyInfo[] properties;
                T t = default(T);
                for (int i = sheet.FirstRowNum + 1; i <= sheet.LastRowNum; i++)
                {
                    HSSFRow row = sheet.GetRow(i) as HSSFRow;
                    t = Activator.CreateInstance<T>();
                    properties = t.GetType().GetProperties();
                    foreach (PropertyInfo column in properties)
                    {
                        int j = headerRow.Cells.FindIndex(delegate (ICell c)
                        {
                            return c.StringCellValue == (head[column.Name] == null ? column.Name : head[column.Name].ToString());
                        });
                        if (j >= 0 && row.GetCell(j) != null)
                        {
                            object value = valueType(column.PropertyType, row.GetCell(j).ToString());
                            column.SetValue(t, value, null);
                        }
                    }
                    lists.Add(t);
                }
                return lists;
            }
            catch (Exception ee)
            {
                string see = ee.Message;
                return null;
            }
        }
        #endregion

        static object valueType(Type t, string value)
        {
            object o = null;
            string strt = "String";
            if (t.Name == "Nullable`1")
            {
                strt = t.GetGenericArguments()[0].Name;
            }
            switch (strt)
            {
                case "Decimal":
                    o = decimal.Parse(value);
                    break;
                case "Int":
                    o = int.Parse(value);
                    break;
                case "Float":
                    o = float.Parse(value);
                    break;
                case "DateTime":
                    o = DateTime.Parse(value);
                    break;
                default:
                    o = value;
                    break;
            }
            return o;
        }

        #region RenderToListToExcel
        //public static void RenderToListToExcel<T>(List<T> lists, Hashtable head, string workbookFile, HttpContextBase context)
        //{
        //    using (MemoryStream ms = GetListToMemoryStream(lists, head, workbookFile))
        //    {
        //        RenderToBrowser(ms, context, workbookFile);
        //    }
        //}
        #endregion

        #region GetListToMemoryStream
        private static MemoryStream GetListToMemoryStream<T>(List<T> lists, Hashtable head, string workbookFile)
        {
            MemoryStream ms = new MemoryStream();
            try
            {
                HSSFWorkbook workbook = new HSSFWorkbook();
                int dtRowsCount = lists.Count;
                int SheetCount = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(dtRowsCount) / 65536));
                int SheetNum = 1;
                int rowIndex = 1;
                int tempIndex = 1; //标示 
                ISheet sheet = workbook.CreateSheet("sheet" + SheetNum);
                Type type = typeof(T);
                PropertyInfo[] properties = type.GetProperties();
                for (int i = 0; i < dtRowsCount; i++)
                {
                    if (i == 0 || tempIndex == 1)
                    {
                        IRow headerRow = sheet.CreateRow(0);

                        for (int j = 0; j < properties.Length; j++)
                        {
                            string name = properties[j].Name;
                            headerRow.CreateCell(j).SetCellValue(head[name] == null ? name : head[name].ToString());
                        }
                    }
                    HSSFRow dataRow = (HSSFRow)sheet.CreateRow(tempIndex);
                    for (int k = 0; k < properties.Length; k++)
                    {
                        PropertyInfo column = properties[k];
                        T item = lists[i];
                        dataRow.CreateCell(k).SetCellValue(column.GetValue(item, null) == null ? "" : column.GetValue(item, null).ToString());
                    }
                    if (tempIndex == 65535)
                    {
                        SheetNum++;
                        sheet = workbook.CreateSheet("sheet" + SheetNum);//
                        tempIndex = 0;
                    }
                    rowIndex++;
                    tempIndex++;
                }

                workbook.Write(ms);
            }
            catch (Exception ee)
            {
                string see = ee.Message;
            }
            return ms;
        }
        #endregion

        #region 输出文件到浏览器
        /// <summary>
        /// 输出文件到浏览器
        /// </summary>
        /// <param name="ms">Excel文档流</param>
        /// <param name="context">HTTP上下文</param>
        /// <param name="fileName">文件名</param>
        //private static void RenderToBrowser(MemoryStream ms, HttpContextBase context, string fileName)
        //{
        //    if (context.Request.Browser.Browser == "IE")
        //        fileName = HttpUtility.UrlEncode(fileName);
        //    context.Response.AddHeader("Content-Disposition", "attachment;fileName=" + fileName);
        //    context.Response.BinaryWrite(ms.ToArray());
        //}
        #endregion
    }
}
