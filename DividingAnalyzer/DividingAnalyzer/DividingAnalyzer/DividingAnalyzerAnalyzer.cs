using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;

namespace DividingAnalyzer
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class DividingAnalyzerAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "DividingAnalyzer";

        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(Resources.AnalyzerTitle), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(Resources.AnalyzerMessageFormat), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(Resources.AnalyzerDescription), Resources.ResourceManager, typeof(Resources));
        private const string Category = "Usage";

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Error, isEnabledByDefault: true, description: Description);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Rule); } }

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();

            context.RegisterSyntaxNodeAction(AnalyzeNode, SyntaxKind.DivideExpression);
            
        }

        private static void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            var divisionExpression = (BinaryExpressionSyntax)context.Node;

            /* Первый способ: 
             * 1) ищем узел, являющийся инструкцией деления,
             * 2) пытаемся обратиться к его листьям : делимое, символ /, делитель, и выбираем делитель (т.е. последний лист)
             * 3) если лист-делитель получен успешно, то проверяем значение токена листа-делителя на неравенство нулю - такие случае нас не интересуют
             * 4) далее создаём диагностическое сообщение, информируем об ошибке
             */
            var denominatorNode = divisionExpression.ChildNodes().Where(n => n.IsKind(SyntaxKind.NumericLiteralExpression)).LastOrDefault() as LiteralExpressionSyntax;
            if (denominatorNode == null) return;
            if (denominatorNode.Token.ValueText != "0") return;

            /* Второй способ (более эффективный):
             * используем то, что деление - бинарная операция, поэтому
             * сразу обращаемся к правой подветви инструкции с делением (т.е. к делителю) и проверяем его значение на неравенство "0"
             *
            if (divisionExpression.Right.ChildTokens().LastOrDefault().ValueText != "0") return;
             */
            var diagnostic = Diagnostic.Create(Rule, divisionExpression.GetLocation());
            context.ReportDiagnostic(diagnostic);
        }
    }
}
