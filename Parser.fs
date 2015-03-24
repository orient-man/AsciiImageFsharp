module Parser

type Dot = int * int
type Shape = 
    | Free of Dot list
    | Line of Dot * Dot
    | Pixel of Dot
    | Ellipse of Dot * Dot * Dot * Dot

let private symbolsOrdered = 
    seq { 
        yield '0'
        yield! [| '1'..'9' |]
        yield! [| 'A'..'Z' |]
        yield! [| 'a'..'z' |]
    }
    |> Array.ofSeq

let getOrderedDots (arr : string []) = 
    let indexOfSymbol (_, idx) = idx
    seq { 
        for y in 0..arr.Length - 1 do
            let row = arr.[y].Replace(" ", "")
            for x in 0..row.Length - 1 do
                let elem = row.[x]
                match symbolsOrdered |> Array.tryFindIndex (elem |> (=)) with
                | Some idx -> yield ((x, y), idx)
                | _ -> ()
    } |> Seq.sortBy indexOfSymbol |> List.ofSeq

let rec private parseDots acc dots =
    let handleOrdered = function
        | [] -> []
        | single::[] -> [Pixel(single)]
        | many -> [Free(many |> List.rev)]
    seq {
        match dots with
        | (d1, i1)::(d2, i2)::tail when i2 = i1 + 1 ->
            yield! parseDots (d1::acc) ((d2, i2)::tail)
        | (d1, i1)::(d2, i2)::(d3, i3)::(d4, i4)::tail
            when i1 = i2 && i2 = i3 && i3 = i4 ->
            yield! handleOrdered acc
            yield Ellipse(d1, d2, d3, d4)
            yield! parseDots [] tail
        | (d1, i1)::(d2, i2)::tail when i1 = i2 ->
            yield! handleOrdered acc
            yield Line(d1, d2)
            yield! parseDots [] tail
        | (d, _)::tail ->
            yield! handleOrdered (d::acc)
            yield! parseDots [] tail
        | _ -> yield! handleOrdered acc
    }

let parse rep = rep |> getOrderedDots |> parseDots [] |> List.ofSeq

