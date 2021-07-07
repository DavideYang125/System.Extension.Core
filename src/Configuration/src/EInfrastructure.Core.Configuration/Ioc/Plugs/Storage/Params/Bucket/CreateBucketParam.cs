﻿// Copyright (c) zhenlei520 All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using EInfrastructure.Core.Configuration.Ioc.Plugs.Storage.Config;

namespace EInfrastructure.Core.Configuration.Ioc.Plugs.Storage.Params.Bucket
{
    /// <summary>
    /// 创建空间
    /// </summary>
    public class CreateBucketParam
    {
        /// <summary>
        ///
        /// </summary>
        public CreateBucketParam()
        {
        }

        /// <summary>
        /// 创建空间
        /// </summary>
        /// <param name="bucketName">待创建的空间名</param>
        /// <param name="region">存储区域</param>
        /// <param name="persistentOps">策略（其中的空间名不作为本次创建空间的条件）</param>
        public CreateBucketParam(string bucketName, int region, BasePersistentOps persistentOps = null) : this()
        {
            BucketName = bucketName;
            Region = region;
            PersistentOps = persistentOps ?? new BasePersistentOps();
        }

        /// <summary>
        /// 空间名
        /// </summary>
        public string BucketName { get; private set; }

        /// <summary>
        /// 空间存储区域
        /// </summary>
        public int Region { get; private set; }

        /// <summary>
        /// 策略
        /// </summary>
        public BasePersistentOps PersistentOps { get; private set; }
    }
}
