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
    let (|Solid|_|) symbol = tryFindIndex  [| 'A'..'Z' |] symbol
    let (|Transparent|_|) symbol = tryFindIndex [| 'a'..'z' |] symbol

    seq { 
        for y in 0..arr.Length - 1 do
            let row = arr.[y].Replace(" ", "")
            for x in 0..row.Length - 1 do
                match row.[x] with
                | Solid idx  -> yield (((x, y), Solid), idx)
                | Transparent idx -> yield (((x, y), Transparent), idx)
                | _ -> ()
    } |> Seq.sortBy snd |> List.ofSeq

let rec private parseDots acc dots =
    let handleOrdered points =
        match points with
        | [] -> []
        | (p, opacity)::[] -> [opacity (Pixel(p))]
        | (_, opacity)::_ -> [opacity (Polygon(points |> List.map fst |> List.rev))]

    seq {
        match dots with
        | (d1, i1)::(d2, i2)::tail when i2 = i1 + 1 ->
            yield! parseDots (d1::acc) ((d2, i2)::tail)
        | ((p1, opacity), i1)::(d2, i2)::(d3, i3)::(d4, i4)::tail
            when i1 = i2 && i2 = i3 && i3 = i4 ->
            yield! handleOrdered acc
            yield opacity (Ellipse(p1, fst d2, fst d3, fst d4))
            yield! parseDots [] tail
        | ((p1, opacity), i1)::(d2, i2)::tail when i1 = i2 ->
            yield! handleOrdered acc
            yield opacity (Line(p1, fst d2))
            yield! parseDots [] tail
        | (d, _)::tail ->
            yield! handleOrdered (d::acc)
            yield! parseDots [] tail
        | _ -> yield! handleOrdered acc
    }

let parse rep = rep |> getOrderedDots |> parseDots [] |> List.ofSeq

