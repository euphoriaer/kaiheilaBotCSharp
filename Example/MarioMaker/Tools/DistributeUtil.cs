﻿using System;
using System.Collections.Generic;
using System.Reflection;

namespace MarioMaker
{
    [AttributeUsage(AttributeTargets.Method)]
    public abstract class DistributeAttr : Attribute
    {
        public readonly string TypeName;

        public DistributeAttr(string msgName)
        {
            this.TypeName = msgName;
        }
    }

    /// <summary>
    /// 通用方法分发工具
    /// </summary>
    /// <typeparam name="T1">方法类型，如Action</typeparam>
    /// <typeparam name="T2">特性类</typeparam>
    /// <typeparam name="T3">要分发方法的class</typeparam>
    public class DistributeUtil<T1, T2, T3>
        where T1 : Delegate
        where T2 : DistributeAttr
    {
        private Dictionary<string, T1> _cache;

        /// <summary>
        ///
        /// </summary>
        /// <param name="obj">哪个对象上的方法，静态为null</param>
        /// <param name="bindingFlags">方法类型，public private static等等，多个判断条件用 | 连接</param>
        public DistributeUtil(object? obj)
        {
            _cache = new Dictionary<string, T1>();
            //todo GetMethods 方法获取 是否会受到Flags在性能方面的影响（Flags 数量影响速度？）
            var methods = typeof(T3).GetMethods(BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);

            foreach (var method in methods)
            {
                foreach (var attribute in method.GetCustomAttributes() as Attribute[])
                {
                    if (attribute is T2 bAtt)
                    {
                        _cache[bAtt.TypeName] = (T1)method.CreateDelegate(typeof(T1), obj);
                        break;
                    }
                }
            }
        }

        public T1 GetMethod(string name)
        {
            return _cache[name];
        }
    }
}