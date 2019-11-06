﻿// Copyright (c) zhenlei520 All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using EInfrastructure.Core.Config.SerializeExtensions;
using EInfrastructure.Core.Config.SerializeExtensions.Interfaces;
using EInfrastructure.Core.Configuration.Data;
using EInfrastructure.Core.Configuration.Enumeration;
using EInfrastructure.Core.Exception;
using EInfrastructure.Core.Serialize.NewtonsoftJson;

namespace EInfrastructure.Core.HelpCommon
{
    /// <summary>
    /// List操作帮助类
    /// </summary>
    public static class ListCommon
    {
        #region List<T>操作

        #region List实体添加操作

        /// <summary>
        /// List实体添加操作
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="t1">集合1</param>
        /// <param name="t2">集合2</param>
        /// <param name="isCheckRepeat">是否检查重复</param>
        /// <returns>返回t1与t2的和的集合</returns>
        public static List<T> Add<T>(this List<T> t1, List<T> t2, bool isCheckRepeat = false)
        {
            if (!isCheckRepeat)
            {
                t1.AddRange(t2);
                return t1;
            }

            return t1.AddCnki(t2);
        }

        /// <summary>
        /// 查重添加
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="t1">集合1</param>
        /// <param name="t2">集合2</param>
        /// <returns></returns>
        private static List<T> AddCnki<T>(this List<T> t1, List<T> t2)
        {
            List<T> resultList = new CloneableClass().DeepClone(t1);
            List<string> resultStrList = new List<string>();
            foreach (var item in t2)
            {
                if (IsExist(item))
                    continue;
                resultList.Add(item);
            }

            return resultList;

            bool IsExist(T t)
            {
                if (t is string)
                {
                    return t1.Contains(t);
                }

                return GetResultList().Contains(new JsonService(new List<IJsonProvider>()
                {
                    new NewtonsoftJsonProvider()
                }).Serializer(t));
            }

            List<string> GetResultList()
            {
                if (resultStrList.Count == 0 && resultList.Count != 0)
                {
                    t1.ForEach(item =>
                    {
                        resultStrList.Add(new JsonService(new List<IJsonProvider>
                        {
                            new NewtonsoftJsonProvider()
                        }).Serializer(item));
                    });
                }

                return resultStrList;
            }
        }

        #endregion

        #region List实体减法操作

        /// <summary>
        /// List实体减法操作
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="t1">集合1</param>
        /// <param name="t2">集合2</param>
        /// <returns>排除t1中包含t2的项</returns>
        public static List<T> Minus<T>(this List<T> t1, List<T> t2)
        {
            return t1.Where(x => !t2.Contains(x)).ToList();
        }

        /// <summary>
        /// List实体减法操作
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="t1">集合1</param>
        /// <param name="t2">集合2</param>
        /// <returns>排除t1中包含t2的项</returns>
        public static List<T> Minus<T>(this List<T> t1, List<T> t2, bool isCheckRepeat = false) where T : class, new()
        {
            if (!isCheckRepeat)
                return Minus(t1, t2);
            return MinusCnki(t1, t2);
        }

        /// <summary>
        /// 查重添加
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="t1">集合1</param>
        /// <param name="t2">集合2</param>
        /// <returns></returns>
        private static List<T> MinusCnki<T>(this List<T> t1, List<T> t2) where T : class, new()
        {
            JsonService jsonService = new JsonService(new List<IJsonProvider>
            {
                new NewtonsoftJsonProvider()
            });

            List<T> resultList = new CloneableClass().DeepClone(t1);
            List<string> resultStrList = new List<string>();
            if (resultStrList.Count == 0 && resultList.Count != 0)
            {
                t1.ForEach(item => { resultStrList.Add(jsonService.Serializer(item)); });
            }

            foreach (var item in t2)
            {
                var str = jsonService.Serializer(item);
                if (resultStrList.Contains(str))
                {
                    resultStrList.Remove(str);
                }
            }

            return resultStrList.Select(x => jsonService.Deserialize<T>(x)).ToList();
        }

