using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace CodeGeneration
{
    [Generator]
    public class SourceGen : ISourceGenerator
    {
        public void Initialize(GeneratorInitializationContext context)
        {
        }

        public void Execute(GeneratorExecutionContext context)
        {
            string path1 =
                @"C:\Users\dellx\Desktop\TechLabs\Lab2\demo\src\main\java\com\example\controllers\TaskController.java";
            string path2 =
                @"C:\Users\dellx\Desktop\TechLabs\Lab2\demo\src\main\java\com\example\controllers\UserController.java";
            string pathToEntity1 =
                @"C:\Users\dellx\Desktop\TechLabs\Lab2\demo\src\main\java\com\example\entity\TaskEntity.java";
            string pathToEntity2 =
                @"C:\Users\dellx\Desktop\TechLabs\Lab2\demo\src\main\java\com\example\entity\UserEntity.java";

            ParseEntity parseEntity1 = new ParseEntity(pathToEntity1);
            EntityDeclaration entity1 = parseEntity1.Parse();

            ParseEntity parseEntity2 = new ParseEntity(pathToEntity2);
            EntityDeclaration entity2 = parseEntity2.Parse();

            Parser parser1 = new Parser(path1);
            List<MethodDeclaration> dto1 = parser1.Parse();

            Parser parser2 = new Parser(path2);
            List<MethodDeclaration> dto2 = parser2.Parse();

            var project = CompilationUnit();
            project = project.AddUsings(UsingDirective(IdentifierName("System")));
            project = project.AddUsings(UsingDirective(QualifiedName(
                QualifiedName(IdentifierName("System"), IdentifierName("Collections")),
                IdentifierName("Generic"))));
            project = project.AddUsings(UsingDirective(QualifiedName(
                QualifiedName(IdentifierName("System"), IdentifierName("Net")),
                IdentifierName("Http"))));
            project = project.AddUsings(UsingDirective(QualifiedName(
                QualifiedName(IdentifierName("System"), IdentifierName("Text")),
                IdentifierName("Json"))));
            project = project.AddUsings(UsingDirective(QualifiedName(
                QualifiedName(QualifiedName(IdentifierName("System"), IdentifierName("Net")),
                    IdentifierName("Http")), IdentifierName("Json"))));
            var myNamespace = NamespaceDeclaration(IdentifierName("Dimas1")).NormalizeWhitespace();

            var myClass1 = ClassDeclaration("Controller1").AddModifiers(Token(SyntaxKind.PublicKeyword))
                .NormalizeWhitespace();
            var myClass2 = ClassDeclaration("Controller2").AddModifiers(Token(SyntaxKind.PublicKeyword))
                .NormalizeWhitespace();

            var myTaskEntity = ClassDeclaration("TaskEntity").AddModifiers(Token(SyntaxKind.PublicKeyword))
                .NormalizeWhitespace();
            var myUserEntity = ClassDeclaration("UserEntity").AddModifiers(Token(SyntaxKind.PublicKeyword))
                .NormalizeWhitespace();

            foreach (var arg in entity1.Fields)
            {
                var propertyDeclaration = PropertyDeclaration(ParseTypeName(arg.typeName), arg.argName)
                    .AddModifiers(Token(SyntaxKind.PublicKeyword))
                    .AddAccessorListAccessors(
                        AccessorDeclaration(SyntaxKind.GetAccessorDeclaration)
                            .WithSemicolonToken(Token(SyntaxKind.SemicolonToken)),
                        AccessorDeclaration(SyntaxKind.SetAccessorDeclaration)
                            .WithSemicolonToken(Token(SyntaxKind.SemicolonToken)));
                ;
                myTaskEntity = myTaskEntity.AddMembers(propertyDeclaration).NormalizeWhitespace();
            }

            foreach (var arg in entity2.Fields)
            {
                var propertyDeclaration = PropertyDeclaration(ParseTypeName(arg.typeName), arg.argName)
                    .AddModifiers(Token(SyntaxKind.PublicKeyword))
                    .AddAccessorListAccessors(
                        AccessorDeclaration(SyntaxKind.GetAccessorDeclaration)
                            .WithSemicolonToken(Token(SyntaxKind.SemicolonToken)),
                        AccessorDeclaration(SyntaxKind.SetAccessorDeclaration)
                            .WithSemicolonToken(Token(SyntaxKind.SemicolonToken)));
                ;
                myUserEntity = myUserEntity.AddMembers(propertyDeclaration).NormalizeWhitespace();
            }

            for (int i = 0; i < dto1.Count; i++)
            {
                if (dto1[i].HttpMethodName == "get")
                {
                    var method = MethodDeclaration(ParseTypeName(dto1[i].ReturnType), dto1[i].MethodName)
                        .NormalizeWhitespace();
                    method = method.AddModifiers(ParseToken("public"));

                    foreach (var z in dto1[i].ArgList)
                    {
                        method = method.AddParameterListParameters(Parameter(Identifier(z.argName))
                            .WithType(ParseTypeName(z.typeName)));
                    }


                    method = method.AddBodyStatements(LocalDeclarationStatement(VariableDeclaration(
                        ParseTypeName("HttpClient"),
                        SeparatedList<VariableDeclaratorSyntax>()
                            .Add(VariableDeclarator("client")
                                .WithInitializer(EqualsValueClause(
                                    ObjectCreationExpression(ParseTypeName("HttpClient"))
                                        .WithNewKeyword(Token(SyntaxKind.NewKeyword))
                                        .AddArgumentListArguments()))))));
                    ExpressionSyntax arg =
                        LiteralExpression(SyntaxKind.StringLiteralExpression, Literal(dto1[i].Url));
                    if (dto1[i].ArgList.Count > 0)
                    {
                        arg = BinaryExpression(SyntaxKind.AddExpression, BinaryExpression(SyntaxKind.AddExpression,
                                arg, LiteralExpression(SyntaxKind.StringLiteralExpression,
                                    Literal("?" + dto1[i].ArgList[0].argName + "="))),
                            IdentifierName(dto1[i].ArgList[0].argName));
                    }

                    method = method.AddBodyStatements(LocalDeclarationStatement(VariableDeclaration(
                        ParseTypeName("string"),
                        SeparatedList<VariableDeclaratorSyntax>()
                            .Add(VariableDeclarator("text")
                                .WithInitializer(EqualsValueClause(MemberAccessExpression(
                                    SyntaxKind.SimpleMemberAccessExpression, InvocationExpression(
                                        MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression,
                                            IdentifierName("client"), IdentifierName("GetStringAsync")),
                                        ArgumentList(SeparatedList<ArgumentSyntax>()
                                            .Add(Argument(arg)))), IdentifierName("Result"))))))));

                    method = method.AddBodyStatements(ReturnStatement(
                        InvocationExpression(MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression,
                                IdentifierName("JsonSerializer"),
                                GenericName("Deserialize")
                                    .AddTypeArgumentListArguments(ParseTypeName(dto1[i].ReturnType))))
                            .AddArgumentListArguments(Argument(IdentifierName("text")))));


                    myClass1 = myClass1.AddMembers(method).NormalizeWhitespace();
                    myClass1.NormalizeWhitespace();
                }
                else if (dto1[i].HttpMethodName == "post")
                {
                    var method = MethodDeclaration(ParseTypeName(dto1[i].ReturnType), dto1[i].MethodName)
                        .NormalizeWhitespace();
                    method = method.AddModifiers(ParseToken("public"));

                    foreach (var z in dto1[i].ArgList)
                    {
                        method = method.AddParameterListParameters(Parameter(Identifier(z.argName))
                            .WithType(ParseTypeName(z.typeName)));
                    }


                    method = method.AddBodyStatements(LocalDeclarationStatement(VariableDeclaration(
                        ParseTypeName("HttpClient"),
                        SeparatedList<VariableDeclaratorSyntax>()
                            .Add(VariableDeclarator("client")
                                .WithInitializer(EqualsValueClause(
                                    ObjectCreationExpression(ParseTypeName("HttpClient"))
                                        .WithNewKeyword(Token(SyntaxKind.NewKeyword))
                                        .AddArgumentListArguments()))))));

                    method = method.AddBodyStatements(LocalDeclarationStatement(VariableDeclaration(
                        ParseTypeName("string"),
                        SeparatedList<VariableDeclaratorSyntax>()
                            .Add(VariableDeclarator("text")
                                .WithInitializer(EqualsValueClause(MemberAccessExpression(
                                    SyntaxKind.SimpleMemberAccessExpression, InvocationExpression(
                                        MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression,
                                            MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression,
                                                MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression,
                                                    InvocationExpression(
                                                        MemberAccessExpression(
                                                            SyntaxKind.SimpleMemberAccessExpression,
                                                            IdentifierName("client"),
                                                            IdentifierName("PostAsJsonAsync")),
                                                        ArgumentList(SeparatedList<ArgumentSyntax>()
                                                            .Add(Argument(LiteralExpression(
                                                                SyntaxKind.StringLiteralExpression,
                                                                Literal(dto1[i].Url))))
                                                            .Add(Argument(
                                                                IdentifierName(dto1[i].ArgList[0].argName))))),
                                                    IdentifierName("Result")), IdentifierName("Content")),
                                            IdentifierName("ReadAsStringAsync"))), IdentifierName("Result"))))))));

                    method = method.AddBodyStatements(ReturnStatement(
                        InvocationExpression(MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression,
                                IdentifierName("JsonSerializer"),
                                GenericName("Deserialize")
                                    .AddTypeArgumentListArguments(ParseTypeName(dto1[i].ReturnType))))
                            .AddArgumentListArguments(Argument(IdentifierName("text")))));


                    myClass1 = myClass1.AddMembers(method).NormalizeWhitespace();
                    myClass1.NormalizeWhitespace();
                }
            }

            for (int i = 0; i < dto2.Count; i++)
            {
                if (dto2[i].HttpMethodName == "get")
                {
                    var method = MethodDeclaration(ParseTypeName(dto2[i].ReturnType), dto2[i].MethodName)
                        .NormalizeWhitespace();
                    method = method.AddModifiers(ParseToken("public"));

                    foreach (var argDec in dto2[i].ArgList)
                    {
                        method = method.AddParameterListParameters(Parameter(Identifier(argDec.argName))
                            .WithType(ParseTypeName(argDec.typeName)));
                    }


                    method = method.AddBodyStatements(LocalDeclarationStatement(VariableDeclaration(
                        ParseTypeName("HttpClient"),
                        SeparatedList<VariableDeclaratorSyntax>()
                            .Add(VariableDeclarator("client")
                                .WithInitializer(EqualsValueClause(
                                    ObjectCreationExpression(ParseTypeName("HttpClient"))
                                        .WithNewKeyword(Token(SyntaxKind.NewKeyword))
                                        .AddArgumentListArguments()))))));
                    ExpressionSyntax arg =
                        LiteralExpression(SyntaxKind.StringLiteralExpression, Literal(dto2[i].Url));
                    if (dto2[i].ArgList.Count > 0)
                    {
                        arg = BinaryExpression(SyntaxKind.AddExpression, BinaryExpression(SyntaxKind.AddExpression,
                                arg, LiteralExpression(SyntaxKind.StringLiteralExpression,
                                    Literal("?" + dto2[i].ArgList[0].argName + "="))),
                            IdentifierName(dto2[i].ArgList[0].argName));
                    }

                    method = method.AddBodyStatements(LocalDeclarationStatement(VariableDeclaration(
                        ParseTypeName("string"),
                        SeparatedList<VariableDeclaratorSyntax>()
                            .Add(VariableDeclarator("text")
                                .WithInitializer(EqualsValueClause(MemberAccessExpression(
                                    SyntaxKind.SimpleMemberAccessExpression, InvocationExpression(
                                        MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression,
                                            IdentifierName("client"), IdentifierName("GetStringAsync")),
                                        ArgumentList(SeparatedList<ArgumentSyntax>()
                                            .Add(Argument(arg)))), IdentifierName("Result"))))))));

                    method = method.AddBodyStatements(ReturnStatement(
                        InvocationExpression(MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression,
                                IdentifierName("JsonSerializer"),
                                GenericName("Deserialize")
                                    .AddTypeArgumentListArguments(ParseTypeName(dto2[i].ReturnType))))
                            .AddArgumentListArguments(Argument(IdentifierName("text")))));


                    myClass2 = myClass2.AddMembers(method).NormalizeWhitespace();
                    myClass2.NormalizeWhitespace();
                }
                else if (dto2[i].HttpMethodName == "post")
                {
                    var method = MethodDeclaration(ParseTypeName(dto2[i].ReturnType), dto2[i].MethodName)
                        .NormalizeWhitespace();
                    method = method.AddModifiers(ParseToken("public"));

                    foreach (var argDec in dto2[i].ArgList)
                    {
                        method = method.AddParameterListParameters(Parameter(Identifier(argDec.argName))
                            .WithType(ParseTypeName(argDec.typeName)));
                    }


                    method = method.AddBodyStatements(LocalDeclarationStatement(VariableDeclaration(
                        ParseTypeName("HttpClient"),
                        SeparatedList<VariableDeclaratorSyntax>()
                            .Add(VariableDeclarator("client")
                                .WithInitializer(EqualsValueClause(
                                    ObjectCreationExpression(ParseTypeName("HttpClient"))
                                        .WithNewKeyword(Token(SyntaxKind.NewKeyword))
                                        .AddArgumentListArguments()))))));

                    method = method.AddBodyStatements(LocalDeclarationStatement(VariableDeclaration(
                        ParseTypeName("string"),
                        SeparatedList<VariableDeclaratorSyntax>()
                            .Add(VariableDeclarator("text")
                                .WithInitializer(EqualsValueClause(MemberAccessExpression(
                                    SyntaxKind.SimpleMemberAccessExpression, InvocationExpression(
                                        MemberAccessExpression(
                                            SyntaxKind.SimpleMemberAccessExpression, MemberAccessExpression(
                                                SyntaxKind.SimpleMemberAccessExpression, MemberAccessExpression(
                                                    SyntaxKind.SimpleMemberAccessExpression, InvocationExpression(
                                                        MemberAccessExpression(
                                                            SyntaxKind.SimpleMemberAccessExpression,
                                                            IdentifierName("client"),
                                                            IdentifierName("PostAsJsonAsync")),
                                                        ArgumentList(SeparatedList<ArgumentSyntax>()
                                                            .Add(Argument(LiteralExpression(
                                                                SyntaxKind.StringLiteralExpression,
                                                                Literal(dto2[i].Url))))
                                                            .Add(Argument(
                                                                IdentifierName(dto2[i].ArgList[0].argName))))),
                                                    IdentifierName("Result")), IdentifierName("Content")),
                                            IdentifierName("ReadAsStringAsync"))), IdentifierName("Result"))))))));

                    method = method.AddBodyStatements(ReturnStatement(
                        InvocationExpression(MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression,
                                IdentifierName("JsonSerializer"),
                                GenericName("Deserialize")
                                    .AddTypeArgumentListArguments(ParseTypeName(dto2[i].ReturnType))))
                            .AddArgumentListArguments(Argument(IdentifierName("text")))));


                    myClass2 = myClass2.AddMembers(method).NormalizeWhitespace();
                    myClass2.NormalizeWhitespace();
                }
            }

            myNamespace = myNamespace.AddMembers(myClass1).AddMembers(myClass2).AddMembers(myTaskEntity)
                .AddMembers(myUserEntity).NormalizeWhitespace();
            project = project.AddMembers(myNamespace).NormalizeWhitespace();
            context.AddSource("dimasik.g.cs", project.ToFullString());
        }
    }
}