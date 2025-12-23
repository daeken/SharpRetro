#include <iostream>
#include "nv.h"

NVNboolean nvnCommandBufferInitialize(NVNcommandBuffer* cmdBuf, NVNdevice* device) {
    std::cout << "nvnCommandBufferInitialize called!" << std::endl;
    return 1;
}

void nvnCommandBufferFinalize(NVNcommandBuffer* cmdBuf) {
    std::cout << "nvnCommandBufferFinalize called!" << std::endl;
}

void nvnCommandBufferSetDebugLabel(NVNcommandBuffer* cmdBuf, const char* label) {
    std::cout << "nvnCommandBufferSetDebugLabel called!" << std::endl;
}

void nvnCommandBufferSetMemoryCallback(NVNcommandBuffer* cmdBuf, PFNNVNCOMMANDBUFFERMEMORYCALLBACKPROC callback) {
    std::cout << "nvnCommandBufferSetMemoryCallback called!" << std::endl;
}

void nvnCommandBufferSetMemoryCallbackData(NVNcommandBuffer* cmdBuf, void* callbackData) {
    std::cout << "nvnCommandBufferSetMemoryCallbackData called!" << std::endl;
}

void nvnCommandBufferAddCommandMemory(NVNcommandBuffer* cmdBuf, const NVNmemoryPool* pool, ptrdiff_t offset, size_t size) {
    std::cout << "nvnCommandBufferAddCommandMemory called!" << std::endl;
}

void nvnCommandBufferAddControlMemory(NVNcommandBuffer* cmdBuf, void* memory, size_t size) {
    std::cout << "nvnCommandBufferAddControlMemory called!" << std::endl;
}

size_t nvnCommandBufferGetCommandMemorySize(const NVNcommandBuffer* cmdBuf) {
    std::cout << "nvnCommandBufferGetCommandMemorySize called!" << std::endl;
    return 0;
}

size_t nvnCommandBufferGetCommandMemoryUsed(const NVNcommandBuffer* cmdBuf) {
    std::cout << "nvnCommandBufferGetCommandMemoryUsed called!" << std::endl;
    return 0;
}

size_t nvnCommandBufferGetCommandMemoryFree(const NVNcommandBuffer* cmdBuf) {
    std::cout << "nvnCommandBufferGetCommandMemoryFree called!" << std::endl;
    return 0;
}

size_t nvnCommandBufferGetControlMemorySize(const NVNcommandBuffer* cmdBuf) {
    std::cout << "nvnCommandBufferGetControlMemorySize called!" << std::endl;
    return 0;
}

size_t nvnCommandBufferGetControlMemoryUsed(const NVNcommandBuffer* cmdBuf) {
    std::cout << "nvnCommandBufferGetControlMemoryUsed called!" << std::endl;
    return 0;
}

size_t nvnCommandBufferGetControlMemoryFree(const NVNcommandBuffer* cmdBuf) {
    std::cout << "nvnCommandBufferGetControlMemoryFree called!" << std::endl;
    return 0;
}

void nvnCommandBufferBeginRecording(NVNcommandBuffer* cmdBuf) {
    std::cout << "nvnCommandBufferBeginRecording called!" << std::endl;
}

NVNcommandHandle nvnCommandBufferEndRecording(NVNcommandBuffer* cmdBuf) {
    std::cout << "nvnCommandBufferEndRecording called!" << std::endl;
    return 0;
}

void nvnCommandBufferCallCommands(NVNcommandBuffer* cmdBuf, int numCommands, const NVNcommandHandle* handles) {
    std::cout << "nvnCommandBufferCallCommands called!" << std::endl;
}

void nvnCommandBufferCopyCommands(NVNcommandBuffer* cmdBuf, int numCommands, const NVNcommandHandle* handles) {
    std::cout << "nvnCommandBufferCopyCommands called!" << std::endl;
}

void nvnCommandBufferBindBlendState(NVNcommandBuffer* cmdBuf, const NVNblendState* blend) {
    std::cout << "nvnCommandBufferBindBlendState called!" << std::endl;
}

void nvnCommandBufferBindChannelMaskState(NVNcommandBuffer* cmdBuf, const NVNchannelMaskState* channelMask) {
    std::cout << "nvnCommandBufferBindChannelMaskState called!" << std::endl;
}

void nvnCommandBufferBindColorState(NVNcommandBuffer* cmdBuf, const NVNcolorState* color) {
    std::cout << "nvnCommandBufferBindColorState called!" << std::endl;
}

void nvnCommandBufferBindMultisampleState(NVNcommandBuffer* cmdBuf, const NVNmultisampleState* multisample) {
    std::cout << "nvnCommandBufferBindMultisampleState called!" << std::endl;
}

