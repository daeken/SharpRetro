#include <iostream>
#include "nv.h"

NVNboolean nvnCommandBufferInitialize(NVNcommandBuffer* _cmdBuf, NVNdevice* _device) {
    std::cout << "nvnCommandBufferInitialize called!" << std::endl;
    auto cmdBuf = UNWRAP(_cmdBuf);
    auto device = UNWRAP(_device);
    return 1;
}

void nvnCommandBufferFinalize(NVNcommandBuffer* _cmdBuf) {
    std::cout << "nvnCommandBufferFinalize called!" << std::endl;
    auto cmdBuf = UNWRAP(_cmdBuf);
}

void nvnCommandBufferSetDebugLabel(NVNcommandBuffer* _cmdBuf, const char* label) {
    std::cout << "nvnCommandBufferSetDebugLabel called!" << std::endl;
    auto cmdBuf = UNWRAP(_cmdBuf);
}

void nvnCommandBufferSetMemoryCallback(NVNcommandBuffer* _cmdBuf, PFNNVNCOMMANDBUFFERMEMORYCALLBACKPROC callback) {
    std::cout << "nvnCommandBufferSetMemoryCallback called!" << std::endl;
    auto cmdBuf = UNWRAP(_cmdBuf);
}

void nvnCommandBufferSetMemoryCallbackData(NVNcommandBuffer* _cmdBuf, void* callbackData) {
    std::cout << "nvnCommandBufferSetMemoryCallbackData called!" << std::endl;
    auto cmdBuf = UNWRAP(_cmdBuf);
}

void nvnCommandBufferAddCommandMemory(NVNcommandBuffer* _cmdBuf, NVNmemoryPool* _pool, ptrdiff_t offset, size_t size) {
    std::cout << "nvnCommandBufferAddCommandMemory called!" << std::endl;
    auto cmdBuf = UNWRAP(_cmdBuf);
    auto pool = UNWRAP(_pool);
}

void nvnCommandBufferAddControlMemory(NVNcommandBuffer* _cmdBuf, void* memory, size_t size) {
    std::cout << "nvnCommandBufferAddControlMemory called!" << std::endl;
    auto cmdBuf = UNWRAP(_cmdBuf);
}

size_t nvnCommandBufferGetCommandMemorySize(NVNcommandBuffer* _cmdBuf) {
    std::cout << "nvnCommandBufferGetCommandMemorySize called!" << std::endl;
    auto cmdBuf = UNWRAP(_cmdBuf);
    return 0;
}

size_t nvnCommandBufferGetCommandMemoryUsed(NVNcommandBuffer* _cmdBuf) {
    std::cout << "nvnCommandBufferGetCommandMemoryUsed called!" << std::endl;
    auto cmdBuf = UNWRAP(_cmdBuf);
    return 0;
}

size_t nvnCommandBufferGetCommandMemoryFree(NVNcommandBuffer* _cmdBuf) {
    std::cout << "nvnCommandBufferGetCommandMemoryFree called!" << std::endl;
    auto cmdBuf = UNWRAP(_cmdBuf);
    return 0;
}

size_t nvnCommandBufferGetControlMemorySize(NVNcommandBuffer* _cmdBuf) {
    std::cout << "nvnCommandBufferGetControlMemorySize called!" << std::endl;
    auto cmdBuf = UNWRAP(_cmdBuf);
    return 0;
}

size_t nvnCommandBufferGetControlMemoryUsed(NVNcommandBuffer* _cmdBuf) {
    std::cout << "nvnCommandBufferGetControlMemoryUsed called!" << std::endl;
    auto cmdBuf = UNWRAP(_cmdBuf);
    return 0;
}

size_t nvnCommandBufferGetControlMemoryFree(NVNcommandBuffer* _cmdBuf) {
    std::cout << "nvnCommandBufferGetControlMemoryFree called!" << std::endl;
    auto cmdBuf = UNWRAP(_cmdBuf);
    return 0;
}

void nvnCommandBufferBeginRecording(NVNcommandBuffer* _cmdBuf) {
    std::cout << "nvnCommandBufferBeginRecording called!" << std::endl;
    auto cmdBuf = UNWRAP(_cmdBuf);
}

NVNcommandHandle nvnCommandBufferEndRecording(NVNcommandBuffer* _cmdBuf) {
    std::cout << "nvnCommandBufferEndRecording called!" << std::endl;
    auto cmdBuf = UNWRAP(_cmdBuf);
    return 0;
}

void nvnCommandBufferCallCommands(NVNcommandBuffer* _cmdBuf, int numCommands, const NVNcommandHandle* handles) {
    std::cout << "nvnCommandBufferCallCommands called!" << std::endl;
    auto cmdBuf = UNWRAP(_cmdBuf);
}

void nvnCommandBufferCopyCommands(NVNcommandBuffer* _cmdBuf, int numCommands, const NVNcommandHandle* handles) {
    std::cout << "nvnCommandBufferCopyCommands called!" << std::endl;
    auto cmdBuf = UNWRAP(_cmdBuf);
}

void nvnCommandBufferBindBlendState(NVNcommandBuffer* _cmdBuf, NVNblendState* _blend) {
    std::cout << "nvnCommandBufferBindBlendState called!" << std::endl;
    auto cmdBuf = UNWRAP(_cmdBuf);
    auto blend = UNWRAP(_blend);
}

void nvnCommandBufferBindChannelMaskState(NVNcommandBuffer* _cmdBuf, NVNchannelMaskState* _channelMask) {
    std::cout << "nvnCommandBufferBindChannelMaskState called!" << std::endl;
    auto cmdBuf = UNWRAP(_cmdBuf);
    auto channelMask = UNWRAP(_channelMask);
}

void nvnCommandBufferBindColorState(NVNcommandBuffer* _cmdBuf, NVNcolorState* _color) {
    std::cout << "nvnCommandBufferBindColorState called!" << std::endl;
    auto cmdBuf = UNWRAP(_cmdBuf);
    auto color = UNWRAP(_color);
}

void nvnCommandBufferBindMultisampleState(NVNcommandBuffer* _cmdBuf, NVNmultisampleState* _multisample) {
    std::cout << "nvnCommandBufferBindMultisampleState called!" << std::endl;
    auto cmdBuf = UNWRAP(_cmdBuf);
    auto multisample = UNWRAP(_multisample);
}

