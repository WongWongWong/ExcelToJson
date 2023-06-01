using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ConfigPacking
{

    /// <summary>
    ///  表格数据
    /// </summary>
    public class LoData
    {
        public LoData()
        {
            notes = new List<string>();
            types = new List<string>();
            names = new List<string>();
            values = new List<Dictionary<string, string>>();
        }

        /// <summary>
        /// 表格路径
        /// </summary>
        public string fullPath;

        /// <summary>
        /// 表格名称
        /// </summary>
        public string excleName;

        /// <summary>
        /// 表名描述
        /// </summary>
        public string nameDesc;

        /// <summary>
        /// 表名
        /// </summary>
        public string name;

        /// <summary>
        /// 注释列表
        /// </summary>
        public List<string> notes;

        /// <summary>
        /// 成员数据类型
        /// </summary>
        public List<string> types;

        /// <summary>
        /// 成员名称
        /// </summary>
        public List<string> names;

        /// <summary>
        /// 行数据<成员名称,成员值>
        /// </summary>
        public List<Dictionary<string, string>> values;
    }
}