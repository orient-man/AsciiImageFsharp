module ParserTests

open NUnit.Framework
open Swensen.Unquote
open Parser

let parse = ParserImplementation.api
let parseAndStripOpacity rep = rep |> parse |> List.map snd

[<Test>]
let empty() = 
    let actual = parseAndStripOpacity Shapes.empty
    let (expected : Shape list) = []
    test <@ expected = actual @>

[<Test>]
let chevron() = 
    let actual = parseAndStripOpacity Shapes.chevron
    let expected = 
        [ Polygon([ (2, 1)
                    (3, 1)
                    (7, 5)
                    (7, 6)
                    (3, 10)
                    (2, 10)
                    (2, 9)
                    (5, 6)
                    (5, 5)
                    (2, 2) ]) ]
    test <@ expected = actual @>

[<Test>]
let ``chevron + 2 shapes``() = 
    let actual = parseAndStripOpacity Shapes.``chevron + 2 shapes``
    let expected = 
        [ Polygon([ (3, 1)
                    (4, 1)
                    (8, 5)
                    (8, 6)
                    (4, 10)
                    (3, 10)
                    (3, 9)
                    (6, 6)
                    (6, 5)
                    (3, 2) ])
          Polygon([ (7, 0)
                    (10, 3)
                    (10, 0) ])
          Polygon([ (0, 4)
                    (2, 4)
                    (2, 6)
                    (3, 6)
                    (3, 7)
                    (0, 7) ]) ]
    test <@ expected = actual @>

[<Test>]
let ``8 lines``() = 
    let actual = parseAndStripOpacity Shapes.``8 lines``
    let expected = 
        [ Line((0, 0), (11, 0))
          Line((11, 1), (11, 10))
          Line((0, 11), (11, 11))
          Line((0, 1), (0, 10))
          Line((2, 2), (9, 2))
          Line((2, 4), (9, 4))
          Line((2, 6), (9, 6))
          Line((4, 9), (7, 9)) ]
    test <@ expected = actual @>

[<Test>]
let ``lines & free shape``() = 
    let actual = parseAndStripOpacity Shapes.``6 lines & free shape``
    let expected = 
        [ Line((0, 0), (11, 0))
          Line((11, 1), (11, 10))
          Line((0, 11), (11, 11))
          Line((0, 1), (0, 10))
          Line((2, 2), (3, 2))
          Line((8, 2), (9, 2))
          Polygon([ (2, 4)
                    (4, 4)
                    (4, 6)
                    (7, 6)
                    (7, 4)
                    (9, 4)
                    (9, 9)
                    (2, 9) ]) ]
    test <@ expected = actual @>

[<Test>]
let ``ellipse + 2 pixels``() = 
    let actual = parseAndStripOpacity Shapes.``ellipse + 2 pixels``
    let expected = 
        [ Ellipse((5, 2), (1, 6), (9, 6), (5, 10))
          Pixel(1, 1)
          Pixel(9, 1) ]
    test <@ expected = actual @>

[<Test>]
let deletion() = 
    let actual = parse Shapes.deletion
    let expected = 
        [ (Solid, Ellipse((5, 0), (0, 5), (10, 5), (5, 10)))
          (Transparent, Line((3, 3), (7, 7)))
          (Transparent, Line((7, 3), (3, 7))) ]
    test <@ expected = actual @>
