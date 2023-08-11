using System;
using System.Collections.Generic;
using System.Text;

namespace VTECHERP.Enums.Bills
{
    public enum TransportForm
    {
        /// <summary>
        /// nhân viên giao
        /// </summary>
        staff =0,
        /// <summary>
        /// Không vận chuyển
        /// </summary>
        None = 1,
        /// <summary>
        /// Nộ bộ
        /// </summary>
        Internal = 2,
        /// <summary>
        /// Qua hãng
        /// </summary>
        Production = 3
    }
}
