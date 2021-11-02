using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Okr_Notifications.DataContract;
using Okr_Notifications.Models;

namespace Okr_Notifications.DataProvider
{
    public class OkrNotificationsDataProvider : IOkrNotificationsDataProvider
    {
        private readonly Okr_NotificationsDbContext _database;

        public OkrNotificationsDataProvider(Okr_NotificationsDbContext database)
        {
            _database = database;
        }

        public void SaveLog(string pageName, string functionName, string applicationName, string errorDetail)
        {

            ErrorLog errorLog = new ErrorLog
            {
                PageName = pageName,
                FunctionName = functionName,
                ApplicationName = applicationName,
                ErrorDetail = errorDetail
            };
            _database.ErrorLog.Add(errorLog);
            _database.SaveChanges();
        }


        public List<Comment> GetOkrNotifications(int userid, int supervisorid, string subordinate)
        {
            var okrComments = new List<Comment>();

            try
            {
                using (var command = _database.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = "Exec getOkrComments " + userid + "," + supervisorid + "," + subordinate;
                    command.CommandType = CommandType.Text;
                    _database.Database.OpenConnection();
                    var dataReader = command.ExecuteReader();

                    while (dataReader.Read())
                    {
                        var rcd = new Comment();
                        rcd.UserID = Convert.ToString(dataReader["UserId"]);
                        rcd.Year = Convert.ToInt32(dataReader["Year"]);
                        rcd.Quarter = Convert.ToInt32(dataReader["Quarter"]);
                        rcd.okrid = Convert.ToInt32(dataReader["Quarter"]);
                        rcd.Comments = Convert.ToString(dataReader["Comment"]);
                        rcd.CommentId = Convert.ToInt32(dataReader["CommentId"]);
                        rcd.IsRead = Convert.ToInt32(dataReader["IsRead"]);
                        rcd.CreatedBy = Convert.ToString(dataReader["createdby"]);
                        rcd.UpdatedBy = Convert.ToInt32(dataReader["updateby"]);
                        rcd.UpdatedOn = Convert.ToDateTime(dataReader["UpdatedOn"]);

                        okrComments.Add(rcd);
                    }

                }

            }
            catch (Exception e)
            {
                SaveLog("OkrNotificationsDataProvider", "GetOkrNotifications", "Okr_Notifications",
                    e + "InnerException:" + e.InnerException);
            }

            finally
            {
                _database.Database.CloseConnection();
            }
            return okrComments;

        }



        public long SaveNotificationsDetail(NotificationsDetails request)
        {

            try
            {
                _database.NotificationsDetails.Add(request);
                _database.SaveChanges();

            }
            catch (Exception e)
            {
                SaveLog("OkrNotificationsDataProvider", "SaveNotificationsDetail", "Okr_Notifications",
                    e + "InnerException:" + e.InnerException);
            }
            return request.Id;
        }



        public async Task<long> SaveNotificationsDetailAsync(NotificationsDetails request)
        {
            try
            {
                await _database.NotificationsDetails.AddAsync(request);
                await _database.SaveChangesAsync();

            }
            catch (Exception e)
            {
                SaveLog("OkrNotificationsDataProvider", "SaveNotificationsDetailAsync", "Okr_Notifications",
                    e + "InnerException:" + e.InnerException);
            }
            return request.Id;
        }


        public List<NotificationsDetails> GetNotificationsDetailsSystem(long employeeid, int appId)
        {
            List<NotificationsDetails> notifications = new List<NotificationsDetails>();
            try
            {
                notifications = _database.NotificationsDetails.Where(x => x.NotificationsTo == employeeid && x.appId == appId && !x.IsDeleted && x.MessageType == 2).OrderByDescending(x => x.Id).ToList();
            }
            catch (Exception e)
            {
                SaveLog("OkrNotificationsDataProvider", "GetNotificationsDetailsSystem", "Okr_Notifications",
                    e + "InnerException:" + e.InnerException);
            }

            return notifications;
        }


        public List<NotificationsDetails> GetNotificationsDetailsSystem(long employeeid, List<int> appIds)
        {
            List<NotificationsDetails> notifications = new List<NotificationsDetails>();
            foreach (var appId in appIds)
            {
                try
                {
                    var notificationDetails = _database.NotificationsDetails.Where(x => x.NotificationsTo == employeeid && x.appId == appId && !x.IsDeleted && x.MessageType == 2).OrderByDescending(x => x.Id);
                    notifications.AddRange(notificationDetails);
                }
                catch (Exception e)
                {
                    SaveLog("OkrNotificationsDataProvider", "GetNotificationsDetailsSystem", "Okr_Notifications",
                        e + "InnerException:" + e.InnerException);
                }
            }
            if (notifications.Count != 0)
            {
                notifications = notifications.OrderByDescending(x => x.CreatedOn).ToList();
            }

            return notifications;
        }

