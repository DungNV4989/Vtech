using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using VTECHERP.DTOs.Base;
using VTECHERP.Enums;

namespace VTECHERP.Services
{
    public interface ICommonService : IScopedDependency
    {
        Task<MasterDataDTO?> GetAudience(AudienceTypes type, Guid? id);

        string GetDocumentTypeName(DocumentTypes type);

        Task<List<MasterDataDTO>> GetAudiences(params (AudienceTypes, Guid?)[] request);

        Task TriggerCalculateSupplierOrderReport(Guid supplierId, DateTime date, bool isOrder = false);

        bool ValidatePhoneNumber(string phoneNumber);

        bool ValidateEmail(string email);

        bool ValidatePassword(string password);

        Task ClearDataInTables(string _connectionString, Guid tenantId, string columnName);

        string GetTicketTypeName(TicketTypes type);
    }
}