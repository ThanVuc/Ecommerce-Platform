using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EPlatform_API.Models;

namespace EPlatform_API.IServices
{
    public interface IBlobServices
    {
        Task UpdloadImageAsync(string name, string filePath, string contentType);
        Task UpdloadImageAsync(FileStreamModel file);
        Task UpdloadImagesAsync(List<FileStreamModel> files);
        Task<Stream> DownloadFileAsync(string name);
        Task DeleteFilePermanentAsync(string name);
        Task DeleteFilePermanentAsync(List<string> name);
        Task<string> GetImageUriOfProduct(string productId);
        Task<List<string>> GetImageUriListOfProduct(List<string> productsId);
        Task<Stream> ResizeImageAsync(string name, int width, int height);
        Stream ConvertToWebP(Stream file);
    }
}