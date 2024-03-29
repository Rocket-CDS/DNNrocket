﻿using System;
using System.Collections.Generic;
using System.Runtime.Remoting;
using Simplisity;
using DNNrocketAPI.Components;
using DNNrocketAPI.Interfaces;

namespace DNNrocketAPI
{
    public interface IMenuInterface
    {
        List<PageRecordData> GetMenuItems(int portalId, string cultureCode, string systemKey, string rootref);
        string TokenPrefix();
        int PageId(int portalId, string cultureCode);
        int ParentId(string rootref);
    }
    public sealed class MenuInterface
    {
        private static Dictionary<string, IMenuInterface> _instances;
        private static object _lock = new object();
        public static IMenuInterface GetInstance(string assembly, string nameSpaceClass)
        {
            var provKey = assembly + "," + nameSpaceClass;
            lock (_lock)
            {
                if ((_instances == null))
                {
                    _instances = new Dictionary<string, IMenuInterface>();
                }
                if (!_instances.ContainsKey(provKey))
                {
                    _instances.Add(provKey, CreateProvider(assembly, nameSpaceClass));
                }
            }
            return _instances[provKey];

        }
        private static IMenuInterface CreateProvider(string assembly, string nameSpaceClass)
        {
            if (!string.IsNullOrEmpty(assembly) && !string.IsNullOrEmpty(nameSpaceClass))
            {
                try
                {
                    var handle = Activator.CreateInstance(assembly.Trim(), nameSpaceClass.Trim());
                    return (IMenuInterface)handle.Unwrap();
                }
                catch (Exception ex)
                {
                    LogUtils.LogException(ex);
                    return null;
                }
            }
            return null;
        }
        private MenuInterface()
        {
        }
    }

}

