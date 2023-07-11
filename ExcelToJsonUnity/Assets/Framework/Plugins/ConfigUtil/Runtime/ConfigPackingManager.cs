using LitJson;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;

namespace ConfigPacking
{
    public static class ConfigPackingManager
    {
        public static event Action<string> messageCall;


        static string classTemplate = "" +
        "/// <summary> \n" +
        "/// Desc: $desc$\n" +
        "/// Excel: $filePath$\n" +
        "/// Author: ConfigTool\n" +
        "/// </summary>\n" +
        "public class $name$LO\n" +
        "{\n" +
        "$member$\n" +
        "}";

        static string memberTemplate = "" +
            "    /// <summary> \n" +
            "    /// $desc$\n" +
            "    /// </summary>\n" +
            "    public $type$ $name$;\n\n";

        static string langTemplate = "" +
            "    public string $name$ => LanguageManager.Instance.GetLanguageText($langId$);\n\n";

        /// <summary>
        /// 导出lo
        /// </summary>
        /// <param name="dict"></param>
        public static void ExprotLO(string excelPath, string exprotPath)
        {
            try
            {
                //创建文件夹
                FileManager.CreateDirPath(exprotPath);
            }
            catch
            {
                Message("文件夹创建失败,路径不合法!");
                return;
            }

            Dictionary<string, LoData> dict = null;
            try
            {
                dict = LoadExcel(excelPath);
            }
            catch (Exception ex)
            {
                Message("读取表格错误 信息:" + ex.Message);
                return;
            }


            try
            {
                foreach (string key in dict.Keys)
                {
                    var data = dict[key];
                    var classStr = LoDataToLoString(data);

                    if (exprotPath[exprotPath.Length - 1] != '\\')
                    {
                        exprotPath += '\\';
                    }
                    FileManager.ExportFile(classStr, exprotPath + data.name + "LO.cs");
                }

                Message("LO导出成功");
            }
            catch (Exception ex)
            {
                Message("LO导出错误 信息:" + ex.Message);
                return;
            }

        }

        /// <summary>
        /// 导出json
        /// </summary>
        /// <param name="excelPath"></param>
        /// <param name="exprotPath"></param>
        public static void ExprotJson(string excelPath, string exprotPath)
        {

            try
            {
                //创建文件夹
                //FileManager.Ins.CreateDirPath(exprotPath.Substring(0, exprotPath.LastIndexOf("\\") + 1));

                if (exprotPath.IndexOf("\\") != -1)
                {
                    FileManager.CreateDirPath(exprotPath.Substring(0, exprotPath.LastIndexOf("\\") + 1));
                }
                else
                {
                    var test1 = exprotPath.LastIndexOf("/");
                    string test = exprotPath.Substring(0, 10);
                    FileManager.CreateDirPath(exprotPath.Substring(0, exprotPath.LastIndexOf("/") + 1));
                }
            }
            catch
            {
                Message("文件夹创建失败,路径不合法!");
                return;
            }

            Dictionary<string, LoData> dict = null;
            try
            {
                //转化本地数据结构,创建表格字典
                dict = LoadExcel(excelPath);
            }
            catch (Exception ex)
            {
                Message("读取表格错误 信息:" + ex.Message);
                return;
            }

            string jsonStr;
            if (Path.HasExtension(exprotPath))
            {
                // 输出到单个文件里
                try
                {
                    //转成json字符串
                    jsonStr = ToJson(dict);
                }
                catch (Exception ex)
                {
                    Message("表格转成json字符串错误 信息:" + ex.Message);
                    return;
                }
                try
                {
                    //输出json文件
                    FileManager.ExportFile(jsonStr, exprotPath);
                    Message("JSON导出成功!");
                }
                catch (Exception ex)
                {
                    Message("表格转成json错误 信息:" + ex.Message);
                    return;
                }
            }
            else
            {
                // 输出到多个文件
                Dictionary<string, string> jsonDict = null;
                try
                {
                    //转成json字符串
                    jsonDict = ToJsonDict(dict);
                }
                catch (Exception ex)
                {
                    Message("表格转成json字符串错误 信息:" + ex.Message);
                    return;
                }

                // 输出到多个文件里
                foreach (var configName in jsonDict.Keys)
                {
                    var fullPath = exprotPath + configName + ".json";
                    jsonStr = jsonDict[configName];
                    try
                    {
                        //输出json文件
                        FileManager.ExportFile(jsonStr, fullPath);
                        Message(configName + "导出成功!");
                    }
                    catch (Exception ex)
                    {
                        Message(configName + "表格转成json错误 信息:" + ex.Message);
                        return;
                    }
                }
            }
        }