        #endregion

        #region 得到新增的集合以及被删除的集合

        /// <summary>
        /// 得到新增的集合以及被删除的集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="oldObject">老的集合</param>
        /// <param name="newOject">新的集合</param>
        /// <param name="newAddObject">新增加的集合</param>
        /// <param name="deleteObject">被删除的集合</param>
        /// <param name="alwaysExist">总是存在的集合</param>
        public static void ConvertToCheckData<T>(List<T> oldObject, List<T> newOject, out List<T> newAddObject,
            out List<T> deleteObject, out List<T> alwaysExist)
        {
            newAddObject = new List<T>();
            deleteObject = new List<T>();
            alwaysExist = new List<T>();
            if (oldObject == null)
            {
                oldObject = new List<T>();
            }

            if (newOject == null)
            {
                newOject = new List<T>();
            }

            foreach (var objects in oldObject)
            {
                if (newOject.Contains(objects))
                {
                    newAddObject.Remove(objects); //从新的中删除原来已存在的
                    alwaysExist.Add(objects); //始终存在的
                }
                else
                {
                    deleteObject.Add(objects); //被删除的
                }
            }
        }

        #endregion

        #endregion

        #region List转String

        #region List转换为string

        /// <summary>
        /// List转换为string
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="s">待转换的list集合</param>
        /// <param name="c">分割字符</param>
        /// <param name="isReplaceEmpty">是否移除Null或者空字符串</param>
        /// <param name="isReplaceSpace">是否去除空格(仅当为string有效)</param>
        /// <returns></returns>
        public static string ConvertListToString<T>(this List<T> s, char c = ',', bool isReplaceEmpty = true,
            bool isReplaceSpace = true) where T : struct
        {
            if (s == null || s.Count == 0)
            {
                return "";
            }

            return ConvertListToString(s.Select(x => x.ToString()).ToList(), c, isReplaceEmpty, isReplaceSpace);
        }

        #endregion

        #region List转换为string

        /// <summary>
        /// List转换为string
        /// </summary>
        /// <param name="s">待转换的list集合</param>
        /// <param name="c">分割字符</param>
        /// <param name="isReplaceEmpty">是否移除Null或者空字符串</param>
        /// <param name="isReplaceSpace">是否去除空格(仅当为string有效)</param>
        /// <returns></returns>
        public static string ConvertListToString(this List<string> s, char c = ',', bool isReplaceEmpty = true,
            bool isReplaceSpace = true)
        {
            if (s == null || s.Count == 0)
            {
                return "";
            }

            string temp = "";
            foreach (var item in s)
            {
                if (isReplaceEmpty)
                {
                    string itemTemp = "";
                    if (isReplaceSpace)
                    {
                        itemTemp = item.Trim();
                    }

                    if (!string.IsNullOrEmpty(itemTemp))
                    {
                        temp = temp + itemTemp + c;
                    }
                }
                else
                {
                    temp = temp + item + c;
                }
            }

            if (temp.Length > 0)
                temp = temp.Substring(0, temp.Length - 1);
            return temp;
        }

        #endregion

        #region 字符串数组转String(简单转换)

        /// <summary>
        /// 字符串数组转String(简单转换)
        /// </summary>
        /// <param name="s">带转换的list集合</param>
        /// <param name="c">分割字符</param>
        /// <param name="isReplaceEmpty">是否移除Null或者空字符串</param>
        /// <param name="isReplaceSpace">是否去除空格(仅当为string有效)</param>
        /// <returns></returns>
        public static string ConvertListToString(this string[] s, char c = ',', bool isReplaceEmpty = true,
            bool isReplaceSpace = true)
        {
            if (s == null || s.Length == 0)
            {
                return "";
            }

            return ConvertListToString(s.ToList(), c, isReplaceEmpty, isReplaceSpace);
        }

        #endregion

        #endregion

        #region 对list集合分页

