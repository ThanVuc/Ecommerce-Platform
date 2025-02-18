using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EPlatform_API.Models;

namespace EPlatform_API.IServices
{
    public interface IBlobServices
    {
        public FileStreamModel ConvertToFileStreamModel(string fileName, Stream stream);
        public FileStreamModel ConvertToFileStreamModel(string fileName, byte[] byteStream);
        public FileStreamModel ConvertToFileStreamModel(string fileName, IFormFile file);
        Task<string> UploadImageAsync(string name, string filePath, string contentType);
        Task<string> UploadImageAsync(FileStreamModel file);
        Task UploadImagesAsync(List<FileStreamModel> files);
        Task<Stream> DownloadFileAsync(string name);
        Task DeleteFilePermanentAsync(string name);
        Task DeleteFilePermanentAsync(List<string> name);
        Task<string> GetImageUriOfProduct(string productId);
        Task<List<string>> GetImageUriListOfProduct(List<string> productsId);
        Task<Stream> ResizeImageAsync(string name, int width, int height);
        Stream ConvertToWebP(Stream file);
        Task<string?> UpdateImageAsync(string? oldFileName, FileStreamModel file);

    }
}