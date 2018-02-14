using System.Collections.Generic;

namespace <%= namespace %>.Tests
{
    public class <%= modelName %>Comparer : IEqualityComparer<<%= modelName %>>
    {
        public bool Equals(<%= modelName %> x, <%= modelName %> y)
        {
            return x.Id == y.Id
               && x.CreationDate == y.CreationDate;
        }

        public int GetHashCode(<%= modelName %> obj)
        {
            return obj.Id.GetHashCode()
                + obj.CreationDate.GetHashCode();
        }
    }
}
