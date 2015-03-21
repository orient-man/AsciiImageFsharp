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

let asciiRep = [|
    " . . . . . . . . . . ."
    " . . 1 2 . . . . . . ."
    " . . A # # . . . . . ."
    " . . . # # # . . . . ."
    " . . . . # # # . . . ."
    " . . . . . 9 # 3 . . ."
    " . . . . . 8 # 4 . . ."
    " . . . . # # # . . . ."
    " . . . # # # . . . . ."
    " . . 7 # # . . . . . ."
    " . . 6 5 . . . . . . ."
    " . . . . . . . . . . ." |]

let dotSymbols =
    seq {
        yield '0'
        yield! [|'1'..'9'|]
        yield! [|'A'..'Z'|]
        yield! [|'a'..'z'|]
    } |> Array.ofSeq

type Dot = int * int
type AsciiImage =
    | Shape of Dot list
    | Line of Dot * Dot
    | Pixel of Dot
    | Ellipse of Dot * Dot * Dot * Dot

boxImage.Image <- drawChevron boxImage.Bounds
[<STAThread>]
do mainForm.Show()
