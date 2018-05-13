using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
/*
Выполнил: Мартиросян Тигран
Группа 176(1)
*/
namespace CoolSR
{
    class Program
    {
        static string[] arguments = { "-q", "-s", "-c", "-p", "-l" };

        static void Main(string[] args)
        {
            //Переведем все в нижниый регистр. Для имени файла это не критично, очевидно почему.
            args= Array.ConvertAll(args, x => x.ToLower());
            string info;
            //посмотрим какой ответ нам дала проверка аргументов командной строки
            Errors res = CheckInput(args, out info);
            //Если что-то плохо, то скажем об этом пользователю и выйдем.
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
                        Console.WriteLine("Invalid Flag:" + info);
                        break;
                }
                return;
            }
            //Ура. Все хорошо, посмотрим указал ли пользовател имя файла.
            if (args[args.Length - 1][0] == '-')
            {
                //Если указал то пойдем работать с консолью
                Start(args, InputKind.Console);
            }
            else
            {
                //Если не указал пойдем работать с файлом
                Start(args, InputKind.File);
            }

            Console.ReadKey();

        }

        /// <summary>
        /// Выполняет основную работу программы.
        /// </summary>
        /// <param name="args"></param>
        /// <param name="inputKind"></param>
        private static void Start(string[] args, InputKind inputKind)
        {
            Text text;
            Text output;
            //Чекаем способ ввода, получчаем текст в зависисости от результата проверки
            if (inputKind == InputKind.Console)
            {
                Console.WriteLine("Введите текст:");
                string userinput = Console.ReadLine();
                text = new Text(userinput);
            }
            else
            {
                string input = File.ReadAllText(args[args.Length - 1]);
                text = new Text(input);
            }
            //Исходящий текст. 
            output = new Text(text);
            //Выполняем просьбы пользователя исходя из параметров

            if (args.Contains("-l")) { PrintLengthInfos(text); }


            if (args.Contains("-s"))
            {
                output.DeleteDupSpaces();
            }

            if (args.Contains("-c"))
            {
                output.OnlyRealWords();
            }

            if (args.Contains("-p"))
            {
                PrintStatistics(text, output);
            }
            //Если пользователь не просил новый файл то его делать не будет
            if (!args.Contains("-q"))
            {
                var sw = File.CreateText("text.txt");
                sw.Write(output.Content);
                sw.Close();
            }
        }

        /// <summary>
        /// Печатает статистику в консоль, а также возвращает в виде string
        /// </summary>
        /// <param name="input"></param>
        /// <param name="output"></param>
        /// <returns></returns>
        private static string PrintStatistics(Text input, Text output)
        {
            string str = "";
            str += $"\nСимволы: входной:{input.Length}; выходной: {output.Length}";
            str += $"\nСтроки: входной:{input.LinesCount}; выходной: {output.LinesCount}";
            str += $"\nСлова: входной:{input.Words.Count}; выходной: {output.Words.Count}";
            str += $"\nПроизвольные слова: входной {input.NormalWords.Count};";
            str += $"\nНастоящие слова: входной {input.RealWords.Count};";
            str += $"\nЗаглавные слова: входной {input.CapitalWords.Count};";
            str += $"\nАкронимы: входной {input.Acronyms.Count};";
            Console.WriteLine(str);
            return str;
        }

        /// <summary>
        /// Печатает информацию по длинам в консоль, а также возвращает в виде string
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        private static string PrintLengthInfos(Text text)
        {
            string str = "";
            if (text.LongestWords.Count != 0)
            {
                var longestWords = text.LongestWords;
                str += $"\nСлово(длина {longestWords[0].Length}): ";
                for (int i = 0; i < longestWords.Count; i++)
                {
                    str += longestWords[i] + "; ";
                }
            }

            if (text.LongestNormalWords.Count != 0)
            {
                var longestnormalWords = text.LongestNormalWords;
                str += $"\nПроизвольное слово(длина {longestnormalWords[0].Length}): ";
                for (int i = 0; i < longestnormalWords.Count; i++)
                {
                    str += longestnormalWords[i] + "; ";
                }
            }

            if (text.LongestRealWords.Count != 0)
            {
                var longetRealWords = text.LongestRealWords;
                str += $"\nНастоящее слово(длина {longetRealWords[0].Length}): ";
                for (int i = 0; i < longetRealWords.Count; i++)
                {
                    str += longetRealWords[i] + "; ";
                }
            }
            Console.WriteLine(str);
            return str;
        }

        
        /// <summary>
        /// Проверяет введенные пользователем параметры на корректность.
        /// </summary>
        /// <param name="args"></param>
        /// <param name="info"></param>
        /// <returns></returns>
        static Errors CheckInput(string[] args, out string info)
        {
            info = "";
            //здесь все понятно
            if (args.Length == 0) return Errors.EmptyInput;

            //Проверяем много ли файлов
            if (IsManyFiles(args)) return Errors.TooManyFiles;

            //Проверяем все аргументы кроме последнего
            for (int i = 0; i < args.Length - 1; i++)
            {
                //Проверяем несуществующие аргументы
                if (!arguments.Contains(args[i]))
                {
                    info = args[i]; return Errors.InvalidFlags;
                }
                //проверяем несовместимые аргументы
                if ((args.Contains("-s") || args.Contains("-c")) && args.Contains("-q")) return Errors.ConflictingFlags;
            }
            //Проверяем последний аргумент отдельно.
            //Если первый символ "-" то это аргумент, а не имя файла
            if (args[args.Length - 1][0] == '-')
            {

                if (!arguments.Contains(args[args.Length - 1]))
                {
                    info = args[args.Length - 1]; return Errors.InvalidFlags;

                }
            }
            else
            {
                //Проверим существвуует ли файл с таким имененем
                if (!File.Exists(args[args.Length - 1])) return Errors.CannotOpen;
            }


            return Errors.Ok;

        }

        /// <summary>
        /// Возвращает True если среди аргументов найдено более одного имени файла.
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        static bool IsManyFiles(string[] args)
        {
            int count = 0;
            string pattern = @"^\w\S+";
            for (int i = 0; i < args.Length; i++)
            {
                count = Regex.IsMatch(args[i], pattern) ? ++count : count;
            }
            if (count >= 2) return true;

            return false;
        }
    }
    //Было бы лучше сделать словарь, но и так неплохо.
    enum Errors
    {
        InvalidFlags,
        ConflictingFlags,
        CannotOpen,
        TooManyFiles,
        Ok,
        EmptyInput

    }

    enum InputKind
    {
        Console,
        File
    }
}