void nvnCommandBufferBindPolygonState(NVNcommandBuffer* cmdBuf, const NVNpolygonState* polygon) {
    std::cout << "nvnCommandBufferBindPolygonState called!" << std::endl;
}

void nvnCommandBufferBindDepthStencilState(NVNcommandBuffer* cmdBuf, const NVNdepthStencilState* depthStencil) {
    std::cout << "nvnCommandBufferBindDepthStencilState called!" << std::endl;
}

void nvnCommandBufferBindVertexAttribState(NVNcommandBuffer* cmdBuf, int numAttribs, const NVNvertexAttribState* attribs) {
    std::cout << "nvnCommandBufferBindVertexAttribState called!" << std::endl;
}

void nvnCommandBufferBindVertexStreamState(NVNcommandBuffer* cmdBuf, int numStreams, const NVNvertexStreamState* streams) {
    std::cout << "nvnCommandBufferBindVertexStreamState called!" << std::endl;
}

void nvnCommandBufferBindProgram(NVNcommandBuffer* cmdBuf, const NVNprogram* program, int stages) {
    std::cout << "nvnCommandBufferBindProgram(program=" << std::hex << reinterpret_cast<uint64_t>(program) << ", stages=" << std::dec << stages << ") called!" << std::endl;
}

void nvnCommandBufferBindVertexBuffer(NVNcommandBuffer* cmdBuf, int index, NVNbufferAddress buffer, size_t size) {
    std::cout << "nvnCommandBufferBindVertexBuffer(index=" << index << ", buffer=" << std::hex << buffer << ", size=" << std::dec << size << ") called!" << std::endl;
}

void nvnCommandBufferBindVertexBuffers(NVNcommandBuffer* cmdBuf, int first, int count, const NVNbufferRange* buffers) {
    std::cout << "nvnCommandBufferBindVertexBuffers(first=" << first << ", count=" << count << ", buffers=" << std::hex << reinterpret_cast<uint64_t>(buffers) << std::dec << ") called!" << std::endl;
}

void nvnCommandBufferBindUniformBuffer(NVNcommandBuffer* cmdBuf, NVNshaderStage stage, int index, NVNbufferAddress buffer, size_t size) {
    std::cout << "nvnCommandBufferBindUniformBuffer(stage=" << stage << ", index=" << index << ", buffer=" << std::hex << buffer << ", size=" << std::dec << size << ") called!" << std::endl;
}

void nvnCommandBufferBindUniformBuffers(NVNcommandBuffer* cmdBuf, NVNshaderStage stage, int first, int count, const NVNbufferRange* buffers) {
    std::cout << "nvnCommandBufferBindUniformBuffers(stage=" << stage << ", first=" << first << ", count=" << count << ", buffers=" << std::hex << reinterpret_cast<uint64_t>(buffers) << std::dec << ") called!" << std::endl;
}

void nvnCommandBufferBindTransformFeedbackBuffer(NVNcommandBuffer* cmdBuf, int index, NVNbufferAddress buffer, size_t size) {
    std::cout << "nvnCommandBufferBindTransformFeedbackBuffer called!" << std::endl;
}

void nvnCommandBufferBindTransformFeedbackBuffers(NVNcommandBuffer* cmdBuf, int first, int count, const NVNbufferRange* buffers) {
    std::cout << "nvnCommandBufferBindTransformFeedbackBuffers called!" << std::endl;
}

void nvnCommandBufferBindStorageBuffer(NVNcommandBuffer* cmdBuf, NVNshaderStage stage, int index, NVNbufferAddress buffer, size_t size) {
    std::cout << "nvnCommandBufferBindStorageBuffer called!" << std::endl;
}

void nvnCommandBufferBindStorageBuffers(NVNcommandBuffer* cmdBuf, NVNshaderStage stage, int first, int count, const NVNbufferRange* buffers) {
    std::cout << "nvnCommandBufferBindStorageBuffers called!" << std::endl;
}

void nvnCommandBufferBindTexture(NVNcommandBuffer* cmdBuf, NVNshaderStage stage, int index, NVNtextureHandle texture) {
    std::cout << "nvnCommandBufferBindTexture(stage=" << stage << ", index=" << index << ", texture=" << std::hex << texture << std::dec << ") called!" << std::endl;
}

void nvnCommandBufferBindTextures(NVNcommandBuffer* cmdBuf, NVNshaderStage stage, int first, int count, const NVNtextureHandle* textures) {
    std::cout << "nvnCommandBufferBindTextures(stage=" << stage << ", first=" << first << ", count=" << count << ", textures=" << std::hex << reinterpret_cast<uint64_t>(textures) << std::dec << ") called!" << std::endl;
}

