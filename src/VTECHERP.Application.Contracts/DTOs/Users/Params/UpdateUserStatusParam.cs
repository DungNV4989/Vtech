using System;
using System.Collections.Generic;
using System.Text;

namespace VTECHERP.DTOs.Users.Params
{
    public class UpdateUserStatusParam
    {
        public Guid UserId { get; set; }
        public bool IsActive { get; set; }
    }
}
