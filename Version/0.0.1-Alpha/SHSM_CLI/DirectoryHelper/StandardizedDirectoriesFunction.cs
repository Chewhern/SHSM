using ASodium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SHSM_CLI.DirectoryHelper
{
    public static class StandardizedDirectoriesFunction
    {
        public static String ServerRootFolder = "";
        public static String UsersRootFolder = "";
        public static String PKCRootFolder = "";
        public static String SecretKeyRootFolder = "";

        public static void CreateDirectoriesIfNotExist() 
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                ServerRootFolder = AppContext.BaseDirectory + "\\ServerIP\\";
                UsersRootFolder = AppContext.BaseDirectory + "\\Users\\";
                PKCRootFolder = AppContext.BaseDirectory + "\\PublicKeyCryptography\\";
                SecretKeyRootFolder = AppContext.BaseDirectory + "\\SecretKeyCryptography\\";
            }
            else
            {
                ServerRootFolder = AppContext.BaseDirectory + "/ServerIP/";
                UsersRootFolder = AppContext.BaseDirectory + "/Users/";
                PKCRootFolder = AppContext.BaseDirectory + "/PublicKeyCryptography/";
                SecretKeyRootFolder = AppContext.BaseDirectory + "/SecretKeyCryptography/";
            }
            if (Directory.Exists(ServerRootFolder) == false)
            {
                Directory.CreateDirectory(ServerRootFolder);
            }
            if (Directory.Exists(UsersRootFolder) == false)
            {
                Directory.CreateDirectory(UsersRootFolder);
            }
            if (Directory.Exists(PKCRootFolder) == false)
            {
                Directory.CreateDirectory(PKCRootFolder);
            }
            if (Directory.Exists(SecretKeyRootFolder) == false)
            {
                Directory.CreateDirectory(SecretKeyRootFolder);
            }
            SodiumInit.Init();
        }

        public static void InitializedDirectories()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                ServerRootFolder = AppContext.BaseDirectory + "\\ServerIP\\";
                UsersRootFolder = AppContext.BaseDirectory + "\\Users\\";
                PKCRootFolder = AppContext.BaseDirectory + "\\PublicKeyCryptography\\";
                SecretKeyRootFolder = AppContext.BaseDirectory + "\\SecretKeyCryptography\\";
            }
            else
            {
                ServerRootFolder = AppContext.BaseDirectory + "/ServerIP/";
                UsersRootFolder = AppContext.BaseDirectory + "/Users/";
                PKCRootFolder = AppContext.BaseDirectory + "/PublicKeyCryptography/";
                SecretKeyRootFolder = AppContext.BaseDirectory + "/SecretKeyCryptography/";
            }
            SodiumInit.Init();
        }
    }
}
