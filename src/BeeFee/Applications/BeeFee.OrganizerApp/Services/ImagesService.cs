using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
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

		/// <summary>
		/// Заливаем логотип через сервер
		/// </summary>
		/// <param name="companyUrl"></param>
		/// <param name="stream"></param>
		/// <returns></returns>
		public Task<string> AddCompanyLogo(string companyUrl, Stream stream)
			=> SendPostFile(String.Concat(_imagesHost, "/api/home?companyName=", companyUrl.HasNotNullArg(nameof(companyUrl))), stream);

		public async Task<string> SendPostFile(string url, Stream stream)
		{
			using (var client = new HttpClient())
			using (var content = new MultipartFormDataContent())
			{
				//client.BaseAddress = new Uri(url);
				//var values = new[]
				//{
				//	new KeyValuePair<string, string>("Name", "name"),
				//	new KeyValuePair<string, string>("id", "id"),
				//};
				//foreach (var keyValuePair in values)
				//{
				//	content.Add(new StringContent(keyValuePair.Value), keyValuePair.Key);
				//}
				var a = new byte[stream.Length];
				stream.Read(a, 0, a.Length);
				var fileContent = new ByteArrayContent(a);
				fileContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
				{
					FileName = Path.GetFileName("file.png"), Name = "file"
				};
				content.Add(fileContent);

				var r = await client.PostAsync(url, content);
				return JsonConvert.DeserializeObject<Dictionary<string, string>>(await r.Content.ReadAsStringAsync())["Path"];
			}
		}

	}
}