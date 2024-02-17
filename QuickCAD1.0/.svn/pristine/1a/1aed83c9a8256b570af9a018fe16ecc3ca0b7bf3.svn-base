using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;


namespace Max.BaseKit.Utils
{
    
    /// <summary>
    /// 提供反射的常用操作方法
    /// </summary>
    public sealed class ReflectionUtil
    {
        
        private static void DistillMethods(Type interfaceType, ref IList<MethodInfo> methodList)
        {
            foreach (MethodInfo meth in interfaceType.GetMethods())
            {
                var isExist = false;
                foreach (MethodInfo temp in methodList)
                {
                    if ((temp.Name == meth.Name) && (temp.ReturnType == meth.ReturnType))
                    {
                        var para1 = temp.GetParameters();
                        var para2 = meth.GetParameters();
                        if (para1.Length == para2.Length)
                        {
                            var same = true;
                            for (var i = 0; i < para1.Length; i++)
                            {
                                if (para1[i].ParameterType != para2[i].ParameterType)
                                {
                                    same = false;
                                }
                            }

                            if (same)
                            {
                                isExist = true;
                                break;
                            }
                        }
                    }
                }

                if (!isExist)
                {
                    methodList.Add(meth);
                }
            }

            foreach (Type superInterfaceType in interfaceType.GetInterfaces())
            {
                ReflectionUtil.DistillMethods(superInterfaceType, ref methodList);
            }
        }
        private static List<string> GetFiles(string dllPath)
        {
            var fileTypeList = new string[] { "Ms*.dll","Ga*.dll","*.exe"};
            var fileList = new List<string>();
            foreach (string fileType in fileTypeList)
            {
                foreach (string fileName in Directory.GetFiles(dllPath, fileType, SearchOption.TopDirectoryOnly))
                {
                    fileList.Add(fileName);
                }
            }
            return fileList;
        }
        private static void LoadDerivedTypeInAllFolder(Type baseType, IList<Type> derivedTypeList, string folderPath, TypeLoadConfig config)
        {
            ReflectionUtil.LoadDerivedTypeInOneFolder(baseType, derivedTypeList, folderPath, config);
            var folders = Directory.GetDirectories(folderPath);
            if (folders != null)
            {
                foreach (string nextFolder in folders)
                {
                    ReflectionUtil.LoadDerivedTypeInAllFolder(baseType, derivedTypeList, nextFolder, config);
                }
            }
        }
        private static void LoadDerivedTypeInOneFolder(Type baseType, IList<Type> derivedTypeList, string folderPath, TypeLoadConfig config)
        {
            var files = Directory.GetFiles(folderPath);
            foreach (string file in files)
            {
                if (config.TargetFilePostfix != null)
                {
                    if (!file.EndsWith(config.TargetFilePostfix))
                    {
                        continue;
                    }
                }

                Assembly asm = null;

                try
                {
                    if (config.CopyToMemory)
                    {
                        var addinStream = FileUtil.ReadFileReturnBytes(file);
                        asm = Assembly.Load(addinStream);
                    }
                    else
                    {
                        asm = Assembly.LoadFrom(file);
                    }
                }
                catch
                {
                }

                if (asm == null)
                {
                    continue;
                }

                var types = asm.GetTypes();

                foreach (Type t in types)
                {
                    if (t.IsSubclassOf(baseType) || baseType.IsAssignableFrom(t))
                    {
                        var canLoad = config.LoadAbstractType ? true : (!t.IsAbstract);
                        if (canLoad)
                        {
                            derivedTypeList.Add(t);
                        }
                    }
                }
            }
        }

        
        /// <summary>
        /// 在当前程序域中，查找所有指定接口的子类
        /// </summary>
        /// <typeparam name="TBase">基础类型（或接口类型）</typeparam>
        /// <returns>子类列表</returns>
        public static IList<Type> FindSubClasses<TBase>()
        {
            var parentType = typeof(TBase);
            return FindSubClasses(parentType);
        }
        /// <summary>
        /// 在当前程序域中，查找所有指定接口的子类
        /// </summary>
        /// <typeparam name="parentType">基础类型（或接口类型）</typeparam>
        /// <returns>子类列表</returns>
        public static IList<Type> FindSubClasses(Type parentType)
        {
            var path = AppDomain.CurrentDomain.BaseDirectory;
            var files = GetFiles(path);

            var list = new List<Type>();

            foreach (string fileName in files)
            {
                try
                {
                    Assembly asm = Assembly.LoadFrom(fileName);
                    foreach (Type t in asm?.GetTypes())
                    {
                        if (parentType.IsAssignableFrom(t) && (!t.IsAbstract) && (!t.IsInterface))
                        {
                            list.Add(t);
                        }
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("反射错误" + ex.Message);
                }
            }

            return list;
        }
        /// <summary>
        /// 在指定程序集中查找所有继承自TBase的类型
        /// </summary>
        /// <typeparam name="TBase">基础类型（或接口类型）</typeparam>
        /// <param name="asm">目标程序集</param>
        /// <returns>TBase子类列表</returns>
        public static IList<Type> FindSubClasses<TBase>(Assembly asm)
        {
            IList<Type> list = new List<Type>();

            var parentType = typeof(TBase);
            foreach (Type t in asm.GetTypes())
            {
                if (parentType.IsAssignableFrom(t) && (!t.IsAbstract) && (!t.IsInterface))
                {
                    list.Add(t);
                }
            }

            return list;
        }
        /// <summary>
        /// 获取所有泛型对象的子类
        /// </summary>
        /// <param name="genericsType"></param>
        /// <returns></returns>
        public static IList<Type> FindSubClassesGenerics(Type genericsType)
        {
            if (genericsType.IsGenericType == false)
            {
                return null;
            }
            var list = new List<Type>();
            foreach (Assembly a in AppDomain.CurrentDomain.GetAssemblies())
            {
                try
                {
                    foreach (Type t in a.GetTypes())
                    {
                        if ((!t.IsAbstract) && (!t.IsInterface))
                        {
                            var types = t.GetInterfaces();
                            types.ToList().ForEach(r =>
                            {
                                if (r.Name == genericsType.Name)
                                {
                                    list.Add(t);
                                }
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("反射错误" + ex.Message);
                }
            }
            return list;
        }
        /// <summary>
        /// 从指定的程序集中获取泛型对象的子类
        /// </summary>
        /// <param name="genericsType">泛型类型</param>
        /// <param name="asm">程序集</param>
        /// <returns></returns>
        public static IList<Type> FindSubClassesGenerics(Type genericsType, Assembly asm)
        {
            if (genericsType.IsGenericType == false)
            {
                return null;
            }
            var list = new List<Type>();

            try
            {
                foreach (Type t in asm.GetTypes())
                {
                    if ((!t.IsAbstract) && (!t.IsInterface))
                    {
                        var types = t.GetInterfaces();
                        types.ToList().ForEach(r =>
                        {
                            if (r.Name == genericsType.Name)
                            {
                                list.Add(t);
                            }
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("反射错误" + ex.Message);
            }

            return list;
        }
        /// <summary>
        /// 从所有运行目录下获取所有泛型对象的子类
        /// </summary>
        /// <param name="genericsType"></param>
        /// <returns></returns>
        public static IList<Type> FindSubClassesGenericsFormFile(Type genericsType)
        {
            if (genericsType.IsGenericType == false)
            {
                return null;
            }

            var path = AppDomain.CurrentDomain.BaseDirectory;

            var files = GetFiles(path);

            var list = new List<Type>();

            foreach (string fileName in files)
            {
                Assembly asm = null;

                try
                {
                    asm = Assembly.LoadFrom(fileName);

                    try
                    {
                        foreach (Type t in asm.GetTypes())
                        {
                            if ((!t.IsAbstract) && (!t.IsInterface))
                            {
                                var types = t.GetInterfaces();
                                types.ToList().ForEach(r =>
                                {
                                    if (r.Name == genericsType.Name)
                                    {
                                        list.Add(t);
                                    }
                                });
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine("反射错误" + ex.Message);
                    }
                }
                catch (Exception)
                {
                    continue;
                }
            }


            return list;
        }
        /// <summary>
        /// LoadDerivedType 加载directorySearched目录下所有程序集中的所有派生自baseType的类型
        /// </summary>
        /// <typeparam name="baseType">基类（或接口）类型</typeparam>
        /// <param name="directorySearched">搜索的目录</param>
        /// <param name="searchChildFolder">是否搜索子目录中的程序集</param>
        /// <param name="config">高级配置，可以传入null采用默认配置</param>
        /// <returns>所有从BaseType派生的类型列表</returns>
        public static IList<Type> FindTypeFromDirectory(Type baseType, string directorySearched, bool searchChildFolder, TypeLoadConfig config)
        {
            if (config == null)
            {
                config = new TypeLoadConfig();
            }

            IList<Type> derivedTypeList = new List<Type>();
            if (searchChildFolder)
            {
                ReflectionUtil.LoadDerivedTypeInAllFolder(baseType, derivedTypeList, directorySearched, config);
            }
            else
            {
                ReflectionUtil.LoadDerivedTypeInOneFolder(baseType, derivedTypeList, directorySearched, config);
            }

            return derivedTypeList;
        }
        /// <summary>
        /// GetAllMethods 获取接口的所有方法信息，包括继承的
        /// </summary>
        public static IList<MethodInfo> GetAllMethods(params Type[] interfaceTypes)
        {
            foreach (Type interfaceType in interfaceTypes)
            {
                if (!interfaceType.IsInterface)
                {
                    throw new Exception("Target Type must be interface!");
                }
            }

            IList<MethodInfo> list = new List<MethodInfo>();
            foreach (Type interfaceType in interfaceTypes)
            {
                ReflectionUtil.DistillMethods(interfaceType, ref list);
            }

            return list;
        }
        /// <summary>
        /// 取得目标对象的指定field的值，field可以是private
        /// </summary>
        public static object GetFieldValue(object obj, string fieldName)
        {
            var t = obj.GetType();
            var field = t.GetField(fieldName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.Instance);
            if (field == null)
            {
                var msg = string.Format("The field named '{0}' not found in '{1}'.", fieldName, t);
                throw new Exception(msg);
            }

            return field.GetValue(obj);
        }
        /// <summary>
        /// GetProperty 根据指定的属性名获取目标对象该属性的值
        /// </summary>
        public static object GetProperty(object obj, string propertyName)
        {
            var t = obj.GetType();

            return t.InvokeMember(propertyName, BindingFlags.Default | BindingFlags.GetProperty, null, obj, null);
        }
        /// <summary>
        /// GetType  通过完全限定的类型名来加载对应的类型。typeAndAssName如"ESBasic.Filters.SourceFilter,ESBasic"。
        /// 如果为系统简单类型，则可以不带程序集名称。
        /// </summary>
        public static Type GetType(string typeName)
        {
            try
            {
                var t = Type.GetType(typeName);
                if (t != null)
                {
                    return t;
                }
            }
            catch
            {
            }
            var names = typeName.Split(',');
            if (names.Length < 2)
            {
                return GetTypeEx(typeName);
            }

            return ReflectionUtil.GetType(names[0].Trim(), names[1].Trim());
        }
        /// <summary>
        /// 加载assemblyName程序集中的名为typeFullName的类型。assemblyName不用带扩展名，如果目标类型在当前程序集中，assemblyName传入null	
        /// </summary>		
        public static Type GetType(string typeFullName, string assemblyName)
        {
            if (assemblyName == null)
            {
                return Type.GetType(typeFullName);
            }


            var asses = AppDomain.CurrentDomain.GetAssemblies();
            foreach (Assembly ass in asses)
            {
                var names = ass.FullName.Split(',');
                if (names[0].Trim() == assemblyName.Trim())
                {
                    return ass.GetType(typeFullName);
                }
            }


            var tarAssem = Assembly.Load(assemblyName);
            if (tarAssem != null)
            {
                return tarAssem.GetType(typeFullName);
            }

            return null;
        }
        /// <summary>
        /// 根据类型名称获取对应的类型，不需要带程序集名称。如ESBasic.Filters.SourceFilter
        /// </summary>
        /// <param name="typeName"></param>
        /// <returns></returns>
        public static Type GetTypeEx(string typeName)
        {
            var arys = typeName.Split('.');
            Type svcType = null;
            for (var i = arys.Length - 1; i > 1; i--)
            {
                var assmb = string.Empty;
                for (var j = 0; j < i; j++)
                {
                    if (j < i - 1)
                    {
                        assmb += arys[j] + ".";
                    }
                    else
                    {
                        assmb += arys[j];
                    }
                }
                svcType = Type.GetType(typeName + "," + assmb);
                if (svcType != null)
                {
                    break;
                }
            }
            return svcType;
        }
        /// <summary>
        /// 获取指定类型的所有字段属性，包括继承的字段，但不包括object一级的字段。且去掉重复值
        /// </summary>
        /// <param name="t0"></param>
        /// <returns></returns>
        public static List<FieldInfo> GetTypeFields(Type type)
        {
            var t0 = typeof(object);
            var list = new List<FieldInfo>();
            while (type != t0)
            {
                list.AddRange(type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic));
                type = type.BaseType;
            }




            var fieldList = new List<FieldInfo>();
            foreach (FieldInfo fld in list)
            {
                if (fld.IsPublic == true)
                {
                    if (fieldList.Find(r => r.Name == fld.Name) == null)
                    {
                        fieldList.Add(fld);
                    }
                }
                else
                {
                    fieldList.Add(fld);
                }
            }
            return fieldList;
        }
        /// <summary>
        /// 获取指定类型的名称，包括所属程序集。
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static string GetTypeFullName(Type t)
        {
            return t.FullName + "," + t.Assembly.FullName.Split(',')[0];
        }
        /// <summary>
        /// SearchGenericMethodInType 搜索指定类型定义的泛型方法，不包括继承的。
        /// </summary>
        public static MethodInfo SearchGenericMethodInType(Type originType, string methodName, Type[] argTypes)
        {
            foreach (MethodInfo method in originType.GetMethods())
            {
                if (method.ContainsGenericParameters && method.Name == methodName)
                {
                    var succeed = true;
                    var paras = method.GetParameters();
                    if (paras.Length == argTypes.Length)
                    {
                        for (var i = 0; i < paras.Length; i++)
                        {
                            if (!paras[i].ParameterType.IsGenericParameter)
                            {
                                if (paras[i].ParameterType.IsGenericType)
                                {
                                    if (paras[i].ParameterType.GetGenericTypeDefinition() != argTypes[i].GetGenericTypeDefinition())
                                    {
                                        succeed = false;
                                        break;
                                    }
                                }
                                else
                                {
                                    if (paras[i].ParameterType != argTypes[i])
                                    {
                                        succeed = false;
                                        break;
                                    }
                                }
                            }
                        }
                        if (succeed)
                        {
                            return method;
                        }
                    }
                }
            }

            return null;
        }
        /// <summary>
        /// SearchMethod 搜索指定类型的指定的方法。 包括被继承的所有方法，也包括泛型方法。
        /// </summary>
        public static MethodInfo SearchMethod(Type originType, string methodName, Type[] argTypes)
        {
            var meth = originType.GetMethod(methodName, argTypes);
            if (meth != null)
            {
                return meth;
            }

            meth = ReflectionUtil.SearchGenericMethodInType(originType, methodName, argTypes);
            if (meth != null)
            {
                return meth;
            }


            var baseType = originType.BaseType;
            if (baseType != null)
            {
                while (baseType != typeof(object))
                {
                    var target = baseType.GetMethod(methodName, argTypes);
                    if (target != null)
                    {
                        return target;
                    }

                    target = ReflectionUtil.SearchGenericMethodInType(baseType, methodName, argTypes);
                    if (target != null)
                    {
                        return target;
                    }

                    baseType = baseType.BaseType;
                }
            }


            if (originType.GetInterfaces() != null)
            {
                var list = ReflectionUtil.GetAllMethods(originType.GetInterfaces());
                foreach (MethodInfo theMethod in list)
                {
                    if (theMethod.Name != methodName)
                    {
                        continue;
                    }
                    var args = theMethod.GetParameters();
                    if (args.Length != argTypes.Length)
                    {
                        continue;
                    }

                    var correctArgType = true;
                    for (var i = 0; i < args.Length; i++)
                    {
                        if (args[i].ParameterType != argTypes[i])
                        {
                            correctArgType = false;
                            break;
                        }
                    }

                    if (correctArgType)
                    {
                        return theMethod;
                    }
                }
            }

            return null;
        }
        /// <summary>
        /// 设置目标对象的指定field的值，field可以是private
        /// </summary>
        public static void SetFieldValue(object obj, string fieldName, object val)
        {
            var t = obj.GetType();
            var field = t.GetField(fieldName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.SetField | BindingFlags.Instance);
            if (field == null)
            {
                var msg = string.Format("The field named '{0}' not found in '{1}'.", fieldName, t);
                throw new Exception(msg);
            }

            field.SetValue(obj, val);
        }

        
        public class TypeLoadConfig
        {
            
            private bool copyToMemory = false;
            private bool loadAbstractType = false;
            private string targetFilePostfix = ".dll";

            // public constructors...
            public TypeLoadConfig()
            {
            }
            public TypeLoadConfig(bool copyToMem, bool loadAbstract, string postfix)
            {
                this.copyToMemory = copyToMem;
                this.loadAbstractType = loadAbstract;
                this.targetFilePostfix = postfix;
            }

            
            /// <summary>
            /// CopyToMem 是否将程序集拷贝到内存后加载
            /// </summary>
            public bool CopyToMemory
            {
                get
                {
                    return copyToMemory;
                }
                set
                {
                    copyToMemory = value;
                }
            }
            /// <summary>
            /// LoadAbstractType 是否加载抽象类型
            /// </summary>
            public bool LoadAbstractType
            {
                get
                {
                    return loadAbstractType;
                }
                set
                {
                    loadAbstractType = value;
                }
            }
            /// <summary>
            /// TargetFilePostfix 搜索的目标程序集的后缀名
            /// </summary>
            public string TargetFilePostfix
            {
                get
                {
                    return targetFilePostfix;
                }
                set
                {
                    targetFilePostfix = value;
                }
            }
        }
    }
}