void nvnCommandBufferBindPolygonState(NVNcommandBuffer* _cmdBuf, NVNpolygonState* _polygon) {
    std::cout << "nvnCommandBufferBindPolygonState called!" << std::endl;
    auto cmdBuf = UNWRAP(_cmdBuf);
    auto polygon = UNWRAP(_polygon);
}

void nvnCommandBufferBindDepthStencilState(NVNcommandBuffer* _cmdBuf, NVNdepthStencilState* _depthStencil) {
    std::cout << "nvnCommandBufferBindDepthStencilState called!" << std::endl;
    auto cmdBuf = UNWRAP(_cmdBuf);
    auto depthStencil = UNWRAP(_depthStencil);
}

void nvnCommandBufferBindVertexAttribState(NVNcommandBuffer* _cmdBuf, int numAttribs, NVNvertexAttribState* _attribs) {
    std::cout << "nvnCommandBufferBindVertexAttribState called!" << std::endl;
    auto cmdBuf = UNWRAP(_cmdBuf);
    auto attribs = UNWRAP(_attribs);
}

void nvnCommandBufferBindVertexStreamState(NVNcommandBuffer* _cmdBuf, int numStreams, NVNvertexStreamState* _streams) {
    std::cout << "nvnCommandBufferBindVertexStreamState called!" << std::endl;
    auto cmdBuf = UNWRAP(_cmdBuf);
    auto streams = UNWRAP(_streams);
}

void nvnCommandBufferBindProgram(NVNcommandBuffer* _cmdBuf, NVNprogram* _program, int stages) {
    std::cout << "nvnCommandBufferBindProgram(program=" << std::hex << reinterpret_cast<uint64_t>(_program) << ", stages=" << std::dec << stages << ") called!" << std::endl;
    auto cmdBuf = UNWRAP(_cmdBuf);
    auto program = UNWRAP(_program);
}

void nvnCommandBufferBindVertexBuffer(NVNcommandBuffer* _cmdBuf, int index, NVNbufferAddress buffer, size_t size) {
    std::cout << "nvnCommandBufferBindVertexBuffer(index=" << index << ", buffer=" << std::hex << buffer << ", size=" << std::dec << size << ") called!" << std::endl;
    auto cmdBuf = UNWRAP(_cmdBuf);
}

void nvnCommandBufferBindVertexBuffers(NVNcommandBuffer* _cmdBuf, int first, int count, const NVNbufferRange* buffers) {
    std::cout << "nvnCommandBufferBindVertexBuffers(first=" << first << ", count=" << count << ", buffers=" << std::hex << reinterpret_cast<uint64_t>(buffers) << std::dec << ") called!" << std::endl;
    auto cmdBuf = UNWRAP(_cmdBuf);
}

void nvnCommandBufferBindUniformBuffer(NVNcommandBuffer* _cmdBuf, NVNshaderStage stage, int index, NVNbufferAddress buffer, size_t size) {
    std::cout << "nvnCommandBufferBindUniformBuffer(stage=" << stage << ", index=" << index << ", buffer=" << std::hex << buffer << ", size=" << std::dec << size << ") called!" << std::endl;
    auto cmdBuf = UNWRAP(_cmdBuf);
}

void nvnCommandBufferBindUniformBuffers(NVNcommandBuffer* _cmdBuf, NVNshaderStage stage, int first, int count, const NVNbufferRange* buffers) {
    std::cout << "nvnCommandBufferBindUniformBuffers(stage=" << stage << ", first=" << first << ", count=" << count << ", buffers=" << std::hex << reinterpret_cast<uint64_t>(buffers) << std::dec << ") called!" << std::endl;
    auto cmdBuf = UNWRAP(_cmdBuf);
}

void nvnCommandBufferBindTransformFeedbackBuffer(NVNcommandBuffer* _cmdBuf, int index, NVNbufferAddress buffer, size_t size) {
    std::cout << "nvnCommandBufferBindTransformFeedbackBuffer called!" << std::endl;
    auto cmdBuf = UNWRAP(_cmdBuf);
}

void nvnCommandBufferBindTransformFeedbackBuffers(NVNcommandBuffer* _cmdBuf, int first, int count, const NVNbufferRange* buffers) {
    std::cout << "nvnCommandBufferBindTransformFeedbackBuffers called!" << std::endl;
    auto cmdBuf = UNWRAP(_cmdBuf);
}

void nvnCommandBufferBindStorageBuffer(NVNcommandBuffer* _cmdBuf, NVNshaderStage stage, int index, NVNbufferAddress buffer, size_t size) {
    std::cout << "nvnCommandBufferBindStorageBuffer called!" << std::endl;
    auto cmdBuf = UNWRAP(_cmdBuf);
}

void nvnCommandBufferBindStorageBuffers(NVNcommandBuffer* _cmdBuf, NVNshaderStage stage, int first, int count, const NVNbufferRange* buffers) {
    std::cout << "nvnCommandBufferBindStorageBuffers called!" << std::endl;
    auto cmdBuf = UNWRAP(_cmdBuf);
}

void nvnCommandBufferBindTexture(NVNcommandBuffer* _cmdBuf, NVNshaderStage stage, int index, NVNtextureHandle texture) {
    std::cout << "nvnCommandBufferBindTexture(stage=" << stage << ", index=" << index << ", texture=" << std::hex << texture << std::dec << ") called!" << std::endl;
    auto cmdBuf = UNWRAP(_cmdBuf);
}

void nvnCommandBufferBindTextures(NVNcommandBuffer* _cmdBuf, NVNshaderStage stage, int first, int count, const NVNtextureHandle* textures) {
    std::cout << "nvnCommandBufferBindTextures(stage=" << stage << ", first=" << first << ", count=" << count << ", textures=" << std::hex << reinterpret_cast<uint64_t>(textures) << std::dec << ") called!" << std::endl;
    auto cmdBuf = UNWRAP(_cmdBuf);
}

void nvnCommandBufferBindImage(NVNcommandBuffer* _cmdBuf, NVNshaderStage stage, int index, NVNimageHandle image) {
    std::cout << "nvnCommandBufferBindImage(stage=" << stage << ", index=" << index << ", image=" << std::hex << image << std::dec << ") called!" << std::endl;
    auto cmdBuf = UNWRAP(_cmdBuf);
}