void nvnCommandBufferBindImage(NVNcommandBuffer* cmdBuf, NVNshaderStage stage, int index, NVNimageHandle image) {
    std::cout << "nvnCommandBufferBindImage(stage=" << stage << ", index=" << index << ", image=" << std::hex << image << std::dec << ") called!" << std::endl;
}

void nvnCommandBufferBindImages(NVNcommandBuffer* cmdBuf, NVNshaderStage stage, int first, int count, const NVNimageHandle* images) {
    std::cout << "nvnCommandBufferBindImages(stage=" << stage << ", first=" << first << ", count=" << count << ", images=" << std::hex << reinterpret_cast<uint64_t>(images) << std::dec << ") called!" << std::endl;
}

void nvnCommandBufferSetPatchSize(NVNcommandBuffer* cmdBuf, int i) {
    std::cout << "nvnCommandBufferSetPatchSize called!" << std::endl;
}

void nvnCommandBufferSetInnerTessellationLevels(NVNcommandBuffer* cmdBuf, const float* f) {
    std::cout << "nvnCommandBufferSetInnerTessellationLevels called!" << std::endl;
}

void nvnCommandBufferSetOuterTessellationLevels(NVNcommandBuffer* cmdBuf, const float* f) {
    std::cout << "nvnCommandBufferSetOuterTessellationLevels called!" << std::endl;
}

void nvnCommandBufferSetPrimitiveRestart(NVNcommandBuffer* cmdBuf, NVNboolean b, int i) {
    std::cout << "nvnCommandBufferSetPrimitiveRestart called!" << std::endl;
}

void nvnCommandBufferBeginTransformFeedback(NVNcommandBuffer* cmdBuf, NVNbufferAddress buffer) {
    std::cout << "nvnCommandBufferBeginTransformFeedback called!" << std::endl;
}

void nvnCommandBufferEndTransformFeedback(NVNcommandBuffer* cmdBuf, NVNbufferAddress buffer) {
    std::cout << "nvnCommandBufferEndTransformFeedback called!" << std::endl;
}

void nvnCommandBufferPauseTransformFeedback(NVNcommandBuffer* cmdBuf, NVNbufferAddress buffer) {
    std::cout << "nvnCommandBufferPauseTransformFeedback called!" << std::endl;
}

void nvnCommandBufferResumeTransformFeedback(NVNcommandBuffer* cmdBuf, NVNbufferAddress buffer) {
    std::cout << "nvnCommandBufferResumeTransformFeedback called!" << std::endl;
}

void nvnCommandBufferDrawTransformFeedback(NVNcommandBuffer* cmdBuf, NVNdrawPrimitive mode, NVNbufferAddress buffer) {
    std::cout << "nvnCommandBufferDrawTransformFeedback called!" << std::endl;
}

void nvnCommandBufferDrawArrays(NVNcommandBuffer* cmdBuf, NVNdrawPrimitive mode, int first, int count) {
    std::cout << "nvnCommandBufferDrawArrays(mode=" << mode << ", first=" << first << ", count=" << count << ") called!" << std::endl;
}

void nvnCommandBufferDrawElements(NVNcommandBuffer* cmdBuf, NVNdrawPrimitive mode, NVNindexType type, int count, NVNbufferAddress indexBuffer) {
    std::cout << "nvnCommandBufferDrawElements(mode=" << mode << ", type=" << type << ", count=" << count << ", indexBuffer=" << std::hex << indexBuffer << std::dec << ") called!" << std::endl;
}

void nvnCommandBufferDrawElementsBaseVertex(NVNcommandBuffer* cmdBuf, NVNdrawPrimitive mode, NVNindexType type, int count, NVNbufferAddress indexBuffer, int baseVertex) {
    std::cout << "nvnCommandBufferDrawElementsBaseVertex(mode=" << mode << ", type=" << type << ", count=" << count << ", indexBuffer=" << std::hex << indexBuffer << ", baseVertex=" << std::dec << baseVertex << ") called!" << std::endl;
}

void nvnCommandBufferDrawArraysInstanced(NVNcommandBuffer* cmdBuf, NVNdrawPrimitive mode, int first, int count, int baseInstance, int instanceCount) {
    std::cout << "nvnCommandBufferDrawArraysInstanced(mode=" << mode << ", first=" << first << ", count=" << count << ", baseInstance=" << baseInstance << ", instanceCount=" << instanceCount << ") called!" << std::endl;
}

