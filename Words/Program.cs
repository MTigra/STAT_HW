using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CoolSR
{
    class Program
    {

        static void Main(string[] args)
        {
            string info;
            Errors res = CheckInput(args,out info);
            if (res != Errors.Ok)
            {
                switch (res)
                {
                    case Errors.ConflictingFlags:
                        Console.WriteLine("ConFlictFlags");
                        break;
                    case Errors.CannotOpen:
                        Console.WriteLine("CAN NOT OPEN");
                        break;
                    case Errors.EmptyInput:
                        Console.WriteLine("Empty Input.");
                        break;
                    case Errors.InvalidFlags:
                        Console.WriteLine("Invalid Flag:"+info);
                        break;
                    case Errors.Ok:
                        break;
                }
            }



        }


        static string[] arguments = { "-q", "-s", "-c", "-p", "-l" };

        static Errors CheckInput(string[] args, out string info)
        {
            info = "";
            if (args.Length == 0) return Errors.EmptyInput;

            //Check if too many files.
            if (IsManyFiles(args)) return Errors.TooManyFiles;
            
            //some stupid checks
            for (int i = 0; i < args.Length - 1; i++)
            {
                if (!arguments.Contains(args[i]))
                {
                    info = args[i]; return Errors.InvalidFlags;
                    
                }
                if ((args.Contains("-s") || args.Contains("-c")) && args.Contains("-q")) return Errors.ConflictingFlags;
            }
            if (args[args.Length - 1][0] == '-')
            {
                if (!arguments.Contains(args[args.Length - 1]))
                {
                    info = args[args.Length - 1]; return Errors.InvalidFlags;

                }
            }
            else
            {
                if (!File.Exists(args[args.Length - 1])) return Errors.CannotOpen;
            }


            return Errors.Ok;

        }

        static  bool IsManyFiles(string[] args)
        {
            int count = 0;
            string pattern = @"^\w\S+";
            for (int i = 0; i < args.Length; i++)
            {
                count = Regex.IsMatch(args[i], pattern) ? ++count : count;
            }
            if (count >=2) return true;

            return false;
        }
    }
    enum Errors
    {
        InvalidFlags,
        ConflictingFlags,
        CannotOpen,
        TooManyFiles,
        Ok,
        EmptyInput

    }
}