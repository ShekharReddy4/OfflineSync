﻿using OfflineSync.DomainModel.Enums;
using OfflineSync.DomainModel.Models;
using OfflineSync.Server.DB;
using System;
using System.Configuration;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Web.Http;

namespace OfflineSync.Server.Controllers
{
    public class SyncController : ApiController
    {
        IDBOperations _DBOperations;

        public SyncController()
        {
            string DBype = ConfigurationManager.AppSettings["ServerDBType"].ToString();

            switch (DBype)
            {
                case "SQLServer":
                    _DBOperations = new SQLServerDBOperations();
                    break;
            }
        }

        #region HTTPCall

        [HttpGet]
        public HttpResponseMessage GetDeviceID()
        {
            try
            {
                return Request.CreateResponse(HttpStatusCode.OK, _DBOperations.GetDeviceID());
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, CreateExceptionMessage(ex));
            }
        }

        [HttpPost]
        public HttpResponseMessage GetData(APIModel model)
        {
            try
            {
                GetCall(model);

                return Request.CreateResponse(HttpStatusCode.OK, model);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, CreateExceptionMessage(ex));
            }
        }

        [HttpPost]
        public HttpResponseMessage PostData(APIModel model)
        {
            try
            {
                PostCall(model);

                return Request.CreateResponse(HttpStatusCode.OK, model);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, CreateExceptionMessage(ex));
            }
        }

        #endregion

        internal object InvokeDBMethod(string serverTableName, string serverAssemblyName,
            string methodName, object param)
        {
            Assembly assembly = Assembly.Load(serverAssemblyName);
            Type classType = null;

            string DBype = ConfigurationManager.AppSettings["ServerDBType"].ToString();

            switch (DBype)
            {
                case "SQLServer":
                    classType = typeof(SQLServerDBOperations);
                    break;
            }

            object obj = Activator.CreateInstance(classType);

            MethodInfo method = classType.GetMethod(methodName);

            if (method.IsGenericMethod)
            {
                Type tableType = assembly.GetType(serverAssemblyName + "." + serverTableName);
                method = method.MakeGenericMethod(tableType);
            }

            object result = null;

            if (param == null)
            {
                result = method.Invoke(obj, null);
            }
            else
            {
                result = method.Invoke(obj, new object[] { param });
            }

            return result;
        }

        internal string CreateExceptionMessage(Exception ex)
        {
            try
            {
                return ex.Message + ex.InnerException != null ? ex.InnerException.Message : string.Empty;
            }
            catch
            {
                return ex.Message;
            }
        }


        #region TestMethods

        public APIModel GetCall(APIModel model)
        {

            if (model.FailedTrasationData != null && model.SyncType != SyncType.SyncServerToClient)
            {
                InvokeDBMethod(model.ServerTableName, model.ServerAssemblyName,
                          "UpdateFailedTransactions", model);
            }

            if (model.AutoSync &&
               (model.SyncType == SyncType.SyncTwoWay ||
                model.SyncType == SyncType.SyncServerToClient))
            {
                if (model.LastSyncDate == null || model.LastSyncDate == DateTime.MinValue)
                {
                    model.Data = InvokeDBMethod(model.ServerTableName, model.ServerAssemblyName, "GetData", null);
                }
                else
                {
                    model.Data = InvokeDBMethod(model.ServerTableName, model.ServerAssemblyName,
                        "GetDataByLastSyncDate", model.LastSyncDate);
                }
            }

            return model;
        }

        public APIModel PostCall(APIModel model)
        {
            if (model.FailedTrasationData != null && model.SyncType != SyncType.SyncServerToClient)
            {
                InvokeDBMethod(model.ServerTableName, model.ServerAssemblyName,
                          "UpdateFailedTransactions", model);
            }
            else
            {
                InvokeDBMethod(model.ServerTableName, model.ServerAssemblyName, "InsertUpdate", model);
            }

            return model;
        }

        public string GetDeviceIDCall()
        {
            return _DBOperations.GetDeviceID();
        }

        #endregion
    }
}