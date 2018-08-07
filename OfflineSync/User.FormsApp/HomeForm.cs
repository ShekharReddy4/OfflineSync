using OfflineSync.Client.Enums;
using OfflineSync.Client.Models;
using OfflineSync.Client.Models.SQLite;
using OfflineSync.Client.Utilities;
using OfflineSync.DomainModel.Enums;
using SQLite;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Windows.Forms;

namespace User.FormsApp
{
    public partial class HomeForm : Form
    {
        public HomeForm()
        {
            InitializeComponent();
        }

        private void HomeForm_Load(object sender, EventArgs e)
        {

        }

        #region Client

        private void btnAddGlobalConfig_Click(object sender, EventArgs e)
        {
            try
            {
                ClearClientValidations();

                SyncGlobalConfig.DBPath = @"SyncDB.db";
                SyncGlobalConfig.Token = "";
                SyncGlobalConfig.APIUrl = @"http://localhost:64115/API/";

                lblClientVal.Text = "Global config updated successfully";
            }
            catch (Exception ex)
            {
                lblClientErrorVal.Text = ex.Message;
            }
        }

        private void btnAddDeviceID_Click(object sender, EventArgs e)
        {
            try
            {
                ClearClientValidations();

                new DeviceIDUtility().InitializeDeviceID();

                UpdateDeviceID();

                lblClientVal.Text = "DeviceID Created Sucessfully";
            }
            catch (Exception ex)
            {
                lblClientErrorVal.Text = ex.Message;
            }
        }

        private void btnCreateTable_Click(object sender, EventArgs e)
        {
            try
            {
                ClearClientValidations();

                SQLite.SQLiteConnection db = new SQLite.SQLiteConnection(SyncGlobalConfig.DBPath);
                db.CreateTable<tblTestACTS>();
                db.CreateTable<tblTestACTSH>();
                db.CreateTable<tblTestASTC>();
                db.CreateTable<tblTestATWS>();
                db.CreateTable<tblTestSTC>();
                db.CreateTable<tblTestTWS>();

                lblClientVal.Text = "Tables Created Sucessfully";
            }
            catch (Exception ex)
            {
                lblClientErrorVal.Text = ex.Message;
            }
        }

        private void btnClientAddSettings_Click(object sender, EventArgs e)
        {
            try
            {
                ClearClientValidations();

                SyncSettingsUtility syncSettings = new SyncSettingsUtility();

                // Autosync is true
                // Sync type is client to server
                syncSettings.Add(
                    new SQLiteSyncSettingsModel
                    {
                        AutoSync = true,
                        ClientTableName = typeof(tblTestACTS).Name,
                        Priority = OveridePriority.LastUpdated,
                        SyncType = SyncType.SyncClientToServer,
                        ServerAssemblyName = "User.APIApp",
                        ServerTableName = "tblTestACTS"
                    }
                );

                syncSettings.Add(
                    new SQLiteSyncSettingsModel
                    {
                        AutoSync = true,
                        ClientTableName = typeof(tblTestACTSH).Name,
                        Priority = OveridePriority.LastUpdated,
                        SyncType = SyncType.SyncClientToServerAndHardDelete,
                        ServerAssemblyName = "User.APIApp",
                        ServerTableName = "tblTestACTSH"
                    }
                );

                syncSettings.Add(
                    new SQLiteSyncSettingsModel
                    {
                        AutoSync = true,
                        ClientTableName = typeof(tblTestASTC).Name,
                        Priority = OveridePriority.LastUpdated,
                        SyncType = SyncType.SyncServerToClient,
                        ServerAssemblyName = "User.APIApp",
                        ServerTableName = "tblTestASTC"
                    }
                );

                syncSettings.Add(
                   new SQLiteSyncSettingsModel
                   {
                       AutoSync = true,
                       ClientTableName = typeof(tblTestATWS).Name,
                       Priority = OveridePriority.LastUpdated,
                       SyncType = SyncType.SyncTwoWay,
                       ServerAssemblyName = "User.APIApp",
                       ServerTableName = "tblTestATWS"
                   }
               );

                // Autosync is false
                // Sync type is client to server
                syncSettings.Add(
                    new SQLiteSyncSettingsModel
                    {
                        AutoSync = false,
                        ClientTableName = typeof(tblTestSTC).Name,
                        Priority = OveridePriority.LastUpdated,
                        SyncType = SyncType.SyncServerToClient,
                        ControllerRoute = "Home/GetData",
                        ControllerData = "tblTestSTC"
                    }
                );

                syncSettings.Add(
                   new SQLiteSyncSettingsModel
                   {
                       AutoSync = false,
                       ClientTableName = typeof(tblTestTWS).Name,
                       Priority = OveridePriority.LastUpdated,
                       ControllerRoute = "Home/GetData",
                       SyncType = SyncType.SyncTwoWay,
                       ServerAssemblyName = "User.APIApp",
                       ServerTableName = "tblTestATWS",
                       ControllerData = "tblTestSTC"
                   }
               );

                lblClientVal.Text = "Settings Added Sucessfully";
            }
            catch (Exception ex)
            {
                lblClientErrorVal.Text = ex.Message;
            }
        }

