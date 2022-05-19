using System.Collections.Generic;

namespace CodeGeneration
{
    public class MethodDeclaration
    {
        public string MethodName;
        public string ReturnType;
        public List<ArgDeclaration> ArgList;
        public string Url;
        public string HttpMethodName;
    }

    public struct ArgDeclaration
    {
        public string typeName;
        public string argName;
    }
}