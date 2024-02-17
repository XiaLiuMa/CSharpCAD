﻿using System;

namespace Max.DbTool.Mod
{
    /// <summary>
    /// 用于 Dapper Model类与数据库表字段的一一对应
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class ColumnAttribute : Attribute
    {
        private string _columnName;

        public ColumnAttribute(string columnName)
        {
            _columnName = columnName;
        }

        /// <summary>
        /// 列明
        /// </summary>
        public string ColumnName
        {
            get
            {
                return _columnName;
            }
        }
    }
}
