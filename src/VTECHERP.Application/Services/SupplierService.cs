using ClosedXML.Excel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vinpearl.Modelling.Library.Utility.Excel;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.MultiTenancy;
using Volo.Abp.ObjectMapping;
using Volo.Abp.Validation;
using VTECHERP.Constants;
using VTECHERP.Domain.Shared.Helper.Excel.Model;
using VTECHERP.DTOs.Base;
using VTECHERP.DTOs.PriceTableProduct.Param;
using VTECHERP.DTOs.PriceTableProduct.Respon;
using VTECHERP.DTOs.Suppliers;
using VTECHERP.DTOs.Suppliers.Params;
using VTECHERP.DTOs.Suppliers.Respons;
using VTECHERP.Entities;
using VTECHERP.Enums;
using VTECHERP.Enums.Bills;

namespace VTECHERP.Services
{
    public class SupplierService : ISupplierService
    {
        private readonly IRepository<Suppliers> _supplierRepository;
        private readonly IObjectMapper _objectMapper;
        private readonly object balanceLock = new object();

        public SupplierService(IRepository<Suppliers> supplierRepository
            , IObjectMapper objectMapper
            )
        {
            _supplierRepository = supplierRepository;
            _objectMapper = objectMapper;
        }

        public async Task<List<MasterDataDTO>> GetIdCodeNameAsync()
        {
            var suppliers = await _supplierRepository.GetListAsync();
            var result = _objectMapper.Map<List<Suppliers>, List<MasterDataDTO>>(suppliers);
            return result;
        }

        public async Task<SupplierDetailDto> GetByIdAsync(Guid id)
        {
            var supplier = await _supplierRepository.FirstOrDefaultAsync(x => x.Id == id);
            if (supplier == null)
                return new SupplierDetailDto();
            var result = _objectMapper.Map<Suppliers, SupplierDetailDto>(supplier);
            return result;
        }

        public async Task<List<MasterDataDTO>> GetChineseSupplierAsync()
        {
            var suppliers = (await _supplierRepository.GetQueryableAsync()).Where(x=>x.Origin == Enums.SupplierOrigin.CN).ToList();
            var result = _objectMapper.Map<List<Suppliers>, List<MasterDataDTO>>(suppliers);
            return result;
        }

        public async Task<bool> Exist(Guid id)
        {
            var supplier = await _supplierRepository.FindAsync(x => x.Id == id);
            if (supplier == null)
                return false;
            return true;
        }

        public async Task<(bool success, string message, Guid? data)> Create(CreateSupplierParam param)
        {
            // Validator
            var validatorNameSupplier = ValidatorNameSuplier(param.Name);
            if (!validatorNameSupplier.success)
                return (false, validatorNameSupplier.message, null);

            var validatorPhoneSupplier = ValidatorPhoneNumberSuplier(param.PhoneNumber);
            if (!validatorPhoneSupplier.success)
                return (false, validatorPhoneSupplier.message, null);

            var validatorCodeSupplier = ValidatorCodeSuplier(param.Code);
            if (!validatorCodeSupplier.success)
                return (false, validatorCodeSupplier.message, null);

            if (!Enum.IsDefined(typeof(SupplierOrigin), param.Type))
                return (false, "Loại nhà cung cấp không hợp lệ", null);

            var newSupplier = new Suppliers
            {
                Code= param.Code,
                Name = param.Name,  
                PhoneNumber = param.PhoneNumber,    
                Origin = param.Type
            };

            newSupplier.Squence = GetSquenceSupplier();

            await _supplierRepository.InsertAsync(newSupplier);
            return (true, "Tạo mới thành công", newSupplier.Id);
        }

        public string GetSquenceSupplier()
        {
            lock (balanceLock)
            {
                int codeLast = 1;
                var lastSupplier = (_supplierRepository.GetQueryableAsync().Result)
                    .OrderByDescending(x => x.CreationTime)
                    .FirstOrDefault();

                if (lastSupplier == null)
                    return "NCC-0000000001";

                var splitCodeLast = lastSupplier.Squence?.Split("-").Last();
                if (splitCodeLast == null || !int.TryParse(splitCodeLast, out codeLast))
                    return "NCC-0000000001";

                codeLast++;
                var codeLastString = codeLast.ToString();

                return $"NCC-{codeLastString.PadLeft(10, '0')}";
            }
        }

        private (bool success, string message) ValidatorNameSuplier(string SupplierName, bool IsUpdate = false, Guid? Id = null)
        {
            if (string.IsNullOrEmpty(SupplierName))
                return (false, "Thiếu tên nhà cung cấp");

            if (!IsUpdate && (_supplierRepository.AnyAsync(x => x.Name == SupplierName.Trim()).Result))
                return (false, "Trùng tên nhà cung cấp");

            if (IsUpdate && (_supplierRepository.AnyAsync(x => x.Name == SupplierName.Trim() && x.Id != Id).Result))
                return (false, "Trùng tên nhà cung cấp");

            return (true, "");
        }

