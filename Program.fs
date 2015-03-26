open System
open System.IO
open ParserImplementation
open DrawingImplementation

let ascii2image inputFile scale =
    let asciiRep = File.ReadAllLines(inputFile)
    let draw = DrawingImplementation.api ParserImplementation.api
    let bitmap = draw scale asciiRep
    bitmap.Save(inputFile.Replace(".txt", ".png"), System.Drawing.Imaging.ImageFormat.Png)

[<EntryPoint>]
let main = function
    | [|inputFile|] -> ascii2image inputFile 1; 0
    | [|inputFile; scale|] -> ascii2image inputFile (Int32.Parse(scale)); 0
    | _ -> printfn "Example usage: ascii2image file.txt"; -1