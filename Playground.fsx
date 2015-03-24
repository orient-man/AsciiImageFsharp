// http://cocoamine.net/blog/2015/03/20/replacing-photoshop-with-nsstring/
open System
open System.Drawing
open System.Windows.Forms

let mainForm = new Form(Width = 400, Height = 400, Text = "AsciiImage")
let menu = new ToolStrip()
let btnSave = new ToolStripButton("Save", Enabled = false)

ignore (menu.Items.Add(btnSave))

let boxImage = 
    new PictureBox(BackColor = Color.White, Dock = DockStyle.Fill, 
                   SizeMode = PictureBoxSizeMode.CenterImage)

mainForm.Controls.Add(menu)
mainForm.Controls.Add(boxImage)

// chevron is defined by 3 points, the angle is always 90 degrees
// 
// A 
//   # 
//     # 
//       B 
//     # 
//   # 
// C 
let drawChevron (bounds : Rectangle) = 
    let chevron = new Bitmap(bounds.Width - 10, bounds.Height - 10)
    let rightMargin = 12.0f
    let chevronHeight = 150.0f // then chevronWidth = chevronHeight/2
    let lineWidth = 10.0f
    let middle = 
        new PointF(float32 (chevron.Width) - rightMargin - lineWidth / 2.0f, 
                   float32 (chevron.Height) / 2.0f)
    let mutable top = middle
    top.X <- top.X - chevronHeight / 2.0f
    top.Y <- top.Y + chevronHeight / 2.0f
    let mutable bottom = top
    bottom.Y <- bottom.Y - chevronHeight
    use gr = Graphics.FromImage(chevron)
    gr.Clear(Color.White)
    let pen = new Pen(Color.Gray, lineWidth)
    gr.DrawLines(pen, [| top; middle; bottom |])
    chevron

let chevron = [|
    ". . . . . . . . . . ."
    ". . 1 2 . . . . . . ."
    ". . A # # . . . . . ."
    ". . . # # # . . . . ."
    ". . . . # # # . . . ."
    ". . . . . 9 # 3 . . ."
    ". . . . . 8 # 4 . . ."
    ". . . . # # # . . . ."
    ". . . # # # . . . . ."
    ". . 7 # # . . . . . ."
    ". . 6 5 . . . . . . ."
    ". . . . . . . . . . ." |]

let imageWith3Shapes = [|
    ". . . . . . . C . . E"
    ". . . 1 2 . . . . . ."
    ". . . A . . . . . . ."
    ". . . . . . . . . . D"
    "G . H . . . . . . . ."
    ". . . . . . 9 . 3 . ."
    ". . I J . . 8 . 4 . ."
    "L . . K . . . . . . ."
    ". . . . . . . . . . ."
    ". . . 7 . . . . . . ."
    ". . . 6 5 . . . . . ."
    ". . . . . . . . . . ." |]

let dotSymbols = 
    seq { 
        yield '0'
        yield! [| '1'..'9' |]
        yield! [| 'A'..'Z' |]
        yield! [| 'a'..'z' |]
    }
    |> Array.ofSeq

let indexOfSymbol (_, idx) = idx

let getOrderedDots (arr : string []) = 
    seq { 
        for y in 0..arr.Length - 1 do
            let row = arr.[y].Replace(" ", "")
            for x in 0..row.Length - 1 do
                let elem = row.[x]
                match dotSymbols |> Array.tryFindIndex (elem |> (=)) with
                | Some idx -> yield ((x, y), idx)
                | _ -> ()
    } |> Seq.sortBy indexOfSymbol |> List.ofSeq

let dotsSorted = getOrderedDots chevron

type Dot = int * int
type Figure = 
    | Shape of Dot list
    | Line of Dot * Dot
    | Pixel of Dot
    | Ellipse of Dot * Dot * Dot * Dot

let rec parseDots acc dots =
    let handleOrdered = function
        | [] -> []
        | single::[] -> [Pixel(single)]
        | many -> [Shape(many |> List.rev)]
    seq {
        match dots with
        | (d1, i1)::(d2, i2)::tail when i2 = i1 + 1 ->
            yield! parseDots (d1::acc) ((d2, i2)::tail)
        | (d1, i1)::(d2, i2)::(d3, i3)::(d4, i4)::tail when i1 = i2 && i2 = i3 && i3 = i4 ->
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

let parse rep =
    rep |> getOrderedDots |> parseDots [] |> List.ofSeq

parse chevron
parse imageWith3Shapes

boxImage.Image <- drawChevron boxImage.Bounds
[<STAThread>]
do mainForm.Show()