void nvnCommandBufferBindImages(NVNcommandBuffer* _cmdBuf, NVNshaderStage stage, int first, int count, const NVNimageHandle* images) {
    std::cout << "nvnCommandBufferBindImages(stage=" << stage << ", first=" << first << ", count=" << count << ", images=" << std::hex << reinterpret_cast<uint64_t>(images) << std::dec << ") called!" << std::endl;
    auto cmdBuf = UNWRAP(_cmdBuf);
}

void nvnCommandBufferSetPatchSize(NVNcommandBuffer* _cmdBuf, int i) {
    std::cout << "nvnCommandBufferSetPatchSize called!" << std::endl;
    auto cmdBuf = UNWRAP(_cmdBuf);
}

void nvnCommandBufferSetInnerTessellationLevels(NVNcommandBuffer* _cmdBuf, const float* f) {
    std::cout << "nvnCommandBufferSetInnerTessellationLevels called!" << std::endl;
    auto cmdBuf = UNWRAP(_cmdBuf);
}

void nvnCommandBufferSetOuterTessellationLevels(NVNcommandBuffer* _cmdBuf, const float* f) {
    std::cout << "nvnCommandBufferSetOuterTessellationLevels called!" << std::endl;
    auto cmdBuf = UNWRAP(_cmdBuf);
}

void nvnCommandBufferSetPrimitiveRestart(NVNcommandBuffer* _cmdBuf, NVNboolean b, int i) {
    std::cout << "nvnCommandBufferSetPrimitiveRestart called!" << std::endl;
    auto cmdBuf = UNWRAP(_cmdBuf);
}

void nvnCommandBufferBeginTransformFeedback(NVNcommandBuffer* _cmdBuf, NVNbufferAddress buffer) {
    std::cout << "nvnCommandBufferBeginTransformFeedback called!" << std::endl;
    auto cmdBuf = UNWRAP(_cmdBuf);
}

void nvnCommandBufferEndTransformFeedback(NVNcommandBuffer* _cmdBuf, NVNbufferAddress buffer) {
    std::cout << "nvnCommandBufferEndTransformFeedback called!" << std::endl;
    auto cmdBuf = UNWRAP(_cmdBuf);
}

void nvnCommandBufferPauseTransformFeedback(NVNcommandBuffer* _cmdBuf, NVNbufferAddress buffer) {
    std::cout << "nvnCommandBufferPauseTransformFeedback called!" << std::endl;
    auto cmdBuf = UNWRAP(_cmdBuf);
}

void nvnCommandBufferResumeTransformFeedback(NVNcommandBuffer* _cmdBuf, NVNbufferAddress buffer) {
    std::cout << "nvnCommandBufferResumeTransformFeedback called!" << std::endl;
    auto cmdBuf = UNWRAP(_cmdBuf);
}

void nvnCommandBufferDrawTransformFeedback(NVNcommandBuffer* _cmdBuf, NVNdrawPrimitive mode, NVNbufferAddress buffer) {
    std::cout << "nvnCommandBufferDrawTransformFeedback called!" << std::endl;
    auto cmdBuf = UNWRAP(_cmdBuf);
}

void nvnCommandBufferDrawArrays(NVNcommandBuffer* _cmdBuf, NVNdrawPrimitive mode, int first, int count) {
    std::cout << "nvnCommandBufferDrawArrays(mode=" << mode << ", first=" << first << ", count=" << count << ") called!" << std::endl;
    auto cmdBuf = UNWRAP(_cmdBuf);
}

void nvnCommandBufferDrawElements(NVNcommandBuffer* _cmdBuf, NVNdrawPrimitive mode, NVNindexType type, int count, NVNbufferAddress indexBuffer) {
    std::cout << "nvnCommandBufferDrawElements(mode=" << mode << ", type=" << type << ", count=" << count << ", indexBuffer=" << std::hex << indexBuffer << std::dec << ") called!" << std::endl;
    auto cmdBuf = UNWRAP(_cmdBuf);
}

void nvnCommandBufferDrawElementsBaseVertex(NVNcommandBuffer* _cmdBuf, NVNdrawPrimitive mode, NVNindexType type, int count, NVNbufferAddress indexBuffer, int baseVertex) {
    std::cout << "nvnCommandBufferDrawElementsBaseVertex(mode=" << mode << ", type=" << type << ", count=" << count << ", indexBuffer=" << std::hex << indexBuffer << ", baseVertex=" << std::dec << baseVertex << ") called!" << std::endl;
    auto cmdBuf = UNWRAP(_cmdBuf);
}

void nvnCommandBufferDrawArraysInstanced(NVNcommandBuffer* _cmdBuf, NVNdrawPrimitive mode, int first, int count, int baseInstance, int instanceCount) {
    std::cout << "nvnCommandBufferDrawArraysInstanced(mode=" << mode << ", first=" << first << ", count=" << count << ", baseInstance=" << baseInstance << ", instanceCount=" << instanceCount << ") called!" << std::endl;
    auto cmdBuf = UNWRAP(_cmdBuf);
}

void nvnCommandBufferDrawElementsInstanced(NVNcommandBuffer* _cmdBuf, NVNdrawPrimitive mode, NVNindexType type, int count, NVNbufferAddress indexBuffer, int baseVertex, int baseInstance, int instanceCount) {
    std::cout << "nvnCommandBufferDrawElementsInstanced(mode=" << mode << ", type=" << type << ", count=" << count << ", indexBuffer=" << std::hex << indexBuffer << ", baseVertex=" << std::dec << baseVertex << ", baseInstance=" << baseInstance << ", instanceCount=" << instanceCount << ") called!" << std::endl;
    auto cmdBuf = UNWRAP(_cmdBuf);
}

void nvnCommandBufferDrawArraysIndirect(NVNcommandBuffer* _cmdBuf, NVNdrawPrimitive mode, NVNbufferAddress buffer) {
    std::cout << "nvnCommandBufferDrawArraysIndirect called!" << std::endl;
    auto cmdBuf = UNWRAP(_cmdBuf);
}

