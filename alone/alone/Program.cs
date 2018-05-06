using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace alone
{
    internal enum Option
    {
        PART,
        PATH,
        OIL,
        RIFLE,
        HEALTH,
        REVOLVER,
        THOMPSON,
        SHOTGUN,
        DERRINGER,
        GRACE,
        RIOTGUN,
    }

    // ReSharper disable once ClassNeverInstantiated.Global
    internal class Program
    {
        public static void Main(string[] args)
        {
            Dictionary<Option, string> options = new Dictionary<Option, string>();
            foreach (string arg in args)
            {
                foreach (Option option in Enum.GetValues(typeof(Option)))
                {
                    string prefix = option.ToString().ToLower() + "=";
                    if (arg.StartsWith(prefix))
                    {
                        options[option] = arg.Substring(prefix.Length);
                    }
                }
            }

            string part;
            if (!options.TryGetValue(Option.PART, out part))
            {
                Console.WriteLine("Cannot get game part. Please, specify one with 'part=' option.");
                return;
            }

            switch (part)
            {
                case "1":
                    SaveGameEditor1 editor1 = new SaveGameEditor1(options);
                    editor1.ApplyChanges();
                    break;

                case "2":
                    SaveGameEditor2 editor2 = new SaveGameEditor2(options);
                    editor2.ApplyChanges();
                    break;

                default:
                    Console.WriteLine("Unexpected game part specified: '{0}'\n" +
                                      "Expected part values are [1,2]", part);
                    break;
            }
        }
    }
}
