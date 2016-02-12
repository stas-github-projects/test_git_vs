using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace test_git
{
    class Program
    {
        static void Main(string[] args)
        {
            Stopwatch _s = new Stopwatch();

            _s.Start();

            /* */
            //Trie<byte[]> _trie = new Trie<byte[]>();
            Trie<string> _trie = new Trie<string>();
            for (int i = 0; i < 1000000; i++)
            {
                //byte[] bkey = Encoding.ASCII.GetBytes("root/sub/key" + i);
                //byte[] bvalue = Encoding.ASCII.GetBytes("test_" + i);
                _trie.Add("root/sub/key" + i, "test_" + i);
            }

            //var f=_trie.Find("root/sub/key999090");
            var g = _trie.FindAll("root/sub/key0");
            // SPEED ~ 1700 msec
            //*/

            /* /
            Trie2 _trie2 = new Trie2();
            for (int i = 0; i < 1000000; i++)
            {
                _trie2.AddNodeForWord("root/sub/key" + i);
            }
            //*/

            _s.Stop();

            Console.WriteLine("hj");
            Console.ReadKey();

        }
    }
}
