﻿namespace OfflineSync.DomainModel.Enums
{
    public enum SyncType
    {
        SyncTwoWay = 0,
        SyncClientToServer = 1,
        SyncClientToServerAndHardDelete = 2,
        SyncServerToClient = 3
    }
}