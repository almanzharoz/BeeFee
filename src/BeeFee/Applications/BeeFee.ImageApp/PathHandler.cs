using System;
using System.IO;
using BeeFee.ImageApp.Embed;

namespace BeeFee.ImageApp
{
	public class PathHandler
	{
		private readonly string _parentDirectory;
		private readonly string _privateOriginalDirectory;
		private readonly string _publicOriginalFolder;
		private readonly string _resizedFolder;
		private readonly string _usersDirectory;
		private readonly string _companiesDirectory;
		private readonly string _userAvatarFileName;
		private readonly string _companyLogoFileName;

		internal PathHandler(string parentDirectory, string privateOriginalDirectory, string publicOriginalFolder,
			string resizedFolder, string usersDirectory, string companiesDirectory, string userAvatarFileName, string companyLogoFileName)
		{
			_parentDirectory = parentDirectory;
			_privateOriginalDirectory = privateOriginalDirectory;
			_publicOriginalFolder = publicOriginalFolder;
			_resizedFolder = resizedFolder;
			_usersDirectory = usersDirectory;
			_companiesDirectory = companiesDirectory;
			_userAvatarFileName = userAvatarFileName;
			_companyLogoFileName = companyLogoFileName;
		}

		private string GetParentDirectory(EImageType imageType, string companyOrUserName, string eventName = "")
		{
			switch(imageType)
				{
					case EImageType.CompanyLogo:
						return Path.Combine(_parentDirectory, _publicOriginalFolder, _companiesDirectory, companyOrUserName);
					case EImageType.UserAvatar:
						return Path.Combine(_parentDirectory, _publicOriginalFolder, _usersDirectory, companyOrUserName);
					case EImageType.EventPrivateOriginalImage:
						return Path.Combine(_parentDirectory, _privateOriginalDirectory, companyOrUserName, eventName);
					case EImageType.EventPublicOriginalImage:
						return Path.Combine(_parentDirectory, _publicOriginalFolder, companyOrUserName, eventName);
					case EImageType.EventResizedImage:
						return Path.Combine(_parentDirectory, _resizedFolder, companyOrUserName, eventName);
					default:
						throw new ArgumentOutOfRangeException(nameof(imageType), imageType, null);
				}
		}

		private string GetFileNameByType(EImageType imageType)
		{
			// ReSharper disable once SwitchStatementMissingSomeCases
			switch (imageType)
			{
				case EImageType.CompanyLogo:
					return _companyLogoFileName;
				case EImageType.UserAvatar:
					return _userAvatarFileName;
				default:
					throw new ArgumentOutOfRangeException(nameof(imageType), imageType, null);
			}
		}

		internal string GetPathToLogoOrAvatar(string companyOrUserName, EImageType imageType)
			=> Path.Combine(GetParentDirectory(imageType, companyOrUserName), GetFileNameByType(imageType));

		internal string GetPathToSizeDirectory(string companyName, string eventName, ImageSize size)
			=> Path.Combine(GetParentDirectory(EImageType.EventResizedImage, companyName, eventName), size.ToString());

		internal string GetPathToImageSize(string companyName, string eventName, ImageSize size, string fileName)
			=> Path.Combine(GetPathToSizeDirectory(companyName, eventName, size), fileName);

		internal string GetPathToOriginalDirectoryByImageType(string companyName, string eventName, EImageType imageType)
			=> Path.Combine(GetParentDirectory(imageType, companyName, eventName));

		internal string GetPathToOriginalImage(string companyName, string eventName, EImageType imageType, string fileName)
			=> Path.Combine(GetPathToOriginalDirectoryByImageType(companyName, eventName, imageType), fileName);

		internal bool IsEventImageExists(string companyName, string eventName, string fileName)
			=> File.Exists(
				   Path.Combine(
					   GetPathToOriginalDirectoryByImageType(companyName, eventName, EImageType.EventPrivateOriginalImage),
					   fileName)) ||
			   File.Exists(Path.Combine(
				   GetPathToOriginalDirectoryByImageType(companyName, eventName, EImageType.EventPublicOriginalImage), fileName));

		internal string GetUniqueName(string companyName, string eventName, string fileName)
		{
			var nameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);

			var index = 0;
			var newName = fileName;

			while (IsEventImageExists(companyName, eventName, newName))
			{
				newName = nameWithoutExtension + index + Path.GetExtension(fileName);
				//fullName = Path.Combine(_folder, _privateOriginalFolder, result);
				index++;
			}
			return newName;
		}
	}
}