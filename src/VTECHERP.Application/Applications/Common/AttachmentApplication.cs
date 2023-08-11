using DocumentFormat.OpenXml.Vml.Spreadsheet;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Volo.Abp.Validation;
using VTECHERP.DTOs.Attachment;
using VTECHERP.DTOs.Base;
using VTECHERP.DTOs.Entries;
using VTECHERP.Enums;
using VTECHERP.Helper;
using VTECHERP.Services;

namespace VTECHERP
{
    [Route("api/app/Attachment/[action]")]
    public class AttachmentApplication : ApplicationService
    {
        private readonly IAttachmentService _attachmentService;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IConfiguration _configuration; 
        public AttachmentApplication(IAttachmentService attachmentService, IHostingEnvironment hostingEnvironment,
            IConfiguration configuration)
        {
            _attachmentService = attachmentService;
            _hostingEnvironment = hostingEnvironment;
            _configuration = configuration;
        }

        [HttpPost]
        public async Task<IActionResult> Upload([FromForm] UploadAttachmentRequest request)
        {
            List<string> imageExtensions = new List<string>() { ".bmp", ".jpg", ".jpeg", ".png" };
            var isImage = true;
            foreach (var item in request.formFiles)
            {
                if (item.Length > (20 * 1024 * 1024))
                    return new BadRequestObjectResult(new
                    {
                        HttpStatusCode = 400,
                        Data = new PagingResponse<AttachmentResponse>(),
                        Message = "File không được vượt quá 20MB"
                    });

                if (!imageExtensions.Any(x=>x == Path.GetExtension(item.FileName)))
                    isImage = false;
            }
            //227d9c84-82f8-a97e-ee74-3a0ce15a2d8b
            if (request.ObjectType != AttachmentObjectType.PaymentReceipt && request.ObjectType != AttachmentObjectType.Entry && !isImage)
                return new BadRequestObjectResult(new
                {
                    HttpStatusCode = 400,
                    Data = new PagingResponse<AttachmentResponse>(),
                    Message = "Chỉ được tải ảnh."
                });

            //string apiUrl = _configuration.GetValue<string>("HostSetting:API");
            string apiUrl = _configuration.GetValue<string>("Settings:Abp.Host.Api");
            var fileModels = await FileUploadHelper.GetFilesFromForm(request.formFiles);
            var uploadFiles = await FileUploadHelper.UploadFile(fileModels, _hostingEnvironment.ContentRootPath, apiUrl);
            var result = await _attachmentService.Save(request, uploadFiles);
            return new OkObjectResult(new
            {
                HttpStatusCode = 200,
                Data = result,
                Message = "Tải lên thành công."
            });
        }

        [HttpDelete]
        public async Task<bool> Delete(Guid id)
        {
            FileUploadHelper.DeleteFile(_attachmentService.GetPathFileById(id).Result, _hostingEnvironment.ContentRootPath);
            await _attachmentService.Delete(id);
            return true;
        }

        [HttpPost]
        public async Task<PagingResponse<AttachmentResponse>> Search(SearchAttachmentRequest request)
        {
            return await _attachmentService.Search(request);
        }

        [HttpPost]
        public async Task<List<AttachmentModuleResponse>> AttachmentByObjectIdAsync(Guid objectId, AttachmentObjectType? objectType = null)
        {
            return await _attachmentService.AttachmentByObjectIdAsync(objectId, objectType);
        }
    }
}