        private (bool success, string message) ValidatorPhoneNumberSuplier(string SupplierPhoneNumber, bool IsUpdate = false, Guid? Id = null)
        {
            if (string.IsNullOrEmpty(SupplierPhoneNumber))
                return (false, "Thiếu số điện thoại nhà cung cấp");

            if (!IsUpdate && (_supplierRepository.AnyAsync(x => x.PhoneNumber == SupplierPhoneNumber.Trim()).Result))
                return (false, "Trùng số điện thoại nhà cung cấp");

            if (IsUpdate &&  (_supplierRepository.AnyAsync(x => x.PhoneNumber == SupplierPhoneNumber.Trim() && x.Id != Id).Result))
                return (false, "Trùng số điện thoại nhà cung cấp");

            return (true, "");
        }

        private (bool success, string message) ValidatorCodeSuplier(string SupplierCode, bool IsUpdate = false, Guid? Id = null)
        {
            if (string.IsNullOrEmpty(SupplierCode))
                return (true, "");

            if (!IsUpdate && (_supplierRepository.AnyAsync(x => x.Code == SupplierCode.Trim()).Result))
                return (false, "Trùng mã nhà cung cấp");

            if (IsUpdate && (_supplierRepository.AnyAsync(x => x.Code == SupplierCode.Trim() && x.Id != Id).Result))
                return (false, "Trùng mã nhà cung cấp");

            return (true, "");
        }

        public async Task<(bool success, string message)> Update(Guid Id, CreateSupplierParam param)
        {
            var supplier = await _supplierRepository.FirstOrDefaultAsync(x => x.Id == Id);
            if (supplier == null)
                return (false, $"Không tìm thấy nhà cung cấp có id là {Id}");

            // Validator
            var validatorNameSupplier = ValidatorNameSuplier(param.Name, true, supplier.Id);
            if (!validatorNameSupplier.success)
                return (false, validatorNameSupplier.message);

            var validatorPhoneSupplier = ValidatorPhoneNumberSuplier(param.PhoneNumber, true, supplier.Id);
            if (!validatorPhoneSupplier.success)
                return (false, validatorPhoneSupplier.message);

            var validatorCodeSupplier = ValidatorCodeSuplier(param.Code, true, supplier.Id);
            if (!validatorCodeSupplier.success)
                return (false, validatorCodeSupplier.message);

            if (!Enum.IsDefined(typeof(SupplierOrigin), param.Type))
                return (false, "Loại nhà cung cấp không hợp lệ");

            supplier.Code = param.Code;
            supplier.Name = param.Name;
            supplier.PhoneNumber = param.PhoneNumber;
            supplier.Origin = param.Type;

            return (true, "Cập nhật thành công");
        }

        public string GetSupplierTypeText(SupplierOrigin param)
        {
            return param switch
            {
                SupplierOrigin.CN => "Trung Quốc",
                SupplierOrigin.VN => "Việt Nam",
                _ => ""
            };
        }

        public async Task<byte[]> DownloadTemplateImport()
        {
            var workbook = new CustomWorkBook();
            var sheet = RenderTemplateImport();
            workbook.Sheets.Add(sheet);

            return ExcelHelper.ExportExcel(workbook);
        }

        private CustomSheet RenderTemplateImport()
        {
            var sheet = new CustomSheet("Sheet 1");

            var startRow = 1;

            var header = new CustomDataTable()
            {
                StartRowIndex = startRow,
                StartColumnIndex = 1,
                RowDirection = Directions.Horizontal,
                Rows = new List<DataRow>
                    {
                        new DataRow(
                            new HeaderCell("Mã nhà cung cấp"),
                            new HeaderCell("Tên nhà cung cấp"),
                            new HeaderCell("Số điện thoại"),
                            new HeaderCell("Loại cung cấp")
                            )
                    }
            };

            sheet.Tables.Add(header);

            return sheet;
        }

        public async Task<byte[]> ImportPriceTableProductData(IFormFile file)
        {
            var workbook = new CustomWorkBook();
            var listData = await MappingData(file);

            // nếu tất cả row đều fail thì return error
            if (listData.All(p => !p.Success))
            {
                workbook.Sheets.Add(RenderTemplateAfterImport(listData));
                return ExcelHelper.ExportExcel(workbook);
            }

            var dataToInsert = new List<Suppliers>();
            var listDatas = listData.Where(x => x.Success).ToList() ?? new List<SupplierImportRespon> ();

            if (listDatas.Any())
            {
                foreach (var item in listDatas)
                {
                    var supplier = new Suppliers()
                    {
                        PhoneNumber = item.PhoneNumber,
                        Name = item.Name,
                        Code = item.Code,
                        Origin = item.Type == "Việt Nam" ? SupplierOrigin.VN : SupplierOrigin.CN,
                    };
                    dataToInsert.Add(supplier);
                };
            }

            if (dataToInsert.Any())
            {
                await _supplierRepository.InsertManyAsync(dataToInsert);
            }

            var sheet = RenderTemplateAfterImport(listData);
            workbook.Sheets.Add(sheet);

            return ExcelHelper.ExportExcel(workbook);
        }

