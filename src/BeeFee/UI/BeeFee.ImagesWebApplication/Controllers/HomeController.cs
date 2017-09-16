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
	}

    [Route("api/[controller]")]
    public class HomeController : Controller
    {
	    private readonly ImageService _service;
	    public HomeController(ImageService service)
	    {
		    _service = service;
	    }

        // GET api/values
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
	    [HttpPost]
		[RequestSizeLimit(10000000)]
	    public async Task<JsonResult> Post(Model model)
		    => Json(await model.File.OpenReadStream()
			    .Using(stream => _service.AddImage(stream, model.Filename ?? model.File.FileName, model.Setting)));
        

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
