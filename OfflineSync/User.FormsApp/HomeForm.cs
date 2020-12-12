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
using User.Client.SQLiteModels;

namespace User.FormsApp
{
    public partial class HomeForm : Form
    {
        public HomeForm()
        {
            InitializeComponent();
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
                        ServerAssemblyName = "User.Server.SQLModels",
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
                        ServerAssemblyName = "User.Server.SQLModels",
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
                        ServerAssemblyName = "User.Server.SQLModels",
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
                       ServerAssemblyName = "User.Server.SQLModels",
                       ServerTableName = "tblTestATWS"
                   }
               );

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
                        ServerAssemblyName = "User.Server.SQLModels",
                        ServerTableName = "tblTestTWS",
                        ControllerData = "tblTestTWS"
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
                                StringType = txtClientName.Text,
                                IntType = Convert.ToInt32(textBox1.Text),
                                FloatType = Convert.ToDouble(textBox2.Text),
                                DateType = dateTimePicker1.Value
                            });
                            break;
                        case "tblTestACTSH":
                            conn.Insert(new tblTestACTSH()
                            {
                                StringType = txtClientName.Text,
                                IntType = Convert.ToInt32(textBox1.Text),
                                FloatType = Convert.ToDouble(textBox2.Text),
                                DateType = dateTimePicker1.Value
                            });
                            break;
                        case "tblTestASTC":
                            conn.Insert(new tblTestASTC()
                            {
                                StringType = txtClientName.Text,
                                IntType = Convert.ToInt32(textBox1.Text),
                                FloatType = Convert.ToDouble(textBox2.Text),
                                DateType = dateTimePicker1.Value
                            });
                            break;
                        case "tblTestATWS":
                            conn.Insert(new tblTestATWS()
                            {
                                StringType = txtClientName.Text,
                                IntType = Convert.ToInt32(textBox1.Text),
                                FloatType = Convert.ToDouble(textBox2.Text),
                                DateType = dateTimePicker1.Value
                            });
                            break;
                        case "tblTestSTC":
                            conn.Insert(new tblTestSTC()
                            {
                                StringType = txtClientName.Text,
                                IntType = Convert.ToInt32(textBox1.Text),
                                FloatType = Convert.ToDouble(textBox2.Text),
                                DateType = dateTimePicker1.Value
                            });
                            break;
                        case "tblTestTWS":
                            conn.Insert(new tblTestTWS()
                            {
                                StringType = txtClientName.Text,
                                IntType = Convert.ToInt32(textBox1.Text),
                                FloatType = Convert.ToDouble(textBox2.Text),
                                DateType = dateTimePicker1.Value
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
                            rec.StringType = txtClientName.Text;
                            rec.IntType = Convert.ToInt32(textBox1.Text);
                            rec.FloatType = Convert.ToDouble(textBox2.Text);
                            rec.DateType = dateTimePicker1.Value;
                            break;
                        case "tblTestACTSH":
                            rec = conn.Table<tblTestACTSH>().ToList().Where(m => m.ID == Convert.ToInt16(txtClientID.Text)).FirstOrDefault();
                            rec.StringType = txtClientName.Text;
                            rec.IntType = Convert.ToInt32(textBox1.Text);
                            rec.FloatType = Convert.ToDouble(textBox2.Text);
                            rec.DateType = dateTimePicker1.Value;
                            break;
                        case "tblTestASTC":
                            rec = conn.Table<tblTestASTC>().ToList().Where(m => m.ID == Convert.ToInt16(txtClientID.Text)).FirstOrDefault();
                            rec.StringType = txtClientName.Text;
                            rec.IntType = Convert.ToInt32(textBox1.Text);
                            rec.FloatType = Convert.ToDouble(textBox2.Text);
                            rec.DateType = dateTimePicker1.Value;
                            break;
                        case "tblTestATWS":
                            rec = conn.Table<tblTestATWS>().ToList().Where(m => m.ID == Convert.ToInt16(txtClientID.Text)).FirstOrDefault();
                            rec.StringType = txtClientName.Text;
                            rec.IntType = Convert.ToInt32(textBox1.Text);
                            rec.FloatType = Convert.ToDouble(textBox2.Text);
                            rec.DateType = dateTimePicker1.Value;
                            break;
                        case "tblTestSTC":
                            rec = conn.Table<tblTestSTC>().ToList().Where(m => m.ID == Convert.ToInt16(txtClientID.Text)).FirstOrDefault();
                            rec.StringType = txtClientName.Text;
                            rec.IntType = Convert.ToInt32(textBox1.Text);
                            rec.FloatType = Convert.ToDouble(textBox2.Text);
                            rec.DateType = dateTimePicker1.Value;
                            break;
                        case "tblTestTWS":
                            rec = conn.Table<tblTestTWS>().ToList().Where(m => m.ID == Convert.ToInt16(txtClientID.Text)).FirstOrDefault();
                            rec.StringType = txtClientName.Text;
                            rec.IntType = Convert.ToInt32(textBox1.Text);
                            rec.FloatType = Convert.ToDouble(textBox2.Text);
                            rec.DateType = dateTimePicker1.Value;
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

        private void btnClientSync_ClickAsync(object sender, EventArgs e)
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
                    txtClientName.Text = selectedRow.Cells["StringType"].Value.ToString();

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
            textBox1.Text = string.Empty;
            textBox2.Text = string.Empty;
            dateTimePicker1.Text = string.Empty;
            lblClientErrorVal.Text = string.Empty;
            lblClientVal.Text = string.Empty;
        }

        #endregion Client


        #region Server

        const string _connString = "Server=.;Database=SyncDB;persist security info=True;Integrated Security = SSPI";

        private void btnServerAdd_Click(object sender, EventArgs e)
        {
            try
            {
                ClearServerValidations();

                using (SqlConnection conn = new SqlConnection(_connString))
                {
                    string query = string.Empty;
                    string serverStringType = txtServerName.Text;
                    string serverIntType = textBox3.Text;
                    string serverFloatType = textBox4.Text;
                    string serverDateType = dateTimePicker2.Value.ToString("yyyy-MM-dd HH:mm:ss");
                    switch (cbServerTableType.SelectedItem)
                    {
                        case "tblTestACTS":
                            query = "INSERT INTO [dbo].[tblTestACTS]([StringType],[IntType],[FloatType],[DateType]) VALUES ('"
                                + serverStringType + "',"+ serverIntType + ","+serverFloatType+ ",'"+ serverDateType + "')";
                            break;
                        case "tblTestACTSH":
                            query = "INSERT INTO [dbo].[tblTestACTSH]([StringType],[IntType],[FloatType],[DateType]) VALUES ('"
                               + serverStringType + "'," + serverIntType + "," + serverFloatType + ",'" + serverDateType + "')";
                            break;
                        case "tblTestASTC":
                            query = "INSERT INTO [dbo].[tblTestASTC]([StringType],[IntType],[FloatType],[DateType]) VALUES ('"
                                + serverStringType + "'," + serverIntType + "," + serverFloatType + ",'" + serverDateType + "')";
                            break;
                        case "tblTestATWS":
                            query = "INSERT INTO [dbo].[tblTestATWS]([StringType],[IntType],[FloatType],[DateType]) VALUES ('"
                               + serverStringType + "'," + serverIntType + "," + serverFloatType + ",'" + serverDateType + "')";
                            break;
                        case "tblTestSTC":
                            query = "INSERT INTO [dbo].[tblTestSTC]([StringType],[IntType],[FloatType],[DateType]) VALUES ('"
                             + serverStringType + "'," + serverIntType + "," + serverFloatType + ",'" + serverDateType + "')";
                            break;
                        case "tblTestTWS":
                            query = "INSERT INTO [dbo].[tblTestTWS]([StringType],[IntType],[FloatType],[DateType]) VALUES ('"
                               + serverStringType + "'," + serverIntType + "," + serverFloatType + ",'" + serverDateType + "')";
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
                    string serverStringType = txtServerName.Text;
                    string serverIntType = textBox3.Text;
                    string serverFloatType = textBox4.Text;
                    string serverDateType = dateTimePicker2.Value.ToString("yyyy-MM-dd HH:mm:ss");
                    switch (cbServerTableType.SelectedItem)
                    {
                        case "tblTestACTS":
                            query = "UPDATE [dbo].[tblTestACTS] SET [StringType] = '" + serverStringType + "',[IntType]=" + serverIntType+ ",[FloatType]=" + serverFloatType+ ",[DateType]='"+ serverDateType+"' "+
                                     "WHERE ID=" + txtServerID.Text + ";";
                            break;
                        case "tblTestACTSH":
                            query = "UPDATE [dbo].[tblTestACTSH] SET [StringType] = '" + serverStringType + "',[IntType]=" + serverIntType + ",[FloatType]=" + serverFloatType + ",[DateType]='" + serverDateType + "' " +
                                     "WHERE ID = " + txtServerID.Text + ";";
                            break;
                        case "tblTestASTC":
                            query = "UPDATE [dbo].[tblTestASTC] SET [StringType] = '" + serverStringType + "',[IntType]=" + serverIntType + ",[FloatType]=" + serverFloatType + ",[DateType]='" + serverDateType + "' " +
                                     "WHERE ID = " + txtServerID.Text + ";";
                            break;
                        case "tblTestATWS":
                            query = "UPDATE [dbo].[tblTestATWS] SET [StringType] = '" + serverStringType + "',[IntType]=" + serverIntType + ",[FloatType]=" + serverFloatType + ",[DateType]='" + serverDateType + "' " +
                                      "WHERE ID = " + txtServerID.Text + ";";
                            break;
                        case "tblTestSTC":
                            query = "UPDATE [dbo].[tblTestSTC] SET [StringType] = '" + serverStringType + "',[IntType]=" + serverIntType + ",[FloatType]=" + serverFloatType + ",[DateType]='" + serverDateType + "' " +
                                     "WHERE ID = " + txtServerID.Text + ";";
                            break;
                        case "tblTestTWS":
                            query = "UPDATE [dbo].[tblTestTWS] SET [StringType] = '" + serverStringType + "',[IntType]=" + serverIntType + ",[FloatType]=" + serverFloatType + ",[DateType]='" + serverDateType + "' " +
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
                lblServerErrorVal.Text = ex.Message;
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
                                      "CREATE TABLE [dbo].[tblTestACTS]([ID] [int] PRIMARY KEY IDENTITY(1,1) NOT NULL,"+
                                      "[StringType] [nvarchar](max) NULL, [IntType] [int] NULL,[FloatType] [float] NULL,[DateType] [datetime] NULL,"+
                                      "[VersionID][nvarchar](max) NULL,[TransactionID] [nvarchar] (max) NULL," +
                                      "[SyncCreatedAt] [datetime] NULL,[SyncModifiedAt] [datetime] NULL) " +

                                      "IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='tblTestACTSH' AND xtype='U')" +
                                      "CREATE TABLE [dbo].[tblTestACTSH]([ID] [int] PRIMARY KEY IDENTITY(1,1) NOT NULL," +
                                      "[StringType] [nvarchar](max) NULL, [IntType] [int] NULL,[FloatType] [float] NULL,[DateType] [datetime] NULL," +
                                      "[VersionID][nvarchar](max) NULL,[TransactionID] [nvarchar] (max) NULL," +
                                      "[SyncCreatedAt] [datetime] NULL,[SyncModifiedAt] [datetime] NULL) " +

                                      "IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='tblTestASTC' AND xtype='U')" +
                                      "CREATE TABLE [dbo].[tblTestASTC]([ID] [int] PRIMARY KEY IDENTITY(1,1) NOT NULL," +
                                      "[StringType] [nvarchar](max) NULL, [IntType] [int] NULL,[FloatType] [float] NULL,[DateType] [datetime] NULL," +
                                      "[VersionID][nvarchar](max) NULL,[TransactionID] [nvarchar] (max) NULL," +
                                      "[SyncCreatedAt] [datetime] NULL,[SyncModifiedAt] [datetime] NULL) " +

                                      "IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='tblTestATWS' AND xtype='U')" +
                                      "CREATE TABLE [dbo].[tblTestATWS]([ID] [int] PRIMARY KEY IDENTITY(1,1) NOT NULL," +
                                      "[StringType] [nvarchar](max) NULL, [IntType] [int] NULL,[FloatType] [float] NULL,[DateType] [datetime] NULL," +
                                      "[VersionID][nvarchar](max) NULL,[TransactionID] [nvarchar] (max) NULL," +
                                      "[SyncCreatedAt] [datetime] NULL,[SyncModifiedAt] [datetime] NULL) " +

                                      "IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='tblTestSTC' AND xtype='U')" +
                                      "CREATE TABLE [dbo].[tblTestSTC]([ID] [int] PRIMARY KEY IDENTITY(1,1) NOT NULL," +
                                      "[StringType] [nvarchar](max) NULL, [IntType] [int] NULL,[FloatType] [float] NULL,[DateType] [datetime] NULL," +
                                      "[VersionID][nvarchar](max) NULL,[TransactionID] [nvarchar] (max) NULL," +
                                      "[SyncCreatedAt] [datetime] NULL,[SyncModifiedAt] [datetime] NULL) " +

                                      "IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='tblTestTWS' AND xtype='U')" +
                                      "CREATE TABLE [dbo].[tblTestTWS]([ID] [int] PRIMARY KEY IDENTITY(1,1) NOT NULL," +
                                      "[StringType] [nvarchar](max) NULL, [IntType] [int] NULL,[FloatType] [float] NULL,[dateType] [datetime] NULL," +
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
                    txtServerName.Text = selectedRow.Cells["StringType"].Value.ToString();

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
            textBox3.Text = string.Empty;
            textBox4.Text = string.Empty;
            dateTimePicker2.Text = string.Empty;
            btnServerAdd.Enabled = true;
            btnServerUpdate.Enabled = false;
            lblServerErrorVal.Text = string.Empty;
            lblServerVal.Text = string.Empty;
        }

        #endregion Server

        private void HomeForm_Load(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void lblClientLastSyncDateTime_Click(object sender, EventArgs e)
        {

        }

        private void lblDeviceID_Click(object sender, EventArgs e)
        {

        }

        private void txtClientName_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtServerName_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {

        }

        private void lblClientVal_Click(object sender, EventArgs e)
        {

        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {

        }

        private void dateTimePicker2_ValueChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click_1(object sender, EventArgs e)
        {

        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }
    }
}
