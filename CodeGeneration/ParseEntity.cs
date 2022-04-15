using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CodeGeneration
{
    public class ParseEntity
    {
        public string path;

        public ParseEntity(string str)
        {
            if (string.IsNullOrWhiteSpace(str))
                throw new ArgumentException("Invalid path to entity");
            path = str;
        }

        public string TypeConverter(string str)
        {
            switch (str)
            {
                case "Long": return "int";
                case "String": return "string";
                case "boolean": return "bool";
                default: return str;
            }
        }

        public EntityDeclaration Parse()
        {
            var entity = new EntityDeclaration();
            entity.Fields = new List<ArgDeclaration>();
            string[] Massive = File.ReadAllLines(path, Encoding.Default);

            foreach (var line in Massive)
            {
                if ((line.Contains("private") || line.Contains("public")) && line.Contains(";"))
                {
                    var newLine = line.Replace(';', ' ');

                    string[] words = newLine.Split(' ');

                    var declaration = new ArgDeclaration();
                    int counter = 0;
                    for (int i = 0; i < words.Length; i++)
                    {
                        if (!string.IsNullOrWhiteSpace(words[i]))
                        {
                            counter++;
                            if (counter == 2)
                                declaration.typeName = TypeConverter(words[i]);
                            if (counter == 3)
                                declaration.argName = words[i];
                        }
                    }

                    entity.Fields.Add(declaration);
                }
            }

            return entity;
        }
    }
}