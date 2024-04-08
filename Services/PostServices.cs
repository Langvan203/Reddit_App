using AutoMapper;
using Reddit_App.Common;
using Reddit_App.Database;
using Reddit_App.Dto;
using Reddit_App.Repositories;
using Reddit_App.Request;
using System.Net.WebSockets;
using Reddit_App.Models;

namespace Reddit_App.Services
{
    public class PostServices
    {
        private readonly PostRespository _postRespository;
        private readonly IMapper _mapper;
        private IWebHostEnvironment _webhost;

        public PostServices(DatabaseContext dbContext, IMapper mapper, IWebHostEnvironment webhost)
        {
            _postRespository = new PostRespository(dbContext, mapper);
            _mapper = mapper; 
            _webhost = webhost;
        }

        public MessageData AddNewProduct(CreateNewPost request, int userID)
        {
            try
            {
                var p = _mapper.Map<Post>(request);
                p.Image = "";
                if(request.Image != null)
                {
                    var date = DateTime.UtcNow.ToString("yyyy_MM_dd_HH_mm");
                    using(FileStream fileStream = File.Create(_webhost.WebRootPath + "\\posts\\images\\" + date + request.Image.FileName))
                    {
                        request.Image.CopyTo(fileStream);
                        fileStream.Flush();
                    }
                    p.Image = "posts/images/" + date + request.Image.FileName;
                }
                p.UserID = userID;
                _postRespository.Create(p);
                _postRespository.SaveChange();
                return new MessageData { Data = p, Des = "Add new post succesfully" };
            }
            catch
            {
                return new MessageData { Data = null, Des = "Fail to add new post" };
            }
        }
    }
}