void nvnCommandBufferDrawElementsInstanced(NVNcommandBuffer* cmdBuf, NVNdrawPrimitive mode, NVNindexType type, int count, NVNbufferAddress indexBuffer, int baseVertex, int baseInstance, int instanceCount) {
    std::cout << "nvnCommandBufferDrawElementsInstanced(mode=" << mode << ", type=" << type << ", count=" << count << ", indexBuffer=" << std::hex << indexBuffer << ", baseVertex=" << std::dec << baseVertex << ", baseInstance=" << baseInstance << ", instanceCount=" << instanceCount << ") called!" << std::endl;
}

void nvnCommandBufferDrawArraysIndirect(NVNcommandBuffer* cmdBuf, NVNdrawPrimitive mode, NVNbufferAddress buffer) {
    std::cout << "nvnCommandBufferDrawArraysIndirect called!" << std::endl;
}

void nvnCommandBufferDrawElementsIndirect(NVNcommandBuffer* cmdBuf, NVNdrawPrimitive mode, NVNindexType type, NVNbufferAddress buffer1, NVNbufferAddress buffer2) {
    std::cout << "nvnCommandBufferDrawElementsIndirect called!" << std::endl;
}

void nvnCommandBufferMultiDrawArraysIndirectCount(NVNcommandBuffer* cmdBuf, NVNdrawPrimitive mode, NVNbufferAddress buffer1, NVNbufferAddress buffer2, int i, ptrdiff_t o) {
    std::cout << "nvnCommandBufferMultiDrawArraysIndirectCount called!" << std::endl;
}

void nvnCommandBufferMultiDrawElementsIndirectCount(NVNcommandBuffer* cmdBuf, NVNdrawPrimitive mode, NVNindexType type, NVNbufferAddress buffer1, NVNbufferAddress buffer2, NVNbufferAddress buffer3, int i, ptrdiff_t o) {
    std::cout << "nvnCommandBufferMultiDrawElementsIndirectCount called!" << std::endl;
}

void nvnCommandBufferClearColor(NVNcommandBuffer* cmdBuf, int index, const float* color, int mask) {
    std::cout << "nvnCommandBufferClearColor called!" << std::endl;
}

void nvnCommandBufferClearColori(NVNcommandBuffer* cmdBuf, int index, const int* color, int mask) {
    std::cout << "nvnCommandBufferClearColori called!" << std::endl;
}

void nvnCommandBufferClearColorui(NVNcommandBuffer* cmdBuf, int index, const uint32_t* color, int mask) {
    std::cout << "nvnCommandBufferClearColorui called!" << std::endl;
}

void nvnCommandBufferClearDepthStencil(NVNcommandBuffer* cmdBuf, float depthValue, NVNboolean depthMask, int stencilValue, int stencilMask) {
    std::cout << "nvnCommandBufferClearDepthStencil called!" << std::endl;
}

void nvnCommandBufferDispatchCompute(NVNcommandBuffer* cmdBuf, int groupsX, int groupsY, int groupsZ) {
    std::cout << "nvnCommandBufferDispatchCompute(groupsX=" << groupsX << ", groupsY=" << groupsY << ", groupsZ=" << groupsZ << ") called!" << std::endl;
}

void nvnCommandBufferDispatchComputeIndirect(NVNcommandBuffer* cmdBuf, NVNbufferAddress buffer) {
    std::cout << "nvnCommandBufferDispatchComputeIndirect called!" << std::endl;
}

void nvnCommandBufferSetViewport(NVNcommandBuffer* cmdBuf, int x, int y, int w, int h) {
    std::cout << "nvnCommandBufferSetViewport(x=" << x << ", y=" << y << ", w=" << w << ", h=" << h << ") called!" << std::endl;
}

void nvnCommandBufferSetViewports(NVNcommandBuffer* cmdBuf, int first, int count, const float* ranges) {
    std::cout << "nvnCommandBufferSetViewports(first=" << first << ", count=" << count << ", ranges=" << std::hex << reinterpret_cast<uint64_t>(ranges) << std::dec << ") called!" << std::endl;
}

void nvnCommandBufferSetViewportSwizzles(NVNcommandBuffer* cmdBuf, int first, int count, const NVNviewportSwizzle* swizzles) {
    std::cout << "nvnCommandBufferSetViewportSwizzles called!" << std::endl;
}

void nvnCommandBufferSetScissor(NVNcommandBuffer* cmdBuf, int x, int y, int w, int h) {
    std::cout << "nvnCommandBufferSetScissor(x=" << x << ", y=" << y << ", w=" << w << ", h=" << h << ") called!" << std::endl;
}

void nvnCommandBufferSetScissors(NVNcommandBuffer* cmdBuf, int first, int count, const int* rects) {
    std::cout << "nvnCommandBufferSetScissors(first=" << first << ", count=" << count << ", rects=" << std::hex << reinterpret_cast<uint64_t>(rects) << std::dec << ") called!" << std::endl;
}

