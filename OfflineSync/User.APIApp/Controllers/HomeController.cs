using OfflineSync.DomainModel.Models;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace User.APIApp.Controllers
{
    public class HomeController : ApiController
    {
        [HttpPost]
        public HttpResponseMessage GetData(APIModel model)
        {
            using (SyncDBContext db = new SyncDBContext())
            {
                try
                {
                    switch (model.ControllerData)
                    {
                        case "tblTestSTC":
                            if (model.LastSyncDate == DateTime.MinValue)
                            {
                                model.Data = db.tblTestSTCs.ToList();
                            }
                            else
                            {
                                model.Data = db.tblTestSTCs.Where(m => DateTime.Compare(model.LastSyncDate.Value, m.SyncModifiedAt.Value) < 0).ToList();
                            }
                            break;
                        case "tblTestTWS":
                            if (model.LastSyncDate == DateTime.MinValue)
                            {
                                model.Data = db.tblTestTWSs.ToList();
                            }
                            else
                            {
                                model.Data = db.tblTestTWSs.Where(m => DateTime.Compare(model.LastSyncDate.Value, m.SyncModifiedAt.Value) < 0).ToList();
                            }
                            break;
                    }

                    return Request.CreateResponse(HttpStatusCode.OK, model);
                }
                catch (Exception ex)
                {
                    return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
                }
            }
        }
    }
}