//-----------------------------------------------------------------------
// <copyright file="WaveHdr.cs" company="Lifeprojects.de">
//     Class: WaveHdr
//     Copyright © Lifeprojects.de 2022
// </copyright>
//
// <author>Gerhard Ahrens - Lifeprojects.de</author>
// <email>gerhard.ahrens@lifeprojects.de</email>
// <date>06.01.2022</date>
//
// <summary>
// Struct Klasse für WaveHeader
// </summary>
//-----------------------------------------------------------------------


namespace TonGeneratorLibrary.Core
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct WaveHeader
    {
        public IntPtr lpData; // pointer to locked data buffer
        public int dwBufferLength; // length of data buffer
        public int dwBytesRecorded; // used for input only
        public IntPtr dwUser; // for client's use
        public int dwFlags; // assorted flags (see defines)
        public int dwLoops; // loop control counter
        public IntPtr lpNext; // PWaveHdr, reserved for driver
        public int reserved; // reserved for driver
    }
}
