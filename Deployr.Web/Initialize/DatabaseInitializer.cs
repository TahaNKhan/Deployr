﻿using Deployr.Web.Constants;
using System.Data.Common;
using System.IO;

namespace Deployr.Web.Initialize
{
	public class DatabaseInitializer
	{
		public static void Initialize()
		{
			var dbFileName = DatabaseConstants.DatabaseFileName;
			if (File.Exists(dbFileName))
				return;
			System.Data.SQLite.SQLiteConnection.CreateFile(dbFileName);
			using var sqliteConnection = new System.Data.SQLite.SQLiteConnection($"Data Source={dbFileName}");
			CreateNecessaryTables(sqliteConnection);
		}

		private static void CreateNecessaryTables(DbConnection connection)
		{
			connection.Open();

			// Deployments table
			using var command = connection.CreateCommand();
			command.CommandText = @"CREATE TABLE deployments(
				id INTEGER PRIMARY KEY AUTOINCREMENT, 
				package_name VARCHAR(100) NOT NULL, 
				version VARCHAR(10) NOT NULL, 
				status TINYINT, 
				artifact_location TEXT,
				timestamp_utc DATETIME DEFAULT CURRENT_TIMESTAMP NOT NULL)";
			command.ExecuteNonQuery();

			// Logs table
			using var command2 = connection.CreateCommand();
			command2.CommandText = @"CREATE TABLE logs(id INTEGER PRIMARY KEY AUTOINCREMENT,  deployment_id INTEGER, log TEXT, timestamp_utc DATETIME DEFAULT CURRENT_TIMESTAMP NOT NULL)";
			command2.ExecuteNonQuery();

			connection.Close();
		}
	}
}