        /// <summary>
        /// 导出服务端json
        /// </summary>
        /// <param name="excelPath"></param>
        /// <param name="exprotPath"></param>
        public static void ExprotServerJson(string excelPath, string exprotPath)
        {

            try
            {
                //创建文件夹
                //FileManager.Ins.CreateDirPath(exprotPath.Substring(0, exprotPath.LastIndexOf("\\") + 1));
                FileManager.CreateDirPath(exprotPath);
            }
            catch
            {
                Message("文件夹创建失败,路径不合法!");
                return;
            }

            Dictionary<string, LoData> dict = null;
            Dictionary<string, string> jsonDict = null;
            try
            {
                //转化本地数据结构,创建表格字典
                dict = LoadExcel(excelPath, true);
            }
            catch (Exception ex)
            {
                Message("读取表格错误 信息:" + ex.Message);
                return;
            }

            try
            {
                //转成json字符串
                jsonDict = ToJsonDict(dict);
            }
            catch (Exception ex)
            {
                Message("表格转成json字符串错误 信息:" + ex.Message);
                return;
            }


            if (exprotPath[exprotPath.Length - 1] != '\\')
            {
                exprotPath += '\\';
            }

            foreach (var configName in jsonDict.Keys)
            {
                var fullPath = exprotPath + configName + ".json";
                var jsonStr = jsonDict[configName];
                try
                {
                    //输出json文件
                    FileManager.ExportFile(jsonStr, fullPath);
                    Message(configName + "导出成功!");
                }
                catch (Exception ex)
                {
                    Message(configName + "表格转成json错误 信息:" + ex.Message);
                    return;
                }
            }

            //try
            //{
            //    //输出json文件
            //    FileManager.Ins.ExportFile(jsonStr, exprotPath);
            //    Message("JSON导出成功!");
            //}
            //catch (Exception ex)
            //{
            //    Message("表格转成json错误 信息:" + ex.Message);
            //    return;
            //}

        }

        /// <summary>
        /// 把表数据解析成json
        /// </summary>
        /// <param name="dict"></param>
        /// <returns></returns>
        public static string ToJson(Dictionary<string, LoData> dict)
        {
            //var jsonDict = new Dictionary<string, Dictionary<string, string>>();
            var jsonDict = JsonMapper.ToObject("{}") as IDictionary;

            foreach (string configName in dict.Keys)
            {
                //jsonDict.Add(configName, new Dictionary<string, string>());
                jsonDict.Add(configName, JsonMapper.ToObject("{}"));
                var configDict = jsonDict[configName] as IDictionary;

                var loData = dict[configName];
                var jdList = LoDataToJsonDataList(loData);

                for (int i = 0; i < jdList.Count; i++)
                {
                    int rowNum = i + 1;
                    configDict.Add(rowNum.ToString(), jdList[i]);
                }
            }

            var str = JsonMapper.ToJson(jsonDict);
            //这句代码是为了解决unicode 转 中文问题
            str = System.Text.RegularExpressions.Regex.Unescape(str);
            return str;
        }

