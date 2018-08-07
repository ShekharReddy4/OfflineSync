using System.Threading.Tasks;
using OfflineSync.Client.DB;
using OfflineSync.Client.Enums;
using OfflineSync.Client.Models;
using OfflineSync.DomainModel.Utilities;

namespace OfflineSync.Client.Utilities
{
    public class DeviceIDUtility
    {
        IDBOperations _dBOperations;

        public DeviceIDUtility()
        {
            switch (SyncGlobalConfig.DBType)
            {
                case ClientDBType.SQLite:
                    _dBOperations = new SQLiteDBOperations();
                    break;
            }
        }

        public void InitializeDeviceID()
        {
           Task.Run(GetDeviceID).Wait();
        }

        internal async Task<string> GetDeviceID()
        {
            SyncAPIUtility syncAPI = new SyncAPIUtility(SyncGlobalConfig.APIUrl, SyncGlobalConfig.Token);

            // Checking if deviceID exists
            string deviceID = _dBOperations.GetConfigurationsByKey("DeviceID");

            if (deviceID == null)
            {
                // Get DeviceID from API
                deviceID = await syncAPI.Get<string>(StringUtility.DeviceIDAPICall);

                // saving the device id to configuration table
                _dBOperations.InsertConfigurationsModel("DeviceID", deviceID);
            }

            return deviceID;
        }
    }
}