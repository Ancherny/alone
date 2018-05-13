using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace alone
{
    internal class SaveGameEditor3 : SaveGameEditor
    {
        private const short XOR_VALUE = 0x0;

        public SaveGameEditor3([NotNull] Dictionary<Option, string> options) : base(options)
        {
            offsets = new []
            {
                new KeyValuePair<Option, long>(Option.HEALTH, 0xAE64),
                new KeyValuePair<Option, long>(Option.REVOLVER, 0xB0CE),
                new KeyValuePair<Option, long>(Option.WINCHESTER, 0xB0C6),
                new KeyValuePair<Option, long>(Option.GATLING, 0xB1C0),
                new KeyValuePair<Option, long>(Option.SHOTGUN, 0xB16A),
                new KeyValuePair<Option, long>(Option.HEALTH2, 0xAE7A),
                new KeyValuePair<Option, long>(Option.COLT, 0xB1EA),
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
