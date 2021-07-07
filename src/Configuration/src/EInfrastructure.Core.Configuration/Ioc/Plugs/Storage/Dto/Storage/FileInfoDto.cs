﻿// Copyright (c) zhenlei520 All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

namespace EInfrastructure.Core.Configuration.Ioc.Plugs.Storage.Dto.Storage
{
    /// <summary>
    /// 文件响应信息
    /// </summary>
    public class FileInfoDto : FileBaseInfoDto
    {
        /// <summary>
        /// 文件md5信息
        /// </summary>
        public string Hash { get; set; }

        /// <summary>
        /// 文件大小
        /// </summary>
        public double Size { get; set; }

        /// <summary>
        /// 文件上传时间
        /// </summary>
        public long PutTime { set; get; }

        /// <summary>
        /// 文件MIME类型
        /// </summary>
        public string MimeType { get; set; }

        /// <summary>
        /// 文件类型
        /// </summary>
        public int FileType { get; set; }
    }
}
