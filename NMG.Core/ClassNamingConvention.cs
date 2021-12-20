using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NMG.Core
{
    public enum ClassNamingConvention
    {
        SameAsDatabase,
        CamelCase,
        Prefixed,
        /// <summary>
        /// Upper camel case.
        /// </summary>
        PascalCase
    }
}
