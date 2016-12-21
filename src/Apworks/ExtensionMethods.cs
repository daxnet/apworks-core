using Apworks.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Apworks
{
    public static class ExtensionMethods
    {
        /// <summary>
        /// Pluralizes the specified word.
        /// </summary>
        /// <param name="word">The word to be pluralized.</param>
        /// <param name="inputIsKnownToBeSingular">True if the caller can ensure that the word
        /// passed in is in the singular form. Otherwise, false.</param>
        /// <returns>The pluralized word.</returns>
        /// <remarks>
        /// This code is from the Humanizer open source library: https://github.com/Humanizr/Humanizer.
        /// </remarks>
        public static string Pluralize(this string word, bool inputIsKnownToBeSingular = true)
        {
            return Vocabularies.Default.Pluralize(word, inputIsKnownToBeSingular);
        }
    }
}
