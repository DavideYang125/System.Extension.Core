﻿// Copyright (c) zhenlei520 All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using EInfrastructure.Core.Configuration.Ioc.Plugs.Storage.Config.Pictures;

namespace EInfrastructure.Core.Configuration.Ioc.Plugs.Storage.Params.Storage.Pictures
{
    /// <summary>
    /// 根据图片base64上传图片
    /// </summary>
    public class UploadByBase64Param
    {
        /// <summary>
        /// 图片Base64编码
        /// </summary>
        public string Base64 { get; set; }

        /// <summary>
        /// 上传图片策略
        /// </summary>
        public ImgPersistentOps ImgPersistentOps { get; set; }
    }
}
