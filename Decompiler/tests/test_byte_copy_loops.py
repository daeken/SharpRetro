"""Rerolled copies retain byte order, overlap, unsigned bounds and exit ownership."""
import html
import os
import test_pipeline as pipeline


HARNESS = r'''
using SharpRetro.Decompiler;
CppSequence S(params CppStatement[] x)=>new(x);
CppLine L(string x,string? n=null)=>new(x,n);
string Emit(CppSequence b){var lines=new List<string>();CppSourceTree.Emit(b,(s,d)=>lines.Add(new string(' ',4*d)+s));return string.Join("\n",lines);}
var terminal=S(L("++commits;"),L("return 7ULL;"));
CppStatement[] Pair(int i,bool native=false){
 string At(string root)=>native?$"*(byte_offset({root},{i}ULL))":$"*reinterpret_cast<{{0}}u8*>(pointer({root}+{i}ULL))";
 string read=native?At("src"):At("src").Replace("{0}","const ");
 string write=native?At("dst"):At("dst").Replace("{0}","");
 return [L($"const std::uint8_t b{i}={read};",$"b{i}"),L($"{write}=b{i};")];
}
CppSequence Stair(int count,bool positive=true,bool native=false,CppSequence? exit=null,bool negate=false){
 exit??=terminal;var list=new List<CppStatement>();
 for(int i=0;i<count;i++){
  string bound=native?$"address(end)<=address(dst)+{i+1}ULL":$"end<=dst+{i+1}ULL";
  if(i==count-1&&positive){
   string keep=native?$"address(end)>address(dst)+{i}ULL":$"end>dst+{i}ULL";
   list.Add(new CppIf(new(keep),S(Pair(i,native)),S()));
  }else{
   list.AddRange(Pair(i,native));
   if(i<count-1&&!(positive&&i==count-2))list.Add(new CppIf(new(negate?bound.Replace("<=",">"):bound,negate),exit,S()));
  }
 }
 list.AddRange(exit.Body);return new(list);
}
Dictionary<string,string> Types(bool native=false)=>new(){["src"]=native?"const u8*":"std::uint64_t",["dst"]=native?"u8*":"std::uint64_t",["end"]=native?"const u8*":"std::uint64_t"};
var rows=new List<(string Name,CppSequence Body,bool Native,int Count,int Rounds)>();
void Add(string name,CppSequence body,int count,bool native=false,int rounds=1){
 var r=CppByteCopyLoops.Reduce(body,Types(native));
 if(r.Loops!=1||r.Copies!=count)throw new Exception(name+" failed to reroll: "+r.Loops+"/"+r.Copies);
 rows.Add((name,body,native,count,rounds));
}
Add("positive3",Stair(3),3);Add("positive15",Stair(15),15);
Add("plain4",Stair(4,false),4);Add("negative_guard",Stair(4,negate:true),4);
Add("native",Stair(4,native:true),4,true);
Add("continue_owner",S(L("std::uint64_t rounds=0ULL;","rounds"),new CppWhile("rounds<2ULL",S(new CppStatement[]{L("++rounds;")}.Concat(Stair(4,exit:S(L("++commits;"),L("continue;"))).Body).ToArray())),L("return rounds;")),4,rounds:2);
Add("break_owner",S(new CppWhile("true",Stair(4,exit:S(L("++commits;"),L("break;")))),L("return 7ULL;")),4);
// Reusing the same immutable statements in disjoint lexical scopes must not
// turn two dead byte temporaries into one live binding.
var duplicate=Stair(3);
var twins=S(new CppIf(new("choose"),duplicate,duplicate));
if(CppByteCopyLoops.Reduce(twins,Types()).Loops!=2)throw new Exception("lexical duplicate bindings");
var collision=S(L("std::uint64_t copy_index_0=0ULL;","copy_index_0"),Stair(3));
if(!Emit(CppByteCopyLoops.Reduce(collision,Types()).Body).Contains("copy_index_1"))throw new Exception("name collision");
void Refuse(string name,CppSequence body,Dictionary<string,string>? types=null){
 var r=CppByteCopyLoops.Reduce(body,types??Types());
 if(r.Loops!=0||!ReferenceEquals(body,r.Body))throw new Exception(name+" unsafe reroll");
}
Refuse("tiny",Stair(2));Refuse("cap",Stair(65));
Refuse("live byte",S(Stair(4),L("consume(b0);")));
Refuse("opaque reference",S(L("remember(src);"),Stair(4)));
Refuse("opaque template reference",S(L("remember<(2>1)>(src);"),Stair(4)));
Refuse("aggregate reference",S(L("Capture capture{end};","capture"),Stair(4)));
Refuse("aggregate copy-list reference",S(L("Capture capture = {end};","capture"),Stair(4)));
Refuse("address exposed",S(L("saved=&dst;"),Stair(4)));
Refuse("reference binding",S(L("auto& alias=end;","alias"),Stair(4)));
var signed=Types();signed["dst"]="std::int64_t";Refuse("signed root",Stair(4),signed);
var volatileTypes=Types();volatileTypes["end"]="volatile std::uint64_t";Refuse("volatile root",Stair(4),volatileTypes);
var narrow=Types();narrow["src"]="std::uint32_t";Refuse("narrow root",Stair(4),narrow);
CppSequence Change(int at,Func<CppStatement,CppStatement> edit){var a=Stair(4).Body.ToArray();a[at]=edit(a[at]);return new(a);}
Refuse("nonsequential",Change(3,x=>((CppLine)x) with {Text=((CppLine)x).Text.Replace("src+1ULL","src+2ULL")}));
Refuse("different source",Change(3,x=>((CppLine)x) with {Text=((CppLine)x).Text.Replace("src+1ULL","dst+1ULL")}));
Refuse("different bound",Change(5,x=>((CppIf)x) with {Condition=new("src<=dst+2ULL")}));
Refuse("different continuation",Change(5,x=>((CppIf)x) with {Then=S(L("commits+=2;"),L("return 7ULL;"))}));
Refuse("nonempty else",Change(5,x=>((CppIf)x) with {Else=S(L("++commits;"))}));
Refuse("volatile memory",Change(0,x=>((CppLine)x) with {Text=((CppLine)x).Text.Replace("const u8*","const volatile u8*")}));
Refuse("extra access",Change(3,x=>((CppLine)x) with {Text=((CppLine)x).Text.Replace("src+1ULL","src+observe()")}));
Refuse("trailing call expression",Change(0,x=>((CppLine)x) with {Text=((CppLine)x).Text.Replace("pointer(src+0ULL)","pointer(src+0ULL)+observe()")}));
var intervening=Stair(4).Body.ToList();intervening.Insert(3,L("observe();"));Refuse("intervening effect",new(intervening));
Refuse("different literal effects",Change(5,x=>((CppIf)x) with {Then=S(L("note(\"other\");"),L("return 7ULL;"))}));
Console.WriteLine("""
#include <cstdint>
#include <cstdio>
#include <array>
#include <vector>
using u8=std::uint8_t;
std::array<u8,64> memory;
std::vector<std::uint64_t> trace;
std::uint64_t commits;
bool tracking=true;
u8* pointer(std::uint64_t a){if(!tracking)return reinterpret_cast<u8*>(a);trace.push_back(a);return memory.data()+(a&63ULL);}
std::uint64_t address(const u8* p){return reinterpret_cast<std::uintptr_t>(p);}
template<class T>T* byte_offset(T* p,std::uint64_t n){return p+n;}
struct Result{std::array<u8,64> bytes;std::vector<std::uint64_t> accesses;std::uint64_t commits,value;bool operator==(const Result&)const=default;};
void reset(){for(unsigned i=0;i<64;i++)memory[i]=u8(i*17+3);trace.clear();commits=0;}
Result snapshot(std::uint64_t value){return {memory,trace,commits,value};}
// Independent byte-at-a-time oracle. No memmove snapshot: an overlapping
// forward match must observe bytes written earlier in the same copy.
Result oracle(std::uint64_t src,std::uint64_t dst,std::uint64_t end,unsigned cap,unsigned rounds,std::uint64_t value){
 reset();for(unsigned r=0;r<rounds;r++){
  for(unsigned i=0;i<cap;i++){
   trace.push_back(src+i);u8 v=memory[(src+i)&63ULL];
   trace.push_back(dst+i);memory[(dst+i)&63ULL]=v;
   if(end<=dst+i+1ULL)break;
  }++commits;
 }return snapshot(value);
}
""");
foreach(var row in rows)foreach(bool after in new[]{false,true}){
 var body=after?CppByteCopyLoops.Reduce(row.Body,Types(row.Native)).Body:row.Body;
 string parameters=row.Native?"const u8* src,u8* dst,const u8* end":"std::uint64_t src,std::uint64_t dst,std::uint64_t end";
 Console.WriteLine($"std::uint64_t {(after?"after":"before")}_{row.Name}({parameters}){{\n{Emit(body)}\n}}");
}
Console.WriteLine("int main(){");
foreach(var row in rows){
 string name=row.Name;string value=name=="continue_owner"?"2ULL":"7ULL";
 if(row.Native){
  Console.WriteLine($"tracking=false;for(unsigned s=0;s<24;s++)for(unsigned d=0;d<24;d++)for(unsigned e=0;e<35;e++){{reset();auto v=before_{name}(memory.data()+s,memory.data()+d,memory.data()+e);auto a=snapshot(v);reset();v=after_{name}(memory.data()+s,memory.data()+d,memory.data()+e);if(!(a==snapshot(v)))return 2;}}tracking=true;");
 }else{
  Console.WriteLine($"for(std::uint64_t s:{{0ULL,1ULL,7ULL,31ULL,~0ULL-7ULL}})for(std::uint64_t d:{{0ULL,1ULL,8ULL,31ULL,~0ULL-7ULL}})for(std::uint64_t delta=0;delta<20;delta++){{auto end=d+delta;auto expected=oracle(s,d,end,{row.Count},{row.Rounds},{value});");
  foreach(string mode in new[]{"before","after"})Console.WriteLine($"reset();auto {mode}=snapshot({mode}_{name}(s,d,end));if(!({mode}==expected)){{std::fprintf(stderr,\"{mode}_{name} s=%llu d=%llu delta=%llu\\n\",(unsigned long long)s,(unsigned long long)d,(unsigned long long)delta);return 1;}}");
  Console.WriteLine("}");
 }
}
Console.WriteLine("}");
'''


