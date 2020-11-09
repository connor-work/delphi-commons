/// Copyright 2020 Connor Roehricht (connor.work)
/// 
/// Licensed under the Apache License, Version 2.0 (the "License");
/// you may not use this file except in compliance with the License.
/// You may obtain a copy of the License at
/// 
///     http://www.apache.org/licenses/LICENSE-2.0
/// 
/// Unless required by applicable law or agreed to in writing, software
/// distributed under the License is distributed on an "AS IS" BASIS,
/// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
/// See the License for the specific language governing permissions and
/// limitations under the License.

using System.Linq;
using Google.Protobuf.Collections;

namespace Work.Connor.Delphi.Commons.CodeWriterExtensions
{
    /// <summary>
    /// Extensions to Delphi source code types for adaptation based on Delphi Commons.
    /// </summary>
    public static partial class SourceCodeExtensions
    {
        /// <summary>
        /// Include file name that defines compiler directives that signal if specific Delphi language features are supported by the current compiler
        /// </summary>
        private static readonly string compilerFeaturesInclude = "Work.Connor.Delphi.CompilerFeatures.inc";

        /// <summary>
        /// Compilation condition that tests whether the compiler supports unit scope names
        /// </summary>
        private static readonly CompilationCondition compilerSupportsUnitScopeNamesCondition = new CompilationCondition() { Symbol = "WORK_CONNOR_DELPHI_COMPILER_UNIT_SCOPE_NAMES" };

        /// <summary>
        /// Compilation condition that tests whether the compiler supports custom RTTI attributes
        /// </summary>
        private static readonly CompilationCondition compilerSupportsCustomAttributesCondition = new CompilationCondition() { Symbol = "WORK_CONNOR_DELPHI_COMPILER_CUSTOM_ATTRIBUTES" };

#pragma warning disable S4136 // Method overloads should be grouped together -> "AdaptForDelphiCommons" and "AddCompilerAbstraction" method order reflects order in protobuf schema here

        /// <summary>
        /// Adapts the Delphi source code of a unit based on Delphi Commons.
        /// This adds convenience features such as compiler feature abstraction.
        /// </summary>
        /// <param name="unit">The unit</param>
        public static void AdaptForDelphiCommons(this Unit unit)
        {
            if (!unit.IncludeFiles.Contains(compilerFeaturesInclude)) unit.IncludeFiles.Add(compilerFeaturesInclude);
            AddCompilerAbstractionToUsesClause(unit.Interface.UsesClause);
            foreach (InterfaceDeclaration declaration in unit.Interface.Declarations)
            {
                if (declaration.DeclarationCase == InterfaceDeclaration.DeclarationOneofCase.ClassDeclaration) AddCompilerAbstractionToAttributes(declaration.ClassDeclaration);
            }
            AddCompilerAbstractionToUsesClause(unit.Implementation.UsesClause);
        }

        /// <summary>
        /// Adds compiler feature abstraction to all unit references in a uses clause.
        /// </summary>
        /// <param name="usesClause">The uses clause</param>
        private static void AddCompilerAbstractionToUsesClause(RepeatedField<ConditionalUnitReference> usesClause)
        {
            foreach (ConditionalUnitReference unitReference in usesClause) AddCompilerAbstraction(unitReference);
        }

        /// <summary>
        ///  Adds compiler feature abstraction to a unit reference.
        /// </summary>
        /// <param name="unitReference">The unit reference</param>
        private static void AddCompilerAbstraction(ConditionalUnitReference unitReference)
        {
            if (unitReference.Condition is null
             && unitReference.Element.Unit.Namespace.Count > 0
             && unitReference.Element.Unit.Namespace[0] == "System")
            {
                unitReference.Condition = compilerSupportsUnitScopeNamesCondition.Clone();
                unitReference.AlternativeElement = new UnitReference()
                {
                    Unit = new UnitIdentifier()
                    {
                        Unit = unitReference.Element.Unit.Unit,
                        Namespace = { unitReference.Element.Unit.Namespace.Skip(1) }
                    }
                };
            }
        }

        /// <summary>
        /// Adds compiler feature abstraction to all attribute annotations of or nested within a class declaration.
        /// </summary>
        /// <param name="class">The class declaration</param>
        private static void AddCompilerAbstractionToAttributes(ClassDeclaration @class)
        {
            foreach (ConditionalAttributeAnnotation annotation in @class.AttributeAnnotations) AddCompilerAbstraction(annotation);
            foreach (ClassDeclarationNestedDeclaration declaration in @class.NestedDeclarations)
            {
                if (declaration.DeclarationCase == ClassDeclarationNestedDeclaration.DeclarationOneofCase.NestedTypeDeclaration
                 && declaration.NestedTypeDeclaration.DeclarationCase == NestedTypeDeclaration.DeclarationOneofCase.ClassDeclaration) AddCompilerAbstractionToAttributes(declaration.NestedTypeDeclaration.ClassDeclaration);
                if (declaration.DeclarationCase == ClassDeclarationNestedDeclaration.DeclarationOneofCase.Member) AddCompilerAbstractionToAttributes(declaration.Member);
            }
        }

        /// <summary>
        /// Adds compiler feature abstraction to all attribute annotations of a class member declaration
        /// </summary>
        /// <param name="member">The member declaration</param>
        private static void AddCompilerAbstractionToAttributes(ClassMemberDeclaration member)
        {
            foreach (ConditionalAttributeAnnotation annotation in member.AttributeAnnotations) AddCompilerAbstraction(annotation);
        }

        /// <summary>
        /// Adds compiler feature abstraction to an attribute annotation.
        /// </summary>
        /// <param name="annotation">The attribute annotation</param>
        private static void AddCompilerAbstraction(ConditionalAttributeAnnotation annotation)
        {
            if (annotation.Condition is null) annotation.Condition = compilerSupportsCustomAttributesCondition.Clone();
        }

        /// <summary>
        /// Adapts the Delphi source code of a program based on Delphi Commons.
        /// This adds convenience features such as compiler feature abstraction.
        /// </summary>
        /// <param name="program">The program</param>
        public static void AdaptForDelphiCommons(this Program program)
        {
            if (!program.IncludeFiles.Contains(compilerFeaturesInclude)) program.IncludeFiles.Add(compilerFeaturesInclude);
            AddCompilerAbstractionToUsesClause(program.UsesClause);
        }

#pragma warning restore S4136 // Method overloads should be grouped together

    }
}