void nvnCommandBufferSetDepthRange(NVNcommandBuffer* cmdBuf, float n, float f) {
    std::cout << "nvnCommandBufferSetDepthRange called!" << std::endl;
}

void nvnCommandBufferSetDepthBounds(NVNcommandBuffer* cmdBuf, NVNboolean enable, float n, float f) {
    std::cout << "nvnCommandBufferSetDepthBounds called!" << std::endl;
}

void nvnCommandBufferSetDepthRanges(NVNcommandBuffer* cmdBuf, int first, int count, const float* ranges) {
    std::cout << "nvnCommandBufferSetDepthRanges called!" << std::endl;
}

void nvnCommandBufferSetTiledCacheAction(NVNcommandBuffer* cmdBuf, NVNtiledCacheAction action) {
    std::cout << "nvnCommandBufferSetTiledCacheAction called!" << std::endl;
}

void nvnCommandBufferSetTiledCacheTileSize(NVNcommandBuffer* cmdBuf, int w, int h) {
    std::cout << "nvnCommandBufferSetTiledCacheTileSize called!" << std::endl;
}

void nvnCommandBufferBindSeparateTexture(NVNcommandBuffer* cmdBuf, NVNshaderStage stage, int i, NVNseparateTextureHandle handle) {
    std::cout << "nvnCommandBufferBindSeparateTexture called!" << std::endl;
}

void nvnCommandBufferBindSeparateSampler(NVNcommandBuffer* cmdBuf, NVNshaderStage stage, int i, NVNseparateSamplerHandle handle) {
    std::cout << "nvnCommandBufferBindSeparateSampler called!" << std::endl;
}

void nvnCommandBufferBindSeparateTextures(NVNcommandBuffer* cmdBuf, NVNshaderStage stage, int i1, int i2, const NVNseparateTextureHandle* handle) {
    std::cout << "nvnCommandBufferBindSeparateTextures called!" << std::endl;
}

void nvnCommandBufferBindSeparateSamplers(NVNcommandBuffer* cmdBuf, NVNshaderStage stage, int i1, int i2, const NVNseparateSamplerHandle* handle) {
    std::cout << "nvnCommandBufferBindSeparateSamplers called!" << std::endl;
}

void nvnCommandBufferSetStencilValueMask(NVNcommandBuffer* cmdBuf, NVNface faces, int mask) {
    std::cout << "nvnCommandBufferSetStencilValueMask called!" << std::endl;
}

void nvnCommandBufferSetStencilMask(NVNcommandBuffer* cmdBuf, NVNface faces, int mask) {
    std::cout << "nvnCommandBufferSetStencilMask called!" << std::endl;
}

void nvnCommandBufferSetStencilRef(NVNcommandBuffer* cmdBuf, NVNface faces, int ref) {
    std::cout << "nvnCommandBufferSetStencilRef called!" << std::endl;
}

void nvnCommandBufferSetBlendColor(NVNcommandBuffer* cmdBuf, const float* blendColor) {
    std::cout << "nvnCommandBufferSetBlendColor called!" << std::endl;
}

void nvnCommandBufferSetPointSize(NVNcommandBuffer* cmdBuf, float pointSize) {
    std::cout << "nvnCommandBufferSetPointSize called!" << std::endl;
}

void nvnCommandBufferSetLineWidth(NVNcommandBuffer* cmdBuf, float lineWidth) {
    std::cout << "nvnCommandBufferSetLineWidth called!" << std::endl;
}

void nvnCommandBufferSetPolygonOffsetClamp(NVNcommandBuffer* cmdBuf, float factor, float units, float clamp) {
    std::cout << "nvnCommandBufferSetPolygonOffsetClamp called!" << std::endl;
}

void nvnCommandBufferSetAlphaRef(NVNcommandBuffer* cmdBuf, float ref) {
    std::cout << "nvnCommandBufferSetAlphaRef called!" << std::endl;
}

void nvnCommandBufferSetSampleMask(NVNcommandBuffer* cmdBuf, int mask) {
    std::cout << "nvnCommandBufferSetSampleMask called!" << std::endl;
}

void nvnCommandBufferSetRasterizerDiscard(NVNcommandBuffer* cmdBuf, NVNboolean discard) {
    std::cout << "nvnCommandBufferSetRasterizerDiscard called!" << std::endl;
}

void nvnCommandBufferSetDepthClamp(NVNcommandBuffer* cmdBuf, NVNboolean clamp) {
    std::cout << "nvnCommandBufferSetDepthClamp called!" << std::endl;
}