        public static List<JsonData> LoDataToJsonDataList(LoData loData)
        {
            List<JsonData> ret = new List<JsonData>();

            var values = loData.values;
            for (int i = 0; i < values.Count; i++)
            {
                //configDict.Add(rowNum.ToString(), new JsonData());
                var jd = JsonMapper.ToObject("{}");

                var rowValue = values[i];
                foreach (string name in rowValue.Keys)
                {
                    var typeIndex = loData.names.IndexOf(name);
                    var type = loData.types[typeIndex];
                    var valueStr = rowValue[name];

                    int int_val;
                    long long_val;
                    double float_val;
                    bool isInt;
                    bool isFloat;

                    switch (type)
                    {
                        case "int": // int32
                        case "lang": // 多语言
                            isInt = int.TryParse(valueStr, out int_val);
                            jd[name] = isInt ? int_val : 0;
                            break;
                        case "long": // int64
                            isInt = long.TryParse(valueStr, out long_val);
                            jd[name] = isInt ? long_val : 0;
                            break;

                        case "string":
                            if (valueStr != null)
                            {
                                jd[name] = valueStr.Replace("\"", "\\\"");
                            }
                            break;

                        case "float":
                        case "double":
                            isInt = int.TryParse(valueStr, out int_val);
                            if (isInt)
                            {
                                jd[name] = int_val;
                            }
                            else
                            {
                                isFloat = double.TryParse(valueStr, out float_val);
                                if (isFloat)
                                {
                                    jd[name] = float_val;
                                }
                                else
                                {
                                    jd[name] = 0;
                                }
                            }
                            break;

                        case "bool":
                            if (valueStr == "true" || valueStr == "TRUE" || valueStr == "1")
                            {
                                jd[name] = true;
                            }
                            else
                            {
                                jd[name] = false;
                            }
                            break;
                    }
                }
                ret.Add(jd);
            }

            return ret;
        }

        public static Dictionary<string, string> ToJsonDict(Dictionary<string, LoData> dict)
        {
            var ret = new Dictionary<string, string>();
            foreach (var configName in dict.Keys)
            {
                var loData = dict[configName];
                var jdList = LoDataToJsonDataList(loData);

                var jd = JsonMapper.ToObject("[]");
                var ary = jd as IList;
                for (int i = 0; i < jdList.Count; i++)
                {
                    ary.Add(jdList[i]);
                }

                var str = JsonMapper.ToJson(ary);
                //这句代码是为了解决unicode 转 中文问题
                str = System.Text.RegularExpressions.Regex.Unescape(str);
                ret.Add(loData.name, str);
            }

            return ret;
        }


        /// <summary>
        /// LoData转Lo类字符串
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string LoDataToLoString(LoData data)
        {
            var memberStr = "";
            for (int i = 0; i < data.names.Count; i++)
            {
                var newMemberStr = memberTemplate;
                if (string.IsNullOrEmpty(data.names[i]))
                {
                    continue;
                }

                var noteStr = data.notes[i].Replace("\n", "///");
                newMemberStr = newMemberStr.Replace("$desc$", noteStr);

                var memberType = data.types[i];
                switch (memberType)
                {
                    // 生成多语言函数
                    case "lang":
                        newMemberStr = newMemberStr.Replace("$type$", "int");
                        newMemberStr = newMemberStr.Replace("$name$", data.names[i]);

                        string langStr = langTemplate.Replace("$name$", $"{data.names[i]}Lang");
                        langStr = langStr.Replace("$langId$", $"{data.names[i]}");

                        newMemberStr += langStr;
                        break;
                    //默认直接表格上填写的类型定义,特殊的另外写处理解析
                    default:
                        newMemberStr = newMemberStr.Replace("$type$", data.types[i]);
                        newMemberStr = newMemberStr.Replace("$name$", data.names[i]);
                        break;
                }

                //newMemberStr = newMemberStr.Replace("$type$", data.types[i]);
                memberStr += newMemberStr;
            }

            var classStr = classTemplate.Replace("$desc$", data.nameDesc).Replace("$name$", data.name).Replace("$filePath$", data.excleName);
            classStr = classStr.Replace("$member$", memberStr);

            return classStr;
        }