void nvnCommandBufferDrawElementsIndirect(NVNcommandBuffer* _cmdBuf, NVNdrawPrimitive mode, NVNindexType type, NVNbufferAddress buffer1, NVNbufferAddress buffer2) {
    std::cout << "nvnCommandBufferDrawElementsIndirect called!" << std::endl;
    auto cmdBuf = UNWRAP(_cmdBuf);
}

void nvnCommandBufferMultiDrawArraysIndirectCount(NVNcommandBuffer* _cmdBuf, NVNdrawPrimitive mode, NVNbufferAddress buffer1, NVNbufferAddress buffer2, int i, ptrdiff_t o) {
    std::cout << "nvnCommandBufferMultiDrawArraysIndirectCount called!" << std::endl;
    auto cmdBuf = UNWRAP(_cmdBuf);
}

void nvnCommandBufferMultiDrawElementsIndirectCount(NVNcommandBuffer* _cmdBuf, NVNdrawPrimitive mode, NVNindexType type, NVNbufferAddress buffer1, NVNbufferAddress buffer2, NVNbufferAddress buffer3, int i, ptrdiff_t o) {
    std::cout << "nvnCommandBufferMultiDrawElementsIndirectCount called!" << std::endl;
    auto cmdBuf = UNWRAP(_cmdBuf);
}

void nvnCommandBufferClearColor(NVNcommandBuffer* _cmdBuf, int index, const float* color, int mask) {
    std::cout << "nvnCommandBufferClearColor called!" << std::endl;
    auto cmdBuf = UNWRAP(_cmdBuf);
}

void nvnCommandBufferClearColori(NVNcommandBuffer* _cmdBuf, int index, const int* color, int mask) {
    std::cout << "nvnCommandBufferClearColori called!" << std::endl;
    auto cmdBuf = UNWRAP(_cmdBuf);
}

void nvnCommandBufferClearColorui(NVNcommandBuffer* _cmdBuf, int index, const uint32_t* color, int mask) {
    std::cout << "nvnCommandBufferClearColorui called!" << std::endl;
    auto cmdBuf = UNWRAP(_cmdBuf);
}

void nvnCommandBufferClearDepthStencil(NVNcommandBuffer* _cmdBuf, float depthValue, NVNboolean depthMask, int stencilValue, int stencilMask) {
    std::cout << "nvnCommandBufferClearDepthStencil called!" << std::endl;
    auto cmdBuf = UNWRAP(_cmdBuf);
}

void nvnCommandBufferDispatchCompute(NVNcommandBuffer* _cmdBuf, int groupsX, int groupsY, int groupsZ) {
    std::cout << "nvnCommandBufferDispatchCompute(groupsX=" << groupsX << ", groupsY=" << groupsY << ", groupsZ=" << groupsZ << ") called!" << std::endl;
    auto cmdBuf = UNWRAP(_cmdBuf);
}

void nvnCommandBufferDispatchComputeIndirect(NVNcommandBuffer* _cmdBuf, NVNbufferAddress buffer) {
    std::cout << "nvnCommandBufferDispatchComputeIndirect called!" << std::endl;
    auto cmdBuf = UNWRAP(_cmdBuf);
}

void nvnCommandBufferSetViewport(NVNcommandBuffer* _cmdBuf, int x, int y, int w, int h) {
    std::cout << "nvnCommandBufferSetViewport(x=" << x << ", y=" << y << ", w=" << w << ", h=" << h << ") called!" << std::endl;
    auto cmdBuf = UNWRAP(_cmdBuf);
}

void nvnCommandBufferSetViewports(NVNcommandBuffer* _cmdBuf, int first, int count, const float* ranges) {
    std::cout << "nvnCommandBufferSetViewports(first=" << first << ", count=" << count << ", ranges=" << std::hex << reinterpret_cast<uint64_t>(ranges) << std::dec << ") called!" << std::endl;
    auto cmdBuf = UNWRAP(_cmdBuf);
}

void nvnCommandBufferSetViewportSwizzles(NVNcommandBuffer* _cmdBuf, int first, int count, const NVNviewportSwizzle* swizzles) {
    std::cout << "nvnCommandBufferSetViewportSwizzles called!" << std::endl;
    auto cmdBuf = UNWRAP(_cmdBuf);
}

void nvnCommandBufferSetScissor(NVNcommandBuffer* _cmdBuf, int x, int y, int w, int h) {
    std::cout << "nvnCommandBufferSetScissor(x=" << x << ", y=" << y << ", w=" << w << ", h=" << h << ") called!" << std::endl;
    auto cmdBuf = UNWRAP(_cmdBuf);
}

void nvnCommandBufferSetScissors(NVNcommandBuffer* _cmdBuf, int first, int count, const int* rects) {
    std::cout << "nvnCommandBufferSetScissors(first=" << first << ", count=" << count << ", rects=" << std::hex << reinterpret_cast<uint64_t>(rects) << std::dec << ") called!" << std::endl;
    auto cmdBuf = UNWRAP(_cmdBuf);
}

void nvnCommandBufferSetDepthRange(NVNcommandBuffer* _cmdBuf, float n, float f) {
    std::cout << "nvnCommandBufferSetDepthRange called!" << std::endl;
    auto cmdBuf = UNWRAP(_cmdBuf);
}

void nvnCommandBufferSetDepthBounds(NVNcommandBuffer* _cmdBuf, NVNboolean enable, float n, float f) {
    std::cout << "nvnCommandBufferSetDepthBounds called!" << std::endl;
    auto cmdBuf = UNWRAP(_cmdBuf);
}

void nvnCommandBufferSetDepthRanges(NVNcommandBuffer* _cmdBuf, int first, int count, const float* ranges) {
    std::cout << "nvnCommandBufferSetDepthRanges called!" << std::endl;
    auto cmdBuf = UNWRAP(_cmdBuf);
}

void nvnCommandBufferSetTiledCacheAction(NVNcommandBuffer* _cmdBuf, NVNtiledCacheAction action) {
    std::cout << "nvnCommandBufferSetTiledCacheAction called!" << std::endl;
    auto cmdBuf = UNWRAP(_cmdBuf);
}

void nvnCommandBufferSetTiledCacheTileSize(NVNcommandBuffer* _cmdBuf, int w, int h) {
    std::cout << "nvnCommandBufferSetTiledCacheTileSize called!" << std::endl;
    auto cmdBuf = UNWRAP(_cmdBuf);
}

