﻿using System;
using System.Collections.Generic;
using System.Text;

namespace DasContract.Blockchain.Plutus.Data.Interfaces
{
    public interface INamable
    {
        /// <summary>
        /// Name of this entity
        /// </summary>
        string Name { get; }
    }
}
