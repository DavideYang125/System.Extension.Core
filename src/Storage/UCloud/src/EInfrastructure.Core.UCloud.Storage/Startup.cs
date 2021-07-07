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
        public static IServiceCollection AddUCloudStorage(this IServiceCollection services)
        {
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
        /// <param name="func"></param>
        public static IServiceCollection AddUCloudStorage(this IServiceCollection services,
            Func<UCloudStorageConfig> func)
        {
            EInfrastructure.Core.StartUp.Run();
            services.AddSingleton(func.Invoke());
            return services;
        }

        /// <summary>
        /// 加载UCloud服务
        /// </summary>
        /// <param name="services"></param>
        /// <param name="action"></param>
        public static IServiceCollection AddUCloudStorage(this IServiceCollection services,
            Action<UCloudStorageConfig> action)
        {
            UCloudStorageConfig uCloudStorageConfig = new UCloudStorageConfig();
            action.Invoke(uCloudStorageConfig);
            return services.AddUCloudStorage(()=>uCloudStorageConfig);
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
            services.Configure<UCloudStorageConfig>(configuration);
            return AddUCloudStorage(services,
                () => configuration.GetSection(nameof(UCloudStorageConfig)).Get<UCloudStorageConfig>());
        }

        #endregion
    }
}
