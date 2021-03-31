using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SQLite;
using TenBlogDroidApp.DataAccess.Entities;
using Environment = System.Environment;

namespace TenBlogDroidApp.DataAccess
{
    public class TenBlogDatabase : SQLiteConnection
    {
        private static readonly object Locker = new();

        public static string DatabaseFilePath
        {
            get
            {
                var sqliteFilename = "TenBlogDB.db3";
                var libraryPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                var path = Path.Combine(libraryPath, sqliteFilename);
                return path;
            }
        }

        public TenBlogDatabase(string connectionString) : base(connectionString)
        {
            CreateTable<ApplicationLog>();
        }

        public IEnumerable<ApplicationLog> GetApplicationLogs()
        {
            lock (Locker)
            {
                return (from i in Table<ApplicationLog>() select i).ToList();
            }
        }

        public IEnumerable<ApplicationLog> GetApplicationLogs(DateTime start, DateTime end)
        {
            lock (Locker)
            {
                return (from i in Table<ApplicationLog>() where i.LogTime >= start && i.LogTime <= end select i).ToList();
            }
        }

        public int SaveApplicationLog(ApplicationLog log)
        {
            lock (Locker)
            {
                return log.Id == 0 ? Insert(log, typeof(ApplicationLog)) : Update(log, typeof(ApplicationLog));
            }
        }
    }
}