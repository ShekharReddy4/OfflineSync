using Newtonsoft.Json;
using OfflineSync.DomainModel.Models;
using OfflineSync.Server.Models.SQLServer;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;

namespace OfflineSync.Server.DB
{
    internal class SQLContext<T> : DbContext where T : class
    {
        public SQLContext() : base("SyncConn")
        {
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // table names doesn't changes
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }

        public DbSet<T> dbSet { get; set; }
    }

    internal class SQLServerDBOperations : IDBOperations
    {
        public List<string> GetFailedTransactionInfo(List<string> transactionIDs, string deviceID)
        {
            using (SQLContext<tblSyncTransaction> db = new SQLContext<tblSyncTransaction>())
            {
                List<string> failedTransactionList = new List<string>();

                foreach (var id in transactionIDs)
                {
                    if (!db.dbSet.Any(m => m.TransactionID == id && m.DeviceID == deviceID))
                    {
                        db.dbSet.Add(new tblSyncTransaction { Status = false, DeviceID = deviceID, TransactionID = id });
                        failedTransactionList.Add(id);
                    }
                    else if (db.dbSet.Any(m => m.TransactionID == id && m.DeviceID == deviceID && m.Status == false))
                    {
                        failedTransactionList.Add(id);
                    }
                }

                db.SaveChanges();

                if (failedTransactionList.Count > 0) { return failedTransactionList; }

                return null;
            }
        }

        public List<T> GetData<T>() where T : class, ISQLSyncServerModel
        {
            using (SQLContext<T> db = new SQLContext<T>())
            {
                var res = db.dbSet.ToList();

                if (res.Count > 0) { return res; }

                return null;
            }
        }

        public List<T> GetDataByLastSyncDate<T>(DateTime dt) where T : class, ISQLSyncServerModel
        {
            using (SQLContext<T> db = new SQLContext<T>())
            {
                var res = db.dbSet.Where(m => DateTime.Compare(dt, m.SyncModifiedAt.Value) < 0).ToList();

                if (res.Count > 0) { return res; }

                return null;
            }
        }

        public void UpdateFailedTransactions<T>(APIModel model) where T : class, ISQLSyncServerModel
        {
            List<T> list = JsonConvert.DeserializeObject<List<T>>(model.FailedTrasationData.ToString());

            List<string> uniqueTransactionIDs = new List<string>();

            foreach (var item in list)
            {
                if (!uniqueTransactionIDs.Contains(item.TransactionID.ToString()))
                {
                    uniqueTransactionIDs.Add(item.TransactionID.ToString());
                }
            }

            // checking for failed transactions
            List<string> failedTransactionIDs = (List<string>)GetFailedTransactionInfo(uniqueTransactionIDs, model.DeviceID);

            if (failedTransactionIDs == null)
            {
                return;
            }

            List<T> failedRecords = new List<T>();

            foreach (var item in list)
            {
                if (failedTransactionIDs.Contains(item.TransactionID))
                {
                    failedRecords.Add(item);
                }
            }

            using (SQLContext<T> db = new SQLContext<T>())
            {
                List<T> newList = new List<T>();
                List<T> updateList = new List<T>();

                foreach (var item in failedRecords)
                {
                    var updatedItem = db.dbSet.Where(m => m.VersionID == item.VersionID).FirstOrDefault();

                    if (updatedItem != null)
                    {
                        var adapter = (IObjectContextAdapter)db;

                        // gets primary key column names
                        //var keyNames = adapter.ObjectContext.CreateObjectSet<T>()
                        //               .EntitySet.ElementType.KeyMembers.Select(m => m.Name);

                        // gets identity columns names
                        var keyNames = adapter.ObjectContext.CreateObjectSet<T>()
                                       .EntitySet.ElementType.Members.Where(m => m.MetadataProperties
                                       .Any(n => n.Value.ToString() == "Identity")).Select(m => m.Name);

                        var itemJson = JsonConvert.SerializeObject(item);
                        var itemDict = JsonConvert.DeserializeObject<Dictionary<string, object>>(itemJson);

                        var updatedItemDict = JsonConvert.DeserializeObject<Dictionary<string, object>>(JsonConvert.SerializeObject(updatedItem));

                        foreach (var keyname in keyNames)
                        {
                            itemDict[keyname] = updatedItemDict[keyname];
                        }

                        var finalJson = JsonConvert.SerializeObject(itemDict);
                        var finalRec = JsonConvert.DeserializeObject<T>(finalJson);

                        db.Entry(updatedItem).CurrentValues.SetValues(finalRec);
                    }
                    else
                    {
                        db.dbSet.Add(item);
                    }
                }

                using (SQLContext<tblSyncTransaction> DB = new SQLContext<tblSyncTransaction>())
                {
                    foreach (var item in failedTransactionIDs)
                    {
                        var rec = DB.Set<tblSyncTransaction>().Where(m => m.TransactionID == item && m.DeviceID == model.DeviceID).FirstOrDefault();
                        rec.Status = true;
                    }

                    DB.SaveChanges();
                }

                db.SaveChanges();
            }
        }

