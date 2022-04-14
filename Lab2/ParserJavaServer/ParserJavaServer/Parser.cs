using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ParserJavaServer
{
    public class Parser
    {
        public string path;

        public Parser(string way)
        {
            if (string.IsNullOrEmpty(way))
                throw new ArgumentException("Null file-path");
            path = way;
        }

        public string TypeConverter(string str)
        {
            switch (str)
            {
                case "Long" : return "int";
                case "String" : return "string";
                case "boolean" : return "bool";
                default: return str;
            }
        }


        public List<MethodDeclaration> Parse()
        {
            List<MethodDeclaration> result = new List<MethodDeclaration>();
            
            string[] Massive = File.ReadAllLines(path, Encoding.Default);
            int cnt = 0;

            List<string> Keywords = new List<string>();
            Keywords.Add("private");
            Keywords.Add("public");
            Keywords.Add("protected");
            Keywords.Add("internal");

            var url = "";
            foreach (var line in Massive)   //парсим \task
            {
                cnt++;
                if (line.Contains("@RequestMapping"))
                {
                    var ind = line.IndexOf('"') + 1;
                    while (ind < line.Length && line[ind] != '"')
                    {
                        url += line[ind];
                        ind++;
                    }
                }
            }
        
            cnt = 0;
            int balance = 0;
            List<int> numbersOfLines = new List<int>();
            
            foreach (var line in Massive)    //находим строки содержащие методы
            {
                if (line.Contains("{") && !line.Contains("}"))
                {
                    balance++;
                    if (balance == 2)
                    {
                        numbersOfLines.Add(cnt);
                    }
                }
                if (line.Contains("}") && !line.Contains("{"))
                {
                    balance--;
                }
                cnt++;
            }

            foreach (var i in numbersOfLines) //разбиваем пробелами строки на слова
            {
                Massive[i] = Massive[i].Replace('(', ' ') ;
                Massive[i] = Massive[i].Replace(')', ' ') ;
                Massive[i] = Massive[i].Replace('{', ' ') ;
                Massive[i] = Massive[i].Replace(',', ' ') ;
            }
            
            foreach (var i in numbersOfLines) //фиксим проблему с двойными пробелами
            {
                string nwString = "";
                if (Massive[i][0] != ' ')
                    nwString += Massive[i][0];
                for (int j = 1; j < Massive[i].Length; j++)
                {
                    if (Massive[i][j] != ' ') 
                        nwString += Massive[i][j];
                    else
                    {
                        if (Massive[i][j] == ' ' && Massive[i][j - 1] != ' ')
                        {
                            nwString += Massive[i][j];
                        }
                    }
                }
                Massive[i] = nwString;
            }

            foreach (var i in numbersOfLines)   //парсим \task
            {
                cnt++;
                MethodDeclaration md = new MethodDeclaration();

                if (Massive[i - 1].Contains("Post"))
                {
                    md.HttpMethodName = "post";
                }
                if (Massive[i - 1].Contains("Get"))
                {
                    md.HttpMethodName = "get";
                }
                if (Massive[i - 1].Contains("Put"))
                {
                    md.HttpMethodName = "put";
                }
                if (Massive[i - 1].Contains("Delete"))
                {
                    md.HttpMethodName = "delete";
                }

                md.Url = "http://localhost:8080" + url + "/" + md.HttpMethodName;
                
                string[] words = Massive[i].Split(' ');
                List<ArgDeclaration> arguments = new List<ArgDeclaration>();
                for (int j = 0; j < words.Length; j++) // собрали аргументы
                {
                    if (words[j].Contains('@'))
                    {
                        ArgDeclaration arg = new ArgDeclaration();
                        arg.typeName = TypeConverter(words[j + 1]);
                        arg.argName = words[j + 2];
                        arguments.Add(arg);
                    }
                }

                int indx = 0;
                while (indx < words.Length && Keywords.Contains(words[indx]))
                {
                    indx++;
                }

                md.ReturnType = TypeConverter(words[indx]);
                md.MethodName = words[indx + 1];
                md.ArgList = arguments;
                result.Add(md);
            }
            
            return result;
        }
    }
}

