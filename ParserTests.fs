module ParserTests

open NUnit.Framework
open Swensen.Unquote
open Parser

[<Test>]
let empty() = 
    let actual = Shapes.empty |> parse
    let (expected : FilledShape list) = []
    test <@ expected = actual @>

[<Test>]
let chevron() = 
    let actual = Shapes.chevron |> parse
    
    let expected = 
        [ Solid(Polygon([ (2, 1)
                          (3, 1)
                          (7, 5)
                          (7, 6)
                          (3, 10)
                          (2, 10)
                          (2, 9)
                          (5, 6)
                          (5, 5)
                          (2, 2) ])) ]
    test <@ expected = actual @>

[<Test>]
let ``chevron + 2 shapes``() = 
    let actual = Shapes.``chevron + 2 shapes`` |> parse
    
    let expected = 
        [ Solid(Polygon([ (3, 1)
                          (4, 1)
                          (8, 5)
                          (8, 6)
                          (4, 10)
                          (3, 10)
                          (3, 9)
                          (6, 6)
                          (6, 5)
                          (3, 2) ]))
          Solid(Polygon([ (7, 0)
                          (10, 3)
                          (10, 0) ]))
          Solid(Polygon([ (0, 4)
                          (2, 4)
                          (2, 6)
                          (3, 6)
                          (3, 7)
                          (0, 7) ])) ]
    test <@ expected = actual @>

[<Test>]
let ``8 lines``() = 
    let actual = Shapes.``8 lines`` |> parse
    
    let expected = 
        [ Solid(Line((0, 0), (11, 0)))
          Solid(Line((11, 1), (11, 10)))
          Solid(Line((0, 11), (11, 11)))
          Solid(Line((0, 1), (0, 10)))
          Solid(Line((2, 2), (9, 2)))
          Solid(Line((2, 4), (9, 4)))
          Solid(Line((2, 6), (9, 6)))
          Solid(Line((4, 9), (7, 9))) ]
    test <@ expected = actual @>

[<Test>]
let ``lines & free shape``() = 
    let actual = Shapes.``6 lines & free shape`` |> parse
    
    let expected = 
        [ Solid(Line((0, 0), (11, 0)))
          Solid(Line((11, 1), (11, 10)))
          Solid(Line((0, 11), (11, 11)))
          Solid(Line((0, 1), (0, 10)))
          Solid(Line((2, 2), (3, 2)))
          Solid(Line((8, 2), (9, 2)))
          Solid(Polygon([ (2, 4)
                          (4, 4)
                          (4, 6)
                          (7, 6)
                          (7, 4)
                          (9, 4)
                          (9, 9)
                          (2, 9) ])) ]
    test <@ expected = actual @>

[<Test>]
let ``ellipse + 2 pixels``() = 
    let actual = Shapes.``ellipse + 2 pixels`` |> parse
    
    let expected = 
        [ Solid(Ellipse((5, 2), (1, 6), (9, 6), (5, 10)))
          Solid(Pixel(1, 1))
          Solid(Pixel(9, 1)) ]
    test <@ expected = actual @>