void nvnCommandBufferSetConservativeRasterEnable(NVNcommandBuffer* cmdBuf, NVNboolean enable) {
    std::cout << "nvnCommandBufferSetConservativeRasterEnable called!" << std::endl;
}

void nvnCommandBufferSetConservativeRasterDilate(NVNcommandBuffer* cmdBuf, float f) {
    std::cout << "nvnCommandBufferSetConservativeRasterDilate called!" << std::endl;
}

void nvnCommandBufferSetSubpixelPrecisionBias(NVNcommandBuffer* cmdBuf, int i1, int i2) {
    std::cout << "nvnCommandBufferSetSubpixelPrecisionBias called!" << std::endl;
}

void nvnCommandBufferCopyBufferToTexture(NVNcommandBuffer* cmdBuf, NVNbufferAddress buffer, const NVNtexture* dstTexture, const NVNtextureView* dstView, const NVNcopyRegion* dstRegion, int flags) {
    std::cout << "nvnCommandBufferCopyBufferToTexture called!" << std::endl;
}

void nvnCommandBufferCopyTextureToBuffer(NVNcommandBuffer* cmdBuf, const NVNtexture* srcTexture, const NVNtextureView* srcView, const NVNcopyRegion* srcRegion, NVNbufferAddress buffer, int flags) {
    std::cout << "nvnCommandBufferCopyTextureToBuffer called!" << std::endl;
}

void nvnCommandBufferCopyTextureToTexture(NVNcommandBuffer* cmdBuf, const NVNtexture* srcTexture, const NVNtextureView* srcView, const NVNcopyRegion* srcRegion, const NVNtexture* dstTexture, const NVNtextureView* dstView, const NVNcopyRegion* dstRegion, int flags) {
    std::cout << "nvnCommandBufferCopyTextureToTexture called!" << std::endl;
}

void nvnCommandBufferCopyBufferToBuffer(NVNcommandBuffer* cmdBuf, NVNbufferAddress src, NVNbufferAddress dst, size_t size, int flags) {
    std::cout << "nvnCommandBufferCopyBufferToBuffer called!" << std::endl;
}

void nvnCommandBufferClearBuffer(NVNcommandBuffer* cmdBuf, NVNbufferAddress buffer, size_t size, uint32_t i) {
    std::cout << "nvnCommandBufferClearBuffer called!" << std::endl;
}

void nvnCommandBufferClearTexture(NVNcommandBuffer* cmdBuf, const NVNtexture* dstTexture, const NVNtextureView* dstView, const NVNcopyRegion* dstRegion, const float* color, int mask) {
    std::cout << "nvnCommandBufferClearTexture called!" << std::endl;
}

void nvnCommandBufferClearTexturei(NVNcommandBuffer* cmdBuf, const NVNtexture* dstTexture, const NVNtextureView* dstView, const NVNcopyRegion* dstRegion, const int* color, int mask) {
    std::cout << "nvnCommandBufferClearTexturei called!" << std::endl;
}

void nvnCommandBufferClearTextureui(NVNcommandBuffer* cmdBuf, const NVNtexture* dstTexture, const NVNtextureView* dstView, const NVNcopyRegion* dstRegion, const uint32_t* color, int mask) {
    std::cout << "nvnCommandBufferClearTextureui called!" << std::endl;
}

void nvnCommandBufferUpdateUniformBuffer(NVNcommandBuffer* cmdBuf, NVNbufferAddress buffer, size_t size, ptrdiff_t o, size_t s, const void* p) {
    std::cout << "nvnCommandBufferUpdateUniformBuffer called!" << std::endl;
}

void nvnCommandBufferReportCounter(NVNcommandBuffer* cmdBuf, NVNcounterType counter, NVNbufferAddress buffer) {
    std::cout << "nvnCommandBufferReportCounter called!" << std::endl;
}

void nvnCommandBufferResetCounter(NVNcommandBuffer* cmdBuf, NVNcounterType counter) {
    std::cout << "nvnCommandBufferResetCounter called!" << std::endl;
}

void nvnCommandBufferReportValue(NVNcommandBuffer* cmdBuf, uint32_t value, NVNbufferAddress buffer) {
    std::cout << "nvnCommandBufferReportValue called!" << std::endl;
}

void nvnCommandBufferSetRenderEnable(NVNcommandBuffer* cmdBuf, NVNboolean enable) {
    std::cout << "nvnCommandBufferSetRenderEnable called!" << std::endl;
}

void nvnCommandBufferSetRenderEnableConditional(NVNcommandBuffer* cmdBuf, NVNconditionalRenderMode mode, NVNbufferAddress addr) {
    std::cout << "nvnCommandBufferSetRenderEnableConditional called!" << std::endl;
}

