//-----------------------------------------------------------------------
// <copyright file="ConsoleMenuSmall.cs" company="Lifeprojects.de">
//     Class: ConsoleMenuSmall
//     Copyright © Lifeprojects.de 2021
// </copyright>
//
// <author>Gerhard Ahrens - Lifeprojects.de</author>
// <email>gerhard.ahrens@lifeprojects.de</email>
// <date>27.12.2021</date>
//
// <summary>
// Konsolenmenü zum aufrufen von Methoden
// </summary>
//-----------------------------------------------------------------------

namespace ValueTypeLibrary.Console
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public sealed class ConsoleMenuSmall
    {
        public static void Run(List<MenueOption> options, MenueOption selectedOption)
        {
            int index = 0;
            WriteMenu(options, options[index]);
            ConsoleKeyInfo keyinfo;
            do
            {
                keyinfo = Console.ReadKey();
                if (keyinfo.Key == ConsoleKey.DownArrow)
                {
                    if (index + 1 < options.Count)
                    {
                        index++;
                        ConsoleMenuSmall.WriteMenu(options, options[index]);
                    }
                }
                else if (keyinfo.Key == ConsoleKey.UpArrow)
                {
                    if (index - 1 >= 0)
                    {
                        index--;
                        ConsoleMenuSmall.WriteMenu(options, options[index]);
                    }
                }
                else if (keyinfo.Key == ConsoleKey.Enter)
                {
                    options[index].Selected.Invoke();
                    index = 0;
                }
                else
                {
                    if (options.Any(f => f.Hotkey == keyinfo.Key) == true)
                    {
                        MenueOption menuPoint = options.First(f => f.Hotkey == keyinfo.Key);
                        menuPoint.Selected.Invoke();
                        index = 0;
                    }
                    else
                    {
                        index = 0;
                        ConsoleMenuSmall.WriteMenu(options, options[index]);
                    }
                }
            }
            while (keyinfo.Key != ConsoleKey.X);
        }

        public static void WriteMenu(List<MenueOption> options, MenueOption selectedOption)
        {
            Console.Clear();

            foreach (MenueOption option in options)
            {
                if (option == selectedOption)
                {
                    Console.Write("> ");
                }
                else
                {
                    Console.Write("  ");
                }

                Console.WriteLine(option.Name);
            }
        }
    }

    public sealed class MenueOption
    {
        public MenueOption(string name, Action selected)
        {
            Name = name;
            Selected = selected;
            Hotkey = ConsoleKey.NoName;
        }

        public MenueOption(string name, Action selected, ConsoleKey hotkey)
        {
            Name = name;
            Selected = selected;
            Hotkey = hotkey;
        }

        public string Name { get; }

        public Action Selected { get; }

        public ConsoleKey Hotkey { get; }

    }
}
