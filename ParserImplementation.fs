module ParserImplementation

open Parser

let ascii2dots (arr : string []) = 
    let tryFindIndex arr symbol = arr |> Array.tryFindIndex (symbol |> (=))
    let (|Solid|_|) = tryFindIndex [| 'A'..'Z' |]
    let (|Transparent|_|) = tryFindIndex [| 'a'..'z' |]

    [ for y in 0..arr.Length - 1 do
          let row = arr.[y].Replace(" ", "")
          for x in 0..row.Length - 1 do
              match row.[x] with
              | Solid idx -> yield (((x, y), Solid), idx)
              | Transparent idx -> yield (((x, y), Transparent), idx)
              | _ -> () ]

let (|Single|_|) = function
    | ((p, op), i1)::(d2, i2)::tail when i2 = i1 + 2 -> Some(p, op, (d2, i2)::tail)
    | ((p, op), _)::[] -> Some(p, op, [])
    | _ -> None

let (|Duo|_|) = function
    | ((p1, op), i1)::((p2, _), i2)::tail when i1 = i2 -> Some((p1, p2), op, tail)
    | _ -> None

let (|Quad|_|) = function
    | ((p1, op), i1)::((p2, _), i2)::((p3, _), i3)::((p4, _), i4)::tail
        when i1 = i2 && i2 = i3 && i3 = i4 -> Some((p1, p2, p3, p4), op, tail)
    | _ -> None

let (|Sequence|_|) dots =
    let wrapResult points tail =
        match points with
        | (_, op)::_ -> Some(points |> List.map fst |> List.rev, op, tail)
        | [] -> None

    let rec collect acc = function
        | (d1, i1)::(d2, i2)::tail when i2 = i1 + 1 -> collect (d1::acc) ((d2, i2)::tail)
        | Single(p, op, tail) -> wrapResult ((p, op)::acc) tail
        | tail -> wrapResult acc tail

    collect [] dots

let (|Shape|_|) = function
    | Single(p, opacity, tail) -> Some((opacity, Pixel(p)), tail)
    | Sequence(points, opacity, tail) -> Some((opacity, Polygon(points)), tail)
    | Quad(points, opacity, tail) -> Some((opacity, Ellipse(points)), tail)
    | Duo(points, opacity, tail)-> Some((opacity, Line(points)), tail)
    | _ -> None

let rec parse dots = 
    [ match dots with Shape(shape, tail) -> yield shape; yield! parse tail | _ -> () ]

let api : ParserApi = fun rep -> rep |> ascii2dots |> List.sortBy snd |> parse