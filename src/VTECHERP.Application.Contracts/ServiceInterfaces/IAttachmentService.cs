using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using VTECHERP.DTOs.Attachment;
using VTECHERP.DTOs.Base;
using VTECHERP.Enums;
using VTECHERP.Models;


namespace VTECHERP.Services
{
    public interface IAttachmentService : IScopedDependency
    {
        Task<PagingResponse<AttachmentResponse>> Save(UploadAttachmentRequest request, UploadFileResult uploadFile);
        Task Delete(Guid id);
        Task<string> GetPathFileById(Guid id);
        Task<PagingResponse<AttachmentResponse>> Search(SearchAttachmentRequest request);
        Task<List<AttachmentDetailDto>> GetByObjectId(Guid objectId);
        Task<List<DetailAttachmentDto>> GetAttachmentByObjectIdAsync(Guid objectId);
        Task<List<DetailAttachmentDto>> ListAttachmentByObjectIdAsync(List<Guid> objectIds);
        Task<List<AttachmentModuleResponse>> AttachmentByObjectIdAsync(Guid objectId, AttachmentObjectType? objectType = null);
    }
}
