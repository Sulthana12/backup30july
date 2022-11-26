using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MileDALibrary.DataRepository;
using MileDALibrary.Models;

namespace MileAPI.Controllers
{
    [Route("api/notification")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService _notificationService;
        public NotificationController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        [Route("SendPushNotification")]
        [HttpPost]
        public async Task<IActionResult> SendNotification()
        {
            ResponseModel result = await _notificationService.SendNotification();
            return Ok(result);
        }
    }
}
