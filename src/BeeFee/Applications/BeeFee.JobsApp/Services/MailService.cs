using System.IO;
using System.Net.Mail;
using System.Text;
using BeeFee.JobsApp.Projections;
using BeeFee.Model;
using BeeFee.Model.Jobs.Data;
using Core.ElasticSearch;
using Microsoft.Extensions.Logging;
using SharpFuncExt;

namespace BeeFee.JobsApp.Services
{
	public class MailService : BaseJobsService<SendMailJob, SendMail>
	{
		private readonly MailServiceSettings _serviceSettings;
		public MailService(ILoggerFactory loggerFactory, BeefeeElasticConnection settings, ElasticScopeFactory<BeefeeElasticConnection> factory, MailServiceSettings serviceSettings) 
			: base(loggerFactory, settings, factory, null)
		{
			_serviceSettings = serviceSettings;
		}

		public bool SendNextMail()
			=> JobExecute(GetNextJob(), SendMail);

		//TODO: Перенести настройки SMTP
		private void SendMail(SendMail data)
			=> data.Using(d => new SmtpClient(){PickupDirectoryLocation = _serviceSettings.PickupDirectory, DeliveryMethod = SmtpDeliveryMethod.SpecifiedPickupDirectory},
				(mail, client) => mail.Using(CreateMessage, (d, m) =>
				{
					client.Send(m);
					m.Attachments.Each(x => x.Dispose());
				}));

		private MailMessage CreateMessage(SendMail data)
			=> new MailMessage(data.From, data.To, data.Subject, data.Message) {IsBodyHtml = true/*, BodyEncoding = Encoding.UTF8*/}
				.If(m => !data.Files.IsNull(), m => data.Files.Each(f => m.Attachments.Add(new Attachment(GetFile(f)))));

		private string GetFile(string path)
			=> Path.Combine(
				_serviceSettings.HasNotNullArg(x => x.AttachmentsFolder, nameof(_serviceSettings.AttachmentsFolder))
					.AttachmentsFolder, path.HasNotNullArg(nameof(path)));
	}
}