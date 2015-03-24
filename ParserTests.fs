module ParserTests

open NUnit.Framework
open Swensen.Unquote
open Parser

[<Test>]
let empty() =
    let actual = Shapes.empty |> parse
    let (expected:Shape list) = []

    test <@ expected = actual @>

[<Test>]
let chevron() =
    let actual = Shapes.chevron |> parse
    let expected =
        [ Polygon [ (2, 1)
                    (3, 1)
                    (7, 5)
                    (7, 6)
                    (3, 10)
                    (2, 10)
                    (2, 9)
                    (5, 6)
                    (5, 5)
                    (2, 2) ] ]

    test <@ expected = actual @>

[<Test>]
let ``chevron + 2 shapes``() =
    let actual = Shapes.``chevron + 2 shapes`` |> parse
    let expected = 
        [ Polygon [ (3, 1)
                    (4, 1)
                    (8, 5)
                    (8, 6)
                    (4, 10)
                    (3, 10)
                    (3, 9)
                    (6, 6)
                    (6, 5)
                    (3, 2) ]
          Polygon [ (7, 0)
                    (10, 3)
                    (10, 0) ]
          Polygon [ (0, 4)
                    (2, 4)
                    (2, 6)
                    (3, 6)
                    (3, 7)
                    (0, 7) ] ]

    test <@ expected = actual @>

[<Test>]
let ``8 lines``() =
    let actual = Shapes.``8 lines`` |> parse
    let expected = 
        [ Line (( 0,  0), (11,  0))
          Line ((11,  1), (11, 10))
          Line (( 0, 11), (11, 11))
          Line (( 0,  1), ( 0, 10))
          Line (( 2,  2), ( 9,  2))
          Line (( 2,  4), ( 9,  4))
          Line (( 2,  6), ( 9,  6))
          Line (( 4,  9), ( 7,  9)) ]

    test <@ expected = actual @>

[<Test>]
let ``lines & free shape``() =
    let actual = Shapes.``6 lines & free shape`` |> parse
    let expected = 
        [ Line (( 0,  0), (11,  0))
          Line ((11,  1), (11, 10))
          Line (( 0, 11), (11, 11))
          Line (( 0,  1), ( 0, 10))
          Line (( 2,  2), ( 3,  2))
          Line (( 8,  2), ( 9,  2))
          Polygon [(2, 4); (4, 4); (4, 6); (7, 6); (7, 4); (9, 4); (9, 9); (2, 9)] ]

    test <@ expected = actual @>

[<Test>]
let ``ellipse + 2 pixels``() =
    let actual = Shapes.``ellipse + 2 pixels`` |> parse
    let expected = [Ellipse ((5, 2),(1, 6),(9, 6),(5, 10)); Pixel (1, 1); Pixel (9, 1)]

    test <@ expected = actual @>
