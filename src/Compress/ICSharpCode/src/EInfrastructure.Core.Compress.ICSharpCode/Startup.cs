// Copyright (c) zhenlei520 All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Microsoft.Extensions.DependencyInjection;

namespace EInfrastructure.Core.Compress.ICSharpCode
{
    /// <summary>
    /// 加载压缩服务
    /// </summary>
    public static class Startup
    {
        /// <summary>
        /// 加载此服务
        /// </summary>
        /// <param name="services"></param>
        public static IServiceCollection AddSystemsCompress(this IServiceCollection services)
        {
            EInfrastructure.Core.StartUp.Run();

            return services;
        }
    }
}
