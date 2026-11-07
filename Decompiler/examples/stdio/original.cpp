#include <cstdio>
#include <cstdint>

// A separate synthetic fixture for locally owned streams. The upstream b64
// source remains untouched and still exercises unproved incoming FILE* values.
__attribute__((noinline)) static int write_pattern(const char* path,unsigned seed) {
    FILE* stream=std::fopen(path,"wb");
    if(!stream) return 117;
    std::uint32_t block=seed^0x8000ff61u;
    int result=0;
    for(unsigned i=0;i<17;++i) {
        if(std::fwrite(&block,1,sizeof(block),stream)!=sizeof(block)) { result=118; break; }
    }
    int closed=std::fclose(stream);
    return closed==0 ? result : 119;
}

__attribute__((noinline)) static int checksum(const char* path) {
    FILE* stream=std::fopen(path,"rb");
    if(!stream) return 117;
    unsigned char byte;
    unsigned sum=0;
    while(std::fread(&byte,1,1,stream)==1) sum=(sum+byte)&127;
    int closed=std::fclose(stream);
    return closed==0 ? int(sum) : 119;
}

int main(int argc,char** argv) {
    if(argc!=3) return 120;
    if(argv[1][0]=='w') return write_pattern(argv[2],unsigned(argc));
    if(argv[1][0]=='r') return checksum(argv[2]);
    return 121;
}
