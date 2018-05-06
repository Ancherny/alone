using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using JetBrains.Annotations;

namespace alone
{
    internal abstract class SaveGameEditor
    {
        protected KeyValuePair<Option, long>[] offsets;

        protected readonly Dictionary<Option, string> options;

        protected SaveGameEditor([NotNull] Dictionary<Option, string> options)
        {
            this.options = options;
        }

        protected abstract bool ApplyChangesImpl([NotNull] byte[] bytes);

        public void ApplyChanges()
        {
            foreach (KeyValuePair<Option, string> pair in options)
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

            long maxOffset = offsets.Max(pair => pair.Value);
            if (maxOffset >= bytes.Length)
            {
                Console.WriteLine("Bad file size at: '{0}'", path);
                return;
            }

            bool isDirty = ApplyChangesImpl(bytes);

            if (!isDirty)
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
