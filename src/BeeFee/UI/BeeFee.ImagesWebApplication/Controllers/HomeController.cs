using System.Collections.Generic;
using System.Threading.Tasks;
using BeeFee.ImageApp.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SharpFuncExt;

namespace BeeFee.ImagesWebApplication.Controllers
{
	public class Model
	{
		public string Setting { get; set; }
		public IFormFile File { get; set; }
		public string Filename { get; set; }
		public string EventName { get; set; }
		public string Key { get; set; }
	}

    [Route("api/[controller]")]
    public class HomeController : Controller
    {
	    private readonly ImageService _service;
	    public HomeController(ImageService service)
	    {
		    _service = service;
	    }

	    [HttpPost]
		[RequestSizeLimit(10000000)]
	    public async Task<JsonResult> Post(Model model)
		    => Json(await model.File.OpenReadStream()
			    .Using(stream => _service.AddImage(stream, model.EventName, model.Filename ?? model.File.FileName, model.Setting, model.Key)));
        

        [HttpPut("{id}")]
        public string Put(string id)
		{
			// TODO: Решить как обеспечить безопасность при регистрации мероприятия, чтобы нельзя было нарегистрировать кучу мероприятий.
			return _service.RegisterEvent(id);
		}

        [HttpDelete("{id}")]
        public JsonResult Delete(string id, string filename, string key)
		{
			return new JsonResult(_service.RemoveImage(id, filename, key));
		}
    }
}
