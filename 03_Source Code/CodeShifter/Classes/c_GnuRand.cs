using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/****************************************************************         
    *                                                           *   
    * Author:  J.Anish Dev                                      *   
    *                                                           *   
    * About :  Solution to NSA's CodeBreaker Challenge 2016     *   
    *          I'd love to hear from you if: You just want to   * 
    *          say hi, if you found this useful, if you         *
    *          use my code, if you solved task 6, or if         * 
    *          you want to team up for 2017's challenge ;)      *    
    *                                                           *
    * Contact me:                                               *
    *          www.dreamersion.com                              *
    *          anishdev@ufl.edu                                 *   
    ************************************************************/

namespace CodeShift
{
    public class GnuRand
    {
        private uint[] r;
        private int n;

        public GnuRand(uint seed)
        {
            r = new uint[344];

            unchecked
            {
                r[0] = seed;
                for (int i = 1; i < 31; i++)
                {
                    r[i] = (uint)((16807 * (ulong)r[i - 1]) % 2147483647);
                }
                for (int i = 31; i < 34; i++)
                {
                    r[i] = r[i - 31];
                }
                for (int i = 34; i < 344; i++)
                {
                    r[i] = r[i - 31] + r[i - 3];
                }
            }

            n = 0;
        }

        public int Next()
        {
            unchecked
            {
                uint x = r[n % 344] = r[(n + 313) % 344] + r[(n + 341) % 344];
                n = (n + 1) % 344;
                return (int)(x >> 1);
            }
        }
    }
}
