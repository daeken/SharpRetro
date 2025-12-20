using ParserCombinators;

namespace SwipcParser;
public partial class Parser {
	internal class Start : Parser {
		internal List<Def> Defs;
	}
	internal class Number : Parser {
		internal string Value;
	}
	internal class Def : Parser {
		internal Parser Value;
	}
	internal class Expression : Parser {
		internal Parser Value;
	}
	internal class Name : Parser {
		internal string Value;
	}
	internal class SName : Parser {
		internal string Value;
	}
	internal class ServiceNameList : Parser {
		internal List<SName> Head;
		internal SName Tail;
	}
	internal class Template : Parser {
		internal List<Expression> Head;
		internal Expression Tail;
	}
	internal class ArrayLength : Parser {
		internal Number length;
	}
	internal class StructField : Parser {
		internal List<Comment> doc;
		internal Type type;
		internal Name name;
	}
	internal class EnumField : Parser {
		internal List<Comment> doc;
		internal Name name;
		internal Number value;
	}
	internal class StructType : Parser {
		internal Template template;
		internal List<StructField> structFields;
	}
	internal class EnumType : Parser {
		internal Template template;
		internal List<EnumField> enumFields;
	}
	internal class ConcreteType : Parser {
		internal Name name;
		internal Template template;
		internal ArrayLength length;
	}
	internal class Type : Parser {
		internal Parser Value;
	}
	internal class TypeDef : Parser {
		internal List<Comment> doc;
		internal List<Parser> decorators;
		internal Name name;
		internal Type type;
	}
	internal class Interface : Parser {
		internal List<Comment> doc;
		internal List<Parser> decorators;
		internal Name name;
		internal ServiceNameList serviceNames;
		internal List<FuncDef> functions;
	}
	internal class NamedType : Parser {
		internal (Type, Name) Value;
	}
	internal class NamedTuple : Parser {
		internal List<NamedType> Head;
		internal NamedType Tail;
	}
	internal class Comment : Parser {
		internal string line;
	}
	internal class Range : Parser {
		internal (Number, string, Number, string, Number) start;
		internal (Number, string, Number, string, Number) end;
	}
	internal class VersionNumber : Parser {
		internal (Number, string, Number, string, Number) Value;
	}
	internal class Ongoing : Parser {
		internal string Value;
	}
	internal class Ended : Parser {
		internal VersionNumber version;
	}
	internal class DecoratorType : Parser {
		internal string type;
		internal VersionNumber startVersion;
		internal Parser postfix;
	}
	internal class FuncDef : Parser {
		internal List<Comment> doc;
		internal List<Parser> decorators;
		internal Number cmdId;
		internal Name name;
		internal NamedTuple inputs;
		internal Parser outputs;
	}
	static readonly Grammar Grammar;
	static Parser() {
		var (_Type, __Type_body) = Patterns.Forward();
		var _Comment = Patterns.Memoize(Patterns.Bind<Comment>(Patterns.TupleLooseSequence([typeof(string), typeof(string)], Patterns.IgnoreLeadingWhitespace(Patterns.Literal("#")), Patterns.With<Comment>((x, d) => x.line = d, Patterns.IgnoreLeadingWhitespace(Patterns.Regex("^[^\n]*"))))));
		var _Number = Patterns.Memoize(Patterns.Bind<Number>(Patterns.With<Number>((x, d) => x.Value = d, Patterns.IgnoreLeadingWhitespace(Patterns.Choice(Patterns.IgnoreLeadingWhitespace(Patterns.Regex("^0x[0-9a-fA-F]+")), Patterns.IgnoreLeadingWhitespace(Patterns.Regex("^[0-9]+")))))));
		var _VersionNumber = Patterns.Memoize(Patterns.Bind<VersionNumber>(Patterns.With<VersionNumber>((x, d) => x.Value = d, Patterns.TupleLooseSequence([typeof(Number), typeof(string), typeof(Number), typeof(string), typeof(Number)], _Number, Patterns.IgnoreLeadingWhitespace(Patterns.Literal(".")), _Number, Patterns.IgnoreLeadingWhitespace(Patterns.Literal(".")), _Number))));
		var _Ongoing = Patterns.Memoize(Patterns.Bind<Ongoing>(Patterns.With<Ongoing>((x, d) => x.Value = d, Patterns.IgnoreLeadingWhitespace(Patterns.Literal("+")))));
		var _Ended = Patterns.Memoize(Patterns.Bind<Ended>(Patterns.TupleLooseSequence([typeof(string), typeof(VersionNumber)], Patterns.IgnoreLeadingWhitespace(Patterns.Literal("-")), Patterns.With<Ended>((x, d) => x.version = d, _VersionNumber))));
		var _DecoratorType = Patterns.Memoize(Patterns.Bind<DecoratorType>(Patterns.IgnoreLeadingWhitespace(Patterns.Choice(Patterns.TupleLooseSequence([typeof(string), typeof(string), typeof(VersionNumber), typeof(Parser), typeof(string)], Patterns.With<DecoratorType>((x, d) => x.type = d, Patterns.IgnoreLeadingWhitespace(Patterns.Literal("version"))), Patterns.IgnoreLeadingWhitespace(Patterns.Literal("(")), Patterns.With<DecoratorType>((x, d) => x.startVersion = d, _VersionNumber), Patterns.With<DecoratorType>((x, d) => x.postfix = d, Patterns.IgnoreLeadingWhitespace(Patterns.Optional(Patterns.IgnoreLeadingWhitespace(Patterns.Choice(_Ongoing, _Ended))))), Patterns.IgnoreLeadingWhitespace(Patterns.Literal(")"))), Patterns.With<DecoratorType>((x, d) => x.type = d, Patterns.IgnoreLeadingWhitespace(Patterns.Literal("undocumented")))))));
		var _Decorator = Patterns.Memoize(Patterns.PopValue(Patterns.TupleLooseSequence([typeof(string), typeof(DecoratorType)], Patterns.IgnoreLeadingWhitespace(Patterns.Literal("@")), Patterns.PushValue(_DecoratorType))));
		var _Name = Patterns.Memoize(Patterns.Bind<Name>(Patterns.With<Name>((x, d) => x.Value = d, Patterns.IgnoreLeadingWhitespace(Patterns.Regex("^[a-zA-Z_][a-zA-Z0-9_:]*")))));
		var _Expression = Patterns.Memoize(Patterns.Bind<Expression>(Patterns.With<Expression>((x, d) => x.Value = d, Patterns.IgnoreLeadingWhitespace(Patterns.Choice(_Type, _Number)))));
		var _SubTemplate = Patterns.Memoize(Patterns.PopValue(Patterns.TupleLooseSequence([typeof(Expression), typeof(string)], Patterns.PushValue(_Expression), Patterns.IgnoreLeadingWhitespace(Patterns.Literal(",")))));
		var _Template = Patterns.Memoize(Patterns.Bind<Template>(Patterns.TupleLooseSequence([typeof(string), typeof(List<Expression>), typeof(Expression), typeof(string)], Patterns.IgnoreLeadingWhitespace(Patterns.Literal("<")), Patterns.With<Template>((x, d) => x.Head = d, Patterns.ZeroOrMore<Expression>(Patterns.IgnoreLeadingWhitespace(_SubTemplate))), Patterns.With<Template>((x, d) => x.Tail = d, _Expression), Patterns.IgnoreLeadingWhitespace(Patterns.Literal(">")))));
		var _StructField = Patterns.Memoize(Patterns.Bind<StructField>(Patterns.TupleLooseSequence([typeof(List<Comment>), typeof(Type), typeof(Name), typeof(string)], Patterns.With<StructField>((x, d) => x.doc = d, Patterns.ZeroOrMore<Comment>(Patterns.IgnoreLeadingWhitespace(_Comment))), Patterns.With<StructField>((x, d) => x.type = d, _Type), Patterns.With<StructField>((x, d) => x.name = d, _Name), Patterns.IgnoreLeadingWhitespace(Patterns.Literal(";")))));
		var _StructType = Patterns.Memoize(Patterns.Bind<StructType>(Patterns.TupleLooseSequence([typeof(string), typeof(Template), typeof(string), typeof(List<StructField>), typeof(string)], Patterns.IgnoreLeadingWhitespace(Patterns.Literal("struct")), Patterns.With<StructType>((x, d) => x.template = d, Patterns.IgnoreLeadingWhitespace(Patterns.Optional(_Template))), Patterns.IgnoreLeadingWhitespace(Patterns.Literal("{")), Patterns.With<StructType>((x, d) => x.structFields = d, Patterns.OneOrMore<StructField>(Patterns.IgnoreLeadingWhitespace(_StructField))), Patterns.IgnoreLeadingWhitespace(Patterns.Literal("}")))));
		var _EnumField = Patterns.Memoize(Patterns.Bind<EnumField>(Patterns.TupleLooseSequence([typeof(List<Comment>), typeof(Name), typeof(string), typeof(Number), typeof(string)], Patterns.With<EnumField>((x, d) => x.doc = d, Patterns.ZeroOrMore<Comment>(Patterns.IgnoreLeadingWhitespace(_Comment))), Patterns.With<EnumField>((x, d) => x.name = d, _Name), Patterns.IgnoreLeadingWhitespace(Patterns.Literal("=")), Patterns.With<EnumField>((x, d) => x.value = d, _Number), Patterns.IgnoreLeadingWhitespace(Patterns.Literal(";")))));
		var _EnumType = Patterns.Memoize(Patterns.Bind<EnumType>(Patterns.TupleLooseSequence([typeof(string), typeof(Template), typeof(string), typeof(List<EnumField>), typeof(string)], Patterns.IgnoreLeadingWhitespace(Patterns.Literal("enum")), Patterns.With<EnumType>((x, d) => x.template = d, _Template), Patterns.IgnoreLeadingWhitespace(Patterns.Literal("{")), Patterns.With<EnumType>((x, d) => x.enumFields = d, Patterns.OneOrMore<EnumField>(Patterns.IgnoreLeadingWhitespace(_EnumField))), Patterns.IgnoreLeadingWhitespace(Patterns.Literal("}")))));
		var _ArrayLength = Patterns.Memoize(Patterns.Bind<ArrayLength>(Patterns.TupleLooseSequence([typeof(string), typeof(Number), typeof(string)], Patterns.IgnoreLeadingWhitespace(Patterns.Literal("[")), Patterns.IgnoreLeadingWhitespace(Patterns.Optional(Patterns.With<ArrayLength>((x, d) => x.length = d, _Number))), Patterns.IgnoreLeadingWhitespace(Patterns.Literal("]")))));
		var _ConcreteType = Patterns.Memoize(Patterns.Bind<ConcreteType>(Patterns.TupleLooseSequence([typeof(Name), typeof(Template), typeof(ArrayLength)], Patterns.With<ConcreteType>((x, d) => x.name = d, _Name), Patterns.With<ConcreteType>((x, d) => x.template = d, Patterns.IgnoreLeadingWhitespace(Patterns.Optional(_Template))), Patterns.With<ConcreteType>((x, d) => x.length = d, Patterns.IgnoreLeadingWhitespace(Patterns.Optional(_ArrayLength))))));
		__Type_body.Value = Patterns.Memoize(Patterns.Bind<Type>(Patterns.With<Type>((x, d) => x.Value = d, Patterns.IgnoreLeadingWhitespace(Patterns.Choice(_StructType, _EnumType, _ConcreteType)))));
		var _TypeDef = Patterns.Memoize(Patterns.Bind<TypeDef>(Patterns.TupleLooseSequence([typeof(List<Comment>), typeof(List<Parser>), typeof(string), typeof(Name), typeof(string), typeof(Type), typeof(string)], Patterns.With<TypeDef>((x, d) => x.doc = d, Patterns.ZeroOrMore<Comment>(Patterns.IgnoreLeadingWhitespace(_Comment))), Patterns.With<TypeDef>((x, d) => x.decorators = d, Patterns.ZeroOrMore<Parser>(Patterns.IgnoreLeadingWhitespace(_Decorator))), Patterns.IgnoreLeadingWhitespace(Patterns.Literal("type")), Patterns.With<TypeDef>((x, d) => x.name = d, _Name), Patterns.IgnoreLeadingWhitespace(Patterns.Literal("=")), Patterns.With<TypeDef>((x, d) => x.type = d, _Type), Patterns.IgnoreLeadingWhitespace(Patterns.Literal(";")))));
		var _SName = Patterns.Memoize(Patterns.Bind<SName>(Patterns.With<SName>((x, d) => x.Value = d, Patterns.IgnoreLeadingWhitespace(Patterns.Regex("^[a-zA-Z_][a-zA-Z0-9_:\\-]*")))));
		var _SubServiceNameList = Patterns.Memoize(Patterns.PopValue(Patterns.TupleLooseSequence([typeof(SName), typeof(string)], Patterns.PushValue(_SName), Patterns.IgnoreLeadingWhitespace(Patterns.Literal(",")))));
		var _ServiceNameList = Patterns.Memoize(Patterns.Bind<ServiceNameList>(Patterns.TupleLooseSequence([typeof(List<SName>), typeof(SName)], Patterns.With<ServiceNameList>((x, d) => x.Head = d, Patterns.ZeroOrMore<SName>(Patterns.IgnoreLeadingWhitespace(_SubServiceNameList))), Patterns.With<ServiceNameList>((x, d) => x.Tail = d, _SName))));
		var _NamedType = Patterns.Memoize(Patterns.Bind<NamedType>(Patterns.With<NamedType>((x, d) => x.Value = d, Patterns.TupleLooseSequence([typeof(Type), typeof(Name)], _Type, Patterns.IgnoreLeadingWhitespace(Patterns.Optional(_Name))))));
		var _SubNamedTuple = Patterns.Memoize(Patterns.PopValue(Patterns.TupleLooseSequence([typeof(NamedType), typeof(string)], Patterns.PushValue(_NamedType), Patterns.IgnoreLeadingWhitespace(Patterns.Literal(",")))));
		var _NamedTuple = Patterns.Memoize(Patterns.Bind<NamedTuple>(Patterns.TupleLooseSequence([typeof(string), typeof(List<NamedType>), typeof(NamedType), typeof(string)], Patterns.IgnoreLeadingWhitespace(Patterns.Literal("(")), Patterns.With<NamedTuple>((x, d) => x.Head = d, Patterns.ZeroOrMore<NamedType>(Patterns.IgnoreLeadingWhitespace(_SubNamedTuple))), Patterns.With<NamedTuple>((x, d) => x.Tail = d, Patterns.IgnoreLeadingWhitespace(Patterns.Optional(_NamedType))), Patterns.IgnoreLeadingWhitespace(Patterns.Literal(")")))));
		var _FuncDef = Patterns.Memoize(Patterns.Bind<FuncDef>(Patterns.TupleLooseSequence([typeof(List<Comment>), typeof(List<Parser>), typeof(string), typeof(Number), typeof(string), typeof(Name), typeof(NamedTuple), typeof((string, Parser)), typeof(string)], Patterns.With<FuncDef>((x, d) => x.doc = d, Patterns.ZeroOrMore<Comment>(Patterns.IgnoreLeadingWhitespace(_Comment))), Patterns.With<FuncDef>((x, d) => x.decorators = d, Patterns.ZeroOrMore<Parser>(Patterns.IgnoreLeadingWhitespace(_Decorator))), Patterns.IgnoreLeadingWhitespace(Patterns.Literal("[")), Patterns.With<FuncDef>((x, d) => x.cmdId = d, _Number), Patterns.IgnoreLeadingWhitespace(Patterns.Literal("]")), Patterns.With<FuncDef>((x, d) => x.name = d, _Name), Patterns.With<FuncDef>((x, d) => x.inputs = d, _NamedTuple), Patterns.IgnoreLeadingWhitespace(Patterns.Optional(Patterns.TupleLooseSequence([typeof(string), typeof(Parser)], Patterns.IgnoreLeadingWhitespace(Patterns.Literal("->")), Patterns.With<FuncDef>((x, d) => x.outputs = d, Patterns.IgnoreLeadingWhitespace(Patterns.Choice(_NamedType, _NamedTuple)))))), Patterns.IgnoreLeadingWhitespace(Patterns.Literal(";")))));
		var _Interface = Patterns.Memoize(Patterns.Bind<Interface>(Patterns.TupleLooseSequence([typeof(List<Comment>), typeof(List<Parser>), typeof(string), typeof(Name), typeof((string, ServiceNameList)), typeof(string), typeof(List<FuncDef>), typeof(string)], Patterns.With<Interface>((x, d) => x.doc = d, Patterns.ZeroOrMore<Comment>(Patterns.IgnoreLeadingWhitespace(_Comment))), Patterns.With<Interface>((x, d) => x.decorators = d, Patterns.ZeroOrMore<Parser>(Patterns.IgnoreLeadingWhitespace(_Decorator))), Patterns.IgnoreLeadingWhitespace(Patterns.Literal("interface")), Patterns.With<Interface>((x, d) => x.name = d, _Name), Patterns.IgnoreLeadingWhitespace(Patterns.Optional(Patterns.TupleLooseSequence([typeof(string), typeof(ServiceNameList)], Patterns.IgnoreLeadingWhitespace(Patterns.Literal("is")), Patterns.With<Interface>((x, d) => x.serviceNames = d, _ServiceNameList)))), Patterns.IgnoreLeadingWhitespace(Patterns.Literal("{")), Patterns.With<Interface>((x, d) => x.functions = d, Patterns.ZeroOrMore<FuncDef>(Patterns.IgnoreLeadingWhitespace(_FuncDef))), Patterns.IgnoreLeadingWhitespace(Patterns.Literal("}")))));
		var _Def = Patterns.Memoize(Patterns.Bind<Def>(Patterns.With<Def>((x, d) => x.Value = d, Patterns.IgnoreLeadingWhitespace(Patterns.Choice(_TypeDef, _Interface)))));
		var _Start = Patterns.Memoize(Patterns.Bind<Start>(Patterns.TupleLooseSequence([typeof(List<Def>), typeof(object)], Patterns.With<Start>((x, d) => x.Defs = d, Patterns.OneOrMore<Def>(Patterns.IgnoreLeadingWhitespace(_Def))), Patterns.IgnoreLeadingWhitespace(Patterns.End))));
		Grammar = new Grammar(_Start);
	}
	static Start Parse(string input) => (Start) Grammar.Parse(input);
}
