
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Text;
#if NET462
using Microsoft.CSharp;
using System.CodeDom.Compiler;
#else
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.Extensions.DependencyModel;
#endif

using System.Threading.Tasks;
using Dubbo.Net.Common;
using Dubbo.Net.Common.Attributes;
using Dubbo.Net.Common.Utils;

namespace Dubbo.Net.Proxy
{
    [DependencyIoc(typeof(IServiceProxyGenerater))]
    public class ServiceProxyGenerater : IServiceProxyGenerater
    {


        /// <summary>
        /// 生成服务代理。
        /// </summary>
        /// <param name="interfaceType">需要被代理的接口类型。</param>
        /// <returns>服务代理实现。</returns>
        public Type GenerateProxys(Type interfaceType)
        {
            var clazz = GetTreeText(interfaceType);
#if !NET462
            var assemblys = DependencyContext.Default.RuntimeLibraries.SelectMany(i => i.GetDefaultAssemblyNames(DependencyContext.Default).Select(z => Assembly.Load(new AssemblyName(z.Name))));

            assemblys = assemblys.Where(i => i.IsDynamic == false).ToArray();
            assemblys = assemblys.Append(interfaceType.Assembly);
            SyntaxTree tree = CSharpSyntaxTree.ParseText(clazz);
            var references = new[]
            {
                MetadataReference.CreateFromFile(typeof(Task).GetTypeInfo().Assembly.Location),
                MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(URL).Assembly.Location),
            };
            references = references.Concat(assemblys.Select(c => MetadataReference.CreateFromFile(c.Location)))
                .ToArray();
            var compilation = CSharpCompilation.Create("MyCompilation",
                 new[] { tree }, references, new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
            var stream = new MemoryStream();
            var result = compilation.Emit(stream);
            if (!result.Success)
            {

                return null;
            }
            stream.Seek(0, SeekOrigin.Begin);
            using (stream)
            {
                var assembly = AssemblyLoadContext.Default.LoadFromStream(stream);
                return assembly.GetExportedTypes().FirstOrDefault();
            }
#else

            var interfaceName = interfaceType.Name;
            var className = (interfaceName.StartsWith("I") ? interfaceName.Substring(1) : interfaceName) + "Proxy";
            return GenerateType(clazz, className);
#endif
        }


