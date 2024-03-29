﻿using System;
using System.Collections.Generic;
using System.Reflection;

namespace CsharpBot
{
    [AttributeUsage(AttributeTargets.Method)]
    internal abstract class DistributeAttr : Attribute
    {
        internal readonly string typeName;

        internal DistributeAttr(string msgName)
        {
            this.typeName = msgName;
        }
    }

    /// <summary>
    /// 通用方法分发工具
    /// </summary>
    /// <typeparam name="T1">方法类型，如Action</typeparam>
    /// <typeparam name="T2">特性类</typeparam>
    /// <typeparam name="T3">要分发方法的class</typeparam>
    internal class DistributeUtil<T1, T2, T3>
        where T1 : Delegate
        where T2 : DistributeAttr
    {
        private Dictionary<string, T1> Cache;

        /// <summary>
        ///
        /// </summary>
        /// <param name="obj">哪个对象上的方法，静态为null</param>
        /// <param name="bindingFlags">方法类型，public private static等等，多个判断条件用 | 连接</param>
        internal DistributeUtil(object? obj)
        {
            Cache = new Dictionary<string, T1>();
            //todo GetMethods 方法获取 是否会受到Flags在性能方面的影响（Flags 数量影响速度？）
            var methods = typeof(T3).GetMethods(BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);

            foreach (var method in methods)
            {
                foreach (var attribute in method.GetCustomAttributes() as Attribute[])
                {
                    if (attribute is T2 bAtt)
                    {
                        Cache[bAtt.typeName] = (T1)method.CreateDelegate(typeof(T1), obj);
                        break;
                    }
                }
            }
        }

        internal T1 GetMethod(string name)
        {
            return Cache[name];
        }
    }
}