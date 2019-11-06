// Copyright (c) zhenlei520 All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Linq;
using EInfrastructure.Core.Configuration.Enumeration;
using EInfrastructure.Core.Exception;
using FluentValidation.Results;

namespace EInfrastructure.Core.Validation.Common
{
    /// <summary>
    /// 校验处理
    /// </summary>
    public static class ValidationCommon
    {
        #region 检查验证

        /// <summary>
        /// 检查验证
        /// </summary>
        /// <param name="results"></param>
        /// <param name="code">异常码</param>
        public static void Check(this ValidationResult results, int? code = null)
        {
            if (!results.IsValid)
            {
                var msg = results.Errors.Select(x => x.ErrorMessage).FirstOrDefault();
                throw new BusinessException(msg, code??HttpStatus.Err.Id);
            }
        }

        #endregion
    }
}
