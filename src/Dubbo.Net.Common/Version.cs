using System;

namespace Dubbo.Net.Common
{
    public sealed class Version
    {
        private static readonly string VERSION = GetVersion(typeof(Version), "2.0.0");
        private static readonly bool Internal = HasResource("com/alibaba/dubbo/registry/internal/RemoteRegistry.class");
        private static readonly bool Compatible = HasResource("com/taobao/remoting/impl/ConnectionRequest.class");

        static Version()
        {
            // check if there's duplicated jar
            Common.Version.CheckDuplicate(typeof(Common.Version));
        }

        private Version()
        {
        }

        public static string GetVersion()
        {
            return VERSION;
        }

        public static bool IsInternalVersion()
        {
            return Internal;
        }

        public static bool IsCompatibleVersion()
        {
            return Compatible;
        }

        private static bool HasResource(string path)
        {
            try
            {
                return false;
                //return Version.class.getClassLoader().getResource(path) != null;
            }
            catch (Exception t)
            {
                return false;
            }
        }

        public static string GetVersion(Type cls, string defaultVersion)
        {
            return "2.6.0";
            //try
            //{
            //    // find version info from MANIFEST.MF first
            //    string version = cls.Namespace.getImplementationVersion();
            //    if (version == null || version.Length == 0)
            //    {
            //        version = cls.Namespace.getSpecificationVersion();
            //    }
            //    if (version == null || version.Length == 0)
            //    {
            //        // guess version fro jar file name if nothing's found from MANIFEST.MF
            //        CodeSource codeSource = cls.getProtectionDomain().getCodeSource();
            //        if (codeSource == null)
            //        {
            //            logger.info("No codeSource for class " + cls.getName() + " when getVersion, use default version " + defaultVersion);
            //        }
            //        else
            //        {
            //            String file = codeSource.getLocation().getFile();
            //            if (file != null && file.length() > 0 && file.endsWith(".jar"))
            //            {
            //                file = file.substring(0, file.length() - 4);
            //                int i = file.lastIndexOf('/');
            //                if (i >= 0)
            //                {
            //                    file = file.substring(i + 1);
            //                }
            //                i = file.indexOf("-");
            //                if (i >= 0)
            //                {
            //                    file = file.substring(i + 1);
            //                }
            //                while (file.length() > 0 && !Character.isDigit(file.charAt(0)))
            //                {
            //                    i = file.indexOf("-");
            //                    if (i >= 0)
            //                    {
            //                        file = file.substring(i + 1);
            //                    }
            //                    else
            //                    {
            //                        break;
            //                    }
            //                }
            //                version = file;
            //            }
            //        }
            //    }
            //    // return default version if no version info is found
            //    return version == null || version.length() == 0 ? defaultVersion : version;
            //}
            //catch (Throwable e)
            //{
            //    // return default version when any exception is thrown
            //    logger.error("return default version, ignore exception " + e.getMessage(), e);
            //    return defaultVersion;
            //}
        }

        public static void CheckDuplicate(Type cls, bool failOnError)
        {
            CheckDuplicate(cls.FullName.Replace('.', '/') + ".class", failOnError);
        }

        public static void CheckDuplicate(Type cls)
        {
            CheckDuplicate(cls, false);
        }

        public static void CheckDuplicate(String path, bool failOnError)
        {
            //            try
            //            {
            //                // search in caller's classloader
            //                Enumeration<URL> urls = ClassHelper.getCallerClassLoader(Version.class).getResources(path);
            //        Set<String> files = new HashSet<String>();
            //            while (urls.hasMoreElements()) {
            //                URL url = urls.nextElement();
            //                if (url != null) {
            //                    String file = url.getFile();
            //                    if (file != null && file.length() > 0) {
            //                        files.add(file);
            //                    }
            //}
            //            }
            //            // duplicated jar is found
            //            if (files.size() > 1) {
            //                String error = "Duplicate class " + path + " in " + files.size() + " jar " + files;
            //                if (failOnError) {
            //                    throw new IllegalStateException(error);
            //                } else {
            //                    logger.error(error);
            //                }
            //            }
            //        } catch (Throwable e) {
            //            logger.error(e.getMessage(), e);
            //        }
        }
    }
}
