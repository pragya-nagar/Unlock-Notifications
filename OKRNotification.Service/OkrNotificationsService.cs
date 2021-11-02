using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OKRNotification.Common;
using OKRNotification.EF;
using OKRNotification.Service.Contracts;
using OKRNotification.ViewModel.Request;
using OKRNotification.ViewModel.Response;

namespace OKRNotification.Service
{
    [ExcludeFromCodeCoverage]
    public class OkrNotificationsService : BaseService, IOkrNotificationsService

    {
        private readonly IRepositoryAsync<ApplicationMaster> applicationMasterRepo;
        private readonly IRepositoryAsync<NotificationsDetails> notificationsDetailsRepo;
        private readonly IRepositoryAsync<ErrorLog> errorLogRepo;
        private readonly IRepositoryAsync<MessageType> messageTypeRepo;
        private readonly IRepositoryAsync<NotificationType> notificationTypeRepo;
        private readonly ICommonService commonService;


        public OkrNotificationsService(IServicesAggregator servicesAggregateService, ICommonService _commonService) : base(servicesAggregateService)
        {
            applicationMasterRepo = UnitOfWorkAsync.RepositoryAsync<ApplicationMaster>();
            notificationsDetailsRepo = UnitOfWorkAsync.RepositoryAsync<NotificationsDetails>();
            errorLogRepo = UnitOfWorkAsync.RepositoryAsync<ErrorLog>();
            messageTypeRepo = UnitOfWorkAsync.RepositoryAsync<MessageType>();
            notificationTypeRepo = UnitOfWorkAsync.RepositoryAsync<NotificationType>();
            commonService = _commonService;


        }




        public async Task<long> SaveNotificationsDetailsAsync(NotificationsRequest notificationsRequest)
        {
            long result = 0;
            foreach (var item in notificationsRequest.To)
            {
                NotificationsDetails notificationsDetails = new NotificationsDetails
                {
                    NotificationsTo = item,
                    NotificationsBy = notificationsRequest.By,
                    NotificationsMessage = notificationsRequest.Text,
                    ApplicationMasterId = notificationsRequest.AppId,
                    NotificationTypeId = notificationsRequest.NotificationType,
                    MessageTypeId = notificationsRequest.MessageType,
                    Url = notificationsRequest.Url,
                    NotificationOnTypeId = notificationsRequest.NotificationOnTypeId,
                    NotificationOnId = notificationsRequest.NotificationOnId

                };
                result = await InsertNotificationsDetailsAsync(notificationsDetails);
            }

            return result;
        }


        public async Task<long> InsertNotificationsDetailsAsync(NotificationsDetails request)
        {

            notificationsDetailsRepo.Add(request);
            await UnitOfWorkAsync.SaveChangesAsync();
            return request.NotificationsDetailsId;

        }

        public int CountUnreadMessages(long employeeid, List<int> appIds)
        {
            int notifications = 0;
            foreach (var appId in appIds)
            {
                notifications += notificationsDetailsRepo.GetQueryable().Where(x => x.NotificationsTo == employeeid && x.ApplicationMasterId == appId && !x.IsDeleted && x.MessageTypeId == 1 && !x.IsRead).ToList().Count;
            }


            return notifications;
        }

        public int CountUnreadEventsForNotifications(long employeeid, List<int> appIds)
        {
            int notifications = 0;
            foreach (var appId in appIds)
            {
                notifications += notificationsDetailsRepo.GetQueryable().Where(x => x.NotificationsTo == employeeid && x.ApplicationMasterId == appId && !x.IsDeleted && x.MessageTypeId == 3 && !x.IsRead).ToList().Count;
            }
            return notifications;
        }

        public int CountUnreadSystemForAlerts(long employeeid, List<int> appIds)
        {
            int notifications = 0;
            foreach (var appId in appIds)
            {
                notifications += notificationsDetailsRepo.GetQueryable().Where(x => x.NotificationsTo == employeeid && x.ApplicationMasterId == appId && !x.IsDeleted && x.MessageTypeId == 2 && !x.IsRead).ToList().Count;
            }
            return notifications;
        }

