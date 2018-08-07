using OfflineSync.Client.DB;
using OfflineSync.Client.Enums;
using OfflineSync.Client.Models;
using OfflineSync.Client.Models.BaseModels;

namespace OfflineSync.Client.Utilities
{
    public class SyncSettingsUtility
    {
        IDBOperations _dBOperations;

        public SyncSettingsUtility()
        {
            switch (SyncGlobalConfig.DBType)
            {
                case ClientDBType.SQLite:
                    _dBOperations = new SQLiteDBOperations();
                    break;
            }
        }

        public void Update(ISyncSettingsBaseModel model)
        {
            _dBOperations.UpdateSyncSettingsModel(model);
        }

        public void Add(ISyncSettingsBaseModel model)
        {
            _dBOperations.AddSyncSettingsModel(model);
        }
    }
}
