using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using UnityEngine;

/// <summary>
/// Json形式でセーブできるクラスを提供します。
/// </summary>
/// <remarks>
/// 最初に値を設定、取得するタイミングでファイル読み出します。
/// </remarks>
public class SaveData
{
    /// <summary>
    /// SingletonなSaveDatabaseクラス
    /// </summary>
    private static SaveDataBase _savedatabase = null;

    private static SaveDataBase savedatabase
    {
        get {
            if(_savedatabase == null)
            {
                string path = $"{Application.dataPath}/";
                string fileName = $"{Application.productName}.savedata.json";
                Debug.Log($"SaveData:{path}{fileName}");
                _savedatabase = new SaveDataBase(path, fileName);
            }
            return _savedatabase;
        }
    }

    private SaveData()
    {
    }

    #region Public Static Methods

    /// <summary>
    /// 指定したキーとT型のクラスコレクションをセーブデータに追加します。
    /// </summary>
    /// <typeparam name="T">ジェネリッククラス</typeparam>
    /// <param name="key">キー</param>
    /// <param name="list">T型のList</param>
    /// <exception cref="System.ArgumentException"></exception>
    /// <remarks>指定したキーとT型のクラスコレクションをセーブデータに追加します。</remarks>
    public static void SetList<T>(string key, List<T> list)
    {
        savedatabase.SetList(key, list);
    }

    /// <summary>
    ///  指定したキーとT型のクラスコレクションをセーブデータから取得します。
    /// </summary>
    /// <typeparam name="T">ジェネリッククラス</typeparam>
    /// <param name="key">キー</param>
    /// <param name="_default">デフォルトの値</param>
    /// <exception cref="System.ArgumentException"></exception>
    /// <returns></returns>
    public static List<T> GetList<T>(string key, List<T> _default) => savedatabase.GetList<T>(key, _default);

    /// <summary>
    ///  指定したキーとT型のクラスをセーブデータに追加します。
    /// </summary>
    /// <typeparam name="T">ジェネリッククラス</typeparam>
    /// <param name="key">キー</param>
    /// <param name="_default">デフォルトの値</param>
    /// <exception cref="System.ArgumentException"></exception>
    /// <returns></returns>
    public static T GetClass<T>(string key, T _default) where T : class, new() => savedatabase.GetClass(key, _default);

    /// <summary>
    ///  指定したキーとT型のクラスコレクションをセーブデータから取得します。
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <param name="obj"></param>
    /// <exception cref="System.ArgumentException"></exception>
    public static void SetClass<T>(string key, T obj) where T : class, new()
    {
        savedatabase.SetClass(key, obj);
    }

    /// <summary>
    /// 指定されたキーに関連付けられている値を取得します。
    /// </summary>
    /// <param name="key">キー</param>
    /// <param name="value">値</param>
    /// <exception cref="System.ArgumentException"></exception>
    public static void SetString(string key, string value)
    {
        savedatabase.SetString(key, value);
    }

    /// <summary>
    /// 指定されたキーに関連付けられているString型の値を取得します。
    /// 値がない場合、_defaultの値を返します。省略した場合、空の文字列を返します。
    /// </summary>
    /// <param name="key">キー</param>
    /// <param name="_default">デフォルトの値</param>
    /// <exception cref="System.ArgumentException"></exception>
    /// <returns></returns>
    public static string GetString(string key, string _default = "") => savedatabase.GetString(key, _default);

    /// <summary>
    /// 指定されたキーに関連付けられているInt型の値を取得します。
    /// </summary>
    /// <param name="key">キー</param>
    /// <param name="value">デフォルトの値</param>
    /// <exception cref="System.ArgumentException"></exception>
    public static void SetInt(string key, int value)
    {
        savedatabase.SetInt(key, value);
    }

    /// <summary>
    /// 指定されたキーに関連付けられているInt型の値を取得します。
    /// 値がない場合、_defaultの値を返します。省略した場合、0を返します。
    /// </summary>
    /// <param name="key">キー</param>
    /// <param name="_default">デフォルトの値</param>
    /// <exception cref="System.ArgumentException"></exception>
    /// <returns></returns>
    public static int GetInt(string key, int _default = 0) => savedatabase.GetInt(key, _default);

