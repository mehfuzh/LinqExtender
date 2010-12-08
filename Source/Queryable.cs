using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace LinqExtender
{
    /// <summary>
    /// Defines various operations that extend the LINQ query.
    /// </summary>
    public static class Queryable
    {
        public static IQueryable<TSource> Where<TSource>(this IQueryContext<TSource> source, Expression<Func<TSource, bool>> predicate)
        {
            MethodInfo currentMethod = (MethodInfo) MethodInfo.GetCurrentMethod();
            var args = new[] { typeof(TSource) };
            return CreateQuery<TSource, TSource, Func<TSource, bool>>(source, currentMethod, predicate, args);
        }

        public static IQueryable<TSource> OrderBy<TSource, TKey>(this IQueryContext<TSource> source, Expression<Func<TSource, TKey>> keySelector)
        {
            MethodInfo currentMethod = (MethodInfo)MethodInfo.GetCurrentMethod();
            var args = new[] { typeof(TSource), typeof(TKey) };
            return CreateQuery<TSource, TSource, Func<TSource, TKey>>(source, currentMethod, keySelector, args);
        }

        public static IQueryable<TSource> OrderByDescending<TSource, TKey>(this IQueryContext<TSource> source, Expression<Func<TSource, TKey>> keySelector)
        {
            var currentMethod = (MethodInfo)MethodInfo.GetCurrentMethod();
            var args = new[] { typeof(TSource), typeof(TKey) };
            return CreateQuery<TSource, TSource, Func<TSource, TKey>>(source, currentMethod, keySelector, args);
        }

        public static IEnumerable<TResult> Join<TOuter, TInner, TKey, TResult>(this IQueryContext<TOuter> outer, 
            IQueryContext<TInner> inner, 
            Expression<Func<TOuter, TKey>> outerKeySelector, 
            Expression<Func<TInner, TKey>> innerKeySelector, 
            Expression<Func<TOuter, TInner, TResult>> resultSelector){

            return null;
        }


        public static IEnumerable<TSource> Intersect<TSource>(this IQueryContext<TSource> first, IEnumerable<TSource> second)
        {
            return null;
        }

        private static IQueryable<TResult> CreateQuery<TSource, TResult, TDelegate>(IQueryContext<TSource> source,
            MethodInfo methodInfo, 
            Expression<TDelegate> expression, 
            Type[] genArgs)
        {
            BindingFlags flags = BindingFlags.Static | BindingFlags.Public;

            var methodGenArgs = methodInfo.GetGenericArguments();

            var targetMethod = typeof(Enumerable).GetMethods(flags)
                .Where(x => x.Name == methodInfo.Name)
                .Where(x => x.GetGenericArguments().All(arg =>
                {
                    return methodGenArgs.Any(y => y.Name == arg.Name);
                }))
                .First();

            targetMethod = targetMethod.MakeGenericMethod(genArgs);

            IList<TSource> list = new List<TSource>();
            Expression constant = Expression.Constant(list.AsQueryable(), typeof(IQueryable<TSource>));
            Expression call = Expression.Call(targetMethod, constant, expression);

            return GetProvider(source).CreateQuery<TResult>(call);
        }

        public static IQueryable<TResult> Select<TSource, TResult>(this IQueryContext<TSource> source, Expression<Func<TSource, int, TResult>> selector)
        {
            return new List<TResult>().AsQueryable();
        }

        public static IQueryable<TResult> Select<TSource, TResult>(this IQueryContext<TSource> source, Expression<Func<TSource, TResult>> selector)
        {
            MethodInfo currentMethod = (MethodInfo)MethodInfo.GetCurrentMethod();
            var args = new[] { typeof(TSource), typeof(TResult) };
            return CreateQuery<TSource, TResult, Func<TSource, TResult>>(source, currentMethod, selector, args) as IQueryable<TResult>;
        }

        public static IQueryable<TResult> Select<TSource, TResult>(this IQueryable<TSource> source, Expression<Func<TSource, TResult>> selector)
        {
            return Select<TSource, TResult>((IQueryContext<TSource>)source, selector);
        }

        private static IQueryProvider GetProvider<TSource>(IQueryContext<TSource> source)
        {
            if (providers.ContainsKey(source))
                return providers[source];

            providers.Add(source, new QueryProvider<TSource>(source));

            return providers[source];

        }

        private static IQueryProvider provider;

        [ThreadStatic]
        static IDictionary<object, IQueryProvider> providers = new Dictionary<object, IQueryProvider>();
    }
}