        private void btnClientRefresh_Click(object sender, EventArgs e)
        {
            try
            {
                ClearClientValidations();

                UpdateClientGrid();
                UpdateLastSyncDateTime();
                UpdateDeviceID();
            }
            catch (Exception ex)
            {
                lblClientErrorVal.Text = ex.Message;
            }
        }

        private void btnClientAdd_Click(object sender, EventArgs e)
        {
            try
            {
                ClearClientValidations();

                using (SQLiteConnection conn = new SQLiteConnection(SyncGlobalConfig.DBPath))
                {
                    switch (cbTblType.SelectedItem.ToString())
                    {
                        case "tblTestACTS":
                            conn.Insert(new tblTestACTS()
                            {
                                Name = txtClientName.Text
                            });
                            break;
                        case "tblTestACTSH":
                            conn.Insert(new tblTestACTSH()
                            {
                                Name = txtClientName.Text
                            });
                            break;
                        case "tblTestASTC":
                            conn.Insert(new tblTestASTC()
                            {
                                Name = txtClientName.Text
                            });
                            break;
                        case "tblTestATWS":
                            conn.Insert(new tblTestATWS()
                            {
                                Name = txtClientName.Text
                            });
                            break;
                        case "tblTestSTC":
                            conn.Insert(new tblTestSTC()
                            {
                                Name = txtClientName.Text
                            });
                            break;
                        case "tblTestTWS":
                            conn.Insert(new tblTestTWS()
                            {
                                Name = txtClientName.Text
                            });
                            break;
                    }
                }

                UpdateClientGrid();

                lblClientVal.Text = "Record Added Sucessfully";

                txtClientName.Text = string.Empty;

                UpdateLastSyncDateTime();
            }
            catch (Exception ex)
            {
                lblClientErrorVal.Text = ex.Message;
            }
        }

        private void btnClientUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                ClearClientValidations();

