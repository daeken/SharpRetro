cmake_minimum_required(VERSION 4.1)
project(NativeLib LANGUAGES OBJCXX)

set(CMAKE_OBJCXX_STANDARD 20)

find_package(SDL2 REQUIRED)
include_directories(NativeLib ${SDL2_INCLUDE_DIRS})

add_library(NativeLib SHARED library.mm
        nv.mm
        nv.h
        vi.mm
        vi.h
        nvn.h
        nvnDevice.mm
        nvnQueue.mm
        nvnWindow.mm
        nvnProgram.mm
        nvnMemoryPool.mm
        nvnTexturePool.mm
        nvnSamplerPool.mm
        nvnBuffer.mm
        nvnTextureBuilder.mm
        nvnTextureView.mm
        nvnTexture.mm
        nvnSamplerBuilder.mm
        nvnSampler.mm
        nvnBlendState.mm
        nvnColorState.mm
        nvnChannelMaskState.mm
        nvnMultisampleState.mm
        nvnPolygonState.mm
        nvnDepthStencilState.mm
        nvnVertexAttribState.mm
        nvnVertexStreamState.mm
        nvnCommandBuffer.mm
        nvnSync.mm
        nvnEvent.mm
        nvnBootstrap.mm
        glslc.h
        glslc.mm)
target_link_libraries(NativeLib ${SDL2_LIBRARIES})
