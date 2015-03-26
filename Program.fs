open System.IO
open ParserImplementation
open DrawingImplementation

[<EntryPoint>]
let main = function
    | [|inputFile|] ->
        let asciiRep = File.ReadAllLines(inputFile)
        let draw = DrawingImplementation.api ParserImplementation.api
        let bitmap = draw 30 asciiRep
        bitmap.Save(inputFile.Replace(".txt", ".png"), System.Drawing.Imaging.ImageFormat.Png)
        0
    | _ ->
        printfn "Example usage: ascii2image file.txt"
        -1;