using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Youtube_Storage
{
    internal class Generic
    {
        public void Write(string str, bool newline = true)
        {
            if (newline == true)
                Console.WriteLine(str);

            else
                Console.Write(str);
        }

        public bool IsInt(string str)
        {
            bool valid = int.TryParse(str, out int result);
            return valid;
        }

        public int StrToInt(string str)
        {
            if (IsInt(str))
            {
                return int.Parse(str);
            }

            else
            {
                return -1;
            }
        }

        public int Add(int[] ints)
        {
            int sum = 0;

            foreach (int i in ints)
            {
                sum += i;
            }

            return sum;
        }

        public int Multiply(int[] ints)
        {
            int prod = 0;

            foreach (int i in ints)
            {
                prod *= i;
            }

            return prod;
        }

        //Writes to "(fileName).txt" in Documents/GenericsOutput/(the name of the project). If no file name is given the file name will be debug.txt.
        public void WriteToFile(string str, string fileName = "debug", string dir = "")
        {
            var fullPath = "";

            var path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            var newFileName = $"{fileName}.txt";

            var projectName = Assembly.GetCallingAssembly().GetName().Name;

            if (dir == "")
            {
                fullPath = Path.Combine(path, "GenericsOutput");
                fullPath = Path.Combine(fullPath, projectName);
            }

            else
            {
                fullPath = Path.Combine(path, dir);
            }

            Directory.CreateDirectory(fullPath);

            fullPath = Path.Combine(fullPath, newFileName);

            StreamWriter sw = new StreamWriter(fullPath);
            {
                sw.WriteLine(str);
                sw.Close();
            }
        }

        public void DeleteFile(string path) 
        {
            var docPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            var fullpath = Path.Combine(docPath, path);

            if (File.Exists(fullpath))
            {
                File.Delete(fullpath);
            }
        }

        public bool FindFile(string path)
        {
            bool found = false;
            var docPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            
            var fullpath = Path.Combine(docPath,path);

            if (File.Exists(fullpath))
            {
                found = true;
            }

            return found;
        }

        public string GetPath(string path) 
        {
            string fullPath = "";
            var docPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            fullPath = Path.Combine(docPath, path);

            return fullPath;
        }

        public bool Sure(string str = "")
        {
            if (str == "")
            {
                Write("Are you sure? Y/N");
            }
            else
            {
                Write(str, false);
                Write(" Y/N");
            }

            while (true)
            {
                string key = Console.ReadKey().KeyChar.ToString().ToLower();

                if (key == "y")
                {
                    return true;
                }
                else if (key == "n")
                {
                    return false;
                }
            }
        }

        public void WaitForInput()
        {
            Console.ReadKey();
        }
    }
}