void nvnCommandBufferBindSeparateTexture(NVNcommandBuffer* _cmdBuf, NVNshaderStage stage, int i, NVNseparateTextureHandle handle) {
    std::cout << "nvnCommandBufferBindSeparateTexture called!" << std::endl;
    auto cmdBuf = UNWRAP(_cmdBuf);
}

void nvnCommandBufferBindSeparateSampler(NVNcommandBuffer* _cmdBuf, NVNshaderStage stage, int i, NVNseparateSamplerHandle handle) {
    std::cout << "nvnCommandBufferBindSeparateSampler called!" << std::endl;
    auto cmdBuf = UNWRAP(_cmdBuf);
}

void nvnCommandBufferBindSeparateTextures(NVNcommandBuffer* _cmdBuf, NVNshaderStage stage, int i1, int i2, const NVNseparateTextureHandle* handle) {
    std::cout << "nvnCommandBufferBindSeparateTextures called!" << std::endl;
    auto cmdBuf = UNWRAP(_cmdBuf);
}

void nvnCommandBufferBindSeparateSamplers(NVNcommandBuffer* _cmdBuf, NVNshaderStage stage, int i1, int i2, const NVNseparateSamplerHandle* handle) {
    std::cout << "nvnCommandBufferBindSeparateSamplers called!" << std::endl;
    auto cmdBuf = UNWRAP(_cmdBuf);
}

void nvnCommandBufferSetStencilValueMask(NVNcommandBuffer* _cmdBuf, NVNface faces, int mask) {
    std::cout << "nvnCommandBufferSetStencilValueMask called!" << std::endl;
    auto cmdBuf = UNWRAP(_cmdBuf);
}

void nvnCommandBufferSetStencilMask(NVNcommandBuffer* _cmdBuf, NVNface faces, int mask) {
    std::cout << "nvnCommandBufferSetStencilMask called!" << std::endl;
    auto cmdBuf = UNWRAP(_cmdBuf);
}

void nvnCommandBufferSetStencilRef(NVNcommandBuffer* _cmdBuf, NVNface faces, int ref) {
    std::cout << "nvnCommandBufferSetStencilRef called!" << std::endl;
    auto cmdBuf = UNWRAP(_cmdBuf);
}

void nvnCommandBufferSetStencilCullCriteria(NVNcommandBuffer* _cmdBuf) {
    std::cout << "nvnCommandBufferSetStencilCullCriteria called!" << std::endl;
    auto cmdBuf = UNWRAP(_cmdBuf);
}

void nvnCommandBufferSetBlendColor(NVNcommandBuffer* _cmdBuf, const float* blendColor) {
    std::cout << "nvnCommandBufferSetBlendColor called!" << std::endl;
    auto cmdBuf = UNWRAP(_cmdBuf);
}

void nvnCommandBufferSetColorReductionEnable(NVNcommandBuffer* _cmdBuf, NVNboolean enable) {
    std::cout << "nvnCommandBufferSetColorReductionEnable called!" << std::endl;
    auto cmdBuf = UNWRAP(_cmdBuf);
}

void nvnCommandBufferSetColorReductionThresholds(NVNcommandBuffer* _cmdBuf, float f) {
    std::cout << "nvnCommandBufferSetColorReductionThresholds called!" << std::endl;
    auto cmdBuf = UNWRAP(_cmdBuf);
}

void nvnCommandBufferSetPointSize(NVNcommandBuffer* _cmdBuf, float pointSize) {
    std::cout << "nvnCommandBufferSetPointSize called!" << std::endl;
    auto cmdBuf = UNWRAP(_cmdBuf);
}

void nvnCommandBufferSetLineWidth(NVNcommandBuffer* _cmdBuf, float lineWidth) {
    std::cout << "nvnCommandBufferSetLineWidth called!" << std::endl;
    auto cmdBuf = UNWRAP(_cmdBuf);
}

void nvnCommandBufferSetPolygonOffsetClamp(NVNcommandBuffer* _cmdBuf, float factor, float units, float clamp) {
    std::cout << "nvnCommandBufferSetPolygonOffsetClamp called!" << std::endl;
    auto cmdBuf = UNWRAP(_cmdBuf);
}

void nvnCommandBufferSetAlphaRef(NVNcommandBuffer* _cmdBuf, float ref) {
    std::cout << "nvnCommandBufferSetAlphaRef called!" << std::endl;
    auto cmdBuf = UNWRAP(_cmdBuf);
}

void nvnCommandBufferSetSampleMask(NVNcommandBuffer* _cmdBuf, int mask) {
    std::cout << "nvnCommandBufferSetSampleMask called!" << std::endl;
    auto cmdBuf = UNWRAP(_cmdBuf);
}

void nvnCommandBufferSetRasterizerDiscard(NVNcommandBuffer* _cmdBuf, NVNboolean discard) {
    std::cout << "nvnCommandBufferSetRasterizerDiscard called!" << std::endl;
    auto cmdBuf = UNWRAP(_cmdBuf);
}

void nvnCommandBufferSetDepthClamp(NVNcommandBuffer* _cmdBuf, NVNboolean clamp) {
    std::cout << "nvnCommandBufferSetDepthClamp called!" << std::endl;
    auto cmdBuf = UNWRAP(_cmdBuf);
}

void nvnCommandBufferSetConservativeRasterEnable(NVNcommandBuffer* _cmdBuf, NVNboolean enable) {
    std::cout << "nvnCommandBufferSetConservativeRasterEnable called!" << std::endl;
    auto cmdBuf = UNWRAP(_cmdBuf);
}

void nvnCommandBufferSetConservativeRasterDilate(NVNcommandBuffer* _cmdBuf, float f) {
    std::cout << "nvnCommandBufferSetConservativeRasterDilate called!" << std::endl;
    auto cmdBuf = UNWRAP(_cmdBuf);
}

void nvnCommandBufferSetSubpixelPrecisionBias(NVNcommandBuffer* _cmdBuf, int i1, int i2) {
    std::cout << "nvnCommandBufferSetSubpixelPrecisionBias called!" << std::endl;
    auto cmdBuf = UNWRAP(_cmdBuf);
}

