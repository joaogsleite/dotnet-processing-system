﻿using System.Collections.Generic;

namespace LibCustomOperator{
    public class IdentityOperator
    {
        public IList<IList<string>> CustomOperation(IList<string> l)
        {
            IList<IList<string>> result = new List<IList<string>>();
            result.Add(l);
            return result;
        }
    }
}