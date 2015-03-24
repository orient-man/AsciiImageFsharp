module Drawing

open System.Drawing
open Parser

let draw (scale : int) (rep : string []) = 
    let scaleF = float32 (scale)
    let width = rep.[0].Length / 2 + 1
    let scaledWidth = scale * width
    let height = rep.Length
    let scaledHeight = scale * height
    let bitmap = new Bitmap(scaledWidth, scaledHeight)

    use gr = Graphics.FromImage(bitmap)
    gr.SmoothingMode <- Drawing2D.SmoothingMode.AntiAlias
    gr.Clear(Color.White)

    let drawGrid() = 
        let grayPen = new Pen(Color.LightGray, 1.0f)
        for i in 0..height do
            gr.DrawLine(grayPen, new Point(0, scale * i), new Point(scaledWidth, scale * i))
        for i in 0..width do
            gr.DrawLine(grayPen, new Point(scale * i, 0), new Point(scale * i, scaledHeight))
        gr.DrawRectangle(grayPen, 0, 0, scaledWidth - 1, scaledHeight - 1)
    
    let drawShape shape = 
        let pen = new Pen(Color.Black, scaleF)
        let brush = new SolidBrush(Color.Black)
        
        let drawPixel p = 
            let x, y = p
            gr.FillRectangle(brush, scale * x, scale * y, scale, scale)
        
        let scalePoint (x, y) = 
            (scaleF * (0.5f + float32 (x)), scaleF * (0.5f + float32 (y)))

        let toPointF (x, y) = new PointF(x, y)

        match shape with
        | Free(points) -> 
            let scaledPoints = 
                points
                |> List.map scalePoint
                |> List.map toPointF
                |> Array.ofList
            gr.DrawPolygon(pen, scaledPoints)
            gr.FillPolygon(brush, scaledPoints)
        | Line(p1, p2) -> 
            drawPixel p1
            drawPixel p2
            gr.DrawLine(pen, p1 |> scalePoint |> toPointF, p2 |> scalePoint |> toPointF)
        | Pixel(p) -> drawPixel p
        | Ellipse(p1, p2, p3, p4) -> 
            let points = [ p1; p2; p3; p4 ]
            let minX = points |> List.map fst |> List.min
            let maxX = points |> List.map fst |> List.max
            let minY = points |> List.map snd |> List.min
            let maxY = points |> List.map snd |> List.max
            let x, y = (minX, minY) |> scalePoint
            let width, height = (maxX - minX, maxY - minY) |> scalePoint
            gr.DrawEllipse(pen, x, y, width, height)
            gr.FillEllipse(brush, x, y, width, height)
    
    if scale > 1 then drawGrid()
    rep
    |> parse
    |> List.iter drawShape
    bitmap

