module Models
open System.Drawing
open System

type Shape = 
    | Circle of Pen * Point * int
    | Polygon of (Pen * (Point [])) 
    | Line of Pen * Point * Point
    | Mix of Shape * Shape
    | FilledCircle of Brush * Point * int
    | FilledRectangle of Brush * Point * Point
    // | Text of Pen * string * Font * Brush * 

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

let hourHand 
    (center:Point) 
    (radius:int) 
    (color: Color) 
    (width: int) : Shape =
    let pen = new Pen (color, float32(width))
    let hourToOrientation () : float = 
        // DateTime.Now.Hour
        (0.5 * System.Math.PI) - 
        (
            ( float(DateTime.Now.Hour) % 12.0 / 12.0 ) 
            * (2.0 * System.Math.PI)
        ) // This gets the orientation in rads
        
    let orientation = hourToOrientation ()
    let x,y = (cos orientation), -(sin orientation) // Y is negated since up is down in forms
    let endPoint = 
        Point( center.X + int(x*float(radius)), center.Y + int(y*float(radius)) )
    Line (pen, endPoint, center)

let minuteHand 
    (center:Point) 
    (radius:int) 
    (color: Color) 
    (width: int) : Shape =
    let pen = new Pen (color, float32(width))
    let minuteToOrientation () : float =
        ( ( 2.0 * System.Math.PI ) / 60.0 )  * ( float(DateTime.Now.Minute) )
        // This gets the orientation in rads
        
    let orientation = (0.5 * System.Math.PI) - minuteToOrientation ()
    let x,y = (cos orientation), -(sin orientation) // Y is negated since up is down in forms
    let endPoint = 
        Point (
            int(x) * radius + center.X,
            int(y) * radius + center.Y
        )
    printfn "Now i'm drawing with x:%f y:%f radius: %i, center %A" x y radius center
    Line(pen, center, endPoint)
    // printfn "Drawing a line of p1: %A p2: %A" center endPoint
    // Line (pen, center, endPoint)

let decoration (size:Size) (center:Point) (radius:int) =
    /// Constants for decoration
    let HOURTICKS = 12 // Hour ticks
    let TICKS = HOURTICKS*5 // Four ticks per hour
    let HOURTICKCOLOR = Color.FromArgb (255, 0, 0, 0)
    let HOURTICKLENGTH = 20
    let HOURTICKWIDTH = 5
    let TICKLENGTH = 12
    let TICKWIDTH = 2
    let TICKCOLOR = Color.FromArgb (200, 40, 40, 40)

    /// Assembling logic
    // Tick marks
    let hourTickMarks = 
        ticks center radius HOURTICKLENGTH HOURTICKWIDTH 
            HOURTICKCOLOR HOURTICKS
    let minuteTickMarks = 
        ticks center radius TICKLENGTH TICKWIDTH
            TICKCOLOR TICKS 
    let mutable decoration = Mix(hourTickMarks, minuteTickMarks)

    let centerFill = 
        let brush = new SolidBrush (Color.FromArgb (255,70,75,65))
        FilledCircle(brush, center, radius / 15)
    decoration <- Mix (decoration, centerFill)
    decoration


let watchFace (size: Size) : Shape = 
    let HOURHANDCOLOR = Color.FromArgb (255, 0, 0, 0)
    let HOURHANDWIDTH = 6
    let MINUTEHANDCOLOR = HOURHANDCOLOR
    let MINUTEHANDWIDTH = 4
    let radius = 
        min size.Height size.Width 
        |> float 
        |> (*) 0.5 
        |> floor 
        |> int
    let center = Point (0 + radius,0 + radius)
    let circlePen = new Pen (Color.Yellow, 5.0f)
    
    let clockShape = 
        Circle ( 
            circlePen, 
            center, 
            radius 
            )
    let hands = 
        Mix( 
            hourHand center (radius/2) HOURHANDCOLOR HOURHANDWIDTH,
            minuteHand center radius MINUTEHANDCOLOR MINUTEHANDWIDTH
        )
    let decoration = decoration size center radius
    Mix (Mix( clockShape, hands), decoration)

