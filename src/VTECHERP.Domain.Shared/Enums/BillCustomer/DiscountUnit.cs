using System;
using System.Collections.Generic;
using System.Text;

namespace VTECHERP.Enums.Bills
{
    public enum DiscountUnit
    {
        Precent = 1,
        Cash = 2
    }

    public enum ApplyFor
    {
        ProductCategory = 1,
        Product = 2
    }
    public enum PromotionStatus
    {
        PreActive = 0,
        Active = 1,
        Inactive = 2,
    }

    public enum VoucherStatus
    {
        New = 0,
        Used = 1,
        Cancel = 2
    }
}
