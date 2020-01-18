using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Design;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using BankDatabase_v2.DataModel;

namespace BankDatabase_v2.Extensions
{

    public static class Extensions
    {
        /// <summary>
        /// Add an item to a list and return a reference to the added item
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static T AddAndReturn<T>(this List<T> list, T obj)
        {
            list.Add(obj);
            return obj;
        }

        /// <summary>
        /// Add an item to a BdSet and return a reference to the added item
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static TEntity AddAndReturn<TEntity>(this DbSet<TEntity> dbSet, TEntity obj) where TEntity : class
        {
            dbSet.Add(obj);
            return obj;
        }

        /// <summary>
        /// Adds an item to a list, or replaced an item determined to be .Equal() to
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="obj"></param>
        public static void AddOrReplace<T>(this List<T> list, T obj)
        {
            list.RemoveAll(x => x.Equals(obj));
            list.Add(obj);
        }

    }

    public class BankEventArgs : EventArgs
    {
        public Bank Bank;
    }

    public class FlagEventArgs : EventArgs
    {
        public bool Flag;
    }

    public class CountEventArgs : EventArgs
    {
        public int Count;
    }


}