using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace FinderInterface;
public class ASTVisualizer
{
    private int _indentLevel = 0;
    private const string INDENT = "  ";

    public void VisualizeAST(string sourceCode)
    {
        SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(sourceCode);
        var root = syntaxTree.GetCompilationUnitRoot();

        Console.WriteLine("=== ПОЛНОЕ AST ДЕРЕВО ===\n");
        
        VisitNode(root);
    }

    private void VisitNode(SyntaxNode node)
    {
        PrintNodeInfo(node);

        _indentLevel++;

        foreach (var child in node.ChildNodes())
        {
            VisitNode(child);
        }

        _indentLevel--;
    }

    private void PrintNodeInfo(SyntaxNode node)
    {
        string indent = string.Concat(Enumerable.Repeat(INDENT, _indentLevel));
        string nodeType = node.GetType().Name;
        
        string additionalInfo = GetAdditionalNodeInfo(node);
        
        ConsoleColor color = GetNodeColor(nodeType);
        
        Console.ForegroundColor = color;
        Console.Write($"{indent}├─ {nodeType}");
        
        if (!string.IsNullOrEmpty(additionalInfo))
        {
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write($" [{additionalInfo}]");
        }
        
        Console.ResetColor();
        Console.WriteLine();
    }

    protected string GetAdditionalNodeInfo(SyntaxNode node)
    {
        return node switch
        {
            ClassDeclarationSyntax classDecl => $"Class: {classDecl.Identifier.ValueText}",
            InterfaceDeclarationSyntax interfaceDecl => $"Interface: {interfaceDecl.Identifier.ValueText}",
            MethodDeclarationSyntax methodDecl => $"Method: {methodDecl.Identifier.ValueText}",
            PropertyDeclarationSyntax propertyDecl => $"Property: {propertyDecl.Identifier.ValueText}",
            FieldDeclarationSyntax fieldDecl => $"Field: {string.Join(", ", fieldDecl.Declaration.Variables.Select(v => v.Identifier.ValueText))}",
            ConstructorDeclarationSyntax ctorDecl => $"Constructor: {ctorDecl.Identifier.ValueText}",
            NamespaceDeclarationSyntax namespaceDecl => $"Namespace: {namespaceDecl.Name}",
            UsingDirectiveSyntax usingDecl => $"Using: {usingDecl.Name}",
            VariableDeclarationSyntax varDecl => $"Variables: {string.Join(", ", varDecl.Variables.Select(v => v.Identifier.ValueText))}",
            ParameterSyntax param => $"Parameter: {param.Identifier.ValueText}",
            IdentifierNameSyntax identifier => $"Identifier: {identifier.Identifier.ValueText}",
            LiteralExpressionSyntax literal => $"Literal: {literal.Token.ValueText}",
            PredefinedTypeSyntax predefinedType => $"Type: {predefinedType.Keyword.ValueText}",
            ReturnStatementSyntax => "Return Statement",
            ExpressionStatementSyntax => "Expression Statement",
            BlockSyntax => "Code Block",
            AccessorDeclarationSyntax accessor => $"Accessor: {accessor.Keyword.ValueText}",
            EventDeclarationSyntax eventDecl => $"Event: {eventDecl.Identifier.ValueText}",
            EventFieldDeclarationSyntax eventField => $"Event Field: {string.Join(", ", eventField.Declaration.Variables.Select(v => v.Identifier.ValueText))}",
            EnumDeclarationSyntax enumDecl => $"Enum: {enumDecl.Identifier.ValueText}",
            StructDeclarationSyntax structDecl => $"Struct: {structDecl.Identifier.ValueText}",
            _ => ""
        };
    }

    protected ConsoleColor GetNodeColor(string nodeType)
    {
        return nodeType switch
        {
            "ClassDeclarationSyntax" or "InterfaceDeclarationSyntax" or "StructDeclarationSyntax" 
                or "EnumDeclarationSyntax" or "NamespaceDeclarationSyntax" => ConsoleColor.Cyan,
            
            "MethodDeclarationSyntax" or "PropertyDeclarationSyntax" or "ConstructorDeclarationSyntax" 
                or "AccessorDeclarationSyntax" => ConsoleColor.Green,
            
            "FieldDeclarationSyntax" or "VariableDeclarationSyntax" or "ParameterSyntax" => ConsoleColor.Yellow,
            
            "PredefinedTypeSyntax" or "IdentifierNameSyntax" or "QualifiedNameSyntax" => ConsoleColor.Magenta,
            
            "ReturnStatementSyntax" or "ExpressionStatementSyntax" or "IfStatementSyntax" 
                or "ForStatementSyntax" or "WhileStatementSyntax" => ConsoleColor.Red,
            
            "LiteralExpressionSyntax" or "InvocationExpressionSyntax" or "MemberAccessExpressionSyntax" 
                or "AssignmentExpressionSyntax" => ConsoleColor.DarkYellow,
            
            "BlockSyntax" or "ParameterListSyntax" or "ArgumentListSyntax" or "BaseListSyntax" => ConsoleColor.DarkGray,
            
            "UsingDirectiveSyntax" or "ModifiersSyntax" => ConsoleColor.White,
            
            _ => ConsoleColor.Gray
        };
    }
}