                using (SQLiteConnection conn = new SQLiteConnection(SyncGlobalConfig.DBPath))
                {
                    dynamic rec = null;

                    switch (cbTblType.SelectedItem.ToString())
                    {
                        case "tblTestACTS":
                            rec = conn.Table<tblTestACTS>().ToList().Where(m => m.ID == Convert.ToInt16(txtClientID.Text)).FirstOrDefault();
                            rec.Name = txtClientName.Text;
                            break;
                        case "tblTestACTSH":
                            rec = conn.Table<tblTestACTSH>().ToList().Where(m => m.ID == Convert.ToInt16(txtClientID.Text)).FirstOrDefault();
                            rec.Name = txtClientName.Text;
                            break;
                        case "tblTestASTC":
                            rec = conn.Table<tblTestASTC>().ToList().Where(m => m.ID == Convert.ToInt16(txtClientID.Text)).FirstOrDefault();
                            rec.Name = txtClientName.Text;
                            break;
                        case "tblTestATWS":
                            rec = conn.Table<tblTestATWS>().ToList().Where(m => m.ID == Convert.ToInt16(txtClientID.Text)).FirstOrDefault();
                            rec.Name = txtClientName.Text;
                            break;
                        case "tblTestSTC":
                            rec = conn.Table<tblTestSTC>().ToList().Where(m => m.ID == Convert.ToInt16(txtClientID.Text)).FirstOrDefault();
                            rec.Name = txtClientName.Text;
                            break;
                        case "tblTestTWS":
                            rec = conn.Table<tblTestTWS>().ToList().Where(m => m.ID == Convert.ToInt16(txtClientID.Text)).FirstOrDefault();
                            rec.Name = txtClientName.Text;
                            break;
                    }

                    conn.Update(rec);
                }

                UpdateClientGrid();

                lblClientVal.Text = "Record Updated Sucessfully";

                txtClientID.Text = string.Empty;
                txtClientName.Text = string.Empty;

                btnClientUpdate.Enabled = false;
                btnClientAdd.Enabled = true;

