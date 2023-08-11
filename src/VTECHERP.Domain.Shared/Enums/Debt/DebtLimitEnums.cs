using System;
using System.Collections.Generic;
using System.Text;

namespace VTECHERP.Enums.Debt
{
    public enum DebtLimitEnums
    {
        /// <summary>
        /// +	Nợ [Phải thu] > Giới hạn
        /// </summary>
        DebtGtLimit,
        /// <summary>
        /// +	Nợ [Phải thu] = Giới hạn
        /// </summary>
        DebtEqLimit,
        /// <summary>
        /// Nợ [Phải thu] < Giới hạn
        /// </summary>
        DebtLtLimit,

    }
    public enum DebtHasCodeEnums
    {
        NotCod = 1, 
        Cod = 2
    }
   
}
