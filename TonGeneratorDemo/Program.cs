namespace TonGeneratorDemo
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using DelegateImportDLL.Core;

    using TonGeneratorLibrary.Core;

    using ValueTypeLibrary.Console;

    public class Program
    {
        private static Sound sound = new Sound();
        private static Dictionary<string, Tuple<int,bool>> tasten = new Dictionary<string, Tuple<int, bool>>();

        private static void Main(string[] args)
        {
            List<MenueOption> options;

            Tastenliste();
            sound.soundVolume = 80;
            sound.OpenDevice(44100);

            options = new List<MenueOption>();
            options.Add(new MenueOption("Sinus Ton [A]", () =>
            {
                SetTon(ConsoleKey.A);
                ConsoleMenuSmall.WriteMenu(options, options.First());
            }, ConsoleKey.A));

            options.Add(new MenueOption("Sinus Ton [B]", () =>
            {
                SetTon(ConsoleKey.B);
                ConsoleMenuSmall.WriteMenu(options, options.First());
            }, ConsoleKey.B));

            options.Add(new MenueOption("Sinus Ton [C]", () =>
            {
                SetTon(ConsoleKey.C);
                ConsoleMenuSmall.WriteMenu(options, options.First());
            }, ConsoleKey.C));

            options.Add(new MenueOption("Sinus Ton [D]", () =>
            {
                SetTon(ConsoleKey.D);
                ConsoleMenuSmall.WriteMenu(options, options.First());
            }, ConsoleKey.D));

            options.Add(new MenueOption("Sinus Ton [E]", () =>
            {
                SetTon(ConsoleKey.E);
                ConsoleMenuSmall.WriteMenu(options, options.First());
            }, ConsoleKey.E));

            options.Add(new MenueOption("Sinus Ton [F]", () =>
            {
                SetTon(ConsoleKey.F);
                ConsoleMenuSmall.WriteMenu(options, options.First());
            }, ConsoleKey.F));

            options.Add(new MenueOption("Sinus Ton [G]", () =>
            {
                SetTon(ConsoleKey.G);
                ConsoleMenuSmall.WriteMenu(options, options.First());
            }, ConsoleKey.G));

            options.Add(new MenueOption("Sinus Ton [Y]", () =>
            {
                SetTon(ConsoleKey.Y);
                ConsoleMenuSmall.WriteMenu(options, options.First());
            }, ConsoleKey.Y));

            options.Add(new MenueOption("Single Ton [Z]", () =>
            {
                sound.Play(3500, 1000, ushort.MaxValue);
                ConsoleMenuSmall.WriteMenu(options, options.First());
            }, ConsoleKey.Z));

            options.Add(new MenueOption("System Ton [1]", () =>
            {
                sound.Play(SystemSound.Critical);
                ConsoleMenuSmall.WriteMenu(options, options.First());
            }, ConsoleKey.D1));

            options.Add(new MenueOption("Test SystemMessageBox [2]", () =>
            {
                uint pMsgTyp = (uint)MsgOptions.MB_OK | (uint)MsgOptions.MB_ICONASTERISK;
                int result = SystemMessageBox.Show("MsgBox Text\nÜber ein Delegate aufgerufenen Win32 API Funktion", "Titel", pMsgTyp);


                ConsoleMenuSmall.WriteMenu(options, options.First());
            }, ConsoleKey.D2));

            ConsoleMenuSmall.Run(options, options[0]);

        }

        private static void Tastenliste()
        {
            tasten.Add("A", new Tuple<int,bool>(262,true));
            tasten.Add("B", new Tuple<int, bool>(277, false));
            tasten.Add("C", new Tuple<int, bool>(294, true));
            tasten.Add("D", new Tuple<int, bool>(311, false));
            tasten.Add("E", new Tuple<int, bool>(330, true));
            tasten.Add("F", new Tuple<int, bool>(349, true));
            tasten.Add("G", new Tuple<int, bool>(370, false));
            tasten.Add("H", new Tuple<int, bool>(392, true));
            tasten.Add("I", new Tuple<int, bool>(415, false));
            tasten.Add("J", new Tuple<int, bool>(440, true));
            tasten.Add("K", new Tuple<int, bool>(466, false));
            tasten.Add("L", new Tuple<int, bool>(494, true));
            tasten.Add("M", new Tuple<int, bool>(523, true));
            tasten.Add("N", new Tuple<int, bool>(554, false));
            tasten.Add("O", new Tuple<int, bool>(587, true));
            tasten.Add("P", new Tuple<int, bool>(622, false));
            tasten.Add("Q", new Tuple<int, bool>(659, true));
            tasten.Add("R", new Tuple<int, bool>(698, true));
            tasten.Add("S", new Tuple<int, bool>(740, false));
            tasten.Add("T", new Tuple<int, bool>(784, true));
            tasten.Add("U", new Tuple<int, bool>(831, false));
            tasten.Add("V", new Tuple<int, bool>(880, true));
            tasten.Add("W", new Tuple<int, bool>(932, false));
            tasten.Add("X", new Tuple<int, bool>(988, true));
            tasten.Add("Y", new Tuple<int, bool>(1047, true));
        }
        private static void SetTon(ConsoleKey taste)
        {
            if (tasten.ContainsKey(taste.ToString()) == true)
            {
                Tuple<int, bool> t = tasten.Where(w => w.Key == taste.ToString()).First().Value;
                int keyFreq = t.Item1;

                if (t.Item2 == true)
                {
                    keyFreq = keyFreq - 5;
                }

                sound.Play(keyFreq, sound.soundVolume);
                sound.Dispose();
            }
        }
    }
}