        private string GetTreeText(Type interfaceType)
        {
            var interfaceName = interfaceType.Name;
            var methods = interfaceType.GetMethods();
            var className = (interfaceName.StartsWith("I") ? interfaceName.Substring(1) : interfaceName) + "Proxy";
            StringBuilder classSource = new StringBuilder();
            classSource.Append(
                $"using System.Threading.Tasks;\nusing Dubbo.Net.Common;\nusing Dubbo.Net.Common.Utils;\nusing Dubbo.Net.Proxy;\nusing Dubbo.Net.Common.Attributes; ");
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
            return clazz;
        }
#if NET462
        private Type GenerateType(string clazz,string className){
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
#else

#endif
        ///// <summary>
        ///// 生成服务代理代码树。
        ///// </summary>
        ///// <param name="interfaceType">需要被代理的接口类型。</param>
        ///// <returns>代码树。</returns>
        //public SyntaxTree GenerateProxyTree(Type interfaceType)
        //{
        //    var className = interfaceType.Name.StartsWith("I") ? interfaceType.Name.Substring(1) : interfaceType.Name;
        //    className += "ClientProxy";

        //    var members = new List<MemberDeclarationSyntax>
        //    {
        //        GetConstructorDeclaration(className)
        //    };

        //    members.AddRange(GenerateMethodDeclarations(interfaceType.GetMethods()));
        //    return CompilationUnit()
        //        .WithUsings(GetUsings())
        //        .WithMembers(
        //            SingletonList<MemberDeclarationSyntax>(
        //                NamespaceDeclaration(
        //                    QualifiedName(
        //                        QualifiedName(
        //                            IdentifierName("Dubbo"),
        //                            IdentifierName("Net")),
        //                        IdentifierName("ClientProxys")))
        //        .WithMembers(
        //            SingletonList<MemberDeclarationSyntax>(
        //                ClassDeclaration(className)
        //                    .WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword)))
        //                    .WithBaseList(
        //                        BaseList(
        //                            SeparatedList<BaseTypeSyntax>(
        //                                new SyntaxNodeOrToken[]
        //                                {
        //                                    SimpleBaseType(IdentifierName("ServiceProxyBase")),
        //                                    Token(SyntaxKind.CommaToken),
        //                                    SimpleBaseType(GetQualifiedNameSyntax(interfaceType))
        //                                })))
        //                    .WithMembers(List(members))))))
        //        .NormalizeWhitespace().SyntaxTree;
        //}



        //private static QualifiedNameSyntax GetQualifiedNameSyntax(Type type)
        //{
        //    var fullName = type.Namespace + "." + type.Name;
        //    return GetQualifiedNameSyntax(fullName);
        //}

        //private static QualifiedNameSyntax GetQualifiedNameSyntax(string fullName)
        //{
        //    return GetQualifiedNameSyntax(fullName.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries));
        //}

        //private static QualifiedNameSyntax GetQualifiedNameSyntax(IReadOnlyCollection<string> names)
        //{
        //    var ids = names.Select(IdentifierName).ToArray();

        //    var index = 0;
        //    QualifiedNameSyntax left = null;
        //    while (index + 1 < names.Count)
        //    {
        //        left = left == null ? QualifiedName(ids[index], ids[index + 1]) : QualifiedName(left, ids[index + 1]);
        //        index++;
        //    }
        //    return left;
        //}

        //private static SyntaxList<UsingDirectiveSyntax> GetUsings()
        //{
        //    return List(
        //        new[]
        //        {
        //            UsingDirective(IdentifierName("System")),
        //            UsingDirective(GetQualifiedNameSyntax("System.Threading.Tasks")),
        //            UsingDirective(GetQualifiedNameSyntax("System.Collections.Generic")),
        //            UsingDirective(GetQualifiedNameSyntax(typeof(URL).Namespace)),
        //            UsingDirective(GetQualifiedNameSyntax(typeof(ServiceProxyBase).Namespace))
        //        });
        //}

        //private static ConstructorDeclarationSyntax GetConstructorDeclaration(string className)
        //{
        //    return ConstructorDeclaration(Identifier(className))
        //        .WithModifiers(
        //            TokenList(
        //                Token(SyntaxKind.PublicKeyword)))
        //        .WithParameterList(
        //            ParameterList(
        //                SeparatedList<ParameterSyntax>(
        //                    new SyntaxNodeOrToken[]
        //                    {
        //                        Parameter(
        //                            Identifier("url"))
        //                            .WithType(
        //                                IdentifierName("URL")),
        //                        //Token(SyntaxKind.CommaToken),
        //                        //Parameter(
        //                        //    Identifier("typeConvertibleService"))
        //                        //    .WithType(
        //                        //        IdentifierName("ITypeConvertibleService")),
        //                        //Token(SyntaxKind.CommaToken),
        //                        //Parameter(
        //                        //    Identifier("serviceKey"))
        //                        //    .WithType(
        //                        //        IdentifierName("String")),
        //                        // Token(SyntaxKind.CommaToken),
        //                        //Parameter(
        //                        //    Identifier("serviceProvider"))
        //                        //    .WithType(
        //                        //        IdentifierName("CPlatformContainer"))
        //                    })))
        //        .WithInitializer(
        //                ConstructorInitializer(
        //                    SyntaxKind.BaseConstructorInitializer,
        //                    ArgumentList(
        //                        SeparatedList<ArgumentSyntax>(
        //                            new SyntaxNodeOrToken[]{
        //                                Argument(
        //                                    IdentifierName("url")),
        //                                //Token(SyntaxKind.CommaToken),
        //                                //Argument(
        //                                //    IdentifierName("typeConvertibleService")),
        //                                //  Token(SyntaxKind.CommaToken),
        //                                //Argument(
        //                                //    IdentifierName("serviceKey")),
        //                                //   Token(SyntaxKind.CommaToken),
        //                                //Argument(
        //                                //    IdentifierName("serviceProvider"))
        //                            }))))
        //        .WithBody(Block());
        //}

        //private IEnumerable<MemberDeclarationSyntax> GenerateMethodDeclarations(IEnumerable<MethodInfo> methods)
        //{
        //    var array = methods.ToArray();
        //    return array.Select(p => GenerateMethodDeclaration(p)).ToArray();
        //}

        //private static TypeSyntax GetTypeSyntax(Type type)
        //{
        //    //没有返回值。
        //    if (type == null)
        //        return null;

        //    //非泛型。
        //    if (!type.GetTypeInfo().IsGenericType)
        //        return GetQualifiedNameSyntax(type.FullName);

        //    var list = new List<SyntaxNodeOrToken>();

        //    foreach (var genericTypeArgument in type.GenericTypeArguments)
        //    {
        //        list.Add(genericTypeArgument.GetTypeInfo().IsGenericType
        //            ? GetTypeSyntax(genericTypeArgument)
        //            : GetQualifiedNameSyntax(genericTypeArgument.FullName));
        //        list.Add(Token(SyntaxKind.CommaToken));
        //    }

        //    var array = list.Take(list.Count - 1).ToArray();
        //    var typeArgumentListSyntax = TypeArgumentList(SeparatedList<TypeSyntax>(array));
        //    return GenericName(type.Name.Substring(0, type.Name.IndexOf('`')))
        //        .WithTypeArgumentList(typeArgumentListSyntax);
        //}

        //private MemberDeclarationSyntax GenerateMethodDeclaration(MethodInfo method)
        //{
        //    var serviceId = "";//_serviceIdGenerator.GenerateServiceId(method);
        //    var returnDeclaration = GetTypeSyntax(method.ReturnType);

        //    var parameterList = new List<SyntaxNodeOrToken>();
        //    var parameterDeclarationList = new List<SyntaxNodeOrToken>();

        //    foreach (var parameter in method.GetParameters())
        //    {
        //        if (parameter.ParameterType.IsGenericType)
        //        {
        //            parameterDeclarationList.Add(Parameter(
        //                             Identifier(parameter.Name))
        //                             .WithType(GetTypeSyntax(parameter.ParameterType)));
        //        }
        //        else
        //        {
        //            parameterDeclarationList.Add(Parameter(
        //                                Identifier(parameter.Name))
        //                                .WithType(GetQualifiedNameSyntax(parameter.ParameterType)));

        //        }
        //        parameterDeclarationList.Add(Token(SyntaxKind.CommaToken));

        //        parameterList.Add(InitializerExpression(
        //            SyntaxKind.ComplexElementInitializerExpression,
        //            SeparatedList<ExpressionSyntax>(
        //                new SyntaxNodeOrToken[]{
        //                    LiteralExpression(
        //                        SyntaxKind.StringLiteralExpression,
        //                        Literal(parameter.Name)),
        //                    Token(SyntaxKind.CommaToken),
        //                    IdentifierName(parameter.Name)})));
        //        parameterList.Add(Token(SyntaxKind.CommaToken));
        //    }
        //    if (parameterList.Any())
        //    {
        //        parameterList.RemoveAt(parameterList.Count - 1);
        //        parameterDeclarationList.RemoveAt(parameterDeclarationList.Count - 1);
        //    }

        //    var declaration = MethodDeclaration(
        //        returnDeclaration,
        //        Identifier(method.Name))
        //        .WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.AsyncKeyword)))
        //        .WithParameterList(ParameterList(SeparatedList<ParameterSyntax>(parameterDeclarationList)));

        //    ExpressionSyntax expressionSyntax;
        //    StatementSyntax statementSyntax;

        //    if (method.ReturnType != typeof(Task))
        //    {
        //        expressionSyntax = GenericName(
        //        Identifier("Invoke")).WithTypeArgumentList(((GenericNameSyntax)returnDeclaration).TypeArgumentList);

        //    }
        //    else
        //    {
        //        expressionSyntax = IdentifierName("Invoke");
        //    }
        //    //expressionSyntax = AwaitExpression(
        //    //    InvocationExpression(expressionSyntax)
        //    //        .WithArgumentList(
        //    //            ArgumentList(
        //    //                SeparatedList<ArgumentSyntax>(
        //    //                    new SyntaxNodeOrToken[]
        //    //                    {
        //    //                            Argument(ObjectCreationExpression(
        //    //                                method))
        //    //                    }))));

        //    if (method.ReturnType != typeof(Task))
        //    {
        //        statementSyntax = ReturnStatement(expressionSyntax);
        //    }
        //    else
        //    {
        //        statementSyntax = ExpressionStatement(expressionSyntax);
        //    }

        //    declaration = declaration.WithBody(
        //                Block(
        //                    SingletonList(statementSyntax)));

        //    return declaration;
        //}
    }
}
