using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace alone
{
    internal class SaveGameEditor3 : SaveGameEditor2
    {
        public SaveGameEditor3([NotNull] Dictionary<Option, string> options) : base(options)
        {
            offsets = new []
            {
                new KeyValuePair<Option, long>(Option.HEALTH, 0xAD64),
                new KeyValuePair<Option, long>(Option.HEALTH2, 0xAF9C),
                new KeyValuePair<Option, long>(Option.WINCHESTER, 0xAFC6),
                new KeyValuePair<Option, long>(Option.REVOLVER, 0xAFCE),
                new KeyValuePair<Option, long>(Option.GATLING, 0xB0C0),
            };
        }
    }
}
