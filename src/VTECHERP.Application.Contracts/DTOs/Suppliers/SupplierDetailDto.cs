using System;
using VTECHERP.Enums;

namespace VTECHERP.DTOs.Suppliers
{
    public class SupplierDetailDto
    {
        public Guid Id { get; set; }

        /// <summary>
        /// Mã nhà cung cấp
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Tên nhà cung cấp
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Địa chỉ
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// Nguồn gốc NCC
        /// </summary>
        public SupplierOrigin Origin { get; set; }

        /// <summary>
        /// Email
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Điện thoại
        /// </summary>
        public string PhoneNumber { get; set; }

        /// <summary>
        /// Ngân hàng
        /// </summary>
        public string BankName { get; set; }

        /// <summary>
        /// Chi nhánh
        /// </summary>
        public string BankBrand { get; set; }

        /// <summary>
        /// Số tài khoản
        /// </summary>
        public string BankNumberAccount { get; set; }

        /// <summary>
        /// Chủ tài khoản
        /// </summary>
        public string BankAccountHolder { get; set; }

        /// <summary>
        /// Trạng thái
        /// </summary>
        public SupplierStatus Status { get; set; }
    }
}