using OfflineSync.Client.Enums;
using OfflineSync.DomainModel.Enums;

namespace OfflineSync.Client.Models.BaseModels
{
    public interface ISyncSettingsBaseModel
    {
        int SyncSettingsID { get; set; }
        string ClientTableName { get; set; }
        string ControllerRoute { get; set; }
        string ControllerData { get; set; }
        string ServerAssemblyName { get; set; }
        string ServerTableName { get; set; }
        bool AutoSync { get; set; }
        string LastSyncedAt { get; set; }
        OveridePriority Priority { get; set; }
        SyncType SyncType { get; set; }
    }
}