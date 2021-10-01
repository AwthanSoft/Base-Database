using System;
using System.Collections.Generic;
using System.Text;

using System.Reflection;
using System.Linq.Expressions;
//using System.Data.Entity.Migrations;
using System.Linq;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Mawa.DBCore
{
    public static class ModelDBControllerExtension
    {
        public static IQueryable<T> Where<T>(this ModelDBController<T> modelDBController, Expression<Func<T, bool>> predicate)
            where T : DBModelCore
        {
            return modelDBController.db_Model_Ex.Where(predicate);
        }
        //public static IQueryable<TSource> Where<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, bool>> predicate)
        //    where TSource : DBModelCore
        //{
        //    return source.Where(predicate);
        //}

        //public static IQueryable<TResult> Select<TSource, TResult>(this IQueryable<TSource> source, Expression<Func<TSource, TResult>> selector)
        //    where TSource : DBModelCore
        //{
        //    return source.Select(selector);
        //}
        public static IQueryable<TResult> Select<T, TResult>(this ModelDBController<T> modelDBController, Expression<Func<T, TResult>> selector)
            where T : DBModelCore
        {
            return modelDBController.db_Model_Ex.Select(selector);
        }

        //public static IQueryable<TResult> Execute<T, TResult>(this ModelDBController<T> modelDBController, Expression<Func<T, TResult>> selector)
        //    where T : DBModelCore
        //{
        //    return modelDBController.db_Model_Ex.Select(selector);
        //}
        public static TSource[] ToArray<TSource>(this IEnumerable<TSource> source , ModelDBController<TSource> modelDBController)
            where TSource : DBModelCore
        {
            modelDBController.open_lock_Ex();
            var resultt = source.ToArray();
            modelDBController.close_lock_Ex();

            return resultt;
        }
        public static TSource[] ToArray<TSource, T>(this IEnumerable<TSource> source, ModelDBController<T> modelDBController)
            where T : DBModelCore
        {
            modelDBController.open_lock_Ex();
            var resultt = source.ToArray();
            modelDBController.close_lock_Ex();

            return resultt;
        }
        //public static TSource[] ToArray<TSource>(this IQueryable<TSource> source, ModelDBController<TSource> modelDBController)
        //{
        //    modelDBController.open_lock_Ex();
        //    var resultt = source.ToArray();
        //    modelDBController.close_lock_Ex();

        //    return resultt;
        //}
    }
}
