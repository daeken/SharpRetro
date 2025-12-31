cmake_minimum_required(VERSION 4.1)
project(NativeLib)

set(CMAKE_CXX_STANDARD 20)

add_library(NativeLib SHARED library.cpp
        nv.cpp
        nv.h
        vi.cpp
        vi.h
        nvn.h
        nvnDevice.cpp
        nvnQueue.cpp
        nvnWindow.cpp
        nvnProgram.cpp
        nvnMemoryPool.cpp
        nvnTexturePool.cpp
        nvnSamplerPool.cpp
        nvnBuffer.cpp
        nvnTextureBuilder.cpp
        nvnTextureView.cpp
        nvnTexture.cpp
        nvnSamplerBuilder.cpp
        nvnSampler.cpp
        nvnBlendState.cpp
        nvnColorState.cpp
        nvnChannelMaskState.cpp
        nvnMultisampleState.cpp
        nvnPolygonState.cpp
        nvnDepthStencilState.cpp
        nvnVertexAttribState.cpp
        nvnVertexStreamState.cpp
        nvnCommandBuffer.cpp
        nvnSync.cpp
        nvnEvent.cpp
        nvnBootstrap.cpp
        glslc.h
        glslc.cpp)
