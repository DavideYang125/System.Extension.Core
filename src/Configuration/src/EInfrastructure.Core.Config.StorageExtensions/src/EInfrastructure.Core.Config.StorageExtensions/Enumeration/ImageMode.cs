﻿// Copyright (c) zhenlei520 All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.ComponentModel;

namespace EInfrastructure.Core.Config.StorageExtensions.Enumeration
{
    /// <summary>
    /// 图片缩放
    /// </summary>
    public class ImageMode : EInfrastructure.Core.Configuration.SeedWork.Enumeration
    {
        /// <summary>
        /// 不处理（原图）
        /// </summary>
        public static ImageMode Nothing = new ImageMode(1, "不处理（原图）");

        /// <summary>
        /// 指定高宽缩放（可能变形）
        /// </summary>
        public static ImageMode Hw = new ImageMode(0, "指定高宽缩放（可能变形）");

        /// <summary>
        /// 指定宽，高按比例
        /// </summary>
        public static ImageMode W = new ImageMode(1, "指定宽，高按比例");

        /// <summary>
        /// 指定高，宽按比例
        /// </summary>
        public static ImageMode H = new ImageMode(2, "指定高，宽按比例");

        /// <summary>
        /// 指定高宽裁减（不变形）
        /// </summary>
        public static ImageMode Cut = new ImageMode(3, "指定高宽裁减（不变形）");

        public ImageMode(int id, string name) : base(id, name)
        {
        }
    }
}