void nvnCommandBufferSetCommandMemoryCallbackEnabled(NVNcommandBuffer* _cmdBuf, NVNboolean enable) {
    std::cout << "nvnCommandBufferSetCommandMemoryCallbackEnabled called!" << std::endl;
    auto cmdBuf = UNWRAP(_cmdBuf);
}

void nvnCommandBufferCopyBufferToTexture(NVNcommandBuffer* _cmdBuf, NVNbufferAddress buffer, NVNtexture* _dstTexture, NVNtextureView* _dstView, NVNcopyRegion* dstRegion, int flags) {
    std::cout << "nvnCommandBufferCopyBufferToTexture called!" << std::endl;
    auto cmdBuf = UNWRAP(_cmdBuf);
    auto dstTexture = UNWRAP(_dstTexture);
    auto dstView = UNWRAP(_dstView);
}

void nvnCommandBufferCopyTextureToBuffer(NVNcommandBuffer* _cmdBuf, NVNtexture* _srcTexture, NVNtextureView* _srcView, NVNcopyRegion* srcRegion, NVNbufferAddress buffer, int flags) {
    std::cout << "nvnCommandBufferCopyTextureToBuffer called!" << std::endl;
    auto cmdBuf = UNWRAP(_cmdBuf);
    auto srcTexture = UNWRAP(_srcTexture);
    auto srcView = UNWRAP(_srcView);
}

void nvnCommandBufferCopyTextureToTexture(NVNcommandBuffer* _cmdBuf, NVNtexture* _srcTexture, NVNtextureView* _srcView, NVNcopyRegion* srcRegion, NVNtexture* _dstTexture, NVNtextureView* _dstView, NVNcopyRegion* dstRegion, int flags) {
    std::cout << "nvnCommandBufferCopyTextureToTexture called!" << std::endl;
    auto cmdBuf = UNWRAP(_cmdBuf);
    auto srcTexture = UNWRAP(_srcTexture);
    auto srcView = UNWRAP(_srcView);
    auto dstTexture = UNWRAP(_dstTexture);
    auto dstView = UNWRAP(_dstView);
}

void nvnCommandBufferCopyBufferToBuffer(NVNcommandBuffer* _cmdBuf, NVNbufferAddress src, NVNbufferAddress dst, size_t size, int flags) {
    std::cout << "nvnCommandBufferCopyBufferToBuffer called!" << std::endl;
    auto cmdBuf = UNWRAP(_cmdBuf);
}

void nvnCommandBufferClearBuffer(NVNcommandBuffer* _cmdBuf, NVNbufferAddress buffer, size_t size, uint32_t i) {
    std::cout << "nvnCommandBufferClearBuffer called!" << std::endl;
    auto cmdBuf = UNWRAP(_cmdBuf);
}

void nvnCommandBufferClearTexture(NVNcommandBuffer* _cmdBuf, NVNtexture* _dstTexture, NVNtextureView* _dstView, NVNcopyRegion* dstRegion, const float* color, int mask) {
    std::cout << "nvnCommandBufferClearTexture called!" << std::endl;
    auto cmdBuf = UNWRAP(_cmdBuf);
    auto dstTexture = UNWRAP(_dstTexture);
    auto dstView = UNWRAP(_dstView);
}

void nvnCommandBufferClearTexturei(NVNcommandBuffer* _cmdBuf, NVNtexture* _dstTexture, NVNtextureView* _dstView, NVNcopyRegion* dstRegion, const int* color, int mask) {
    std::cout << "nvnCommandBufferClearTexturei called!" << std::endl;
    auto cmdBuf = UNWRAP(_cmdBuf);
    auto dstTexture = UNWRAP(_dstTexture);
    auto dstView = UNWRAP(_dstView);
}

void nvnCommandBufferClearTextureui(NVNcommandBuffer* _cmdBuf, NVNtexture* _dstTexture, NVNtextureView* _dstView, NVNcopyRegion* dstRegion, const uint32_t* color, int mask) {
    std::cout << "nvnCommandBufferClearTextureui called!" << std::endl;
    auto cmdBuf = UNWRAP(_cmdBuf);
    auto dstTexture = UNWRAP(_dstTexture);
    auto dstView = UNWRAP(_dstView);
}

void nvnCommandBufferUpdateUniformBuffer(NVNcommandBuffer* _cmdBuf, NVNbufferAddress buffer, size_t size, ptrdiff_t o, size_t s, const void* p) {
    std::cout << "nvnCommandBufferUpdateUniformBuffer called!" << std::endl;
    auto cmdBuf = UNWRAP(_cmdBuf);
}

void nvnCommandBufferReportCounter(NVNcommandBuffer* _cmdBuf, NVNcounterType counter, NVNbufferAddress buffer) {
    std::cout << "nvnCommandBufferReportCounter called!" << std::endl;
    auto cmdBuf = UNWRAP(_cmdBuf);
}

void nvnCommandBufferResetCounter(NVNcommandBuffer* _cmdBuf, NVNcounterType counter) {
    std::cout << "nvnCommandBufferResetCounter called!" << std::endl;
    auto cmdBuf = UNWRAP(_cmdBuf);
}

void nvnCommandBufferReportValue(NVNcommandBuffer* _cmdBuf, uint32_t value, NVNbufferAddress buffer) {
    std::cout << "nvnCommandBufferReportValue called!" << std::endl;
    auto cmdBuf = UNWRAP(_cmdBuf);
}

void nvnCommandBufferSetRenderEnable(NVNcommandBuffer* _cmdBuf, NVNboolean enable) {
    std::cout << "nvnCommandBufferSetRenderEnable called!" << std::endl;
    auto cmdBuf = UNWRAP(_cmdBuf);
}

void nvnCommandBufferSetRenderEnableConditional(NVNcommandBuffer* _cmdBuf, NVNconditionalRenderMode mode, NVNbufferAddress addr) {
    std::cout << "nvnCommandBufferSetRenderEnableConditional called!" << std::endl;
    auto cmdBuf = UNWRAP(_cmdBuf);
}

