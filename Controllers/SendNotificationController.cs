using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Reddit_App.Services;

namespace Reddit_App.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SendNotificationController : ControllerBase
    {
        
        
        public SendNotificationController()
        {
            
        }

        [HttpPost]
        public async Task<IActionResult> SendNotification([FromBody] dynamic data)
        {
            var userID = data.UserID;
            var postID = data.PostID;
            var usernames = data.Username.ToObject<List<string>>();

           

            return Ok();
        }
    }
}
