using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.DotnetRuntime.Extensions;
using Microsoft.CodeAnalysis.Text;

namespace ObservableWrapper;

[Generator(LanguageNames.CSharp)]
public class SourceGenerator : IIncrementalGenerator
{

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterPostInitializationOutput(static x => SetAttribute(x));

        var provider = context.SyntaxProvider.ForAttributeWithMetadataName
            (
                context,
                "ObservableWrapperGenerator.ObservableWrapperAttribute",
                static (node, _) => node is VariableDeclaratorSyntax,
                static (cont, _) => cont
            )
            .Combine(context.CompilationProvider);

        context.RegisterSourceOutput(
            context.CompilationProvider.Combine(provider.Collect()),
            static (sourceProductionContext, t) =>
            {
                var (_, list) = t;

                var typeMetas = new List<SubjectTypeMeta>();

                foreach (var (x, y) in list)
                {
                    var typeMeta = SubjectTypeMeta.TryCreate(x.TargetSymbol, x.TargetNode);
                    if (typeMeta != null) typeMetas.Add(typeMeta);
                }
                
                var generatedClassNames = new List<string>();

                foreach(var typeMeta in typeMetas)
                {
                    var fullClassName = typeMeta.GetFullClassName();
                    
                    if (generatedClassNames.Contains(fullClassName)) continue;
                    
                    var commonTypeMetas = typeMetas
                        .Where(x => x.GetFullClassName() == fullClassName)
                        .ToList();

                    var builder = new StringBuilder();
                    var fileName = SubjectEmit.Emit(builder, commonTypeMetas);

                    if (fileName != null)
                    {
                        sourceProductionContext.AddSource(
                            $"{fileName}.g.cs",
                            SourceText.From(builder.ToString(), Encoding.UTF8)
                        );
                        
                        generatedClassNames.Add(fullClassName);
                    }

                    builder.Clear();
                }
            });
    }
    
    private static void SetAttribute(IncrementalGeneratorPostInitializationContext context)
    { 
        const string attributeText = """
                                   using System;

                                   namespace ObservableWrapperGenerator
                                   {
                                      [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
                                      sealed class ObservableWrapperAttribute : Attribute {
                                          public ObservableWrapperAttribute() {}
                                      }
                                   }
                                   """;                
        context.AddSource
        (
            "ObservableWrapperAttribute.cs",
            SourceText.From(attributeText, Encoding.UTF8)
        );
    }
}