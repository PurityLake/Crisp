using System;
using System.IO;

namespace Crisp
{
    public class REPL
    {
        private static String prompt = "> ";
        private static String continuePrompt = ". ";

        private String GetInput()
        {
            bool error = true;
            Console.Write(prompt);
            String buffer = Console.ReadLine();
            if (buffer.Length == 0)
            {
                return GetInput();
            }
            Parser p = new Parser();
            while (error)
            {
                try
                {
                    p.Parse(buffer);
                    break;
                }
                catch (CrispParserExpcetion exc)
                {
                    if (exc.Message.Contains("parenthesises"))
                    {
                        buffer += '\n';
                    }
                    else
                    {
                        throw new CrispException("Failed to parse in REPL", -1);
                    }
                }
                Console.Write(continuePrompt);
                buffer += Console.ReadLine();
            }
            return buffer;
        }

        public void Run()
        {
            Environment e = new Environment(null);
            Parser p = new Parser();
            MemoryStream ms = new MemoryStream();
            while (true)
            {
                String val = GetInput();
                if (val.ToLower().Equals("quit"))
                {
                    break;
                }

                try
                {
                    var t = Evaluator.Evaluate(ref e, ms, p.Parse(val));
                    ms.Position = 0;
                    StreamReader sr = new StreamReader(ms);
                    Console.Write(sr.ReadToEnd());
                }
                catch (CrispArgumentException exc)
                {
                    ms.Position = 0;
                    StreamReader sr = new StreamReader(ms);
                    Console.Write(sr.ReadToEnd());
                    Console.WriteLine();
                    Console.WriteLine(String.Format("Argument Exception (Line {0}):", exc.Line));
                    Console.WriteLine(exc.Message);
                }
                catch (CrispParserExpcetion exc)
                {
                    ms.Position = 0;
                    StreamReader sr = new StreamReader(ms);
                    Console.Write(sr.ReadToEnd());
                    Console.WriteLine();
                    Console.WriteLine(String.Format("Parser Exception (Line {0}):", exc.Line));
                    Console.WriteLine(exc.Message);
                }
                catch (CrispNotExistsException exc)
                {
                    ms.Position = 0;
                    StreamReader sr = new StreamReader(ms);
                    Console.Write(sr.ReadToEnd());
                    Console.WriteLine();
                    Console.WriteLine(String.Format("Identifer Exception (Line {0})", exc.Line));
                    Console.WriteLine(exc.Message);
                }
                catch (CrispException exc)
                {
                    ms.Position = 0;
                    StreamReader sr = new StreamReader(ms);
                    Console.Write(sr.ReadToEnd());
                    Console.WriteLine();
                    Console.WriteLine(String.Format("Exception (Line {0}):", exc.Line));
                    Console.WriteLine(exc.Message);
                }
                ms.SetLength(0);
            }
            Console.WriteLine("\nThanks for using Crisp");
        }
    }
}
