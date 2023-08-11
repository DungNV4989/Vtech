using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Text;

namespace VTECHERP.Helper.Templates
{
    public static class TemplatePdfBillCustomer
    {
        public const string Html = $@"
 <div style=""display: flex;align-items: baseline; justify-content: center;"">
        <div style=""text-align: center;"">
            <h2>HÓA ĐƠN BÁN HÀNG</h2>
            <span>Số phiếu: </span><strong>HD10019023</strong><br/>
            <span>13/02/2023</span><br/>
            <strong>Hotline: 0817093333</strong>
        </div>
        <div style=""margin-top: 10px;"">
            <table>
                <tr>
                    <td>Tên khách hàng:</td>
                    <td>Toại Cao-575/60 CMT8 P15 Q10 (SG)</td>
                    <td>Điện thoại: </td>
                    <td><strong>0886444486</strong></td>
                </tr>
                <tr>
                    <td>Địa chỉ</td>
                    <td></td>
                    <td>Loại tiền</td>
                    <td>VNĐ</td>
                </tr>
                <tr>
                    <td>Cửa hàng</td>
                    <td>Linh kiện sài gòn</td>
                    <td>Số HĐ</td>
                    <td>1007567</td>
                </tr>
            </table>
        </div>
    </div>
"; 
    }
}