        public List<NotificationsDetails> GetNotificationsDetailsMessages(long employeeid, List<int> appIds)
        {
            List<NotificationsDetails> notifications = new List<NotificationsDetails>();
            foreach (var appId in appIds)
            {

                var notificationDetails = notificationsDetailsRepo.GetQueryable().Where(x => x.NotificationsTo == employeeid && x.ApplicationMasterId == appId && !x.IsDeleted && x.MessageTypeId == 1);
                notifications.AddRange(notificationDetails);

            }
            if (notifications.Count != 0)
            {
                notifications = notifications.OrderByDescending(x => x.CreatedOn).ToList();
            }

            return notifications;
        }

        public List<NotificationsDetails> GetNotificationsDetailsSystem(long employeeid, List<int> appIds)
        {
            List<NotificationsDetails> notifications = new List<NotificationsDetails>();
            foreach (var appId in appIds)
            {
                var notificationDetails = notificationsDetailsRepo.GetQueryable().Where(x => x.NotificationsTo == employeeid && x.ApplicationMasterId == appId && !x.IsDeleted && x.MessageTypeId == 2).OrderByDescending(x => x.NotificationsDetailsId);
                notifications.AddRange(notificationDetails);
            }
            if (notifications.Count != 0)
            {
                notifications = notifications.OrderByDescending(x => x.CreatedOn).ToList();
            }

            return notifications;
        }

        public int TotalCountUnread(long employeeid, List<int> appIds)
        {
            int notifications = 0;
            foreach (var appId in appIds)
            {
                notifications += notificationsDetailsRepo.GetQueryable().Where(x => x.NotificationsTo == employeeid && x.ApplicationMasterId == appId && !x.IsDeleted && !x.IsRead).ToList().Count;
            }
            return notifications;
        }

        public async Task<NotificationsMessageType> GetNotificationsDetails(long employeeId, string jwtToken, List<int> appIds)
        {
            List<UnreadNotifications> UnreadNotifications = new List<UnreadNotifications>();
            UnreadNotifications CountUnreadMessage = new UnreadNotifications();
            UnreadNotifications CountUnreadSystem = new UnreadNotifications();
            UnreadNotifications CountUnreadEvents = new UnreadNotifications();
            List<NotificationResponse> messageResponses = new List<NotificationResponse>();

            var userList = commonService.GetAllUserFromUsers(jwtToken);
            NotificationsMessageType notificationsMessageType = new NotificationsMessageType();


            CountUnreadMessage.CountUnreadNotifications = CountUnreadMessages(employeeId, appIds);
            CountUnreadSystem.CountUnreadNotifications = CountUnreadSystemForAlerts(employeeId, appIds);
            CountUnreadEvents.CountUnreadNotifications = CountUnreadEventsForNotifications(employeeId, appIds);
            UnreadNotifications.Add(CountUnreadMessage);
            UnreadNotifications.Add(CountUnreadSystem);
            UnreadNotifications.Add(CountUnreadEvents);
            notificationsMessageType.UnreadNotifications = UnreadNotifications;

            var messages = GetNotificationsDetailsMessages(employeeId, appIds);
            if (messages != null)
            {
                var messageDetails = Mapper.Map<List<NotificationResponse>>(messages);
                foreach (NotificationResponse item in messageDetails)
                {
                    var list = userList.Results.FirstOrDefault(x => x.EmployeeId == item.NotificationsBy);
                    item.FirstName = list == null ? "" : list.FirstName;
                    item.LastName = list == null ? "" : list.LastName;
                    item.ImagePath = list == null ? "" : list.ImagePath;
                    if (item.OkrId == 0 && (item.NotificationTypeId == (int)NotificationTypeId.AskFeedback || item.NotificationTypeId == (int)NotificationTypeId.ProvideFeedback || item.NotificationTypeId == (int)NotificationTypeId.Comments))
                    {
                        var krDetails = await commonService.GetKeyDetails(item.KrId, jwtToken);
                        if (krDetails != null && krDetails.AssignmentTypeId != 1)
                        {
                            item.OkrId = krDetails.ObjectiveId;

                        }
                        else
                        {
                            item.OkrId = item.KrId;
                        }
                    }

                    List<string> splitnew = new List<string>();
                    string[] split;

                    if (item.Url.Contains("krAssignment"))
                    {
                        split = item.Url.Split('/');

                        var key = split[2];
                        item.GoalKeyId = key;
                    }
                    messageResponses.Add(item);
                }
            }


            notificationsMessageType.GetMessage = messageResponses;

            notificationsMessageType.GetSystem = GetNotificationsDetailsSystem(employeeId, appIds);

            notificationsMessageType.TotalUnreadNotifications = TotalCountUnread(employeeId, appIds);

            return notificationsMessageType;
        }