class ByteCopyLoopTests(pipeline.PipelineCase):
    def test_copy_loops_preserve_overlap_bounds_effects_and_exit_ownership(self):
        project = self.root / 'Harness.csproj'
        project.write_text(f'''<Project Sdk="Microsoft.NET.Sdk"><PropertyGroup><OutputType>Exe</OutputType><TargetFramework>net10.0</TargetFramework><ImplicitUsings>enable</ImplicitUsings><Nullable>enable</Nullable></PropertyGroup><ItemGroup><Reference Include="Decompiler"><HintPath>{html.escape(str(pipeline.DLL))}</HintPath></Reference></ItemGroup></Project>''')
        (self.root / 'Program.cs').write_text(HARNESS)
        (self.root / 'NuGet.Config').write_text('<configuration><packageSources><clear /></packageSources></configuration>')
        built = pipeline.run('dotnet', 'build', project, '-c', 'Release', '--verbosity',
                             'quiet', '-p:UseSharedCompilation=false',
                             env={**os.environ, 'DOTNET_NOLOGO': '1'})
        self.assertEqual(built.returncode, 0, built.stdout + built.stderr)
        emitted = pipeline.run('dotnet', self.root / 'bin/Release/net10.0/Harness.dll')
        self.assertEqual(emitted.returncode, 0, emitted.stdout + emitted.stderr)
        self.compile_run(emitted.stdout)
