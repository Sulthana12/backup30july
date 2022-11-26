using CorePush.Google;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using MileDALibrary.Interfaces;
using MileDALibrary.Models;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using static MileDALibrary.Models.GoogleNotification;

namespace MileDALibrary.DataRepository
{
    public interface INotificationService
    {
        Task<ResponseModel> SendNotification();
    }

    public class NotificationService : INotificationService
    {
        private readonly IUserRepository _userRepo;
        private readonly FcmNotificationSetting _fcmNotificationSetting;
        private readonly IConfiguration _configuration;

        public NotificationService(IOptions<FcmNotificationSetting> settings, IUserRepository userRepo, IConfiguration configuration)
        {
            _fcmNotificationSetting = settings.Value;
            _userRepo = userRepo;
            _configuration = configuration;
        }

        public async Task<ResponseModel> SendNotification()
        {
            ResponseModel response = new ResponseModel();
            try
            {
                List<DriverNotification> driverNotificationDetails = _userRepo.GetDriverNotificationDetails();
                if (driverNotificationDetails != null)
                {
                    foreach (DriverNotification driverDetails in driverNotificationDetails)
                    {
                        if (driverDetails.Notification_token != "")
                        {
                            /* FCM Sender (Android Device) */
                            FcmSettings settings = new FcmSettings()
                            {
                                //SenderId = _fcmNotificationSetting.SenderId,
                                //ServerKey = _fcmNotificationSetting.ServerKey
                                ServerKey = _fcmNotificationSetting.ServerKey
                            };
                            HttpClient httpClient = new HttpClient();

                            string authorizationKey = string.Format("keyy={0}", settings.ServerKey);
                            string deviceToken = driverDetails.Notification_token;

                            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", authorizationKey);
                            httpClient.DefaultRequestHeaders.Accept
                                    .Add(new MediaTypeWithQualityHeaderValue("application/json"));

                            GoogleNotification.DataPayload dataPayload = new GoogleNotification.DataPayload();
                            string TitleKey = string.Format("PushNotificationSettings:Messages:{0}:Title","Hai");
                            string BodyKey = string.Format("PushNotificationSettings:Messages:{0}:Body", "Hai");
                            dataPayload.Title = _configuration.GetSection(TitleKey).Value;
                            dataPayload.Body = _configuration.GetSection(BodyKey).Value;

                            GoogleNotification notification = new GoogleNotification();
                            notification.Data = dataPayload;
                            notification.Notification = dataPayload;

                            var fcm = new FcmSender(settings, httpClient);
                            var fcmSendResponse = await fcm.SendAsync(deviceToken, notification);

                            if (fcmSendResponse.IsSuccess())
                            {
                                response.IsSuccess = true;
                                response.Message = "Notification sent successfully";
                            }
                            else
                            {
                                response.IsSuccess = false;
                                response.Message = fcmSendResponse.Results[0].Error;
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                response.IsSuccess = false;
                response.Message = "Something went wrong";
            }
            return response;
        }
    }
}
