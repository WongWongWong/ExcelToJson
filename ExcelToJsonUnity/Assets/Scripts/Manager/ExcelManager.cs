using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Excel;
using System.IO;
using System.Data;
using LitJson;
using System.Text;
using System;

public class ExcelManager
{
    private static ExcelManager _ins = new ExcelManager();

    public static ExcelManager Ins { get { return _ins; } }

    public List<DataTable> GetSheetTable(string excelFile)
    {
        var ret = new List<DataTable>();

        // 打开文件
        FileStream fileStream = File.Open(excelFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        IExcelDataReader excelDataReader = ExcelReaderFactory.CreateOpenXmlReader(fileStream);
        //转成数组,遍历
        DataSet result = excelDataReader.AsDataSet();
        excelDataReader.Close();

        if (result != null)
        {
            var tables = result.Tables;

            for (int i = 0; i < tables.Count; i++)
            {
                var table = tables[i];
                table.Namespace = excelFile;
                ret.Add(table);
            }
        }

        return ret;
    }

}
