using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Xml.Schema;
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

        private const long OIL_OFFSET = 19856;
        private const long RIFLE_OFFSET = 19866;
        private const long HEALTH_OFFSET = 19882;
        private const long REVOLVER_OFFSET = 20024;

        private static readonly long[] offsets =
        {
            OIL_OFFSET,
            RIFLE_OFFSET,
            HEALTH_OFFSET,
            REVOLVER_OFFSET,
        };

        private static readonly long maxOffset;

        static Program()
        {
            maxOffset = offsets.Max();
        }

        private static Dictionary<Option, string> ParseArgs([NotNull] IEnumerable<string> args)
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

            return options;
        }
        
        public static void Main(string[] args)
        {
            Dictionary<Option, string> options = ParseArgs(args);
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

            byte[] bytes = null;
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

            byte oil = bytes[OIL_OFFSET];
            byte rifle = bytes[RIFLE_OFFSET];
            byte health = bytes[HEALTH_OFFSET];
            byte revolver = bytes[REVOLVER_OFFSET];

            Console.WriteLine("Oil: " + oil);
            Console.WriteLine("Rifle: " + rifle);
            Console.WriteLine("Health: " + health);
            Console.WriteLine("Revolver: " + revolver);
        }
    }
}
