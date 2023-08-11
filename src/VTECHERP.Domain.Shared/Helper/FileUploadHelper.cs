using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using VTECHERP.Models;

namespace VTECHERP.Helper
{
    public class FileUploadHelper
    {
        public static async Task<UploadFileResult> UploadFile(IEnumerable<FileModel> files, string contentRootPath, string contentUrl)
        {
            if(files == null || files.Count() == 0) return null;

            var uploadKey = Guid.NewGuid().ToString();
            var uploadFileResults = new List<UploadFileResultDetail>();
            string uploadsFolder = Path.Combine($"Resources/Attachments/{uploadKey}");

            bool uploadFolderExist = Directory.Exists(Path.Combine(contentRootPath, uploadsFolder));
            if (!uploadFolderExist)
            {
                Directory.CreateDirectory(Path.Combine(contentRootPath, uploadsFolder));
            }

            foreach (var file in files)
            {
                string uniqueFileName = file.Name;

                if (string.IsNullOrEmpty(uniqueFileName)) uniqueFileName = Guid.NewGuid().ToString();

                if (!string.IsNullOrEmpty(file.Extensions)) uniqueFileName = uniqueFileName + "." + file.Extensions;

                string filePath = Path.Combine(uploadsFolder, $"{uniqueFileName}");
                var physicalFilePath = Path.Combine(contentRootPath, filePath);
                using (var fileStream = new FileStream(physicalFilePath, FileMode.Create))
                {
                    await fileStream.WriteAsync(file.Content, 0, file.Content.Length);
                }

                var url = Uri.EscapeDataString($"{contentUrl}/{filePath}");

                uploadFileResults.Add(new UploadFileResultDetail(file.Name, url, file.Extensions, filePath));
            }

            return new UploadFileResult
            {
                Key = uploadKey,
                Files = uploadFileResults,
            };
        }

        public static bool DeleteFolder(string attachments, string contentRootPath)
        {
            try
            {
                var attachObj = JsonSerializer.Deserialize<UploadFileResult>(attachments);

                string uploadsFolder = Path.Combine($"Resources/Attachments/{attachObj.Key}");

                bool uploadFolderExist = Directory.Exists(Path.Combine(contentRootPath, uploadsFolder));
                if (uploadFolderExist)
                {
                    Directory.Delete(Path.Combine(contentRootPath, uploadsFolder));
                }

                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public static bool DeleteFile(string pathFile, string contentRootPath)
        {
            try
            {
                bool uploadFolderExist = Directory.Exists(Path.Combine(contentRootPath, pathFile));
                if (uploadFolderExist)
                {
                    Directory.Delete(Path.Combine(contentRootPath, pathFile));
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static async Task<List<FileModel>> GetFilesFromForm(IEnumerable<IFormFile>? files)
        {
            var fileModels = new List<FileModel>();
            if (files != null)
            {
                foreach (var file in files)
                {
                    if (file.Length > 0)
                    {
                        using var ms = new MemoryStream();
                        await file.CopyToAsync(ms);
                        fileModels.Add(new FileModel
                        {
                            Content = ms.ToArray(),
                            Name = file.FileName
                        });
                    }
                }
            }

            return fileModels;
        }

        public static async Task<List<FileModel>> GetFilesFromForm(IEnumerable<string>? files)
        {
            var fileModels = new List<FileModel>();
            if (files != null)
            {
                foreach (var file in files)
                {
                    var base64EncodedBytes = System.Convert.FromBase64String(file);
                    fileModels.Add(new FileModel
                    {
                        Content = base64EncodedBytes,

                    });
                }
            }

            return fileModels;
        }

        public static async Task<List<FileModel>> GetFilesFromForm(IEnumerable<FileAttachment>? files)
        {
            var fileModels = new List<FileModel>();
            if (files != null)
            {
                foreach (var file in files)
                {
                    if(string.IsNullOrEmpty(file.FileName)) continue;

                    var strBase64 = file.Base64;
                    if(!string.IsNullOrEmpty(strBase64) && strBase64.StartsWith("data:"))
                    {
                        var arr = strBase64.Split(";base64,");
                        strBase64 = arr[1];
                    }

                    var base64EncodedBytes = System.Convert.FromBase64String(strBase64);
                    fileModels.Add(new FileModel
                    {
                        Content = base64EncodedBytes,
                        Name = file.FileName,
                        Extensions = file.Extensions,
                        ContentType = file.ContentType
                    });
                }
            }

            return fileModels;
        }

        public static string AppendResponseFileName(string fileName)
        {
            var now = DateTime.UtcNow;
            var lastDot = fileName.LastIndexOf('.');
            var name = fileName.Substring(0, lastDot);
            var extension = fileName.Substring(lastDot + 1);
            name = $"{name}_{now:yyyy_MMM_dd_HH_mm_ss}";
            return $"{name}.{extension}";
        }
    }
}
