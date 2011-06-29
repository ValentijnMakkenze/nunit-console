﻿// ***********************************************************************
// Copyright (c) 2011 Charlie Poole
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// ***********************************************************************

using System;
#if CLR_2_0 || CLR_4_0
using System.Collections.Generic;
#else
using System.Collections;
using System.Collections.Specialized;
#endif
using System.IO;

namespace NUnit.Engine
{
    /// <summary>
    /// TestPackage holds information about a set of tests to
    /// be loaded by a TestRunner. Each TestPackage represents
    /// tests for a single assembly. Multiple assemblies are
    /// represented by use of subpackages.
    /// </summary>
    [Serializable]
    public class TestPackage
    {
        private string filePath;
#if CLR_2_0 || CLR_4_0
        private Dictionary<string, object> settings = new Dictionary<string, object>();
        private List<TestPackage> subPackages = new List<TestPackage>();
#else
        private ListDictionary settings = new ListDictionary();
        private ArrayList subPackages = new ArrayList();
#endif

        #region Constructors

        /// <summary>
        /// Construct a TestPackage, specifying a file path for
        /// the assembly or project to be used.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        public TestPackage(string filePath)
        {
            this.filePath = Path.GetFullPath(filePath);
        }

        /// <summary>
        /// Construct an anonymous TestPackage that wraps 
        /// multiple assemblies or projects as subpackages.
        /// </summary>
        /// <param name="testFiles"></param>
        public TestPackage(params string[] testFiles)
        {
            foreach (string testFile in testFiles)
                Add(new TestPackage(testFile));
        }

        /// <summary>
        /// Construct an anonymous TestPackage.
        /// </summary>
        public TestPackage() { }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the name of the package
        /// </summary>
        public string Name
        {
            get { return filePath == null ? null : Path.GetFileName(filePath); }
        }

        /// <summary>
        /// Gets the path to the file containing tests. It may be
        /// an assembly or a recognized project type.
        /// </summary>
        public string FilePath
        {
            get { return filePath; }
        }

        /// <summary>
        /// Gets an array of SubPackages contained in this package.
        /// </summary>
        public TestPackage[] SubPackages
        {
#if CLR_2_0 || CLR_4_0
            get { return subPackages.ToArray(); }
#else
            get { return (TestPackage[])subPackages.ToArray(typeof(TestPackage)); }
#endif
        }

        /// <summary>
        /// Gets an indicator showing whether this package
        /// contains any subpackages.
        /// </summary>
        public bool HasSubPackages
        {
            get { return subPackages.Count > 0; }
        }

        /// <summary>
        /// Gets the settings dictionary for this package.
        /// </summary>
#if CLR_2_0 || CLR_4_0
        public IDictionary<string,object> Settings
#else
        public IDictionary Settings
#endif
        {
            get { return settings; }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Add a subpackage to the package.
        /// </summary>
        /// <param name="package">The package to be added</param>
        public void Add(TestPackage package)
        {
            subPackages.Add(package);
        }

        /// <summary>
        /// Returns the test assemblies to be loaded by this package
        /// </summary>
        public string[] GetAssemblies()
        {
#if CLR_2_0 || CLR_4_0
            List<string> assemblies = new List<string>();
#else
            ArrayList assemblies = new ArrayList();
#endif

            if (HasSubPackages)
            {
                foreach (TestPackage subPackage in subPackages)
                    assemblies.AddRange(subPackage.GetAssemblies());
            }
            else
                assemblies.Add(FilePath);

#if CLR_2_0 || CLR_4_0
            return assemblies.ToArray();
#else
            return (string[])assemblies.ToArray(typeof(string));
#endif
        }

#if CLR_2_0 || CLR_4_0
        /// <summary>
        /// Return the value of a setting or a default.
        /// </summary>
        /// <param name="name">The name of the setting</param>
        /// <param name="defaultSetting">The default value</param>
        /// <returns></returns>
        public T GetSetting<T>(string name, T defaultSetting)
        {
            return Settings.ContainsKey(name)
                ? (T)Settings[name]
                : defaultSetting;
        }
#else
        /// <summary>
        /// Return the value of a setting or a default.
        /// </summary>
        /// <param name="name">The name of the setting</param>
        /// <param name="defaultSetting">The default value</param>
        /// <returns></returns>
        public object GetSetting(string name, object defaultSetting)
        {
            object setting = settings[name];

            return setting == null ? defaultSetting : setting;
        }

        /// <summary>
        /// Return the value of a string setting or a default.
        /// </summary>
        /// <param name="name">The name of the setting</param>
        /// <param name="defaultSetting">The default value</param>
        /// <returns></returns>
        public string GetSetting(string name, string defaultSetting)
        {
            object setting = settings[name];

            return setting == null ? defaultSetting : (string)setting;
        }

        /// <summary>
        /// Return the value of a bool setting or a default.
        /// </summary>
        /// <param name="name">The name of the setting</param>
        /// <param name="defaultSetting">The default value</param>
        /// <returns></returns>
        public bool GetSetting(string name, bool defaultSetting)
        {
            object setting = settings[name];

            return setting == null ? defaultSetting : (bool)setting;
        }

        /// <summary>
        /// Return the value of an int setting or a default.
        /// </summary>
        /// <param name="name">The name of the setting</param>
        /// <param name="defaultSetting">The default value</param>
        /// <returns></returns>
        public int GetSetting(string name, int defaultSetting)
        {
            object setting = settings[name];

            return setting == null ? defaultSetting : (int)setting;
        }

        /// <summary>
        /// Return the value of a enum setting or a default.
        /// </summary>
        /// <param name="name">The name of the setting</param>
        /// <param name="defaultSetting">The default value</param>
        /// <returns></returns>
        public System.Enum GetSetting(string name, System.Enum defaultSetting)
        {
            object setting = settings[name];

            return setting == null ? defaultSetting : (System.Enum)setting;
        }
#endif

        #endregion
    }
}