public class DetailedASTVisualizer : ASTVisualizer
{
    public void VisualizeASTWithDetails(string sourceCode)
    {
        SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(sourceCode);
        var root = syntaxTree.GetCompilationUnitRoot();

        Console.WriteLine("=== ДЕТАЛЬНОЕ AST ДЕРЕВО ===\n");
        Console.WriteLine($"Исходный код ({sourceCode.Length} символов):");
        Console.WriteLine(new string('-', 50));
        Console.ForegroundColor = ConsoleColor.DarkGreen;
        Console.WriteLine(sourceCode);
        Console.ResetColor();
        Console.WriteLine(new string('-', 50));
        Console.WriteLine("\nAST Структура:\n");
        
        VisitNodeWithDetails(root, 0);
        
        Console.WriteLine("\n=== СТАТИСТИКА AST ===");
        PrintASTStatistics(root);
    }

    private void VisitNodeWithDetails(SyntaxNode node, int depth)
    {
        string indent = string.Concat(Enumerable.Repeat("│  ", depth));
        string connector = depth > 0 ? "├─ " : "";
        
        Console.ForegroundColor = GetNodeColor(node.GetType().Name);
        Console.Write($"{indent}{connector}{node.GetType().Name}");
        
        string details = GetDetailedNodeInfo(node);
        if (!string.IsNullOrEmpty(details))
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write($" → {details}");
        }
        
        var span = node.GetLocation().GetLineSpan();
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.Write($" (Line: {span.StartLinePosition.Line + 1}, Col: {span.StartLinePosition.Character + 1})");
        
        Console.ResetColor();
        Console.WriteLine();

        foreach (var child in node.ChildNodes())
        {
            VisitNodeWithDetails(child, depth + 1);
        }
    }

    private string GetDetailedNodeInfo(SyntaxNode node)
    {
        return node switch
        {
            ClassDeclarationSyntax classDecl => 
                $"'{classDecl.Identifier.ValueText}' " +
                $"[Modifiers: {string.Join(" ", classDecl.Modifiers.Select(m => m.ValueText))}] " +
                $"[Members: {classDecl.Members.Count}]",
                
            MethodDeclarationSyntax methodDecl => 
                $"'{methodDecl.Identifier.ValueText}' " +
                $"[Return: {methodDecl.ReturnType}] " +
                $"[Params: {methodDecl.ParameterList.Parameters.Count}] " +
                $"[Modifiers: {string.Join(" ", methodDecl.Modifiers.Select(m => m.ValueText))}]",
                
            PropertyDeclarationSyntax propertyDecl => 
                $"'{propertyDecl.Identifier.ValueText}' " +
                $"[Type: {propertyDecl.Type}] " +
                $"[Accessors: {propertyDecl.AccessorList?.Accessors.Count ?? 0}]",
                
            FieldDeclarationSyntax fieldDecl => 
                $"[{string.Join(", ", fieldDecl.Declaration.Variables.Select(v => v.Identifier.ValueText))}] " +
                $"[Type: {fieldDecl.Declaration.Type}] " +
                $"[Modifiers: {string.Join(" ", fieldDecl.Modifiers.Select(m => m.ValueText))}]",
                
            LiteralExpressionSyntax literal => $"Value: '{literal.Token.ValueText}'",
            IdentifierNameSyntax identifier => $"Name: '{identifier.Identifier.ValueText}'",
            ParameterSyntax param => $"'{param.Identifier.ValueText}' : {param.Type}",
            
            _ => GetAdditionalNodeInfo(node)
        };
    }

    private void PrintASTStatistics(SyntaxNode root)
    {
        var stats = new
        {
            TotalNodes = root.DescendantNodesAndSelf().Count(),
            Classes = root.DescendantNodes().OfType<ClassDeclarationSyntax>().Count(),
            Interfaces = root.DescendantNodes().OfType<InterfaceDeclarationSyntax>().Count(),
            Methods = root.DescendantNodes().OfType<MethodDeclarationSyntax>().Count(),
            Properties = root.DescendantNodes().OfType<PropertyDeclarationSyntax>().Count(),
            Fields = root.DescendantNodes().OfType<FieldDeclarationSyntax>().Count(),
            Constructors = root.DescendantNodes().OfType<ConstructorDeclarationSyntax>().Count(),
            Events = root.DescendantNodes().OfType<EventDeclarationSyntax>().Count() + 
                     root.DescendantNodes().OfType<EventFieldDeclarationSyntax>().Count(),
            MaxDepth = GetMaxDepth(root, 0)
        };

        Console.WriteLine($"Всего узлов: {stats.TotalNodes}");
        Console.WriteLine($"Классы: {stats.Classes}");
        Console.WriteLine($"Интерфейсы: {stats.Interfaces}");
        Console.WriteLine($"Методы: {stats.Methods}");
        Console.WriteLine($"Свойства: {stats.Properties}");
        Console.WriteLine($"Поля: {stats.Fields}");
        Console.WriteLine($"Конструкторы: {stats.Constructors}");
        Console.WriteLine($"События: {stats.Events}");
        Console.WriteLine($"Максимальная глубина: {stats.MaxDepth}");
    }

    private int GetMaxDepth(SyntaxNode node, int currentDepth)
    {
        var children = node.ChildNodes();
        if (!children.Any())
            return currentDepth;

        return children.Max(child => GetMaxDepth(child, currentDepth + 1));
    }
}