        public void InsertUpdate<T>(APIModel model) where T : class, ISQLSyncServerModel
        {
            List<T> list = JsonConvert.DeserializeObject<List<T>>(model.Data.ToString());

            string transactionID = list.FirstOrDefault().TransactionID;

            using (SQLContext<T> db = new SQLContext<T>())
            {
                using (SQLContext<tblSyncTransaction> DB = new SQLContext<tblSyncTransaction>())
                {
                    if (!DB.Set<tblSyncTransaction>().Any(m => m.TransactionID == transactionID && m.DeviceID == model.DeviceID))
                    {
                        DB.Set<tblSyncTransaction>().Add(
                           new tblSyncTransaction
                           {
                               DeviceID = model.DeviceID,
                               TransactionID = transactionID,
                               Status = false
                           }
                        );
                        DB.SaveChanges();
                    }
                    else
                    {
                        // TransactionID with DeviceID already exists
                        // Dublicate transactionID error
                        model.FailedTransactionIDs[0] = transactionID;
                    }
                }

                foreach (var item in list)
                {
                    try
                    {
                        var updatedItem = db.dbSet.Where(m => m.VersionID == item.VersionID).FirstOrDefault();

                        if (updatedItem != null)
                        {
                            var adapter = (IObjectContextAdapter)db;
                            var keyNames = adapter.ObjectContext.CreateObjectSet<T>()
                                            .EntitySet.ElementType.KeyMembers.Select(m => m.Name);

                            // Need to get the list of autoincrement columns

                            var itemJson = JsonConvert.SerializeObject(item);
                            var itemDict = JsonConvert.DeserializeObject<Dictionary<string, object>>(itemJson);

                            var updatedItemDict = JsonConvert.DeserializeObject<Dictionary<string, object>>(JsonConvert.SerializeObject(updatedItem));

                            foreach (var keyname in keyNames)
                            {
                                itemDict[keyname] = updatedItemDict[keyname];
                            }

                            var finalJson = JsonConvert.SerializeObject(itemDict);
                            var finalRec = JsonConvert.DeserializeObject<T>(finalJson);

                            db.Entry(updatedItem).CurrentValues.SetValues(finalRec);
                        }
                        else
                        {
                            db.dbSet.Add(item);
                        }
                    }
                    catch (Exception ex)
                    {
                        // need to write logic for capturing dublicate versionid
                    }
                }

                db.SaveChanges();

                using (SQLContext<tblSyncTransaction> DB = new SQLContext<tblSyncTransaction>())
                {
                    var transc = DB.Set<tblSyncTransaction>().Where(m => m.TransactionID == transactionID).FirstOrDefault();
                    transc.Status = true;
                    DB.SaveChanges();
                }
            }
        }

        public string GetDeviceID()
        {
            using (SQLContext<tblSyncDevice> db = new SQLContext<tblSyncDevice>())
            {


                string newID = Guid.NewGuid().ToString();

                while (db.Set<tblSyncDevice>().Any(m => m.DeviceID == newID))
                {
                    newID = Guid.NewGuid().ToString();
                }

                db.dbSet.Add(new tblSyncDevice
                {
                    DeviceID = newID
                });

                db.SaveChanges();

                return newID;
            }
        }
    }