        /// <summary>
        /// 对list集合分页
        /// </summary>
        /// <param name="query"></param>
        /// <param name="pageSize">页码</param>
        /// <param name="pageIndex">页大小</param>
        /// <param name="isTotal">是否计算总条数</param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static PageData<T> ListPager<T>(this ICollection<T> query, int pageSize, int pageIndex, bool isTotal)
        {
            PageData<T> list = new PageData<T>();

            if (isTotal)
            {
                list.RowCount = query.Count();
            }

            if (pageIndex - 1 < 0)
            {
                throw new BusinessException("页码必须大于等于1");
            }

            query = query.Skip((pageIndex - 1) * pageSize).ToList();
            if (pageSize > 0)
            {
                list.Data = query.Take(pageSize).ToList();
            }
            else if (pageSize < 1 && pageSize != -1)
            {
                throw new BusinessException("页大小须等于-1或者大于0");
            }
            else
            {
                list.Data = query.ToList();
            }

            return list;
        }

        #endregion

        #region 对list集合分页

        /// <summary>
        /// 对list集合分页执行某个方法
        /// </summary>
        /// <param name="query"></param>
        /// <param name="action"></param>
        /// <param name="pageSize">页大小</param>
        /// <param name="pageIndex">当前页数（默认第一页）</param>
        /// <param name="errCode">错误码</param>
        /// <typeparam name="T"></typeparam>
        public static void ListPager<T>(this ICollection<T> query, Action<List<T>> action, int pageSize = -1,
            int pageIndex = 1, int? errCode = null)
        {
            if (pageSize <= 0 && pageSize != -1)
            {
                throw new BusinessException("页大小必须为正数", errCode ?? HttpStatus.Err.Id);
            }

            var totalCount = query.Count * 1.0d;
            int pageMax = 1;
            if (pageSize != -1)
            {
                pageMax = Math.Ceiling(totalCount / pageSize).ConvertToInt(1);
            }
            else
            {
                pageSize = totalCount.ConvertToInt(0) * 1;
            }

            for (int index = pageIndex; index <= pageMax; index++)
            {
                action(query.Skip((index - 1) * pageSize).Take(pageSize).ToList());
            }
        }

        #endregion

        #region 添加单个对象

        /// <summary>
        /// 添加单个对象
        /// </summary>
        /// <param name="list"></param>
        /// <param name="item"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static List<T> AddNew<T>(this List<T> list, T item)
        {
            list.Add(item);
            return list;
        }

        #endregion

        #region 添加多个集合

        /// <summary>
        /// 添加多个集合
        /// </summary>
        /// <param name="list"></param>
        /// <param name="item"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static List<T> AddNewMult<T>(this List<T> list, List<T> item)
        {
            list.AddRange(item);
            return list;
        }

        #endregion

        #region 移除

        /// <summary>
        /// 移除
        /// </summary>
        /// <param name="list"></param>
        /// <param name="item"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static List<T> RemoveNew<T>(this List<T> list, T item)
        {
            list.Remove(item);
            return list;
        }

        #endregion

        #region 按条件移除

        #region 移除单条符合条件的数据

        /// <summary>
        /// 移除单条符合条件的数据
        /// </summary>
        /// <param name="list"></param>
        /// <param name="condtion">条件</param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static List<T> RemoveNew<T>(this List<T> list, Func<T, bool> condtion)
        {
            List<T> listTemp = list;
            var item = listTemp.FirstOrDefault(condtion);
            if (item != null)
            {
                listTemp.Remove(item);
            }

            return list;
        }

        #endregion

        #region 移除多条满足条件

        /// <summary>
        /// 移除多条满足条件
        /// </summary>
        /// <param name="list"></param>
        /// <param name="condtion">条件</param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static List<T> RemoveMultNew<T>(this List<T> list, Func<T, bool> condtion)
        {
            List<T> listTemp = list;
            var items = listTemp.Where(condtion).ToList();
            foreach (var item in items)
            {
                listTemp.Remove(item);
            }

            return list;
        }

        #endregion

        #endregion
    }
}
