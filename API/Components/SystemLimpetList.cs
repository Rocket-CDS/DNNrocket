using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using DNNrocketAPI.Components;
using Simplisity;

namespace DNNrocketAPI.Components
{

    public class SystemLimpetList
    {
        private static readonly object _lockObject = new object();
        private readonly ConcurrentDictionary<string, SystemLimpet> _systemDictionary;
        private static readonly string _globalCacheKey = "SystemLimpetList_Global";

        public SystemLimpetList()
        {
            _systemDictionary = new ConcurrentDictionary<string, SystemLimpet>();
            LoadSystems();
        }

        private void LoadSystems()
        {
            // Try to get the complete system list from cache first
            var cachedSystems = (ConcurrentDictionary<string, SystemLimpet>)CacheUtils.GetCache(_globalCacheKey);
            
            if (cachedSystems != null && cachedSystems.Count > 0)
            {
                // Use cached data
                foreach (var kvp in cachedSystems)
                {
                    _systemDictionary.TryAdd(kvp.Key, kvp.Value);
                }
                return;
            }

            // If not cached, scan folders and build the list
            lock (_lockObject)
            {
                // Double-check pattern for cache
                cachedSystems = (ConcurrentDictionary<string, SystemLimpet>)CacheUtils.GetCache(_globalCacheKey);
                if (cachedSystems != null && cachedSystems.Count > 0)
                {
                    foreach (var kvp in cachedSystems)
                    {
                        _systemDictionary.TryAdd(kvp.Key, kvp.Value);
                    }
                    return;
                }

                // Scan both folders and collect all systems
                var searchFolders = new[]
                {
                    DNNrocketUtils.MapPath("/DesktopModules/DNNrocketModules"),
                    DNNrocketUtils.MapPath("/DesktopModules/DNNrocket")
                };

                var foundSystems = new ConcurrentDictionary<string, SystemLimpet>();

                foreach (var searchFolder in searchFolders)
                {
                    if (Directory.Exists(searchFolder))
                    {
                        ScanFolderForSystems(searchFolder, foundSystems);
                    }
                }

                // Copy to instance dictionary
                foreach (var kvp in foundSystems)
                {
                    _systemDictionary.TryAdd(kvp.Key, kvp.Value);
                }

                // Cache the complete result
                CacheUtils.SetCache(_globalCacheKey, foundSystems);
            }
        }

        private void ScanFolderForSystems(string searchFolder, ConcurrentDictionary<string, SystemLimpet> systemCollection)
        {
            try
            {
                var directories = Directory.GetDirectories(searchFolder);
                
                foreach (var directory in directories)
                {
                    var systemFile = Path.Combine(directory, "system.rules");
                    if (File.Exists(systemFile))
                    {
                        string dirName = new DirectoryInfo(directory).Name.ToLower();
                        
                        // Avoid duplicates by checking if system already exists
                        if (!systemCollection.ContainsKey(dirName))
                        {
                            var systemData = SystemSingleton.Instance(dirName);
                            
                            if (ModuleUtils.ExtensionExists(systemData.SystemKey) && !systemData.IsPlugin)
                            {
                                systemCollection.TryAdd(dirName, systemData);
                            }
                        }
                    }
                }
            }
            catch (UnauthorizedAccessException)
            {
                // Log or handle access denied scenarios silently
                // Could add logging here if needed
            }
            catch (DirectoryNotFoundException)
            {
                // Handle case where directory doesn't exist silently
                // Could add logging here if needed
            }
            catch (Exception)
            {
                // Handle any other unexpected exceptions silently
                // Could add logging here if needed
            }
        }

        /// <summary>
        /// Clears the global cache and reloads systems. Use sparingly as this affects all instances.
        /// </summary>
        public void RefreshSystems()
        {
            lock (_lockObject)
            {
                CacheUtils.RemoveCache(_globalCacheKey);
                _systemDictionary.Clear();
                LoadSystems();
            }
        }

        #region "base methods"

        public List<SystemLimpet> GetSystemList()
        {
            return _systemDictionary.Values.ToList();
        }

        public List<SystemLimpet> GetSystemActiveList()
        {
            return _systemDictionary.Values.Where(s => s.Active).ToList();
        }

        public SystemLimpet GetSystemByKey(string key)
        {
            if (string.IsNullOrEmpty(key)) return null;
            
            key = key.ToLower();
            
            // Try direct lookup first (most efficient)
            if (_systemDictionary.TryGetValue(key, out SystemLimpet system))
            {
                return system;
            }

            // Fallback to XML property search if key doesn't match dictionary key
            return _systemDictionary.Values.FirstOrDefault(s => 
                s.Record.GetXmlProperty("genxml/systemkey").Equals(key, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Gets count of loaded systems
        /// </summary>
        public int Count => _systemDictionary.Count;

        /// <summary>
        /// Checks if a system exists by key
        /// </summary>
        public bool ContainsKey(string key)
        {
            if (string.IsNullOrEmpty(key)) return false;
            return _systemDictionary.ContainsKey(key.ToLower());
        }

        #endregion
    }
}
