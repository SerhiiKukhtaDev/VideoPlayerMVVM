using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace VideoPlayer.Utils
{
    public class NaturalComparer : IComparer<string>
    {
        /// <summary>
        /// Вызов WinApi-функции для натурального сравнения строк
        /// </summary>
        [DllImport("shlwapi.dll", CharSet = CharSet.Unicode)]
        private static extern int StrCmpLogicalW(string psz1, string psz2);

        /// <summary>
        /// Натуральное сравнение строк
        /// </summary>
        /// <param name="x">Первая строка</param>
        /// <param name="y">Вторая строка</param>
        /// <returns>Сравнивает две строки, возвращая -1, 0 или 1</returns>
        public static int Compare(string x, string y)
        {
            return StrCmpLogicalW(x, y);
        }

        /// <summary>
        /// Натуральное сравнение строк
        /// </summary>
        /// <param name="x">Первая строка</param>
        /// <param name="y">Вторая строка</param>
        /// <returns>Сравнивает две строки, возвращая -1, 0 или 1</returns>
        int IComparer<string>.Compare(string x, string y)
        {
            return StrCmpLogicalW(x, y);
        }
    }
}