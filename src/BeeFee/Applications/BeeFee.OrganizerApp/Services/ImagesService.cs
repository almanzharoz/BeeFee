using System.Net;
using System.Threading.Tasks;
using SharpFuncExt;

namespace BeeFee.OrganizerApp.Services
{
	public class ImagesService
	{
		private readonly string _imagesHost;
		public ImagesService(string imagesHost)
		{
			_imagesHost = imagesHost.HasNotNullArg(nameof(imagesHost));
		}

		public Task<string> RegisterEvent(string url)
			=> url.HasNotNullArg(nameof(url)).Using(x => new WebClient {BaseAddress = _imagesHost},
				async (u, c) => await c.UploadStringTaskAsync("/api/home/"+url, "PUT", ""));
	}
}