using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BSOL.Core.Models.Common
{
    public class ReturnMessage
    {
        public bool HasError { get; set; }
        public string Message { get; set; }
        public object Id { get; set; }
        public object Data { get; set; }
    }
}