        private async Task<List<SupplierImportRespon>> MappingData(IFormFile file)
        {
            var result = new List<SupplierImportRespon>();

            using (var workbook = new XLWorkbook(file.OpenReadStream()))
            {
                var worksheet = workbook.Worksheet(1); // Chỉ định số trang tính trong tệp Excel (index bắt đầu từ 1)

                var firstRowUsed = worksheet.FirstRowUsed();
                var headers = firstRowUsed.CellsUsed()
                    .Select(c => c.Value.ToString().Trim())
                    .ToList();

                var suppliers = await _supplierRepository.GetListAsync();

                foreach (var row in worksheet.RowsUsed().Skip(1)) // Bỏ qua hàng tiêu đề (hàng đầu tiên)
                {
                    var obj = new SupplierImportRespon();

                    for (int i = 0; i < headers.Count; i++)
                    {
                        var cellValue = row.Cell(i + 1).Value?.ToString();

                        // Gán giá trị cho thuộc tính tương ứng trong đối tượng
                        if (headers[i] == "Mã nhà cung cấp")
                        {
                            obj.Code = cellValue ?? "";
                            if (!string.IsNullOrEmpty(obj.Code) 
                                && (suppliers.Any(x => x.Code == obj.Code) 
                                || result.Any(x => x.Success && x.Code == obj.Code)))
                            {
                                obj.Success = false;
                                obj.Message += "Trùng mã nhà cung cấp,";
                            }
                        }
                        else if (headers[i] == "Tên nhà cung cấp")
                        {
                            obj.Name = cellValue ?? "";
                            if (string.IsNullOrEmpty(obj.Name))
                            {
                                obj.Success = false;
                                obj.Message += "Thiếu tên nhà cung cấp,";
                            }
                            if (suppliers.Any(x => x.Name == obj.Name) 
                                || result.Any(x => x.Success && x.Name == obj.Name))
                            {
                                obj.Success = false;
                                obj.Message += "Trùng tên nhà cung cấp,";
                            }
                        }
                        else if (headers[i] == "Số điện thoại")
                        {
                            obj.PhoneNumber = cellValue ?? "";
                            if (string.IsNullOrEmpty(obj.PhoneNumber))
                            {
                                obj.Success = false;
                                obj.Message += "Thiếu số điện thoại nhà cung cấp,";
                            }
                            if (suppliers.Any(x => x.PhoneNumber == obj.PhoneNumber) 
                                || result.Any(x => x.Success && x.PhoneNumber == obj.PhoneNumber))
                            {
                                obj.Success = false;
                                obj.Message += "Trùng số điện thoại nhà cung cấp,";
                            }
                        }
                        else if (headers[i] == "Loại cung cấp")
                        {
                            obj.Type = cellValue;
                            if (string.IsNullOrEmpty(obj.Type) || (obj.Type.Trim() != "Việt Nam" && obj.Type.Trim() != "Trung Quốc"))
                            {
                                obj.Success = false;
                                obj.Message += "Loại nhà cung cấp không hợp lệ";
                            }
                        }
                    }

                    if (obj.Success)
                    {
                        obj.Message = "Nhà cung cấp được nhập thành công";
                    }

                    result.Add(obj);
                }

                return result;
            }
        }
        private CustomSheet RenderTemplateAfterImport(List<SupplierImportRespon> list)
        {
            var sheet = new CustomSheet("Sheet 1");

            var startRow = 1;

            var header = new CustomDataTable()
            {
                StartRowIndex = startRow,
                StartColumnIndex = 1,
                RowDirection = Directions.Horizontal,
                Rows = new List<DataRow>
                    {
                        new DataRow(
                            new HeaderCell("Mã nhà cung cấp"),
                            new HeaderCell("Tên nhà cung cấp"),
                            new HeaderCell("Số điện thoại"),
                            new HeaderCell("Loại nhà cung cấp"),
                            new HeaderCell("Trạng thái"),
                            new HeaderCell("Diễn giải")
                            )
                    }
            };
            sheet.Tables.Add(header);

            var indexSaleOrderColumn = startRow + 1;
            foreach (var item in list)
            {
                var row = new CustomDataTable()
                {
                    StartRowIndex = indexSaleOrderColumn++,
                    StartColumnIndex = 1,
                    RowDirection = Directions.Horizontal,
                    Rows = new List<DataRow>
                        {
                        new DataRow(
                            new Cell(item.Code),
                            new Cell(item.Name),
                            new Cell(item.PhoneNumber),
                            new Cell(item.Type),
                            new Cell(item.Success ? "Thành công" : "Thất bại"),
                            new Cell(item.Message)
                            )
                    }
                };

                sheet.Tables.Add(row);
            }

            return sheet;
        }
    }
}