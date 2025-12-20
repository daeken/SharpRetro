using ParserCombinators;
using System.Collections.Generic;
namespace SwipcParser;
public partial class Parser {
	public partial class Start : Parser {
		public List<Parser.Def> Defs;
	}
	public partial class Number : Parser {
		public string Value;
	}
	public partial class Def : Parser {
		public Parser Value;
	}
	public partial class Expression : Parser {
		public Parser Value;
	}
	public partial class Name : Parser {
		public string Value;
	}
	public partial class SName : Parser {
		public string Value;
	}
	public partial class ServiceNameList : Parser {
		public List<Parser.SName> Head;
		public Parser.SName Tail;
	}
	public partial class Template : Parser {
		public List<Parser.Expression> Head;
		public Parser.Expression Tail;
	}
	public partial class ArrayLength : Parser {
		public Parser.Number length;
	}
	public partial class StructField : Parser {
		public List<Parser.Comment> doc;
		public Parser.Type type;
		public Parser.Name name;
	}
	public partial class EnumField : Parser {
		public List<Parser.Comment> doc;
		public Parser.Name name;
		public Parser.Number value;
	}
	public partial class StructType : Parser {
		public Parser.Template template;
		public List<Parser.StructField> structFields;
	}
	public partial class EnumType : Parser {
		public Parser.Template template;
		public List<Parser.EnumField> enumFields;
	}
	public partial class ConcreteType : Parser {
		public Parser.Name name;
		public Parser.Template template;
		public Parser.ArrayLength length;
	}
	public partial class Type : Parser {
		public Parser Value;
	}
	public partial class TypeDef : Parser {
		public List<Parser.Comment> doc;
		public List<Parser> decorators;
		public Parser.Type name;
		public Parser.Type type;
	}
	public partial class Interface : Parser {
		public List<Parser.Comment> doc;
		public List<Parser> decorators;
		public Parser.Name name;
		public Parser.ServiceNameList serviceNames;
		public List<Parser.FuncDef> functions;
	}
	public partial class NamedType : Parser {
		public (Parser.Type, Parser.Name) Value;
	}
	public partial class NamedTuple : Parser {
		public List<Parser.NamedType> Head;
		public Parser.NamedType Tail;
	}
	public partial class Comment : Parser {
		public string line;
	}
	public partial class Range : Parser {
		public (Parser.Number, string, Parser.Number, string, Parser.Number) start;
		public (Parser.Number, string, Parser.Number, string, Parser.Number) end;
	}
	public partial class VersionNumber : Parser {
		public (Parser.Number, string, Parser.Number, string, Parser.Number) Value;
	}
	public partial class Ongoing : Parser {
		public string Value;
	}
	public partial class Ended : Parser {
		public Parser.VersionNumber version;
	}
	public partial class DecoratorType : Parser {
		public string type;
		public Parser.VersionNumber startVersion;
		public Parser postfix;
	}
	public partial class FuncDef : Parser {
		public List<Parser.Comment> doc;
		public List<Parser> decorators;
		public Parser.Number cmdId;
		public Parser.Name name;
		public Parser.NamedTuple inputs;
		public Parser outputs;
	}
	static readonly Grammar Grammar;
	static Parser() {
		var (_Type, __Type_body) = Patterns.Forward();
		var _Comment = Patterns.Memoize(Patterns.Bind<Parser.Comment>(Patterns.TupleLooseSequence([typeof(string), typeof(string)], Patterns.IgnoreLeadingWhitespace(Patterns.Literal("#")), Patterns.With<Parser.Comment>((x, d) => x.line = (string) d, Patterns.IgnoreLeadingWhitespace(Patterns.Regex("^[^\n]*"))))));
		var _Number = Patterns.Memoize(Patterns.Bind<Parser.Number>(Patterns.With<Parser.Number>((x, d) => x.Value = (string) d, Patterns.IgnoreLeadingWhitespace(Patterns.Choice(Patterns.IgnoreLeadingWhitespace(Patterns.Regex("^0x[0-9a-fA-F]+")), Patterns.IgnoreLeadingWhitespace(Patterns.Regex("^[0-9]+")))))));
		var _VersionNumber = Patterns.Memoize(Patterns.Bind<Parser.VersionNumber>(Patterns.With<Parser.VersionNumber>((x, d) => x.Value = ((Parser.Number, string, Parser.Number, string, Parser.Number)) d, Patterns.TupleLooseSequence([typeof(Parser.Number), typeof(string), typeof(Parser.Number), typeof(string), typeof(Parser.Number)], _Number, Patterns.IgnoreLeadingWhitespace(Patterns.Literal(".")), _Number, Patterns.IgnoreLeadingWhitespace(Patterns.Literal(".")), _Number))));
		var _Ongoing = Patterns.Memoize(Patterns.Bind<Parser.Ongoing>(Patterns.With<Parser.Ongoing>((x, d) => x.Value = (string) d, Patterns.IgnoreLeadingWhitespace(Patterns.Literal("+")))));
		var _Ended = Patterns.Memoize(Patterns.Bind<Parser.Ended>(Patterns.TupleLooseSequence([typeof(string), typeof(Parser.VersionNumber)], Patterns.IgnoreLeadingWhitespace(Patterns.Literal("-")), Patterns.With<Parser.Ended>((x, d) => x.version = (Parser.VersionNumber) d, _VersionNumber))));
		var _DecoratorType = Patterns.Memoize(Patterns.Bind<Parser.DecoratorType>(Patterns.IgnoreLeadingWhitespace(Patterns.Choice(Patterns.TupleLooseSequence([typeof(string), typeof(string), typeof(Parser.VersionNumber), typeof(Parser), typeof(string)], Patterns.With<Parser.DecoratorType>((x, d) => x.type = (string) d, Patterns.IgnoreLeadingWhitespace(Patterns.Literal("version"))), Patterns.IgnoreLeadingWhitespace(Patterns.Literal("(")), Patterns.With<Parser.DecoratorType>((x, d) => x.startVersion = (Parser.VersionNumber) d, _VersionNumber), Patterns.With<Parser.DecoratorType>((x, d) => x.postfix = (Parser) d, Patterns.IgnoreLeadingWhitespace(Patterns.Optional(Patterns.IgnoreLeadingWhitespace(Patterns.Choice(_Ongoing, _Ended))))), Patterns.IgnoreLeadingWhitespace(Patterns.Literal(")"))), Patterns.With<Parser.DecoratorType>((x, d) => x.type = (string) d, Patterns.IgnoreLeadingWhitespace(Patterns.Literal("undocumented")))))));
		var _Decorator = Patterns.Memoize(Patterns.PopValue(Patterns.TupleLooseSequence([typeof(string), typeof(Parser.DecoratorType)], Patterns.IgnoreLeadingWhitespace(Patterns.Literal("@")), Patterns.PushValue(_DecoratorType))));
		var _Expression = Patterns.Memoize(Patterns.Bind<Parser.Expression>(Patterns.With<Parser.Expression>((x, d) => x.Value = (Parser) d, Patterns.IgnoreLeadingWhitespace(Patterns.Choice(_Type, _Number)))));
		var _SubTemplate = Patterns.Memoize(Patterns.PopValue(Patterns.TupleLooseSequence([typeof(Parser.Expression), typeof(string)], Patterns.PushValue(_Expression), Patterns.IgnoreLeadingWhitespace(Patterns.Literal(",")))));
		var _Template = Patterns.Memoize(Patterns.Bind<Parser.Template>(Patterns.TupleLooseSequence([typeof(string), typeof(List<Parser.Expression>), typeof(Parser.Expression), typeof(string)], Patterns.IgnoreLeadingWhitespace(Patterns.Literal("<")), Patterns.With<Parser.Template>((x, d) => x.Head = (List<Parser.Expression>) d, Patterns.ZeroOrMore<Parser.Expression>(Patterns.IgnoreLeadingWhitespace(_SubTemplate))), Patterns.With<Parser.Template>((x, d) => x.Tail = (Parser.Expression) d, _Expression), Patterns.IgnoreLeadingWhitespace(Patterns.Literal(">")))));
		var _Name = Patterns.Memoize(Patterns.Bind<Parser.Name>(Patterns.With<Parser.Name>((x, d) => x.Value = (string) d, Patterns.IgnoreLeadingWhitespace(Patterns.Regex("^[a-zA-Z_][a-zA-Z0-9_:]*")))));
		var _StructField = Patterns.Memoize(Patterns.Bind<Parser.StructField>(Patterns.TupleLooseSequence([typeof(List<Parser.Comment>), typeof(Parser.Type), typeof(Parser.Name), typeof(string)], Patterns.With<Parser.StructField>((x, d) => x.doc = (List<Parser.Comment>) d, Patterns.ZeroOrMore<Parser.Comment>(Patterns.IgnoreLeadingWhitespace(_Comment))), Patterns.With<Parser.StructField>((x, d) => x.type = (Parser.Type) d, _Type), Patterns.With<Parser.StructField>((x, d) => x.name = (Parser.Name) d, _Name), Patterns.IgnoreLeadingWhitespace(Patterns.Literal(";")))));
		var _StructType = Patterns.Memoize(Patterns.Bind<Parser.StructType>(Patterns.TupleLooseSequence([typeof(string), typeof(Parser.Template), typeof(string), typeof(List<Parser.StructField>), typeof(string)], Patterns.IgnoreLeadingWhitespace(Patterns.Literal("struct")), Patterns.With<Parser.StructType>((x, d) => x.template = (Parser.Template) d, Patterns.IgnoreLeadingWhitespace(Patterns.Optional(_Template))), Patterns.IgnoreLeadingWhitespace(Patterns.Literal("{")), Patterns.With<Parser.StructType>((x, d) => x.structFields = (List<Parser.StructField>) d, Patterns.OneOrMore<Parser.StructField>(Patterns.IgnoreLeadingWhitespace(_StructField))), Patterns.IgnoreLeadingWhitespace(Patterns.Literal("}")))));
		var _EnumField = Patterns.Memoize(Patterns.Bind<Parser.EnumField>(Patterns.TupleLooseSequence([typeof(List<Parser.Comment>), typeof(Parser.Name), typeof(string), typeof(Parser.Number), typeof(string)], Patterns.With<Parser.EnumField>((x, d) => x.doc = (List<Parser.Comment>) d, Patterns.ZeroOrMore<Parser.Comment>(Patterns.IgnoreLeadingWhitespace(_Comment))), Patterns.With<Parser.EnumField>((x, d) => x.name = (Parser.Name) d, _Name), Patterns.IgnoreLeadingWhitespace(Patterns.Literal("=")), Patterns.With<Parser.EnumField>((x, d) => x.value = (Parser.Number) d, _Number), Patterns.IgnoreLeadingWhitespace(Patterns.Literal(";")))));
		var _EnumType = Patterns.Memoize(Patterns.Bind<Parser.EnumType>(Patterns.TupleLooseSequence([typeof(string), typeof(Parser.Template), typeof(string), typeof(List<Parser.EnumField>), typeof(string)], Patterns.IgnoreLeadingWhitespace(Patterns.Literal("enum")), Patterns.With<Parser.EnumType>((x, d) => x.template = (Parser.Template) d, _Template), Patterns.IgnoreLeadingWhitespace(Patterns.Literal("{")), Patterns.With<Parser.EnumType>((x, d) => x.enumFields = (List<Parser.EnumField>) d, Patterns.OneOrMore<Parser.EnumField>(Patterns.IgnoreLeadingWhitespace(_EnumField))), Patterns.IgnoreLeadingWhitespace(Patterns.Literal("}")))));
		var _ArrayLength = Patterns.Memoize(Patterns.Bind<Parser.ArrayLength>(Patterns.TupleLooseSequence([typeof(string), typeof(Parser.Number), typeof(string)], Patterns.IgnoreLeadingWhitespace(Patterns.Literal("[")), Patterns.IgnoreLeadingWhitespace(Patterns.Optional(Patterns.With<Parser.ArrayLength>((x, d) => x.length = (Parser.Number) d, _Number))), Patterns.IgnoreLeadingWhitespace(Patterns.Literal("]")))));
		var _ConcreteType = Patterns.Memoize(Patterns.Bind<Parser.ConcreteType>(Patterns.TupleLooseSequence([typeof(Parser.Name), typeof(Parser.Template), typeof(Parser.ArrayLength)], Patterns.With<Parser.ConcreteType>((x, d) => x.name = (Parser.Name) d, _Name), Patterns.With<Parser.ConcreteType>((x, d) => x.template = (Parser.Template) d, Patterns.IgnoreLeadingWhitespace(Patterns.Optional(_Template))), Patterns.With<Parser.ConcreteType>((x, d) => x.length = (Parser.ArrayLength) d, Patterns.IgnoreLeadingWhitespace(Patterns.Optional(_ArrayLength))))));
		__Type_body.Value = Patterns.Memoize(Patterns.Bind<Parser.Type>(Patterns.With<Parser.Type>((x, d) => x.Value = (Parser) d, Patterns.IgnoreLeadingWhitespace(Patterns.Choice(_StructType, _EnumType, _ConcreteType)))));
		var _TypeDef = Patterns.Memoize(Patterns.Bind<Parser.TypeDef>(Patterns.TupleLooseSequence([typeof(List<Parser.Comment>), typeof(List<Parser>), typeof(string), typeof(Parser.Type), typeof(string), typeof(Parser.Type), typeof(string)], Patterns.With<Parser.TypeDef>((x, d) => x.doc = (List<Parser.Comment>) d, Patterns.ZeroOrMore<Parser.Comment>(Patterns.IgnoreLeadingWhitespace(_Comment))), Patterns.With<Parser.TypeDef>((x, d) => x.decorators = (List<Parser>) d, Patterns.ZeroOrMore<Parser>(Patterns.IgnoreLeadingWhitespace(_Decorator))), Patterns.IgnoreLeadingWhitespace(Patterns.Literal("type")), Patterns.With<Parser.TypeDef>((x, d) => x.name = (Parser.Type) d, _Type), Patterns.IgnoreLeadingWhitespace(Patterns.Literal("=")), Patterns.With<Parser.TypeDef>((x, d) => x.type = (Parser.Type) d, _Type), Patterns.IgnoreLeadingWhitespace(Patterns.Literal(";")))));
		var _SName = Patterns.Memoize(Patterns.Bind<Parser.SName>(Patterns.With<Parser.SName>((x, d) => x.Value = (string) d, Patterns.IgnoreLeadingWhitespace(Patterns.Regex("^[a-zA-Z_][a-zA-Z0-9_:\\-]*")))));
		var _SubServiceNameList = Patterns.Memoize(Patterns.PopValue(Patterns.TupleLooseSequence([typeof(Parser.SName), typeof(string)], Patterns.PushValue(_SName), Patterns.IgnoreLeadingWhitespace(Patterns.Literal(",")))));
		var _ServiceNameList = Patterns.Memoize(Patterns.Bind<Parser.ServiceNameList>(Patterns.TupleLooseSequence([typeof(List<Parser.SName>), typeof(Parser.SName)], Patterns.With<Parser.ServiceNameList>((x, d) => x.Head = (List<Parser.SName>) d, Patterns.ZeroOrMore<Parser.SName>(Patterns.IgnoreLeadingWhitespace(_SubServiceNameList))), Patterns.With<Parser.ServiceNameList>((x, d) => x.Tail = (Parser.SName) d, _SName))));
		var _NamedType = Patterns.Memoize(Patterns.Bind<Parser.NamedType>(Patterns.With<Parser.NamedType>((x, d) => x.Value = ((Parser.Type, Parser.Name)) d, Patterns.TupleLooseSequence([typeof(Parser.Type), typeof(Parser.Name)], _Type, Patterns.IgnoreLeadingWhitespace(Patterns.Optional(_Name))))));
		var _SubNamedTuple = Patterns.Memoize(Patterns.PopValue(Patterns.TupleLooseSequence([typeof(Parser.NamedType), typeof(string)], Patterns.PushValue(_NamedType), Patterns.IgnoreLeadingWhitespace(Patterns.Literal(",")))));
		var _NamedTuple = Patterns.Memoize(Patterns.Bind<Parser.NamedTuple>(Patterns.TupleLooseSequence([typeof(string), typeof(List<Parser.NamedType>), typeof(Parser.NamedType), typeof(string)], Patterns.IgnoreLeadingWhitespace(Patterns.Literal("(")), Patterns.With<Parser.NamedTuple>((x, d) => x.Head = (List<Parser.NamedType>) d, Patterns.ZeroOrMore<Parser.NamedType>(Patterns.IgnoreLeadingWhitespace(_SubNamedTuple))), Patterns.With<Parser.NamedTuple>((x, d) => x.Tail = (Parser.NamedType) d, Patterns.IgnoreLeadingWhitespace(Patterns.Optional(_NamedType))), Patterns.IgnoreLeadingWhitespace(Patterns.Literal(")")))));
		var _FuncDef = Patterns.Memoize(Patterns.Bind<Parser.FuncDef>(Patterns.TupleLooseSequence([typeof(List<Parser.Comment>), typeof(List<Parser>), typeof(string), typeof(Parser.Number), typeof(string), typeof(Parser.Name), typeof(Parser.NamedTuple), typeof((string, Parser)), typeof(string)], Patterns.With<Parser.FuncDef>((x, d) => x.doc = (List<Parser.Comment>) d, Patterns.ZeroOrMore<Parser.Comment>(Patterns.IgnoreLeadingWhitespace(_Comment))), Patterns.With<Parser.FuncDef>((x, d) => x.decorators = (List<Parser>) d, Patterns.ZeroOrMore<Parser>(Patterns.IgnoreLeadingWhitespace(_Decorator))), Patterns.IgnoreLeadingWhitespace(Patterns.Literal("[")), Patterns.With<Parser.FuncDef>((x, d) => x.cmdId = (Parser.Number) d, _Number), Patterns.IgnoreLeadingWhitespace(Patterns.Literal("]")), Patterns.With<Parser.FuncDef>((x, d) => x.name = (Parser.Name) d, _Name), Patterns.With<Parser.FuncDef>((x, d) => x.inputs = (Parser.NamedTuple) d, _NamedTuple), Patterns.IgnoreLeadingWhitespace(Patterns.Optional(Patterns.TupleLooseSequence([typeof(string), typeof(Parser)], Patterns.IgnoreLeadingWhitespace(Patterns.Literal("->")), Patterns.With<Parser.FuncDef>((x, d) => x.outputs = (Parser) d, Patterns.IgnoreLeadingWhitespace(Patterns.Choice(_NamedType, _NamedTuple)))))), Patterns.IgnoreLeadingWhitespace(Patterns.Literal(";")))));
		var _Interface = Patterns.Memoize(Patterns.Bind<Parser.Interface>(Patterns.TupleLooseSequence([typeof(List<Parser.Comment>), typeof(List<Parser>), typeof(string), typeof(Parser.Name), typeof((string, Parser.ServiceNameList)), typeof(string), typeof(List<Parser.FuncDef>), typeof(string)], Patterns.With<Parser.Interface>((x, d) => x.doc = (List<Parser.Comment>) d, Patterns.ZeroOrMore<Parser.Comment>(Patterns.IgnoreLeadingWhitespace(_Comment))), Patterns.With<Parser.Interface>((x, d) => x.decorators = (List<Parser>) d, Patterns.ZeroOrMore<Parser>(Patterns.IgnoreLeadingWhitespace(_Decorator))), Patterns.IgnoreLeadingWhitespace(Patterns.Literal("interface")), Patterns.With<Parser.Interface>((x, d) => x.name = (Parser.Name) d, _Name), Patterns.IgnoreLeadingWhitespace(Patterns.Optional(Patterns.TupleLooseSequence([typeof(string), typeof(Parser.ServiceNameList)], Patterns.IgnoreLeadingWhitespace(Patterns.Literal("is")), Patterns.With<Parser.Interface>((x, d) => x.serviceNames = (Parser.ServiceNameList) d, _ServiceNameList)))), Patterns.IgnoreLeadingWhitespace(Patterns.Literal("{")), Patterns.With<Parser.Interface>((x, d) => x.functions = (List<Parser.FuncDef>) d, Patterns.ZeroOrMore<Parser.FuncDef>(Patterns.IgnoreLeadingWhitespace(_FuncDef))), Patterns.IgnoreLeadingWhitespace(Patterns.Literal("}")))));
		var _Def = Patterns.Memoize(Patterns.Bind<Parser.Def>(Patterns.With<Parser.Def>((x, d) => x.Value = (Parser) d, Patterns.IgnoreLeadingWhitespace(Patterns.Choice(_TypeDef, _Interface)))));
		var _Start = Patterns.Memoize(Patterns.Bind<Parser.Start>(Patterns.TupleLooseSequence([typeof(List<Parser.Def>), typeof(object)], Patterns.With<Parser.Start>((x, d) => x.Defs = (List<Parser.Def>) d, Patterns.OneOrMore<Parser.Def>(Patterns.IgnoreLeadingWhitespace(_Def))), Patterns.IgnoreLeadingWhitespace(Patterns.End))));
		Grammar = new Grammar(_Start);
	}
	public static Parser.Start Parse(string input) => (Parser.Start) Grammar.Parse(input);
}
