using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ObservableWrapper;

public class SubjectTypeMeta(ISymbol symbol, TypeSyntax parent)
{
    public static SubjectTypeMeta? TryCreate(ISymbol symbol, SyntaxNode node)
    {
        var parent = (node.Parent as VariableDeclarationSyntax)?.Type;

        if (parent == null)
        {
            return null;
        }

        var result = new SubjectTypeMeta(symbol, parent);

        if (!parent.ToString().Contains("Subject"))
        {
            return null;
        }

        return result;
    }

    public string GetNamespace()
    {
        return symbol.ContainingNamespace.ToString();
    }

    public string GetFullClassName()
    {
        return symbol.ContainingSymbol.ToString();
    }

    public string GetShortClassName()
    {
        return GetFullClassName()
            .Replace(GetNamespace() + ".", "");
    }

    public string GetTypeName()
    {
        return parent.ToString()
            .Replace("BehaviorSubject", "Observable")
            .Replace("Subject", "Observable");
    }

    public string GetObservablePropertyName()
    {
        var str = GetSubjectPropertyName()
            .Replace("_", "");
        return char.ToUpper(str[0]) + str.Substring(1);
    }

    public string GetSubjectPropertyName()
    {
        return symbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
    }
}

public static class SubjectEmit
{
    public static string? Emit(
        StringBuilder builder, 
        List<SubjectTypeMeta> typeMetas)
    {
        var common = typeMetas.First();
        if (common == null) return null;
        
        builder.AppendLine("using R3;");
        builder.AppendLine("");
        builder.AppendLine($"namespace {common.GetNamespace()} {{");
        builder.AppendLine($"public partial class {common.GetShortClassName()}");
        builder.AppendLine("{");
        
        foreach(var typeMeta in typeMetas)
        {
            builder.AppendLine($"public {typeMeta.GetTypeName()} {typeMeta.GetObservablePropertyName()} => {typeMeta.GetSubjectPropertyName()}.AsObservable();");
        }
        
        builder.AppendLine("}");
        builder.AppendLine("}");
        
        return common.GetFullClassName();
    }
}