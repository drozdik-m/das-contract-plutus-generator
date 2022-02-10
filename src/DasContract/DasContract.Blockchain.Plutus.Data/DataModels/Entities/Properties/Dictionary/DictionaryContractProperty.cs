﻿using DasContract.Blockchain.Plutus.Data.DataModels.Entities.Properties.Primitive;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace DasContract.Blockchain.Plutus.Data.DataModels.Entities.Properties.Dictionary
{
    public class DictionaryContractProperty : ContractProperty
    {
        /// <summary>
        /// Key data type of this property
        /// </summary>
        public PrimitiveContractPropertyType KeyType { get; set; }

        /// <summary>
        /// Value data type of this property
        /// </summary>
        public ContractProperty Property { get; set; }
    }
}
