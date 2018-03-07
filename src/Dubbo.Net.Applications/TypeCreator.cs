using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Dubbo.Net.Common.Attributes;
using Dubbo.Net.Common.Utils;
using Microsoft.CSharp;

namespace Dubbo.Net.Applications
{
    public static class TypeCreator
    {
        public static Type ImplType(Type interfaceType)
        {
            //创建编译器实例。   
            var provider = new CSharpCodeProvider();
            //设置编译参数。   
            var cp = new CompilerParameters();
            cp.GenerateExecutable = false;
            cp.GenerateInMemory = true;

            // Generate an executable instead of 
            // a class library.
            //cp.GenerateExecutable = true;

            // Set the assembly file name to generate.
            //cp.OutputAssembly = "c:\\1.dll";

            // Generate debug information.
            cp.IncludeDebugInformation = true;


            // Save the assembly as a physical file.
            cp.GenerateInMemory = false;

            // Set the level at which the compiler 
            // should start displaying warnings.
            cp.WarningLevel = 3;

            // Set whether to treat all warnings as errors.
            cp.TreatWarningsAsErrors = false;

            // Set compiler argument to optimize output.
            cp.CompilerOptions = "/optimize";

            //cp.ReferencedAssemblies.Add("System.dll");
            //cp.ReferencedAssemblies.Add("System.Core.dll");
            cp.ReferencedAssemblies.Add("Dubbo.Net.Common.dll");
            //cp.ReferencedAssemblies.Add("System.Data.DataSetExtensions.dll");
            cp.ReferencedAssemblies.Add("Dubbo.Net.Applications.dll");
            foreach (var assembly1 in AppDomain.CurrentDomain.GetAssemblies())
            {
                cp.ReferencedAssemblies.Add(assembly1.Location);
            }

            //cp.ReferencedAssemblies.Add("Dubbo.Net.Test.exe");
            //cp.ReferencedAssemblies.Add("System.Drawing.dll");
            //cp.ReferencedAssemblies.Add("System.Windows.Forms.dll");


            //创建动态代码。   
            var interfaceName = interfaceType.Name;
            var methods = interfaceType.GetMethods();
            var className = (interfaceName.StartsWith("I") ? interfaceName.Substring(1) : interfaceName) + "Proxy";
            StringBuilder classSource = new StringBuilder();
            classSource.Append(
                $"using System.Threading.Tasks;\nusing Dubbo.Net.Common;\nusing Dubbo.Net.Common.Attributes;\nusing Dubbo.Net.Common.Utils;\nusing Dubbo.Net.Applications;\n ");
            classSource.Append("\nnamespace Dubbo.Net.ClientProxys{");
            classSource.Append("\npublic   class   " + className + ":ServiceProxyBase, " + interfaceType.FullName + " \n");
            classSource.Append("{\n");
            classSource.Append("public " + className + "(URL url):base(url)\n{}\n");
            foreach (var method in methods)
            {
                var type = method.ReturnType;
                var genericName = "";
                if (type.IsGenericType)
                {
                    genericName = type.GenericTypeArguments[0].FullName;
                }

                var attr = method.GetCustomAttribute<ReferAttribute>();
                if (attr != null)
                    classSource.Append("[Refer(\"" + attr.Name + "\")]\n");
                classSource.Append("public Task<" + genericName + "> " + method.Name + "(");
                var parameters = method.GetParameters();
                var args = new List<string>();
                var i = 0;
                foreach (var parameterInfo in parameters)
                {
                    var split = i == parameters.Length - 1 ? "" : ",";
                    classSource.Append(parameterInfo.ParameterType.FullName + " pram" + i + split);
                    args.Add("pram" + i);
                    i++;
                }

                classSource.Append("){\nvar method=this.GetType().GetMethod(\"" + method.Name + "\");\n");
                classSource.Append("var args=new object[]{");
                foreach (var arg in args)
                {
                    classSource.Append(arg + ",");
                }

                classSource.Append("};\n");
                classSource.Append("return base.Invoke<" + genericName + ">(method, args);\n}\n");
            }

            classSource.Append("}\n}");

            var clazz = classSource.ToString();

            //编译代码。   
            CompilerResults result = provider.CompileAssemblyFromSource(cp, clazz);
            if (result.Errors.Count > 0)
            {
                for (int i = 0; i < result.Errors.Count; i++)
                    Console.WriteLine(result.Errors[i]);
                Console.WriteLine("error");
                return null;
            }

            //获取编译后的程序集。   
            var assembly = result.CompiledAssembly;

            return assembly.GetType("Dubbo.Net.ClientProxys." + className);
        }

    }
}
