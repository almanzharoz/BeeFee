using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SharpFuncExt;

namespace BeeFee.OrganizerApp.Services
{
	public class AcceptModel
	{
		public string TempPath { get; set; }
		public ImageSaveSetting[] Images { get; set; }
	}

	public struct ImageSaveSetting
	{
		public Size Size { get; set; }
		public string Path { get; set; }

		public ImageSaveSetting(string path, int width, int height)
		{
			Path = path;
			Size = new Size {Width = width, Height = height};
		}
	}

	public struct Size
	{
		public int Width { get; set; }
		public int Height { get; set; }
	}

	public class ImagesService
	{
		private readonly string _imagesHost;
		public ImagesService(string imagesHost)
		{
			_imagesHost = imagesHost.HasNotNullArg(nameof(imagesHost));
		}

		public void GetAccess(string folder, string token, string host)
			=> folder.HasNotNullArg(nameof(folder)).Using(x => new WebClient {BaseAddress = _imagesHost},
				(u, c) => c.DownloadDataAsync(new Uri(String.Concat(_imagesHost, "/api/home/access?directory=", u, "&remoteIp=", host, "&token=", token))));

		public Task<bool> Accept(AcceptModel model)
			=> model.Using(x => new WebClient {BaseAddress = _imagesHost}.Fluent(c => c.Headers[HttpRequestHeader.ContentType] = "application/json"),
				async (m, c) => Convert.ToBoolean((byte)(
					await c.UploadStringTaskAsync("/api/home", "PUT",
						JsonConvert.SerializeObject(model)))[0]));

		///// <summary>
		///// Заливаем логотип через сервер
		///// </summary>
		///// <param name="companyUrl"></param>
		///// <param name="stream"></param>
		///// <returns></returns>
		//public Task<string> AddCompanyLogo(string companyUrl, Stream stream)
		//	=> SendPostFile(String.Concat(_imagesHost, "/api/home?companyName=", companyUrl.HasNotNullArg(nameof(companyUrl))), "logo.jpg", stream);

		//public Task<string> AddEventCover(string companyUrl, string eventUrl, string filename, Stream stream)
		//	=> SendPostFile(String.Concat(_imagesHost, "/api/home?setting=event&companyName=", companyUrl.HasNotNullArg(nameof(companyUrl)), "&eventName=", eventUrl.HasNotNullArg(nameof(eventUrl))), filename, stream);

		//public async Task<string> SendPostFile(string url, string filename, Stream stream)
		//{
		//	using (var client = new HttpClient())
		//	using (var content = new MultipartFormDataContent())
		//	{
		//		//client.BaseAddress = new Uri(url);
		//		//var values = new[]
		//		//{
		//		//	new KeyValuePair<string, string>("Name", "name"),
		//		//	new KeyValuePair<string, string>("id", "id"),
		//		//};
		//		//foreach (var keyValuePair in values)
		//		//{
		//		//	content.Add(new StringContent(keyValuePair.Value), keyValuePair.Key);
		//		//}
		//		var a = new byte[stream.Length];
		//		stream.Read(a, 0, a.Length);
		//		var fileContent = new ByteArrayContent(a);
		//		fileContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
		//		{
		//			FileName = filename, Name = "file"
		//		};
		//		content.Add(fileContent);

		//		var r = await client.PostAsync(url, content);
		//		var s = await r.Content.ReadAsStringAsync();
		//		return JsonConvert.DeserializeObject<Dictionary<string, string>>(s)["path"];
		//	}
		//}

	}
}