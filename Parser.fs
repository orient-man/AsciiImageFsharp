module Parser

type Dot = int * int
type Shape =
    | Polygon of Dot list
    | Line of Dot * Dot
    | Pixel of Dot
    | Ellipse of Dot * Dot * Dot * Dot
type FilledShape = Solid of Shape | Transparent of Shape

let getOrderedDots (arr : string []) = 
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
    |> List.sortBy snd

let (|Ordered|_|) dots =
    let rec collect acc = function
        | (d1, i1)::(d2, i2)::tail when i2 = i1 + 1 -> collect (d1::acc) ((d2, i2)::tail)
        | (_, i1)::(_, i2)::_ when i2 = i1 -> [], []
        | (d, _)::tail -> (d::acc), tail
        | [] -> acc, []

    let points, tail = collect [] dots
    match points with
    | (_, opacity)::_ -> Some(points |> List.map fst |> List.rev, opacity, tail)
    | [] -> None

let (|Shape|_|) = function
    | Ordered(p::[], opacity, tail) -> Some(opacity (Pixel(p)), tail)
    | Ordered(points, opacity, tail) -> Some(opacity (Polygon(points)), tail)
    | ((p1, opacity), i1)::(d2, i2)::(d3, i3)::(d4, i4)::tail
        when i1 = i2 && i2 = i3 && i3 = i4 ->
        Some(opacity (Ellipse(p1, fst d2, fst d3, fst d4)), tail)
    | ((p1, opacity), i1)::(d2, i2)::tail when i1 = i2 ->
      Some(opacity (Line(p1, fst d2)), tail)
    | _ -> None

let rec parseDots dots = 
    [ match dots with Shape(shape, tail) -> yield shape; yield! parseDots tail | _ -> () ]

let parse rep = rep |> getOrderedDots |> parseDots