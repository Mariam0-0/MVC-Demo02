using GymManagement.BLL.Services.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace GymManagement.BLL.Services.Classes
{
    public class AttachmentService : IAttachmentService
    {
        private readonly ILogger<AttachmentService> _logger;
        private readonly long _maxFileSize = 5 *1024 * 1024;
        private readonly string[] _allowedExtensions = { ".png", ".jpeg", ".jpg" };
        private readonly IWebHostEnvironment _env;
        public AttachmentService(ILogger<AttachmentService> logger, IWebHostEnvironment env) 
        {
            _logger = logger;
            _env = env;
        }
        public async Task<string?> UploadAsync(Stream fileStream, string filename, string folderName, CancellationToken ct = default)
        {
            if (fileStream is null || !fileStream.CanRead) return null;
            if (fileStream.Length == 0) return null;

            // check file size
            if(fileStream.Length > _maxFileSize)
            {
                _logger.LogError($"File Rejected: Too Large {fileStream.Length} Bytes");
                return null;
            }

            // check extensions
            var extension = Path.GetExtension(filename);
            if(String.IsNullOrWhiteSpace(extension) || !_allowedExtensions.Contains(extension))
            {
                _logger.LogError($"File Rejected: This Extension Not Allowed");
                return null;
            }

            // locate folder
            // locate membersPhotos
            var uploadsFolder = Path.Combine(_env.ContentRootPath, folderName);
            Directory.CreateDirectory(uploadsFolder);
            //if folder exist return it, if not create folder

            var storedFilename = $"{Guid.NewGuid()}{filename}";

            var FilePath = Path.Combine(uploadsFolder, storedFilename);


            // file stream
            try
            {

                using var fs = new FileStream(FilePath, FileMode.Create, FileAccess.Write);
                await fileStream.CopyToAsync(fs, ct);
                return storedFilename;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to upload photo");
                return null;
            }
        }

        public bool Delete(string fileName, string folderName)
        {
            var fullPath = Path.Combine(_env.ContentRootPath, folderName, fileName);
            try
            {
                if (!File.Exists(fullPath)) return false;
                File.Delete(fullPath);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete file");
                return false;
            }
        }

        public (Stream stream, string contentType)? GetFile(string fileName, string folderName)
        {
            if(String.IsNullOrWhiteSpace(fileName) || String.IsNullOrWhiteSpace(folderName)) return null;

            var fullPath = Path.Combine(_env.ContentRootPath, folderName, fileName);
            if (!File.Exists(fullPath)) return null;

            var stream = new FileStream(fullPath, FileMode.Open, FileAccess.Read);
            var extension = Path.GetExtension(fullPath).ToLower();
            var contentType = extension switch
            {
                ".png" => "image/png",
                ".jpg" or ".jpeg" => "image/jpeg",
                _ => "application/octet-stream" // binary data
            };

            return (stream, contentType);
        }

    }
}
