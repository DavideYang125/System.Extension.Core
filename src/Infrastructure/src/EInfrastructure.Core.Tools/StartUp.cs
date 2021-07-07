// Copyright (c) zhenlei520 All rights reserved.

namespace EInfrastructure.Core.Tools
{
    /// <summary>
    ///
    /// </summary>
    public static class StartUp
    {
        #region 日志配置

        /// <summary>
        /// 设置是否启用日志
        /// </summary>
        /// <param name="enableLog">启用日志</param>
        /// <returns></returns>
        internal static void SetLog(bool enableLog= false)
        {
            EnableLog = enableLog;
        }

        /// <summary>
        /// 是否启用日志
        /// </summary>
        internal static bool EnableLog { get; private set; }

        #endregion

        private static bool _isStartUp;

        /// <summary>
        /// 启用配置
        /// <param name="enableLog">默认不启用日志</param>
        /// </summary>
        public static void Run(bool enableLog = false)
        {
            if (!_isStartUp)
            {
                SetLog(_isStartUp);
                _isStartUp = true;
            }
            Config.Entities.StartUp.Run(enableLog);
        }
    }
}
