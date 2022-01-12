//-----------------------------------------------------------------------
// <copyright file="WaveFormat.cs" company="Lifeprojects.de">
//     Class: WaveFormat
//     Copyright © Lifeprojects.de 2022
// </copyright>
//
// <author>Gerhard Ahrens - Lifeprojects.de</author>
// <email>gerhard.ahrens@lifeprojects.de</email>
// <date>06.01.2022</date>
//
// <summary>
// Struct Klasse für WaveFormat
// </summary>
//-----------------------------------------------------------------------


namespace TonGeneratorLibrary.Core
{
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public class WaveFormat
    {
        public short wFormatTag;
        public short nChannels;
        public int nSamplesPerSec;
        public int nAvgBytesPerSec;
        public short nBlockAlign;
        public short wBitsPerSample;
        public short cbSize;

    }
}
