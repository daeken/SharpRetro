#include <algorithm>
#include <bit>
#include <cstdint>
#include <cstdio>
#include <cstdlib>
#include <iostream>
#include <stdexcept>
#include <string>
#include <vector>
#ifdef SOURCE_REFERENCE
#include "upstream/lz4.h"
#else
#ifndef RECOVERED_CPP
#define RECOVERED_CPP "build/recovered.cpp"
#endif
#include RECOVERED_CPP
#endif

int main(int argc,char** argv) {
    try {
        if(argc!=3) throw std::runtime_error("expected HEXINPUT CAPACITY");
        std::string hex=argv[1]; std::size_t used=0; auto capacity=std::stoul(argv[2],&used);
        if(used!=std::string(argv[2]).size() || capacity>1024*1024 || hex.size()%2 || hex.size()>2*1024*1024)
            throw std::runtime_error("invalid buffer size");
        std::vector<unsigned char> source(hex.size()/2+64,0xee),output(capacity+64,0xcd);
        std::fill_n(output.begin()+32,capacity,0xa5);
        auto nibble=[](char c)->unsigned {
            if(c>='0' && c<='9') return unsigned(c-'0');
            if(c>='a' && c<='f') return unsigned(c-'a'+10);
            if(c>='A' && c<='F') return unsigned(c-'A'+10);
            throw std::runtime_error("invalid hex input");
        };
        for(std::size_t i=0;i<hex.size()/2;i++) source[i+32]=static_cast<unsigned char>((nibble(hex[i*2])<<4)|nibble(hex[i*2+1]));
        auto before=source;
#ifdef SOURCE_REFERENCE
        int result=LZ4_decompress_safe(reinterpret_cast<const char*>(source.data()+32),reinterpret_cast<char*>(output.data()+32),int(hex.size()/2),int(capacity));
#else
        auto raw=sharp_retro::source::recovered_export(reinterpret_cast<std::uintptr_t>(source.data()+32),
            reinterpret_cast<std::uintptr_t>(output.data()+32),hex.size()/2,capacity);
        int result=std::bit_cast<std::int32_t>(static_cast<std::uint32_t>(raw));
#endif
        bool canaries=source==before && std::all_of(output.begin(),output.begin()+32,[](auto b){return b==0xcd;}) &&
            std::all_of(output.end()-32,output.end(),[](auto b){return b==0xcd;});
        std::cout<<"{\"return\":"<<result<<",\"output\":\"";
        constexpr char digits[]="0123456789abcdef";
        for(std::size_t i=32;i<capacity+32;i++) std::cout<<digits[output[i]>>4]<<digits[output[i]&15];
        std::cout<<"\",\"canaries_ok\":"<<(canaries ? "true" : "false")<<"}\n";
        return canaries ? 0 : 124;
    } catch(const std::exception& e) { std::cerr<<"lz4 driver: "<<e.what()<<'\n'; return 125; }
}
