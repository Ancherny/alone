using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using JetBrains.Annotations;

namespace alone
{
    // ReSharper disable once ClassNeverInstantiated.Global
    internal class Program
    {
        private enum Option
        {
            PATH,
            OIL,
            RIFLE,
            HEALTH,
            REVOLVER
        }

        private static readonly KeyValuePair<Option, long>[] offsets =
        {
            new KeyValuePair<Option, long>(Option.OIL, 19856),
            new KeyValuePair<Option, long>(Option.RIFLE, 19866),
            new KeyValuePair<Option, long>(Option.HEALTH, 19882),
            new KeyValuePair<Option, long>(Option.REVOLVER, 20024),
        };

        private static readonly long maxOffset;

        private static readonly Dictionary<Option, string> options;

        static Program()
        {
            maxOffset = offsets.Max(pair => pair.Value);
            options = new Dictionary<Option, string>();
        }

        private static void ParseArgs([NotNull] IEnumerable<string> args)
        {
            options.Clear();
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
        }

        private static bool TryToParseByteOption(out byte value, Option option)
        {
            value = 0;
            string optionValue;
            if (!options.TryGetValue(option, out optionValue))
            {
                return false;
            }

            try
            {
                value = byte.Parse(optionValue);
            }
            catch (Exception e)
            {
               Console.WriteLine(e);
               Console.WriteLine("Failed to parse byte value from '{0}'", optionValue);
               return false;
            }
            return true;
        }

        public static void Main(string[] args)
        {
            ParseArgs(args);

            foreach (KeyValuePair<Option,string> pair in options)
            {
                Option option = pair.Key;
                string value = pair.Value;
                Console.WriteLine("{0} = {1}", option, value);
            }

            string path;
            if (!options.TryGetValue(Option.PATH, out path))
            {
                Console.WriteLine("Cannot get path to save file. Please, set it via '{0}=' option", Option.PATH);
                return;
            }

            if (!File.Exists(path))
            {
                Console.WriteLine("Save file does not exist at path: '{0}'", path);
                return;
            }

            byte[] bytes;
            try
            {
                bytes = File.ReadAllBytes(path);
            }
            catch (IOException e)
            {
                Console.WriteLine(e);
                Console.WriteLine("Failed to read file from: '{0}'", path);
                return;
            }

            if (maxOffset >= bytes.Length)
            {
                Console.WriteLine("Bad file size at: '{0}'", path);
                return;
            }

            bool isDirty = false;

            foreach (KeyValuePair<Option,long> pair in offsets)
            {
                Option option = pair.Key;
                long offset = pair.Value;

                byte oldValue = bytes[offset];
                Console.WriteLine("Old value {0}={1}", option.ToString().ToLower(), oldValue);

                byte newValue;
                if (TryToParseByteOption(out newValue, option) && newValue != oldValue)
                {
                    isDirty = true;
                    bytes[offset] = newValue;
                    Console.WriteLine("New value {0}={1}", option.ToString().ToLower(), newValue);
                }
            }

            if (isDirty)
            {
               return;
            }

            try
            {
                File.WriteAllBytes(path, bytes);
            }
            catch (IOException e)
            {
                Console.WriteLine(e);
                Console.WriteLine("Failed to write back cheated valued to '{0}'", path);
                return;
            }

            Console.WriteLine("Success!");
        }
    }
}
