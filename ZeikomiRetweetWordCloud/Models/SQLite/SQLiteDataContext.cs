using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZeikomiRetweetWordCloud.Models.SQLite
{
    public class SQLiteDataContext : DbContext
    {
        public DbSet<target_tweetBase> DbSet_target_tweet { get; internal set; }


        // 最初にココを変更する
        static string db_file_path = @"tmp\tmp.db";

        /// <summary>
        /// SQLiteのファイルパス
        /// </summary>
        public static string SQLitePath
        {
            get { return db_file_path; }
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var connectionString = new SqliteConnectionStringBuilder { DataSource = db_file_path }.ToString();
            optionsBuilder.UseSqlite(new SqliteConnection(connectionString));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<target_tweetBase>().HasKey(c => new { c.id });

        }
    }
}
