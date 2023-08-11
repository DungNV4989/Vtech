using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using VTECHERP.DTOs.IntegratedCasso;
using VTECHERP.Entities;
using VTECHERP.Helper;
using VTECHERP.ServiceInterfaces;

namespace VTECHERP.Services
{
    public class IntegratedCassoService : IIntegratedCassoService
    {
        private readonly IRepository<IntegratedCassoLog> _cassoLogRepository;
        public IntegratedCassoService(IRepository<IntegratedCassoLog> cassoLogRepository) 
        { 
            _cassoLogRepository = cassoLogRepository;
        }
        public async Task<IActionResult> HandlerWebhookCasso(CassoWebhookDTO request)
        {
            try
            {
                await _cassoLogRepository.InsertAsync(new IntegratedCassoLog
                {
                    Data = JsonConvert.SerializeObject(request),
                    Status = 1,
                    Log = "Success"
                });
                return new GenericActionResult(200, true, "Thành công", null);
            }catch (Exception ex)
            {
                await _cassoLogRepository.InsertAsync(new IntegratedCassoLog
                {
                    Data = JsonConvert.SerializeObject(request),
                    Status = 0,
                    Log = "Failed"
                });
                return new GenericActionResult(500, false, "Thất bại", null);
            }
        }
    }
}