    /// <summary>
    /// 指定されたキーに関連付けられているfloat型の値を取得します。
    /// </summary>
    /// <param name="key">キー</param>
    /// <param name="value">デフォルトの値</param>
    /// <exception cref="System.ArgumentException"></exception>
    public static void SetFloat(string key, float value)
    {
        savedatabase.SetFloat(key, value);
    }

    /// <summary>
    /// 指定されたキーに関連付けられているfloat型の値を取得します。
    /// 値がない場合、_defaultの値を返します。省略した場合、0.0fを返します。
    /// </summary>
    /// <param name="key">キー</param>
    /// <param name="_default">デフォルトの値</param>
    /// <exception cref="System.ArgumentException"></exception>
    /// <returns></returns>
    public static float GetFloat(string key, float _default = 0.0f) => savedatabase.GetFloat(key, _default);

    /// <summary>
    /// セーブデータからすべてのキーと値を削除します。
    /// </summary>
    public static void Clear()
    {
        savedatabase.Clear();
    }

    /// <summary>
    /// 指定したキーを持つ値を セーブデータから削除します。
    /// </summary>
    /// <param name="key">キー</param>
    /// <exception cref="System.ArgumentException"></exception>
    public static void Remove(string key)
    {
        savedatabase.Remove(key);
    }

    /// <summary>
    /// セーブデータ内にキーが存在するかを取得します。
    /// </summary>
    /// <param name="_key">キー</param>
    /// <exception cref="System.ArgumentException"></exception>
    /// <returns></returns>
    public static bool ContainsKey(string _key) => savedatabase.ContainsKey(_key);

    /// <summary>
    /// セーブデータに格納されたキーの一覧を取得します。
    /// </summary>
    /// <exception cref="System.ArgumentException"></exception>
    /// <returns></returns>
    public static List<string> Keys() => savedatabase.Keys();

    /// <summary>
    /// 明示的にファイルに書き込みます。
    /// </summary>
    public static void Save()
    {
        savedatabase.Save();
    }

    #endregion

    #region SaveDatabase Class

    [Serializable]
    private class SaveDataBase
    {
        #region Fields

        private string _path;
        //保存先
        public string path
        {
            get { return _path; }
            set { _path = value; }
        }

        private string _fileName;
        //ファイル名
        public string fileName
        {
            get { return _fileName; }
            set { _fileName = value; }
        }

        private Dictionary<string, string> saveDictionary;
        //keyとjson文字列を格納

        #endregion

        #region Constructor&Destructor

        public SaveDataBase(string _path, string _fileName)
        {
            this._path = _path;
            this._fileName = _fileName;
            saveDictionary = new Dictionary<string, string>();
            Load();
        }

        /// <summary>
        /// クラスが破棄される時点でファイルに書き込みます。
        /// </summary>
        ~SaveDataBase()
        {
            Save();
        }

        #endregion

        #region Public Methods

        public void SetList<T>(string key, List<T> list)
        {
            CheckKey(key);
            var serializableList = new Serialization<T>(list);
            string json = JsonUtility.ToJson(serializableList);
            saveDictionary[key] = json;
        }

        public List<T> GetList<T>(string key, List<T> _default)
        {
            CheckKey(key);
            if(!saveDictionary.ContainsKey(key)) return _default;
            string json = saveDictionary[key];
            Serialization<T> deserializeList = JsonUtility.FromJson<Serialization<T>>(json);

            return deserializeList.ToList();
        }

        public T GetClass<T>(string key, T _default) where T : class, new()
        {
            CheckKey(key);
            if(!saveDictionary.ContainsKey(key)) return _default;

            string json = saveDictionary[key];
            T obj = JsonUtility.FromJson<T>(json);
            return obj;
        }

