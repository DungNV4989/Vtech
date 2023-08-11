using System;

namespace VTECHERP.DTOs.Base
{
    public class EnumMasterData<T> where T : Enum
    {
        public T Id { get; set; }
        public string Name { get; set; }
        public string Value { get
            {
                return Name;
            } 
        }
    }
}
