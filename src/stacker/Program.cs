using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;

namespace stacker
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.Write("Component name: ");
            var component = Console.ReadLine();
            Console.WriteLine();

            string[] componentDirectories = Directory.GetDirectories(System.IO.Directory.GetCurrentDirectory(), "components", SearchOption.AllDirectories).Where(d => !d.Contains("node_modules")).ToArray();

            var allDirectories = new List<KeyValuePair<int, string>>();

            foreach (string a in componentDirectories)
            {
                allDirectories.Add(MakeDict(1, a));

                string[] bSubs = Directory.GetDirectories(a);

                foreach (string b in bSubs)
                {
                    allDirectories.Add(MakeDict(2, b));

                    string[] cSubs = Directory.GetDirectories(b);

                    foreach (string c in cSubs)
                    {
                        allDirectories.Add(MakeDict(3, c));

                        string[] dSubs = Directory.GetDirectories(c);

                        foreach (string d in dSubs)
                        {
                            allDirectories.Add(MakeDict(4, d));
                        }
                    }
                }
            }

            for (int i = 1; i <= allDirectories.Count; i++)
            {
                var dict = allDirectories[i - 1];
                Console.WriteLine(Indent(dict.Key * 4) + "[" + i + "] " + ShortenPath(dict.Value));
            }

            bool selected = false;
            string finalDirectory = "";

            while (selected == false)
            {
                Console.WriteLine();
                Console.Write("Choose directory: ");

                int directory;
                bool isInt = int.TryParse(Console.ReadLine(), out directory);

                Console.WriteLine();

                if (!isInt)
                {
                    Console.WriteLine("You may only enter a number that corresponds with the directory options above.");
                }
                else if (directory > allDirectories.Count || directory <= 0)
                {
                    Console.WriteLine("That is not a valid option.");
                }
                else
                {
                    finalDirectory = allDirectories[directory - 1].Value + "\\" + component;
                    if (Directory.Exists(finalDirectory))
                    {
                        finalDirectory = null;
                        Console.WriteLine("This directory already exists.");
                    }
                    else
                    {
                        selected = true;
                    }
                }
            }

            Directory.CreateDirectory(finalDirectory);
            using (StreamWriter writer = new StreamWriter(finalDirectory + "\\" + component + ".jsx", true))
            {
                writer.WriteLine("import React, { Component } from 'react'");
                writer.WriteLine();
                writer.WriteLine("export default class " + component + " extends Component {");
                writer.WriteLine(Indent(2) + "render() {");
                writer.WriteLine(Indent(4) + "return (");
                writer.WriteLine(Indent(6) + "<h1>" + component + "</h1>");
                writer.WriteLine(Indent(4) + ")");
                writer.WriteLine(Indent(2) + "}");
                writer.WriteLine("}");
            }

            using (StreamWriter writer = new StreamWriter(finalDirectory + "\\package.json", true))
            {
                writer.WriteLine("{");
                writer.WriteLine(Indent(2) + "\"name\": \"" + component + "\",");
                writer.WriteLine(Indent(2) + "\"version\": \"0.0.0\",");
                writer.WriteLine(Indent(2) + "\"private\": true,");
                writer.WriteLine(Indent(2) + "\"main\": \"./" + component + "\"");
                writer.WriteLine("}");
            }
        }

        public static KeyValuePair<int, string> MakeDict(int num, string s)
        {
            return new KeyValuePair<int, string>(num, s);
        }

        public static string ShortenPath(string input)
        {
            var directories = input.Split('\\');
            return directories.Last();
        }

        public static string Indent(int count)
        {
            return "".PadLeft(count);
        }
    }
}
