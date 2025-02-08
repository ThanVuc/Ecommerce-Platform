using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using EPlatform_API.ExtensionMethods;
using EPlatform_API.IServices;
using EPlatform_API.Models;
using SkiaSharp;

namespace EPlatform_API.Services
{
    public class BlobServices : IBlobServices
    {
        private readonly IConfiguration _configuration;
        protected BlobContainerClient _publicImageContainerClient;

        public BlobServices(
            IConfiguration configuration,
            string containerName
        )
        {
            _configuration = configuration;
            _publicImageContainerClient = new BlobContainerClient(
                _configuration.GetConnectionString("AzureBlobStorage"),
                containerName
            );
        }

        public FileStreamModel ConvertToFileStreamModel(string fileName, Stream stream)
        {
            return new FileStreamModel
            {
                Name = fileName,
                Stream = stream
            };
        }

        public FileStreamModel ConvertToFileStreamModel(string fileName, byte[] byteStream)
        {
            var stream = new MemoryStream(byteStream);
            return new FileStreamModel
            {
                Name = fileName,
                Stream = stream
            };
        }

        public FileStreamModel ConvertToFileStreamModel(string fileName, IFormFile file)
        {
            return new FileStreamModel
            {
                Name = fileName,
                Stream = file.OpenReadStream()
            };
        }

        public async Task DeleteFilePermanentAsync(string name)
        {
            var blobClient = _publicImageContainerClient.GetBlobClient(name);
            await blobClient.DeleteIfExistsAsync();
        }

        public async Task DeleteFilePermanentAsync(List<string> names)
        {
            var tasks = new List<Task>();
            foreach (var n in names)
            {
                tasks.Add(DeleteFilePermanentAsync(n));
            }
            await Task.WhenAll(tasks);
        }

        public async Task<Stream> DownloadFileAsync(string name)
        {
            var blobClient = _publicImageContainerClient.GetBlobClient(name);
            if (!await blobClient.ExistsAsync())
            {
                throw new Exception("File not found");
            }
            Stream stream = await blobClient.OpenReadAsync();
            return stream;
        }

        public Task<List<string>> GetImageUriListOfProduct(List<string> productsId)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetImageUriOfProduct(string productId)
        {
            throw new NotImplementedException();
        }

        public Task<Stream> ResizeImageAsync(string name, int width, int height)
        {
            throw new NotImplementedException();
        }

        public Stream ConvertToWebP(Stream file)
        {
            /***
                * 1. Decode file to bitmap
                * 2. Encode bitmap to webp
                * 3. Return stream
            ***/
            using (var bitmap = SKBitmap.Decode(file))
            {
                var imageFormat = SKEncodedImageFormat.Webp;
                int quality = 80;
                Stream outputStream = new MemoryStream();

                bitmap.Encode(outputStream, imageFormat, quality);
                return (Stream)outputStream;

            }
        }

        public async Task<string> UpdloadImageAsync(string name, string filePath, string contentType)
        {
            /***
                * 1. Get blob client
                * 2. Open file stream
                * 3. Convert to webp
                * 4. Upload to blob
                * 5. Set content type
                * 6. Return url
            ***/
            var blobClient = _publicImageContainerClient.GetBlobClient(name);
            var stream = (Stream)File.OpenRead(filePath);
            stream = ConvertToWebP(stream);
            stream.Position = 0;
            await blobClient.UploadAsync(stream, new BlobHttpHeaders { ContentType = "image/webp" });
            var url = blobClient.Uri.AbsoluteUri;
            return url;
        }

        public async Task<string> UpdloadImageAsync(FileStreamModel file)
        {
            if (file.Stream == null)
            {
                throw new Exception("File stream is null");
            }

            var blobClient = _publicImageContainerClient.GetBlobClient(file.Name);

            file.Stream = ConvertToWebP(file.Stream);
            file.Stream.Position = 0;
            // save image-uri to blob file system
            await blobClient.UploadAsync(file.Stream, new BlobHttpHeaders { ContentType = "image/webp" });
                        var url = blobClient.Uri.AbsoluteUri;
            return url;
        }

        public async Task UpdloadImagesAsync(List<FileStreamModel> files)
        {
            var task = new List<Task>();

            foreach (var file in files)
            {
                task.Add(UpdloadImageAsync(file));
            }

            await Task.WhenAll(task);
        }
    }
}