        public List<NotificationsDetails> GetNotificationsDetailsEvents(long employeeid, int appId)
        {
            List<NotificationsDetails> notifications = new List<NotificationsDetails>();
            try
            {
                notifications = _database.NotificationsDetails.Where(x => x.NotificationsTo == employeeid && x.appId == appId && !x.IsDeleted && x.MessageType == 3).OrderByDescending(x => x.Id).ToList();

            }
            catch (Exception e)
            {
                SaveLog("OkrNotificationsDataProvider", "GetNotificationsDetailsEvents", "Okr_Notifications",
                    e + "InnerException:" + e.InnerException);
            }

            return notifications;
        }


        public List<NotificationsDetails> GetNotificationsDetailsEvents(long employeeid)
        {
            List<NotificationsDetails> notifications = new List<NotificationsDetails>();
            try
            {
                notifications = _database.NotificationsDetails.Where(x => x.NotificationsTo == employeeid && !x.IsDeleted && x.MessageType == 3).OrderByDescending(x => x.Id).ToList();

            }
            catch (Exception e)
            {
                SaveLog("OkrNotificationsDataProvider", "GetNotificationsDetailsEvents", "Okr_Notifications",
                    e + "InnerException:" + e.InnerException);
            }

            return notifications;
        }




        public List<NotificationsDetails> GetNotificationsDetailsMessages(long employeeid, int appId)
        {
            List<NotificationsDetails> notifications = new List<NotificationsDetails>();
            try
            {
                notifications = _database.NotificationsDetails.Where(x => x.NotificationsTo == employeeid && x.appId == appId && !x.IsDeleted && x.MessageType == 1).OrderByDescending(x => x.Id).ToList();

            }
            catch (Exception e)
            {
                SaveLog("OkrNotificationsDataProvider", "GetNotificationsDetailsMessages", "Okr_Notifications",
                    e + "InnerException:" + e.InnerException);
            }

            return notifications;
        }


        public List<NotificationsDetails> GetNotificationsDetailsMessages(long employeeid)
        {
            List<NotificationsDetails> notifications = new List<NotificationsDetails>();
            try
            {
                notifications = _database.NotificationsDetails.Where(x => x.NotificationsTo == employeeid && !x.IsDeleted && x.MessageType == 1).OrderByDescending(x => x.Id).ToList();

            }
            catch (Exception e)
            {
                SaveLog("OkrNotificationsDataProvider", "GetNotificationsDetailsMessages", "Okr_Notifications",
                    e + "InnerException:" + e.InnerException);
            }

            return notifications;
        }

        public List<NotificationsDetails> GetNotificationsDetailsMessages(long employeeid, List<int> appIds)
        {
            List<NotificationsDetails> notifications = new List<NotificationsDetails>();
            foreach (var appId in appIds)
            {
                try
                {
                    var notificationDetails = _database.NotificationsDetails.Where(x => x.NotificationsTo == employeeid && x.appId == appId && !x.IsDeleted && x.MessageType == 1);
                    notifications.AddRange(notificationDetails);
                }
                catch (Exception e)
                {
                    SaveLog("OkrNotificationsDataProvider", "GetNotificationsDetailsMessages", "Okr_Notifications",
                        e + "InnerException:" + e.InnerException);
                }
            }
            if (notifications.Count != 0)
            {
                notifications = notifications.OrderByDescending(x => x.CreatedOn).ToList();
            }

            return notifications;
        }

        public int CountUnreadMessage(long employeeid, int appId)
        {
            int notifications = 0;
            notifications = _database.NotificationsDetails.Where(x => x.NotificationsTo == employeeid && x.appId == appId && !x.IsDeleted && x.MessageType == 1 && !x.IsRead).ToList().Count;
            return notifications;
        }


        public int CountUnreadMessage(long employeeid, List<int> appIds)
        {
            int notifications = 0;
            foreach (var appId in appIds)
            {
                notifications += _database.NotificationsDetails.Where(x => x.NotificationsTo == employeeid && x.appId == appId && !x.IsDeleted && x.MessageType == 1 && !x.IsRead).ToList().Count;
            }


            return notifications;
        }


