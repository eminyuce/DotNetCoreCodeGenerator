using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetCodeGenerator.Domain.Entities.Enums
{
    public enum UserMessageState
    {
        Error=-1,
        Welcome = 0,
        Warning =1,
        Success=2,
    }
}
