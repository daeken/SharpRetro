import re, sys

def walk(s):
    """Yield (i, c, is_code) — is_code=True iff at code-level (not inside string/char literal/comment)."""
    i = 0; sstate = None; hole_depth = 0
    while i < len(s):
        c = s[i]; c2 = s[i:i+2]
        if sstate is None:
            if c2 == '$"' or s[i:i+3] in ('$@"', '@$"'):
                sstate='istr'; skip=2 if c2=='$"' else 3
                for k in range(skip): yield (i+k,s[i+k],False)
                i+=skip; continue
            if c2=='@"': sstate='vstr'; yield(i,'@',False);yield(i+1,'"',False);i+=2;continue
            if c=='"': sstate='str';yield(i,c,False);i+=1;continue
            if c=="'":
                yield(i,c,False);i+=1
                while i<len(s):
                    if s[i]=='\\':yield(i,s[i],False);yield(i+1,s[i+1],False);i+=2;continue
                    yield(i,s[i],False);ch=s[i];i+=1
                    if ch=="'":break
                continue
            if c2=='//':
                while i<len(s) and s[i]!='\n':yield(i,s[i],False);i+=1
                continue
            yield(i,c,True);i+=1
        elif sstate=='str':
            if c=='\\':yield(i,c,False);yield(i+1,s[i+1],False);i+=2;continue
            yield(i,c,False)
            if c=='"':sstate=None
            i+=1
        elif sstate=='vstr':
            if c2=='""':yield(i,c,False);yield(i+1,c,False);i+=2;continue
            yield(i,c,False)
            if c=='"':sstate=None
            i+=1
        elif sstate=='istr':
            if c2 in('{{','}}'):yield(i,c,False);yield(i+1,s[i+1],False);i+=2;continue
            if c=='{':sstate='ihole';hole_depth=0;yield(i,c,False);i+=1;continue
            yield(i,c,False)
            if c=='"':sstate=None
            i+=1
        elif sstate=='ihole':
            if c=='"':
                yield(i,c,False);i+=1
                while i<len(s):
                    if s[i]=='\\':yield(i,s[i],False);yield(i+1,s[i+1],False);i+=2;continue
                    yield(i,s[i],False);ch=s[i];i+=1
                    if ch=='"':break
                continue
            if c2=='$"':
                yield(i,c,False);yield(i+1,s[i+1],False);i+=2
                nd=0
                while i<len(s):
                    ch=s[i]
                    if s[i:i+2] in('{{','}}'):yield(i,ch,False);yield(i+1,s[i+1],False);i+=2;continue
                    if ch=='{':nd+=1
                    elif ch=='}':nd-=1
                    elif ch=='"' and nd<=0:yield(i,ch,False);i+=1;break
                    yield(i,ch,False);i+=1
                continue
            if c=='{':hole_depth+=1
            elif c=='}':
                if hole_depth==0:sstate='istr';yield(i,c,False);i+=1;continue
                hole_depth-=1
            yield(i,c,False);i+=1

def paren_scan(src, i):
    assert src[i]=='('
    depth=0
    for j,c,is_code in walk(src[i:]):
        if not is_code:continue
        if c=='(':depth+=1
        elif c==')':
            depth-=1
            if depth==0:return i+j+1
    raise ValueError()

def split_tl(s):
    depth=0;parts=[];cur=0
    for i,c,is_code in walk(s):
        if not is_code:continue
        if c in '([{':depth+=1
        elif c in ')]}':depth-=1
        elif c==',' and depth==0:parts.append(s[cur:i]);cur=i+1
    parts.append(s[cur:])
    return [p.strip() for p in parts]

def brace_scan(src, i):
    """Find matching } for { at src[i], code-aware."""
    assert src[i]=='{'
    depth=0
    for j,c,is_code in walk(src[i:]):
        if not is_code:continue
        if c=='{':depth+=1
        elif c=='}':
            depth-=1
            if depth==0:return i+j+1
    raise ValueError()

