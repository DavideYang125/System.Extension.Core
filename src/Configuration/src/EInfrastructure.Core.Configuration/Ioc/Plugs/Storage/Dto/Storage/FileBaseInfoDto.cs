﻿// Copyright (c) zhenlei520 All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;

namespace EInfrastructure.Core.Configuration.Ioc.Plugs.Storage.Dto.Storage
{
    /// <summary>
    /// 文件基础信息
    /// </summary>
    public class FileBaseInfoDto
    {
        /// <summary>
        /// 是否成功
        /// </summary>
        public bool Success { get; set; } = true;

        /// <summary>
        /// 文件流水号-调用方赋值，为方便定位文件
        /// </summary>
        public Guid FileNo { get; set; }

        /// <summary>
        /// 文件域
        /// </summary>
        public string Host { get; set; }

        /// <summary>
        /// 文件相对路径
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// 提示信息
        /// </summary>
        public string Msg { get; set; }
    }
}
