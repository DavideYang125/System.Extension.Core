// Copyright (c) zhenlei520 All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using EInfrastructure.Core.Config.Entities.Data;
using EInfrastructure.Core.Config.Entities.Ioc;
using Microsoft.EntityFrameworkCore;

namespace EInfrastructure.Core.SqlServer.Repository
{
    /// <summary>
    ///
    /// </summary>
    public class SpatialDimensionQuery<TEntity, T> : ISpatialDimensionQuery<TEntity, T>
        where TEntity : class, IEntity<T>
        where T : IComparable
    {
        private EInfrastructure.Core.SqlServer.Common.SpatialDimensionBaseQuery<TEntity, T> _spatialDimensionBase;
        protected DbContext Dbcontext;

        /// <summary>
        ///
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <param name="execute"></param>
        public SpatialDimensionQuery(IUnitOfWork unitOfWork, IExecute execute)
        {
            Dbcontext = unitOfWork as DbContext;
            _spatialDimensionBase =
                new EInfrastructure.Core.SqlServer.Common.SpatialDimensionBaseQuery<TEntity, T>(unitOfWork, execute);
        }

        /// <summary>
        /// get list
        /// </summary>
        /// <param name="param"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public List<TOpt> GetList<TOpt>(SpatialDimensionParam param)
        {
            return _spatialDimensionBase.GetList<TOpt>(param);
        }

        /// <summary>
        /// GetPageData
        /// </summary>
        /// <param name="param"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public PageData<TOpt> GetPageData<TOpt>(SpatialDimensionPagingParam param)
        {
            return _spatialDimensionBase.GetPageData<TOpt>(param);
        }

        #region get list

        /// <summary>
        /// get IQueryable
        /// </summary>
        /// <returns></returns>
        public IQueryable<TEntity> GetQueryable(SpatialDimensionParam param)
        {
            return _spatialDimensionBase.GetQueryable(param);
        }

        #endregion
    }
}