void nvnCommandBufferSetRenderTargets(NVNcommandBuffer* _cmdBuf, int numColors, NVNtexture* const* _colors, NVNtextureView* const* _colorViews, NVNtexture* _depthStencil, NVNtextureView* _depthStencilView) {
    std::cout << "nvnCommandBufferSetRenderTargets(numColors=" << numColors << ", colors=" << std::hex << reinterpret_cast<uint64_t>(_colors) << ", colorViews=" << reinterpret_cast<uint64_t>(_colorViews) << ", depthStencil=" << reinterpret_cast<uint64_t>(_depthStencil) << ", depthStencilView=" << reinterpret_cast<uint64_t>(_depthStencilView) << std::dec << ") called!" << std::endl;
    auto cmdBuf = UNWRAP(_cmdBuf);
    auto depthStencil = UNWRAP(_depthStencil);
    auto depthStencilView = UNWRAP(_depthStencilView);
}

void nvnCommandBufferDiscardColor(NVNcommandBuffer* _cmdBuf, int i) {
    std::cout << "nvnCommandBufferDiscardColor called!" << std::endl;
    auto cmdBuf = UNWRAP(_cmdBuf);
}

void nvnCommandBufferDiscardDepthStencil(NVNcommandBuffer* _cmdBuf) {
    std::cout << "nvnCommandBufferDiscardDepthStencil called!" << std::endl;
    auto cmdBuf = UNWRAP(_cmdBuf);
}

void nvnCommandBufferDownsample(NVNcommandBuffer* _cmdBuf, NVNtexture* _src, NVNtexture* _dst) {
    std::cout << "nvnCommandBufferDownsample called!" << std::endl;
    auto cmdBuf = UNWRAP(_cmdBuf);
    auto src = UNWRAP(_src);
    auto dst = UNWRAP(_dst);
}

void nvnCommandBufferTiledDownsample(NVNcommandBuffer* _cmdBuf, NVNtexture* _texture1, NVNtexture* _texture2) {
    std::cout << "nvnCommandBufferTiledDownsample called!" << std::endl;
    auto cmdBuf = UNWRAP(_cmdBuf);
    auto texture1 = UNWRAP(_texture1);
    auto texture2 = UNWRAP(_texture2);
}

void nvnCommandBufferDownsampleTextureView(NVNcommandBuffer* _cmdBuf, NVNtexture* _texture1, NVNtextureView* _view1, NVNtexture* _texture2, NVNtextureView* _view2) {
    std::cout << "nvnCommandBufferDownsampleTextureView called!" << std::endl;
    auto cmdBuf = UNWRAP(_cmdBuf);
    auto texture1 = UNWRAP(_texture1);
    auto view1 = UNWRAP(_view1);
    auto texture2 = UNWRAP(_texture2);
    auto view2 = UNWRAP(_view2);
}

void nvnCommandBufferTiledDownsampleTextureView(NVNcommandBuffer* _cmdBuf, NVNtexture* _texture1, NVNtextureView* _view1, NVNtexture* _texture2, NVNtextureView* _view2) {
    std::cout << "nvnCommandBufferTiledDownsampleTextureView called!" << std::endl;
    auto cmdBuf = UNWRAP(_cmdBuf);
    auto texture1 = UNWRAP(_texture1);
    auto view1 = UNWRAP(_view1);
    auto texture2 = UNWRAP(_texture2);
    auto view2 = UNWRAP(_view2);
}

void nvnCommandBufferBarrier(NVNcommandBuffer* _cmdBuf, int barrier) {
    std::cout << "nvnCommandBufferBarrier called!" << std::endl;
    auto cmdBuf = UNWRAP(_cmdBuf);
}

void nvnCommandBufferWaitSync(NVNcommandBuffer* _cmdBuf, NVNsync* _sync) {
    std::cout << "nvnCommandBufferWaitSync called!" << std::endl;
    auto cmdBuf = UNWRAP(_cmdBuf);
    auto sync = UNWRAP(_sync);
}

void nvnCommandBufferFenceSync(NVNcommandBuffer* _cmdBuf, NVNsync* _sync, NVNsyncCondition condition, int fence) {
    std::cout << "nvnCommandBufferFenceSync called!" << std::endl;
    auto cmdBuf = UNWRAP(_cmdBuf);
    auto sync = UNWRAP(_sync);
}

void nvnCommandBufferSetTexturePool(NVNcommandBuffer* _cmdBuf, NVNtexturePool* _pool) {
    std::cout << "nvnCommandBufferSetTexturePool called!" << std::endl;
    auto cmdBuf = UNWRAP(_cmdBuf);
    auto pool = UNWRAP(_pool);
}

void nvnCommandBufferSetSamplerPool(NVNcommandBuffer* _cmdBuf, NVNsamplerPool* _pool) {
    std::cout << "nvnCommandBufferSetSamplerPool called!" << std::endl;
    auto cmdBuf = UNWRAP(_cmdBuf);
    auto pool = UNWRAP(_pool);
}

void nvnCommandBufferSetShaderScratchMemory(NVNcommandBuffer* _cmdBuf, NVNmemoryPool* _pool, ptrdiff_t offset, size_t size) {
    std::cout << "nvnCommandBufferSetShaderScratchMemory called!" << std::endl;
    auto cmdBuf = UNWRAP(_cmdBuf);
    auto pool = UNWRAP(_pool);
}

void nvnCommandBufferSaveZCullData(NVNcommandBuffer* _cmdBuf, NVNbufferAddress addr, size_t size) {
    std::cout << "nvnCommandBufferSaveZCullData called!" << std::endl;
    auto cmdBuf = UNWRAP(_cmdBuf);
}

void nvnCommandBufferRestoreZCullData(NVNcommandBuffer* _cmdBuf, NVNbufferAddress addr, size_t size) {
    std::cout << "nvnCommandBufferRestoreZCullData called!" << std::endl;
    auto cmdBuf = UNWRAP(_cmdBuf);
}

void nvnCommandBufferSetCopyRowStride(NVNcommandBuffer* _cmdBuf, ptrdiff_t stride) {
    std::cout << "nvnCommandBufferSetCopyRowStride called!" << std::endl;
    auto cmdBuf = UNWRAP(_cmdBuf);
}

void nvnCommandBufferSetCopyImageStride(NVNcommandBuffer* _cmdBuf, ptrdiff_t stride) {
    std::cout << "nvnCommandBufferSetCopyImageStride called!" << std::endl;
    auto cmdBuf = UNWRAP(_cmdBuf);
}

