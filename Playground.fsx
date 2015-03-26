// http://cocoamine.net/blog/2015/03/20/replacing-photoshop-with-nsstring/
open System
open System.Drawing
open System.Windows.Forms

#load "Shapes.fs"
#load "Parser.fs"
#load "ParserImplementation.fs"
#load "Drawing.fs"
#load "DrawingImplementation.fs"

let mainForm = new Form(Width = 400, Height = 400, Text = "AsciiImage")
let boxImage = 
    new PictureBox(BackColor = Color.White, Dock = DockStyle.Fill, 
                   SizeMode = PictureBoxSizeMode.CenterImage)

mainForm.Controls.Add(boxImage)

let draw = DrawingImplementation.api ParserImplementation.api
let image = Shapes.``DevTalk.pl`` |> draw 30
//let image = Shapes.``chevron + 2 shapes`` |> draw 30
//let image = Shapes.``8 lines`` |> draw 30
//let image = Shapes.``ellipse + 2 pixels`` |> draw 30
//let image = Shapes.``6 lines & free shape`` |> draw 30

boxImage.Image <- image
[<STAThread>]
do mainForm.Show()
