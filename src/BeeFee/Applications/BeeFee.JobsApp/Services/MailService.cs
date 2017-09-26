using System;
using System.IO;
using System.Net;
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

		private void SendMail(SendMail data)
			=> data.Using(d => CreateSmtpClient(),
				(mail, client) => mail.Using(CreateMessage, (d, m) =>
				{
					try
					{
						client.Send(m);
					}
					finally
					{
						m.Attachments.Each(x => x.Dispose());
					}
				}));

		private SmtpClient CreateSmtpClient()
			=> _serviceSettings.If(x => x.PickupDirectory.NotNull(),
				x => new SmtpClient
				{
					PickupDirectoryLocation = x.PickupDirectory,
					DeliveryMethod = SmtpDeliveryMethod.SpecifiedPickupDirectory
				},
				x => new SmtpClient(x.Host, x.Port)
				{
					DeliveryMethod = SmtpDeliveryMethod.Network,
					EnableSsl = x.Ssl,
					Credentials = x.User.NotNull() ? new NetworkCredential(x.User, x.Password) : null
				});

		private MailMessage CreateMessage(SendMail data)
			=> new MailMessage(data.From ?? _serviceSettings.From, data.To, data.Subject, data.Message) {IsBodyHtml = true/*, BodyEncoding = Encoding.UTF8*/}
				.If(m => !data.Files.IsNull(), m => data.Files.Each(f => m.Attachments.Add(new Attachment(GetFile(f)))));

		private string GetFile(string path)
			=> Path.Combine(
				_serviceSettings.HasNotNullArg(x => x.AttachmentsFolder, nameof(_serviceSettings.AttachmentsFolder))
					.AttachmentsFolder, path.HasNotNullArg(nameof(path)));
	}
}