        public void SetClass<T>(string key, T obj) where T : class, new()
        {
            CheckKey(key);
            string json = JsonUtility.ToJson(obj);
            saveDictionary[key] = json;
        }

        public void SetString(string key, string value)
        {
            CheckKey(key);
            saveDictionary[key] = value;
        }

        public string GetString(string key, string _default)
        {
            CheckKey(key);

            if(!saveDictionary.ContainsKey(key)) return _default;
            return saveDictionary[key];
        }

        public void SetInt(string key, int value)
        {
            CheckKey(key);
            saveDictionary[key] = value.ToString();
        }

        public int GetInt(string key, int _default)
        {
            CheckKey(key);
            if(!saveDictionary.ContainsKey(key)) return _default;
            int ret;
            if(!int.TryParse(saveDictionary[key], out ret)) ret = 0;
            return ret;
        }

        public void SetFloat(string key, float value)
        {
            CheckKey(key);
            saveDictionary[key] = value.ToString();
        }

        public float GetFloat(string key, float _default)
        {
            float ret;
            CheckKey(key);
            if(!saveDictionary.ContainsKey(key)) ret = _default;

            if(!float.TryParse(saveDictionary[key], out ret)) ret = 0.0f;
            return ret;
        }

        public void Clear()
        {
            saveDictionary.Clear();
        }

        public void Remove(string key)
        {
            CheckKey(key);
            if(saveDictionary.ContainsKey(key)) saveDictionary.Remove(key);
        }

        public bool ContainsKey(string _key) => saveDictionary.ContainsKey(_key);

        public List<string> Keys() => saveDictionary.Keys.ToList();

        public void Save()
        {
            using(var writer = new StreamWriter(_path + _fileName, false, Encoding.GetEncoding("utf-8")))
            {
                var serialDict = new Serialization<string, string>(saveDictionary);
                serialDict.OnBeforeSerialize();
                string dictJsonString = JsonUtility.ToJson(serialDict);
                writer.WriteLine(dictJsonString);
            }
        }

        public void Load()
        {
            if(File.Exists(_path + _fileName))
            {
                using(var stream = new StreamReader(_path + _fileName, Encoding.GetEncoding("utf-8")))
                {
                    if(saveDictionary != null)
                    {
                        var sDict = JsonUtility.FromJson<Serialization<string, string>>(stream.ReadToEnd());
                        sDict.OnAfterDeserialize();
                        saveDictionary = sDict.ToDictionary();
                    }
                }
            }
            else
            {
                saveDictionary = new Dictionary<string, string>();
            }
        }

        public string GetJsonString(string key)
        {
            CheckKey(key);
            if(saveDictionary.ContainsKey(key))
            {
                return saveDictionary[key];
            }
            else
            {
                return null;
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// キーに不正がないかチェックします。
        /// </summary>
        private void CheckKey(string _key)
        {
            if(string.IsNullOrEmpty(_key)) throw new ArgumentException("invalid key!!");
        }

        #endregion
    }

    #endregion

    #region Serialization Class

    // List<T>
    [Serializable]
    private class Serialization<T>
    {
        public List<T> target;

        public List<T> ToList() => target;

        public Serialization()
        {
        }

        public Serialization(List<T> target)
        {
            this.target = target;
        }
    }
    // Dictionary<TKey, TValue>
    [Serializable]
    private class Serialization<TKey, TValue>
    {
        public List<TKey> keys;
        public List<TValue> values;
        private Dictionary<TKey, TValue> target;

        public Dictionary<TKey, TValue> ToDictionary() => target;

        public Serialization()
        {
        }

        public Serialization(Dictionary<TKey, TValue> target)
        {
            this.target = target;
        }

        public void OnBeforeSerialize()
        {
            keys = new List<TKey>(target.Keys);
            values = new List<TValue>(target.Values);
        }

        public void OnAfterDeserialize()
        {
            int count = Math.Min(keys.Count, values.Count);
            target = new Dictionary<TKey, TValue>(count);
            Enumerable.Range(0, count).ToList().ForEach(i => target.Add(keys[i], values[i]));
        }
    }

    #endregion
}