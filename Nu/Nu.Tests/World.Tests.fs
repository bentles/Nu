﻿// Nu Game Engine.
// Copyright (C) Bryan Edds, 2013-2023.

namespace Nu.Tests
open System
open NUnit.Framework
open Prime
open Nu
module WorldTests =

    do Nu.init ()
    Constants.Engine.OctnodeSize <- 128.0f // NOTE: reducing construction cost of octrees to speed up world construction for unit test.
    Constants.Engine.OctreeDepth <- 3

    let [<Test>] runOneEmptyFrameThenCleanUp () =
        let world = World.makeEmpty { WorldConfig.defaultConfig with Accompanied = true } (TestPlugin ())
        let result = World.runWithCleanUp (fun world -> world.UpdateTime < 1L) id id id id id Live true world
        Assert.Equal (result, Constants.Engine.ExitCodeSuccess)

    let [<Test; Category "Integration">] runOneIntegrationFrameThenCleanUp () =
        let worldConfig = { WorldConfig.defaultConfig with Accompanied = true }
        match SdlDeps.tryMake worldConfig.SdlConfig with
        | Right sdlDeps ->
            use sdlDeps = sdlDeps // bind explicitly to dispose automatically
            match World.tryMake sdlDeps worldConfig (TestPlugin ()) with
            | Right world ->
                let result = World.runWithCleanUp (fun world -> world.UpdateTime < 1L) id id id id id Live true world
                Assert.Equal (result, Constants.Engine.ExitCodeSuccess)
            | Left _ -> Assert.Fail ()
        | Left _ -> Assert.Fail ()