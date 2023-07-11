using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Xml;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Framework.Providers;
using Microsoft.ApplicationBlocks.Data;
using Simplisity;

namespace DNNrocketAPI
{

	/// -----------------------------------------------------------------------------
	/// <summary>
	/// SQL Server implementation of the abstract DataProvider class
	/// </summary>
	/// -----------------------------------------------------------------------------
	public class SqlDataProvider : DataProvider
	{

		#region Private Members

		private const string ProviderType = "data";
		private string ModuleQualifier = "DNNrocket_";

		private readonly ProviderConfiguration _providerConfiguration = ProviderConfiguration.GetProviderConfiguration(ProviderType);
		private readonly string _connectionString;
		private readonly string _providerPath;
		private readonly string _objectQualifier;
		private readonly string _databaseOwner;

		#endregion

		#region Constructors

		public SqlDataProvider()
		{

            // Read the configuration specific information for this provider
            Provider objProvider = (Provider)(_providerConfiguration.Providers[_providerConfiguration.DefaultProvider]);

			// Read the attributes for this provider

			//Get Connection string from web.config
			_connectionString = Config.GetConnectionString();

			if (string.IsNullOrEmpty(_connectionString))
			{
				// Use connection string specified in provider
				_connectionString = objProvider.Attributes["connectionString"];
			}

			_providerPath = objProvider.Attributes["providerPath"];

			_objectQualifier = objProvider.Attributes["objectQualifier"];
			if (!string.IsNullOrEmpty(_objectQualifier) && _objectQualifier.EndsWith("_", StringComparison.Ordinal) == false)
			{
				_objectQualifier += "_";
			}

			_databaseOwner = objProvider.Attributes["databaseOwner"];
			if (!string.IsNullOrEmpty(_databaseOwner) && _databaseOwner.EndsWith(".", StringComparison.Ordinal) == false)
			{
				_databaseOwner += ".";
			}

		}

		#endregion

		#region Properties

		public string ConnectionString
		{
			get
			{
				return _connectionString;
			}
		}

		public string ProviderPath
		{
			get
			{
				return _providerPath;
			}
		}

		public string ObjectQualifier
		{
			get
			{
				return _objectQualifier;
			}
		}

		public string DatabaseOwner
		{
			get
			{
				return _databaseOwner;
			}
		}

		private string NamePrefix
		{
			get { return DatabaseOwner + ObjectQualifier + ModuleQualifier; }
		}

		#endregion

		#region Private Methods

		private static object GetNull(object Field)
		{
			return DotNetNuke.Common.Utilities.Null.GetNull(Field, DBNull.Value);
		}

        #endregion

        #region Public Methods

        public override int GetListCount(int portalId, int moduleId, string typeCode, string sqlSearchFilter = "", string lang = "", string tableName = "DNNrocket")
        {
            var rtncount = 0;
            return Convert.ToInt32(SqlHelper.ExecuteScalar(ConnectionString, DatabaseOwner + ObjectQualifier + ModuleQualifier + "GetListCount", portalId, moduleId, typeCode, sqlSearchFilter, lang, rtncount, tableName));
        }

        public override IDataReader GetList(int portalId, int moduleId, string typeCode, string sqlSearchFilter = "", string lang = "", string sqlOrderBy = "", int returnLimit = 0, int pageNumber = 0, int pageSize = 0, int recordCount = 0, string tableName = "DNNrocket")
        {
            return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + ModuleQualifier + "GetList", portalId, moduleId, typeCode, sqlSearchFilter, sqlOrderBy, returnLimit, pageNumber, pageSize, recordCount, lang, tableName);
        }

        public override IDataReader GetInfo(int itemId, string lang = "", string tableName = "DNNrocket")
        {
            return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + ModuleQualifier + "Get", itemId, lang, tableName);
        }

        public override int Update(int ItemId, int PortalId, int ModuleId, string TypeCode, string XMLData, string GUIDKey, DateTime ModifiedDate, string TextData, int XrefItemId, int ParentItemId, int UserId, string Lang, int sortOrder, string tableName = "DNNrocket")
        {
            return Convert.ToInt32(SqlHelper.ExecuteScalar(ConnectionString, DatabaseOwner + ObjectQualifier + ModuleQualifier + "Update", ItemId, PortalId, ModuleId, TypeCode, XMLData, GUIDKey, ModifiedDate, TextData, XrefItemId, ParentItemId, UserId, Lang, sortOrder, tableName));
        }

        public override void Delete(int ItemID, string tableName = "DNNrocket")
        {
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + ModuleQualifier + "Delete", ItemID, tableName);
        }

        public override void CleanData(string tableName = "DNNrocket")
        {
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + ModuleQualifier + "CleanData", tableName);
        }
        public override void DeleteAllData(string tableName)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + ModuleQualifier + "DeleteAllData", tableName);
        }

        public override IDataReader GetRecord(int itemId, string tableName = "DNNrocket")
        {
            return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + ModuleQualifier + "GetRecord", itemId, tableName);
        }

        public override IDataReader GetRecordLang(int parentitemId, string lang, string tableName = "DNNrocket")
        {
            return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + ModuleQualifier + "GetRecordLang", parentitemId, lang, tableName);
        }
        public override IDataReader GetUsersCMS(int portalId, string sqlSearchFilter = "", int returnLimit = 0, int pageNumber = 0, int pageSize = 0, int recordCount = 0)
        {
            return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + ModuleQualifier + "GetDNNUsers", portalId, sqlSearchFilter, returnLimit, pageNumber, pageSize, recordCount);
        }
        public override int GetUsersCountCMS(int portalId, string sqlSearchFilter = "")
        {
            return Convert.ToInt32(SqlHelper.ExecuteScalar(ConnectionString, DatabaseOwner + ObjectQualifier + ModuleQualifier + "GetDNNUsersCount", portalId, sqlSearchFilter));
        }
        public override String ExecSql(string commandText)
        {
            commandText = commandText.Replace("{databaseOwner}", DatabaseOwner);
            commandText = commandText.Replace("{objectQualifier}", ObjectQualifier);
            return Convert.ToString(SqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, commandText));
        }
        public override IDataReader ExecSqlList(string commandText)
        {
            commandText = commandText.Replace("{databaseOwner}", DatabaseOwner);
            commandText = commandText.Replace("{objectQualifier}", ObjectQualifier);
            return SqlHelper.ExecuteReader(ConnectionString, CommandType.Text, commandText);
        }
        public override String GetSqlxml(string commandText)
        {
            // With the XML return we often want a large data return, so we need to increase the default command timout.
            // becuase we're compiling against DNN6 we can't use PetaPocoHelper class.  So create a new connection and command with timeout.

            commandText = commandText.Replace("{databaseOwner}", DatabaseOwner);
            commandText = commandText.Replace("{objectQualifier}", ObjectQualifier);

            //Create a new connection
            var rtnData = "Error data reader fail";
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                //Create a new command (with no timeout)
                var command = new SqlCommand(commandText, connection) { CommandTimeout = 200 };
                try
                {
                    System.Text.StringBuilder sb = new System.Text.StringBuilder();
                    using (XmlReader dr = command.ExecuteXmlReader())
                    {
                        dr.MoveToContent();
                        var doloop = true;
                        while (doloop)
                        {
                            string s = dr.ReadOuterXml();
                            if (s != "")
                                sb.AppendLine(s);
                            else
                                doloop = false;
                        }
                    }
                    rtnData = sb.ToString();
                }
                finally
                {
                    // make sure we always close.
                    connection.Close();
                }
            }

            return rtnData;
        }

        #endregion




    }

}