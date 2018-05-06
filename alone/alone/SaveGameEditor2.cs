using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace alone
{
    internal class SaveGameEditor2 : SaveGameEditor
    {
        private const short XOR_VALUE = 0x3535;

        public SaveGameEditor2([NotNull] Dictionary<Option, string> options) : base(options)
        {
            offsets = new []
            {
                new KeyValuePair<Option, long>(Option.HEALTH, 0x68C4),
                new KeyValuePair<Option, long>(Option.REVOLVER, 0x68C6),
                new KeyValuePair<Option, long>(Option.THOMPSON, 0x68B4),
                new KeyValuePair<Option, long>(Option.RIOTGUN, 0x6C0E),
                new KeyValuePair<Option, long>(Option.SHOTGUN, 0x6C1E),
                new KeyValuePair<Option, long>(Option.DERRINGER, 0x68FC),
                new KeyValuePair<Option, long>(Option.GRACE, 0x694E),
            };
        }

        private bool TryToParseShortOption(out short value, Option option)
        {
            value = 0;
            string optionValue;
            if (!options.TryGetValue(option, out optionValue))
            {
                return false;
            }

            try
            {
                value = short.Parse(optionValue);
            }
            catch (Exception e)
            {
               Console.WriteLine(e);
               Console.WriteLine("Failed to parse short value from '{0}'", optionValue);
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

                short oldValue = (short)(bytes[offset+1] << 8 | bytes[offset]);
                oldValue ^= XOR_VALUE;

                Console.WriteLine("Old value {0}={1}", option.ToString().ToLower(), oldValue);

                short newValue;
                if (TryToParseShortOption(out newValue, option) && newValue != oldValue)
                {
                    isDirty = true;
                    Console.WriteLine("New value {0}={1}", option.ToString().ToLower(), newValue);

                    newValue ^= XOR_VALUE;
                    bytes[offset] = (byte)(newValue & 0xFF);
                    bytes[offset+1] = (byte) (newValue >> 8 & 0xFF);
                }
            }

            return isDirty;
        }
    }
}
