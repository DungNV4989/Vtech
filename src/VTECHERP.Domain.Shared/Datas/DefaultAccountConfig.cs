using System;
using System.Collections.Generic;
using System.Text;

namespace VTECHERP.Datas
{
    /// <summary>
    /// Chứa danh sách tài khoản kế toán mặc định để import vào db khi thiết lập
    /// </summary>
    public class DefaultAccountConfig
    {
        private DefaultAccountConfig() { }
        private static DefaultAccountConfig instance = null;
        public static DefaultAccountConfig Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new DefaultAccountConfig();
                }
                return instance;
            }
        }

        public List<DefaultAccountDto> LoadListDefaultAccount()
        {
            var results = new List<DefaultAccountDto>()
            {
                new DefaultAccountDto("111", "Tiền mặt"),
                new DefaultAccountDto("1111", "Tiền Việt Nam", "111"),
                new DefaultAccountDto("1112", "Ngoại tệ", "111"),
                new DefaultAccountDto("1113", "Vàng tiền tệ", "111"),

                new DefaultAccountDto("112", "Tiền gửi ngân hàng"),
                new DefaultAccountDto("1121", "Tiền Việt Nam", "112"),
                new DefaultAccountDto("1122", "Ngoại tệ", "112"),
                new DefaultAccountDto("1123", "Vàng tiền tệ", "112"),

                new DefaultAccountDto("113", "Tiền đang chuyển"),
                new DefaultAccountDto("1131", "Tiền Việt Nam", "113"),
                new DefaultAccountDto("1132", "Ngoại tệ", "113"),

                new DefaultAccountDto("121", "Chứng khoán kinh doanh"),
                new DefaultAccountDto("1211", "Cổ phiếu", "121"),
                new DefaultAccountDto("1212", "Trái phiếu", "121"),
                new DefaultAccountDto("1213", "Chứng khoán và công cụ tài chính khác", "121"),

                new DefaultAccountDto("128", "Đầu tư nắm giữ đến ngày đáo hạn"),
                new DefaultAccountDto("1281", "Tiền gửi có kỳ hạn", "128"),
                new DefaultAccountDto("1282", "Trái phiếu", "128"),
                new DefaultAccountDto("1283", "Cho vay", "128"),
                new DefaultAccountDto("1288", "Các khoản đầu tư khác nắm giữ đến ngày đáo hạn", "128"),

                new DefaultAccountDto("131", "Phải thu của khách hàng"),

                new DefaultAccountDto("133", "Thuế GTGT được khấu trừ"),
                new DefaultAccountDto("1331", "Thuế GTGT được khấu trừ của hàng hóa, dịch vụ", "133"),
                new DefaultAccountDto("1332", "Thuế GTGT được khấu trừ của TSCĐ", "133"),

                new DefaultAccountDto("136", "Phải thu nội bộ"),
                new DefaultAccountDto("1361", "Vốn kinh doanh ở các đơn vị trực thuộc", "136"),
                new DefaultAccountDto("1362", "Phải thu nội bộ về chênh lệch tỷ giá", "136"),
                new DefaultAccountDto("1363", "Phải thu nội bộ về chi phí đi vay đủ điều kiện được vốn hoá", "136"),
                new DefaultAccountDto("1368", "Phải thu nội bộ khác", "136"),

                new DefaultAccountDto("138", "Phải thu khác"),
                new DefaultAccountDto("1381", "Tài sản thiếu chờ xử lý", "138"),
                new DefaultAccountDto("1385", "Phải thu về cổ phần hoá", "138"),
                new DefaultAccountDto("1388", "Phải thu khác", "138"),

                new DefaultAccountDto("141", "Tạm ứng"),
                new DefaultAccountDto("151", "Hàng mua đang đi đường"),
                new DefaultAccountDto("152", "Nguyên liệu, vật liệu"),

                new DefaultAccountDto("153", "Công cụ, dụng cụ"),
                new DefaultAccountDto("1531", "Công cụ, dụng cụ", "153"),
                new DefaultAccountDto("1532", "Bao bì luân chuyển", "153"),
                new DefaultAccountDto("1533", "Đồ dùng cho thuê", "153"),
                new DefaultAccountDto("1534", "Thiết bị, phụ tùng thay thế", "153"),

                new DefaultAccountDto("154", "Chi phí sản xuất, kinh doanh dở dang"),

                new DefaultAccountDto("155", "Phải thu khác"),
                new DefaultAccountDto("1551", "Thành phẩm nhập kho", "155"),
                new DefaultAccountDto("1557", "Thành phẩm bất động sản", "155"),

                new DefaultAccountDto("156", "Phải thu khác"),
                new DefaultAccountDto("1561", "Giá mua hàng hóa", "156"),
                new DefaultAccountDto("1562", "Chi phí thu mua hàng hóa", "156"),
                new DefaultAccountDto("1567", "Hàng hóa bất động sản", "156"),

                new DefaultAccountDto("157", "Hàng gửi đi bán"),
                new DefaultAccountDto("158", "Hàng hoá kho bảo thuế"),

                new DefaultAccountDto("161", "Chi sự nghiệp"),
                new DefaultAccountDto("1611", "Chi sự nghiệp năm trước", "161"),
                new DefaultAccountDto("1612", "Chi sự nghiệp năm nay", "161"),

                new DefaultAccountDto("171", "Giao dịch mua bán lại trái phiếu chính phủ"),

                new DefaultAccountDto("211", "Tài sản cố định hữu hình"),
                new DefaultAccountDto("2111", "Nhà cửa, vật kiến trúc", "211"),
                new DefaultAccountDto("2112", "Máy móc, thiết bị", "211"),
                new DefaultAccountDto("2113", "Phương tiện vận tải, truyền dẫn", "211"),
                new DefaultAccountDto("2114", "Thiết bị, dụng cụ quản lý", "211"),
                new DefaultAccountDto("2115", "Cây lâu năm, súc vật làm việc và cho sản phẩm", "211"),
                new DefaultAccountDto("2118", "TSCĐ khác", "211"),

                new DefaultAccountDto("212", "Tài sản cố định thuê tài chính"),
                new DefaultAccountDto("2121", "TSCĐ hữu hình thuê tài chính.", "212"),
                new DefaultAccountDto("2122", "TSCĐ vô hình thuê tài chính.", "212"),

                new DefaultAccountDto("213", "Tài sản cố định vô hình"),
                new DefaultAccountDto("2131", "Quyền sử dụng đất", "213"),
                new DefaultAccountDto("2132", "Quyền phát hành", "213"),
                new DefaultAccountDto("2133", "Bản quyền, bằng sáng chế", "213"),
                new DefaultAccountDto("2134", "Nhãn hiệu, tên thương mại", "213"),
                new DefaultAccountDto("2135", "Chương trình phần mềm", "213"),
                new DefaultAccountDto("2136", "Giấy phép và giấy phép nhượng quyền", "213"),
                new DefaultAccountDto("2138", "TSCĐ vô hình khác", "213"),

                new DefaultAccountDto("214", "Hao mòn tài sản cố định"),
                new DefaultAccountDto("2141", "Hao mòn TSCĐ hữu hình", "214"),
                new DefaultAccountDto("2142", "Hao mòn TSCĐ thuê tài chính", "214"),
                new DefaultAccountDto("2143", "Hao mòn TSCĐ vô hình", "214"),
                new DefaultAccountDto("2147", "Hao mòn bất động sản đầu tư", "214"),

                new DefaultAccountDto("217", "Bất động sản đầu tư"),
                new DefaultAccountDto("221", "Đầu tư vào công ty con"),
                new DefaultAccountDto("222", "Đầu tư vào công ty liên doanh, liên kết"),

                new DefaultAccountDto("228", "Đầu tư khác"),
                new DefaultAccountDto("2281", "Đầu tư góp vốn vào đơn vị khác", "228"),
                new DefaultAccountDto("2288", "Đầu tư khác", "228"),

                new DefaultAccountDto("229", "Dự phòng tổn thất tài sản"),
                new DefaultAccountDto("2291", "Dự phòng giảm giá chứng khoán kinh doanh", "229"),
                new DefaultAccountDto("2292", "Dự phòng tổn thất đầu tư vào đơn vị khác", "229"),
                new DefaultAccountDto("2293", "Dự phòng phải thu khó đòi", "229"),
                new DefaultAccountDto("2294", "Dự phòng giảm giá hàng tồn kho", "229"),

                new DefaultAccountDto("241", "Xây dựng cơ bản dở dang"),
                new DefaultAccountDto("2411", "Mua sắm TSCĐ", "241"),
                new DefaultAccountDto("2412", "Xây dựng cơ bản", "241"),
                new DefaultAccountDto("2413", "Sửa chữa lớn TSCĐ", "241"),

                new DefaultAccountDto("242", "Chi phí trả trước"),
                new DefaultAccountDto("243", "Tài sản thuế thu nhập hoãn lại"),
                new DefaultAccountDto("244", "Cầm cố, thế chấp, ký quỹ, ký cược"),
                new DefaultAccountDto("331", "Phải trả cho người bán"),

                new DefaultAccountDto("333", "Thuế và các khoản phải nộp Nhà nước"),
                new DefaultAccountDto("3331", "Thuế giá trị gia tăng phải nộp", "333"),
                new DefaultAccountDto("3332", "Thuế tiêu thụ đặc biệt", "333"),
                new DefaultAccountDto("3333", "Thuế xuất, nhập khẩu", "333"),
                new DefaultAccountDto("3334", "Thuế thu nhập doanh nghiệp", "333"),
                new DefaultAccountDto("3335", "Thuế thu nhập cá nhân", "333"),
                new DefaultAccountDto("3336", "Thuế tài nguyên", "333"),
                new DefaultAccountDto("3337", "Thuế nhà đất, tiền thuê đất", "333"),
                new DefaultAccountDto("3338", "Thuế bảo vệ môi trường và các loại thuế khác", "333"),
                new DefaultAccountDto("3339", "Phí, lệ phí và các khoản phải nộp khác", "333"),
                new DefaultAccountDto("33311", "Thuế GTGT đầu ra", "333"),
                new DefaultAccountDto("33312", "Thuế GTGT hàng nhập khẩu", "333"),
                new DefaultAccountDto("33381", "Thuế bảo vệ môi trường", "333"),
                new DefaultAccountDto("33382", "Các loại thuế khác", "333"),

                new DefaultAccountDto("334", "Phải trả người lao động"),
                new DefaultAccountDto("3341", "Phải trả công nhân viên", "334"),
                new DefaultAccountDto("3348", "Phải trả người lao động khác", "334"),

                new DefaultAccountDto("335", "Chi phí phải trả"),

                new DefaultAccountDto("336", "Phải trả nội bộ"),
                new DefaultAccountDto("3361", "Phải trả nội bộ về vốn kinh doanh", "336"),
                new DefaultAccountDto("3362", "Phải trả nội bộ về chênh lệch tỷ giá", "336"),
                new DefaultAccountDto("3363", "Phải trả nội bộ về chi phí đi vay đủ điều kiện được vốn hoá", "336"),
                new DefaultAccountDto("3368", "Phải trả nội bộ khác", "336"),

                new DefaultAccountDto("337", "Thanh toán theo tiến độ kế hoạch hợp đồng xây dựng"),

                new DefaultAccountDto("338", "Phải trả, phải nộp khác"),
                new DefaultAccountDto("3381", "Tài sản thừa chờ giải quyết", "338"),
                new DefaultAccountDto("3382", "Kinh phí công đoàn", "338"),
                new DefaultAccountDto("3383", "Bảo hiểm xã hội", "338"),
                new DefaultAccountDto("3384", "Bảo hiểm y tế", "338"),
                new DefaultAccountDto("3385", "Phải trả về cổ phần hoá", "338"),
                new DefaultAccountDto("3386", "Bảo hiểm thất nghiệp", "338"),
                new DefaultAccountDto("3387", "Doanh thu chưa thực hiện", "338"),
                new DefaultAccountDto("3388", "Phải trả, phải nộp khác", "338"),

                new DefaultAccountDto("341", "Vay và nợ thuê tài chính"),
                new DefaultAccountDto("3411", "Các khoản đi vay", "341"),
                new DefaultAccountDto("3412", "Nợ thuê tài chính", "341"),

                new DefaultAccountDto("343", "Trái phiếu phát hành"),
                new DefaultAccountDto("3431", "Trái phiếu thường", "343"),
                new DefaultAccountDto("34311", "Mệnh giá", "343"),
                new DefaultAccountDto("34312", "Chiết khấu trái phiếu", "343"),
                new DefaultAccountDto("34313", "Phụ trội trái phiếu", "343"),
                new DefaultAccountDto("3432", "Trái phiếu chuyển đổi", "343"),

                new DefaultAccountDto("344", "Nhận ký quỹ, ký cược"),
                new DefaultAccountDto("347", "Thuế thu nhập hoãn lại phải trả"),

                new DefaultAccountDto("352", "Dự phòng phải trả"),
                new DefaultAccountDto("3521", "Dự phòng bảo hành sản phẩm hàng hóa", "352"),
                new DefaultAccountDto("3522", "Dự phòng bảo hành công trình xây dựng", "352"),
                new DefaultAccountDto("3523", "Dự phòng tái cơ cấu doanh nghiệp", "352"),
                new DefaultAccountDto("3524", "Dự phòng phải trả khác", "352"),

                new DefaultAccountDto("353", "Quỹ khen thưởng phúc lợi"),
                new DefaultAccountDto("3531", "Quỹ khen thưởng", "353"),
                new DefaultAccountDto("3532", "Quỹ phúc lợi", "353"),
                new DefaultAccountDto("3533", "Quỹ phúc lợi đã hình thành TSCĐ", "353"),
                new DefaultAccountDto("3534", "Quỹ thưởng ban quản lý điều hành công ty", "353"),

                new DefaultAccountDto("356", "Quỹ phát triển khoa học và công nghệ"),
                new DefaultAccountDto("3561", "Quỹ phát triển khoa học và công nghệ", "356"),
                new DefaultAccountDto("3562", "Quỹ phát triển khoa học và công nghệ đã hình thành TSCĐ", "356"),

                new DefaultAccountDto("357", "Quỹ bình ổn giá"),

                new DefaultAccountDto("411", "Vốn đầu tư của chủ sở hữu"),
                new DefaultAccountDto("4111", "Vốn góp của chủ sở hữu", "411"),
                new DefaultAccountDto("41111", "Cổ phiếu phổ thông có quyền biểu quyết", "4111"),
                new DefaultAccountDto("41112", "Cổ phiếu ưu đãi", "4111"),
                new DefaultAccountDto("4112", "Thặng dư vốn cổ phần", "411"),
                new DefaultAccountDto("4113", "Quyền chọn chuyển đổi trái phiếu", "411"),
                new DefaultAccountDto("4118", "Vốn khác", "411"),

                new DefaultAccountDto("412", "Chênh lệch đánh giá lại tài sản"),

                new DefaultAccountDto("413", "Chênh lệch tỷ giá hối đoái"),
                new DefaultAccountDto("4131", "Chênh lệch tỷ giá do đánh giá lại các khoản mục tiền tệ có gốc ngoại tệ", "413"),
                new DefaultAccountDto("4132", "Chênh lệch tỷ giá hối đoái trong giai đoạn trước hoạt động", "413"),

                new DefaultAccountDto("414", "Quỹ đầu tư phát triển"),
                new DefaultAccountDto("417", "Quỹ hỗ trợ sắp xếp doanh nghiệp"),
                new DefaultAccountDto("418", "Các quỹ khác thuộc vốn chủ sở hữu"),
                new DefaultAccountDto("419", "Cổ phiếu quỹ"),

                new DefaultAccountDto("421", "Lợi nhuận sau thuế chưa phân phối"),
                new DefaultAccountDto("4211", "Lợi nhuận sau thuế chưa phân phối năm trước", "421"),
                new DefaultAccountDto("4212", "Lợi nhuận sau thuế chưa phân phối năm nay", "421"),

                new DefaultAccountDto("441", "Nguồn vốn đầu tư xây dựng cơ bản"),

                new DefaultAccountDto("446", "Nguồn kinh phí sự nghiệp"),
                new DefaultAccountDto("4461", "Nguồn kinh phí sự nghiệp năm trước", "446"),
                new DefaultAccountDto("4462", "Nguồn kinh phí sự nghiệp năm nay", "446"),

                new DefaultAccountDto("466", "Nguồn kinh phí đã hình thành TSCĐ"),

                new DefaultAccountDto("511", "Doanh thu bán hàng và cung cấp dịch vụ"),
                new DefaultAccountDto("5111", "Doanh thu bán hàng hóa", "511"),
                new DefaultAccountDto("5112", "Doanh thu bán các thành phẩm", "511"),
                new DefaultAccountDto("5113", "Doanh thu cung cấp dịch vụ", "511"),
                new DefaultAccountDto("5114", "Doanh thu trợ cấp, trợ giá", "511"),
                new DefaultAccountDto("5117", "Doanh thu kinh doanh bất động sản đầu tư", "511"),
                new DefaultAccountDto("5118", "Doanh thu khác", "511"),

                new DefaultAccountDto("515", "Doanh thu hoạt động tài chính"),

                new DefaultAccountDto("521", "Các khoản giảm trừ doanh thu"),
                new DefaultAccountDto("5211", "Chiết khấu thương mại", "521"),
                new DefaultAccountDto("5212", "Hàng bán bị trả lại", "521"),
                new DefaultAccountDto("5213", "Giảm giá hàng bán", "521"),

                new DefaultAccountDto("611", "Mua hàng"),
                new DefaultAccountDto("6111", "Mua nguyên liệu, vật liệu", "611"),
                new DefaultAccountDto("6112", "Mua hàng hóa", "611"),

                new DefaultAccountDto("621", "Chi phí nguyên liệu, vật liệu trực tiếp"),
                new DefaultAccountDto("622", "Chi phí nhân công trực tiếp"),

                new DefaultAccountDto("623", "Chi phí sử dụng máy thi công"),
                new DefaultAccountDto("6231", "Chi phí nhân công", "623"),
                new DefaultAccountDto("6232", "Chi phí nguyên, vật liệu", "623"),
                new DefaultAccountDto("6233", "Chi phí dụng cụ sản xuất", "623"),
                new DefaultAccountDto("6234", "Chi phí khấu hao máy thi công", "623"),
                new DefaultAccountDto("6237", "Chi phí dịch vụ mua ngoài", "623"),
                new DefaultAccountDto("6238", "Chi phí bằng tiền khác", "623"),

                new DefaultAccountDto("627", "Chi phí sản xuất chung"),
                new DefaultAccountDto("6271", "Chi phí nhân viên phân xưởng", "627"),
                new DefaultAccountDto("6272", "Chi phí nguyên, vật liệu", "627"),
                new DefaultAccountDto("6273", "Chi phí dụng cụ sản xuất", "627"),
                new DefaultAccountDto("6274", "Chi phí khấu hao TSCĐ", "627"),
                new DefaultAccountDto("6277", "Chi phí dịch vụ mua ngoài", "627"),
                new DefaultAccountDto("6278", "Chi phí bằng tiền khác", "627"),

                new DefaultAccountDto("631", "Giá thành sản xuất"),
                new DefaultAccountDto("632", "Giá vốn hàng bán"),
                new DefaultAccountDto("635", "Chi phí tài chính"),

                new DefaultAccountDto("641", "Chi phí bán hàng"),
                new DefaultAccountDto("6411", "Chi phí nhân viên", "641"),
                new DefaultAccountDto("6412", "Chi phí nguyên vật liệu, bao bì", "641"),
                new DefaultAccountDto("6413", "Chi phí dụng cụ, đồ dùng", "641"),
                new DefaultAccountDto("6414", "Chi phí khấu hao TSCĐ", "641"),
                new DefaultAccountDto("6415", "Chi phí bảo hành", "641"),
                new DefaultAccountDto("6417", "Chi phí dịch vụ mua ngoài", "641"),
                new DefaultAccountDto("6418", "Chi phí bằng tiền khác", "641"),

                new DefaultAccountDto("642", "Chi phí quản lý doanh nghiệp"),
                new DefaultAccountDto("6421", "Chi phí nhân viên quản lý", "642"),
                new DefaultAccountDto("6422", "Chi phí vật liệu quản lý", "642"),
                new DefaultAccountDto("6423", "Chi phí đồ dùng văn phòng", "642"),
                new DefaultAccountDto("6424", "Chi phí khấu hao TSCĐ", "642"),
                new DefaultAccountDto("6425", "Thuế, phí và lệ phí", "642"),
                new DefaultAccountDto("6426", "Chi phí dự phòng", "642"),
                new DefaultAccountDto("6427", "Chi phí dịch vụ mua ngoài", "642"),
                new DefaultAccountDto("6428", "Chi phí bằng tiền khác", "642"),

                new DefaultAccountDto("711", "Thu nhập khác"),
                new DefaultAccountDto("811", "Chi phí khác"),

                new DefaultAccountDto("821", "Chi phí thuế thu nhập doanh nghiệp"),
                new DefaultAccountDto("8211", "Chi phí thuế TNDN hiện hành", "821"),
                new DefaultAccountDto("8212", "Chi phí thuế TNDN hoãn lại", "821"),

                new DefaultAccountDto("911", "Xác định kết quả kinh doanh"),
            };

            return results;
        }
    }

    public class DefaultAccountDto
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public string ParrentCode { get; set; }
        public DefaultAccountDto(string code, string name , string parrentCode = "")
        {
            Name = name;
            Code = code;
            ParrentCode = parrentCode;
        }
    }
}
