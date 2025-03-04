﻿using Microsoft.EntityFrameworkCore;
using Unshackled.Fitness.Core.Configuration;

namespace Unshackled.Fitness.Core.Data;

public class MySqlServerDbContext : BaseDbContext
{
	public MySqlServerDbContext(DbContextOptions<MySqlServerDbContext> options,
		ConnectionStrings connectionStrings,
		DbConfiguration dbConfig) : base(options, connectionStrings, dbConfig) { }

	protected override void OnConfiguring(DbContextOptionsBuilder options)
	{
		if (!string.IsNullOrEmpty(ConnectionStrings.DefaultDatabase))
		{
			string prefix = DbConfig.TablePrefix.EndsWith("_") ? DbConfig.TablePrefix : $"{DbConfig.TablePrefix}_";
			// connect to MySql database
			options.UseMySql(ConnectionStrings.DefaultDatabase, ServerVersion.AutoDetect(ConnectionStrings.DefaultDatabase), o =>
			{
				o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
				o.MigrationsHistoryTable($"{prefix}_EFMigrationsHistory");
				o.TranslateParameterizedCollectionsToConstants();
			});
		}
	}

	public static MySqlServerDbContext Create(string connString, string tablePrefix)
	{
		var ob = new DbContextOptionsBuilder<MySqlServerDbContext>();
		ConnectionStrings connStrings = new() { DefaultDatabase = connString };
		DbConfiguration dbConfig = new()
		{
			DatabaseType = DbConfiguration.MYSQL,
			TablePrefix = tablePrefix
		};
		return new MySqlServerDbContext(ob.Options, connStrings, dbConfig);
	}
}