void nvnCommandBufferSetRenderTargets(NVNcommandBuffer* cmdBuf, int numColors, const NVNtexture* const* colors, const NVNtextureView* const* colorViews, const NVNtexture* depthStencil, const NVNtextureView* depthStencilView) {
    std::cout << "nvnCommandBufferSetRenderTargets(numColors=" << numColors << ", colors=" << std::hex << reinterpret_cast<uint64_t>(colors) << ", colorViews=" << reinterpret_cast<uint64_t>(colorViews) << ", depthStencil=" << reinterpret_cast<uint64_t>(depthStencil) << ", depthStencilView=" << reinterpret_cast<uint64_t>(depthStencilView) << std::dec << ") called!" << std::endl;
}

void nvnCommandBufferDiscardColor(NVNcommandBuffer* cmdBuf, int i) {
    std::cout << "nvnCommandBufferDiscardColor called!" << std::endl;
}

void nvnCommandBufferDiscardDepthStencil(NVNcommandBuffer* cmdBuf) {
    std::cout << "nvnCommandBufferDiscardDepthStencil called!" << std::endl;
}

void nvnCommandBufferDownsample(NVNcommandBuffer* cmdBuf, const NVNtexture* src, const NVNtexture* dst) {
    std::cout << "nvnCommandBufferDownsample called!" << std::endl;
}

void nvnCommandBufferTiledDownsample(NVNcommandBuffer* cmdBuf, const NVNtexture* texture1, const NVNtexture* texture2) {
    std::cout << "nvnCommandBufferTiledDownsample called!" << std::endl;
}

void nvnCommandBufferDownsampleTextureView(NVNcommandBuffer* cmdBuf, const NVNtexture* texture1, const NVNtextureView* view1, const NVNtexture* texture2, const NVNtextureView* view2) {
    std::cout << "nvnCommandBufferDownsampleTextureView called!" << std::endl;
}

void nvnCommandBufferTiledDownsampleTextureView(NVNcommandBuffer* cmdBuf, const NVNtexture* texture1, const NVNtextureView* view1, const NVNtexture* texture2, const NVNtextureView* view2) {
    std::cout << "nvnCommandBufferTiledDownsampleTextureView called!" << std::endl;
}

void nvnCommandBufferBarrier(NVNcommandBuffer* cmdBuf, int barrier) {
    std::cout << "nvnCommandBufferBarrier called!" << std::endl;
}

void nvnCommandBufferWaitSync(NVNcommandBuffer* cmdBuf, const NVNsync* sync) {
    std::cout << "nvnCommandBufferWaitSync called!" << std::endl;
}

void nvnCommandBufferFenceSync(NVNcommandBuffer* cmdBuf, NVNsync* sync, NVNsyncCondition condition, int fence) {
    std::cout << "nvnCommandBufferFenceSync called!" << std::endl;
}

void nvnCommandBufferSetTexturePool(NVNcommandBuffer* cmdBuf, const NVNtexturePool* pool) {
    std::cout << "nvnCommandBufferSetTexturePool called!" << std::endl;
}

void nvnCommandBufferSetSamplerPool(NVNcommandBuffer* cmdBuf, const NVNsamplerPool* pool) {
    std::cout << "nvnCommandBufferSetSamplerPool called!" << std::endl;
}

void nvnCommandBufferSetShaderScratchMemory(NVNcommandBuffer* cmdBuf, const NVNmemoryPool* pool, ptrdiff_t offset, size_t size) {
    std::cout << "nvnCommandBufferSetShaderScratchMemory called!" << std::endl;
}

void nvnCommandBufferSaveZCullData(NVNcommandBuffer* cmdBuf, NVNbufferAddress addr, size_t size) {
    std::cout << "nvnCommandBufferSaveZCullData called!" << std::endl;
}

void nvnCommandBufferRestoreZCullData(NVNcommandBuffer* cmdBuf, NVNbufferAddress addr, size_t size) {
    std::cout << "nvnCommandBufferRestoreZCullData called!" << std::endl;
}

void nvnCommandBufferSetCopyRowStride(NVNcommandBuffer* cmdBuf, ptrdiff_t stride) {
    std::cout << "nvnCommandBufferSetCopyRowStride called!" << std::endl;
}

void nvnCommandBufferSetCopyImageStride(NVNcommandBuffer* cmdBuf, ptrdiff_t stride) {
    std::cout << "nvnCommandBufferSetCopyImageStride called!" << std::endl;
}

