﻿namespace Unshackled.Fitness.DataMigrator.Enums;

internal enum MigrationTypes
{
	MsSqlToMySql,
	MsSqlToPostgreSql,
	MsSqlToSqlite,
	MySqlToMsSql,
	MySqlToPostgreSql,
	MySqlToSqlite,
	PostgreSqlToMsSql,
	PostgreSqlToMySql,
	PostgreSqlToSqlite,
	SqliteToMsSql,
	SqliteToMySql,
	SqliteToPostgreSql
}

internal static class MigrationTypesExtensions
{
	public static string Target(this MigrationTypes type)
	{
		return type switch
		{
			MigrationTypes.MsSqlToMySql => "MySQL Server",
			MigrationTypes.MsSqlToPostgreSql => "PostgreSQL Server",
			MigrationTypes.MsSqlToSqlite => "Sqlite database file",
			MigrationTypes.MySqlToMsSql => "MS SQL Server",
			MigrationTypes.MySqlToPostgreSql => "PostgreSQL Server",
			MigrationTypes.MySqlToSqlite => "Sqlite database file",
			MigrationTypes.PostgreSqlToMsSql => "MS SQL Server",
			MigrationTypes.PostgreSqlToMySql => "MySQL Server",
			MigrationTypes.PostgreSqlToSqlite => "Sqlite database file",
			MigrationTypes.SqliteToMsSql => "MS SQL Server",
			MigrationTypes.SqliteToMySql => "MySQL Server",
			MigrationTypes.SqliteToPostgreSql => "PostgreSQL Server",
			_ => string.Empty
		};
	}

	public static string Title(this MigrationTypes type)
	{
		return type switch
		{
			MigrationTypes.MsSqlToMySql => "MS SQL to MySQL",
			MigrationTypes.MsSqlToPostgreSql => "MS SQL to PostgreSQL",
			MigrationTypes.MsSqlToSqlite => "MS SQL to Sqlite",
			MigrationTypes.MySqlToMsSql => "MySQL to MS SQL",
			MigrationTypes.MySqlToPostgreSql => "MySQL to PostgreSQL",
			MigrationTypes.MySqlToSqlite => "MySQL to Sqlite",
			MigrationTypes.PostgreSqlToMsSql => "PostgreSQL to MS SQL",
			MigrationTypes.PostgreSqlToMySql => "PostgreSQL to MySQL",
			MigrationTypes.PostgreSqlToSqlite => "PostgreSQL to Sqlite",
			MigrationTypes.SqliteToMsSql => "Sqlite to MS SQL",
			MigrationTypes.SqliteToMySql => "Sqlite to MySQL",
			MigrationTypes.SqliteToPostgreSql => "Sqlite to PostgreSQL",
			_ => string.Empty
		};
	}
}

