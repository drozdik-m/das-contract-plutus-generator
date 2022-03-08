using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DasContract.Blockchain.Plutus.Utils
{
    /// <summary>
    /// Stable (non-random) generator for temporaty names
    /// </summary>
    public class StableVariableNameGenerator
    {
        string current = string.Empty;

        /// <summary>
        /// Returns the next name in queue
        /// </summary>
        /// <returns></returns>
        public string GetNext()
        {
            current = GetNextLetter();
            return current;
        }

        string GetNextLetter()
        {
            if (current == string.Empty) return "a";
            if (current.Last() == 'a') return CurrentWithLastAs('b');
            if (current.Last() == 'b') return CurrentWithLastAs('c');
            if (current.Last() == 'c') return CurrentWithLastAs('d');
            if (current.Last() == 'd') return CurrentWithLastAs('e');
            if (current.Last() == 'e') return CurrentWithLastAs('f');
            if (current.Last() == 'f') return CurrentWithLastAs('g');
            if (current.Last() == 'g') return CurrentWithLastAs('h');
            if (current.Last() == 'h') return CurrentWithLastAs('i');
            if (current.Last() == 'i') return CurrentWithLastAs('j');
            if (current.Last() == 'j') return CurrentWithLastAs('k');
            if (current.Last() == 'k') return CurrentWithLastAs('l');
            if (current.Last() == 'l') return CurrentWithLastAs('m');
            if (current.Last() == 'm') return CurrentWithLastAs('n');
            if (current.Last() == 'n') return CurrentWithLastAs('o');
            if (current.Last() == 'o') return CurrentWithLastAs('p');
            if (current.Last() == 'p') return CurrentWithLastAs('q');
            if (current.Last() == 'q') return CurrentWithLastAs('r');
            if (current.Last() == 'r') return CurrentWithLastAs('s');
            if (current.Last() == 's') return CurrentWithLastAs('t');
            if (current.Last() == 't') return CurrentWithLastAs('u');
            if (current.Last() == 'u') return CurrentWithLastAs('v');
            if (current.Last() == 'v') return CurrentWithLastAs('w');
            if (current.Last() == 'w') return CurrentWithLastAs('x');
            if (current.Last() == 'x') return CurrentWithLastAs('y');
            if (current.Last() == 'y') return CurrentWithLastAs('z');
            if (current.Last() == 'z') return current + "a";

            throw new Exception("Unknown trailing character");
        }

        string CurrentWithLastAs(char last)
        {
            return current.Remove(current.Length - 1) + last;
        }
    }
}
