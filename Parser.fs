module Parser

type Dot = int * int
type Shape =
    | Pixel of Dot
    | Line of Dot * Dot
    | Ellipse of Dot * Dot * Dot * Dot
    | Polygon of Dot list
type Opacity = Solid | Transparent
type ParserApi = string [] -> (Opacity * Shape) list