using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BeeFee.ImageApp;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SharpFuncExt;

namespace BeeFee.ImagesWebApplication.Controllers
{
	public class ImgSize
	{
		public int Width { get; set; }
		public int Height { get; set; }
	}

	public class Model
	{
		public ImgSize[] Sizes { get; set; }
		public IFormFile File { get; set; }
		public string Filename { get; set; }
	}

    [Route("api/[controller]")]
    public class HomeController : Controller
    {
	    private readonly ImageService _service;
	    public HomeController(ImageApp.ImageService service)
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
	    public async Task<JsonResult> Post(Model model)
		    => Json(await model.File.OpenReadStream()
			    .Using(stream => _service.AddImage(stream, model.Filename ?? model.File.FileName,
				    model.Sizes.Select(x => new ImageSize(x.Width, x.Height)).ToArray())));
        

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
