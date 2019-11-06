// Copyright (c) zhenlei520 All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Linq;
using EInfrastructure.Core.UCloud.Storage.Config;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EInfrastructure.Core.UCloud.Storage
{
    /// <summary>
    /// 加载UCloud存储服务
    /// </summary>
    public static class Startup
    {
        #region 加载UCloud服务

        /// <summary>
        /// 加载UCloud服务
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        public static IServiceCollection AddUCloudStorage(this IServiceCollection services)
        {
            EInfrastructure.Core.StartUp.Run();

            var service = services.First(x => x.ServiceType == typeof(IConfiguration));
            var configuration = (IConfiguration) service.ImplementationInstance;
            return AddUCloudStorage(services, configuration);
        }

        #endregion

        #region 加载UCloud服务

        /// <summary>
        /// 加载UCloud服务
        /// </summary>
        /// <param name="services"></param>
        /// <param name="action"></param>
        public static IServiceCollection AddUCloudStorage(this IServiceCollection services,
            Action<UCloudStorageConfig> action)
        {
            EInfrastructure.Core.StartUp.Run();

            services.Configure(action);
            return services;
        }

        #endregion

        #region 加载UCloud服务

        /// <summary>
        /// 加载UCloud服务
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        public static IServiceCollection AddUCloudStorage(this IServiceCollection services,
            IConfiguration configuration)
        {
            EInfrastructure.Core.StartUp.Run();

            services.Configure<UCloudStorageConfig>(configuration);
            return services;
        }

        #endregion
    }
}
