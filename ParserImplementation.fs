﻿module ParserImplementation

open Parser

let ascii2dots (arr : string []) = 
    let (|InRange|_|) first last = function
        | c when c >= first && c <= last -> Some(int(c) - int(first))
        | _ -> None

    [ for y in 0..arr.Length - 1 do
          let row = arr.[y].Replace(" ", "")
          for x in 0..row.Length - 1 do
              match row.[x] with
              | InRange 'A' 'Z' idx -> yield (((x, y), Solid), idx)
              | InRange 'a' 'z' idx -> yield (((x, y), Transparent), idx)
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

let rec parse dots = 
    [ match dots with
      | Single(p, op, tail) -> yield op, Pixel(p); yield! parse tail
      | Sequence(points, op, tail) -> yield op, Polygon(points); yield! parse tail
      | Quad(points, op, tail) -> yield op, Ellipse(points); yield! parse tail
      | Duo(points, op, tail)-> yield op, Line(points); yield! parse tail
      | _ -> () ]

let api : ParserApi = fun rep -> rep |> ascii2dots |> List.sortBy snd |> parse