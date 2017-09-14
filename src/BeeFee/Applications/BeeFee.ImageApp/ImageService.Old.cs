//using System;
//using System.IO;
//using System.Linq;
//using System.Threading.Tasks;
//using ImageSharp;
//using ImageSharp.Formats;
//using ImageSharp.Processing;
//using SixLabors.Primitives;

//namespace BeeFee.ImageApp
//{
//    public class ImageServiceOld
//    {
//	    private readonly string _folder;
//	    public ImageServiceOld(string folder)
//	    {
//		    _folder = folder;
//	    }

//	    public async Task<ImageOperationResult> AddImage(Stream stream, string filename, ImageSize[] sizes)
//	    {
//		    try
//		    {
//			    var fullpath = Path.Combine(_folder, filename);
//			    if (File.Exists(fullpath))
//				    return new ImageOperationResult(EAddImageResut.Exists, filename, null);
//				if (!Directory.Exists(Path.Combine(_folder, Path.GetDirectoryName(filename))))
//				    Directory.CreateDirectory(Path.Combine(_folder, Path.GetDirectoryName(filename)));
//			    using (stream)
//			    {
//				    await SaveToFile(stream, fullpath);
//				    stream.Position = 0;
//				    if (sizes != null && sizes.Any())
//				    {
//					    var image = Image.Load(stream);
//					    return await Task.Run(() =>
//					    {
//						    try
//						    {
//							    foreach (var size in sizes)
//								    ResizeImage(image, size, filename);
//						    }
//						    catch (Exception e)
//						    {
//							    return new ImageOperationResult(EAddImageResut.Error, filename, e.Message);
//						    }
//							finally
//							{
//							    image.Dispose();
//						    }
//						    return new ImageOperationResult(EAddImageResut.Ok, filename, null);
//					    }).ConfigureAwait(false);
//				    }
//			    }
//		    }
//		    catch (Exception e)
//		    {
//			    return new ImageOperationResult(EAddImageResut.Error, filename, e.Message);
//		    }
//		    return new ImageOperationResult(EAddImageResut.Ok, filename, null);
//	    }

//		public string GetImageUrl(ImageSize size, string filename)
//	    {
//		    var path = Path.Combine(_folder, $"{size.Width}_{size.Height}", filename);
//		    return File.Exists(path) ? $"/{size}/{filename}" : "";
//	    }

//	    public string GetImageUrl(string filename)
//	    {
//		    var path = Path.Combine(_folder, filename);
//		    return File.Exists(path) ? $"/{filename}" : "";
//	    }

//	    public void Remove(string filename)
//	    {
//		    var dirInfo = new DirectoryInfo(_folder);
//		    foreach (var dir in dirInfo.GetDirectories())
//			    dir.EnumerateFiles(filename).ToList().ForEach(x => x.Delete());
//		    dirInfo.EnumerateFiles(filename).ToList().ForEach(x => x.Delete());
//	    }

//	    public async Task<ImageOperationResult> Update(Stream stream, string filename, ImageSize[] sizes)
//	    {
//		    var files = new DirectoryInfo(_folder).GetFiles(filename, SearchOption.AllDirectories);
//		    foreach (var file in files)
//		    {
//			    var isExists = sizes
//				    .Any(size =>
//					    file.Directory.Name
//						    .Contains(size.ToString()));
//			    if(!isExists) return new ImageOperationResult(EAddImageResut.Error, filename, "Not all sizes for changing");
//		    }

//		    Remove(filename);
//		    return await AddImage(stream, filename, sizes);
//	    }

//	    private void ResizeImage(Image<Rgba32> image, ImageSize size, string filename)
//	    {
//		    var folder = Path.Combine(_folder, Path.GetDirectoryName(filename), size.ToString());
//		    var minfullpath = Path.Combine(folder, Path.GetFileName(filename));
//		    if (!Directory.Exists(folder))
//			    Directory.CreateDirectory(folder);
//		    using (var newimg = new Image<Rgba32>(image))
//			    newimg.Resize(new ResizeOptions() {Mode = ResizeMode.Max, Size = new Size(size.Width, size.Height)})
//				    .Save(minfullpath, new JpegEncoder {Quality = 85});
//	    }


//		private async Task SaveToFile(Stream stream, string filename)
//	    {
//		    var buffer = new byte[65000];
//		    var task = Task.CompletedTask;
//		    var len = 1;
//		    using (var writeStream = File.Create(filename))
//		    {
//			    while (len > 0)
//			    {
//				    await task;
//				    len = stream.Read(buffer, 0, buffer.Length);
//					if (len > 0)
//						task = writeStream.WriteAsync(buffer, 0, len);
//			    }
//			    await task;
//		    }
//		}
//	}
//}
