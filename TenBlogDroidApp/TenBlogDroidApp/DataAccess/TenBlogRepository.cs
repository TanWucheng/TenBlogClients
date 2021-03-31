using System;
using System.Collections.Generic;
using TenBlogDroidApp.DataAccess.Entities;

namespace TenBlogDroidApp.DataAccess
{
    public class TenBlogRepository
    {
        protected static TenBlogRepository Instance;
        private readonly TenBlogDatabase _db;

        static TenBlogRepository()
        {
            Instance = new TenBlogRepository();
        }

        protected TenBlogRepository()
        {
            _db = new TenBlogDatabase(TenBlogDatabase.DatabaseFilePath);
        }

        public static IEnumerable<ApplicationLog> GetApplicationLogs()
        {
            return Instance._db.GetApplicationLogs();
        }

        public static IEnumerable<ApplicationLog> GetApplicationLogs(DateTime start, DateTime end)
        {
            return Instance._db.GetApplicationLogs(start, end);
        }

        public static int SaveApplicationLog(ApplicationLog log)
        {
            return Instance._db.SaveApplicationLog(log);
        }
    }
}