// http://cocoamine.net/blog/2015/03/20/replacing-photoshop-with-nsstring/
open System
open System.Drawing
open System.Windows.Forms

#load "Parser.fs"
#load "Shapes.fs"
open Parser

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

let draw (bounds : Rectangle) (shapes : Shape list) = 
    let margin = 10
    let scale = 20.0f
    let lineWidth = scale * 1.0f

    let bitmap = new Bitmap(bounds.Width - margin, bounds.Height - margin)
    use gr = Graphics.FromImage(bitmap)
    gr.Clear(Color.White)
    let pen = new Pen(Color.Black, lineWidth)
    let brush = new SolidBrush(Color.Black);

    let drawShape (shape : Shape) =
        let toPointF (x, y) = new PointF(scale * float32(x), scale * float32(y))
        match shape with
        | Free(points) -> gr.FillPolygon(brush, points |> List.map toPointF |> Array.ofList)
        | Line(p1, p2) -> gr.DrawLine(pen, toPointF p1, toPointF p2)
        | Pixel(p) -> ()
        | Ellipse(p1, p2, p3, p4) -> ()

    shapes |> List.iter drawShape
    bitmap

//let dotsSorted = getOrderedDots chevron
//parse chevron
//parse imageWith3Shapes

boxImage.Image <- draw boxImage.Bounds (Shapes.``6 lines & free shape`` |> parse)

[<STAThread>]
do mainForm.Show()
