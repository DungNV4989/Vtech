using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Uow;
using VTECHERP.Entities;
using VTECHERP.Helper;
using VTECHERP.ServiceInterfaces;

namespace VTECHERP.Applications.BillCustomers
{
    public class DeleteData : ApplicationService
    {
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly IBillCustomerService _billCustomerService;
        private readonly IRepository<Entities.CustomerReturn> _customerReturnRepository;
        private readonly IRepository<CustomerReturnProduct> _customerReturnProductRepository;
        private readonly IRepository<Entry> _entryRepository;
        private readonly IRepository<Debt> _debtRepository;
        public DeleteData(
            IUnitOfWorkManager unitOfWorkManager,
            IBillCustomerService billCustomerService,
            IRepository<Entities.CustomerReturn> customerReturnRepository,
            IRepository<CustomerReturnProduct> customerReturnProductRepository,
            IRepository<Entry> entryRepository,
            IRepository<Debt> debtRepository
            )
        {
            _unitOfWorkManager = unitOfWorkManager;
            _billCustomerService = billCustomerService;
            _customerReturnRepository = customerReturnRepository;
            _entryRepository = entryRepository;
            _debtRepository = debtRepository;
            _customerReturnProductRepository = customerReturnProductRepository;
        }

        [HttpPost]
        public async Task<IActionResult> DeleteManyBillCustomer(List<Guid> param)
        {
            using (var uow = _unitOfWorkManager.Begin(requiresNew: true, isTransactional: true))
            {
                try
                {
                    await _billCustomerService.DeleteBillCustomers(param);
                    await uow.CompleteAsync();
                    return new OkResult();
                }
                catch (Exception ex)
                {
                    return new GenericActionResult(500, false, ex.Message);
                }
            }
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteCustomerReturn()
        {
            using (var uow = _unitOfWorkManager.Begin(requiresNew: true, isTransactional: true))
            {
                try
                {
                    var customerReturns = await _customerReturnRepository.GetListAsync();
                    await _customerReturnRepository.DeleteManyAsync(customerReturns);
                    var customerReturnIds = customerReturns.Select(x => x.Id).ToList();

                    var customerReturnProducts = await _customerReturnProductRepository.GetListAsync(x => customerReturnIds.Contains(x.CustomerReturnId ?? Guid.Empty));
                    await _customerReturnProductRepository.DeleteManyAsync(customerReturnProducts);

                    var entries = await _entryRepository.GetListAsync(x => customerReturnIds.Contains(x.SourceId ?? Guid.Empty));
                    if (entries.Any())
                    {
                        var lstEntriesId = entries.Select(x => x.Id).ToList();
                        var debtEntriesBill = await _debtRepository.GetListAsync(x => lstEntriesId.Contains(x.EntryId ?? Guid.Empty));

                        await _entryRepository.DeleteManyAsync(entries);
                        await _debtRepository.DeleteManyAsync(debtEntriesBill);
                    }

                    await uow.CompleteAsync();
                    return new OkResult();
                }
                catch (Exception ex)
                {
                    return new GenericActionResult(500, false, ex.Message);
                }
            }
        }
    }
}
