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

    using static TonGeneratorLibrary.Core.SoundStructure;

    public class WaveOutAPI
    {
        [DllImport("winmm.dll")]
        private static extern int waveOutOpen(out IntPtr hWaveOut, int uDeviceID, WaveFormat lpFormat, WaveDelegate dwCallback, int dwInstance, int dwFlags);

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
    }
}
