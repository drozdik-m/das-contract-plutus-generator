namespace DasContract.Blockchain.Plutus.Code
{
    public interface IPlutusLine : IStringable
    {
        /// <summary>
        /// Indentation level of this line of code
        /// </summary>
        public int Indent { get; }


    }

}
