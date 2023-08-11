using System;
using System.Collections.Generic;
using System.Text;

namespace VTECHERP.Enums.DraftTicket
{
    public enum Status
    {
        /// <summary>
        /// Mới
        /// </summary>
        New,
        /// <summary>
        /// Đã duyệt
        /// </summary>
        Approved,
        /// <summary>
        /// Đã xác nhận
        /// </summary>
        Confirmed,
        /// <summary>
        /// Đã hủy
        /// </summary>
        Cancel,
    }
}
