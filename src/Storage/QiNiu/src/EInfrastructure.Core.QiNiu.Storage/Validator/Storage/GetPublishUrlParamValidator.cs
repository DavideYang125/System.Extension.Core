﻿// Copyright (c) zhenlei520 All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using EInfrastructure.Core.Configuration.Ioc.Plugs.Storage.Params.Storage;
using FluentValidation;

namespace EInfrastructure.Core.QiNiu.Storage.Validator.Storage
{
    /// <summary>
    ///
    /// </summary>
    internal class GetPublishUrlParamValidator:AbstractValidator<GetPublishUrlParam>
    {
        /// <summary>
        ///
        /// </summary>
        public GetPublishUrlParamValidator()
        {
            RuleFor(x => x.Key).Must(x => !string.IsNullOrEmpty(x)).WithMessage("请输入文件key");
        }
    }
}