ptrdiff_t nvnCommandBufferGetCopyRowStride(NVNcommandBuffer* _cmdBuf) {
    std::cout << "nvnCommandBufferGetCopyRowStride called!" << std::endl;
    auto cmdBuf = UNWRAP(_cmdBuf);
    return 0;
}

ptrdiff_t nvnCommandBufferGetCopyImageStride(NVNcommandBuffer* _cmdBuf) {
    std::cout << "nvnCommandBufferGetCopyImageStride called!" << std::endl;
    auto cmdBuf = UNWRAP(_cmdBuf);
    return 0;
}

void nvnCommandBufferDrawTexture(NVNcommandBuffer* _cmdBuf, NVNtextureHandle handle, const NVNdrawTextureRegion* region1, const NVNdrawTextureRegion* region2) {
    std::cout << "nvnCommandBufferDrawTexture called!" << std::endl;
    auto cmdBuf = UNWRAP(_cmdBuf);
}

NVNboolean nvnProgramSetSubroutineLinkage(NVNprogram* _program, int i, const NVNsubroutineLinkageMapPtr* ptr) {
    std::cout << "nvnProgramSetSubroutineLinkage called!" << std::endl;
    auto program = UNWRAP(_program);
    return 1;
}

void nvnCommandBufferSetProgramSubroutines(NVNcommandBuffer* _cmdBuf, NVNprogram* _program, NVNshaderStage stage, const int i1, const int i2, const int* i3) {
    std::cout << "nvnCommandBufferSetProgramSubroutines called!" << std::endl;
    auto cmdBuf = UNWRAP(_cmdBuf);
    auto program = UNWRAP(_program);
}

void nvnCommandBufferBindCoverageModulationTable(NVNcommandBuffer* _cmdBuf, const float* f) {
    std::cout << "nvnCommandBufferBindCoverageModulationTable called!" << std::endl;
    auto cmdBuf = UNWRAP(_cmdBuf);
}

void nvnCommandBufferResolveDepthBuffer(NVNcommandBuffer* _cmdBuf) {
    std::cout << "nvnCommandBufferResolveDepthBuffer called!" << std::endl;
    auto cmdBuf = UNWRAP(_cmdBuf);
}

void nvnCommandBufferPushDebugGroupStatic(NVNcommandBuffer* _cmdBuf, uint32_t i, const char* description) {
    std::cout << "nvnCommandBufferPushDebugGroupStatic called!" << std::endl;
    auto cmdBuf = UNWRAP(_cmdBuf);
}

void nvnCommandBufferPushDebugGroupDynamic(NVNcommandBuffer* _cmdBuf, uint32_t i, const char* description) {
    std::cout << "nvnCommandBufferPushDebugGroupDynamic called!" << std::endl;
    auto cmdBuf = UNWRAP(_cmdBuf);
}

void nvnCommandBufferPushDebugGroup(NVNcommandBuffer* _cmdBuf, uint32_t i, const char* description) {
    std::cout << "nvnCommandBufferPushDebugGroup called!" << std::endl;
    auto cmdBuf = UNWRAP(_cmdBuf);
}

void nvnCommandBufferPopDebugGroup(NVNcommandBuffer* _cmdBuf) {
    std::cout << "nvnCommandBufferPopDebugGroup called!" << std::endl;
    auto cmdBuf = UNWRAP(_cmdBuf);
}

void nvnCommandBufferPopDebugGroupId(NVNcommandBuffer* _cmdBuf, uint32_t i) {
    std::cout << "nvnCommandBufferPopDebugGroupId called!" << std::endl;
    auto cmdBuf = UNWRAP(_cmdBuf);
}

void nvnCommandBufferInsertDebugMarkerStatic(NVNcommandBuffer* _cmdBuf, uint32_t i, const char* description) {
    std::cout << "nvnCommandBufferInsertDebugMarkerStatic called!" << std::endl;
    auto cmdBuf = UNWRAP(_cmdBuf);
}

void nvnCommandBufferInsertDebugMarkerDynamic(NVNcommandBuffer* _cmdBuf, uint32_t i, const char* description) {
    std::cout << "nvnCommandBufferInsertDebugMarkerDynamic called!" << std::endl;
    auto cmdBuf = UNWRAP(_cmdBuf);
}

void nvnCommandBufferInsertDebugMarker(NVNcommandBuffer* _cmdBuf, const char* description) {
    std::cout << "nvnCommandBufferInsertDebugMarker called!" << std::endl;
    auto cmdBuf = UNWRAP(_cmdBuf);
}

PFNNVNCOMMANDBUFFERMEMORYCALLBACKPROC nvnCommandBufferGetMemoryCallback(NVNcommandBuffer* _cmdBuf) {
    std::cout << "nvnCommandBufferGetMemoryCallback called!" << std::endl;
    auto cmdBuf = UNWRAP(_cmdBuf);
    return nullptr;
}

void nvnCommandBufferGetMemoryCallbackData(NVNcommandBuffer* _cmdBuf) {
    std::cout << "nvnCommandBufferGetMemoryCallbackData called!" << std::endl;
    auto cmdBuf = UNWRAP(_cmdBuf);
}

NVNboolean nvnCommandBufferIsRecording(NVNcommandBuffer* _cmdBuf) {
    std::cout << "nvnCommandBufferIsRecording called!" << std::endl;
    auto cmdBuf = UNWRAP(_cmdBuf);
    return 0;
}

void nvnCommandBufferWaitEvent(NVNcommandBuffer* _cmdBuf, NVNevent* _event, NVNeventWaitMode mode, uint32_t i) {
    std::cout << "nvnCommandBufferWaitEvent called!" << std::endl;
    auto cmdBuf = UNWRAP(_cmdBuf);
    auto event = UNWRAP(_event);
}

void nvnCommandBufferSignalEvent(NVNcommandBuffer* _cmdBuf, NVNevent* _event, NVNeventSignalMode mode, NVNeventSignalLocation location, int flags, uint32_t i) {
    std::cout << "nvnCommandBufferSignalEvent called!" << std::endl;
    auto cmdBuf = UNWRAP(_cmdBuf);
    auto event = UNWRAP(_event);
}
