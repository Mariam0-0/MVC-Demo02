using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagement.BLL.Services.Interfaces
{
    public interface IAttachmentService
    {
        // fileStream
        Task<string?> UploadAsync(Stream fileStream, string filename, string folderName, CancellationToken ct = default);

        bool Delete(string fileName, string folderName);
        (Stream stream, string contentType)? GetFile(string fileName, string folderName);
        
    }
}
