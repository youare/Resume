using System;
using System.Collections.Generic;
using System.Text;

namespace Common
{
    public static class Contracts
    {
        public static void ErrorIf(bool predicate, string errorMessage)
        {
            if (predicate) throw new Exception(errorMessage);
        }
    }
}
