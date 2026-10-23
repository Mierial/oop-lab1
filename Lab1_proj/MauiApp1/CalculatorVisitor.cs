using Antlr4.Runtime.Misc;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace MauiApp1
{
    public class CalculatorVisitor : LabCalculatorBaseVisitor<double>
    {
        private readonly Dictionary<string, Cell> _cells;

        public CalculatorVisitor(Dictionary<string, Cell> cells)
        {
            _cells = cells;
        }

        public override double VisitCompileUnit([NotNull] LabCalculatorParser.CompileUnitContext context)
        {
            return Visit(context.expression());
        }

        public override double VisitNumberExpr([NotNull] LabCalculatorParser.NumberExprContext context)
        {
            return double.Parse(context.NUMBER().GetText(), CultureInfo.InvariantCulture);
        }

        public override double VisitCellExpr([NotNull] LabCalculatorParser.CellExprContext context)
        {
            string name = context.IDENTIFIER().GetText().ToUpper();
            return _cells.TryGetValue(name, out var cell) ? cell.Value : 0;
        }

        public override double VisitParenthesesExpr([NotNull] LabCalculatorParser.ParenthesesExprContext context)
        {
            return Visit(context.expression());
        }


        public override double VisitUnaryExpr([NotNull] LabCalculatorParser.UnaryExprContext context)
        {
            double value = Visit(context.expression());
            return context.op.Type == LabCalculatorParser.SUB ? -value : value;
        }

        public override double VisitIncExpr([NotNull] LabCalculatorParser.IncExprContext context)
        {
            return Visit(context.expression()) + 1;
        }

   
        public override double VisitDecExpr([NotNull] LabCalculatorParser.DecExprContext context)
        {
            return Visit(context.expression()) - 1;
        }

  
        public override double VisitExponentExpr([NotNull] LabCalculatorParser.ExponentExprContext context)
        {
            return Math.Pow(Visit(context.left), Visit(context.right));
        }

 
        public override double VisitMultiplicativeExpr([NotNull] LabCalculatorParser.MultiplicativeExprContext context)
        {
            double left = Visit(context.left);
            double right = Visit(context.right);
            if (context.op.Type == LabCalculatorParser.DIV)
            {
                if (right == 0) throw new DivideByZeroException();
                return left / right;
            }
            return left * right;
        }

        public override double VisitAdditiveExpr([NotNull] LabCalculatorParser.AdditiveExprContext context)
        {
            double left = Visit(context.left);
            double right = Visit(context.right);
            return context.op.Type == LabCalculatorParser.ADD ? left + right : left - right;
        }
    }
}