// Build as x86-64, then recover the executable's code; this source is never
// included in or linked into the recovered ARM64 program.
#include <stdint.h>
#include <unistd.h>

static __attribute__((noinline)) unsigned length(const char* s) {
    unsigned n=0; while(s[n]) ++n; return n;
}
static __attribute__((noinline)) bool equal(const char* a, const char* b) {
    while(*a && *a==*b) { ++a; ++b; } return *a==*b;
}
static __attribute__((noinline)) bool output(int fd, const char* data, unsigned count) {
    while(count) {
        ssize_t n=write(fd,data,count);
        if(n<=0) return false;
        data+=n; count-=(unsigned)n;
    }
    return true;
}
static __attribute__((noinline)) bool text(int fd, const char* s) {
    return output(fd,s,length(s));
}
static __attribute__((noinline)) bool number(uint64_t value) {
    char digits[32]; unsigned used=0;
    do { digits[used++]=char('0'+value%10); value/=10; } while(value);
    for(unsigned i=0; i<used/2; ++i) {
        char c=digits[i]; digits[i]=digits[used-1-i]; digits[used-1-i]=c;
    }
    return output(1,digits,used);
}
static __attribute__((noinline)) bool parse(const char* s, uint64_t* result) {
    uint64_t n=0;
    if(!*s) return false;
    while(*s) {
        unsigned d=(unsigned char)*s++-'0';
        if(d>9 || n>UINT64_C(1844674407370955161) ||
           (n==UINT64_C(1844674407370955161) && d>5)) return false;
        n=n*10+d;
    }
    *result=n; return true;
}
static __attribute__((noinline)) uint64_t hash_bytes(uint64_t hash, const unsigned char* p, unsigned n) {
    for(unsigned i=0; i<n; ++i) hash=(hash^p[i])*UINT64_C(1099511628211);
    return hash;
}
static int invalid() { text(2,"invalid arguments\n"); return 2; }

int main(int argc, char** argv) {
    if(argc<2 || equal(argv[1],"help")) {
        text(1,"commands: hash [text] | stats N... | seq SEED COUNT\n"); return argc<2 ? 2 : 0;
    }
    if(equal(argv[1],"hash")) {
        uint64_t hash=UINT64_C(14695981039346656037);
        if(argc==3) hash=hash_bytes(hash,(const unsigned char*)argv[2],length(argv[2]));
        else if(argc==2) {
            unsigned char buffer[256];
            while(true) {
                ssize_t n=read(0,buffer,sizeof(buffer));
                if(n<0) { text(2,"read failed\n"); return 3; }
                if(!n) break;
                hash=hash_bytes(hash,buffer,(unsigned)n);
            }
        } else return invalid();
        if(!number(hash) || !text(1,"\n")) return 3;
        return 0;
    }
    if(equal(argv[1],"stats")) {
        if(argc<3) return invalid();
        uint64_t sum=0, low=UINT64_MAX, high=0;
        for(int i=2; i<argc; ++i) {
            uint64_t value;
            if(!parse(argv[i],&value)) return invalid();
            sum+=value;
            if(value<low) low=value;
            if(value>high) high=value;
        }
        if(!text(1,"count=") || !number((unsigned)(argc-2)) || !text(1," sum=") || !number(sum) ||
           !text(1," min=") || !number(low) || !text(1," max=") || !number(high) || !text(1,"\n")) return 3;
        return 0;
    }
    if(equal(argv[1],"seq")) {
        uint64_t seed, count;
        if(argc!=4 || !parse(argv[2],&seed) || !parse(argv[3],&count) || count>32) return invalid();
        for(uint64_t i=0; i<count; ++i) {
            seed^=seed<<13; seed^=seed>>7; seed^=seed<<17;
            if(!number(seed) || !text(1,"\n")) return 3;
        }
        return 0;
    }
    return invalid();
}
