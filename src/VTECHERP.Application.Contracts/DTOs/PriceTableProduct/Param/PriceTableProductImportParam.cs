using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace VTECHERP.DTOs.PriceTableProduct.Param
{
    public class PriceTableProductImportParam
    {
        public Guid PriceTableId { get; set; }
        public IFormFile File { get; set; }
    }
}
