module Models
open System.Drawing

type Shape = 
    | Circle of Pen * Point * int
    | Shape of (Pen * (Point [])) list
    | Line of Pen * Point * Point
    | Mix of Shape * Shape

let ticks 
    (center: Point) 
    (radius: int) 
    (ticklength: int)
    (tickwidth: int)
    (color: Color) 
    (amount: int) : Shape =
    let mutable tickLines : Shape = 
        Circle (
            new Pen (Color.FromArgb (0,0,0,0) ),
            center,
            0
            )
    let tickPen = new Pen(color, float32(tickwidth) )
    for i = 0 to amount do
        let orientation = 
            ((2.0 * System.Math.PI) / float(amount)) * float(i)
            // |> floor |> int

        let p1 = 
            Point(
                center.X + int(float(radius - ticklength) * cos orientation),
                center.Y + int(float(radius - ticklength) * sin orientation)
                )
        let p2 = 
            Point(
                center.X + int(float(radius) * cos orientation),
                center.Y + int(float(radius) * sin orientation)
                )
        tickLines <- Mix (tickLines, Line (tickPen, p1, p2))
    tickLines



let watchFace (size: Size) : Shape = 
    let HOURTICKS = 12 // Hour ticks
    let TICKS = HOURTICKS*4 // Four ticks per hour
    let HOURTICKCOLOR = Color.FromArgb (255, 0, 0, 0)
    let HOURTICKLENGTH = 20
    let HOURTICKWIDTH = 5
    let TICKCOLOR = Color.FromArgb (200, 40, 40, 40)
    let radius = 
        min size.Height size.Width 
        |> float 
        |> (*) 0.5 
        |> floor 
        |> int
    let center = Point (0 + radius,0 + radius)
    let circlePen = new Pen (Color.Yellow, 5.0f)
    
    let clockShape = 
        Mix (
            Circle ( 
                circlePen, 
                center, 
                radius 
                ), 
            ticks center radius HOURTICKLENGTH HOURTICKWIDTH HOURTICKCOLOR HOURTICKS
            )
    clockShape
