using System.Collections.Generic;

namespace VTECHERP.Domain.Shared.Helper.Excel.Model
{
    public class MappingData
    {
        public List<List<string>> RawDatas { get; set; } = new List<List<string>>();
        public List<string> Keys { get; set; } = new List<string>();
        public List<List<MapData>> MappedDatas { get; set; } = new List<List<MapData>>();
    }

    public class MapData
    {
        public string Code { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
        public string CellAddress { get; set; } = string.Empty;
        public int Row { get; set; }
        public int Column { get; set; }
    }
}