ptrdiff_t nvnCommandBufferGetCopyRowStride(const NVNcommandBuffer* cmdBuf) {
    std::cout << "nvnCommandBufferGetCopyRowStride called!" << std::endl;
    return 0;
}

ptrdiff_t nvnCommandBufferGetCopyImageStride(const NVNcommandBuffer* cmdBuf) {
    std::cout << "nvnCommandBufferGetCopyImageStride called!" << std::endl;
    return 0;
}

void nvnCommandBufferDrawTexture(NVNcommandBuffer* cmdBuf, NVNtextureHandle handle, const NVNdrawTextureRegion* region1, const NVNdrawTextureRegion* region2) {
    std::cout << "nvnCommandBufferDrawTexture called!" << std::endl;
}

NVNboolean nvnProgramSetSubroutineLinkage(NVNprogram* program, int i, const NVNsubroutineLinkageMapPtr* ptr) {
    std::cout << "nvnProgramSetSubroutineLinkage called!" << std::endl;
    return 1;
}

void nvnCommandBufferSetProgramSubroutines(NVNcommandBuffer* cmdBuf, NVNprogram* program, NVNshaderStage stage, const int i1, const int i2, const int* i3) {
    std::cout << "nvnCommandBufferSetProgramSubroutines called!" << std::endl;
}

void nvnCommandBufferBindCoverageModulationTable(NVNcommandBuffer* cmdBuf, const float* f) {
    std::cout << "nvnCommandBufferBindCoverageModulationTable called!" << std::endl;
}

void nvnCommandBufferResolveDepthBuffer(NVNcommandBuffer* cmdBuf) {
    std::cout << "nvnCommandBufferResolveDepthBuffer called!" << std::endl;
}

void nvnCommandBufferPushDebugGroupStatic(NVNcommandBuffer* cmdBuf, uint32_t i, const char* description) {
    std::cout << "nvnCommandBufferPushDebugGroupStatic called!" << std::endl;
}

void nvnCommandBufferPushDebugGroupDynamic(NVNcommandBuffer* cmdBuf, uint32_t i, const char* description) {
    std::cout << "nvnCommandBufferPushDebugGroupDynamic called!" << std::endl;
}

void nvnCommandBufferPushDebugGroup(NVNcommandBuffer* cmdBuf, uint32_t i, const char* description) {
    std::cout << "nvnCommandBufferPushDebugGroup called!" << std::endl;
}

void nvnCommandBufferPopDebugGroup(NVNcommandBuffer* cmdBuf) {
    std::cout << "nvnCommandBufferPopDebugGroup called!" << std::endl;
}

void nvnCommandBufferPopDebugGroupId(NVNcommandBuffer* cmdBuf, uint32_t i) {
    std::cout << "nvnCommandBufferPopDebugGroupId called!" << std::endl;
}

void nvnCommandBufferInsertDebugMarkerStatic(NVNcommandBuffer* cmdBuf, uint32_t i, const char* description) {
    std::cout << "nvnCommandBufferInsertDebugMarkerStatic called!" << std::endl;
}

void nvnCommandBufferInsertDebugMarkerDynamic(NVNcommandBuffer* cmdBuf, uint32_t i, const char* description) {
    std::cout << "nvnCommandBufferInsertDebugMarkerDynamic called!" << std::endl;
}

void nvnCommandBufferInsertDebugMarker(NVNcommandBuffer* cmdBuf, const char* description) {
    std::cout << "nvnCommandBufferInsertDebugMarker called!" << std::endl;
}

PFNNVNCOMMANDBUFFERMEMORYCALLBACKPROC nvnCommandBufferGetMemoryCallback(const NVNcommandBuffer* cmdBuf) {
    std::cout << "nvnCommandBufferGetMemoryCallback called!" << std::endl;
    return nullptr;
}

void nvnCommandBufferGetMemoryCallbackData(const NVNcommandBuffer* cmdBuf) {
    std::cout << "nvnCommandBufferGetMemoryCallbackData called!" << std::endl;
}

NVNboolean nvnCommandBufferIsRecording(const NVNcommandBuffer* cmdBuf) {
    std::cout << "nvnCommandBufferIsRecording called!" << std::endl;
    return 0;
}

void nvnCommandBufferWaitEvent(NVNcommandBuffer* cmdBuf, const NVNevent* event, NVNeventWaitMode mode, uint32_t i) {
    std::cout << "nvnCommandBufferWaitEvent called!" << std::endl;
}

void nvnCommandBufferSignalEvent(NVNcommandBuffer* cmdBuf, const NVNevent* event, NVNeventSignalMode mode, NVNeventSignalLocation location, int flags, uint32_t i) {
    std::cout << "nvnCommandBufferSignalEvent called!" << std::endl;
}