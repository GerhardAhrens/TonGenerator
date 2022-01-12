﻿//-----------------------------------------------------------------------
// <copyright file="DLLFunctionLoader.cs" company="Lifeprojects.de">
//     Class: DLLFunctionLoader
//     Copyright © Lifeprojects.de 2022
// </copyright>
//
// <author>Gerhard Ahrens - Lifeprojects.de</author>
// <email>gerhard.ahrens@lifeprojects.de</email>
// <date>11.01.2022</date>
//
// <summary>
// Klasse zum laden von Win API DLLs
// </summary>
//-----------------------------------------------------------------------


namespace System.Runtime.InteropServices
{
    using System;
    using System.IO;

    public class DLLFunctionLoader
    {
        [DllImport("Kernel32.dll")]
        private static extern IntPtr LoadLibrary(string path);

        [DllImport("Kernel32.dll")]
        private static extern IntPtr GetProcAddress(IntPtr hModule, string procName);

        [DllImport("kernel32.dll")]
        private static extern bool FreeLibrary(IntPtr hModule);

        public static T LoadFunction<T>(string pDllPath, string pFunctionName, out IntPtr functionAddress)
        {
            Delegate result = null;
            IntPtr hModule = System.IntPtr.Zero;

            try
            {
                bool isWindowsDefaultDLL = false;
                if (pDllPath.Contains("\\") == false)
                {
                    isWindowsDefaultDLL = true;
                }

                if (isWindowsDefaultDLL == true)
                {
                    hModule = LoadLibrary(pDllPath);
                    functionAddress = GetProcAddress(hModule, pFunctionName);
                    if (functionAddress.ToInt64() > 0)
                    {
                        result = Marshal.GetDelegateForFunctionPointer(functionAddress, typeof(T));
                    }
                    else
                    {
                        string expText = string.Format("Die in der DLL '{0}'\ngesuchte Methode '{1}' wurde nicht gefunden", pDllPath, pFunctionName);
                        throw new Exception(expText);
                    }
                }
                else
                {
                    if (File.Exists(pDllPath) == true)
                    {
                        hModule = LoadLibrary(pDllPath);
                        functionAddress = GetProcAddress(hModule, pFunctionName);
                        if (functionAddress.ToInt32() > 0)
                        {
                            result = Marshal.GetDelegateForFunctionPointer(functionAddress, typeof(T));
                        }
                        else
                        {
                            string expText = string.Format("Die in der DLL '{0}'\ngesuchte Methode '{1}' wurde nicht gefunden", pDllPath, pFunctionName);
                            throw new Exception(expText);
                        }
                    }
                    else
                    {
                        string expText = string.Format("Die DLL '{0}' wurde nicht gefunden", pDllPath);
                        throw new Exception(expText);
                    }
                }

            }
            catch (Exception ex)
            {
                string errText = ex.Message;
                throw;
            }
            finally
            {
                FreeLibrary(hModule);
            }

            return (T)(object)result;
        }
    }
}
