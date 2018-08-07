using OfflineSync.Client.Enums;
using OfflineSync.Client.Models.BaseModels;
using OfflineSync.DomainModel.Enums;
using SQLite;

namespace OfflineSync.Client.Models.SQLite
{
    [Table("SyncSettings")]
    public class SQLiteSyncSettingsModel : ISyncSettingsBaseModel
    {
        [PrimaryKey, AutoIncrement]
        public int SyncSettingsID { get; set; }
        [Unique]
        public string ClientTableName { get; set; }
        public string ControllerRoute { get; set; }
        public string ControllerData { get; set; }
        public string ServerAssemblyName { get; set; }
        public string ServerTableName { get; set; }
        public bool AutoSync { get; set; }
        public string LastSyncedAt { get; set; }
        public OveridePriority Priority { get; set; }
        public SyncType SyncType { get; set; }
    }
}