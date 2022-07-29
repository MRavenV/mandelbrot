// For more information see https://aka.ms/fsharp-console-apps

open Mnd.Core

printfn "Mandelbrot Generator"

open Mnd.Core.Util
open Mnd.Core.Render

let fname = "out.png"
let width = 800
let height = 600
let screenSize = (width, height)

let cameraPosition = (0.0, 0.0)
let rotationAngle = 0.0
let frameHeight = 1.0
let cameraScale = frameHeight   // The same?

let maxIterations = 100
let subsampleCells = 1
let escapeRadius = 2000.0
let cameraRotation = degToRad rotationAngle


let inverseViewProj =
    inverseProject (toFloatTuple2 screenSize)
    >> inverseView cameraPosition cameraRotation cameraScale
    
let coreShader = Shaders.cycleMandelbrotSmooth escapeRadius maxIterations
let screenShader = inverseViewProj >> coreShader

let aaShader = wrapAntiAliasing subsampleCells screenShader
let s = Seq.average aaShader

let finalShader =
    toFloatTuple2
    >> Vector2.add (0.5, 0.5)
    >> aaShader

printfn "Rendering frame..."
let stopWatch = System.Diagnostics.Stopwatch.StartNew()
let bmp = render finalShader screenSize
stopWatch.Stop()
printfn "Render complete."
printfn "Elapsed time: %f seconds." stopWatch.Elapsed.TotalSeconds

printfn "Saving to: %s" fname
bmp.Save(fname)
printfn "Render saved."