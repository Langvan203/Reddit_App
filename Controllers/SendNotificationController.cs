using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Reddit_App.Services;

namespace Reddit_App.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SendNotificationController : ControllerBase
    {
        private readonly SendNotificationServices _sendNotiServices;
        
        public SendNotificationController(SendNotificationServices sendNotiServices)
        {
            _sendNotiServices = sendNotiServices;
        }

        [HttpPost]
        public async Task<IActionResult> SendNotification([FromBody] dynamic data)
        {
            var userID = data.UserID;
            var postID = data.PostID;
            var usernames = data.Username.ToObject<List<string>>();

            await _sendNotiServices.SendNotificationAsync(userID, postID, usernames);

            return Ok();
        }
    }
}
