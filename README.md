<p align="center">
  <img src="/OfflineSync/MD-Resources/logo.png" />
</p>
<hr>


[![NuGet](https://img.shields.io/nuget/v/Nuget.Core.svg)](https://www.nuget.org/packages/OfflineSyncTest.Server/)

## **Table of Contents**

* [Introduction](#introduction)
* [Supported frameworks](#supported-frameworks)
* [How to Use?](#how-to-use-offlinesync)
  - [Types of Sync](#types-of-sync)
    - [Client to server](#client-to-server)
    - [Client to server and Hard Delete](#client-to-server-and-hard-delete)
    - [Server to Client](#server-to-client)
    - [TwoWay Sync](#twoway-sync)
* [Terminology](#terminology)
  - [AutoSync](#autosync)
  * [Priority](#priority)
  * [VersionID](#version-id)
  * [TransactionID](#transaction-id)
  * [IsSynced](#is-synced)
  * [SyncCreatedAt (client/server)](#sync-created-at)
  * [SyncModifiedAt (client/server)](#sync-modified-at)
* [Contribute](#contribute)
* [License](#license)
* [Contact Us](#contact-us)

## **Introduction**

Offline Sync is a Nuget Package which can be used to keep client and server side Databases in sync with no data loss and tuned to configure all possible scenarios.

<p align="center"><b>Diagrammatical representation of a typical project using</b> <u><a href="https://github.com/KishoreIthadi/OfflineSync">Offline Sync</a></u></p>

<p align="center">
  <img src="/OfflineSync/MD-Resources/illustration.PNG" />
</p>
<hr>

* **Note:**
  >* **Currently only <u>SQLite</u> DB is supported on the client side and <u>SQLServer</u> DB on the server side. Future implementation would include other databases too.**
  
## **Supported Frameworks**

Please refer the following table to know the frameworks we support and tested so far.


## **How to Use OfflineSync?**

There are two dll's you need to add to your project. One in the client side and the other in the server side application and whichever table you are trying to sync you need to do the following in the client side application.

<p align="center"><b>Diagrammatical heirarchy representation of tables in a typical project using</b> <u><a href="https://github.com/KishoreIthadi/OfflineSync">Offline Sync</a></u></p>

<p align="center">
  <img src="/OfflineSync/MD-Resources/models.PNG" />
</p>
<hr>

### Types of Sync

Offline Sync suppports any of the below mentioned types of Sync.

1. [Client to Server.](#client-to-server)
2. [Client to Server and Hard Delete.](#client-to-server-and-hard-delete)
3. [Server to Client.](#server-to-client)
4. [TwoWay Sync.](#twoway-sync)

#### **Client To Server**

* Client to Sever sync will Sync the server table records to match with the client table records.

* **Note:**
  >* **Discards the client changes if there are any.**

#### **Client To Server And Hard Delete**

* Client to Server and Hard Delete is same as the Client to Server but hard deletes the Client records upon successful Sync.

#### **Server To Client**

* Server to client will Sync the client table records with that of server table records.

* **Note:**
  >* **Discards the server changes if there are any.**
  **Use Case: Any Metadata tables(like Dropdown options etc..) in an application.**

#### **TwoWay Sync**

* TwoWay Sync will sync both client and server depending upon the autosync/priority configured in the project.

* **Note:**
  >* **Here we resolve the conflicts if there are any based on the priority provided and then keep both client and server in Sync with the other**
  
## **Terminology**

These are a few terms that you would come across while you use the _offline sync_ in your project.

* [AutoSync](#autosync)
* [Priority](#priority)
* [VersionID](#version-id)
* [TransactionID](#transaction-id)
* [IsSynced](#is-synced)
* [SyncCreatedAt (client/server)](#sync-created-at)
* [SyncModifiedAt (client/server)](#sync-modified-at)

#### **AutoSync**

#### **Priority**
When the sync type is Twoway Sync the priority value for a table will be setup before we do a sync.
These values could be any of these 3.

* Server
>* The record on the server side will be considered over the client side record.
* Client
>* The record on the client side will be considered over the server side record.
* LastUpdated
>* Lastupdated priority means the record which was recently updated will be considered.

When in conflict the priority value will be crucial in making the changes.

#### **Version ID**

This column value for a record in a tables holds a unique id for that record.

#### **Transaction ID**
 
#### **Is Synced**

This column value for a record in a table says if the record is synced or not at that point of time.

#### **Sync Created At**

This column value for a record in a table holds the timestamp of the record created at.

#### **Sync Modified At**

This column value for a record in a table holds the timestamp of the record modified at last time.

---

## Configure a sample project to use OfflineSync

Following steps illustrate the configuration and usage of offline sync in a project.

### Client Side

In the client project install this nuget package, please follow below steps.

[![Nuget](./OfflineSync/MD-Resources/Nuget_white.png)](https://www.nuget.org/)

> **Note: Examples  below isllustrate using offlinesync to sync one table on client side with that of a respective one on the server side.**

<details open>
<summary>Step 1</summary>
<br>
Adding Global Configurations on the client side:

* In the client application  add the following lines which adds global configuration (API URL, Client DB Path, Auth Token, Client DB Type)

<pre>
SyncGlobalConfig.DBPath = @"SyncDB.db";
SyncGlobalConfig.Token = "";
SyncGlobalConfig.APIUrl = @"http://localhost:64115/API/";
SyncGlobalConfig.APIUrl = ClientDBType.SQLite;
</pre>
For reference Enum `ClientDBType.cs` is as follows:
<pre>
public enum ClientDBType
{
  SQLite = 0
}
</pre> 
</details>

<details open>
<summary>Step 2</summary>
<br>
Add Device ID
<br><br>
<pre>
new DeviceIDUtility().InitializeDeviceID();
</pre>
</details>

<details open>
<summary>Step 3</summary>
<br>
Add Settings for each of the tables you want to sync as follows.
<br><br>
<pre>
SyncSettingsUtility syncSettings = new SyncSettingsUtility();
syncSettings.Add(
    new SQLiteSyncSettingsModel
    {
        AutoSync = true,
        ClientTableName = typeof(tblTestClient).Name,
        Priority = OveridePriority.LastUpdated,
        SyncType = SyncType.SyncClientToServer,
        ServerAssemblyName = "User.APIApp",
        ServerTableName = "tblTestModelServer"
    }
);
</pre>
</details>

<details open>
<summary>Step 4</summary>
<br>
Call SyncAsync
<br><br>
<pre>
new SyncUtility&lt;tblTestACTS>().StartSyncAsync();
</pre>
</details>

### Server Side Configurations

Please install the nuget package on the server application and follow below steps to enable offlinesync successfully.

[![Nuget](./OfflineSync/MD-Resources/Nuget_white.png)](https://www.nuget.org/)

* add connnection string in the web.config check the below example.

## **Contribute**

Contributions are always welcome!
Please read the contribution guidelines [here](CONTRIBUTION.md).

## **ToDo**

* [ ] Support for .net core

---

## **License**

[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)

## **Contact Us**

We have a dedicated telegram channel for the support. You may reach us there. We will follow up as and when we are free.

Click this link to join the channel.
