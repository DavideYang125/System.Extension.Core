// Copyright (c) zhenlei520 All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using EInfrastructure.Core.Tools;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace EInfrastructure.Core.MySql
{
    /// <summary>
    ///
    /// </summary>
    public static class Ext
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="optionsBuilder"></param>
        public static void EnableDebugTrace(this DbContextOptionsBuilder optionsBuilder)
        {
            LoggerFactory loggerFactory = new LoggerFactory();
            loggerFactory.AddProvider(new TraceLoggerProvider());
            optionsBuilder.UseLoggerFactory(loggerFactory);
            optionsBuilder.EnableSensitiveDataLogging();
        }

        /// <summary>
        /// 自动映射
        /// </summary>
        /// <param name="modelBuilder"></param>
        /// <param name="assType"></param>
        public static void AutoMap(this ModelBuilder modelBuilder, Type assType)
        {
            // Interface that all of our Entity maps implement
            var mappingInterface = typeof(IEntityMap<>);


            // Types that do entity mapping
            var mappingTypes = assType.GetTypeInfo().Assembly.GetTypes()
                .Where(x => x.GetInterfaces().Any(y =>
                    y.GetTypeInfo().IsGenericType && y.GetGenericTypeDefinition() == mappingInterface));

            // Get the generic Entity method of the ModelBuilder type
            var entityMethod = typeof(ModelBuilder).GetMethods()
                .Single(x => x.Name == "Entity" &&
                             x.IsGenericMethod &&
                             x.ReturnType.Name == "EntityTypeBuilder`1");

            foreach (var mappingType in mappingTypes)
            {
                // Get the type of entity to be mapped
                var genericTypeArg = mappingType.GetInterfaces().Single().GenericTypeArguments.Single();

                // Get the method builder.Entity<TEntity>
                var genericEntityMethod = entityMethod.MakeGenericMethod(genericTypeArg);

                // Invoke builder.Entity<TEntity> to get a builder for the entity to be mapped
                var entityBuilder = genericEntityMethod.Invoke(modelBuilder, null);

                // Create the mapping type and do the mapping
                var mapper = Activator.CreateInstance(mappingType);
                mapper.GetType().GetMethod("Map")?.Invoke(mapper, new[] { entityBuilder });
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="sql"></param>
        /// <param name="action"></param>
        public static void Reader(this DbContext dbContext, string sql, Action<DbDataReader> action)
        {
            var conn = dbContext.Database.GetDbConnection();

            try
            {
                conn.Open();

                using (var command = conn.CreateCommand())
                {
                    string query = sql;

                    command.CommandText = query;

                    using (DbDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                action(reader);
                            }
                        }
                    }
                }
            }
            finally
            {
                conn.Close();
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="sql">sql语句</param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static List<T> SqlQuery<T>(this DbContext dbContext, string sql)
        {
            List<T> list = new List<T>();

            var conn = dbContext.Database.GetDbConnection();

            try
            {
                conn.Open();

                using (var command = conn.CreateCommand())
                {
                    string query = sql;

                    command.CommandText = query;

                    using (DbDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            list = reader.ConvertReaderToList<T>();
                        }
                    }
                }
            }
            finally
            {
                conn.Close();
            }


            return list;
        }
    }

    /// <summary>
    /// 跟踪日志
    /// </summary>
    public class TraceLogger : ILogger
    {
        private readonly string categoryName;

        /// <summary>
        ///
        /// </summary>
        /// <param name="categoryName"></param>
        public TraceLogger(string categoryName) => this.categoryName = categoryName;

        /// <summary>
        /// 是否启用日志
        /// </summary>
        /// <param name="logLevel"></param>
        /// <returns></returns>
        public bool IsEnabled(LogLevel logLevel) => true;

        /// <summary>
        /// 增加日志
        /// </summary>
        /// <param name="logLevel"></param>
        /// <param name="eventId"></param>
        /// <param name="state"></param>
        /// <param name="exception"></param>
        /// <param name="formatter"></param>
        /// <typeparam name="TState"></typeparam>
        public void Log<TState>(
            LogLevel logLevel,
            EventId eventId,
            TState state,
            System.Exception exception,
            Func<TState, System.Exception, string> formatter)
        {
            Trace.WriteLine($"{DateTime.Now:o} {logLevel} {eventId.Id} {this.categoryName}");
            Trace.WriteLine(formatter(state, exception));
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="state"></param>
        /// <typeparam name="TState"></typeparam>
        /// <returns></returns>
        public IDisposable BeginScope<TState>(TState state) => null;
    }

    /// <summary>
    ///
    /// </summary>
    public class TraceLoggerProvider : ILoggerProvider
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="categoryName"></param>
        /// <returns></returns>
        public ILogger CreateLogger(string categoryName) => new TraceLogger(categoryName);

        /// <summary>
        ///
        /// </summary>
        public void Dispose()
        {
        }
    }
}
