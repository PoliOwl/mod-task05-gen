using System;
using System.Collections.Generic;
using System.Text;
using System.IO;


namespace generator
{
    class CharGenerator 
    {
        private string syms = "абвгдеёжзийклмнопрстуфхцчшщьыъэюя"; 
        private char[] data;
        private int size;
        private Random random = new Random();
        public CharGenerator() 
        {
           size = syms.Length;
           data = syms.ToCharArray();
        }
        public char getSym() 
        {
           return data[random.Next(0, size)]; 
        }
    }

    class BigrammGenerator
    {
        public static int size = 31;
        private Random random = new Random();
        private class bigramm
        {
            public static List<string> syms = new List<string>{
                "а", "б","в", "г", "д",
                "е", "ё", "ж", "з",
                "и", "й", "к", "л",
                "м", "н", "о", "п",
                "р", "с", "т", "у",
                "ф", "х", "ц", "ч",
                "ш", "щ", "ь","ы",
                "ъ", "э", "ю", "я" };
            static int total = 0;
            public static int Total
            {
                get { return total; }
            }
            private string value = "";
            public string Value
            {
                get { return value; }
            }
            private int weight;
            public int Weight
            {
                get { return weight; }
            }

            private int indWeight;

            public int IndexWeight
            {
                get { return indWeight; }
            }

            public static void set_syms(List<string> Syms)
            {
                syms = Syms;
            }

            public bigramm(int wei, int firstInd, int secondInd, bool two = false)
            {
                if (two)
                {
                    value += syms[firstInd];
                    value += syms[secondInd];
                } else
                {
                    value = syms[firstInd + secondInd];
                }
                weight = wei;
                total += weight;
                indWeight = total;
            }

            public static void reset()
            {
                syms = new List<string>{
                "а", "б","в", "г", "д",
                "е", "ё", "ж", "з",
                "и", "й", "к", "л",
                "м", "н", "о", "п",
                "р", "с", "т", "у",
                "ф", "х", "ц", "ч",
                "ш", "щ", "ь","ы",
                "ъ", "э", "ю", "я" };
                total = 0;
            }

        }

        List<bigramm> bigramms = new List<bigramm>();

        public BigrammGenerator(string file, int bSize = 31, bool two = true, bool from_file = false, char spliter = ' ')
        {
            size = bSize;
            string[] lines = File.ReadAllLines(file);
            if (from_file)
            {
                List<string> syms = new List<string>(lines[0].Split(spliter));
                bigramm.set_syms(syms);
                int l = lines.Length;
                string[] newLines = new string[l-1];
                Array.Copy(lines, 1, newLines, 0, l-1);
                lines = newLines;
            }
            for (int i = 0; i < lines.Length; ++i)
            {
                string[] line = lines[i].Split(spliter);
                for (int j = 0; j < line.Length; ++j)
                {
                    bigramm bigr = new bigramm(Int32.Parse(line[j]), i, j, two);
                    bigramms.Add(bigr);
                }
            }
            List<bigramm> newBigr = new List<bigramm>();
            foreach (bigramm b in bigramms)
            {
                if (b.Weight != 0)
                {
                    newBigr.Add(b);
                }
            }
            bigramms = newBigr;
        }

        public string nexBigramm()
        {
            int rand = random.Next(1, bigramm.Total);
            bigramm ans = null;
            foreach (bigramm bigr in bigramms)
            {
                if (rand >= (bigr.IndexWeight - bigr.Weight) && rand <= bigr.IndexWeight)
                {
                    ans = bigr;
                    break;
                }
            }
            if (ans == null)
            {
                Console.WriteLine(rand.ToString() + " " + bigramm.Total.ToString());
            }

            return ans.Value;
        }

        public void reset()
        {
            bigramm.reset();
        }

    }

         
    class Program
    {
        static void writeFile(string file, BigrammGenerator bigrGen, char? sep = null)
        {
            string ans = "";
            for (int i = 0; i < 1000; ++i)
            {
                ans += bigrGen.nexBigramm();
                if (sep != null)
                {
                    ans += sep;
                }
            }
            File.WriteAllText(file, ans);
        }
        static void Main(string[] args)
        {
            BigrammGenerator bigrGen = new BigrammGenerator("../../../bigramms.txt");
            writeFile("bigrammsGenerated.txt", bigrGen);
            bigrGen.reset();
            bigrGen = new BigrammGenerator("../../../words.txt", 100, false, true);
            writeFile("wordsGenerated.txt", bigrGen, ' ');
            bigrGen.reset();
            bigrGen = new BigrammGenerator("../../../wordsPair.txt", 100, false, true, '#');
            writeFile("wordsPairGenerated.txt", bigrGen, ' ');
            bigrGen.reset();
        }
    }
}

