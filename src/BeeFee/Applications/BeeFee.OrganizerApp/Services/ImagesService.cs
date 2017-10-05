using System;
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

		public void GetAccessToFolder(string companyUrl, string host)
			=> companyUrl.HasNotNullArg(nameof(companyUrl)).Using(x => new WebClient {BaseAddress = _imagesHost},
				(u, c) => c.DownloadDataAsync(new Uri(String.Concat(_imagesHost, "/api/home/", u, "?host=", host))));

		public Task<bool> RegisterEvent(string companyUrl, string eventUrl, string host)
			=> eventUrl.HasNotNullArg(nameof(eventUrl)).Using(x => new WebClient {BaseAddress = _imagesHost},
				async (u, c) => Convert.ToBoolean(
					(await c.UploadDataTaskAsync(String.Concat("/api/home/", companyUrl, "/", eventUrl, "?host=", host), "PUT", new byte[0]))[0]));

	}
}