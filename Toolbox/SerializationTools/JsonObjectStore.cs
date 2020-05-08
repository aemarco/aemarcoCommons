using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Toolbox.SerializationTools
{
    public class JsonObjectStore<T>
        where T : class
    {
        #region props

        public string Filepath { get; set; }
        public DateTimeOffset Generated { get; set; } = DateTimeOffset.Now;
        public DateTimeOffset LastSaved { get; set; } = DateTimeOffset.MinValue;
        public Dictionary<string, T> Storage { get; set; } = new Dictionary<string, T>();

        #endregion

        #region Handling entries

        public T GetEntry(string key)
        {
            return Storage.ContainsKey(key) ? Storage[key] : null;
        }

        public void AddOrUpdateEntry(string key, T entry)
        {
            if (!Storage.ContainsKey(key))
            {
                Storage.Add(key, entry);
            }
            else
            {
                Storage[key] = entry;
            }
        }

        public List<string> GetAllKeys()
        {
            return Storage.Keys.ToList();
        }


        public List<T> GetAllValues()
        {
            return Storage.Values.ToList();
        }


       
        public void RemoveMatching(Func<T, bool> filter)
        {
            var removalKeys = Storage
                .Where(x => filter(x.Value))
                .Select(x => x.Key)
                .ToList();
            
            foreach (var key in removalKeys)
            {
                Storage.Remove(key);
            }
        }




        public void RemoveEntry(string key)
        {
            if (key != null && Storage.ContainsKey(key))
            {
                Storage.Remove(key);
            }
        }

        public void RemoveEntry(T entry)
        {
            var key = Storage.FirstOrDefault(x => x.Value == entry).Key;
            if (key != null && Storage.ContainsKey(key))
            {
                Storage.Remove(key);
            }
        }

        public void ClearEntries()
        {
            Storage.Clear();
            Generated = DateTimeOffset.Now;
            LastSaved = DateTimeOffset.MinValue;
        }

        #endregion

        #region serialization

        public bool CheckForAutosave()
        {
            if (LastSaved.AddMinutes(5) < DateTimeOffset.Now)
            {
                Save();
                return true;
            }

            return false;
        }

        /// <summary>
        /// Saves the current object
        /// </summary>
        /// <returns>true if save was successfull</returns>
        public void Save(string file = null)
        {
            if (string.IsNullOrWhiteSpace(file)) file = Filepath;

            try
            {
                lock (Storage)
                {
                    LastSaved = DateTimeOffset.Now;
                    var text = JsonConvert.SerializeObject(this, Formatting.Indented);
                    File.WriteAllText(file, text);
                    
                }
            }
            catch
            {
                try { File.Delete(file); } catch { }
            }
        }

        /// <summary>
        /// gets a storage instance, new one or from saved one
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public static JsonObjectStore<T> Load(string file)
        {
            if (File.Exists(file))
            {
                try
                {
                    var text = File.ReadAllText(file);
                    var result = JsonConvert.DeserializeObject<JsonObjectStore<T>>(text);
                    result.Filepath = file;
                    return result;
                }
                catch { }
            }

            return new JsonObjectStore<T>
            {
                Filepath = file
            };
        }

        #endregion




    }
}