        /// <summary>
        /// 加载excel
        /// </summary>
        /// <param name="path"></param>
        public static Dictionary<string, LoData> LoadExcel(string path, bool isServer = false)
        {
            //获得文件路径列表
            var fileList = FileManager.GetExcelFilePathList(path);

            //记录所有sheet的数据
            var sheetDatas = new List<DataTable>();
            for (int i = 0; i < fileList.Count; i++)
            {
                var newSheets = ExcelManager.GetSheetTable(fileList[i]);
                sheetDatas.AddRange(newSheets);
            }

            var dict = new Dictionary<string, LoData>();

            //把所有sheet转化成loData
            for (int i = 0; i < sheetDatas.Count; i++)
            {
                bool isEmpty = true;
                var loData = GetLoDataByTable(sheetDatas[i], isServer);
                if (loData != null)
                {
                    //检测是否有字段，如果都没有，代表这个表不需要到处
                    foreach (var name in loData.names)
                    {
                        if (!string.IsNullOrEmpty(name))
                        {
                            isEmpty = false;
                            break;
                        }
                    }
                    if (isEmpty)
                        continue;

                    if (dict.ContainsKey(loData.name))
                    {
                        //重复名字的表,做合并处理
                        var oldLoData = dict[loData.name];

                        //遍历老成员名称,看缺了哪个
                        for (int k = 0; k < loData.names.Count; k++)
                        {
                            string name = loData.names[k];
                            string type = loData.types[k];
                            if (string.IsNullOrEmpty(type)) continue;
                            if (string.IsNullOrEmpty(name)) continue;

                            //判断老表是否有该成员的基础记录
                            var nameIndex = oldLoData.names.IndexOf(name);
                            if (nameIndex == -1)
                            {
                                //添加相关数据
                                string note = "";
                                if (k < loData.types.Count) note = loData.notes[k];
                                oldLoData.notes.Add(note);
                                oldLoData.types.Add(type);
                                oldLoData.names.Add(name);
                            }
                        }

                        //合并值数据
                        oldLoData.values.AddRange(loData.values);
                    }
                    else
                    {
                        dict.Add(loData.name, loData);
                    }
                }
            }

            return dict;
        }

        /// <summary>
        /// 通过表格sheet获得一份lo数据
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        public static LoData GetLoDataByTable(DataTable table, bool isServer)
        {
            var loData = new LoData();
            loData.nameDesc = table.TableName;
            loData.fullPath = table.Namespace;
            loData.excleName = Path.GetFileName(loData.fullPath);

            int columns = table.Columns.Count;//列
            int rows = table.Rows.Count;//行


            for (int y = 0; y < rows; y++)
            {
                Dictionary<string, string> newValues = new Dictionary<string, string>();
                bool markLast = false;

                for (int x = 0; x < columns; x++)
                {

                    string value = table.Rows[y][x].ToString();
                    if (y == 0)
                    {
                        //第一行,记录表名,成员注释
                        if (x == 0)
                        {
                            //记录表名
                            loData.name = value;
                            //conficDict.Add(value, loData);
                        }
                        else
                        {
                            //都是成员注释
                            loData.notes.Add(value);
                        }
                    }
                    else if (y == 1)
                    {
                        //记录数据类型
                        if (x > 0)
                        {
                            loData.types.Add(value);
                        }
                    }
                    else if (y == 2)
                    {
                        //记录字段名
                        if (x == 0)
                        {
                            if (value != "CLIENT")
                            {
                                //这表表没有数据,不需要解释
                                return null;
                            }
                        }
                        else if (x > 0 && !isServer)
                        {
                            loData.names.Add(value);
                        }
                    }
                    else if (y == 3)
                    {
                        //服务端字段名
                        if (x == 0)
                        {
                            if (value != "SERVER")
                            {
                                //这表表没有数据,不需要解释
                                return null;
                            }
                        }
                        else if (x > 0 && isServer)
                        {
                            loData.names.Add(value);
                        }
                    }
                    else
                    {
                        if (x == 0)
                        {
                            if (value == "END")
                            {
                                markLast = true;
                            }
                            //values = new Dictionary<string, string>();
                            //loData.values.Add(new Dictionary<string, string>());
                        }
                        else
                        {
                            var name = loData.names[x - 1];
                            if (name.Length > 0)
                            {
                                try
                                {
                                    if (!string.IsNullOrEmpty(value))
                                    {
                                        newValues.Add(name, value);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Message("解析成员错误 报错信息:" + ex.Message + "sheet名称:" + table.TableName + "    字段名:" + name + "   值:" + value + "\n" +
                                        "value字典:" + JsonMapper.ToJson(newValues));
                                    //Debug.LogError("[*] Add name Error!\rDict:" + JsonMapper.ToJson(valueDect) + "\r name:" + name);
                                    return null;
                                }
                            }
                        }
                    }
                }

                if (newValues.Values.Count > 0)
                {
                    loData.values.Add(newValues);
                }

                if (markLast)
                {
                    break;
                }
            }

            return loData;
        }

        static void Message(string message)
        {
            messageCall?.Invoke(message);
        }
    }
}