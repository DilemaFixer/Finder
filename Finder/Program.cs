using System.Text;
using CsFinder;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace FinderInterface
{
    internal class Program
    {
        static void Main(string[] args)
        { 
            // >> string[],int -> (int,string)
            // Test.cs | row: 50 , line:15
            // public int Run1(string[] args , int i)
            //
            // ------------------------------
            
            Type type = typeof(MethodDeclarationSyntax); 
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Fello in CsFinder! Write exit to end \n");
            Console.WriteLine("Write path to folder");
            string path = StartInLoop(GetPathCondition, GetUserInput);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Path successfully set!\n");
            
            Console.WriteLine("Write query");
            string input = string.Empty;
            while (!string.Equals(input.ToLower().Trim() , "exit"))
            {
                input = StartInLoop(GetQueryCondition, GetUserInput);
                Query query = new Query(input);
                query.Parse();
                
                string[] csFiles = Directory.GetFiles(path, "*.cs", SearchOption.AllDirectories);
                
                if (csFiles.Length == 0)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("No .cs files found!");
                    return;
                }

                List<(string filename, SyntaxNode nodes)> csAsSyntaxNode = new();

                foreach (string csFile in csFiles)
                {
                    SyntaxNode current = CSharpSyntaxTree.ParseText(File.ReadAllText(csFile)).GetCompilationUnitRoot();
                    csAsSyntaxNode.Add((csFile,current));
                }
                
                if (csAsSyntaxNode.Count == 0)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("No SyntaxNode found!");
                    return;
                }
                
                foreach ((string fileCs , SyntaxNode syntaxNode)in csAsSyntaxNode)
                {
                    var nodes = syntaxNode.DescendantNodes();

                    foreach (SyntaxNode node in nodes)
                    {
                        if (node is MethodDeclarationSyntax mds && IsMatch(mds, query.Inputs, query.Outputs))
                        {
                            var location = mds.GetLocation();
                            var span = location.GetLineSpan();
                            
                            Console.ForegroundColor = ConsoleColor.DarkYellow;
                            string print = $"\nFile : {fileCs} \nPosition in file : {span.StartLinePosition.Line + 1}:{span.StartLinePosition.Character + 1}\n{GetFormattedMethodDeclaration(mds)}\n";
                            Console.WriteLine(print);
                            Console.ResetColor();
                        }
                    }
                } 
            }
            static string GetFormattedMethodDeclaration(MethodDeclarationSyntax method)
            {
                var declaration = method
                    .WithBody(null)
                    .WithExpressionBody(null)
                    .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken))
                    .NormalizeWhitespace();
    
                return declaration.ToString();
            }
            
            static bool GetQueryCondition(StringBuilder context)
            {
                string query = context.ToString();
                
                if(string.Equals(query , "NULL"))
                    return false;

                if (string.IsNullOrEmpty(query))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Query is empty or null");
                    Console.ResetColor();
                    return false;
                }
                
                Query queryObj = new Query(query);
                queryObj.Parse();
                if (!queryObj.IsValid())
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Query is invalid : " + query);
                    Console.ResetColor();
                    return false;
                }
                
                return true;
            }
            
            static void GetUserInput(StringBuilder context)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write(">> ");
                Console.ResetColor();
                context.Clear();
                context.Append(Console.ReadLine());
            }

            static bool GetPathCondition(StringBuilder context)
            {
                string path = context.ToString();
                
                if(string.Equals(path , "NULL"))
                    return false;
                
                if (string.IsNullOrEmpty(path))
                {
                    Console.ResetColor();
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Path to folder is empty or null");
                    return false;
                }

                if (!Directory.Exists(path))
                {
                    Console.ResetColor();
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Path folder not exist");
                    return false;
                }
                return true;
            }

            static string StartInLoop(Func<StringBuilder , bool> condition, Action<StringBuilder> action)
            {
                StringBuilder context = new StringBuilder();
                context.Append("NULL");
                while (!condition(context)) 
                    action(context);
                return context.ToString();
            }
            
            static bool IsMatch(MethodDeclarationSyntax mds , string[] inputs, string? output)
            {
                var parameters = mds.ParameterList.Parameters;
                if(parameters.Count != inputs.Length)
                    return false;
            
                for (int i = 0; i < parameters.Count; i++)
                {
                    string param = parameters[i].Type?.ToString()?.Trim() ?? "";
                    string input = inputs[i].Trim();
                    if (!string.Equals(param , input))
                        return false;
                }

                if (string.IsNullOrEmpty(output))
                    return true;
            
                string actualReturn = mds.ReturnType?.ToString()?.Trim() ?? "";
                string expectedReturn = output.Trim();
        
                actualReturn = actualReturn.Replace(" ", "");
                expectedReturn = expectedReturn.Replace(" ", "");
        
                if (!string.Equals(actualReturn, expectedReturn, StringComparison.OrdinalIgnoreCase))
                    return false;
        
                return true;
            }
        }
        
        /*
 Type type = typeof(MethodDeclarationSyntax); 
                     string input = "func pattern string[],int -> (int,string)";
                     Console.WriteLine(input);
                     Query query = new Query(input);
                     query.Parse();

                     string[] inputs = [];
                     string returnType = String.Empty;
                     string[] _temp = [];

                     if (query.Body.Contains("->"))
                     {
                         _temp = query.Body.Split("->");
                         inputs = _temp[0].Trim().Split(",");
                         returnType = _temp[1];
                     }
                     else
                         inputs = query.Body.Trim().Split(",");

                     Console.WriteLine(returnType);
                     inputs.ToList().ForEach(Console.WriteLine);

                     string[] csFiles = Directory.GetFiles("/Users/illashisko/Documents/GitHub/CSharpFinder", "*.cs", SearchOption.AllDirectories);
                     SyntaxNode[] csAsSyntaxNode = csFiles.Select((path, i) => CSharpSyntaxTree.ParseText(File.ReadAllText(path)).GetCompilationUnitRoot()).ToArray();

                     foreach (SyntaxNode syntaxNode in csAsSyntaxNode)
                     {
                         var nodes = syntaxNode.DescendantNodes();

                         foreach (SyntaxNode node in nodes)
                         {
                             if (node is MethodDeclarationSyntax mds && IsMatch(mds, inputs, returnType))
                             {
                                 Console.WriteLine(node.ToString());
                             }
                         }
                     }
        static bool IsMatch(MethodDeclarationSyntax mds , string[] inputs, string output)
        {
            var parameters = mds.ParameterList.Parameters;
            if(parameters.Count != inputs.Length)
                return false;
            
            for (int i = 0; i < parameters.Count; i++)
            {
                string param = parameters[i].Type?.ToString()?.Trim() ?? "";
                string input = inputs[i].Trim();
                if (!string.Equals(param , input))
                    return false;
            }

            if (string.IsNullOrEmpty(output))
                return true;
            
            string actualReturn = mds.ReturnType?.ToString()?.Trim() ?? "";
            string expectedReturn = output.Trim();
        
            actualReturn = actualReturn.Replace(" ", "");
            expectedReturn = expectedReturn.Replace(" ", "");
        
            if (!string.Equals(actualReturn, expectedReturn, StringComparison.OrdinalIgnoreCase))
                return false;
        
            return true;
        }
        */
    }
}