                UpdateLastSyncDateTime();
            }
            catch (Exception ex)
            {
                lblClientErrorVal.Text = ex.Message;
            }
        }

        private void btnClientSync_Click(object sender, EventArgs e)
        {
            try
            {
                ClearClientValidations();

                switch (cbTblType.SelectedItem.ToString())
                {
                    case "tblTestACTS":
                        new SyncUtility<tblTestACTS>().StartSyncAsync();
                        break;
                    case "tblTestACTSH":
                        new SyncUtility<tblTestACTSH>().StartSyncAsync();
                        break;
                    case "tblTestASTC":
                        new SyncUtility<tblTestASTC>().StartSyncAsync();
                        break;
                    case "tblTestATWS":
                        new SyncUtility<tblTestATWS>().StartSyncAsync();
                        break;
                    case "tblTestSTC":
                        new SyncUtility<tblTestSTC>().StartSyncAsync();
                        break;
                    case "tblTestTWS":
                        new SyncUtility<tblTestTWS>().StartSyncAsync();
                        break;
                }

                lblClientVal.Text = "Sync Sucessfully";

                UpdateLastSyncDateTime();
            }
            catch (Exception ex)
            {
                lblClientErrorVal.Text = ex.Message;
            }
        }

        private void dgvRecords_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                ClearClientValidations();

                if (e.RowIndex >= 0)
                {
                    DataGridViewRow selectedRow = dgvRecords.Rows[e.RowIndex];

                    txtClientID.Text = selectedRow.Cells["ID"].Value.ToString();
                    txtClientName.Text = selectedRow.Cells["Name"].Value.ToString();

                    btnClientUpdate.Enabled = true;
                    btnClientAdd.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                lblClientErrorVal.Text = ex.Message;
            }
        }

        private void cbTblType_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnClientRefresh_Click(null, null);
        }

        private void UpdateLastSyncDateTime()
        {
            try
            {
                using (SQLiteConnection conn = new SQLiteConnection(SyncGlobalConfig.DBPath))
                {
                    var rec = conn.Table<SQLiteSyncSettingsModel>().ToList()
                        .Where(m => m.ClientTableName == cbTblType.SelectedItem.ToString()).FirstOrDefault();

                    if (rec != null)
                    {
                        lblClientLastSyncDateTime.Text = rec.LastSyncedAt;
                    }
                }
            }
            catch (Exception ex)
            {
                lblClientErrorVal.Text = ex.Message;
            }
        }

        private void UpdateDeviceID()
        {
            try
            {
                using (SQLiteConnection conn = new SQLiteConnection(SyncGlobalConfig.DBPath))
                {
                    var rec = conn.Table<SQLiteConfigurationsModel>()
                                          .ToList()
                                          .Where(m => m.Key == "DeviceID")
                                          .FirstOrDefault();

                    if (rec != null)
                    {
                        lblDeviceID.Text = rec.Value;
                    }
                }
            }
            catch (Exception ex)
            {
                lblClientErrorVal.Text = ex.Message;
            }
        }

        private void UpdateClientGrid()
        {
            try
            {
                using (SQLiteConnection conn = new SQLiteConnection(SyncGlobalConfig.DBPath))
                {
                    switch (cbTblType.SelectedItem.ToString())
                    {
                        case "tblTestACTS":
                            dgvRecords.DataSource = conn.Table<tblTestACTS>().ToList();
                            break;
                        case "tblTestACTSH":
                            dgvRecords.DataSource = conn.Table<tblTestACTSH>().ToList();
                            break;
                        case "tblTestASTC":
                            dgvRecords.DataSource = conn.Table<tblTestASTC>().ToList();
                            break;
                        case "tblTestATWS":
                            dgvRecords.DataSource = conn.Table<tblTestATWS>().ToList();
                            break;
                        case "tblTestSTC":
                            dgvRecords.DataSource = conn.Table<tblTestSTC>().ToList();
                            break;
                        case "tblTestTWS":
                            dgvRecords.DataSource = conn.Table<tblTestTWS>().ToList();
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                lblClientErrorVal.Text = ex.Message;
            }
        }

        private void ClearClientValidations()
        {
            lblClientErrorVal.Text = string.Empty;
            lblClientVal.Text = string.Empty;
        }

        private void btnClientReset_Click(object sender, EventArgs e)
        {
            txtClientName.Text = string.Empty;
            txtClientID.Text = string.Empty;
            btnClientAdd.Enabled = true;
            btnClientUpdate.Enabled = false;

            lblClientErrorVal.Text = string.Empty;
            lblClientVal.Text = string.Empty;
        }

        #endregion Client


        #region Server

        const string _connString = "Server=ggku4mpc71;User ID=test;Password=Welcome2ggk;DataBase=SyncDB;";

        private void btnServerAdd_Click(object sender, EventArgs e)
        {
            try
            {
                ClearServerValidations();

                using (SqlConnection conn = new SqlConnection(_connString))
                {
                    string query = string.Empty;

                    switch (cbServerTableType.SelectedItem.ToString())
                    {
                        case "tblTestACTS":
                            query = "INSERT INTO [dbo].[tblTestACTS]([Name]) VALUES ('"
                                + txtServerName.Text + "')";
                            break;
                        case "tblTestACTSH":
                            query = "INSERT INTO [dbo].[tblTestACTSH]([Name]) VALUES ('"
                                + txtServerName.Text + "')";
                            break;
                        case "tblTestASTC":
                            query = "INSERT INTO [dbo].[tblTestASTC]([Name]) VALUES ('"
                                + txtServerName.Text + "')";
                            break;
                        case "tblTestATWS":
                            query = "INSERT INTO [dbo].[tblTestATWS]([Name]) VALUES ('"
                                + txtServerName.Text + "')";
                            break;
                        case "tblTestSTC":
                            query = "INSERT INTO [dbo].[tblTestSTC]([Name]) VALUES ('"
                               + txtServerName.Text + "')";
                            break;
                        case "tblTestTWS":
                            query = "INSERT INTO [dbo].[tblTestTWS]([Name]) VALUES ('"
                                + txtServerName.Text + "')";
                            break;
                    }

                    conn.Open();
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.ExecuteNonQuery();
                    conn.Close();
                }

                UpdateServerGrid();

                lblServerVal.Text = "Record Added Sucessfully";
                txtServerName.Text = string.Empty;
            }
            catch (Exception ex)
            {
                lblServerErrorVal.Text = ex.Message;
            }
        }

        private void btnServerUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                ClearServerValidations();

                using (SqlConnection conn = new SqlConnection(_connString))
                {
                    string query = string.Empty;

                    switch (cbServerTableType.SelectedItem.ToString())
                    {
                        case "tblTestACTS":
                            query = "UPDATE [dbo].[tblTestACTS] SET [Name] = '" + txtServerName.Text + "' " +
                                     "WHERE ID=" + txtServerID.Text + ";";
                            break;
                        case "tblTestACTSH":
                            query = "UPDATE [dbo].[tblTestACTSH] SET [Name] = '" + txtServerName.Text + "' " +
                                     "WHERE ID = " + txtServerID.Text + ";";
                            break;
                        case "tblTestASTC":
                            query = "UPDATE [dbo].[tblTestASTC] SET [Name] = '" + txtServerName.Text + "' " +
                                     "WHERE ID = " + txtServerID.Text + ";";
                            break;
                        case "tblTestATWS":
                            query = "UPDATE [dbo].[tblTestATWS] SET [Name] = '" + txtServerName.Text + "' " +
                                      "WHERE ID = " + txtServerID.Text + ";";
                            break;
                        case "tblTestSTC":
                            query = "UPDATE [dbo].[tblTestSTC] SET [Name] = '" + txtServerName.Text + "' " +
                                     "WHERE ID = " + txtServerID.Text + ";";
                            break;
                        case "tblTestTWS":
                            query = "UPDATE [dbo].[tblTestTWS] SET [Name] = '" + txtServerName.Text + "' " +
                                     "WHERE ID = " + txtServerID.Text + ";";
                            break;
                    }

                    conn.Open();
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.ExecuteNonQuery();
                    conn.Close();
                }

                UpdateServerGrid();

                lblServerVal.Text = "Record Updated Sucessfully";

                txtServerID.Text = string.Empty;
                txtServerName.Text = string.Empty;

                btnServerUpdate.Enabled = false;
                btnServerAdd.Enabled = true;
            }
            catch (Exception ex)
            {
                lblClientErrorVal.Text = ex.Message;
            }
        }

        private void btnServerRefresh_Click(object sender, EventArgs e)
        {
            try
            {
                ClearServerValidations();

                UpdateServerGrid();
            }
            catch (Exception ex)
            {
                lblServerErrorVal.Text = ex.Message;
            }
        }

        private void btnCreateServerTable_Click(object sender, EventArgs e)
        {
            try
            {
                ClearServerValidations();

                using (SqlConnection conn = new SqlConnection(_connString))
                {
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = conn;
                    cmd.CommandText = "IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='tblTestACTS' AND xtype='U')" +
                                      "CREATE TABLE [dbo].[tblTestACTS]([ID] [int] PRIMARY KEY IDENTITY(1,1) NOT NULL,[Name] [nvarchar](max) NULL," +
                                      "[VersionID][nvarchar](max) NULL,[TransactionID] [nvarchar] (max) NULL," +
                                      "[SyncCreatedAt] [datetime] NULL,[SyncModifiedAt] [datetime] NULL) " +

                                      "IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='tblTestACTSH' AND xtype='U')" +
                                      "CREATE TABLE [dbo].[tblTestACTSH]([ID] [int] PRIMARY KEY IDENTITY(1,1) NOT NULL,[Name] [nvarchar](max) NULL," +
                                      "[VersionID][nvarchar](max) NULL,[TransactionID] [nvarchar] (max) NULL," +
                                      "[SyncCreatedAt] [datetime] NULL,[SyncModifiedAt] [datetime] NULL) " +

                                      "IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='tblTestASTC' AND xtype='U')" +
                                      "CREATE TABLE [dbo].[tblTestASTC]([ID] [int] PRIMARY KEY IDENTITY(1,1) NOT NULL,[Name] [nvarchar](max) NULL," +
                                      "[VersionID][nvarchar](max) NULL,[TransactionID] [nvarchar] (max) NULL," +
                                      "[SyncCreatedAt] [datetime] NULL,[SyncModifiedAt] [datetime] NULL) " +

                                      "IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='tblTestATWS' AND xtype='U')" +
                                      "CREATE TABLE [dbo].[tblTestATWS]([ID] [int] PRIMARY KEY IDENTITY(1,1) NOT NULL,[Name] [nvarchar](max) NULL," +
                                      "[VersionID][nvarchar](max) NULL,[TransactionID] [nvarchar] (max) NULL," +
                                      "[SyncCreatedAt] [datetime] NULL,[SyncModifiedAt] [datetime] NULL) " +

                                      "IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='tblTestSTC' AND xtype='U')" +
                                      "CREATE TABLE [dbo].[tblTestSTC]([ID] [int] PRIMARY KEY IDENTITY(1,1) NOT NULL,[Name] [nvarchar](max) NULL," +
                                      "[VersionID][nvarchar](max) NULL,[TransactionID] [nvarchar] (max) NULL," +
                                      "[SyncCreatedAt] [datetime] NULL,[SyncModifiedAt] [datetime] NULL) " +

                                      "IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='tblTestTWS' AND xtype='U')" +
                                      "CREATE TABLE [dbo].[tblTestTWS]([ID] [int] PRIMARY KEY IDENTITY(1,1) NOT NULL,[Name] [nvarchar](max) NULL," +
                                      "[VersionID][nvarchar](max) NULL,[TransactionID] [nvarchar] (max) NULL," +
                                      "[SyncCreatedAt] [datetime] NULL,[SyncModifiedAt] [datetime] NULL) ";

                    conn.Open();
                    cmd.ExecuteNonQuery();
                    conn.Close();

                    lblServerVal.Text = "Tables Created Sucessfully";

                    UpdateServerGrid();

                    txtServerName.Text = string.Empty;
                }
            }
            catch (Exception ex)
            {
                lblServerErrorVal.Text = ex.Message;
            }
        }

        private void ClearServerValidations()
        {
            lblServerErrorVal.Text = string.Empty;
            lblServerVal.Text = string.Empty;
        }

        private void UpdateServerGrid()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connString))
                {
                    SqlDataAdapter adapter = new SqlDataAdapter();
                    DataSet dataSet = new DataSet();
                    string command = "SELECT * FROM ";

                    switch (cbServerTableType.SelectedItem.ToString())
                    {
                        case "tblTestACTS":
                            command += "tblTestACTS";
                            break;
                        case "tblTestACTSH":
                            command += "tblTestACTSH";
                            break;
                        case "tblTestASTC":
                            command += "tblTestASTC";
                            break;
                        case "tblTestATWS":
                            command += "tblTestATWS";
                            break;
                        case "tblTestSTC":
                            command += "tblTestSTC";
                            break;
                        case "tblTestTWS":
                            command += "tblTestTWS";
                            break;
                    }

                    if (cbServerTableType.SelectedItem != null)
                    {
                        adapter = new SqlDataAdapter(command, conn);
                        adapter.Fill(dataSet);
                        dgvServerRecords.DataSource = dataSet.Tables[0];
                    }
                }
            }
            catch (Exception ex)
            {
                lblServerErrorVal.Text = ex.Message;
            }
        }

        private void cbServerTableType_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnServerRefresh_Click(null, null);
        }

        private void dgvServerRecords_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                ClearServerValidations();

                if (e.RowIndex >= 0)
                {
                    DataGridViewRow selectedRow = dgvServerRecords.Rows[e.RowIndex];

                    txtServerID.Text = selectedRow.Cells["ID"].Value.ToString();
                    txtServerName.Text = selectedRow.Cells["Name"].Value.ToString();

                    btnServerUpdate.Enabled = true;
                    btnServerAdd.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                lblServerErrorVal.Text = ex.Message;
            }
        }

        private void btnServerReset_Click(object sender, EventArgs e)
        {
            txtServerName.Text = string.Empty;
            txtServerID.Text = string.Empty;
            btnServerAdd.Enabled = true;
            btnServerUpdate.Enabled = false;

            lblServerErrorVal.Text = string.Empty;
            lblServerVal.Text = string.Empty;
        }

        #endregion Server
    }
}