        public string ReadAlerts(long to)
        {
            return ReadAlertsForNotifications(to);
        }

        public string ReadAlertsForNotifications(long to)
        {
            using (var command = NotificationDbContext.Database.GetDbConnection().CreateCommand())
            {

                command.CommandText = "EXEC sp_ReadAlerts " + to;
                command.CommandType = CommandType.Text;
                NotificationDbContext.Database.OpenConnection();
                command.ExecuteReader();
                NotificationDbContext.Database.CloseConnection();
            }


            return "";
        }

        public async Task<string> ReadNotificationsForFeedback(long id)
        {
            string read = string.Empty;
            var record = await GetNotificationsById(id);
            if (record != null)
            {
                record.IsRead = true;
                await UpdateNotifications(record);
            }
            return read;
        }

        public async Task<NotificationsDetails> GetNotificationsById(long id)
        {
            return await notificationsDetailsRepo.GetQueryable().FirstOrDefaultAsync(x => x.NotificationsDetailsId == id && !x.IsDeleted);

        }


        public async Task<NotificationsDetails> UpdateNotifications(NotificationsDetails notificationsDetails)
        {
            notificationsDetailsRepo.Update(notificationsDetails);
            await UnitOfWorkAsync.SaveChangesAsync();
            return notificationsDetails;
        }


        public async Task<string> DeleteNotifications(long id)
        {
            string result = "";
            var details = await GetNotificationsById(id);
            if (details != null)
            {
                details.IsDeleted = true;
                await UpdateNotifications(details);

            }
            return result;
        }

        public void SaveLog(string pageName, string functionName, string errorDetail)
        {
            var errorLog = new ErrorLog
            {
                PageName = pageName,
                FunctionName = functionName,
                ErrorDetail = errorDetail,
                ApplicationName = " Okr_Notifications"
            };
            errorLogRepo.Add(errorLog);
            UnitOfWorkAsync.SaveChanges();
        }

        public async Task<NotificationsDetails> UpdateNotificationTextAsync(UpdateNotificationTextRequest updateNotificationTextRequest)
        {
            var record = await GetNotificationsById(updateNotificationTextRequest.NotificationsDetailsId);
            if (record != null)
            {
                record.NotificationsMessage = updateNotificationTextRequest.Text;
                record.NotificationTypeId = updateNotificationTextRequest.NotificationTypeId;
                await UpdateNotifications(record);
            }
            return record;
        }

        public async Task<NotificationsDetails> UpdateNotificationUrlAsync(UpdateNotificationURL updateNotificationURL)
        {
            var record = await GetNotificationsById(updateNotificationURL.NotificationsDetailsId);
            if (record != null)
            {
                record.Url = updateNotificationURL.URL;

                await UpdateNotifications(record);
            }
            return record;
        }

        public async Task<List<NotificationsDetailsResponse>> GetAllNotifications()
        {
            List<NotificationsDetailsResponse> notifications = new List<NotificationsDetailsResponse>();
            var notificationDetails = await notificationsDetailsRepo.GetQueryable().Where(x => x.IsDeleted == false && x.NotificationTypeId == (int)NotificationTypeId.AskFeedback || x.NotificationTypeId == (int)NotificationTypeId.ProvideFeedback || x.NotificationTypeId == (int)NotificationTypeId.Comments).ToListAsync();
            var notificationsList = Mapper.Map<List<NotificationsDetailsResponse>>(notificationDetails);
            notifications.AddRange(notificationsList);
            return notifications;
        }
    }

}

