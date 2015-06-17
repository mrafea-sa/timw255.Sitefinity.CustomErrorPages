﻿using System;
using System.Collections.Generic;
using System.Linq;
using Telerik.Sitefinity.Configuration;
using Telerik.Sitefinity.Data.Configuration;

namespace timw255.Sitefinity.CustomErrorPages.Data.EntityFramework
{
    public class CustomErrorPagesEFDataConnection
    {
        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="CustomErrorPagesEFDataConnection" /> class.
        /// </summary>
        /// <param name="connectionName">Name of the connection.</param>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="dataProvider">The data provider.</param>
        private CustomErrorPagesEFDataConnection(string connectionName, string connectionString, ICustomErrorPagesEFDataProvider dataProvider)
        {
            this.connectionName = connectionName;
            this.connectionString = connectionString;
            this.dataProvider = dataProvider;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the name of the connection.
        /// </summary>
        /// <value>The name.</value>
        public string Name
        {
            get
            {
                return this.connectionName;
            }
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Initializes the connection.
        /// </summary>
        /// <param name="connectionName">Name of the connection.</param>
        /// <param name="dataProvider">The data provider.</param>
        /// <returns></returns>
        public static CustomErrorPagesEFDataConnection InitializeConnection(string connectionName, ICustomErrorPagesEFDataProvider dataProvider)
        {
            IConnectionStringSettings connectionSettings = CustomErrorPagesEFDataConnection.GetConnectionStringSettings(connectionName);

            CustomErrorPagesEFDataConnection connection;
            if (!CustomErrorPagesEFDataConnection.connections.TryGetValue(connectionSettings.Name, out connection))
            {
                lock (CustomErrorPagesEFDataConnection.connectionsLock)
                {
                    if (!CustomErrorPagesEFDataConnection.connections.TryGetValue(connectionSettings.Name, out connection))
                    {
                        connection = new CustomErrorPagesEFDataConnection(connectionSettings.Name, connectionSettings.ConnectionString, dataProvider);
                        CustomErrorPagesEFDataConnection.connections.Add(connectionSettings.Name, connection);
                    }
                }
            }
            return connection;
        }

        /// <summary>
        /// Gets the entity framework context.
        /// </summary>
        /// <param name="connectionName">Name of the connection.</param>
        /// <param name="provider">The provider.</param>
        /// <returns></returns>
        public static CustomErrorPagesEFDbContext GetContext(string connectionName, ICustomErrorPagesEFDataProvider provider)
        {
            CustomErrorPagesEFDataConnection connection;

            if (!connections.TryGetValue(connectionName, out connection))
                connection = InitializeConnection(connectionName, provider);

            return new CustomErrorPagesEFDbContext(connection.connectionString);
        }
        #endregion

        #region Private methods
        /// <summary>
        /// Gets the connection string settings.
        /// </summary>
        /// <param name="connectionStringName">Name of the connection string.</param>
        /// <returns></returns>
        private static IConnectionStringSettings GetConnectionStringSettings(string connectionStringName)
        {
            DataConfig dataConfig = Config.Get<DataConfig>();

            if (!dataConfig.ConnectionStrings.ContainsKey(connectionStringName))
                throw new KeyNotFoundException(connectionStringName);

            return dataConfig.ConnectionStrings[connectionStringName];
        }
        #endregion

        #region Private members
        private static readonly IDictionary<string, CustomErrorPagesEFDataConnection> connections = new Dictionary<string, CustomErrorPagesEFDataConnection>();
        private static readonly object connectionsLock = new object();
        private readonly ICustomErrorPagesEFDataProvider dataProvider;
        private readonly string connectionName;
        private string connectionString;
        #endregion
    }
}