def extract_emit(path, out, cls_name, extra_using=""):
    src = open(path).read()
    cls_m = re.search(r'(?:public\s+)?class \w+ : Builtin\s*\{', src)
    define_m = re.search(r'public override void Define\(\)\s*\{', src)
    helpers = src[cls_m.end():define_m.start()].rstrip() if cls_m else ""
    body_start = define_m.end() - 1  # at the {
    body_end = brace_scan(src, body_start)
    body = src[body_start+1 : body_end-1]  # inside Define() { ... }

    # Find every registration span [start, end) including trailing .Interpret()/.NoInterpret() + ;
    spans = []
    for m in re.finditer(r'\b(Expression|Statement|BranchExpression)\s*\(', body):
        kind = m.group(1); ps = m.end()-1; pe = paren_scan(body, ps)
        args = split_tl(body[ps+1:pe-1])
        # consume trailing .Interpret(...)/.NoInterpret() chain + ;
        cursor = pe
        while True:
            tail = body[cursor:]
            im = re.match(r'\s*\.\s*(Interpret|NoInterpret)\s*\(', tail)
            if not im: break
            ist = cursor + im.end() - 1
            cursor = paren_scan(body, ist)
        # trailing ;
        sm = re.match(r'\s*;', body[cursor:])
        if sm: cursor += sm.end()
        # back up start to line-start (so gap text stays clean)
        line_start = body.rfind('\n', 0, m.start()) + 1
        spans.append((line_start, cursor, kind, args))

    # Postprocess a text chunk: Core.X → X (using-static CSharpEmit exposes them)
    def fixup(txt):
        txt = re.sub(r'\bCore\.', '', txt)
        # LibSharpRetro + DoubleSharp both extend .ForEach — pin to LinqExtensions (matches legacy)
        return txt

    with open(out, 'w') as f:
        f.write("using System.Diagnostics;\nusing ArchCompilerCore;\nusing DoubleSharp.Linq;\nusing LibSharpRetro;\nusing PrettyPrinter;\n")
        if extra_using: f.write(extra_using + "\n")
        f.write("using static Backends.CSharp.CSharpEmit;\nusing static ArchCompilerCore.BuiltinTypes;\n\n")
        f.write("namespace Backends.CSharp;\n\n")
        f.write(f"// Emit-lambdas extracted from legacy {path}\n")
        f.write(f"public static class {cls_name}Emit {{\n")
        if helpers.strip():
            # class-level helpers → make static
            h = re.sub(r'^(\t)((?:unsafe\s+)?[\w<>\[\]?]+\s+\w+\s*\()', r'\1static \2', helpers, flags=re.M)
            f.write(fixup(h) + "\n\n")
        f.write("    public static void Register() {\n")
        cursor = 0
        for start, end, kind, args in spans:
            gap = body[cursor:start]
            if gap.strip():
                f.write(fixup(gap))
            names = args[0]
            ct = args[2] if len(args) > 2 else None
            rt = args[3] if len(args) > 3 else None
            if ct is None:
                f.write(f"        // {names}: no emit-lambda\n")
            elif rt:
                f.write(f"        {kind}({names},\n            {fixup(ct)},\n            {fixup(rt)});\n")
            else:
                f.write(f"        {kind}({names},\n            {fixup(ct)});\n")
            cursor = end
        # trailing gap (post-last-registration)
        tail = body[cursor:]
        if tail.strip():
            f.write(fixup(tail))
        f.write("    }\n}\n")
    return len(spans)

modules = [
    ("CoreArchCompiler/ScalarMath.cs", "ScalarMath", ""),
    ("CoreArchCompiler/Logic.cs", "Logic", ""),
    ("CoreArchCompiler/ControlFlow.cs", "ControlFlow", ""),
    ("CoreArchCompiler/VectorMath.cs", "VectorMath", ""),
    ("CoreArchCompiler/StringManipulation.cs", "StringManipulation", ""),
    ("CoreArchCompiler/TopLevelProcessing.cs", "TopLevelProcessing", ""),
    ("Aarch64Generator/Builtins.cs", "Aarch64", "using static Aarch64Common.Common;"),
]
for path, cls, u in modules:
    n = extract_emit(path, f"Backends/CSharp/{cls}Emit.cs", cls, u)
    print(f"  {cls}: {n}")
