using System;
using System.Collections.Generic;

namespace Assets
{
    public static class ListExtensions
    {
        public static TResult For<TIn, TEntity, TResult>(this int count, TIn input, Func<int, TIn, TEntity> iterationByIndex, Func<IEnumerable<TEntity>, TResult> result)
        {
            IList<TEntity> list = new List<TEntity>();
            for (var i = 0; i < count; i++)
            {
                list.Add(iterationByIndex(i, input));
            }
            return result(list);
        }
    }
}
