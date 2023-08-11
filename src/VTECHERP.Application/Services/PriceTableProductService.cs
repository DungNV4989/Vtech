using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vinpearl.Modelling.Library.Utility.Excel;
using Volo.Abp.Domain.Repositories;
using VTECHERP.Domain.Shared.Helper.Excel.Model;
using VTECHERP.DTOs.PriceTableProduct.Param;
using VTECHERP.DTOs.PriceTableProduct.Respon;
using VTECHERP.Entities;
using VTECHERP.ServiceInterfaces;

namespace VTECHERP.Services
{
    public class PriceTableProductService : IPriceTableProductService
    {
        private readonly IRepository<PriceTableProduct> _priceTableProductRepository;
        private readonly IRepository<Products> _productRepository;
        public PriceTableProductService(
            IRepository<Products> productRepository
            , IRepository<PriceTableProduct> priceTableProductRepository
            )
        {
            _productRepository = productRepository;
            _priceTableProductRepository = priceTableProductRepository;
        }

        public async Task<byte[]> DownloadTemplateImport()
        {
            var workbook = new CustomWorkBook();
            var sheet = RenderTemplateImport();
            workbook.Sheets.Add(sheet);

            return ExcelHelper.ExportExcel(workbook);
        }

        public async Task<byte[]> ImportPriceTableProductData(PriceTableProductImportParam param)
        {
            var workbook = new CustomWorkBook();
            var listData = await MappingData(param);

            // nếu tất cả row đều fail thì return error
            if(listData.All(p => !p.Status))
            {
                workbook.Sheets.Add(RenderTemplateAfterImport(listData));
                return ExcelHelper.ExportExcel(workbook);
            }
            var mappingDataProductIds = listData.Select(p => p.ProductId).ToList();
            var priceTableProducts = await _priceTableProductRepository.GetListAsync(p => p.PriceTableId == param.PriceTableId);
            var priceTableProductIds = priceTableProducts.Select(p => p.ProductId).ToList();
            var dataToInsert = listData.Where(x => x.Status && !priceTableProductIds.Contains(x.ProductId))
                .Select(x => new PriceTableProduct
                {
                    Price = x.Price,
                    PriceTableId = x.TablePriceId,
                    ProductId = x.ProductId
                })
                .ToList();

            var dataToUpdate = priceTableProducts.Where(x => mappingDataProductIds.Contains(x.ProductId)).ToList();
            dataToUpdate.ForEach(data =>
            {
                var newData = listData.FirstOrDefault(d => d.ProductId == data.ProductId);
                data.Price = newData.Price;
            });

            if (dataToInsert.Any())
            {
                await _priceTableProductRepository.InsertManyAsync(dataToInsert);
            }
            if (dataToUpdate.Any())
            {
                await _priceTableProductRepository.UpdateManyAsync(dataToUpdate);
            }

            var sheet = RenderTemplateAfterImport(listData);
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
                            //new HeaderCell("Mã sản phẩm"),
                            new HeaderCell("Tên sản phẩm"),
                            new HeaderCell("Giá mới")
                            )
                    }
            };

            sheet.Tables.Add(header);

            return sheet;
        }

        private async Task<List<PriceTableProductImportRespon>> MappingData(PriceTableProductImportParam param)
        {
            var result = new List<PriceTableProductImportRespon>();

            using (var workbook = new XLWorkbook(param.File.OpenReadStream()))
            {
                var worksheet = workbook.Worksheet(1); // Chỉ định số trang tính trong tệp Excel (index bắt đầu từ 1)

                var firstRowUsed = worksheet.FirstRowUsed();
                var headers = firstRowUsed.CellsUsed()
                    .Select(c => c.Value.ToString().Trim())
                    .ToList();
                var products = await _productRepository.GetListAsync();
                //var priceTableProducts = await _priceTableProductRepository.GetListAsync(p => p.PriceTableId == param.PriceTableId);

                foreach (var row in worksheet.RowsUsed().Skip(1)) // Bỏ qua hàng tiêu đề (hàng đầu tiên)
                {
                    var obj = new PriceTableProductImportRespon();
                    obj.TablePriceId = param.PriceTableId;
                    Products productOrigin = null;

                    for (int i = 0; i < headers.Count; i++)
                    {
                        var cellValue = row.Cell(i + 1).Value?.ToString();

                        // Gán giá trị cho thuộc tính tương ứng trong đối tượng
                        if (headers[i] == "Tên sản phẩm")
                        {
                            obj.ProductName = cellValue ?? "";
                            if (obj.ProductName.IsNullOrWhiteSpace())
                            {
                                obj.Status = false;
                                obj.Message = "Tên sản phẩm không thể để trống";
                                continue;
                            }
                            productOrigin = products.FirstOrDefault(x => x.SequenceId == obj.ProductName.Trim()
                                || (x.Name != null && x.Name.Trim().ToLower() == obj.ProductName.Trim().ToLower())
                                || (x.BarCode != null && x.BarCode.Trim().ToLower() == obj.ProductName.Trim().ToLower())
                                || (x.BarCode != null && x.Code.Trim().ToLower() == obj.ProductName.Trim().ToLower())
                            );

                            if (productOrigin == null)
                            {
                                obj.Status = false;
                                obj.Message = "Không tìm thấy sản phẩm trong danh sách sản phẩm";
                            }
                            else 
                            {
                                //var priceTableProduct = priceTableProducts.FirstOrDefault(p => p.ProductId == productOrigin.Id);
                                obj.ProductId = productOrigin.Id;
                                if (result.Any(x => x.ProductId == obj.ProductId))
                                {
                                    obj.Status = false;
                                    obj.Message = "Trùng sản phẩm trong file";
                                }
                                //if (priceTableProduct != null)
                                //{
                                //    obj.Status = false;
                                //    obj.Message = "Trùng sản phẩm trong bảng giá";
                                //}
                                //else
                                //{
                                //    if (result.Any(x => x.ProductId == obj.ProductId))
                                //    {
                                //        obj.Status = false;
                                //        obj.Message = "Trùng sản phẩm trong file";
                                //    }
                                //}
                            }
                        }
                        else if (headers[i] == "Giá mới")
                        {
                            obj.Price = string.IsNullOrEmpty(cellValue) ? 0 : (decimal.TryParse(cellValue, out decimal priceConvert) ? priceConvert : 0);
                            if (obj.Price < 0)
                            {
                                obj.Status = false;
                                obj.Message += "\nGiá sản phẩm không hợp lệ";
                            }
                        }
                    }

                    result.Add(obj);
                }

                return result;
            }
        }
        private CustomSheet RenderTemplateAfterImport(List<PriceTableProductImportRespon> list)
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
                            //new HeaderCell("Mã sản phẩm"),
                            new HeaderCell("Tên sản phẩm"),
                            new HeaderCell("Giá mới"),
                            new HeaderCell("Trạng thái"),
                            new HeaderCell("Ghi chú")
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
                            //new Cell(item.ProductCode) { Format = "@"},
                            new Cell(item.ProductName),
                            new Cell(item.Price),
                            new Cell(item.Status ? "Thành công" : "Thất bại"),
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
