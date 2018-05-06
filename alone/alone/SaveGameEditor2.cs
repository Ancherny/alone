using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace alone
{
    internal class SaveGameEditor2 : SaveGameEditor
    {
        public SaveGameEditor2([NotNull] Dictionary<Option, string> options) : base(options)
        {
            offsets = new []
            {
                new KeyValuePair<Option, long>(Option.OIL, 19856),
                new KeyValuePair<Option, long>(Option.RIFLE, 19866),
                new KeyValuePair<Option, long>(Option.HEALTH, 19882),
                new KeyValuePair<Option, long>(Option.REVOLVER, 20024),
            };
        }

        private bool TryToParseByteOption(out byte value, Option option)
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

        protected override bool ApplyChangesImpl(byte[] bytes)
        {
            bool isDirty = false;

            foreach (KeyValuePair<Option, long> pair in offsets)
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

            return isDirty;
        }
    }
}
