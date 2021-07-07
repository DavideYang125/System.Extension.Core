// Copyright (c) zhenlei520 All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using EInfrastructure.Core.Configuration.Ioc.Plugs.Words;
using EInfrastructure.Core.HelpCommon.Files;
using EInfrastructure.Core.Words.Extension.ImportDict.Common;

namespace EInfrastructure.Core.Words.Extension.ImportDict
{
    /// <summary>
    /// </summary>
    internal class ImportSouGouCommon
    {
        private static IWordProvider _wordService;

        internal static void Initialize(IWordProvider wordService, string path)
        {
            _wordService = wordService;
            var files = FileCommon.GetFiles(path, "*.scel");
            HashSet<PinYinWords> pyws = new HashSet<PinYinWords>();
            foreach (var file in files)
            {
                GetPinYinWords(file, pyws);
            }

            var list = SimplifyPinYinWords(pyws);
            list = list.OrderBy(q => q.Words).Distinct().ToList();

            for (int i = list.Count - 1; i >= 1; i--)
            {
                var t = list[i];
                var t0 = list[i - 1];
                if (t.Words == t0.Words)
                {
                    list.RemoveAt(i);
                }
            }

            list = list.OrderByDescending(q => q.Words.Length).ToList();

            var text = "";
            var py = "";
            var index = 0;
            var pyIndex = "0,";

            for (int i = 0; i < list.Count; i++)
            {
                if (i > 0)
                {
                    text += ",";
                    py += ",";
                }

                text += list[i].Words;
                var pys = list[i].GetPinYinIndex();
                py += string.Join(",", pys);
                index += pys.Count();
                pyIndex += "," + index;
            }
            //PinYinIndexPath
//            WordPinYinPath
//                PinYinDataPath
            File.WriteAllText(BaseWordService.DictPinYinPathConfig.WordPath, text);//确定
//            File.WriteAllText(BaseWordService.DictPinYinPathConfig., pyIndex);
            File.WriteAllText(BaseWordService.DictPinYinPathConfig.WordPinYinPath, py);
        }

        #region 得到拼音单词

        /// <summary>
        /// 得到拼音单词
        /// </summary>
        /// <param name="file"></param>
        /// <param name="pyws"></param>
        static void GetPinYinWords(string file, HashSet<PinYinWords> pyws)
        {
            SougouPinyinScel scel = new SougouPinyinScel();
            var t = scel.Import(file);
            List<Tuple<string, string>> list = new List<Tuple<string, string>>();

            foreach (var item in t)
            {
                var w = item.Word;
                var py = _wordService.GetPinYinFast(w).ToLower();
                var py1 = item.PinYinString.Replace("'", "");
                if (py != py1)
                {
                    list.Add(Tuple.Create(w, item.PinYinString));
                }
            }

            for (int i = 2; i < 5; i++)
            {
                SimplifyPinYin(list, pyws, i);
            }
        }

        #endregion

        #region 得到单词拼音

        /// <summary>
        /// 得到单词拼音
        /// </summary>
        /// <param name="pyws"></param>
        /// <returns></returns>
        static List<PinYinWords> SimplifyPinYinWords(HashSet<PinYinWords> pyws)
        {
            List<PinYinWords> list = new List<PinYinWords>();
            for (int i = 2; i < 5; i++)
            {
                SimplifyPinYinWords(pyws, list, i);
            }

            return list;
        }


        /// <summary>
        /// 得到拼音单词
        /// </summary>
        /// <param name="pyws"></param>
        /// <param name="list"></param>
        /// <param name="length"></param>
        static void SimplifyPinYinWords(HashSet<PinYinWords> pyws, List<PinYinWords> list, int length)
        {
            foreach (var item in pyws)
            {
                var w = item.Words;
                if (w.Length != length) continue;
                foreach (var pyw in pyws)
                {
                    if (pyw.Words.Length >= length) continue;
                    w = w.Replace(pyw.Words, pyw.PinYins);
                }

                var py = _wordService.GetPinYinFast(w).ToLower().Replace("'", "");
                var py1 = item.PinYins.Replace("'", "");

                if (py != py1)
                {
                    list.Add(item);
                }
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="list"></param>
        /// <param name="pyws"></param>
        /// <param name="length"></param>
        static void SimplifyPinYin(List<Tuple<string, string>> list, HashSet<PinYinWords> pyws, int length)
        {
            foreach (var item in list)
            {
                if (item.Item1.Length != length) continue;
                var w = item.Item1;
                foreach (var pyw in pyws)
                {
                    if (pyw.Words.Length >= length) continue;

                    w = w.Replace(pyw.Words, pyw.PinYins);
                }

                var py = _wordService.GetPinYinFast(w).ToLower().Replace("'", "");
                var py1 = item.Item2.Replace("'", "");

                if (py != py1)
                {
                    PinYinWords p = new PinYinWords()
                    {
                        Words = item.Item1,
                        PinYins = item.Item2
                    };
                    pyws.Add(p);
                }
            }
        }

        #endregion
    }
}
