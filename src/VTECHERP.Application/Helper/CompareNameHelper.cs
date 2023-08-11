using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace VTECHERP.Helper
{
    public static class CompareNameHelper
    {
        public static bool CompareName(string s1, string s2)
        {
            return EF.Functions.Collate(s1, "SQL_Latin1_General_CP1_CI_AI").Contains(EF.Functions.Collate(s2, "SQL_Latin1_General_CP1_CI_AI"));
        }
        public static bool CompareName(List<string> s1, string s2)
        {
            return EF.Functions.Collate(s1, "SQL_Latin1_General_CP1_CI_AI").Contains(EF.Functions.Collate(s2, "SQL_Latin1_General_CP1_CI_AI"));
        }
    }
}
