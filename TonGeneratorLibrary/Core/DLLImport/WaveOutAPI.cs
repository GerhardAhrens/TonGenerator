//-----------------------------------------------------------------------
// <copyright file="WaveOutAPI.cs" company="Lifeprojects.de">
//     Class: WaveOutAPI
//     Copyright © Lifeprojects.de 2022
// </copyright>
//
// <author>Gerhard Ahrens - Lifeprojects.de</author>
// <email>gerhard.ahrens@lifeprojects.de</email>
// <date>11.01.2022 17:40:10</date>
//
// <summary>
// Klasse für 
// </summary>
//-----------------------------------------------------------------------

namespace TonGeneratorLibrary.Core.DLLImport
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Threading.Tasks;

    public class WaveOutAPI
    {
        [DllImport("winmm.dll")]
        private static extern int waveOutOpen(out IntPtr hWaveOut, int uDeviceID, WaveFormat lpFormat, WaveDelegate dwCallback, int dwInstance, int dwFlags);

        [DllImport("winmm.dll")]
        private static extern int waveOutPrepareHeader(IntPtr hWaveOut, ref WaveHeader lpWaveOutHdr, int uSize);

        [DllImport("winmm.dll")]
        private static extern int waveOutWrite(IntPtr hWaveOut, ref WaveHeader lpWaveOutHdr, int uSize);

        [DllImport("winmm.dll")]
        private static extern int waveOutUnprepareHeader(IntPtr hWaveOut, ref WaveHeader lpWaveOutHdr, int uSize);

        [DllImport("winmm.dll")]
        private static extern int waveOutClose(IntPtr hWaveOut);

        public delegate void WaveDelegate(IntPtr hdrvr, int uMsg, int dwUser, ref WaveHeader wavhdr, int dwParam2);

        static WaveOutAPI()
        {
            try
            {
            }
            catch (Exception ex)
            {
                string errText = ex.Message;
                throw;
            }
        }

        public static int WaveOutOpen(out IntPtr hWnd, int uDeviceID, WaveFormat lpFormat, WaveDelegate dwCallback, int dwInstance, int dwFlags)
        {
            int result = -1;

            try
            {
                result = waveOutOpen(out hWnd, uDeviceID, lpFormat, dwCallback, dwInstance, dwFlags);
            }
            catch (Exception ex)
            {
                string errText = ex.Message;
                throw;
            }

            return result;
        }

        public static int WaveOutPrepareHeader(IntPtr hWnd, ref WaveHeader lpWaveOutHdr, int uSize)
        {
            int result = -1;

            try
            {
                result = waveOutPrepareHeader(hWnd, ref lpWaveOutHdr, uSize);
            }
            catch (Exception ex)
            {
                string errText = ex.Message;
                throw;
            }

            return result;
        }

        public static int WaveOutWrite(IntPtr hWnd, ref WaveHeader lpWaveOutHdr, int uSize)
        {
            int result = -1;

            try
            {
                result = waveOutWrite(hWnd, ref lpWaveOutHdr, uSize);
            }
            catch (Exception ex)
            {
                string errText = ex.Message;
                throw;
            }

            return result;
        }

        public static int WaveOutUnprepareHeader(IntPtr hWnd, ref WaveHeader lpWaveOutHdr, int uSize)
        {
            int result = -1;

            try
            {
                result = waveOutUnprepareHeader(hWnd, ref lpWaveOutHdr, uSize);
            }
            catch (Exception ex)
            {
                string errText = ex.Message;
                throw;
            }

            return result;
        }

        public static int WaveOutClose(IntPtr hWnd)
        {
            int result = -1;

            try
            {
                result = waveOutClose(hWnd);
            }
            catch (Exception ex)
            {
                string errText = ex.Message;
                throw;
            }

            return result;
        }
    }
}