    public static class SQLServerDBUtility
    {
        public static void CreateGlobalTrigger()
        {
            using (SQLContext<tblSyncDevice> db = new SQLContext<tblSyncDevice>())
            {
                try
                {
                    db.Database.ExecuteSqlCommand(
                        " IF EXISTS(SELECT 1 FROM sys.triggers WHERE Name = 'Trigger_Sync') " +
                        " BEGIN " +
                        " DROP TRIGGER Trigger_Sync ON DATABASE " +
                        " END ");

                    db.Database.ExecuteSqlCommand(
                        " CREATE TRIGGER Trigger_Sync" +
                        " ON DATABASE" +
                        " FOR CREATE_Table" +
                        " AS" +

                        " BEGIN TRY" +

                        " DECLARE @Name VARCHAR(MAX);" +
                        " SET @Name = (SELECT EVENTDATA().value('(/EVENT_INSTANCE/ObjectName)[1]', 'VARCHAR(MAX)'));" +

                        " IF 4 = (SELECT COUNT(*) Names from SYS.COLUMNS WHERE OBJECT_ID = OBJECT_ID(@Name)" +

                        "" +

                        " AND NAME IN('VersionID', 'TransactionID', 'SyncCreatedAt', 'SyncModifiedAt'))" +
                        " BEGIN" +

                        " EXEC(N'CREATE TRIGGER ' + @Name + '_SyncModifyTrigger ON ' + @Name +" +
                        " ' AFTER UPDATE ' +" +
                        " ' AS' +" +
                        " ' BEGIN' +" +

                        " ' DECLARE @VersionID VARCHAR(500);' +" +

                        " ' SET @VersionID = (SELECT VersionID FROM DELETED)' +" +

                        " ' IF @VersionID IS NOT NULL' +" +
                        " ' BEGIN' +" +

                        " ' SET @VersionID = (SELECT T.VersionID ' +" +
                        " ' FROM '+ @Name +' AS T ' +" +
                        " ' JOIN [INSERTED] AS I ' +" +
                        " ' ON I.VersionID = T.VersionID)' +" +

                        " ' DECLARE @SyncModifiedAt VARCHAR(50);' +" +

                        " ' SET @SyncModifiedAt = (SELECT CONVERT(CHAR(19), CONVERT(DATETIME,' +" +
                        " ' CASE' +" +
                        " ' WHEN D.[SyncModifiedAt] IS NULL' +" +
                        " ' THEN I.[SyncModifiedAt]' +" +
                        " ' WHEN CAST(D.[SyncModifiedAt] AS DATETIME) > CAST(I.[SyncModifiedAt] AS DATETIME) ' +" +
                        " ' THEN D.[SyncModifiedAt]' +" +
                        " ' WHEN CAST(D.[SyncModifiedAt] AS DATETIME) < CAST(I.[SyncModifiedAt] AS DATETIME) ' +" +
                        " ' THEN I.[SyncModifiedAt]' +" +
                        " ' WHEN CAST(D.[SyncModifiedAt] AS DATETIME) = CAST(I.[SyncModifiedAt] AS DATETIME) ' +" +
                        " ' THEN (SELECT GETUTCDATE())' +" +
                        " ' END,' +" +
                        " ' 101),120)' +" +
                        " ' FROM [DELETED] AS D' +" +
                        " ' JOIN [INSERTED] AS I' +" +
                        " ' ON D.VersionID = I.VersionID)' +" +

                        " ' UPDATE ' + @Name + " +
                        " ' SET [SyncModifiedAt] = @SyncModifiedAt' +" +
                        " ' WHERE VersionID = (SELECT VersionID FROM INSERTED WHERE VersionID = @VersionID );' +" +

                        " ' END' +" +
                        " ' END');" +

                        " EXEC(N'CREATE TRIGGER ' + @Name + '_SyncCreateTrigger ON ' + @Name +" +
                        " ' AFTER INSERT ' +" +
                        " ' AS ' +" +
                        " ' BEGIN' +" +

                        " ' UPDATE' + ' ' + @Name +" +
                        " ' SET VersionID =  (CONVERT(VARCHAR(64), NEWID()) + CONVERT(VARCHAR(64), NEWID()))," +

                        " SyncCreatedAt = CONVERT(CHAR(19), CONVERT(DATETIME, GETUTCDATE(), 101), 120)," +
                        " SyncModifiedAt = CONVERT(CHAR(19), CONVERT(DATETIME, GETUTCDATE(), 101), 120)" +

                        " WHERE VersionID IS NULL; ' +" +

                        " ' END');" +

                        " END" +

                        " END TRY" +
                        " BEGIN CATCH" +
                        " END CATCH");

                    db.Database.ExecuteSqlCommand(
                        " DECLARE @Cursor CURSOR; " +
                        " DECLARE @TableName Varchar(20); " +
                        "  " +
                        " SET @Cursor = CURSOR FOR " +
                        " SELECT NAME FROM SYS.TABLES " +
                        "  " +
                        " OPEN @Cursor " +
                        " FETCH NEXT FROM @Cursor " +
                        " INTO @TableName " +
                        "  " +
                        " WHILE @@FETCH_STATUS = 0 " +
                        " BEGIN " +

                        " IF(SELECT COUNT(*) Names FROM SYS.COLUMNS WHERE OBJECT_ID = OBJECT_ID(@TableName) " +
                        "  " +
                        " AND NAME IN('VersionID', 'TransactionID', 'SyncCreatedAt', 'SyncModifiedAt')) = 4 " +
                        "  " +
                        " BEGIN " +
                        " IF EXISTS(SELECT Tr.NAME FROM SYS.TRIGGERS Tr JOIN SYS.TABLES T ON " +
                        "  " +
                        " Tr.parent_id = T.object_id WHERE T.object_id = object_id(@TableName) " +
                        "  " +
                        " AND Tr.NAME like @TableName + '_SyncCreateTrigger') " +
                        "  " +
                        " BEGIN " +
                        "  " +
                        " EXEC(N'DROP TRIGGER ' + @TableName + '_SyncCreateTrigger'); " +
                        " END " +
                        "  " +
                        " IF EXISTS(SELECT Tr.NAME FROM SYS.TRIGGERS Tr JOIN SYS.TABLES T ON " +
                        "  " +
                        " Tr.parent_id = T.object_id WHERE T.object_id = object_id(@TableName) " +
                        "  " +
                        " AND Tr.NAME like @TableName + '_SyncModifyTrigger') " +
                        "  " +
                        " BEGIN " +
                        "  " +
                        " EXEC(N'DROP TRIGGER ' + @TableName + '_SyncModifyTrigger'); " +
                        " END " +
                        " " +

                        " EXEC(N'CREATE TRIGGER ' + @TableName + '_SyncModifyTrigger ON ' + @TableName +" +
                        " ' AFTER UPDATE ' +" +
                        " ' AS' +" +
                        " ' BEGIN' +" +

                        " ' DECLARE @VersionID VARCHAR(500);' +" +

                        " ' SET @VersionID = (SELECT VersionID FROM DELETED)' +" +

                        " ' IF @VersionID IS NOT NULL' +" +
                        " ' BEGIN' +" +

                        " ' SET @VersionID = (SELECT T.VersionID ' +" +
                        " ' FROM '+ @TableName +' AS T ' +" +
                        " ' JOIN [INSERTED] AS I ' +" +
                        " ' ON I.VersionID = T.VersionID)' +" +

                        " ' DECLARE @SyncModifiedAt VARCHAR(50);' +" +

                        " ' SET @SyncModifiedAt = (SELECT CONVERT(CHAR(19), CONVERT(DATETIME,' +" +
                        " ' CASE' +" +
                        " ' WHEN D.[SyncModifiedAt] IS NULL' +" +
                        " ' THEN I.[SyncModifiedAt]' +" +
                        " ' WHEN CAST(D.[SyncModifiedAt] AS DATETIME) > CAST(I.[SyncModifiedAt] AS DATETIME) ' +" +
                        " ' THEN D.[SyncModifiedAt]' +" +
                        " ' WHEN CAST(D.[SyncModifiedAt] AS DATETIME) < CAST(I.[SyncModifiedAt] AS DATETIME) ' +" +
                        " ' THEN I.[SyncModifiedAt]' +" +
                        " ' WHEN CAST(D.[SyncModifiedAt] AS DATETIME) = CAST(I.[SyncModifiedAt] AS DATETIME) ' +" +
                        " ' THEN (SELECT GETUTCDATE())' +" +
                        " ' END,' +" +
                        " ' 101),120)' +" +
                        " ' FROM [DELETED] AS D' +" +
                        " ' JOIN [INSERTED] AS I' +" +
                        " ' ON D.VersionID = I.VersionID)' +" +

                        " ' UPDATE ' + @TableName + " +
                        " ' SET [SyncModifiedAt] = @SyncModifiedAt' +" +
                        " ' WHERE VersionID = (SELECT VersionID FROM INSERTED WHERE VersionID = @VersionID );' +" +

                        " ' END' +" +
                        " ' END');" +

                        " EXEC(N'CREATE TRIGGER ' + @TableName + '_SyncCreateTrigger ON ' + @TableName +" +
                        " ' AFTER INSERT ' +" +
                        " ' AS ' +" +
                        " ' BEGIN' +" +

                        " ' UPDATE' + ' ' + @TableName +" +
                        " ' SET VersionID =  (CONVERT(VARCHAR(64), NEWID()) + CONVERT(VARCHAR(64), NEWID()))," +

                        " SyncCreatedAt = CONVERT(CHAR(19), CONVERT(DATETIME, GETUTCDATE(), 101), 120)," +
                        " SyncModifiedAt = CONVERT(CHAR(19), CONVERT(DATETIME, GETUTCDATE(), 101), 120)" +

                        " WHERE VersionID IS NULL; ' +" +

                        " ' END');" +

                        " END " +

                        " FETCH NEXT FROM @Cursor INTO @TableName " +
                        " END " +
                        " CLOSE @Cursor " +
                        " DEALLOCATE @Cursor "
                        );
                }
                catch (Exception ex)
                {
                }
            }
        }
    }
}