        public int CountUnreadEvents(long employeeid, int appId)
        {
            int notifications = 0;
            notifications = _database.NotificationsDetails.Where(x => x.NotificationsTo == employeeid && x.appId == appId && !x.IsDeleted && x.MessageType == 3 && !x.IsRead).ToList().Count;
            return notifications;
        }


        public int CountUnreadEvents(long employeeid, List<int> appIds)
        {
            int notifications = 0;
            foreach (var appId in appIds)
            {
                notifications += _database.NotificationsDetails.Where(x => x.NotificationsTo == employeeid && x.appId == appId && !x.IsDeleted && x.MessageType == 3 && !x.IsRead).ToList().Count;
            }
            return notifications;
        }

        public int CountUnreadSystem(long employeeid, int appId)
        {
            int notifications = 0;
            notifications = _database.NotificationsDetails.Where(x => x.NotificationsTo == employeeid && x.appId == appId && !x.IsDeleted && x.MessageType == 2 && !x.IsRead).ToList().Count;
            return notifications;
        }


        public int CountUnreadSystem(long employeeid, List<int> appIds)
        {
            int notifications = 0;
            foreach (var appId in appIds)
            {
                notifications += _database.NotificationsDetails.Where(x => x.NotificationsTo == employeeid && x.appId == appId && !x.IsDeleted && x.MessageType == 2 && !x.IsRead).ToList().Count;
            }
            return notifications;
        }

        public int TotalCountUnread(long employeeid, int appId)
        {
            int notifications = 0;
            notifications = _database.NotificationsDetails.Where(x => x.NotificationsTo == employeeid && x.appId == appId && !x.IsDeleted && !x.IsRead).ToList().Count;
            return notifications;
        }


        public int TotalCountUnread(long employeeid, List<int> appIds)
        {
            int notifications = 0;
            foreach (var appId in appIds)
            {
                notifications += _database.NotificationsDetails.Where(x => x.NotificationsTo == employeeid && x.appId == appId && !x.IsDeleted && !x.IsRead).ToList().Count;
            }
            return notifications;
        }
        public List<ApplicationMaster> GetApplicationMasters()
        {
            List<ApplicationMaster> applicationMasters = new List<ApplicationMaster>();
            try
            {
                applicationMasters = _database.ApplicationMaster.Where(x => !x.IsDeleted).ToList();
            }
            catch (Exception e)
            {
                SaveLog("OkrNotificationsDataProvider", "GetApplicationMasters", "Okr_Notifications",
                    e + "InnerException:" + e.InnerException);
            }

            return applicationMasters;
        }

        public string ReadNotifications(long to, int appId)
        {
            try
            {
                using (var command = _database.Database.GetDbConnection().CreateCommand())
                {

                    command.CommandText = "EXEC sp_ReadNotifications " + to + "," + appId;
                    command.CommandType = CommandType.Text;
                    _database.Database.OpenConnection();
                    var dataReader = command.ExecuteReader();
                    _database.Database.CloseConnection();
                }
            }
            catch (Exception ex)
            {
                _database.Database.CloseConnection();
                SaveLog("OkrNotificationsDataProvider", "NotificationsDetails", "Okr_Notifications",
                     ex + "InnerException:" + ex.InnerException);
            }
            return "";
        }


        public string ReadAlerts(long to)
        {
            try
            {
                using (var command = _database.Database.GetDbConnection().CreateCommand())
                {

                    command.CommandText = "EXEC sp_ReadAlerts " + to;
                    command.CommandType = CommandType.Text;
                    _database.Database.OpenConnection();
                    var dataReader = command.ExecuteReader();
                    _database.Database.CloseConnection();
                }
            }
            catch (Exception ex)
            {
                _database.Database.CloseConnection();
                SaveLog("OkrNotificationsDataProvider", "ReadAlerts", "Okr_Notifications",
                     ex + "InnerException:" + ex.InnerException);
            }
            return "";
        }

        public NotificationsDetails GetNotificationsById(long id)
        {
            NotificationsDetails notificationsDetails = new NotificationsDetails();
            notificationsDetails = _database.NotificationsDetails.FirstOrDefault(x => x.Id == id && !x.IsDeleted);
            return notificationsDetails;
        }
        public NotificationsDetails UpdateNotifications(NotificationsDetails notificationsDetails)
        {
            _database.Update(notificationsDetails);
            _database.SaveChanges();
            return notificationsDetails;
        }

    }

}





