﻿// Nu Game Engine.
// Copyright (C) Bryan Edds, 2013-2023.

namespace OpenGL
open System
open Prime
open Nu

[<RequireQualifiedAccess>]
module Framebuffer =

    /// Attempt to create texture 2d buffers.
    let TryCreateTextureBuffers () =

        // create frame buffer object
        let framebuffer = Gl.GenFramebuffer ()
        Gl.BindFramebuffer (FramebufferTarget.Framebuffer, framebuffer)
        Hl.Assert ()

        // create texture 2d buffer
        let textureId = Gl.GenTexture ()
        Gl.BindTexture (TextureTarget.Texture2d, textureId)
        Gl.TexImage2D (TextureTarget.Texture2d, 0, InternalFormat.Rgba32f, Constants.Render.ResolutionX, Constants.Render.ResolutionY, 0, PixelFormat.Rgba, PixelType.Float, nativeint 0)
        Gl.TexParameter (TextureTarget.Texture2d, TextureParameterName.TextureMinFilter, int TextureMinFilter.Nearest)
        Gl.TexParameter (TextureTarget.Texture2d, TextureParameterName.TextureMagFilter, int TextureMagFilter.Nearest)
        Gl.FramebufferTexture2D (FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2d, textureId, 0)
        Hl.Assert ()

        // create depth and stencil buffers
        let depthStencilBuffer = Gl.GenRenderbuffer ()
        Gl.BindRenderbuffer (RenderbufferTarget.Renderbuffer, depthStencilBuffer)
        Gl.RenderbufferStorage (RenderbufferTarget.Renderbuffer, InternalFormat.Depth24Stencil8, Constants.Render.ResolutionX, Constants.Render.ResolutionY)
        Gl.FramebufferRenderbuffer (FramebufferTarget.Framebuffer, FramebufferAttachment.DepthStencilAttachment, RenderbufferTarget.Renderbuffer, depthStencilBuffer)
        Hl.Assert ()

        // ensure framebuffer is complete
        if Gl.CheckFramebufferStatus FramebufferTarget.Framebuffer = FramebufferStatus.FramebufferComplete then
            let texture = Texture.CreateTextureFromId textureId
            Right (texture, framebuffer)
        else Left ("Could not create complete texture 2d framebuffer.")

    /// Destroy texture buffers.
    let DestroyTextureBuffers (position, framebuffer) =
        Gl.DeleteFramebuffers [|framebuffer|]
        Texture.DestroyTexture position

    /// Create filter box 1d buffers.
    let TryCreateFilterBox1dBuffers (resolutionX, resolutionY) =

        // create frame buffer object
        let framebuffer = Gl.GenFramebuffer ()
        Gl.BindFramebuffer (FramebufferTarget.Framebuffer, framebuffer)
        Hl.Assert ()

        // create filter box buffer
        let filterBoxId = Gl.GenTexture ()
        Gl.BindTexture (TextureTarget.Texture2d, filterBoxId)
        Gl.TexImage2D (TextureTarget.Texture2d, 0, InternalFormat.R32f, resolutionX, resolutionY, 0, PixelFormat.Rgba, PixelType.Float, nativeint 0)
        Gl.TexParameter (TextureTarget.Texture2d, TextureParameterName.TextureMinFilter, int TextureMinFilter.Nearest)
        Gl.TexParameter (TextureTarget.Texture2d, TextureParameterName.TextureMagFilter, int TextureMagFilter.Nearest)
        Gl.TexParameter (TextureTarget.Texture2d, TextureParameterName.TextureWrapS, int TextureWrapMode.ClampToEdge)
        Gl.TexParameter (TextureTarget.Texture2d, TextureParameterName.TextureWrapT, int TextureWrapMode.ClampToEdge)
        Gl.FramebufferTexture2D (FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2d, filterBoxId, 0)
        Hl.Assert ()

        // associate draw buffers
        Gl.DrawBuffers [|int FramebufferAttachment.ColorAttachment0|]
        Hl.Assert ()

        // create render buffer with depth and stencil
        let renderbuffer = Gl.GenRenderbuffer ()
        Gl.BindRenderbuffer (RenderbufferTarget.Renderbuffer, renderbuffer)
        Gl.RenderbufferStorage (RenderbufferTarget.Renderbuffer, InternalFormat.Depth24Stencil8, resolutionX, resolutionY)
        Gl.FramebufferRenderbuffer (FramebufferTarget.Framebuffer, FramebufferAttachment.DepthStencilAttachment, RenderbufferTarget.Renderbuffer, renderbuffer)
        Hl.Assert ()

        // ensure framebuffer is complete
        if Gl.CheckFramebufferStatus FramebufferTarget.Framebuffer = FramebufferStatus.FramebufferComplete then
            let filterBox = Texture.CreateTextureFromId filterBoxId
            Right (filterBox, renderbuffer, framebuffer)
        else Left "Could not create complete filter box 1d framebuffer."

    /// Destroy filter box 1d buffers.
    let DestroyFilterBox1dBuffers (filterBoxBlurTexture : Texture.Texture, renderbuffer, framebuffer) =
        Gl.DeleteRenderbuffers [|renderbuffer|]
        Gl.DeleteFramebuffers [|framebuffer|]
        Texture.DestroyTexture filterBoxBlurTexture

    /// Create filter gaussian 2d buffers.
    let TryCreateFilterGaussian2dBuffers (resolutionX, resolutionY) =

        // create frame buffer object
        let framebuffer = Gl.GenFramebuffer ()
        Gl.BindFramebuffer (FramebufferTarget.Framebuffer, framebuffer)
        Hl.Assert ()

        // create filter gaussian buffer
        let filterGaussianId = Gl.GenTexture ()
        Gl.BindTexture (TextureTarget.Texture2d, filterGaussianId)
        Gl.TexImage2D (TextureTarget.Texture2d, 0, InternalFormat.Rg32f, resolutionX, resolutionY, 0, PixelFormat.Red, PixelType.Float, nativeint 0)
        Gl.TexParameter (TextureTarget.Texture2d, TextureParameterName.TextureMinFilter, int TextureMinFilter.Nearest)
        Gl.TexParameter (TextureTarget.Texture2d, TextureParameterName.TextureMagFilter, int TextureMagFilter.Nearest)
        Gl.TexParameter (TextureTarget.Texture2d, TextureParameterName.TextureWrapS, int TextureWrapMode.ClampToEdge)
        Gl.TexParameter (TextureTarget.Texture2d, TextureParameterName.TextureWrapT, int TextureWrapMode.ClampToEdge)
        Gl.FramebufferTexture2D (FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2d, filterGaussianId, 0)
        Hl.Assert ()

        // associate draw buffers
        Gl.DrawBuffers [|int FramebufferAttachment.ColorAttachment0|]
        Hl.Assert ()

        // create render buffer with depth and stencil
        let renderbuffer = Gl.GenRenderbuffer ()
        Gl.BindRenderbuffer (RenderbufferTarget.Renderbuffer, renderbuffer)
        Gl.RenderbufferStorage (RenderbufferTarget.Renderbuffer, InternalFormat.Depth24Stencil8, resolutionX, resolutionY)
        Gl.FramebufferRenderbuffer (FramebufferTarget.Framebuffer, FramebufferAttachment.DepthStencilAttachment, RenderbufferTarget.Renderbuffer, renderbuffer)
        Hl.Assert ()

        // ensure framebuffer is complete
        if Gl.CheckFramebufferStatus FramebufferTarget.Framebuffer = FramebufferStatus.FramebufferComplete then
            let filterGaussian = Texture.CreateTextureFromId filterGaussianId
            Right (filterGaussian, renderbuffer, framebuffer)
        else Left "Could not create complete filter gaussian 2d framebuffer."

    /// Destroy filter gaussian 2d buffers.
    let DestroyFilterGaussian2dBuffers (filterGaussianTexture : Texture.Texture, renderbuffer, framebuffer) =
        Gl.DeleteRenderbuffers [|renderbuffer|]
        Gl.DeleteFramebuffers [|framebuffer|]
        Texture.DestroyTexture filterGaussianTexture

    /// Create filter buffers.
    let TryCreateFilterBuffers (resolutionX, resolutionY) =

        // create frame buffer object
        let framebuffer = Gl.GenFramebuffer ()
        Gl.BindFramebuffer (FramebufferTarget.Framebuffer, framebuffer)
        Hl.Assert ()

        // create filter buffer
        let filterId = Gl.GenTexture ()
        Gl.BindTexture (TextureTarget.Texture2d, filterId)
        Gl.TexImage2D (TextureTarget.Texture2d, 0, InternalFormat.Rgba32f, resolutionX, resolutionY, 0, PixelFormat.Rgba, PixelType.Float, nativeint 0)
        Gl.TexParameter (TextureTarget.Texture2d, TextureParameterName.TextureMinFilter, int TextureMinFilter.Nearest)
        Gl.TexParameter (TextureTarget.Texture2d, TextureParameterName.TextureMagFilter, int TextureMagFilter.Nearest)
        Gl.TexParameter (TextureTarget.Texture2d, TextureParameterName.TextureWrapS, int TextureWrapMode.ClampToEdge)
        Gl.TexParameter (TextureTarget.Texture2d, TextureParameterName.TextureWrapT, int TextureWrapMode.ClampToEdge)
        Gl.FramebufferTexture2D (FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2d, filterId, 0)
        Hl.Assert ()

        // associate draw buffers
        Gl.DrawBuffers [|int FramebufferAttachment.ColorAttachment0|]
        Hl.Assert ()

        // create render buffer with depth and stencil
        let renderbuffer = Gl.GenRenderbuffer ()
        Gl.BindRenderbuffer (RenderbufferTarget.Renderbuffer, renderbuffer)
        Gl.RenderbufferStorage (RenderbufferTarget.Renderbuffer, InternalFormat.Depth24Stencil8, resolutionX, resolutionY)
        Gl.FramebufferRenderbuffer (FramebufferTarget.Framebuffer, FramebufferAttachment.DepthStencilAttachment, RenderbufferTarget.Renderbuffer, renderbuffer)
        Hl.Assert ()

        // ensure framebuffer is complete
        if Gl.CheckFramebufferStatus FramebufferTarget.Framebuffer = FramebufferStatus.FramebufferComplete then
            let filter = Texture.CreateTextureFromId filterId
            Right (filter, renderbuffer, framebuffer)
        else Left "Could not create complete filter framebuffer."

    /// Destroy filter buffers.
    let DestroyFilterBuffers (filter : Texture.Texture, renderbuffer, framebuffer) =
        Gl.DeleteRenderbuffers [|renderbuffer|]
        Gl.DeleteFramebuffers [|framebuffer|]
        Texture.DestroyTexture filter

    /// Attempt to create hdr buffers.
    let TryCreateHdrBuffers () =

        // create frame buffer object
        let framebuffer = Gl.GenFramebuffer ()
        Gl.BindFramebuffer (FramebufferTarget.Framebuffer, framebuffer)
        Hl.Assert ()

        // create position buffer
        let positionId = Gl.GenTexture ()
        Gl.BindTexture (TextureTarget.Texture2d, positionId)
        Gl.TexImage2D (TextureTarget.Texture2d, 0, InternalFormat.Rgba32f, Constants.Render.ResolutionX, Constants.Render.ResolutionY, 0, PixelFormat.Rgba, PixelType.Float, nativeint 0)
        Gl.TexParameter (TextureTarget.Texture2d, TextureParameterName.TextureMinFilter, int TextureMinFilter.Nearest)
        Gl.TexParameter (TextureTarget.Texture2d, TextureParameterName.TextureMagFilter, int TextureMagFilter.Nearest)
        Gl.FramebufferTexture2D (FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2d, positionId, 0)
        Hl.Assert ()

        // create depth and stencil buffers
        let depthStencilBuffer = Gl.GenRenderbuffer ()
        Gl.BindRenderbuffer (RenderbufferTarget.Renderbuffer, depthStencilBuffer)
        Gl.RenderbufferStorage (RenderbufferTarget.Renderbuffer, InternalFormat.Depth24Stencil8, Constants.Render.ResolutionX, Constants.Render.ResolutionY)
        Gl.FramebufferRenderbuffer (FramebufferTarget.Framebuffer, FramebufferAttachment.DepthStencilAttachment, RenderbufferTarget.Renderbuffer, depthStencilBuffer)
        Hl.Assert ()

        // ensure framebuffer is complete
        if Gl.CheckFramebufferStatus FramebufferTarget.Framebuffer = FramebufferStatus.FramebufferComplete then
            let position = Texture.CreateTextureFromId positionId
            Right (position, framebuffer)
        else Left ("Could not create complete HDR framebuffer.")

    /// Destroy hdr buffers.
    let DestroyHdrFrameBuffers (position, framebuffer) =
        Gl.DeleteFramebuffers [|framebuffer|]
        Texture.DestroyTexture position

    /// Create shadow buffers.
    let TryCreateShadowBuffers (shadowResolutionX, shadowResolutionY) =

        // create frame buffer object
        let framebuffer = Gl.GenFramebuffer ()
        Gl.BindFramebuffer (FramebufferTarget.Framebuffer, framebuffer)
        Hl.Assert ()

        // create shadow texture
        let shadowTextureId = Gl.GenTexture ()
        Gl.BindTexture (TextureTarget.Texture2d, shadowTextureId)
        Gl.TexImage2D (TextureTarget.Texture2d, 0, InternalFormat.Rg32f, shadowResolutionX, shadowResolutionY, 0, PixelFormat.Rgba, PixelType.Float, nativeint 0)
        Gl.TexParameter (TextureTarget.Texture2d, TextureParameterName.TextureMinFilter, int TextureMinFilter.Linear)
        Gl.TexParameter (TextureTarget.Texture2d, TextureParameterName.TextureMagFilter, int TextureMagFilter.Linear)
        Gl.TexParameter (TextureTarget.Texture2d, TextureParameterName.TextureWrapS, int TextureWrapMode.ClampToEdge)
        Gl.TexParameter (TextureTarget.Texture2d, TextureParameterName.TextureWrapT, int TextureWrapMode.ClampToEdge)
        Gl.FramebufferTexture2D (FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2d, shadowTextureId, 0)
        Hl.Assert ()

        // associate draw buffers
        Gl.DrawBuffers [|int FramebufferAttachment.ColorAttachment0|]
        Hl.Assert ()

        // create render buffer with depth and stencil
        let renderbuffer = Gl.GenRenderbuffer ()
        Gl.BindRenderbuffer (RenderbufferTarget.Renderbuffer, renderbuffer)
        Gl.RenderbufferStorage (RenderbufferTarget.Renderbuffer, InternalFormat.Depth24Stencil8, shadowResolutionX, shadowResolutionY)
        Gl.FramebufferRenderbuffer (FramebufferTarget.Framebuffer, FramebufferAttachment.DepthStencilAttachment, RenderbufferTarget.Renderbuffer, renderbuffer)
        Hl.Assert ()

        // ensure framebuffer is complete
        if Gl.CheckFramebufferStatus FramebufferTarget.Framebuffer = FramebufferStatus.FramebufferComplete then
            let shadowTexture = Texture.CreateTextureFromId shadowTextureId
            Right (shadowTexture, renderbuffer, framebuffer)
        else Left "Could not create complete shadow texture framebuffer."

    /// Destroy shadow buffers.
    let DestroyShadowBuffers (shadowTexture : Texture.Texture, renderbuffer, framebuffer) =
        Gl.DeleteRenderbuffers [|renderbuffer|]
        Gl.DeleteFramebuffers [|framebuffer|]
        Texture.DestroyTexture shadowTexture

    /// Create a geometry buffers.
    let TryCreateGeometryBuffers () =

        // create frame buffer object
        let framebuffer = Gl.GenFramebuffer ()
        Gl.BindFramebuffer (FramebufferTarget.Framebuffer, framebuffer)
        Hl.Assert ()

        // create position buffer
        let positionId = Gl.GenTexture ()
        Gl.BindTexture (TextureTarget.Texture2d, positionId)
        Gl.TexImage2D (TextureTarget.Texture2d, 0, InternalFormat.Rgba32f, Constants.Render.ResolutionX, Constants.Render.ResolutionY, 0, PixelFormat.Rgba, PixelType.Float, nativeint 0)
        Gl.TexParameter (TextureTarget.Texture2d, TextureParameterName.TextureMinFilter, int TextureMinFilter.Nearest)
        Gl.TexParameter (TextureTarget.Texture2d, TextureParameterName.TextureMagFilter, int TextureMagFilter.Nearest)
        Gl.TexParameter (TextureTarget.Texture2d, TextureParameterName.TextureWrapS, int TextureWrapMode.ClampToEdge)
        Gl.TexParameter (TextureTarget.Texture2d, TextureParameterName.TextureWrapT, int TextureWrapMode.ClampToEdge)
        Gl.FramebufferTexture2D (FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2d, positionId, 0)
        Hl.Assert ()

        // create albedo buffer
        let albedoId = Gl.GenTexture ()
        Gl.BindTexture (TextureTarget.Texture2d, albedoId)
        Gl.TexImage2D (TextureTarget.Texture2d, 0, InternalFormat.Rgba32f, Constants.Render.ResolutionX, Constants.Render.ResolutionY, 0, PixelFormat.Rgba, PixelType.Float, nativeint 0)
        Gl.TexParameter (TextureTarget.Texture2d, TextureParameterName.TextureMinFilter, int TextureMinFilter.Nearest)
        Gl.TexParameter (TextureTarget.Texture2d, TextureParameterName.TextureMagFilter, int TextureMagFilter.Nearest)
        Gl.TexParameter (TextureTarget.Texture2d, TextureParameterName.TextureWrapS, int TextureWrapMode.ClampToEdge)
        Gl.TexParameter (TextureTarget.Texture2d, TextureParameterName.TextureWrapT, int TextureWrapMode.ClampToEdge)
        Gl.FramebufferTexture2D (FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment1, TextureTarget.Texture2d, albedoId, 0)
        Hl.Assert ()

        // create material buffer
        let materialId = Gl.GenTexture ()
        Gl.BindTexture (TextureTarget.Texture2d, materialId)
        Gl.TexImage2D (TextureTarget.Texture2d, 0, InternalFormat.Rgba32f, Constants.Render.ResolutionX, Constants.Render.ResolutionY, 0, PixelFormat.Rgba, PixelType.Float, nativeint 0)
        Gl.TexParameter (TextureTarget.Texture2d, TextureParameterName.TextureMinFilter, int TextureMinFilter.Nearest)
        Gl.TexParameter (TextureTarget.Texture2d, TextureParameterName.TextureMagFilter, int TextureMagFilter.Nearest)
        Gl.TexParameter (TextureTarget.Texture2d, TextureParameterName.TextureWrapS, int TextureWrapMode.ClampToEdge)
        Gl.TexParameter (TextureTarget.Texture2d, TextureParameterName.TextureWrapT, int TextureWrapMode.ClampToEdge)
        Gl.FramebufferTexture2D (FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment2, TextureTarget.Texture2d, materialId, 0)
        Hl.Assert ()

        // create normal plus buffer
        let normalPlusId = Gl.GenTexture ()
        Gl.BindTexture (TextureTarget.Texture2d, normalPlusId)
        Gl.TexImage2D (TextureTarget.Texture2d, 0, InternalFormat.Rgba32f, Constants.Render.ResolutionX, Constants.Render.ResolutionY, 0, PixelFormat.Rgba, PixelType.Float, nativeint 0)
        Gl.TexParameter (TextureTarget.Texture2d, TextureParameterName.TextureMinFilter, int TextureMinFilter.Nearest)
        Gl.TexParameter (TextureTarget.Texture2d, TextureParameterName.TextureMagFilter, int TextureMagFilter.Nearest)
        Gl.TexParameter (TextureTarget.Texture2d, TextureParameterName.TextureWrapS, int TextureWrapMode.ClampToEdge)
        Gl.TexParameter (TextureTarget.Texture2d, TextureParameterName.TextureWrapT, int TextureWrapMode.ClampToEdge)
        Gl.FramebufferTexture2D (FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment3, TextureTarget.Texture2d, normalPlusId, 0)
        Hl.Assert ()

        // associate draw buffers
        Gl.DrawBuffers
            [|int FramebufferAttachment.ColorAttachment0
              int FramebufferAttachment.ColorAttachment1
              int FramebufferAttachment.ColorAttachment2
              int FramebufferAttachment.ColorAttachment3|]
        Hl.Assert ()

        // create render buffer with depth and stencil
        let renderbuffer = Gl.GenRenderbuffer ()
        Gl.BindRenderbuffer (RenderbufferTarget.Renderbuffer, renderbuffer)
        Gl.RenderbufferStorage (RenderbufferTarget.Renderbuffer, InternalFormat.Depth24Stencil8, Constants.Render.ResolutionX, Constants.Render.ResolutionY)
        Gl.FramebufferRenderbuffer (FramebufferTarget.Framebuffer, FramebufferAttachment.DepthStencilAttachment, RenderbufferTarget.Renderbuffer, renderbuffer)
        Hl.Assert ()

        // ensure framebuffer is complete
        if Gl.CheckFramebufferStatus FramebufferTarget.Framebuffer = FramebufferStatus.FramebufferComplete then
            let position = Texture.CreateTextureFromId positionId
            let albedo = Texture.CreateTextureFromId albedoId
            let material = Texture.CreateTextureFromId materialId
            let normalPlus = Texture.CreateTextureFromId normalPlusId
            Right (position, albedo, material, normalPlus, renderbuffer, framebuffer)
        else Left "Could not create complete geometry framebuffer."

    /// Destroy geometry buffers.
    let DestroyGeometryBuffers (position : Texture.Texture, albedo : Texture.Texture, material : Texture.Texture, normalPlus : Texture.Texture, renderbuffer, framebuffer) =
        Gl.DeleteRenderbuffers [|renderbuffer|]
        Gl.DeleteFramebuffers [|framebuffer|]
        Texture.DestroyTexture position
        Texture.DestroyTexture albedo
        Texture.DestroyTexture material
        Texture.DestroyTexture normalPlus

    /// Create light mapping buffers.
    let TryCreateLightMappingBuffers () =

        // create frame buffer object
        let framebuffer = Gl.GenFramebuffer ()
        Gl.BindFramebuffer (FramebufferTarget.Framebuffer, framebuffer)
        Hl.Assert ()

        // create light mapping buffer
        let lightMappingId = Gl.GenTexture ()
        Gl.BindTexture (TextureTarget.Texture2d, lightMappingId)
        Gl.TexImage2D (TextureTarget.Texture2d, 0, InternalFormat.Rgba32f, Constants.Render.ResolutionX, Constants.Render.ResolutionY, 0, PixelFormat.Rgba, PixelType.Float, nativeint 0)
        Gl.TexParameter (TextureTarget.Texture2d, TextureParameterName.TextureMinFilter, int TextureMinFilter.Nearest)
        Gl.TexParameter (TextureTarget.Texture2d, TextureParameterName.TextureMagFilter, int TextureMagFilter.Nearest)
        Gl.TexParameter (TextureTarget.Texture2d, TextureParameterName.TextureWrapS, int TextureWrapMode.ClampToEdge)
        Gl.TexParameter (TextureTarget.Texture2d, TextureParameterName.TextureWrapT, int TextureWrapMode.ClampToEdge)
        Gl.FramebufferTexture2D (FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2d, lightMappingId, 0)
        Hl.Assert ()

        // associate draw buffers
        Gl.DrawBuffers [|int FramebufferAttachment.ColorAttachment0|]
        Hl.Assert ()

        // create render buffer with depth and stencil
        let renderbuffer = Gl.GenRenderbuffer ()
        Gl.BindRenderbuffer (RenderbufferTarget.Renderbuffer, renderbuffer)
        Gl.RenderbufferStorage (RenderbufferTarget.Renderbuffer, InternalFormat.Depth24Stencil8, Constants.Render.ResolutionX, Constants.Render.ResolutionY)
        Gl.FramebufferRenderbuffer (FramebufferTarget.Framebuffer, FramebufferAttachment.DepthStencilAttachment, RenderbufferTarget.Renderbuffer, renderbuffer)
        Hl.Assert ()

        // ensure framebuffer is complete
        if Gl.CheckFramebufferStatus FramebufferTarget.Framebuffer = FramebufferStatus.FramebufferComplete then
            let lightMapping = Texture.CreateTextureFromId lightMappingId
            Right (lightMapping, renderbuffer, framebuffer)
        else Left "Could not create complete light mapping framebuffer."

    /// Destroy light mapping buffers.
    let DestroyLightMappingBuffers (lightMapping : Texture.Texture, renderbuffer, framebuffer) =
        Gl.DeleteRenderbuffers [|renderbuffer|]
        Gl.DeleteFramebuffers [|framebuffer|]
        Texture.DestroyTexture lightMapping

    /// Create irradiance buffers.
    let TryCreateIrradianceBuffers () =

        // create frame buffer object
        let framebuffer = Gl.GenFramebuffer ()
        Gl.BindFramebuffer (FramebufferTarget.Framebuffer, framebuffer)
        Hl.Assert ()

        // create irradiance buffer
        let irradianceId = Gl.GenTexture ()
        Gl.BindTexture (TextureTarget.Texture2d, irradianceId)
        Gl.TexImage2D (TextureTarget.Texture2d, 0, InternalFormat.Rgba32f, Constants.Render.ResolutionX, Constants.Render.ResolutionY, 0, PixelFormat.Rgba, PixelType.Float, nativeint 0)
        Gl.TexParameter (TextureTarget.Texture2d, TextureParameterName.TextureMinFilter, int TextureMinFilter.Nearest)
        Gl.TexParameter (TextureTarget.Texture2d, TextureParameterName.TextureMagFilter, int TextureMagFilter.Nearest)
        Gl.TexParameter (TextureTarget.Texture2d, TextureParameterName.TextureWrapS, int TextureWrapMode.ClampToEdge)
        Gl.TexParameter (TextureTarget.Texture2d, TextureParameterName.TextureWrapT, int TextureWrapMode.ClampToEdge)
        Gl.FramebufferTexture2D (FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2d, irradianceId, 0)
        Hl.Assert ()

        // associate draw buffers
        Gl.DrawBuffers [|int FramebufferAttachment.ColorAttachment0|]
        Hl.Assert ()

        // create render buffer with depth and stencil
        let renderbuffer = Gl.GenRenderbuffer ()
        Gl.BindRenderbuffer (RenderbufferTarget.Renderbuffer, renderbuffer)
        Gl.RenderbufferStorage (RenderbufferTarget.Renderbuffer, InternalFormat.Depth24Stencil8, Constants.Render.ResolutionX, Constants.Render.ResolutionY)
        Gl.FramebufferRenderbuffer (FramebufferTarget.Framebuffer, FramebufferAttachment.DepthStencilAttachment, RenderbufferTarget.Renderbuffer, renderbuffer)
        Hl.Assert ()

        // ensure framebuffer is complete
        if Gl.CheckFramebufferStatus FramebufferTarget.Framebuffer = FramebufferStatus.FramebufferComplete then
            let irradiance = Texture.CreateTextureFromId irradianceId
            Right (irradiance, renderbuffer, framebuffer)
        else Left "Could not create complete irradiance framebuffer."

    /// Destroy irradiance buffers.
    let DestroyIrradianceBuffers (irradiance : Texture.Texture, renderbuffer, framebuffer) =
        Gl.DeleteRenderbuffers [|renderbuffer|]
        Gl.DeleteFramebuffers [|framebuffer|]
        Texture.DestroyTexture irradiance

    /// Create environment filter buffers.
    let TryCreateEnvironmentFilterBuffers () =

        // create frame buffer object
        let framebuffer = Gl.GenFramebuffer ()
        Gl.BindFramebuffer (FramebufferTarget.Framebuffer, framebuffer)
        Hl.Assert ()

        // create environmentFilter buffer
        let environmentFilterId = Gl.GenTexture ()
        Gl.BindTexture (TextureTarget.Texture2d, environmentFilterId)
        Gl.TexImage2D (TextureTarget.Texture2d, 0, InternalFormat.Rgba32f, Constants.Render.ResolutionX, Constants.Render.ResolutionY, 0, PixelFormat.Rgba, PixelType.Float, nativeint 0)
        Gl.TexParameter (TextureTarget.Texture2d, TextureParameterName.TextureMinFilter, int TextureMinFilter.Nearest)
        Gl.TexParameter (TextureTarget.Texture2d, TextureParameterName.TextureMagFilter, int TextureMagFilter.Nearest)
        Gl.TexParameter (TextureTarget.Texture2d, TextureParameterName.TextureWrapS, int TextureWrapMode.ClampToEdge)
        Gl.TexParameter (TextureTarget.Texture2d, TextureParameterName.TextureWrapT, int TextureWrapMode.ClampToEdge)
        Gl.FramebufferTexture2D (FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2d, environmentFilterId, 0)
        Hl.Assert ()

        // associate draw buffers
        Gl.DrawBuffers [|int FramebufferAttachment.ColorAttachment0|]
        Hl.Assert ()

        // create render buffer with depth and stencil
        let renderbuffer = Gl.GenRenderbuffer ()
        Gl.BindRenderbuffer (RenderbufferTarget.Renderbuffer, renderbuffer)
        Gl.RenderbufferStorage (RenderbufferTarget.Renderbuffer, InternalFormat.Depth24Stencil8, Constants.Render.ResolutionX, Constants.Render.ResolutionY)
        Gl.FramebufferRenderbuffer (FramebufferTarget.Framebuffer, FramebufferAttachment.DepthStencilAttachment, RenderbufferTarget.Renderbuffer, renderbuffer)
        Hl.Assert ()

        // ensure framebuffer is complete
        if Gl.CheckFramebufferStatus FramebufferTarget.Framebuffer = FramebufferStatus.FramebufferComplete then
            let environmentFilter = Texture.CreateTextureFromId environmentFilterId
            Right (environmentFilter, renderbuffer, framebuffer)
        else Left "Could not create complete environment filter framebuffer."

    /// Destroy environment filter buffers.
    let DestroyEnvironmentFilterBuffers (environmentFilter : Texture.Texture, renderbuffer, framebuffer) =
        Gl.DeleteRenderbuffers [|renderbuffer|]
        Gl.DeleteFramebuffers [|framebuffer|]
        Texture.DestroyTexture environmentFilter

    /// Create ssao buffers.
    let TryCreateSsaoBuffers () =

        // create frame buffer object
        let framebuffer = Gl.GenFramebuffer ()
        Gl.BindFramebuffer (FramebufferTarget.Framebuffer, framebuffer)
        Hl.Assert ()

        // create ssao buffer
        let ssaoId = Gl.GenTexture ()
        Gl.BindTexture (TextureTarget.Texture2d, ssaoId)
        Gl.TexImage2D (TextureTarget.Texture2d, 0, InternalFormat.R32f, Constants.Render.SsaoResolutionX, Constants.Render.SsaoResolutionY, 0, PixelFormat.Red, PixelType.Float, nativeint 0)
        Gl.TexParameter (TextureTarget.Texture2d, TextureParameterName.TextureMinFilter, int TextureMinFilter.Nearest)
        Gl.TexParameter (TextureTarget.Texture2d, TextureParameterName.TextureMagFilter, int TextureMagFilter.Nearest)
        Gl.TexParameter (TextureTarget.Texture2d, TextureParameterName.TextureWrapS, int TextureWrapMode.ClampToEdge)
        Gl.TexParameter (TextureTarget.Texture2d, TextureParameterName.TextureWrapT, int TextureWrapMode.ClampToEdge)
        Gl.FramebufferTexture2D (FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2d, ssaoId, 0)
        Hl.Assert ()

        // associate draw buffers
        Gl.DrawBuffers [|int FramebufferAttachment.ColorAttachment0|]
        Hl.Assert ()

        // create render buffer with depth and stencil
        let renderbuffer = Gl.GenRenderbuffer ()
        Gl.BindRenderbuffer (RenderbufferTarget.Renderbuffer, renderbuffer)
        Gl.RenderbufferStorage (RenderbufferTarget.Renderbuffer, InternalFormat.Depth24Stencil8, Constants.Render.SsaoResolutionX, Constants.Render.SsaoResolutionY)
        Gl.FramebufferRenderbuffer (FramebufferTarget.Framebuffer, FramebufferAttachment.DepthStencilAttachment, RenderbufferTarget.Renderbuffer, renderbuffer)
        Hl.Assert ()

        // ensure framebuffer is complete
        if Gl.CheckFramebufferStatus FramebufferTarget.Framebuffer = FramebufferStatus.FramebufferComplete then
            let ssao = Texture.CreateTextureFromId ssaoId
            Right (ssao, renderbuffer, framebuffer)
        else Left "Could not create complete ssao framebuffer."

    /// Destroy ssao buffers.
    let DestroySsaoBuffers (ssao : Texture.Texture, renderbuffer, framebuffer) =
        Gl.DeleteRenderbuffers [|renderbuffer|]
        Gl.DeleteFramebuffers [|framebuffer|]
        Texture.DestroyTexture ssao